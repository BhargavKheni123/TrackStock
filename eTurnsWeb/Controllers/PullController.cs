using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsWeb.Helper;
using eTurnsWeb.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace eTurnsWeb.Controllers
{
    [AuthorizeHelper]
    public partial class PullController : eTurnsControllerBase
    {
        private PullMasterDAL pullMasterDAL { get; set; }
        private CommonDAL commonDAL { get; set; }
        string enterPriseDBName;

        #region Constructor
        public PullController()
        {
            enterPriseDBName = SessionHelper.EnterPriseDBName;
            pullMasterDAL = new PullMasterDAL(this.enterPriseDBName);
            commonDAL = new CommonDAL(this.enterPriseDBName);
        }

        #endregion

        #region Destuctor

        bool disposed = false;

        // Protected implementation of Dispose pattern.
        protected override void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                pullMasterDAL.Dispose();
                commonDAL.Dispose();
                // Free any other managed objects here.
                //
            }

            // Free any unmanaged objects here.
            //

            disposed = true;
            // Call base class implementation.
            base.Dispose(disposing);
        }

        ~PullController()
        {

        }

        #endregion


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PullNew()
        {
            ItemMasterDAL objPullNew = new ItemMasterDAL(this.enterPriseDBName);
            int TotalRecordCount = 0;
            bool IsAllowConsignedCredit = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowConsignedCreditPull, eTurnsWeb.Helper.SessionHelper.PermissionType.AllowPull);
            Session["ConsignedAllowed"] = IsAllowConsignedCredit;
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            Session["PullItemList"] = objPullNew.GetPulledItemsForModel(0,
                                                            1000,
                                                            out TotalRecordCount,
                                                            string.Empty,
                                                            "Id desc",
                                                            SessionHelper.RoomID,
                                                            SessionHelper.CompanyID,
                                                            false,
                                                            false,
                                                            SessionHelper.UserSupplierIds,
                                                            true, IsAllowConsignedCredit, true, SessionHelper.UserID, "pull", Convert.ToString(SessionHelper.RoomDateFormat), CurrentTimeZone, true, null, null, Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.NumberDecimalDigits)
                                                            ).ToList();
            //new RoomDAL(SessionHelper.EnterPriseDBName).GetRoomByIDPlain(SessionHelper.RoomID)
            CommonDAL objCommonDAL = this.commonDAL;//new CommonDAL(SessionHelper.EnterPriseDBName);
            string columnList = "ID,RoomName,IsProjectSpendMandatory";
            RoomDTO ROOMDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");

            ItemModelPerameter obj = new ItemModelPerameter()
            {
                AjaxURLAddItemToSession = "~/QuickList/AddItemToSession/",
                ModelHeader = eTurns.DTO.ResQuickList.ModelHeader,
                AjaxURLAddMultipleItemToSession = "~/QuickList/AddItemToSessionMultiple/",
                AjaxURLToFillItemGrid = "~/Pull/GetItemsModelMethod/",
                IsProjectSpendMandatoryInRoom = ROOMDTO.IsProjectSpendMandatory,

            };

            return View("_NewPull", obj);
        }

        public ActionResult SetFilterSession(string FilterValue, Guid? mntsGUID)
        {
            Session["PullItemList"] = null; ;
            if (FilterValue == "1" && mntsGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
            {
                ItemMasterDAL objPullNew = new ItemMasterDAL(this.enterPriseDBName);
                string itemGUIDs = string.Empty;

                List<ToolsSchedulerDetailsDTO> lstitems = new List<ToolsSchedulerDetailsDTO>();

                bool IsAllowConsignedCredit = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowConsignedCreditPull, eTurnsWeb.Helper.SessionHelper.PermissionType.AllowPull);

                string Itempopupfor = "maint";

                ToolsMaintenanceDAL objToolsMaintenanceDAL = new ToolsMaintenanceDAL(this.enterPriseDBName);
                lstitems = objToolsMaintenanceDAL.GetSchedulerItems(mntsGUID ?? Guid.Empty);
                if (lstitems != null)
                {
                    itemGUIDs = string.Join(",", lstitems.Select(t => t.ItemGUID).Where(t => t != null).ToArray());
                }

                int TotalRecordCount = 0;
                TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
                Session["PullItemList"] = objPullNew.GetPulledItemsForModel(0,
                                                              int.MaxValue,
                                                              out TotalRecordCount,
                                                              string.Empty,
                                                              "Id desc",
                                                              SessionHelper.RoomID,
                                                              SessionHelper.CompanyID,
                                                              false,
                                                              false,
                                                              SessionHelper.UserSupplierIds,
                                                              true, IsAllowConsignedCredit, true, SessionHelper.UserID, Itempopupfor, Convert.ToString(SessionHelper.RoomDateFormat), CurrentTimeZone,
                                                              true, null, itemGUIDs, Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.NumberDecimalDigits)
                                                              ).ToList();
            }
            return Json(new { flag = "true" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ClearSession()
        {
            Session["PullItemList"] = null;
            return Json(true);
        }


        public ActionResult PullMasterList()
        {
            List<string> lstKeys = new List<string>();
            foreach (string key in Session.Keys)
            {
                //"PullMasterUDFOptions"
                if (key.ToLower().Contains("pullmasterudfoptions"))
                    lstKeys.Add(key);
            }

            foreach (string item in lstKeys)
            {
                Session[item] = null;
            }

            Session["ConsignedAllowed"] = SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowConsignedCreditPull);
            Session["PullList"] = null;
            return View();
        }

        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        public ActionResult PullCreate()
        {
            PullMasterViewDTO objDTO = new PullMasterViewDTO()
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
                ItemMasterView = new ItemMasterDTO(),
            };

            UDFController objUDFController = new UDFController();
            ViewBag.UDFs = objUDFController.GetUDFDataPageWise("PullMaster");

            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }

            ItemMasterDAL objItemMasterApi = new ItemMasterDAL(this.enterPriseDBName);
            ViewBag.ItemMaster = objItemMasterApi.GetAllItemsPlain(SessionHelper.RoomID, SessionHelper.CompanyID);
            ProjectMasterDAL objProjectApi = new ProjectMasterDAL(this.enterPriseDBName);
            ViewBag.ProjectSpent = objProjectApi.GetAllProjectMasterByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            return PartialView("_CreatePull", objDTO);
        }

        public ActionResult NewPull()
        {
            //XElement Settinfile = XElement.Load(Server.MapPath("/SiteSettings.xml"));
            int iUDFMaxLength = 200;
            //int.TryParse(Convert.ToString(Settinfile.Element("UDFMaxLength").Value), out iUDFMaxLength);
            int.TryParse(Convert.ToString(SiteSettingHelper.UDFMaxLength), out iUDFMaxLength);
            ItemMasterDAL objPullNew = new ItemMasterDAL(this.enterPriseDBName);
            int TotalRecordCount = 0;
            bool IsAllowConsignedCredit = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowConsignedCreditPull, eTurnsWeb.Helper.SessionHelper.PermissionType.AllowPull);
            Session["ConsignedAllowed"] = IsAllowConsignedCredit;
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            Session["PullItemList"] = objPullNew.GetPulledItemsForModel(0,
                                                            int.MaxValue,
                                                            out TotalRecordCount,
                                                            string.Empty,
                                                            "Id desc",
                                                            SessionHelper.RoomID,
                                                            SessionHelper.CompanyID,
                                                            false,
                                                            false,
                                                            SessionHelper.UserSupplierIds,
                                                            true, IsAllowConsignedCredit, true, SessionHelper.UserID, "pull", Convert.ToString(SessionHelper.RoomDateFormat), CurrentTimeZone, true, null, null, Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.NumberDecimalDigits)
                                                            ).ToList();

            CommonDAL objCommonDAL = this.commonDAL; //new CommonDAL(this.enterPriseDBName);
            string columnList = "ID,RoomName,IsProjectSpendMandatory,AllowPullBeyondAvailableQty";
            RoomDTO ROOMDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");

            ItemModelPerameter obj = new ItemModelPerameter()
            {
                AjaxURLAddItemToSession = "~/QuickList/AddItemToSession/",
                ModelHeader = eTurns.DTO.ResQuickList.ModelHeader,
                AjaxURLAddMultipleItemToSession = "~/QuickList/AddItemToSessionMultiple/",
                AjaxURLToFillItemGrid = "~/Pull/GetItemsModelMethod/",
                IsProjectSpendMandatoryInRoom = ROOMDTO.IsProjectSpendMandatory,
                AllowPullBeyondAvailableQty = ROOMDTO.AllowPullBeyondAvailableQty
            };
            UDFDAL objUDFDAL = new UDFDAL(this.enterPriseDBName);
            UDFDTO objUDFDTO = new UDFDTO();
            objUDFDTO = objUDFDAL.GetPullPORecord(SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, iUDFMaxLength);
            ViewBag.ControlType = objUDFDTO.UDFControlType;


            ViewBag.SuppPOPair = GetSupplierPOPairList();

            //return PartialView("_NewPull", obj);
            return View("NewConsumePull", obj);
        }


        public ActionResult GetItemsModelMethod(QuickListJQueryDataTableParamModel param)
        {
            ItemMasterDAL obj = new ItemMasterDAL(this.enterPriseDBName);
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
            List<ToolsSchedulerDetailsDTO> lstitems = new List<ToolsSchedulerDetailsDTO>();
            bool IsArchived = false; //bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = false; //bool.Parse(Request["IsDeleted"].ToString());
            Guid MaintenaceGUID = Guid.Empty;
            BinMasterDTO objBin = new BinMasterDTO();
            BinMasterDAL objBinMasterDAL = new BinMasterDAL(this.enterPriseDBName);
            string firsttimepop = string.Empty;
            string itemGUIDs = string.Empty;
            string itemsFilter = "all";
            ProjectMasterDAL objDAL = new ProjectMasterDAL(this.enterPriseDBName);
            var totalProjectCount = objDAL.GetTotalProjectCountByRoom(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
            bool isdefaultPs = false;
            string DefaultProjectSpend = string.Empty;
            Guid DefaultPSGuid = Guid.Empty;

            if (Request["FirstTimePop"] != null)
            {
                firsttimepop = Convert.ToString(Convert.ToString(Request["FirstTimePop"]));
            }
            if (Request["mntsGUID"] != null)
            {
                Guid.TryParse(Convert.ToString(Request["mntsGUID"]), out MaintenaceGUID);
            }
            if (Request["sItemsFilter"] != null)
            {
                itemsFilter = Convert.ToString(Request["sItemsFilter"]);
            }

            if (itemsFilter == "1" && MaintenaceGUID != Guid.Empty)
            {
                ToolsMaintenanceDAL objToolsMaintenanceDAL = new ToolsMaintenanceDAL(this.enterPriseDBName);
                lstitems = objToolsMaintenanceDAL.GetSchedulerItems(MaintenaceGUID);
                if (lstitems != null)
                {
                    itemGUIDs = string.Join(",", lstitems.Select(t => t.ItemGUID).Where(t => t != null).ToArray());
                }

                ViewBag.FirstTimePopup = "yes";
            }
            string ActionFilter = Request["ActionFilter"].ToString();

            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined")
                    || sortColumnName.Contains("sDispImg") || sortColumnName.Contains("txtQty")
                    || sortColumnName.Contains("txtProjectSpentCol") || sortColumnName.Contains("StagingHeaderName"))
                    sortColumnName = "ID desc";
            }
            else
                sortColumnName = "ID desc";

            if (sortColumnName.Trim().ToLower().Contains("itemudf1"))
                sortColumnName = sortColumnName.Replace("ItemUDF1", "UDF1");
            else if (sortColumnName.Trim().ToLower().Contains("itemudf2"))
                sortColumnName = sortColumnName.Replace("ItemUDF2", "UDF2");
            else if (sortColumnName.Trim().ToLower().Contains("itemudf3"))
                sortColumnName = sortColumnName.Replace("ItemUDF3", "UDF3");
            else if (sortColumnName.Trim().ToLower().Contains("itemudf4"))
                sortColumnName = sortColumnName.Replace("ItemUDF4", "UDF4");
            else if (sortColumnName.Trim().ToLower().Contains("itemudf5"))
                sortColumnName = sortColumnName.Replace("ItemUDF5", "UDF5");
            else if (sortColumnName.Trim().ToLower().Contains("itemudf6"))
                sortColumnName = sortColumnName.Replace("ItemUDF6", "UDF6");
            else if (sortColumnName.Trim().ToLower().Contains("itemudf7"))
                sortColumnName = sortColumnName.Replace("ItemUDF7", "UDF7");
            else if (sortColumnName.Trim().ToLower().Contains("itemudf8"))
                sortColumnName = sortColumnName.Replace("ItemUDF8", "UDF8");
            else if (sortColumnName.Trim().ToLower().Contains("itemudf9"))
                sortColumnName = sortColumnName.Replace("ItemUDF9", "UDF9");
            else if (sortColumnName.Trim().ToLower().Contains("itemudf10"))
                sortColumnName = sortColumnName.Replace("ItemUDF10", "UDF10");

            string searchQuery = string.Empty;
            int TotalRecordCount = 0;
            List<ItemMasterDTO> DataFromDB = new List<ItemMasterDTO>();

            #region Pull

            if (ActionFilter == "Pull")
            {
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

                    QuickListDAL objQLDtlDAL = new QuickListDAL(this.enterPriseDBName);
                    List<QuickListDetailDTO> objQLDtlDTO = objQLDtlDAL.GetQuickListItemsRecords(SessionHelper.RoomID, SessionHelper.CompanyID, param.sSearch.Replace("QLGuid=", "").ToString(), SessionHelper.UserSupplierIds).Where(x => x.IsDeleted == false).ToList();
                    ItemMasterDAL objItemDAL = new ItemMasterDAL(this.enterPriseDBName);

                    if (objQLDtlDTO.Count > 0)
                    {
                        foreach (QuickListDetailDTO qlItem in objQLDtlDTO)
                        {
                            qlItem.Quantity = qlItem.Quantity * QLQty;
                            ItemMasterDTO tempItemDTO = new ItemMasterDTO();

                            if (SessionHelper.UserSupplierIds != null && SessionHelper.UserSupplierIds.Any())
                            {
                                tempItemDTO = objItemDAL.GetItemWithMasterTableJoins(null, (qlItem.ItemGUID ?? Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                                if (!SessionHelper.UserSupplierIds.Contains(tempItemDTO.SupplierID.GetValueOrDefault(0)))
                                {
                                    tempItemDTO = null;
                                }
                            }
                            else
                            {
                                tempItemDTO = objItemDAL.GetItemWithMasterTableJoins(null, (qlItem.ItemGUID ?? Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                            }
                            tempItemDTO.BinNumber = qlItem.BinName;
                            tempItemDTO.BinID = qlItem.BinID;
                            tempItemDTO.DefaultLocationName = qlItem.DefaultLocationName;
                            if (ActionFilter == "Pull" && tempItemDTO != null && tempItemDTO.ID > 0)
                            {
                                tempItemDTO.DefaultPullQuantity = qlItem.Quantity;
                                DataFromDB.Add(tempItemDTO);
                            }
                        }

                        TotalRecordCount = DataFromDB.Count();

                        if (TotalRecordCount > param.iDisplayLength)
                        {
                            DataFromDB = DataFromDB.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                        }
                    }
                }
                else
                {
                    string qtyFormat = "N";

                    if (!string.IsNullOrEmpty(SessionHelper.NumberDecimalDigits))
                        qtyFormat = "N" + SessionHelper.NumberDecimalDigits;
                    string Itempopupfor = "pull";

                    if (MaintenaceGUID != Guid.Empty && itemsFilter == "1")
                    {
                        Itempopupfor = "maint";
                    }

                    bool IsAllowConsignedCredit = SessionHelper.GetModulePermission(SessionHelper.ModuleList.AllowConsignedCreditPull, SessionHelper.PermissionType.AllowPull);
                    TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
                    DataFromDB = obj.GetPulledItemsForModel(param.iDisplayStart,
                                                            param.iDisplayLength,
                                                            out TotalRecordCount,
                                                            param.sSearch,
                                                            sortColumnName,
                                                            SessionHelper.RoomID,
                                                            SessionHelper.CompanyID,
                                                            IsArchived,
                                                            IsDeleted,
                                                            SessionHelper.UserSupplierIds,
                                                            true, IsAllowConsignedCredit,
                                                            true, SessionHelper.UserID,
                                                            Itempopupfor,
                                                            Convert.ToString(SessionHelper.RoomDateFormat), CurrentTimeZone, true,
                                                            null, itemGUIDs,
                                                            Convert.ToInt32(SessionHelper.NumberDecimalDigits)
                                                            ).ToList();

                    bool isAssignDefaultProjectSpend = true;

                    Action FnAssignDefaultProjectSpend = () =>
                    {
                        if (isAssignDefaultProjectSpend)
                        {
                            DataFromDB.ToList().ForEach(t =>
                            {
                                t.IsDefaultProjectSpend = isdefaultPs;
                                t.DefaultProjectSpend = DefaultProjectSpend;
                                t.DefaultProjectSpendGuid = DefaultPSGuid;
                            });
                            isAssignDefaultProjectSpend = false;
                        }
                    };


                    if (totalProjectCount > 0)
                    {
                        var projectTrackAllUsageAgainstThis = objDAL.GetDefaultProjectSpendRecord(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

                        if (projectTrackAllUsageAgainstThis != null && projectTrackAllUsageAgainstThis.GUID != Guid.Empty)
                        {
                            isdefaultPs = true;
                            DefaultProjectSpend = projectTrackAllUsageAgainstThis.ProjectSpendName;
                            DefaultPSGuid = projectTrackAllUsageAgainstThis.GUID;

                        }
                        else
                        {
                            FnAssignDefaultProjectSpend();
                            //DataFromDB.ToList().ForEach(t =>
                            //{
                            //    t.IsDefaultProjectSpend = isdefaultPs;
                            //    t.DefaultProjectSpend = DefaultProjectSpend;
                            //    t.DefaultProjectSpendGuid = DefaultPSGuid;
                            //});
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(param.sSearch) && param.sSearch.Contains("[###]"))
                    {
                        string[] stringSeparators = new string[] { "[###]" };
                        string[] Fields = param.sSearch.Split(stringSeparators, StringSplitOptions.None);
                        string[] FieldsPara = Fields[1].Split('@');

                        string StagingBinNumbers = null;
                        if (!string.IsNullOrWhiteSpace(FieldsPara[43]))
                        {
                            StagingBinNumbers = FieldsPara[43].TrimEnd(',');
                        }

                        string StagingHeaderGuid = null;
                        if (!string.IsNullOrWhiteSpace(FieldsPara[26]))
                        {
                            StagingHeaderGuid = FieldsPara[26].TrimEnd(',');
                        }

                        if (!string.IsNullOrWhiteSpace(StagingBinNumbers) && !StagingBinNumbers.Contains(","))
                        {
                            MaterialStagingDetailDAL oMSD = new MaterialStagingDetailDAL(this.enterPriseDBName);
                            //List<MaterialStagingDetailDTO> oMSDList = oMSD.GetAllRecords(StagingBinNumbers, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
                            List<MaterialStagingDetailDTO> oMSDList = oMSD.GetMaterialStagingDetailByStagingBinName(StagingBinNumbers, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();

                            oMSDList = (from x in oMSDList
                                        group x by new { x.ItemGUID, x.StagingBinID, x.StagingBinName } into groupedX
                                        select new MaterialStagingDetailDTO
                                        {
                                            ItemGUID = groupedX.Key.ItemGUID,
                                            StagingBinID = groupedX.Key.StagingBinID,
                                            StagingBinName = groupedX.Key.StagingBinName,
                                            Quantity = groupedX.Sum(y => y.Quantity),
                                        }).ToList();

                            DataFromDB.ToList().ForEach(t =>
                            {
                                MaterialStagingDetailDTO objMSD = oMSDList.Where(x => x.ItemGUID == t.GUID).FirstOrDefault();
                                if (objMSD != null)
                                {
                                    t.DefaultLocation = objMSD.StagingBinID;
                                    t.DefaultPullQuantity = objMSD.Quantity;
                                    t.DefaultLocationName = objMSD.StagingBinName.Replace("[|EmptyStagingBin|]", string.Empty) + " " + string.Format(ResPullMaster.Staging, objMSD.Quantity);
                                    t.IsDefaultProjectSpend = isdefaultPs;
                                    t.DefaultProjectSpend = DefaultProjectSpend;
                                    t.DefaultProjectSpendGuid = DefaultPSGuid;

                                }
                            });
                        }
                        else
                        {
                            FnAssignDefaultProjectSpend();
                            //DataFromDB.ToList().ForEach(t =>
                            //{
                            //    t.IsDefaultProjectSpend = isdefaultPs;
                            //    t.DefaultProjectSpend = DefaultProjectSpend;
                            //    t.DefaultProjectSpendGuid = DefaultPSGuid;
                            //});
                        }
                    }
                    else
                    {
                        FnAssignDefaultProjectSpend();
                        //DataFromDB.ToList().ForEach(t =>
                        //{
                        //    t.IsDefaultProjectSpend = isdefaultPs;
                        //    t.DefaultProjectSpend = DefaultProjectSpend;
                        //    t.DefaultProjectSpendGuid = DefaultPSGuid;
                        //});
                    }
                }
            }
            #endregion

            else
            {
                //PullMasterDAL pullMasterDAL = new PullMasterDAL(this.enterPriseDBName);
                List<Guid> arrItemGuids;
                if (ActionFilter == "CreditMS")
                {
                    arrItemGuids = pullMasterDAL.GetItemGuidsByPullActionType(SessionHelper.RoomID, SessionHelper.CompanyID, "ms pull");
                }
                else
                {
                    arrItemGuids = pullMasterDAL.GetItemGuidsByPullActionType(SessionHelper.RoomID, SessionHelper.CompanyID, "pull");
                }

                Session["PullItemList"] = null;

                if (param.sSearch != null && param.sSearch.Contains("QLGuid="))
                {
                    QuickListDAL objQLDtlDAL = new QuickListDAL(this.enterPriseDBName);
                    List<QuickListDetailDTO> objQLDtlDTO = objQLDtlDAL.GetQuickListItemsRecords(SessionHelper.RoomID, SessionHelper.CompanyID, param.sSearch.Replace("QLGuid=", "").ToString(), SessionHelper.UserSupplierIds).Where(x => x.IsDeleted == false).ToList();
                    ItemMasterDAL objItemDAL = new ItemMasterDAL(this.enterPriseDBName);
                    int Count = 0;
                    if (objQLDtlDTO.Count > 0)
                    {

                        foreach (QuickListDetailDTO qlItem in objQLDtlDTO)
                        {
                            ItemMasterDTO tempItemDTO = new ItemMasterDTO();

                            if (SessionHelper.UserSupplierIds != null && SessionHelper.UserSupplierIds.Any())
                            {
                                tempItemDTO = objItemDAL.GetItemWithMasterTableJoins(null, (qlItem.ItemGUID ?? Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);

                                if (!SessionHelper.UserSupplierIds.Contains(tempItemDTO.SupplierID.GetValueOrDefault(0)))
                                {
                                    tempItemDTO = null;
                                }
                            }
                            else
                            {
                                tempItemDTO = objItemDAL.GetItemWithMasterTableJoins(null, (qlItem.ItemGUID ?? Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                            }
                            tempItemDTO.BinNumber = qlItem.BinName;
                            tempItemDTO.BinID = qlItem.BinID;
                            tempItemDTO.DefaultLocationName = qlItem.DefaultLocationName;
                            if (tempItemDTO != null && tempItemDTO.ID > 0 && arrItemGuids.Contains(tempItemDTO.GUID))
                            {
                                tempItemDTO.DefaultPullQuantity = qlItem.Quantity;
                                tempItemDTO.QLCreditQuantity = qlItem.Quantity;
                                Count++;
                                DataFromDB.Add(tempItemDTO);
                            }
                        }
                        TotalRecordCount = Count;
                    }

                }
                else // if (includeItemGuid != null && includeItemGuid.Length > 0)
                {
                    bool IsAllowConsignedCredit = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowConsignedCreditPull, eTurnsWeb.Helper.SessionHelper.PermissionType.AllowPull);
                    string ActionType = "credit";

                    if (ActionFilter == "CreditMS")
                    {
                        ActionType = "credit ms";
                    }
                    else
                    {
                        ActionType = "credit";
                    }

                    // RoomDTO objRoomDTO = new RoomDAL(this.enterPriseDBName).GetRoomByIDPlain(SessionHelper.RoomID);
                    CommonDAL objCommonDAL = this.commonDAL; //new CommonDAL(this.enterPriseDBName);
                    string columnList = "ID,RoomName,IsIgnoreCreditRule";
                    RoomDTO objRoomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");

                    bool applayIsIgnoreCreditRule = false;
                    if (objRoomDTO != null)
                    {
                        applayIsIgnoreCreditRule = objRoomDTO.IsIgnoreCreditRule;

                    }
                    if (!applayIsIgnoreCreditRule)
                    {
                        TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
                        DataFromDB = obj.GetPagedItemsForCreditModel(param.iDisplayStart,
                                                                  param.iDisplayLength,
                                                                  out TotalRecordCount,
                                                                  param.sSearch,
                                                                  sortColumnName,
                                                                  SessionHelper.RoomID,
                                                                  SessionHelper.CompanyID,
                                                                  IsArchived,
                                                                  IsDeleted,
                                                                  SessionHelper.UserSupplierIds,
                                                                  true, true, true, SessionHelper.UserID, ActionType, Convert.ToString(SessionHelper.RoomDateFormat), CurrentTimeZone, true, null, null
                                                                  ).ToList();
                    }
                    else
                    {
                        TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
                        DataFromDB = obj.GetPagedItemsForCreditPageForCreditRule(param.iDisplayStart,
                                                                  param.iDisplayLength,
                                                                  out TotalRecordCount,
                                                                  param.sSearch,
                                                                  sortColumnName,
                                                                  SessionHelper.RoomID,
                                                                  SessionHelper.CompanyID,
                                                                  IsArchived,
                                                                  IsDeleted,
                                                                  SessionHelper.UserSupplierIds,
                                                                  true, true, true, SessionHelper.UserID, Convert.ToString(SessionHelper.RoomDateFormat), CurrentTimeZone, true, null, null
                                                                  ).ToList();
                    }

                }

                DataFromDB.ToList().ForEach(t =>
                {
                    t.IsDefaultProjectSpend = isdefaultPs;
                    t.DefaultProjectSpend = DefaultProjectSpend;
                    t.DefaultProjectSpendGuid = DefaultPSGuid;
                });

                if (ActionFilter == "CreditMS")
                {
                    DataFromDB.ToList().ForEach(t =>
                        {
                            string StagingBinNumbers = null;
                            if (!string.IsNullOrWhiteSpace(t.DefaultLocationName))
                            {
                                StagingBinNumbers = t.DefaultLocationName;

                                MaterialStagingDetailDAL oMSD = new MaterialStagingDetailDAL(this.enterPriseDBName);
                                //List<MaterialStagingDetailDTO> oMSDList = oMSD.GetAllRecordsWithoutCaching(StagingBinNumbers, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
                                List<MaterialStagingDetailDTO> oMSDList = oMSD.GetMaterialStagingDetailByRoomIDCompanyID(SessionHelper.RoomID, SessionHelper.CompanyID).ToList();

                                oMSDList = (from x in oMSDList
                                            group x by new { x.ItemGUID, x.StagingBinID, x.StagingBinName } into groupedX
                                            select new MaterialStagingDetailDTO
                                            {
                                                ItemGUID = groupedX.Key.ItemGUID,
                                                StagingBinID = groupedX.Key.StagingBinID,
                                                StagingBinName = groupedX.Key.StagingBinName,
                                                Quantity = groupedX.Sum(y => y.Quantity),
                                            }).ToList();


                                MaterialStagingDetailDTO objMSD = oMSDList.Where(x => x.ItemGUID == t.GUID).FirstOrDefault();
                                if (objMSD != null)
                                {
                                    t.DefaultLocation = objMSD.StagingBinID;
                                    t.DefaultPullQuantity = objMSD.Quantity;
                                    t.DefaultLocationName = objMSD.StagingBinName.Replace("[|EmptyStagingBin|]", string.Empty); // +" [Staging](" + objMSD.Quantity + ")";
                                    t.IsDefaultProjectSpend = isdefaultPs;
                                    t.DefaultProjectSpend = DefaultProjectSpend;
                                    t.DefaultProjectSpendGuid = DefaultPSGuid;
                                }
                                else
                                {
                                    t.DefaultLocation = null;
                                    t.DefaultLocationName = string.Empty;
                                }
                            }
                        });
                }
                else
                {
                    DataFromDB.ToList().ForEach(t =>
                    {
                        t.IsDefaultProjectSpend = isdefaultPs;
                        t.DefaultProjectSpend = DefaultProjectSpend;
                        t.DefaultProjectSpendGuid = DefaultPSGuid;
                    });
                }
            }
            if (MaintenaceGUID != Guid.Empty)
            {
                ToolsMaintenanceDAL objToolsMaintenanceDAL1 = new ToolsMaintenanceDAL(this.enterPriseDBName);
                List<PullOnMaintenanceDTO> lstItems = objToolsMaintenanceDAL1.GetPullOnMaintenance(MaintenaceGUID);
                DataFromDB.ToList().ForEach(t =>
                {
                    if (itemsFilter == "1" && lstitems != null && lstitems.Count() > 0)
                    {
                        objBin = new BinMasterDTO();
                        ToolsSchedulerDetailsDTO objToolsSchedulerDetailsDTO = lstitems.FirstOrDefault(f => f.ItemGUID == t.GUID);
                        if ((t.DefaultLocation ?? 0) > 0)
                        {
                            objBin = objBinMasterDAL.GetBinByID(t.DefaultLocation ?? 0, SessionHelper.RoomID, SessionHelper.CompanyID);
                        }

                        if (objToolsSchedulerDetailsDTO != null)
                        {

                            t.DefaultPullQuantity = objToolsSchedulerDetailsDTO.Quantity;
                            t.DefaultLocationName = objBin.BinNumber;
                            //t.DefaultLocationName =                         
                        }
                    }
                    if (lstItems.Any(m => m.ItemGUID == t.GUID))
                    {
                        t.QuanityPulled = lstItems.FirstOrDefault(g => g.ItemGUID == t.GUID).QuanityPulled;
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

        //public ActionResult NewConsumePull()
        //{
        //    ItemModelPerameter obj = new ItemModelPerameter()
        //    {
        //        AjaxURLAddItemToSession = "~/QuickList/AddItemToSession/",
        //        ModelHeader = eTurns.DTO.ResQuickList.ModelHeader,
        //        AjaxURLAddMultipleItemToSession = "~/QuickList/AddItemToSessionMultiple/",
        //        AjaxURLToFillItemGrid = "~/Pull/GetItemsModelMethod/",
        //        IsProjectSpendMandatoryInRoom = new RoomDAL(this.enterPriseDBName).GetRecord(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).IsProjectSpendMandatory,

        //    };
        //    return View(obj);
        //}


        /// <summary>
        /// JSON Record Save - Enter key Save/Update
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Technician"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult PullSave(PullMasterViewDTO objDTO)
        {
            if (!ModelState.IsValid)
                return Json(new { Message = ResMessage.InvalidModel, Status = "fail" }, JsonRequestBehavior.AllowGet);

            string message = "";
            string status = "";

            PullMasterDAL obj = this.pullMasterDAL; //new PullMasterDAL(this.enterPriseDBName);
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.Room = SessionHelper.RoomID;

            if (objDTO.ID == 0)
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
            else
            {
                bool ReturnVal = obj.Edit(objDTO);
                if (ReturnVal)
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
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadItemMasterModel(string ParentId)
        {
            Int64 RoomID = SessionHelper.RoomID;
            Int64 CompanyID = SessionHelper.CompanyID;
            ItemModelPerameter obj = new ItemModelPerameter()
            {
                AjaxURLAddItemToSession = "~/QuickList/AddItemToSession/",
                PerentID = ParentId,
                ModelHeader = eTurns.DTO.ResQuickList.ModelHeader,
                AjaxURLAddMultipleItemToSession = "~/QuickList/AddItemToSessionMultiple/",
                AjaxURLToFillItemGrid = "~/Pull/GetItemsModelMethod/",
                CallingFromPageName = "NewPULL",
            };

            var regionalSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
            var regionInfo = regionalSettingDAL.GetRegionSettingsById(RoomID, CompanyID, -1);
            ViewBag.WeightDP = regionInfo.WeightDecimalPoints;

            return PartialView("ItemMasterModel", obj);
        }

        public string PullDetails(Guid ItemID)
        {
            ViewBag.PullGUID = ItemID;
            PullDetailsDAL objPullDetails = new PullDetailsDAL(this.enterPriseDBName);
            var objModel = new List<PullDetailsDTO>();
            //Note: below line is commnted because the partial view is having model => IEnumerable<eTurns.DTO.PullDetailsDTO>, but that model is not used in the partview view
            //var objModel = objPullDetails.GetPullDetailsByPullGuidPlain(ItemID, SessionHelper.RoomID, SessionHelper.CompanyID).OrderByDescending(e => e.ID).ToList();
            return RenderRazorViewToString("_PullDetails", objModel);
        }

        public ActionResult PullDetailsListAjax(JQueryDataTableParamModel param)
        {
            Guid PullGUID = Guid.Parse(Request["ItemID"].ToString());

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
            if (sortColumnName == "0" || sortColumnName == "undefined" || sortColumnName == "ShippingMethod" || sortColumnName == "PULLID")
                sortColumnName = "ID";

            if (sortDirection == "asc")
                sortColumnName = sortColumnName + " asc";
            else
                sortColumnName = sortColumnName + " desc";

            ///////////// requried when paging needs in this method /////////////////

            ViewBag.PullGUID = PullGUID;
            int TotalRecordCount = 0;
            List<PullDetailsDTO> objModel;

            using (PullDetailsDAL objPullDetails = new PullDetailsDAL(this.enterPriseDBName))
            {
                objModel = objPullDetails.GetPagedPullDetails(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, PullGUID);
            }

            //objModel.ToList().ForEach(t =>
            //{
            //    t.CreatedDate = CommonUtility.ConvertDateByTimeZone(t.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
            //    t.UpdatedDate = CommonUtility.ConvertDateByTimeZone(t.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
            //});
            objModel.ForEach(t =>
                {
                    t.BinName = t.BinName == "[|EmptyStagingBin|]" ? string.Empty : t.BinName;
                    t.ReceivedOnDate = CommonUtility.ConvertDateByTimeZone(t.ReceivedOn, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                    t.ReceivedOnDateWeb = CommonUtility.ConvertDateByTimeZone(t.ReceivedOnWeb, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                    t.Received = (!string.IsNullOrWhiteSpace(t.Received)) ? FnCommon.ConvertDateByTimeZone(t.ReceivedOn, true, true) : string.Empty;                    
                });
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = objModel
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

        #region Ajax Data Provider
        public ActionResult PullMasterListAjax(JQueryDataTableParamModel param)
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

            sortColumnName = (sortColumnName ?? string.Empty).Replace("_LabelView", "");
            string searchQuery = string.Empty;
            int TotalRecordCount = 0;
            bool UserConsignmentAllowed = SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowConsignedCreditPull);
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            List<PullMasterListViewDTO> DataFromDB = null;
            PullMasterDAL obj = this.pullMasterDAL; //new PullMasterDAL(this.enterPriseDBName))

            DataFromDB = obj.GetPagedPullRecordsView(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, SessionHelper.UserSupplierIds, UserConsignmentAllowed, RoomDateFormat, CurrentTimeZone);


            //  var DataFromDB = obj.GetPagedPullRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, SessionHelper.UserSupplierIds, UserConsignmentAllowed);

            bool UDF1Encrypt = false;
            bool UDF2Encrypt = false;
            bool UDF3Encrypt = false;
            bool UDF4Encrypt = false;
            bool UDF5Encrypt = false;
            IEnumerable<UDFDTO> DataFromDBUDF;
            int TotalRecordCountUDF = 0;

            using (UDFDAL objUdf = new UDFDAL(this.enterPriseDBName))
            {
                DataFromDBUDF = objUdf.GetPagedUDFsByUDFTableNamePlain(0, 10, out TotalRecordCountUDF, "Id asc", SessionHelper.CompanyID, "PullMaster", SessionHelper.RoomID);
            }

            foreach (UDFDTO u in DataFromDBUDF)
            {
                if (u.UDFColumnName == "UDF1")
                {
                    UDF1Encrypt = u.IsEncryption ?? false;
                }
                if (u.UDFColumnName == "UDF2")
                {
                    UDF2Encrypt = u.IsEncryption ?? false;
                }
                if (u.UDFColumnName == "UDF3")
                {
                    UDF3Encrypt = u.IsEncryption ?? false;
                }
                if (u.UDFColumnName == "UDF4")
                {
                    UDF4Encrypt = u.IsEncryption ?? false;
                }
                if (u.UDFColumnName == "UDF5")
                {
                    UDF5Encrypt = u.IsEncryption ?? false;
                }
            }

            //DataFromDB.ToList().ForEach(t =>
            for (int i = 0; i < DataFromDB.Count; i++)
            {
                var t = DataFromDB[i];

                t.AverageUsage_LabelView = RegionHelper.FormatNumbers(t.AverageUsage, 4);
                t.CriticalQuantity_LabelView = (t.IsItemLevelMinMaxQtyRequired ?? false) ? RegionHelper.FormatNumbers(t.CriticalQuantity, 2) : "N/A";
                t.DefaultPullQuantity_LabelView = RegionHelper.FormatNumbers(t.DefaultPullQuantity, 2);
                t.InTransitquantity_LabelView = RegionHelper.FormatNumbers(t.InTransitquantity, 2);
                t.ItemCost_LabelView = RegionHelper.FormatNumbers(t.ItemCost, 1);
                t.ItemLocationOnHandQty_LabelView = RegionHelper.FormatNumbers(t.ItemLocationOnHandQty, 2);
                t.ItemOnhandQty_LabelView = RegionHelper.FormatNumbers(t.ItemLocationOnHandQty, 2);
                t.MinimumQuantity_LabelView = (t.IsItemLevelMinMaxQtyRequired ?? false) ? RegionHelper.FormatNumbers(t.MinimumQuantity, 2) : "N/A";
                t.OnHandQuantity_LabelView = RegionHelper.FormatNumbers(t.OnHandQuantity, 2);
                t.OnOrderQuantity_LabelView = RegionHelper.FormatNumbers(t.OnOrderQuantity, 2);
                t.OnTransferQuantity_LabelView = RegionHelper.FormatNumbers(t.OnTransferQuantity, 2);
                t.PoolQuantity_LabelView = RegionHelper.FormatNumbers(t.PoolQuantity, 2);
                t.PullCost_LabelView = RegionHelper.FormatNumbers(t.PullCost, 1);
                t.PullPrice_LabelView = RegionHelper.FormatNumbers(t.PullPrice, 1);
                t.SellPrice_LabelView = RegionHelper.FormatNumbers(t.SellPrice, 1);
                t.Turns_LabelView = RegionHelper.FormatNumbers(t.Turns, 5);
                t.MaximumQuantity_LabelView = (t.IsItemLevelMinMaxQtyRequired ?? false) ? RegionHelper.FormatNumbers(t.MaximumQuantity, 2) : "N/A";
                t.Taxable_LabelView = RegionHelper.FormatBoolean(t.Taxable);
                t.ItemType_LabelView = RegionHelper.FormatItemType(t.ItemType);
                t.IsCustomerEDISent_LabelView = RegionHelper.FormatBoolean(t.IsCustomerEDISent);


                t.BinNumber = t.BinNumber == "[|EmptyStagingBin|]" ? string.Empty : t.BinNumber;
                if (UDF1Encrypt || UDF2Encrypt || UDF3Encrypt || UDF4Encrypt || UDF5Encrypt)
                {
                    CommonDAL objCommon = this.commonDAL; //new CommonDAL(this.enterPriseDBName);
                    t.UDF1 = UDF1Encrypt == true ? objCommon.GetDecryptValue(t.UDF1) : t.UDF1;
                    t.UDF2 = UDF2Encrypt == true ? objCommon.GetDecryptValue(t.UDF2) : t.UDF2;
                    t.UDF3 = UDF3Encrypt == true ? objCommon.GetDecryptValue(t.UDF3) : t.UDF3;
                    t.UDF4 = UDF4Encrypt == true ? objCommon.GetDecryptValue(t.UDF4) : t.UDF4;
                    t.UDF5 = UDF5Encrypt == true ? objCommon.GetDecryptValue(t.UDF5) : t.UDF5;
                }
            }
            //);


            //if (UDF1Encrypt || UDF2Encrypt || UDF3Encrypt || UDF4Encrypt || UDF5Encrypt)
            //{
            //    CommonDAL objCommon = new CommonDAL(this.enterPriseDBName);
            //    DataFromDB.ToList().ForEach(t =>
            //    {
            //        //t.RoomName = SessionHelper.RoomName;
            //        t.UDF1 = UDF1Encrypt == true ? objCommon.GetDecryptValue(t.UDF1) : t.UDF1;
            //        t.UDF2 = UDF2Encrypt == true ? objCommon.GetDecryptValue(t.UDF2) : t.UDF2;
            //        t.UDF3 = UDF3Encrypt == true ? objCommon.GetDecryptValue(t.UDF3) : t.UDF3;
            //        t.UDF4 = UDF4Encrypt == true ? objCommon.GetDecryptValue(t.UDF4) : t.UDF4;
            //        t.UDF5 = UDF5Encrypt == true ? objCommon.GetDecryptValue(t.UDF5) : t.UDF5;
            //        t.BinNumber = t.BinNumber == "[|EmptyStagingBin|]" ? string.Empty : t.BinNumber;
            //        //t.UpdatedDate = CommonUtility.ConvertDateByTimeZone(t.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
            //    });
            //}



            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);

        }

        public JsonResult RequisitionPullAll(List<RequisitionItemsToPull> objItemsToPull)
        {
            List<ReqPullAllJsonResponse> lstErrorMessages = new List<ReqPullAllJsonResponse>();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            if (objItemsToPull != null && objItemsToPull.Count > 0)
            {
                foreach (var item in objItemsToPull)
                {
                    ReqPullAllJsonResponse errMsg = new ReqPullAllJsonResponse();
                    JsonResult repsonse = null;
                    if (item.ItemGUID != "" && Guid.Parse(item.ItemGUID) != Guid.Empty)
                    {
                        repsonse = UpdatePullData(item.ID, item.ItemGUID, item.ProjectGUID, item.PullCreditQuantity, item.BinID, item.PullCredit, item.TempPullQTY, item.UDF1, item.UDF2, item.UDF3, item.UDF4, item.UDF5, item.RequisitionDetailGUID, item.WorkOrderDetailGUID, item.ICDtlGUID, item.ProjectSpendName, item.PullOrderNumber, item.SupplierAccountGuid);
                    }
                    else
                    {
                        repsonse = RequisitionToolCheckout(item);
                    }
                    errMsg = serializer.Deserialize<ReqPullAllJsonResponse>(serializer.Serialize(repsonse.Data));
                    errMsg.ReqDetailGuid = item.RequisitionDetailGUID;

                    if (!string.IsNullOrEmpty(item.ItemNumber))
                    {
                        errMsg.ItemNumber = item.ItemNumber;
                    }
                    else
                    {
                        ToolMasterDAL toolDAL = new ToolMasterDAL(this.enterPriseDBName);
                        ToolMasterDTO toolDTO = toolDAL.GetToolByGUIDPlain(item.ToolGUID.GetValueOrDefault(Guid.Empty));
                        errMsg.ItemNumber = toolDTO.ToolName;
                        if (errMsg.Message == "ok")
                        {
                            errMsg.Message = " " + ResToolMaster.MsgCheckoutDoneSuccess; 
                        }
                    }
                    lstErrorMessages.Add(errMsg);

                }

                ConsumeController objcntrl = new ConsumeController();
                objcntrl.ControllerContext = ControllerContext;
                objcntrl.CloseRequisitionIfPullCompleted(objItemsToPull[0].RequisitionMasterGUID);
            }

            return Json(new { Status = true, ErrorMessages = lstErrorMessages }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RequisitionToolCheckout(RequisitionItemsToPull objReqToolToCheckout)
        {
            AssetsController toolController = new AssetsController();
            TechnicialMasterDAL objTechnicialMasterDAL = new TechnicialMasterDAL(this.enterPriseDBName);
            string Technicain = "";
            if (objReqToolToCheckout.TechnicianGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
            {
                TechnicianMasterDTO objTechnicianMasterDTO = objTechnicialMasterDAL.GetTechnicianByGuidPlain(objReqToolToCheckout.TechnicianGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                Technicain = objTechnicianMasterDTO.TechnicianCode;

                if (!string.IsNullOrEmpty(objTechnicianMasterDTO.Technician))
                    Technicain = Technicain + " --- " + objTechnicianMasterDTO.Technician;
            }
            long SessionUserId = SessionHelper.UserID;
            Guid? WOGuid = null;
            if (!string.IsNullOrEmpty(objReqToolToCheckout.WorkOrderDetailGUID))
            {
                Guid tempWOGuid = Guid.Empty;
                Guid.TryParse(objReqToolToCheckout.WorkOrderDetailGUID, out tempWOGuid);
                if (tempWOGuid != Guid.Empty)
                    WOGuid = tempWOGuid;
            }
            string retunMsg = toolController.CheckOutCheckIn("co", (int)objReqToolToCheckout.PullCreditQuantity, false, objReqToolToCheckout.ToolGUID.GetValueOrDefault(Guid.Empty),
                                             0, 0, 0, objReqToolToCheckout.ToolCheckoutUDF1, objReqToolToCheckout.ToolCheckoutUDF2, objReqToolToCheckout.ToolCheckoutUDF3,
                                             objReqToolToCheckout.ToolCheckoutUDF4, objReqToolToCheckout.ToolCheckoutUDF5, "", true, Technicain, Guid.Parse(objReqToolToCheckout.RequisitionDetailGUID), WOGuid);

            string status = "ok";
            if (retunMsg == "ok")
            {
                RequisitionDetailsDAL reqDetailDAL = new RequisitionDetailsDAL(this.enterPriseDBName);
                RequisitionDetailsDTO reqDetailDTO = reqDetailDAL.GetRequisitionDetailsByGUIDPlain(Guid.Parse(objReqToolToCheckout.RequisitionDetailGUID));
                reqDetailDTO.QuantityPulled = reqDetailDTO.QuantityPulled.GetValueOrDefault(0) + objReqToolToCheckout.PullCreditQuantity;
                reqDetailDTO.EditedFrom = "Web";
                reqDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                reqDetailDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                reqDetailDTO.LastUpdatedBy = SessionHelper.UserID;

                reqDetailDAL.Edit(reqDetailDTO, SessionUserId);

                ConsumeController objcntrl = new ConsumeController();
                objcntrl.CloseRequisitionIfPullCompleted(reqDetailDTO.RequisitionGUID.GetValueOrDefault(Guid.Empty).ToString());
            }
            else
                status = "fail";

            return Json(new { Message = retunMsg, Status = status, LocationMSG = "", PSLimitExceed = false }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdatePullData(Int64 ID, string ItemGUID, string ProjectGUID, double PullCreditQuantity, Int64 BinID, string PullCredit, double TempPullQTY, string UDF1, string UDF2, string UDF3, string UDF4, string UDF5, string RequisitionDetailGUID, string WorkOrderDetailGUID, Guid? ICDtlGUID, string ProjectSpendName = "", string PullOrderNumber = "", Guid? SupplierAccountNumberGuid = null, string callFrom = "singlepull", Int64 PullType = 1, double? EditedSellPrice = null)
        {
            bool IsPSLimitExceed = false;
            string message = "";
            string status = "";
            string locationMSG = "";
            long ModuleId = 0;

            PullMasterDAL obj = this.pullMasterDAL; //new PullMasterDAL(this.enterPriseDBName);
            PullMasterViewDTO objDTO = new PullMasterViewDTO();
            objDTO.ID = ID;
            objDTO.ItemGUID = Guid.Parse(ItemGUID);
            objDTO.PullType = PullType;

            ItemMasterDTO oItemRecord = new ItemMasterDAL(this.enterPriseDBName).GetItemWithoutJoins(null, objDTO.ItemGUID);
            ProjectMasterDAL objPrjDAL = new ProjectMasterDAL(this.enterPriseDBName);

            if (PullCredit.ToUpper().Trim() != "CREDIT")
            {
                bool isBinlevelEnforce = false;
                if (BinID != 0)
                {
                    BinMasterDTO objBin = new BinMasterDAL(this.enterPriseDBName).GetBinByID(BinID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (objBin != null && objBin.IsEnforceDefaultPullQuantity.GetValueOrDefault(false) == true)
                    {
                        if (PullCreditQuantity < objBin.DefaultPullQuantity || (decimal)PullCreditQuantity % (decimal)objBin.DefaultPullQuantity != 0)
                        {
                            return Json(new { Message = string.Format(ResPullMaster.LocationPullQtyMustBeDefaultPullQty, objBin.BinNumber, objBin.DefaultPullQuantity), Status = "fail", LocationMSG = locationMSG, PSLimitExceed = IsPSLimitExceed }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            isBinlevelEnforce = true;
                        }
                    }

                }

                if (oItemRecord.PullQtyScanOverride && oItemRecord.DefaultPullQuantity > 0 && isBinlevelEnforce == false)
                {
                    if (PullCreditQuantity < oItemRecord.DefaultPullQuantity || (decimal)PullCreditQuantity % (decimal)oItemRecord.DefaultPullQuantity != 0)
                    {  
                        return Json(new { Message = string.Format(ResPullMaster.PullQtyMustBeDefaultPullQty, oItemRecord.DefaultPullQuantity), Status = "fail", LocationMSG = locationMSG, PSLimitExceed = IsPSLimitExceed }, JsonRequestBehavior.AllowGet);
                    }
                }
                
                bool HasOnTheFlyEntryRight = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.OnTheFlyEntry);
                bool IsProjectSpendInsertAllow = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ProjectMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                if ((string.IsNullOrEmpty(ProjectGUID) && !string.IsNullOrEmpty(ProjectSpendName)) && (HasOnTheFlyEntryRight == false || IsProjectSpendInsertAllow == false))
                {
                    if (!string.IsNullOrEmpty(ProjectSpendName))
                    {
                        ProjectMasterDTO objProjectDTO = objPrjDAL.GetProjectspendByName(ProjectSpendName.Trim(), SessionHelper.RoomID, SessionHelper.CompanyID, false, false, null);
                        if(objProjectDTO == null)
                            return Json(new { Message = ResPullMaster.NoProjectspendOntheFlyRight, Status = "fail", LocationMSG = "", PSLimitExceed = "" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Message = ResPullMaster.NoProjectspendOntheFlyRight, Status = "fail", LocationMSG = "", PSLimitExceed = "" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else if ((!string.IsNullOrEmpty(ProjectGUID) && !string.IsNullOrEmpty(ProjectSpendName)) && (HasOnTheFlyEntryRight == false || IsProjectSpendInsertAllow == false))
                {
                    
                    ProjectMasterDTO objProjectDTO = objPrjDAL.GetProjectMasterByGuidNormal(Guid.Parse(ProjectGUID));
                    if (objProjectDTO != null)
                    {
                        if(objProjectDTO.ProjectSpendName.Trim().ToLower() != ProjectSpendName.Trim().ToLower())
                            return Json(new { Message = ResPullMaster.NoProjectspendOntheFlyRight, Status = "fail", LocationMSG = "", PSLimitExceed = "" }, JsonRequestBehavior.AllowGet);
                    }
                }

            }

            if (BinID != 0)
                objDTO.BinID = BinID;
            if (PullCreditQuantity != 0)
                objDTO.PoolQuantity = PullCreditQuantity;
            if (TempPullQTY != 0)
                objDTO.TempPullQTY = TempPullQTY;
           

            if (!string.IsNullOrEmpty(RequisitionDetailGUID))
            {
                objDTO.RequisitionDetailGUID = Guid.Parse(RequisitionDetailGUID);
                RequisitionDetailsDTO objReqDetailsDTO = new RequisitionDetailsDAL(this.enterPriseDBName).GetRequisitionDetailsByGUIDPlain(objDTO.RequisitionDetailGUID.GetValueOrDefault(Guid.Empty));
                if (objReqDetailsDTO != null && objReqDetailsDTO.QuantityApproved.GetValueOrDefault(0) < (objDTO.TempPullQTY.GetValueOrDefault(0) + objReqDetailsDTO.QuantityPulled.GetValueOrDefault(0)))
                { 
                    return Json(new { Message = ResPullMaster.PullQtyGreaterThanApproveQty, Status = "fail", LocationMSG = locationMSG, PSLimitExceed = IsPSLimitExceed }, JsonRequestBehavior.AllowGet);
                }
            }

            if (!string.IsNullOrEmpty(WorkOrderDetailGUID))
                objDTO.WorkOrderDetailGUID = Guid.Parse(WorkOrderDetailGUID);

            objDTO.CountLineItemGuid = ICDtlGUID;

            if (oItemRecord != null && string.IsNullOrWhiteSpace(PullOrderNumber) && (string.IsNullOrEmpty(RequisitionDetailGUID)))
            {
                SupplierMasterDAL objSupplierDAL = new SupplierMasterDAL(this.enterPriseDBName);
                SupplierMasterDTO objSupplierDTO = objSupplierDAL.GetSupplierByIDPlain(oItemRecord.SupplierID.GetValueOrDefault(0));
                if (objSupplierDTO != null && objSupplierDTO.PullPurchaseNumberType != null && objSupplierDTO.PullPurchaseNumberType.Value == 0)
                {
                    NotificationDAL objNotificationDAL = new NotificationDAL(this.enterPriseDBName);
                    SchedulerDTO objSchedulerDTO = objNotificationDAL.GetRoomSchedulesBySupplierID(objSupplierDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    bool isPoRequired = true;
                    if (objSchedulerDTO != null && objSchedulerDTO.ScheduleMode == 6)
                    {
                        isPoRequired = false;
                    }
                    if (isPoRequired)
                    {
                        return Json(new { Message = ResMessage.PullOrderNumberRequired, Status = "fail", LocationMSG = locationMSG, PSLimitExceed = IsPSLimitExceed }, JsonRequestBehavior.AllowGet);
                    }
                }
            }

            objDTO.PullOrderNumber = PullOrderNumber;
            objDTO.UDF1 = UDF1 != null ? UDF1.Trim() : UDF1;
            objDTO.UDF2 = UDF2 != null ? UDF2.Trim() : UDF2;
            objDTO.UDF3 = UDF3 != null ? UDF3.Trim() : UDF3;
            objDTO.UDF4 = UDF4 != null ? UDF4.Trim() : UDF4;
            objDTO.UDF5 = UDF5 != null ? UDF5.Trim() : UDF5;
            bool UDF1Encrypt = false;
            bool UDF2Encrypt = false;
            bool UDF3Encrypt = false;
            bool UDF4Encrypt = false;
            bool UDF5Encrypt = false;
            UDFDAL objUdf = new UDFDAL(this.enterPriseDBName);
            int TotalRecordCount = 0;
            var DataFromDBUDF = objUdf.GetPagedUDFsByUDFTableNamePlain(0, 5, out TotalRecordCount, "ID asc", SessionHelper.CompanyID, "PullMaster", SessionHelper.RoomID);

            foreach (UDFDTO u in DataFromDBUDF)
            {
                if (u.UDFColumnName == "UDF1")
                {
                    UDF1Encrypt = u.IsEncryption ?? false;
                }
                if (u.UDFColumnName == "UDF2")
                {
                    UDF2Encrypt = u.IsEncryption ?? false;
                }
                if (u.UDFColumnName == "UDF3")
                {
                    UDF3Encrypt = u.IsEncryption ?? false;
                }
                if (u.UDFColumnName == "UDF4")
                {
                    UDF4Encrypt = u.IsEncryption ?? false;
                }
                if (u.UDFColumnName == "UDF5")
                {
                    UDF5Encrypt = u.IsEncryption ?? false;
                }
            }
            CommonDAL objCommon = this.commonDAL; //new CommonDAL(this.enterPriseDBName);
            objDTO.UDF1 = (UDF1Encrypt == true && objDTO.UDF1 != null && (!string.IsNullOrWhiteSpace(objDTO.UDF1))) ? objCommon.GetEncryptValue(objDTO.UDF1) : objDTO.UDF1;
            objDTO.UDF2 = (UDF2Encrypt == true && objDTO.UDF2 != null && (!string.IsNullOrWhiteSpace(objDTO.UDF2))) ? objCommon.GetEncryptValue(objDTO.UDF2) : objDTO.UDF2;
            objDTO.UDF3 = (UDF3Encrypt == true && objDTO.UDF3 != null && (!string.IsNullOrWhiteSpace(objDTO.UDF3))) ? objCommon.GetEncryptValue(objDTO.UDF3) : objDTO.UDF3;
            objDTO.UDF4 = (UDF4Encrypt == true && objDTO.UDF4 != null && (!string.IsNullOrWhiteSpace(objDTO.UDF4))) ? objCommon.GetEncryptValue(objDTO.UDF4) : objDTO.UDF4;
            objDTO.UDF5 = (UDF5Encrypt == true && objDTO.UDF5 != null && (!string.IsNullOrWhiteSpace(objDTO.UDF5))) ? objCommon.GetEncryptValue(objDTO.UDF5) : objDTO.UDF5;
            UDFDAL objUDFApiController = new UDFDAL(this.enterPriseDBName);
            IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetRequiredUDFsByUDFTableNamePlain("PullMaster", SessionHelper.RoomID, SessionHelper.CompanyID);
            string udfRequier = string.Empty;
            
            foreach (var i in DataFromDB)
            {
                    if (i.UDFColumnName == "UDF1"  && string.IsNullOrWhiteSpace(objDTO.UDF1))
                    {
                        string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                        string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                        if (!string.IsNullOrEmpty(val))
                            i.UDFDisplayColumnName = val;
                        else
                            i.UDFDisplayColumnName = i.UDFColumnName;
                        udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                    }
                    else if (i.UDFColumnName == "UDF2"  && string.IsNullOrWhiteSpace(objDTO.UDF2))
                    {
                        string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                        string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                        if (!string.IsNullOrEmpty(val))
                            i.UDFDisplayColumnName = val;
                        else
                            i.UDFDisplayColumnName = i.UDFColumnName;
                        udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                    }
                    else if (i.UDFColumnName == "UDF3"  && string.IsNullOrWhiteSpace(objDTO.UDF3))
                    {
                        string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                        string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                        if (!string.IsNullOrEmpty(val))
                            i.UDFDisplayColumnName = val;
                        else
                            i.UDFDisplayColumnName = i.UDFColumnName;
                        udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                    }
                    else if (i.UDFColumnName == "UDF4"  && string.IsNullOrWhiteSpace(objDTO.UDF4))
                    {
                        string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                        string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                        if (!string.IsNullOrEmpty(val))
                            i.UDFDisplayColumnName = val;
                        else
                            i.UDFDisplayColumnName = i.UDFColumnName;
                        udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                    }
                    else if (i.UDFColumnName == "UDF5"  && string.IsNullOrWhiteSpace(objDTO.UDF5))
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

            if (!string.IsNullOrEmpty(udfRequier))
            {
                return Json(new { Message = udfRequier, Status = "fail", LocationMSG = locationMSG, PSLimitExceed = IsPSLimitExceed }, JsonRequestBehavior.AllowGet);
            }

            objDTO.SupplierID = (SessionHelper.UserSupplierIds != null && SessionHelper.UserSupplierIds.Any()) ? SessionHelper.UserSupplierIds[0] : 0;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
            objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
            objDTO.AddedFrom = "Web";
            objDTO.EditedFrom = "Web";

            if (!string.IsNullOrEmpty(ItemGUID))
                objDTO.ItemGUID = Guid.Parse(ItemGUID);
            objDTO.CreatedBy = SessionHelper.UserID;
            objDTO.CompanyID = SessionHelper.CompanyID;
            objDTO.Room = SessionHelper.RoomID;

            int IsCreditPullNothing = 0; // false means its PULL
            if (PullCredit == "credit")
                IsCreditPullNothing = 1;
            else if (PullCredit == "pull")
                IsCreditPullNothing = 2;
            else
                IsCreditPullNothing = 3;

            string ItemLocationMSG = "";

            if (PullCredit == "pull")
            {
                objDTO.PullCredit = PullCredit;
                objDTO.ActionType = PullCredit;
            }
            else
            {
                objDTO.PullCredit = "MS Pull";
                objDTO.ActionType = "MS Pull";
                PullCredit = "MS Pull";
            }

            if (objDTO.BinID != null)
            {
                BinMasterDTO objBinList = new BinMasterDTO();
                objBinList = new BinMasterDAL(this.enterPriseDBName).GetBinByID(objDTO.BinID ?? 0, SessionHelper.RoomID, SessionHelper.CompanyID);
                if (objBinList != null && objBinList.IsStagingLocation)
                {
                    objDTO.PullCredit = "MS Pull";
                    objDTO.ActionType = "MS Pull";
                    PullCredit = "MS Pull";
                }
                else
                {
                    objDTO.PullCredit = PullCredit;
                    objDTO.ActionType = PullCredit;
                }
            }

            if (objDTO.RequisitionDetailGUID != null && objDTO.RequisitionDetailGUID != Guid.Empty)
                ModuleId = (long)eTurnsWeb.Helper.SessionHelper.ModuleList.Requisitions;
            else if (objDTO.WorkOrderDetailGUID != null && objDTO.WorkOrderDetailGUID != Guid.Empty)
                ModuleId = (long)eTurnsWeb.Helper.SessionHelper.ModuleList.WorkOrders;

            if (!string.IsNullOrEmpty(ProjectGUID))
                objDTO.ProjectSpendGUID = Guid.Parse(ProjectGUID);
            else
            {
                if (!string.IsNullOrWhiteSpace(ProjectSpendName))
                {
                    ProjectMasterDAL objProjectSpendDAL = new ProjectMasterDAL(this.enterPriseDBName);
                    ProjectMasterDTO projectMaster = objProjectSpendDAL.GetProjectspendByName(ProjectSpendName, SessionHelper.RoomID, SessionHelper.CompanyID, false, false, null);

                    if (projectMaster != null && projectMaster.GUID != Guid.Empty)
                    {
                        objDTO.ProjectSpendGUID = projectMaster.GUID;
                    }
                    else
                    {
                        ProjectMasterDTO objProjectSpendDTO = new ProjectMasterDTO();
                        objProjectSpendDTO.ProjectSpendName = ProjectSpendName;
                        objProjectSpendDTO.AddedFrom = "Web";
                        objProjectSpendDTO.EditedFrom = "Web";
                        objProjectSpendDTO.CompanyID = SessionHelper.CompanyID;
                        objProjectSpendDTO.Room = SessionHelper.RoomID;
                        objProjectSpendDTO.DollarLimitAmount = 0;
                        objProjectSpendDTO.Description = string.Empty;
                        objProjectSpendDTO.DollarUsedAmount = null;
                        objProjectSpendDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objProjectSpendDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objProjectSpendDTO.Created = DateTimeUtility.DateTimeNow;
                        objProjectSpendDTO.Updated = DateTimeUtility.DateTimeNow;
                        objProjectSpendDTO.CreatedBy = SessionHelper.UserID;
                        objProjectSpendDTO.LastUpdatedBy = SessionHelper.UserID;
                        objProjectSpendDTO.UDF1 = string.Empty;
                        objProjectSpendDTO.UDF2 = string.Empty;
                        objProjectSpendDTO.UDF3 = string.Empty;
                        objProjectSpendDTO.UDF4 = string.Empty;
                        objProjectSpendDTO.UDF5 = string.Empty;
                        objProjectSpendDTO.GUID = Guid.NewGuid();
                        //objProjectSpendDTO.ProjectSpendItems = Guid.Parse(ItemGUID);

                        ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(this.enterPriseDBName);
                        ItemMasterDTO objItemmasterDTO = new ItemMasterDTO();
                        objItemmasterDTO = objItemMasterDAL.GetItemWithoutJoins(null, objDTO.ItemGUID);

                        List<ProjectSpendItemsDTO> projectSpendItemList = new List<ProjectSpendItemsDTO>();
                        ProjectSpendItemsDTO projectSpendItem = new ProjectSpendItemsDTO();
                        projectSpendItem.QuantityLimit = null;
                        projectSpendItem.QuantityUsed = null;
                        projectSpendItem.DollarLimitAmount = null;
                        projectSpendItem.DollarUsedAmount = null;
                        projectSpendItem.ItemGUID = Guid.Parse(ItemGUID);
                        projectSpendItem.CreatedBy = SessionHelper.UserID;
                        projectSpendItem.LastUpdatedBy = SessionHelper.UserID;
                        projectSpendItem.Room = SessionHelper.RoomID;
                        projectSpendItem.CompanyID = SessionHelper.CompanyID;
                        if (objItemmasterDTO != null)
                            projectSpendItem.ItemNumber = objItemmasterDTO.ItemNumber;
                        projectSpendItem.IsArchived = false;
                        projectSpendItem.IsDeleted = false;

                        projectSpendItem.ProjectSpendName = ProjectSpendName;
                        projectSpendItem.IsDeleted = false;
                        projectSpendItem.IsArchived = false;
                        projectSpendItemList.Add(projectSpendItem);

                        objProjectSpendDTO.ProjectSpendItems = projectSpendItemList;

                        objProjectSpendDTO.IsDeleted = false;
                        objProjectSpendDTO.IsArchived = false;


                        objProjectSpendDAL.Insert(objProjectSpendDTO);
                        projectSpendItem.ProjectGUID = objProjectSpendDTO.GUID;

                        objDTO.ProjectSpendGUID = objProjectSpendDTO.GUID;
                    }
                }
            }
            try
            {
                // <param name="IsCreditPullNothing"></param> 1 = credit, 2 = pull, 3 = nothing
                long SessionUserId = SessionHelper.UserID;
                #region "Check Project Spend Condition"
                bool IsProjecSpendAllowed = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowOverrideProjectSpendLimits, eTurnsWeb.Helper.SessionHelper.PermissionType.Approval);
                #endregion
                string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);

                objDTO.SupplierAccountGuid = SupplierAccountNumberGuid;

                RoomDAL objRoomDAL = new RoomDAL(this.enterPriseDBName);
                RoomDTO objRoomDTO = objRoomDAL.GetRoomByIDPlain(SessionHelper.RoomID);
                bool AllowNegetive = false;
                if(objRoomDTO != null && objRoomDTO.AllowPullBeyondAvailableQty == true
                   && objDTO.ActionType.ToLower().Equals("pull"))
                {
                    AllowNegetive = true;
                }

                bool AllowEditItemSellPriceonWorkOrderPull = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowEditItemSellPriceonWorkOrderPull);
                bool isFromWorkOrder = false;
                if (objDTO.WorkOrderDetailGUID != null && objDTO.WorkOrderDetailGUID != Guid.Empty)
                {
                    isFromWorkOrder = true;
                }

                obj.UpdatePullData(objDTO, IsCreditPullNothing, SessionHelper.RoomID, SessionHelper.CompanyID, ModuleId, out ItemLocationMSG, IsProjecSpendAllowed, out IsPSLimitExceed, RoomDateFormat, SessionUserId,SessionHelper.EnterPriceID, ResourceHelper.CurrentCult.Name, AllowNegetive: AllowNegetive,EditedSellPrice, isFromWorkOrder,AllowEditItemSellPriceonWorkOrderPull);
                if (ItemLocationMSG != "")
                    locationMSG = ItemLocationMSG;

                message = ResMessage.SaveMessage;
                status = "ok";

                if(objDTO.ItemGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                {
                    //QuickBookItemDAL objQBItemDAL = new QuickBookItemDAL(SessionHelper.EnterPriseDBName);
                    //objQBItemDAL.InsertQuickBookItem(objDTO.ItemGUID.GetValueOrDefault(Guid.Empty), SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, "Update", false, SessionHelper.UserID, "Web", null, "Pull");
                }

                //ItemMasterDAL objItemMaster = new ItemMasterDAL(this.enterPriseDBName);
                //objItemMaster.EditDate(Guid.Parse(ItemGUID), "EditPulledDate");
            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                message = string.Format(ResMessage.SaveErrorMsg, ex.Message);
                status = "fail";
            }
            if (status == "ok" && ICDtlGUID != null && ICDtlGUID != Guid.Empty)
            {

            }

            string strSupPOPair = string.Empty;
            if (string.IsNullOrEmpty(locationMSG))
            {
                strSupPOPair = GetSupplierPOPairList();
            }

            string pullGUIDs = "<DataGuids>" + Convert.ToString(objDTO.GUID) + "</DataGuids>";
            string eTurnsScheduleDBName = (Convert.ToString(ConfigurationManager.AppSettings["eTurnsScheduleDBName"]) ?? "eTurnsSchedule");
            NotificationDAL objSCHNotificationDAL = new NotificationDAL(this.enterPriseDBName);

            if (status == "ok" && string.IsNullOrWhiteSpace(locationMSG) && ModuleId == 0 && (callFrom ?? string.Empty) == "singlepull")
            {
                try
                {
                    List<NotificationDTO> lstNotification = objSCHNotificationDAL.GetCurrentNotificationListByEventName("OPC", SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                    if (lstNotification != null && lstNotification.Count > 0)
                    {
                        objSCHNotificationDAL.SendMailForImmediate(lstNotification, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.EnterPriceID, eTurnsScheduleDBName, pullGUIDs);
                    }
                }
                catch (Exception ex)
                {
                    CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                }
            }
            if (status == "ok" && string.IsNullOrWhiteSpace(locationMSG) && ModuleId == 66 && (callFrom ?? string.Empty) == "singlepull")
            {
                try
                {                  
                    List<NotificationDTO> lstORPCCNotification = objSCHNotificationDAL.GetCurrentNotificationListByEventName("ORPC", SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                    if (lstORPCCNotification != null && lstORPCCNotification.Count > 0)
                    {
                        objSCHNotificationDAL.SendMailForImmediate(lstORPCCNotification, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.EnterPriceID, eTurnsScheduleDBName, pullGUIDs);
                    }
                }
                catch (Exception ex)
                {
                    CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                }
            }
            if (status == "ok" && string.IsNullOrWhiteSpace(locationMSG) && ModuleId == 67 && (callFrom ?? string.Empty) == "singlepull")
            {
                try
                {
                    List<NotificationDTO> lstOWPCNotification = objSCHNotificationDAL.GetCurrentNotificationListByEventName("OWPC", SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                    if (lstOWPCNotification != null && lstOWPCNotification.Count > 0)
                    {
                        objSCHNotificationDAL.SendMailForImmediate(lstOWPCNotification, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.EnterPriceID, eTurnsScheduleDBName, pullGUIDs);
                    }
                }
                catch (Exception ex)
                {
                    CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                }
            }

            return Json(new { Message = message, Status = status, LocationMSG = locationMSG, PSLimitExceed = IsPSLimitExceed, SupPOParitList = strSupPOPair, GeneratedPullGUID = (objDTO.GUID == Guid.Empty) ? "" : Convert.ToString(objDTO.GUID) }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult SavePullGuidsInSchedule(string DataGuids)
        {
            if (!string.IsNullOrWhiteSpace(DataGuids))
            {
                try
                {
                    char[] commaTrim = { ',' };
                    string pullGUIDs = "<DataGuids>" + DataGuids.Trim(commaTrim) + "</DataGuids>";
                    string eTurnsScheduleDBName = (Convert.ToString(ConfigurationManager.AppSettings["eTurnsScheduleDBName"]) ?? "eTurnsSchedule");
                    NotificationDAL objNotificationDAL = new NotificationDAL(this.enterPriseDBName);
                    List<NotificationDTO> lstNotification = objNotificationDAL.GetCurrentNotificationListByEventName("OPC", SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                    if (lstNotification != null && lstNotification.Count > 0)
                    {
                        objNotificationDAL.SendMailForImmediate(lstNotification, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.EnterPriceID, eTurnsScheduleDBName, pullGUIDs);
                    }
                }
                catch (Exception ex)
                {
                    CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                }
            }
            return Json(new { Message = "ok", Status = "success" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckValidPullData(Int64 ID, string ItemGUID, string ProjectGUID, double PullCreditQuantity, Int64 BinID, string PullCredit, double TempPullQTY, string UDF1, string UDF2, string UDF3, string UDF4, string UDF5, string RequisitionDetailGUID, string WorkOrderDetailGUID, Guid? ICDtlGUID)
        {
            bool IsPSLimitExceed = false;
            string message = "";
            string status = "";
            string locationMSG = "";

            PullMasterDAL obj = this.pullMasterDAL;//new PullMasterDAL(this.enterPriseDBName);
            PullMasterViewDTO objDTO = new PullMasterViewDTO();
            objDTO.ID = ID;
            objDTO.ItemGUID = Guid.Parse(ItemGUID);

            if (!string.IsNullOrEmpty(ProjectGUID))
                objDTO.ProjectSpendGUID = Guid.Parse(ProjectGUID);
            if (BinID != 0)
                objDTO.BinID = BinID;
            if (PullCreditQuantity != 0)
                objDTO.PoolQuantity = PullCreditQuantity;
            if (TempPullQTY != 0)
                objDTO.TempPullQTY = TempPullQTY;

            if (!string.IsNullOrEmpty(RequisitionDetailGUID))
                objDTO.RequisitionDetailGUID = Guid.Parse(RequisitionDetailGUID);

            if (!string.IsNullOrEmpty(WorkOrderDetailGUID))
                objDTO.WorkOrderDetailGUID = Guid.Parse(WorkOrderDetailGUID);

            objDTO.CountLineItemGuid = ICDtlGUID;


            objDTO.UDF1 = UDF1;
            objDTO.UDF2 = UDF2;
            objDTO.UDF3 = UDF3;
            objDTO.UDF4 = UDF4;
            objDTO.UDF5 = UDF5;

            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            if (!string.IsNullOrEmpty(ItemGUID))
                objDTO.ItemGUID = Guid.Parse(ItemGUID);
            objDTO.CreatedBy = SessionHelper.UserID;
            objDTO.CompanyID = SessionHelper.CompanyID;
            objDTO.Room = SessionHelper.RoomID;
            objDTO.PullCredit = PullCredit;
            try
            {
                // <param name="IsCreditPullNothing"></param> 1 = credit, 2 = pull, 3 = nothing

                int IsCreditPullNothing = 0; // false means its PULL
                if (PullCredit == "credit")
                    IsCreditPullNothing = 1;
                else if (PullCredit == "pull")
                    IsCreditPullNothing = 2;
                else
                    IsCreditPullNothing = 3;

                string ItemLocationMSG = "";


                #region "Check Project Spend Condition"
                bool IsProjecSpendAllowed = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowOverrideProjectSpendLimits, eTurnsWeb.Helper.SessionHelper.PermissionType.Approval);
                #endregion

                obj.CheckValidPullData(objDTO, IsCreditPullNothing, SessionHelper.RoomID, SessionHelper.CompanyID, out ItemLocationMSG, IsProjecSpendAllowed, out IsPSLimitExceed,SessionHelper.EnterPriceID,ResourceHelper.CurrentCult.Name);

                if (ItemLocationMSG != "")
                    locationMSG = ItemLocationMSG;

                message = ResMessage.SaveMessage;
                status = "ok";
            }
            catch
            {
                message = ResMessage.SaveErrorMsg;
                status = "fail";
            }
            if (status == "ok" && ICDtlGUID != null && ICDtlGUID != Guid.Empty)
            {

            }
            return Json(new { Message = message, Status = status, LocationMSG = locationMSG, PSLimitExceed = IsPSLimitExceed }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProjectDataByItemID(Int64 ItemIDKey)
        {

            ItemMasterDAL objItem = new ItemMasterDAL(this.enterPriseDBName);
            //var Result = objItem.GetRecord(ItemIDKey, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
            ProjectMasterDAL objProjects = new ProjectMasterDAL(this.enterPriseDBName);
            var ProjectsByItem = new List<ProjectMasterDTO>();//objProjects.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            return Json(new
            {
                ProjectData = ProjectsByItem,
            },
                       JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeletePullMasterRecords(string ids, string fromwhere, Guid? WOGUID)
        {
            try
            {
                long SessionUserId = SessionHelper.UserID;
                //                PullMasterDAL obj = new PullMasterDAL(SessionHelper.EnterPriseDBName);
                PullMasterDAL obj = this.pullMasterDAL; //new PullMasterDAL(this.enterPriseDBName);
                obj.DeletePullsByPullIds(ids, SessionHelper.UserID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionUserId,SessionHelper.EnterPriceID);
                //return "ok";
                if (!string.IsNullOrWhiteSpace(fromwhere) && fromwhere == "wo" && WOGUID != null && WOGUID != Guid.Empty)
                {
                    new WorkOrderLineItemsDAL(this.enterPriseDBName).UpdateWOItemAndTotalCost(WOGUID.ToString(), SessionHelper.RoomID, SessionHelper.CompanyID);
                }
                return Json(new { Message = ResMessage.DeletedSuccessfully, Status = "ok" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                //return ex.Message;
                return Json(new { Message = ex.Message, Status = "ok" }, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpPost]
        public JsonResult GetQTYFromLocationAndItem(Int64 BindID, string ItemGUID)
        {
            ItemLocationQTYDAL objLocQTY = new ItemLocationQTYDAL(this.enterPriseDBName);
            Guid tmpITEmGUID = Guid.Parse(ItemGUID);
            //ItemLocationQTYDTO lstLocDTO = objLocQTY.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => x.BinID == BindID && x.ItemGUID == tmpITEmGUID).SingleOrDefault();
            ItemLocationQTYDTO lstLocDTO = objLocQTY.GetItemLocationQTY(SessionHelper.RoomID, SessionHelper.CompanyID, BindID, Convert.ToString(tmpITEmGUID)).FirstOrDefault();

            if (lstLocDTO != null && lstLocDTO.Quantity > 0)
                return Json(new
                {
                    AvalQTY = lstLocDTO.Quantity,
                },
                       JsonRequestBehavior.AllowGet);
            else
                return Json(new
                {
                    AvalQTY = 0,
                },
                       JsonRequestBehavior.AllowGet);
        }


        #endregion


        /// <summary>
        /// GetProjectSpentList
        /// </summary>
        /// <param name="NameStartWith"></param>
        /// <returns></returns>
        public JsonResult GetProjectSpentListForNewPULL(string NameStartWith)
        {
            ProjectMasterDAL objDAL = null;
            IEnumerable<ProjectMasterDTO> lstDTO = null;
            Int64 RoomID = SessionHelper.RoomID;
            Int64 CompanyID = SessionHelper.CompanyID;
            List<DTOForAutoComplete> returnKeyValList = new List<DTOForAutoComplete>();
            try
            {
                objDAL = new ProjectMasterDAL(this.enterPriseDBName);
                var projectTrackAllUsageAgainstThis = objDAL.GetDefaultProjectSpendRecord(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

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

                if (returnKeyValList.Count > 0 && !string.IsNullOrEmpty(NameStartWith) && !string.IsNullOrWhiteSpace(NameStartWith))
                {
                    returnKeyValList = returnKeyValList.Where(x => x.Key.ToLower().StartsWith(NameStartWith.ToLower())).ToList();
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

        /// <summary>
        /// Get Item Locations For NewPull Grid
        /// </summary>
        /// <param name="ItemGuid"></param>
        /// <param name="NameStartWith"></param>
        /// <returns></returns>
        public JsonResult GetItemLocationsForNewPullGrid(Guid ItemGuid, string NameStartWith, bool IsStagingHeaderSelected = false)
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
            bool isNewBinCreated = false;
            Int64? NewBinID = 0;
            try
            {
                //if (SessionHelper.CompanyConfig != null)
                //    qtyFormat = "N" + SessionHelper.CompanyConfig.QuantityDecimalPoints;

                bool isAllowNegetive = false;
                ItemMasterDTO oItemRecord = new ItemMasterDAL(this.enterPriseDBName).GetItemWithoutJoins(null, ItemGuid);
                RoomDAL objRoomDAL = new RoomDAL(this.enterPriseDBName);
                RoomDTO objRoomDTO = objRoomDAL.GetRoomByIDPlain(RoomID);
                if (objRoomDTO != null && objRoomDTO.AllowPullBeyondAvailableQty == true
                    && oItemRecord != null
                    && oItemRecord.SerialNumberTracking == false
                    && oItemRecord.LotNumberTracking == false
                    && oItemRecord.DateCodeTracking == false)
                {
                    isAllowNegetive = true;
                    if (!string.IsNullOrWhiteSpace(NameStartWith))
                    {
                        BinMasterDAL objItemLocationDetailsDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                        NewBinID = objItemLocationDetailsDAL.GetOrInsertBinIDByName(ItemGuid, NameStartWith, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID, false);
                        isNewBinCreated = true;
                    }
                }
                
                if (!string.IsNullOrEmpty(SessionHelper.NumberDecimalDigits))
                    qtyFormat = "N" + SessionHelper.NumberDecimalDigits;
                objBinDAL = new BinMasterDAL(this.enterPriseDBName);
                //lstBinList = objBinDAL.GetAllRecords(RoomID, CompanyID, false, false).Where(x => x.ItemGUID.GetValueOrDefault(Guid.Empty) == ItemGuid);
                //lstBinList = objBinDAL.GetItemLocation(RoomID, CompanyID, false, false, ItemGuid, 0, null, null);//.Where(x => x.ItemGUID.GetValueOrDefault(Guid.Empty) == ItemGuid);
                lstBinList = objBinDAL.GetInventoryAndStagingBinsByItem(RoomID, CompanyID, ItemGuid);
                var stagingResourceValue = ResPullMaster.Staging;
                foreach (var item in lstBinList)
                {
                    if (item.IsStagingLocation)
                    {
                        objMSDAL = new MaterialStagingDetailDAL(this.enterPriseDBName);
                        //lstMSDetailDTO = objMSDAL.GetStagingLocationByItem(ItemGuid, RoomID, CompanyID).Where(x => x.Quantity > 0 && x.StagingBinID == item.ID);
                        lstMSDetailDTO = objMSDAL.GetStagingLocationByItem(ItemGuid, RoomID, CompanyID, item.ID, string.Empty, true);

                        if (isAllowNegetive == true)
                        {
                            if (lstMSDetailDTO != null)
                            { 
                                DTOForAutoComplete obj = new DTOForAutoComplete()
                                {
                                    Key = item.BinNumber,
                                    Value = item.BinNumber == "[|EmptyStagingBin|]" ? string.Format(stagingResourceValue, lstMSDetailDTO.Sum(x => x.Quantity)) : item.BinNumber + " " + string.Format(stagingResourceValue, lstMSDetailDTO.Sum(x => x.Quantity)),
                                    ID = item.ID,
                                    OtherInfo1 = "",
                                    GUID = item.GUID,
                                };
                                returnKeyValList.Add(obj);
                            }
                            else
                            {
                                DTOForAutoComplete obj = new DTOForAutoComplete()
                                {
                                    Key = item.BinNumber,
                                    Value = item.BinNumber == "[|EmptyStagingBin|]" ? string.Format(stagingResourceValue, 0) : item.BinNumber + " " + string.Format(stagingResourceValue, 0),
                                    ID = item.ID,
                                    OtherInfo1 = "",
                                    GUID = item.GUID,
                                };
                                returnKeyValList.Add(obj);
                            }
                        }
                        else
                        {
                            if (lstMSDetailDTO != null && lstMSDetailDTO.Count() > 0 && lstMSDetailDTO.Sum(x => x.Quantity) > 0)
                            {
                                DTOForAutoComplete obj = new DTOForAutoComplete()
                                {
                                    Key = item.BinNumber,
                                    Value = item.BinNumber == "[|EmptyStagingBin|]" ? string.Format(stagingResourceValue, lstMSDetailDTO.Sum(x => x.Quantity)) : item.BinNumber + " " + string.Format(stagingResourceValue, lstMSDetailDTO.Sum(x => x.Quantity)),
                                    ID = item.ID,
                                    OtherInfo1 = "",
                                    GUID = item.GUID,
                                };
                                returnKeyValList.Add(obj);
                            }
                        }
                    }
                    else
                    {
                        if (!IsStagingHeaderSelected)
                        {
                            objLocationQtyDAL = new ItemLocationQTYDAL(this.enterPriseDBName);
                            objLocatQtyDTO = objLocationQtyDAL.GetRecordByBinItem(ItemGuid, item.ID, RoomID, CompanyID);
                            if (isAllowNegetive == true)
                            {
                                if (objLocatQtyDTO != null)
                                {
                                    DTOForAutoComplete obj = new DTOForAutoComplete()
                                    {
                                        Key = item.BinNumber,
                                        Value = item.BinNumber + " (" + objLocatQtyDTO.Quantity.ToString(qtyFormat) + ")",
                                        ID = item.ID,
                                        OtherInfo1 = (item.DefaultPullQuantity.GetValueOrDefault(0) > 0) ? item.DefaultPullQuantity.Value.ToString() : "",
                                        GUID = item.GUID,
                                    };
                                    returnKeyValList.Add(obj);
                                }
                                else
                                {
                                    DTOForAutoComplete obj = new DTOForAutoComplete()
                                    {
                                        Key = item.BinNumber,
                                        Value = item.BinNumber + " (" + 0.ToString(qtyFormat) + ")",
                                        ID = item.ID,
                                        OtherInfo1 = (item.DefaultPullQuantity.GetValueOrDefault(0) > 0) ? item.DefaultPullQuantity.Value.ToString() : "",
                                        GUID = item.GUID,
                                    };
                                    returnKeyValList.Add(obj);
                                }
                            }
                            else
                            {
                                if (objLocatQtyDTO != null && objLocatQtyDTO.Quantity > 0)
                                {
                                    DTOForAutoComplete obj = new DTOForAutoComplete()
                                    {
                                        Key = item.BinNumber,
                                        Value = item.BinNumber + " (" + objLocatQtyDTO.Quantity.ToString(qtyFormat) + ")",
                                        ID = item.ID,
                                        OtherInfo1 = (item.DefaultPullQuantity.GetValueOrDefault(0) > 0) ? item.DefaultPullQuantity.Value.ToString() : "",
                                        GUID = item.GUID,
                                    };
                                    returnKeyValList.Add(obj);
                                }
                            }
                        }
                    }
                }

                if (returnKeyValList.Count > 0 && !string.IsNullOrEmpty(NameStartWith) && !string.IsNullOrWhiteSpace(NameStartWith))
                {
                    returnKeyValList = returnKeyValList.Where(x => x.Key.ToLower().StartsWith(NameStartWith.ToLower())).ToList();
                }
                return Json(new { returnKeyValList = returnKeyValList,NewBinID=NewBinID, isNewBinCreated = isNewBinCreated }, JsonRequestBehavior.AllowGet);
                //return Json(returnKeyValList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { returnKeyValList = returnKeyValList, NewBinID=0, isNewBinCreated = isNewBinCreated }, JsonRequestBehavior.AllowGet);
                //return Json(returnKeyValList, JsonRequestBehavior.AllowGet);
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

        public ActionResult SetBillingForPull(string PullGUIDs)
        {
            PullMasterDAL oPullMasterDAL = this.pullMasterDAL; //new PullMasterDAL(this.enterPriseDBName);
            List<PullMasterViewDTO> oPullList = oPullMasterDAL.GetPullsByGuidsNormal(PullGUIDs);

            foreach (var item in oPullList)
            {
                item.Billing = true;
            }

            return PartialView("_SetBillingForPull", oPullList);
        }

        [HttpPost]
        public JsonResult SetBillingForPullMaster(List<PullMasterViewDTO> oPullList)
        {
            int errorCount = 0;
            string udfRequier = string.Empty;
            PullMasterDAL oPullMasterDAL = this.pullMasterDAL; //new PullMasterDAL(this.enterPriseDBName);
            List<string> errorList = new List<string>();

            foreach (var item in oPullList)
            {
                UDFDAL objUDFApiController = new UDFDAL(this.enterPriseDBName);
                IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetRequiredUDFsByUDFTableNamePlain("PullMaster", SessionHelper.RoomID, SessionHelper.CompanyID);
                udfRequier = string.Empty;
                
                foreach (var i in DataFromDB)
                {
                    string udfRequierMsg = string.Empty;
                        if (i.UDFColumnName == "UDF1"  && string.IsNullOrWhiteSpace(item.UDF1))
                        {
                            string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                            string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                            if (!string.IsNullOrEmpty(val))
                                i.UDFDisplayColumnName = val;
                            else
                                i.UDFDisplayColumnName = i.UDFColumnName;
                            udfRequierMsg = i.UDFDisplayColumnName + ",";
                        }
                        else if (i.UDFColumnName == "UDF2"  && string.IsNullOrWhiteSpace(item.UDF2))
                        {
                            string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                            string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                            if (!string.IsNullOrEmpty(val))
                                i.UDFDisplayColumnName = val;
                            else
                                i.UDFDisplayColumnName = i.UDFColumnName;
                            udfRequierMsg = i.UDFDisplayColumnName + ",";
                        }
                        else if (i.UDFColumnName == "UDF3"  && string.IsNullOrWhiteSpace(item.UDF3))
                        {
                            string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                            string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                            if (!string.IsNullOrEmpty(val))
                                i.UDFDisplayColumnName = val;
                            else
                                i.UDFDisplayColumnName = i.UDFColumnName;
                            udfRequierMsg = i.UDFDisplayColumnName + ",";
                        }
                        else if (i.UDFColumnName == "UDF4"  && string.IsNullOrWhiteSpace(item.UDF4))
                        {
                            string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                            string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                            if (!string.IsNullOrEmpty(val))
                                i.UDFDisplayColumnName = val;
                            else
                                i.UDFDisplayColumnName = i.UDFColumnName;
                            udfRequierMsg = i.UDFDisplayColumnName + ",";
                        }
                        else if (i.UDFColumnName == "UDF5"  && string.IsNullOrWhiteSpace(item.UDF5))
                        {
                            string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                            string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                            if (!string.IsNullOrEmpty(val))
                                i.UDFDisplayColumnName = val;
                            else
                                i.UDFDisplayColumnName = i.UDFColumnName;
                            udfRequierMsg = i.UDFDisplayColumnName + ",";
                        }

                        if (!string.IsNullOrEmpty(udfRequier))
                        {
                            udfRequier += udfRequierMsg;
                        }
                        else
                        {
                            udfRequier = udfRequierMsg;
                        }
                    
                }
                bool IsError = false;
                if (!string.IsNullOrWhiteSpace(udfRequier))
                {
                    udfRequier = item.ItemNumber + " : " + udfRequier.Trim(',') + " " + ResPullMaster.IsAreRequired; 
                    errorList.Add(udfRequier);
                    errorCount++;
                    IsError = true;
                }

                if (string.IsNullOrWhiteSpace(item.PullOrderNumber))
                {
                    udfRequier += item.ItemNumber + " : " + ResMessage.PullOrderNumberRequired;
                    errorList.Add(udfRequier);
                    errorCount++;
                    IsError = true;
                }

                if (!IsError)
                {
                    #region WI-6832 : Production, Complete pull popup should insert newly entered pull order number                                        
                    bool IsPullInsert = SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.PullMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                    if (IsPullInsert)
                    {
                        string _returnmessage = ResCommon.RecordsSavedSuccessfully;
                        string _returnstatus = ResCommon.Ok;
                        oPullMasterDAL.UpdatePullOrderNumberInPullHistory(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, Convert.ToString(item.GUID), item.PullOrderNumber, out _returnmessage, out _returnstatus,SessionHelper.EnterPriceID,ResourceHelper.CurrentCult.Name);
                    }
                    #endregion

                    PullMasterViewDTO oPullMaster = oPullMasterDAL.GetPullByGuidPlain(item.GUID);
                    oPullMaster.Billing = item.Billing;
                    oPullMaster.PullOrderNumber = item.PullOrderNumber;
                    oPullMaster.LastUpdatedBy = SessionHelper.UserID;
                    oPullMaster.UDF1 = item.UDF1;
                    oPullMaster.UDF2 = item.UDF2;
                    oPullMaster.UDF3 = item.UDF3;
                    oPullMaster.UDF4 = item.UDF4;
                    oPullMaster.UDF5 = item.UDF5;
                    oPullMasterDAL.UpdateBillingAndPullOrderNumber(oPullMaster);
                    errorList.Add(item.ItemNumber + ": " + ResOrder.msgSavedSuccessfully);
                }
            }

            if (errorCount > 0)
            {
                return Json(new { Message = udfRequier, Status = "fail", itemList = errorList }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Message = ResCommon.RecordsSavedSuccessfully, Status = "OK" }, JsonRequestBehavior.AllowGet);
            //return Json(new { Message = "Fail", Status = "Fail" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SetBillingForPullMaster(string PullGUID, string PullOrderNumber, bool IsBilling, string UDF1 = "", string UDF2 = "", string UDF3 = "", string UDF4 = "", string UDF5 = "")
        {
            PullMasterDAL oPullMasterDAL = this.pullMasterDAL; //new PullMasterDAL(this.enterPriseDBName);
            
            if (!string.IsNullOrWhiteSpace(PullOrderNumber))
            {
                PullMasterViewDTO oPullMaster = oPullMasterDAL.GetPullByGuidPlain(Guid.Parse(PullGUID));
                oPullMaster.Billing = IsBilling;
                oPullMaster.PullOrderNumber = PullOrderNumber;
                oPullMaster.LastUpdatedBy = SessionHelper.UserID;
                oPullMaster.UDF1 = UDF1;
                oPullMaster.UDF2 = UDF2;
                oPullMaster.UDF3 = UDF3;
                oPullMaster.UDF4 = UDF4;
                oPullMaster.UDF5 = UDF5;
                UDFDAL objUDFApiController = new UDFDAL(this.enterPriseDBName);
                IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetRequiredUDFsByUDFTableNamePlain("PullMaster", SessionHelper.RoomID, SessionHelper.CompanyID);
                string udfRequier = string.Empty;
                
                foreach (var i in DataFromDB)
                {
                        if (i.UDFColumnName == "UDF1"  && string.IsNullOrWhiteSpace(oPullMaster.UDF1))
                        {
                            string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                            string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                            if (!string.IsNullOrEmpty(val))
                                i.UDFDisplayColumnName = val;
                            else
                                i.UDFDisplayColumnName = i.UDFColumnName;
                            udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                        }
                        else if (i.UDFColumnName == "UDF2"  && string.IsNullOrWhiteSpace(oPullMaster.UDF2))
                        {
                            string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                            string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                            if (!string.IsNullOrEmpty(val))
                                i.UDFDisplayColumnName = val;
                            else
                                i.UDFDisplayColumnName = i.UDFColumnName;
                            udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                        }
                        else if (i.UDFColumnName == "UDF3"  && string.IsNullOrWhiteSpace(oPullMaster.UDF3))
                        {
                            string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                            string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                            if (!string.IsNullOrEmpty(val))
                                i.UDFDisplayColumnName = val;
                            else
                                i.UDFDisplayColumnName = i.UDFColumnName;
                            udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                        }
                        else if (i.UDFColumnName == "UDF4"  && string.IsNullOrWhiteSpace(oPullMaster.UDF4))
                        {
                            string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                            string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                            if (!string.IsNullOrEmpty(val))
                                i.UDFDisplayColumnName = val;
                            else
                                i.UDFDisplayColumnName = i.UDFColumnName;
                            udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                        }
                        else if (i.UDFColumnName == "UDF5"  && string.IsNullOrWhiteSpace(oPullMaster.UDF5))
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

                if (!string.IsNullOrEmpty(udfRequier))
                {
                    return Json(new { Message = udfRequier, Status = "fail" }, JsonRequestBehavior.AllowGet);
                }
                oPullMasterDAL.UpdateBillingAndPullOrderNumber(oPullMaster);
            }

            return Json(new { Message = ResMessage.SaveMessage, Status = "OK" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateUDFInPullHistory(string PullGUID, string UDFID, string UDFValue)
        {
            string _returnmessage = ResMessage.SaveMessage;
            string _returnstatus = "OK";
            UDFDAL objUDFDAL = new UDFDAL(this.enterPriseDBName);
            List<UDFDTO> result = objUDFDAL.GetNonDeletedUDFsByUDFTableNamePlain("PullMaster", SessionHelper.RoomID, SessionHelper.CompanyID);
            PullMasterDAL objPullDAL = this.pullMasterDAL; //new PullMasterDAL(this.enterPriseDBName);

            foreach (var i in result)
            {
                if (!string.IsNullOrEmpty(UDFID) && i.UDFColumnName == UDFID)
                {
                    if (i.UDFIsRequired.GetValueOrDefault(false) == true && string.IsNullOrWhiteSpace(UDFValue))
                    {
                        _returnmessage = string.Format(ResPullMaster.ValueRequiredForUDF, UDFID);
                        return Json(new { Message = _returnmessage, Status = "fail" }, JsonRequestBehavior.AllowGet);
                    }

                    objPullDAL.UpdateUDFFromPullHistory(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, PullGUID, UDFID, UDFValue, i.UDFControlType, i.ID, out _returnmessage, out _returnstatus, SessionHelper.EnterPriceID, ResourceHelper.CurrentCult.Name);
                    break;
                }
            }

            return Json(new { Message = _returnmessage, Status = _returnstatus }, JsonRequestBehavior.AllowGet);
        }



        public JsonResult UpdatePullOrderNumberInPullHistory(long? SupplierID, string PullGUID, string PullOrderNumber,bool allowBlank = false)
        {
            string _returnmessage = ResMessage.SaveMessage;
            string _returnstatus = "OK";
            PullMasterDAL objPullDAL = this.pullMasterDAL; //new PullMasterDAL(this.enterPriseDBName);

            if (string.IsNullOrWhiteSpace((PullOrderNumber ?? string.Empty).Trim()))
            {
                SupplierMasterDAL objSupplierDAL = new SupplierMasterDAL(this.enterPriseDBName);
                SupplierMasterDTO objSupplierDTO = objSupplierDAL.GetSupplierByIDPlain(SupplierID.GetValueOrDefault(0));
                if (objSupplierDTO != null && objSupplierDTO.PullPurchaseNumberType != null && objSupplierDTO.PullPurchaseNumberType.Value == 0)
                {
                    if (!allowBlank)
                        return Json(new { Message = ResMessage.PullOrderNumberRequired, Status = "fail" }, JsonRequestBehavior.AllowGet);
                }
            }

            objPullDAL.UpdatePullOrderNumberInPullHistory(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, PullGUID, PullOrderNumber, out _returnmessage, out _returnstatus, SessionHelper.EnterPriceID, ResourceHelper.CurrentCult.Name);

            return Json(new
            {
                Message = _returnmessage,
                Status = _returnstatus
            }, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// GetQLItemForCredit
        /// </summary>
        /// <param name="QLItems"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetQLItemForCredit(List<ItemInfoToCredit> QLItems)
        {
            QuickListDAL QLDAL = null;
            QuickListMasterDTO QLMaster = null;
            List<QuickListDetailDTO> lstQLDetails = null;
            List<ItemInfoToCredit> lstItemsToCredit = null;
            PullMasterDAL pullDAL = this.pullMasterDAL; //new PullMasterDAL(this.enterPriseDBName);
            QLDAL = new QuickListDAL(this.enterPriseDBName);
            lstItemsToCredit = new List<ItemInfoToCredit>();
            int rowID = 50000;

            // RoomDTO objRoomDTO = new RoomDAL(this.enterPriseDBName).GetRoomByIDPlain(SessionHelper.RoomID);
            CommonDAL objCommonDAL = this.commonDAL; //new CommonDAL(this.enterPriseDBName);
            string columnList = "ID,RoomName,IsIgnoreCreditRule";
            RoomDTO objRoomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");

            bool IsIgnoreCreditRule = (objRoomDTO != null ? objRoomDTO.IsIgnoreCreditRule : false);

            foreach (var itemToCredit in QLItems)
            {
                QLMaster = QLDAL.GetRecord(itemToCredit.QLGuid.GetValueOrDefault(Guid.Empty).ToString(), SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                itemToCredit.QuickListName = QLMaster.Name;
                var tmpsupplierIds = new List<long>();
                lstQLDetails = QLDAL.GetQuickListItemsRecords(SessionHelper.RoomID, SessionHelper.CompanyID, QLMaster.GUID.ToString(), tmpsupplierIds);

                if (lstQLDetails != null && lstQLDetails.Count > 0)
                {
                    itemToCredit.PrevPullsToCredit = new List<PullDetailToCredit>();
                    foreach (var item in lstQLDetails)
                    {
                        rowID = rowID + 1;

                        if (IsIgnoreCreditRule)
                        {
                            ItemInfoToCredit itmInfo = new ItemInfoToCredit()
                            {
                                ItemGuid = item.ItemGUID,
                                Quantity = item.Quantity.GetValueOrDefault(0) * itemToCredit.Quantity,
                                ItemQtyInQL = item.DefaultPullQuantity.GetValueOrDefault(0),
                                CreditQLQty = itemToCredit.Quantity,
                                ItemNumber = item.ItemNumber,
                                ItemType = item.ItemType,
                                UDF1 = itemToCredit.UDF1,
                                UDF2 = itemToCredit.UDF2,
                                UDF3 = itemToCredit.UDF3,
                                UDF4 = itemToCredit.UDF4,
                                UDF5 = itemToCredit.UDF5,
                                ProjectName = itemToCredit.ProjectName,
                                PullOrderNumber = itemToCredit.PullOrderNumber,
                                Bin = item.BinName,
                                ItemTracking = itemToCredit.ItemTracking,
                                QuickListName = QLMaster.Name,
                                IsModelShow = itemToCredit.IsModelShow,
                                RowID = rowID,
                                QLGuid = itemToCredit.QLGuid,
                                WOGuid = itemToCredit.WOGuid,
                                SupplierAccountGuid = itemToCredit.SupplierAccountGuid
                            };

                            if (item.LotNumberTracking)
                            {
                                itmInfo.ItemTracking = "LOTTRACK";
                            }
                            else if (item.SerialNumberTracking)
                            {
                                itmInfo.ItemTracking = "SERIALTRACK";
                            }
                            else if (item.DateCodeTracking)
                            {
                                itmInfo.ItemTracking = "DATECODETRACK";
                            }
                            else
                            {
                                itmInfo.ItemTracking = "";
                            }

                            //List<PullDetailToCredit> lstPullDetails = pullDAL.GetPreviousPulls(itmInfo, SessionHelper.RoomID, SessionHelper.CompanyID, (long)SessionHelper.UserID);
                            //if (lstPullDetails != null && lstPullDetails.Count > 0)
                            //{
                            //    foreach (var pldtl in lstPullDetails)
                            //    {
                            //        itemToCredit.PrevPullsToCredit.Add(pldtl);
                            //    }
                            //}
                            //else
                            //{
                            //    itmInfo.ErrorMessage = "Item have not any pull available to credit.";
                            //    itmInfo.IsModelShow = true;
                            //}

                            lstItemsToCredit.Add(itmInfo);
                        }
                        else
                        {
                            ItemInfoToCredit itmInfo = new ItemInfoToCredit()
                            {
                                ItemGuid = item.ItemGUID,
                                Quantity = item.Quantity.GetValueOrDefault(0) * itemToCredit.Quantity,
                                ItemQtyInQL = item.Quantity.GetValueOrDefault(0),
                                CreditQLQty = itemToCredit.Quantity,
                                ItemNumber = item.ItemNumber,
                                ItemType = item.ItemType,
                                UDF1 = itemToCredit.UDF1,
                                UDF2 = itemToCredit.UDF2,
                                UDF3 = itemToCredit.UDF3,
                                UDF4 = itemToCredit.UDF4,
                                UDF5 = itemToCredit.UDF5,
                                ProjectName = itemToCredit.ProjectName,
                                PullOrderNumber = itemToCredit.PullOrderNumber,
                                Bin = itemToCredit.Bin,
                                ItemTracking = itemToCredit.ItemTracking,
                                QuickListName = QLMaster.Name,
                                IsModelShow = itemToCredit.IsModelShow,
                                RowID = rowID,
                                QLGuid = itemToCredit.QLGuid,
                                WOGuid = itemToCredit.WOGuid,
                                SupplierAccountGuid = itemToCredit.SupplierAccountGuid
                            };

                            if (item.LotNumberTracking)
                            {
                                itmInfo.ItemTracking = "LOTTRACK";
                            }
                            else if (item.SerialNumberTracking)
                            {
                                itmInfo.ItemTracking = "SERIALTRACK";
                            }
                            else if (item.DateCodeTracking)
                            {
                                itmInfo.ItemTracking = "DATECODETRACK";
                            }
                            else
                            {
                                itmInfo.ItemTracking = "";
                            }

                            List<PullDetailToCredit> lstPullDetails = pullDAL.GetPreviousPulls(itmInfo, SessionHelper.RoomID, SessionHelper.CompanyID, (long)SessionHelper.UserID,SessionHelper.EnterPriceID);
                            if (lstPullDetails != null && lstPullDetails.Count > 0)
                            {
                                foreach (var pldtl in lstPullDetails)
                                {
                                    itemToCredit.PrevPullsToCredit.Add(pldtl);
                                }
                            }
                            else
                            {
                                itmInfo.ErrorMessage = ResPullMaster.ItemDontHavePullToCredit; 
                                itmInfo.IsModelShow = true;
                            }

                            lstItemsToCredit.Add(itmInfo);
                        }
                    }
                }
            }
            return Json(new { Message = "OK", Status = true, QLItems = lstItemsToCredit }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Fill Pre Credit Info
        /// </summary>
        /// <param name="itemToCredit"></param>
        /// <returns></returns>
        public ActionResult FillPreCreditInfo(ItemInfoToCredit itemToCredit)
        {
            try
            {
                PullMasterDAL pullDAL = this.pullMasterDAL; //new PullMasterDAL(this.enterPriseDBName);
                if (itemToCredit.Quantity > 0 && itemToCredit.ItemType != 2 && itemToCredit.ItemTracking != "QUICKLIST")
                {
                    itemToCredit.PrevPullsToCredit = pullDAL.GetPreviousPulls(itemToCredit, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID,SessionHelper.EnterPriceID);
                }
                return PartialView("_PrefillForCredit", itemToCredit);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
        }


        /// <summary>
        /// Fill MS Credit
        /// </summary>
        /// <param name="itemToCredit"></param>
        /// <returns></returns>
        public ActionResult FillPreMSCreditInfo(ItemInfoToMSCredit itemToCredit)
        {
            try
            {
                PullMasterDAL pullDAL = this.pullMasterDAL; //new PullMasterDAL(this.enterPriseDBName);
                if (itemToCredit.Quantity > 0 && itemToCredit.ItemType != 2 && itemToCredit.ItemTracking != "QUICKLIST")
                {
                    itemToCredit.PrevPullsToMSCredit = pullDAL.GetPrevLoadMSPull(itemToCredit, SessionHelper.RoomID, SessionHelper.CompanyID,SessionHelper.EnterPriceID,ResourceHelper.CurrentCult.Name);
                }

                return PartialView("_PrefillForMSCredit", itemToCredit);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
        }

        /// <summary>
        /// Save Pull Credit
        /// </summary>
        /// <param name="pullDetails"></param>
        /// <returns></returns>
        public ActionResult SavePullCredit(List<ItemInfoToCredit> CreditDetails)
        {
            UDFDAL objUDFApiController = new UDFDAL(this.enterPriseDBName);
            IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetRequiredUDFsByUDFTableNamePlain("PullMaster", SessionHelper.RoomID, SessionHelper.CompanyID);
            string udfRequier = string.Empty;
            PullMasterDAL pullDAL = this.pullMasterDAL; //new PullMasterDAL(this.enterPriseDBName);
            //   RoomDTO objRoomDTO = new RoomDAL(this.enterPriseDBName).GetRoomByIDPlain(SessionHelper.RoomID);
            CommonDAL objCommonDAL = this.commonDAL; //new CommonDAL(this.enterPriseDBName);
            string columnList = "ID,RoomName,IsIgnoreCreditRule,IsProjectSpendMandatory";
            RoomDTO objRoomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");
            string message = string.Empty;
            string ErrorMessage = string.Empty;
            ProjectMasterDAL objPrjDAL = new ProjectMasterDAL(this.enterPriseDBName);

            foreach (var item in CreditDetails)
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

                if (!string.IsNullOrEmpty(udfRequier))
                {
                    return Json(new { Message = udfRequier, Status = "Fail" }, JsonRequestBehavior.AllowGet);
                }

                #region Project Spend validation
                if (item.ItemType != 4 && objRoomDTO != null && objRoomDTO.IsProjectSpendMandatory)
                {
                    if (string.IsNullOrEmpty(item.ProjectName))
                    {
                        message += item.ItemNumber + ": " + ResPullMaster.ProjectSpendMandatorySelectIt + Environment.NewLine; 

                    }
                }

                
                bool HasOnTheFlyEntryRight = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.OnTheFlyEntry);
                bool IsProjectSpendInsertAllow = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ProjectMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                if (((item.ProjectSpendGUID == Guid.Empty || item.ProjectSpendGUID == null) && !string.IsNullOrEmpty(item.ProjectName)) && (HasOnTheFlyEntryRight == false || IsProjectSpendInsertAllow == false))
                {
                    if (!string.IsNullOrEmpty(item.ProjectName))
                    {
                        ProjectMasterDTO objProjectDTO = objPrjDAL.GetProjectspendByName(item.ProjectName.Trim(), SessionHelper.RoomID, SessionHelper.CompanyID, false, false, null);
                        if (objProjectDTO == null)
                            message += item.ItemNumber +": " + ResPullMaster.NoProjectspendOntheFlyRight + Environment.NewLine;
                    }
                    else
                    {
                        message += item.ItemNumber + ": " + ResPullMaster.NoProjectspendOntheFlyRight + Environment.NewLine;
                    }
                }
                else if (((item.ProjectSpendGUID != Guid.Empty && item.ProjectSpendGUID != null) && !string.IsNullOrEmpty(item.ProjectName)) && (HasOnTheFlyEntryRight == false || IsProjectSpendInsertAllow == false))
                {

                    ProjectMasterDTO objProjectDTO = objPrjDAL.GetProjectMasterByGuidNormal(item.ProjectSpendGUID.Value);
                    if (objProjectDTO != null)
                    {
                        if (objProjectDTO.ProjectSpendName.Trim().ToLower() != item.ProjectName.Trim().ToLower())
                            message += item.ItemNumber + ": " + ResPullMaster.NoProjectspendOntheFlyRight + Environment.NewLine;
                    }
                }
                
                #endregion
            }

            if (!string.IsNullOrEmpty(message))
            {
                return Json(new { Message = message, Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }

            List<ItemLocationDetailsDTO> itemLocations = null;
            ItemLocationDetailsDAL ildDAL = new ItemLocationDetailsDAL(this.enterPriseDBName);
            ItemMasterDAL objItemDAL = new ItemMasterDAL(this.enterPriseDBName);
            List<Guid> ItemGuids = new List<Guid>();
            var enterpriseId = SessionHelper.EnterPriceID;

            if (CreditDetails != null && CreditDetails.Count > 0)
            {
                foreach (var item in CreditDetails)
                {
                    if (item.ItemTracking == "SERIALTRACK")
                    {
                        string lstSerialNumber = string.Empty;
                        foreach (var SerialItem in item.PrevPullsToCredit)
                        {
                            CommonDAL objCommonDal = this.commonDAL; //new CommonDAL(this.enterPriseDBName);
                            string serailErrorMessage = objCommonDal.CheckDuplicateSerialNumbers(SerialItem.Serial, 0, SessionHelper.RoomID, SessionHelper.CompanyID, item.ItemGuid.GetValueOrDefault(Guid.Empty));

                            if (serailErrorMessage.ToLower().Trim() == "duplicate")
                            {
                                if (lstSerialNumber != string.Empty)
                                    lstSerialNumber = lstSerialNumber + " , " + SerialItem.Serial;
                                else
                                    lstSerialNumber = SerialItem.Serial;
                            }
                        }

                        if (lstSerialNumber != string.Empty)
                        {
                            return Json(new { Message = ResPullMaster.CreditTransactionDoneForSerial + " " + lstSerialNumber, Status = false }, JsonRequestBehavior.AllowGet);

                            //item.PrevPullsToCredit = item.PrevPullsToCredit.Where(x => !lstSerialNumber.Contains(x.Serial.ToLower().Trim())).ToList();
                        }
                    }
                    else if (item.ItemTracking == "LOTTRACK" || item.ItemTracking == "DATECODETRACK")
                    {
                        var itemDetail = objItemDAL.GetItemByGuidPlain(item.ItemGuid.GetValueOrDefault(Guid.Empty),SessionHelper.RoomID,SessionHelper.CompanyID);

                        if (itemDetail != null && itemDetail.LotNumberTracking && itemDetail.DateCodeTracking)
                        {
                            string lstLotNumber = string.Empty;
                            string lotDuplicationMsg = string.Empty;

                            foreach (var lotItem in item.PrevPullsToCredit)
                            {
                                CommonDAL objCommonDal = this.commonDAL; //new CommonDAL(this.enterPriseDBName);
                                DateTime ExpDate = DateTime.MinValue;
                                DateTime.TryParseExact(lotItem.ExpireDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture, DateTimeStyles.None, out ExpDate);
                                string Expiration = ExpDate.ToString("MM/dd/yyyy");

                                if (ExpDate != DateTime.MinValue)
                                {
                                    string msg = objCommonDal.CheckDuplicateLotAndExpiration(lotItem.Lot, Expiration, ExpDate, 0, SessionHelper.RoomID, SessionHelper.CompanyID, item.ItemGuid.GetValueOrDefault(Guid.Empty), SessionHelper.UserID, SessionHelper.EnterPriceID);

                                    if (!(string.IsNullOrWhiteSpace(msg) || (msg ?? string.Empty).ToLower() == "ok"))
                                    {
                                        lotDuplicationMsg += msg;

                                        if (lstLotNumber != string.Empty)
                                            lstLotNumber = lstLotNumber + " , " + lotItem.Lot;
                                        else
                                            lstLotNumber = lotItem.Lot;
                                    }
                                }
                            }

                            if (lstLotNumber != string.Empty)
                            {
                                return Json(new { Message = lotDuplicationMsg, Status = false }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        
                    }
                    /* WI-4693-Allow specified rooms to ignore credit rules */

                    if (!objRoomDTO.IsIgnoreCreditRule)
                    {
                        List<PullDetailsDTO> prepulls = pullDAL.GetPrevPull(item, SessionHelper.RoomID, SessionHelper.CompanyID);
                        itemLocations = new List<ItemLocationDetailsDTO>();
                        foreach (var prePullItem in prepulls)
                        {
                            if (prePullItem.SerialNumber != null && (!string.IsNullOrWhiteSpace(prePullItem.SerialNumber)))
                            {
                                if (ildDAL.CheckSerialExistsOrNot(prePullItem.SerialNumber, item.ItemGuid ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID))
                                {
                                    itemLocations.Add(pullDAL.ConvertPullDetailtoItemLocationDetail(prePullItem, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.RoomDateFormat));
                                }
                            }
                            else
                            {
                                itemLocations.Add(pullDAL.ConvertPullDetailtoItemLocationDetail(prePullItem, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.RoomDateFormat));
                            }
                        }

                        List<CreditHistory> lstCreditGuids = new List<CreditHistory>();
                        long SessionUserId = SessionHelper.UserID;
                        if (ildDAL.ItemLocationDetailsSaveForCreditPullnew(itemLocations, "Credit", SessionHelper.RoomDateFormat, out lstCreditGuids, SessionUserId,enterpriseId, "credit", ""))
                        {
                            pullDAL.UpdatePullRecordsForCreditQuantity(prepulls, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, lstCreditGuids, "");
                            if (item != null && item.WOGuid.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                            {
                                WorkOrderLineItemsDAL objWOLDAL = new WorkOrderLineItemsDAL(this.enterPriseDBName);
                                objWOLDAL.UpdateWOItemAndTotalCost(item.WOGuid.GetValueOrDefault(Guid.Empty).ToString(), SessionHelper.RoomID, SessionHelper.CompanyID);
                            }
                        }
                    }
                    else
                    {
                        #region WI-4693-Allow specified rooms to ignore credit rules

                        List<PullDetailsDTO> prepulls = pullDAL.GetPrevPull(item, SessionHelper.RoomID, SessionHelper.CompanyID);

                        double TotalAvailablePulls = prepulls.Sum(x => (x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0)));
                        double TotalRemainingCredit = (item.Quantity - TotalAvailablePulls);
                        List<PullDetailsDTO> pulls = new List<PullDetailsDTO>();
                        if (TotalRemainingCredit > 0)
                        {
                            pulls = pullDAL.GetPrevPullForCreditEntry(prepulls, item, TotalRemainingCredit, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.RoomDateFormat, SessionHelper.EnterPriceID, ResourceHelper.CurrentCult.Name);
                            prepulls.AddRange(pulls);
                        }
                        bool IsValid = true;
                        if (prepulls.Where(x => x.EditedFrom == "Fail").Count() > 0)
                        {
                            IsValid = false;
                        }

                        if (IsValid)
                        {
                            itemLocations = new List<ItemLocationDetailsDTO>();
                            foreach (var prePullItem in prepulls)
                            {
                                itemLocations.Add(pullDAL.ConvertPullDetailtoItemLocationDetailForCreditRule(prePullItem, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.RoomDateFormat));
                            }
                            long SessionUserId = SessionHelper.UserID;
                            List<CreditHistory> lstCreditGuids = new List<CreditHistory>();
                            if (ildDAL.ItemLocationDetailsSaveForCreditPullnew(itemLocations, "Credit", SessionHelper.RoomDateFormat, out lstCreditGuids, SessionUserId,enterpriseId, "credit"))
                            {
                                pullDAL.UpdatePullRecordsForCreditQuantity(prepulls, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, lstCreditGuids);
                                if (pulls != null && pulls.Count > 0)
                                {
                                    List<PullDetailsDTO> lstPulls = new List<PullDetailsDTO>();
                                    lstPulls.AddRange(pulls);
                                    pullDAL.InsertintoCreditHistory(lstPulls, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "Pull Credit");
                                }
                                if (item != null && item.WOGuid.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                                {
                                    WorkOrderLineItemsDAL objWOLDAL = new WorkOrderLineItemsDAL(this.enterPriseDBName);
                                    objWOLDAL.UpdateWOItemAndTotalCost(item.WOGuid.GetValueOrDefault(Guid.Empty).ToString(), SessionHelper.RoomID, SessionHelper.CompanyID);
                                }

                                QuickBookItemDAL objQBItemDAL = new QuickBookItemDAL(SessionHelper.EnterPriseDBName);
                                List<Guid> lstItemGUID = itemLocations.Select(x => x.ItemGUID.GetValueOrDefault(Guid.Empty)).Distinct().ToList();
                                //if (lstItemGUID != null && lstItemGUID.Count > 0)
                                //{
                                //    foreach (Guid itemGuid in lstItemGUID)
                                //    {
                                //        objQBItemDAL.InsertQuickBookItem(itemGuid, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, "Update", false, SessionHelper.UserID, "Web", null, "Pull Credit");
                                //    }
                                //}

                            }
                        }
                        else
                        {
                            ErrorMessage = ErrorMessage + prepulls[0].AddedFrom + Environment.NewLine;
                        }
                        #endregion
                    }

                    if (ItemGuids.IndexOf(item.ItemGuid.GetValueOrDefault(Guid.Empty)) < 0)
                        ItemGuids.Add(item.ItemGuid.GetValueOrDefault(Guid.Empty));

                    // #region "Update Ext Cost And Avg Cost"
                    //new ItemMasterDAL(this.enterPriseDBName).GetAndUpdateExtCostAndAvgCost(item.ItemGuid.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                    // #endregion
                }

                if (objRoomDTO.IsIgnoreCreditRule && !string.IsNullOrEmpty(ErrorMessage))
                    return Json(new { Message = ErrorMessage, Status = false }, JsonRequestBehavior.AllowGet);


                #region "Update Ext Cost And Avg Cost"
                if (ItemGuids.Count > 0)
                {
                    long SessionUserId = SessionHelper.UserID;
                    // objItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                    //objItemDAL = new ItemMasterDAL(this.enterPriseDBName);
                    foreach (var guid in ItemGuids)
                    {
                        objItemDAL.UpdateItemCost(guid, SessionHelper.RoomID, SessionHelper.CompanyID, "Web", SessionUserId,enterpriseId);
                        objItemDAL.GetAndUpdateExtCostAndAvgCost(guid, SessionHelper.RoomID, SessionHelper.CompanyID);
                    }
                    objItemDAL = null;
                }
                #endregion


            }
            return Json(new { Message = ResPullMaster.ItemCredited, Status = true, RetData = ItemGuids }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveCreditEditableUDF(List<ItemInfoToCredit> CreditDetails)
        {
            UDFDAL objUDFApiController = new UDFDAL(this.enterPriseDBName);
            UDFController objUDFController = new UDFController();
            IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetNonDeletedUDFsByUDFTableNamePlain("PullMaster", SessionHelper.RoomID, SessionHelper.CompanyID);
            string udfRequier = string.Empty;
            string message = string.Empty;
            string ErrorMessage = string.Empty;
            foreach (var item in CreditDetails)
            {
                foreach (var i in DataFromDB)
                {
                    if (i.UDFControlType.ToLower().Contains("dropdown editable"))
                    {
                        if (i.UDFColumnName == "UDF1" && !string.IsNullOrWhiteSpace(item.UDF1))
                        {
                            objUDFController.InsertUDFOption(i.ID, item.UDF1, "PullMaster", null, false, false, "");
                        }
                        else if (i.UDFColumnName == "UDF2" && !string.IsNullOrWhiteSpace(item.UDF2))
                        {
                            objUDFController.InsertUDFOption(i.ID, item.UDF2, "PullMaster", null, false, false, "");
                        }
                        else if (i.UDFColumnName == "UDF3" && !string.IsNullOrWhiteSpace(item.UDF3))
                        {
                            objUDFController.InsertUDFOption(i.ID, item.UDF3, "PullMaster", null, false, false, "");
                        }
                        else if (i.UDFColumnName == "UDF4" && !string.IsNullOrWhiteSpace(item.UDF4))
                        {
                            objUDFController.InsertUDFOption(i.ID, item.UDF4, "PullMaster", null, false, false, "");
                        }
                        else if (i.UDFColumnName == "UDF5" && !string.IsNullOrWhiteSpace(item.UDF5))
                        {
                            objUDFController.InsertUDFOption(i.ID, item.UDF5, "PullMaster", null, false, false, "");
                        }
                    }
                }
            }
            return Json(new { Message = "Success", Status = true }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Save Pull for MS Credit
        /// </summary>
        /// <param name="pullDetails"></param>
        /// <returns></returns>
        public ActionResult SavePullMSCredit(List<ItemInfoToMSCredit> CreditDetails)
        {
            UDFDAL objUDFApiController = new UDFDAL(this.enterPriseDBName);
            IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetRequiredUDFsByUDFTableNamePlain("PullMaster", SessionHelper.RoomID, SessionHelper.CompanyID);
            string udfRequier = string.Empty;
            PullMasterDAL pullDAL = this.pullMasterDAL; //new PullMasterDAL(this.enterPriseDBName);
            MaterialStagingPullDetailDAL objMSDAL = new MaterialStagingPullDetailDAL(this.enterPriseDBName);
            //RoomDTO objRoomDTO = new RoomDAL(this.enterPriseDBName).GetRoomByIDPlain(SessionHelper.RoomID);
            CommonDAL objCommonDAL = this.commonDAL; //new CommonDAL(this.enterPriseDBName);
            string columnList = "ID,RoomName,IsIgnoreCreditRule,IsProjectSpendMandatory";
            RoomDTO objRoomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");
            ProjectMasterDAL objPrjDAL = new ProjectMasterDAL(this.enterPriseDBName);

            string message = string.Empty;
            string ErrorMessage = string.Empty;
            foreach (var item in CreditDetails)
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

                if (!string.IsNullOrEmpty(udfRequier))
                {
                    return Json(new { Message = udfRequier, Status = "Fail" }, JsonRequestBehavior.AllowGet);
                }

                #region Project Spend validation
                if (item.ItemType != 4 && objRoomDTO != null && objRoomDTO.IsProjectSpendMandatory)
                {
                    if (string.IsNullOrEmpty(item.ProjectName))
                    {
                        message += item.ItemNumber + ": " + ResPullMaster.ProjectSpendMandatorySelectIt + Environment.NewLine;
                    }
                }
                
                bool HasOnTheFlyEntryRight = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.OnTheFlyEntry);
                bool IsProjectSpendInsertAllow = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ProjectMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                if (!string.IsNullOrEmpty(item.ProjectName) && (HasOnTheFlyEntryRight == false || IsProjectSpendInsertAllow == false))
                {
                    ProjectMasterDTO objProjectDTO = objPrjDAL.GetProjectspendByName(item.ProjectName.Trim(), SessionHelper.RoomID, SessionHelper.CompanyID, false, false, null);
                    if (objProjectDTO == null)
                        message += item.ItemNumber + ": " + ResPullMaster.NoProjectspendOntheFlyRight + Environment.NewLine;
                }
                
                #endregion
            }

            if (!string.IsNullOrEmpty(message))
            {
                return Json(new { Message = message, Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }

            List<MaterialStagingPullDetailDTO> MSData = null;
            ItemLocationDetailsDAL ildDAL = new ItemLocationDetailsDAL(this.enterPriseDBName);
            List<Guid> ItemGuids = new List<Guid>();
            long SessionUserId = SessionHelper.UserID;
            if (CreditDetails != null && CreditDetails.Count > 0)
            {
                foreach (var item in CreditDetails)
                {
                    if (item.ItemTracking == "SERIALTRACK")
                    {
                        string lstSerialNumber = string.Empty;

                        foreach (var SerialItem in item.PrevPullsToMSCredit)
                        {
                            CommonDAL objCommonDal = this.commonDAL; //new CommonDAL(this.enterPriseDBName);
                            string serailErrorMessage = objCommonDal.CheckDuplicateSerialNumbers(SerialItem.Serial, 0, SessionHelper.RoomID, SessionHelper.CompanyID, item.ItemGuid.GetValueOrDefault(Guid.Empty));

                            if (serailErrorMessage.ToLower().Trim() == "duplicate")
                            {
                                if (lstSerialNumber != string.Empty)
                                    lstSerialNumber = lstSerialNumber + " , " + SerialItem.Serial;
                                else
                                    lstSerialNumber = SerialItem.Serial;
                            }
                        }

                        if (lstSerialNumber != string.Empty)
                        {
                            return Json(new { Message = ResPullMaster.CreditTransactionDoneForSerial + " " + lstSerialNumber, Status = false }, JsonRequestBehavior.AllowGet);
                            // item.PrevPullsToMSCredit = item.PrevPullsToMSCredit.Where(x => !lstSerialNumber.Contains(x.Serial.ToLower().Trim())).ToList();
                        }
                    }

                    /* WI-4693-Allow specified rooms to ignore credit rules */

                    if (!objRoomDTO.IsIgnoreCreditRule)
                    {
                        List<PullDetailsDTO> prepulls = pullDAL.GetPrevMSPull(item, SessionHelper.RoomID, SessionHelper.CompanyID);
                        MSData = new List<MaterialStagingPullDetailDTO>();
                        foreach (var prePullItem in prepulls)
                        {
                            if (item.Bin == null && string.IsNullOrEmpty(item.Bin))
                            {
                                prePullItem.CreditBinName = "[|EmptyStagingBin|]";
                                //prePullItem.BinName = "[|EmptyStagingBin|]";
                            }
                            if (prePullItem.SerialNumber != null && (!string.IsNullOrWhiteSpace(prePullItem.SerialNumber)))
                            {
                                if (objMSDAL.CheckSerialExistsOrNot(prePullItem.SerialNumber, item.ItemGuid ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID))
                                {
                                    MSData.Add(pullDAL.ConvertPullDetailtoMaterialStaginPullDetail(prePullItem, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.RoomDateFormat));
                                }
                            }
                            else
                            {
                                MSData.Add(pullDAL.ConvertPullDetailtoMaterialStaginPullDetail(prePullItem, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.RoomDateFormat));
                            }
                        }

                        List<CreditHistory> lstCreditGuids = new List<CreditHistory>();

                        if (ildDAL.MaterialStagingPoolDetailsSaveForMSCreditPullnew(MSData, "MS Credit", SessionHelper.RoomDateFormat, out lstCreditGuids, SessionUserId,SessionHelper.EnterPriceID,"ms credit"))
                        {
                            pullDAL.UpdatePullRecordsForMSCreditQuantity(prepulls, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, lstCreditGuids);
                            if (item != null && item.WOGuid.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                            {
                                WorkOrderLineItemsDAL objWOLDAL = new WorkOrderLineItemsDAL(this.enterPriseDBName);
                                objWOLDAL.UpdateWOItemAndTotalCost(item.WOGuid.GetValueOrDefault(Guid.Empty).ToString(), SessionHelper.RoomID, SessionHelper.CompanyID);
                            }
                        }
                    }
                    else
                    {
                        #region WI-4693-Allow specified rooms to ignore credit rules

                        List<PullDetailsDTO> prepulls = pullDAL.GetPrevMSPull(item, SessionHelper.RoomID, SessionHelper.CompanyID);

                        double TotalAvailablePulls = prepulls.Sum(x => (x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0)));
                        double TotalRemainingCredit = (item.Quantity - TotalAvailablePulls);
                        List<PullDetailsDTO> pulls = new List<PullDetailsDTO>();
                        if (TotalRemainingCredit > 0)
                        {
                            pulls = pullDAL.GetPrevMSPullForCreditEntry(prepulls, item, TotalRemainingCredit, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.RoomDateFormat,SessionHelper.CurrentTimeZone,SessionHelper.EnterPriceID,ResourceHelper.CurrentCult.Name);
                            prepulls.AddRange(pulls);
                        }
                        bool IsValid = true;

                        if (prepulls.Where(x => x.EditedFrom == "Fail").Count() > 0)
                        {
                            IsValid = false;
                        }

                        if (IsValid)
                        {
                            MSData = new List<MaterialStagingPullDetailDTO>();
                            foreach (var prePullItem in prepulls)
                            {
                                if (item.Bin == null && string.IsNullOrEmpty(item.Bin))
                                {
                                    prePullItem.CreditBinName = "[|EmptyStagingBin|]";
                                }

                                MSData.Add(pullDAL.ConvertPullDetailtoMaterialStaginPullDetailForCreditRule(prePullItem, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.RoomDateFormat, item.QLGuid.GetValueOrDefault(Guid.Empty)));
                            }
                            List<CreditHistory> lstCreditGuids = new List<CreditHistory>();

                            if (ildDAL.MaterialStagingPoolDetailsSaveForMSCreditPullnew(MSData, "MS Credit", SessionHelper.RoomDateFormat, out lstCreditGuids, SessionUserId,SessionHelper.EnterPriceID, "ms credit"))
                            {
                                pullDAL.UpdatePullRecordsForMSCreditQuantity(prepulls, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, lstCreditGuids);
                                if (pulls != null && pulls.Count > 0)
                                {
                                    List<PullDetailsDTO> lstPulls = new List<PullDetailsDTO>();
                                    lstPulls.AddRange(pulls);
                                    pullDAL.InsertintoCreditHistory(lstPulls, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "MS Pull Credit");
                                }

                                if (item != null && item.WOGuid.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                                {
                                    WorkOrderLineItemsDAL objWOLDAL = new WorkOrderLineItemsDAL(this.enterPriseDBName);
                                    objWOLDAL.UpdateWOItemAndTotalCost(item.WOGuid.GetValueOrDefault(Guid.Empty).ToString(), SessionHelper.RoomID, SessionHelper.CompanyID);
                                }
                            }
                        }
                        else
                        {
                            ErrorMessage = ErrorMessage + prepulls[0].AddedFrom + Environment.NewLine;
                        }

                        #endregion
                    }

                    if (ItemGuids.IndexOf(item.ItemGuid.GetValueOrDefault(Guid.Empty)) < 0)
                        ItemGuids.Add(item.ItemGuid.GetValueOrDefault(Guid.Empty));

                    // #region "Update Ext Cost And Avg Cost"
                    //new ItemMasterDAL(this.enterPriseDBName).GetAndUpdateExtCostAndAvgCost(item.ItemGuid.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                    // #endregion
                }

                if (objRoomDTO.IsIgnoreCreditRule && !string.IsNullOrEmpty(ErrorMessage))
                    return Json(new { Message = ErrorMessage, Status = false }, JsonRequestBehavior.AllowGet);

                //#region "Update Ext Cost And Avg Cost"
                //if (ItemGuids.Count > 0)
                //{
                //    objItemDAL = new ItemMasterDAL(this.enterPriseDBName);
                //    foreach (var guid in ItemGuids)
                //    {
                //        objItemDAL.UpdateItemCost(guid, SessionHelper.RoomID, SessionHelper.CompanyID, "Web");
                //        objItemDAL.GetAndUpdateExtCostAndAvgCost(guid, SessionHelper.RoomID, SessionHelper.CompanyID);
                //    }
                //    objItemDAL = null;
                //}
                //#endregion


            }

            return Json(new { Message = ResPullMaster.ItemMSCredited, Status = true, RetData = ItemGuids }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveMSCreditEditableUDF(List<ItemInfoToMSCredit> CreditDetails)
        {
            UDFDAL objUDFApiController = new UDFDAL(this.enterPriseDBName);
            UDFController objUDFController = new UDFController();
            IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetNonDeletedUDFsByUDFTableNamePlain("PullMaster", SessionHelper.RoomID, SessionHelper.CompanyID);
            string udfRequier = string.Empty;
            string message = string.Empty;
            string ErrorMessage = string.Empty;
            foreach (var item in CreditDetails)
            {
                foreach (var i in DataFromDB)
                {
                    if (i.UDFControlType.ToLower().Contains("dropdown editable"))
                    {
                        if (i.UDFColumnName == "UDF1" && !string.IsNullOrWhiteSpace(item.UDF1))
                        {
                            objUDFController.InsertUDFOption(i.ID, item.UDF1, "PullMaster", null, false, false, "");
                        }
                        else if (i.UDFColumnName == "UDF2" && !string.IsNullOrWhiteSpace(item.UDF2))
                        {
                            objUDFController.InsertUDFOption(i.ID, item.UDF2, "PullMaster", null, false, false, "");
                        }
                        else if (i.UDFColumnName == "UDF3" && !string.IsNullOrWhiteSpace(item.UDF3))
                        {
                            objUDFController.InsertUDFOption(i.ID, item.UDF3, "PullMaster", null, false, false, "");
                        }
                        else if (i.UDFColumnName == "UDF4" && !string.IsNullOrWhiteSpace(item.UDF4))
                        {
                            objUDFController.InsertUDFOption(i.ID, item.UDF4, "PullMaster", null, false, false, "");
                        }
                        else if (i.UDFColumnName == "UDF5" && !string.IsNullOrWhiteSpace(item.UDF5))
                        {
                            objUDFController.InsertUDFOption(i.ID, item.UDF5, "PullMaster", null, false, false, "");
                        }
                    }
                }
            }
            return Json(new { Message = "Success", Status = true }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Get PullOrderNumber For NewPull Grid
        /// </summary>
        /// <param name="ItemGuid"></param>
        /// <param name="NameStartWith"></param>
        /// <returns></returns>
        public JsonResult GetPullOrderNumberForNewPullGrid(string NameStartWith)
        {
            Int64 RoomID = SessionHelper.RoomID;
            Int64 CompanyID = SessionHelper.CompanyID;
            List<DTOForAutoComplete> returnKeyValList = new List<DTOForAutoComplete>();
            try
            {
                returnKeyValList = this.pullMasterDAL.GetPullOrderNumberForNewPullGrid(NameStartWith, RoomID, CompanyID);  //new PullMasterDAL(this.enterPriseDBName).GetPullOrderNumberForNewPullGrid(NameStartWith, RoomID, CompanyID);
                return Json(returnKeyValList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(returnKeyValList, JsonRequestBehavior.AllowGet);
            }
            finally
            {

            }
        }




        [HttpPost]
        public JsonResult ClearPullSession()
        {
            Session["PullList"] = null;
            return Json(true);
        }

        /// <summary>
        /// ItemsToCredit
        /// </summary>
        /// <param name="lstItems"></param>
        /// <returns></returns>
        //[HttpPost]
        //public JsonResult ItemsToCredit(List<ItemInfoToCredit> lstItems)
        //{
        //    ItemLocationDetailsDAL ILDAL = null;
        //    try
        //    {
        //        string strReturnMassage = string.Empty;
        //        ILDAL = new ItemLocationDetailsDAL(this.enterPriseDBName);
        //        if (lstItems != null)
        //        {
        //            foreach (var item in lstItems)
        //            {
        //                if (item.ItemType == 2 && item.QLGuid.HasValue)
        //                {
        //                    strReturnMassage += QuaickListCredit(item);
        //                }
        //                else if (item.ItemGuid.HasValue)
        //                {
        //                    List<ItemLocationDetailsDTO> itmLocations = ItemCredit(item);
        //                    ILDAL.ItemLocationDetailsSaveForCreditPull(itmLocations, "Pull Credit", "credit");
        //                }
        //                else
        //                {
        //                    strReturnMassage += "item id not exist";
        //                }
        //            }
        //        }

        //        return Json(new { Message = "Item Credited", Status = true }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { Message = ex.Message, Status = false }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        /// <summary>
        /// ItemCredit
        /// </summary>
        /// <param name="itm"></param>
        /// <returns></returns>
        //private List<ItemLocationDetailsDTO> ItemCredit(ItemInfoToCredit itm)
        //{
        //    List<ItemLocationDetailsDTO> itmlocatoinDetails = new List<ItemLocationDetailsDTO>();
        //    BinMasterDAL binDAL = new BinMasterDAL(this.enterPriseDBName);
        //    ItemMasterDAL itmDAL = new ItemMasterDAL(this.enterPriseDBName);
        //    PullDetailsDAL pullDetailDAL = new PullDetailsDAL(this.enterPriseDBName);
        //    Int64? BinID = binDAL.GetOrInsertBinIDByName(itm.ItemGuid.GetValueOrDefault(Guid.Empty), itm.Bin, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID);

        //    IEnumerable<PullDetailsDTO> lstPullDetailDTO = pullDetailDAL.GetPullDetailsByItemGuid(itm.ItemGuid.GetValueOrDefault(Guid.Empty));
        //    lstPullDetailDTO = (from x in lstPullDetailDTO
        //                        where x.PullCredit.ToLower() == "pull"
        //                           && x.PoolQuantity.GetValueOrDefault(0) > (x.CreditConsignedQuantity.GetValueOrDefault(0) + x.CreditCustomerOwnedQuantity.GetValueOrDefault(0))
        //                        select x
        //                          ).OrderBy(y => y.Created.GetValueOrDefault(DateTime.MinValue));


        //    List<ItemLocationDetailsDTO> itmLocationDetails = new List<ItemLocationDetailsDTO>();
        //    double creditQty = itm.Quantity;
        //    double qtyCheck = 0;
        //    double remainingQty = itm.Quantity;
        //    if (lstPullDetailDTO != null && lstPullDetailDTO.Count() > 0)
        //    {
        //        foreach (var item in lstPullDetailDTO)
        //        {
        //            qtyCheck += item.PoolQuantity.GetValueOrDefault(0);
        //            if (qtyCheck < creditQty)
        //            {
        //                remainingQty -= item.PoolQuantity.GetValueOrDefault(0);
        //                ItemLocationDetailsDTO itml = ConvertPullDetailtoItemLocationDetail(item, itm, BinID.GetValueOrDefault(0));
        //                itmLocationDetails.Add(itml);
        //            }
        //            else if (qtyCheck >= creditQty)
        //            {
        //                ItemLocationDetailsDTO itml = ConvertPullDetailtoItemLocationDetail(item, itm, BinID.GetValueOrDefault(0));

        //                if (itml.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
        //                {
        //                    itml.CustomerOwnedQuantity = remainingQty;
        //                }
        //                else if (itml.ConsignedQuantity.GetValueOrDefault(0) > 0)
        //                {
        //                    itml.ConsignedQuantity = remainingQty;
        //                }
        //                itmLocationDetails.Add(itml);
        //            }
        //        }
        //    }

        //    return itmLocationDetails;
        //}

        /// <summary>
        /// QuaickListCredit
        /// </summary>
        /// <param name="ql"></param>
        /// <returns></returns>
        //private string QuaickListCredit(ItemInfoToCredit ql)
        //{
        //    return string.Empty;

        //}

        /// <summary>
        /// Convert Pull Detailto ItemLocationDetail
        /// </summary>
        /// <param name="item"></param>
        /// <param name="itm"></param>
        /// <param name="BinID"></param>
        /// <returns></returns>
        //private ItemLocationDetailsDTO ConvertPullDetailtoItemLocationDetail(PullDetailsDTO item, ItemInfoToCredit itm, Int64 BinID)
        //{
        //    ItemLocationDetailsDTO itml = new ItemLocationDetailsDTO()
        //    {
        //        BinID = BinID,
        //        BinNumber = itm.Bin,
        //        ConsignedQuantity = item.ConsignedQuantity,
        //        CustomerOwnedQuantity = item.CustomerOwnedQuantity,
        //        ItemGUID = itm.ItemGuid,
        //        ProjectSpentGUID = item.ProjectSpendGUID,
        //        SerialNumber = item.SerialNumber,
        //        LotNumber = item.LotNumber,
        //        Expiration = item.Expiration,
        //        UDF1 = itm.UDF1,
        //        UDF2 = itm.UDF2,
        //        UDF3 = itm.UDF3,
        //        UDF4 = itm.UDF4,
        //        UDF5 = itm.UDF5,
        //        CompanyID = SessionHelper.CompanyID,
        //        Room = SessionHelper.RoomID,
        //        RoomName = SessionHelper.RoomName,
        //        Created = DateTimeUtility.DateTimeNow,
        //        CreatedByName = SessionHelper.UserName,
        //        IsOnlyFromUI = true,
        //        IsPDAEdit = false,
        //        IsWebEdit = true,
        //        EditedFrom = "Web",
        //        AddedFrom = "Web",
        //        InsertedFrom = "Pull Credit",
        //        Cost = item.ItemCost,
        //        CreatedBy = SessionHelper.UserID,
        //        CountCustOrConsQty = itm.Quantity,
        //        LastUpdatedBy = SessionHelper.UserID,
        //        ReceivedOn = DateTimeUtility.DateTimeNow,
        //        ReceivedOnWeb = DateTimeUtility.DateTimeNow,
        //        Updated = DateTimeUtility.DateTimeNow,
        //        UpdatedByName = SessionHelper.UserName,
        //        ItemNumber = item.ItemNumber,
        //    };

        //    return itml;
        //}
        #region Pull PO Master
        public ActionResult PullPOMasterList()
        {
            //XElement Settinfile = XElement.Load(Server.MapPath("/SiteSettings.xml"));
            int iUDFMaxLength = 200;

            //int.TryParse(Convert.ToString(Settinfile.Element("UDFMaxLength").Value), out iUDFMaxLength);
            int.TryParse(Convert.ToString(SiteSettingHelper.UDFMaxLength), out iUDFMaxLength);

            UDFDAL objUDFDAL = new UDFDAL(this.enterPriseDBName);
            UDFDTO objUDFDTO = new UDFDTO();
            objUDFDTO = objUDFDAL.GetPullPORecord(SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, iUDFMaxLength);
            ViewBag.ControlType = objUDFDTO.UDFControlType;
            return View();
        }
        public ActionResult PullPOMasterListAjax(QuickListJQueryDataTableParamModel param)
        {
            PullPOMasterDAL obj = new PullPOMasterDAL(this.enterPriseDBName);
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
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = obj.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, RoomDateFormat, CurrentTimeZone);

            var result = from u in DataFromDB
                         select new PullPOMasterDTO
                         {
                             ID = u.ID,
                             PullOrderNumber = u.PullOrderNumber,
                             Created = u.Created,
                             Updated = u.Updated,
                             CreatedBy = u.CreatedBy,
                             UpdatedBy = u.UpdatedBy,
                             IsDeleted = u.IsDeleted,
                             IsArchived = u.IsArchived,
                             CompanyID = u.CompanyID,
                             RoomId = u.RoomId,
                             CreatedByName = u.CreatedByName,
                             UpdatedByName = u.UpdatedByName,
                             RoomName = u.RoomName,
                             CreatedDate = CommonUtility.ConvertDateByTimeZone(u.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             UpdatedDate = CommonUtility.ConvertDateByTimeZone(u.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             AddedFrom = u.AddedFrom,
                             EditedFrom = u.EditedFrom,
                             ReceivedOnWeb = u.ReceivedOnWeb,
                             ReceivedOn = u.ReceivedOn,
                             IsActive = u.IsActive,
                         };


            JsonResult jsresult = Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = result
            }, JsonRequestBehavior.AllowGet);
            jsresult.MaxJsonLength = int.MaxValue;
            return jsresult;


        }

        public JsonResult UpdatePOActive(bool IsActive, long ID)
        {
            JsonResult jsresult = Json(new { result = "true", Message = "sucess" }, JsonRequestBehavior.AllowGet);
            try
            {
                PullPOMasterDAL obj = new PullPOMasterDAL(this.enterPriseDBName);
                Int64 ReturnID = obj.UpdateStatus(ID, IsActive, SessionHelper.UserID);
                if (ReturnID == 0)
                {
                    jsresult = Json(new { result = "false", Message = "notfound" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                jsresult = Json(new { result = "false", Message = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
            return jsresult;
        }
        public JsonResult UpdatePOControl(string currentControlType)
        {
            JsonResult jsresult = Json(new { result = "true", Message = "sucess" }, JsonRequestBehavior.AllowGet);
            try
            {
                //XElement Settinfile = XElement.Load(Server.MapPath("/SiteSettings.xml"));
                int iUDFMaxLength = 200;

                //int.TryParse(Convert.ToString(Settinfile.Element("UDFMaxLength").Value), out iUDFMaxLength);
                int.TryParse(Convert.ToString(SiteSettingHelper.UDFMaxLength), out iUDFMaxLength);

                UDFDAL objUDFDAL = new UDFDAL(this.enterPriseDBName);
                UDFDTO objUDFDTO = new UDFDTO();
                objUDFDTO.CompanyID = SessionHelper.CompanyID;
                objUDFDTO = objUDFDAL.GetPullPORecord(SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, iUDFMaxLength);
                objUDFDTO.UDFControlType = currentControlType;
                objUDFDTO.Updated = DateTimeUtility.DateTimeNow;
                objUDFDTO.LastUpdatedBy = SessionHelper.UserID;
                UDFDAL objMaster = new UDFDAL(this.enterPriseDBName);
                objMaster.Edit(objUDFDTO);

            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                jsresult = Json(new { result = "false", Message = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
            return jsresult;
        }

        public JsonResult DeletePORecords(string ids)
        {
            try
            {
                PullPOMasterDAL objPullPOMasterDAL = new PullPOMasterDAL(this.enterPriseDBName);
                objPullPOMasterDAL.DeleteOrUnDeletePullPOMaster(ids, SessionHelper.UserID, true);
                return Json(new { Message = "Deleted", Status = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                return Json(new { Message = "", Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UnDeletePORecords(string ids)
        {
            try
            {
                PullPOMasterDAL objPullPOMasterDAL = new PullPOMasterDAL(this.enterPriseDBName);
                objPullPOMasterDAL.DeleteOrUnDeletePullPOMaster(ids, SessionHelper.UserID, false);
                return Json(new { Message = "UnDeleted", Status = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                return Json(new { Message = "", Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult _CreatePullPO()
        {
            return PartialView("_CreatePullPO");
        }
        public ActionResult PullPOEdit(Int64 ID)
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

            PullPOMasterDAL obj = new PullPOMasterDAL(this.enterPriseDBName);
            PullPOMasterDTO objDTO = new PullPOMasterDTO();

            objDTO = obj.GetPullPOMasterByIdFull(ID);

            return PartialView("_CreatePullPO", objDTO);
        }

        public ActionResult PullPOCreate()
        {
            PullPOMasterDTO objDTO = new PullPOMasterDTO();
            objDTO.ID = 0;
            objDTO.PullOrderNumber = string.Empty;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.CreatedBy = SessionHelper.UserID;
            objDTO.RoomId = SessionHelper.RoomID;
            objDTO.CompanyID = SessionHelper.CompanyID;
            objDTO.RoomName = SessionHelper.RoomName;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.CreatedByName = SessionHelper.UserName;
            objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
            objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
            objDTO.UpdatedBy = SessionHelper.UserID;
            objDTO.IsDeleted = false;
            objDTO.IsArchived = false;
            objDTO.IsActive = true;
            return PartialView("_CreatePullPO", objDTO);
        }

        [HttpPost]
        public JsonResult PullPOSave(PullPOMasterDTO objDTO)
        {
            PullPOMasterDAL obj = new PullPOMasterDAL(this.enterPriseDBName);
            string message = "";
            string status = "true";
            bool isSuccess = true;
            if (string.IsNullOrWhiteSpace(objDTO.PullOrderNumber))
            {
                return Json(new { IsSuccess = false, Massage = string.Format(ResMessage.Required,ResPullPOMaster.PullOrderNumber, objDTO.PullOrderNumber), RetrunDTO = objDTO, Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                if (objDTO.ID == 0)
                {
                    //create code
                    obj = new PullPOMasterDAL(this.enterPriseDBName);
                    bool CheckDuplicate = obj.DuplicateRecordCheck(objDTO.ID, objDTO.PullOrderNumber, SessionHelper.RoomID, SessionHelper.CompanyID);

                    if (!CheckDuplicate)
                    {
                        return Json(new { IsSuccess = false, Massage = string.Format(ResMessage.DuplicateMessage, ResPullPOMaster.PullOrderNumber, objDTO.PullOrderNumber), RetrunDTO = objDTO, Status = "fail" }, JsonRequestBehavior.AllowGet);
                    }

                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.Created = DateTimeUtility.DateTimeNow;

                    objDTO.IsDeleted = false;
                    objDTO.RoomId = SessionHelper.RoomID;
                    objDTO.UpdatedBy = SessionHelper.UserID;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.AddedFrom = "Web";
                    objDTO.EditedFrom = "Web";
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    obj.Insert(objDTO);
                    message = ResMessage.SaveMessage;
                }
                else
                {
                    bool CheckDuplicate = obj.DuplicateRecordCheck(objDTO.ID, objDTO.PullOrderNumber, SessionHelper.RoomID, SessionHelper.CompanyID);

                    if (!CheckDuplicate)
                    {
                        return Json(new { IsSuccess = false, Massage = string.Format(ResMessage.DuplicateMessage, ResPullPOMaster.PullOrderNumber,objDTO.PullOrderNumber), RetrunDTO = objDTO, Status = "fail" }, JsonRequestBehavior.AllowGet);
                    }
                    objDTO.UpdatedBy = SessionHelper.UserID;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.EditedFrom = "Web";
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;

                    obj.Update(objDTO);
                    message = ResMessage.SaveMessage;
                }

            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                isSuccess = false;
                message = ex.Message.ToString();
            }

            return Json(new { IsSuccess = isSuccess, Massage = message, RetrunDTO = objDTO, Status = status }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region MS Credit

        [HttpPost]
        public JsonResult GetQLItemForMSCredit(List<ItemInfoToMSCredit> QLItems)
        {
            QuickListDAL QLDAL = null;
            QuickListMasterDTO QLMaster = null;
            List<QuickListDetailDTO> lstQLDetails = null;
            List<ItemInfoToMSCredit> lstItemsToCredit = null;
            PullMasterDAL pullDAL = this.pullMasterDAL; //new PullMasterDAL(this.enterPriseDBName);
            QLDAL = new QuickListDAL(this.enterPriseDBName);
            lstItemsToCredit = new List<ItemInfoToMSCredit>();
            int rowID = 50000;

            //  RoomDTO objRoomDTO = new RoomDAL(this.enterPriseDBName).GetRoomByIDPlain(SessionHelper.RoomID);
            CommonDAL objCommonDAL = this.commonDAL; //new CommonDAL(this.enterPriseDBName);
            string columnList = "ID,RoomName,IsIgnoreCreditRule";
            RoomDTO objRoomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");

            bool IsIgnoreCreditRule = (objRoomDTO != null ? objRoomDTO.IsIgnoreCreditRule : false);

            foreach (var itemToCredit in QLItems)
            {
                QLMaster = QLDAL.GetRecord(itemToCredit.QLGuid.GetValueOrDefault(Guid.Empty).ToString(), SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                itemToCredit.QuickListName = QLMaster.Name;
                var tmpSupplierIds = new List<long>();
                lstQLDetails = QLDAL.GetQuickListItemsRecords(SessionHelper.RoomID, SessionHelper.CompanyID, QLMaster.GUID.ToString(), tmpSupplierIds);

                if (lstQLDetails != null && lstQLDetails.Count > 0)
                {
                    itemToCredit.PrevPullsToMSCredit = new List<PullDetailToMSCredit>();
                    foreach (var item in lstQLDetails)
                    {
                        rowID = rowID + 1;
                        if (IsIgnoreCreditRule)
                        {
                            ItemInfoToMSCredit itmInfo = new ItemInfoToMSCredit()
                            {
                                ItemGuid = item.ItemGUID,
                                Quantity = item.Quantity.GetValueOrDefault(0) * itemToCredit.Quantity,
                                ItemQtyInQL = item.DefaultPullQuantity.GetValueOrDefault(0),
                                CreditQLQty = itemToCredit.Quantity,
                                ItemNumber = item.ItemNumber,
                                ItemType = item.ItemType,
                                UDF1 = itemToCredit.UDF1,
                                UDF2 = itemToCredit.UDF2,
                                UDF3 = itemToCredit.UDF3,
                                UDF4 = itemToCredit.UDF4,
                                UDF5 = itemToCredit.UDF5,
                                ProjectName = itemToCredit.ProjectName,
                                PullOrderNumber = itemToCredit.PullOrderNumber,
                                Bin = item.BinName,
                                ItemTracking = itemToCredit.ItemTracking,
                                QuickListName = QLMaster.Name,
                                IsModelShow = itemToCredit.IsModelShow,
                                RowID = rowID,
                                QLGuid = itemToCredit.QLGuid,
                                WOGuid = itemToCredit.WOGuid,
                                SupplierAccountGuid = itemToCredit.SupplierAccountGuid
                            };

                            if (item.LotNumberTracking)
                            {
                                itmInfo.ItemTracking = "LOTTRACK";
                            }
                            else if (item.SerialNumberTracking)
                            {
                                itmInfo.ItemTracking = "SERIALTRACK";
                            }
                            else if (item.DateCodeTracking)
                            {
                                itmInfo.ItemTracking = "DATECODETRACK";
                            }
                            else
                            {
                                itmInfo.ItemTracking = "";
                            }
                            lstItemsToCredit.Add(itmInfo);
                        }
                        else
                        {
                            ItemInfoToMSCredit itmInfo = new ItemInfoToMSCredit()
                            {
                                ItemGuid = item.ItemGUID,
                                Quantity = item.Quantity.GetValueOrDefault(0) * itemToCredit.Quantity,
                                ItemQtyInQL = item.Quantity.GetValueOrDefault(0),
                                CreditQLQty = itemToCredit.Quantity,
                                ItemNumber = item.ItemNumber,
                                ItemType = item.ItemType,
                                UDF1 = itemToCredit.UDF1,
                                UDF2 = itemToCredit.UDF2,
                                UDF3 = itemToCredit.UDF3,
                                UDF4 = itemToCredit.UDF4,
                                UDF5 = itemToCredit.UDF5,
                                ProjectName = itemToCredit.ProjectName,
                                Bin = itemToCredit.Bin,
                                ItemTracking = itemToCredit.ItemTracking,
                                QuickListName = QLMaster.Name,
                                IsModelShow = itemToCredit.IsModelShow,
                                RowID = rowID,
                                QLGuid = itemToCredit.QLGuid,
                                WOGuid = itemToCredit.WOGuid,
                                SupplierAccountGuid = itemToCredit.SupplierAccountGuid
                            };

                            if (item.LotNumberTracking)
                            {
                                itmInfo.ItemTracking = "LOTTRACK";
                            }
                            else if (item.SerialNumberTracking)
                            {
                                itmInfo.ItemTracking = "SERIALTRACK";
                            }
                            else if (item.DateCodeTracking)
                            {
                                itmInfo.ItemTracking = "DATECODETRACK";
                            }
                            else
                            {
                                itmInfo.ItemTracking = "";
                            }

                            List<PullDetailToMSCredit> lstPullDetails = pullDAL.GetPrevLoadMSPull(itmInfo, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID, ResourceHelper.CurrentCult.Name);
                            if (lstPullDetails != null && lstPullDetails.Count > 0)
                            {
                                foreach (var pldtl in lstPullDetails)
                                {
                                    itemToCredit.PrevPullsToMSCredit.Add(pldtl);
                                }
                            }
                            else
                            {
                                itmInfo.ErrorMessage = ResPullMaster.ItemDontHavePullToCredit;
                                itmInfo.IsModelShow = true;
                            }

                            lstItemsToCredit.Add(itmInfo);
                        }
                    }
                }
            }
            return Json(new { Message = "OK", Status = true, QLItems = lstItemsToCredit }, JsonRequestBehavior.AllowGet);
        }


        #endregion

        public string GetSupplierPOPairList()
        {
            List<SupplierPODTO> supplierPOPairList = new List<SupplierPODTO>();
            supplierPOPairList = new AutoSequenceDAL(this.enterPriseDBName).GetSupplierPOPair(SessionHelper.RoomID, SessionHelper.CompanyID,SessionHelper.UserID,SessionHelper.EnterPriceID);
            string strJsonSuppPOPair = string.Empty;
            if (supplierPOPairList != null && supplierPOPairList.Count > 0)
            {
                var lstjson = new JavaScriptSerializer().Serialize(supplierPOPairList);
                strJsonSuppPOPair = lstjson.ToString();
            }

            return strJsonSuppPOPair;
        }

        #region Add Quick list item to direct to cart

        public JsonResult GetItemByQLGuid(string QuickListGUID, double PullQuantity)
        {
            List<ItemMasterDTO> returnKeyValList = new List<ItemMasterDTO>();
            try
            {
                QuickListDAL objQLDtlDAL = new QuickListDAL(this.enterPriseDBName);
                string QLGuid = QuickListGUID;

                List<QuickListDetailDTO> objQLDtlDTO = objQLDtlDAL.GetQuickListItemsRecords(SessionHelper.RoomID, SessionHelper.CompanyID, QLGuid, SessionHelper.UserSupplierIds).Where(x => x.IsDeleted == false).ToList();
                ItemMasterDAL objItemDAL = new ItemMasterDAL(this.enterPriseDBName);
                if (objQLDtlDTO.Count > 0)
                {
                    List<CartItemDTO> lstCartItems = new List<CartItemDTO>();

                    foreach (QuickListDetailDTO qlItem in objQLDtlDTO)
                    {
                        CartItemDTO objCartItem = new CartItemDTO();
                        ItemMasterDTO tempItemDTO = new ItemMasterDTO();
                        tempItemDTO = objItemDAL.GetItemWithoutJoins(null, qlItem.ItemGUID);
                        if (tempItemDTO != null && tempItemDTO.ItemType != 4)
                        {
                            /// used for return QL Qty to AJax response 
                            tempItemDTO.QuanityPulled = qlItem.Quantity;
                            //////////////////
                            tempItemDTO.BinID = qlItem.BinID;
                            tempItemDTO.BinNumber = qlItem.BinName;

                            returnKeyValList.Add(tempItemDTO);
                        }
                    }
                }
                if (returnKeyValList.Count > 0)
                    returnKeyValList = returnKeyValList.OrderBy(x => x.GUID).ToList();
                return Json(returnKeyValList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(returnKeyValList, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region WI-4693-Allow specified rooms to ignore credit rules

        public ActionResult PullLotSrSelectionForCredit(List<PullMasterDTO> lstPullRequestInfo)
        {
            try
            {
                List<PullMasterDTO> lstPullWithAllBinRequest = new List<PullMasterDTO>();
                List<PullMasterDTO> lstPullRequest = new List<PullMasterDTO>();

                foreach (PullMasterDTO objPullMasterDTO in lstPullRequestInfo)
                {
                    if (!lstPullRequest.Select(x => x.ItemGUID).Contains(objPullMasterDTO.ItemGUID))
                        lstPullRequest.Add(objPullMasterDTO);
                }

                PullTransactionDAL objPullMasterDAL = new PullTransactionDAL(this.enterPriseDBName);
                lstPullRequestInfo = objPullMasterDAL.GetPullWithDetailsForSerialLot(lstPullRequest, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, false);

                return PartialView("PullLotSrSelectionForCredit", lstPullRequestInfo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
        }

        public ActionResult PullLotSrSelectionForMSCredit(List<PullMasterDTO> lstPullRequestInfo)
        {
            try
            {
                List<PullMasterDTO> lstPullWithAllBinRequest = new List<PullMasterDTO>();
                List<PullMasterDTO> lstPullRequest = new List<PullMasterDTO>();

                foreach (PullMasterDTO objPullMasterDTO in lstPullRequestInfo)
                {
                    if (!lstPullRequest.Select(x => x.ItemGUID).Contains(objPullMasterDTO.ItemGUID))
                        lstPullRequest.Add(objPullMasterDTO);
                }

                PullTransactionDAL objPullMasterDAL = new PullTransactionDAL(this.enterPriseDBName);
                lstPullRequestInfo = objPullMasterDAL.GetPullWithDetailsForSerialLot(lstPullRequest, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, true);

                return PartialView("PullLotSrSelectionForMSCredit", lstPullRequestInfo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
        }

        public ActionResult PullLotSrSelectionForCreditPopup(JQueryDataTableParamModel param)
        {
            Guid ItemGUID = Guid.Empty;
            Int64 BinID = 0;
            double PullQuantity = 0;
            double EnteredPullQuantity = 0;

            Guid.TryParse(Convert.ToString(Request["ItemGUID"]), out ItemGUID);
            long.TryParse(Convert.ToString(Request["BinID"]), out BinID);
            double.TryParse(Convert.ToString(Request["PullQuantity"]), out PullQuantity);
            string InventoryConsuptionMethod = Convert.ToString(Request["InventoryConsuptionMethod"]);
            string CurrentLoaded = Convert.ToString(Request["CurrentLoaded"]);
            string CurrentDeletedLoaded = Convert.ToString(Request["CurrentDeletedLoaded"]);
            string ViewRight = Convert.ToString(Request["ViewRight"]);
            bool IsDeleteRowMode = Convert.ToBoolean(Request["IsDeleteRowMode"]);
            bool IsStagginLocation = false;
            EnteredPullQuantity = PullQuantity;

            string[] arrIds = new string[] { };

            if (!string.IsNullOrWhiteSpace(CurrentLoaded))
            {
                arrIds = CurrentLoaded.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }

            int TotalRecordCount = 0;
            PullTransactionDAL objPullDetails = new PullTransactionDAL(this.enterPriseDBName);
            List<ItemLocationLotSerialDTO> lstLotSrs = new List<ItemLocationLotSerialDTO>();
            Dictionary<string, double> dicSerialLots = new Dictionary<string, double>();

            List<ItemLocationLotSerialDTO> lstsetPulls = new List<ItemLocationLotSerialDTO>();

            string[] arrItem;

            List<ItemLocationLotSerialDTO> retlstLotSrs = new List<ItemLocationLotSerialDTO>();

            ItemMasterDTO oItem = null;
            BinMasterDTO objLocDTO = null;
            if (ItemGUID != Guid.Empty)
            {
                oItem = new ItemMasterDAL(this.enterPriseDBName).GetItemWithoutJoins(null, ItemGUID);
                objLocDTO = new BinMasterDAL(this.enterPriseDBName).GetBinByID(BinID, SessionHelper.RoomID, SessionHelper.CompanyID);
                if (objLocDTO != null && objLocDTO.ID > 0 && objLocDTO.IsStagingLocation)
                    IsStagginLocation = true;
            }

            if (oItem != null && oItem.ItemType == 4)
            {
                ItemLocationLotSerialDTO oLotSr = new ItemLocationLotSerialDTO();
                oLotSr.BinID = BinID;
                oLotSr.ID = BinID;
                oLotSr.BinNumber = string.Empty;
                oLotSr.ItemGUID = ItemGUID;
                oLotSr.LotOrSerailNumber = string.Empty;
                oLotSr.Expiration = string.Empty;
                oLotSr.PullQuantity = oItem.DefaultPullQuantity.GetValueOrDefault(0) > PullQuantity ? oItem.DefaultPullQuantity.GetValueOrDefault(0) : PullQuantity;
                oLotSr.LotSerialQuantity = PullQuantity;//oItem.DefaultPullQuantity.GetValueOrDefault(0);

                retlstLotSrs.Add(oLotSr);
            }
            else
            {
                if (arrIds.Count() > 0)
                {
                    string[] arrSerialLots = new string[arrIds.Count()];
                    for (int i = 0; i < arrIds.Count(); i++)
                    {
                        if ((oItem.SerialNumberTracking && !oItem.DateCodeTracking)
                            || (oItem.LotNumberTracking && !oItem.DateCodeTracking)
                            || !oItem.DateCodeTracking)
                        {
                            arrItem = new string[2];
                            arrItem[0] = arrIds[i].Substring(0, arrIds[i].LastIndexOf("_"));
                            arrItem[1] = arrIds[i].Replace(arrItem[0] + "_", "");
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
                            arrItem = arrIds[i].Split('_');
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
                            arrItem = arrIds[i].Split('_');
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
                            arrItem = arrIds[i].Split('_');
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

                    lstLotSrs = objPullDetails.GetItemLocationsWithLotSerialsForPullForMoreCredit(ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, PullQuantity, false, string.Empty, IsStagginLocation);

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
                    else
                        retlstLotSrs = objPullDetails.GetItemLocationsWithLotSerialsForPullForMoreCredit(ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, PullQuantity, true, string.Empty, IsStagginLocation);
                }

                foreach (ItemLocationLotSerialDTO item in retlstLotSrs)
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
                }
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

            if (CurrentDeletedLoaded != "")
            {
                string[] arrDeletedIds = new string[] { };
                if (!string.IsNullOrWhiteSpace(CurrentDeletedLoaded))
                {
                    arrDeletedIds = CurrentDeletedLoaded.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                    if (arrDeletedIds.Count() > 0)
                    {
                        string[] arrSerialLots = new string[arrDeletedIds.Count()];
                        for (int i = 0; i < arrDeletedIds.Count(); i++)
                        {
                            PullQuantity += 1;
                            if ((oItem.SerialNumberTracking && !oItem.DateCodeTracking)
                                || (oItem.LotNumberTracking && !oItem.DateCodeTracking)
                                || !oItem.DateCodeTracking)
                            {
                                arrItem = new string[2];
                                arrItem[0] = arrDeletedIds[i].Substring(0, arrDeletedIds[i].LastIndexOf("_"));
                                arrItem[1] = arrDeletedIds[i].Replace(arrItem[0] + "_", "");
                                if (arrItem.Length > 1)
                                {
                                    if (oItem.SerialNumberTracking)
                                    {
                                        retlstLotSrs.RemoveAll(x => x.SerialNumber == arrItem[0]);
                                    }
                                    if (oItem.LotNumberTracking)
                                    {
                                        retlstLotSrs.RemoveAll(x => x.LotNumber == arrItem[0]);
                                    }
                                }
                            }
                            else if ((oItem.SerialNumberTracking && oItem.DateCodeTracking)
                                || (oItem.LotNumberTracking && oItem.DateCodeTracking))
                            {
                                arrItem = arrDeletedIds[i].Split('_');
                                if (arrItem.Length > 1)
                                {
                                    if (oItem.SerialNumberTracking && oItem.DateCodeTracking)
                                    {
                                        if (!string.IsNullOrWhiteSpace(arrItem[1]))
                                        {
                                            DateTime DtExpirationDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(arrItem[1], SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone);
                                            retlstLotSrs.RemoveAll(x => x.SerialNumber == arrItem[0] && Convert.ToDateTime(x.ExpirationDate).Date == DtExpirationDate.Date);
                                        }
                                        else
                                            retlstLotSrs.RemoveAll(x => x.SerialNumber == arrItem[0]);
                                    }
                                    if (oItem.LotNumberTracking && oItem.DateCodeTracking)
                                    {
                                        if (!string.IsNullOrWhiteSpace(arrItem[1]))
                                        {
                                            DateTime DtExpirationDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(arrItem[1], SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone);
                                            retlstLotSrs.RemoveAll(x => x.LotNumber == arrItem[0] && Convert.ToDateTime(x.ExpirationDate).Date == DtExpirationDate.Date);
                                        }
                                        else
                                        {
                                            retlstLotSrs.RemoveAll(x => x.LotNumber == arrItem[0]);
                                        }
                                    }
                                }
                            }
                            else if (!oItem.SerialNumberTracking && !oItem.DateCodeTracking && oItem.DateCodeTracking)
                            {
                                arrItem = arrDeletedIds[i].Split('_');
                                if (arrItem.Length > 1)
                                {
                                    if (oItem.DateCodeTracking)
                                    {
                                        if (!string.IsNullOrWhiteSpace(arrItem[0]))
                                        {
                                            DateTime DtExpirationDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(arrItem[0], SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone);
                                            retlstLotSrs.RemoveAll(x => Convert.ToDateTime(x.ExpirationDate).Date == DtExpirationDate.Date);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                arrItem = arrDeletedIds[i].Split('_');
                                if (arrItem.Length > 1)
                                {
                                    if (oItem.SerialNumberTracking)
                                    {
                                        retlstLotSrs.RemoveAll(x => x.SerialNumber == arrItem[0]);
                                    }
                                    if (oItem.LotNumberTracking)
                                    {
                                        retlstLotSrs.RemoveAll(x => x.LotNumber == arrItem[0]);
                                    }
                                    if (oItem.DateCodeTracking)
                                    {
                                        if (!string.IsNullOrWhiteSpace(arrItem[0]))
                                        {
                                            DateTime DtExpirationDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(arrItem[0], SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone);
                                            retlstLotSrs.RemoveAll(x => Convert.ToDateTime(x.ExpirationDate).Date == DtExpirationDate.Date);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

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

        public JsonResult GetLotOrSerailNumberListForCredit(int maxRows, string name_startsWith, Guid? ItemGuid, long BinID, string prmSerialLotNos = null)
        {
            bool IsStagginLocation = false;

            BinMasterDTO objLocDTO = new BinMasterDAL(this.enterPriseDBName).GetBinByID(BinID, SessionHelper.RoomID, SessionHelper.CompanyID);
            if (objLocDTO != null && objLocDTO.ID > 0 && objLocDTO.IsStagingLocation)
            {
                IsStagginLocation = true;
            }

            PullTransactionDAL objPullDetails = new PullTransactionDAL(this.enterPriseDBName);
            List<ItemLocationLotSerialDTO> objItemLocationLotSerialDTO = objPullDetails.GetItemLocationsWithLotSerialsForPullForMoreCredit(ItemGuid.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID, 0, false, string.Empty, IsStagginLocation);

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
        public JsonResult ValidateSerialNumberForCredit(Guid ItemGuid, string SerialNumber, Int64 BinID)
        {
            if (!string.IsNullOrWhiteSpace(SerialNumber))
            {
                SerialNumber = SerialNumber.Trim();
            }

            bool IsStagginLocation = false;
            BinMasterDTO objLocDTO = new BinMasterDAL(this.enterPriseDBName).GetBinByID(BinID, SessionHelper.RoomID, SessionHelper.CompanyID);
            if (objLocDTO != null)
            {
                IsStagginLocation = objLocDTO.IsStagingLocation;
            }

            PullTransactionDAL objPullDetails = new PullTransactionDAL(this.enterPriseDBName);
            bool IsSerailAvailableForCredit = objPullDetails.ValidateSerialNumberForCredit(ItemGuid, SerialNumber, SessionHelper.CompanyID, SessionHelper.RoomID);

            return Json(new
            {
                IsSerailAvailableForCredit = IsSerailAvailableForCredit
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ValidateLotDateCodeForCredit(Guid ItemGuid, string LotNumber, string ExpirationDate, Int64 BinID)
        {
            if (!string.IsNullOrWhiteSpace(LotNumber))
            {
                LotNumber = LotNumber.Trim();
            }

            bool IsStagginLocation = false;
            BinMasterDTO objLocDTO = new BinMasterDAL(this.enterPriseDBName).GetBinByID(BinID, SessionHelper.RoomID, SessionHelper.CompanyID);
            if (objLocDTO != null)
            {
                IsStagginLocation = objLocDTO.IsStagingLocation;
            }

            PullTransactionDAL objPullDetails = new PullTransactionDAL(this.enterPriseDBName);
            DateTime ExpirationDateUTC = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(ExpirationDate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone);
            bool IsSerailAvailableForCredit = objPullDetails.ValidateLotDateCodeForCredit(ItemGuid, LotNumber, ExpirationDateUTC, SessionHelper.CompanyID, SessionHelper.RoomID);

            return Json(new
            {
                IsSerailAvailableForCredit = IsSerailAvailableForCredit
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public Int64 GetOrInsertBinIDByName(Guid ItemGuid, string Name, bool IsStagingLoc = false)
        {
            Int64? BinID = 0;
            BinMasterDAL binDAL = new BinMasterDAL(this.enterPriseDBName);
            BinID = binDAL.GetOrInsertBinIDByName(ItemGuid, Name, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID, IsStagingLoc);
            return BinID.GetValueOrDefault(0);
        }

        [HttpPost]
        public JsonResult SaveMaterialStagingFromCredit(Int64 BinID, string StagingName)
        {
            MaterialStagingDAL objMaterialStagingDAL = new MaterialStagingDAL(this.enterPriseDBName);
            string strOK = string.Empty;
            long SessionUserId = SessionHelper.UserID;
            MaterialStagingDTO stagingData = objMaterialStagingDAL.GetRecordByName(StagingName, SessionHelper.RoomID, SessionHelper.CompanyID);
            if (stagingData != null && stagingData.ID > 0)
            {
                return Json(new
                {
                    MaterialStagingGuid = stagingData.GUID,
                    MaterialStagingID = stagingData.ID,
                    Status = "Success"
                }, JsonRequestBehavior.AllowGet);

                //return Json(new
                //{
                //    MaterialStagingGuid = "",
                //    MaterialStagingID = "",
                //    Status = "duplicate"
                //}, JsonRequestBehavior.AllowGet);
            }
            else
            {
                MaterialStagingDTO objMaterialStagingDTO = new MaterialStagingDTO();
                BinMasterDTO objBin = new BinMasterDTO();
                if (BinID > 0)
                {
                    objBin = new BinMasterDAL(this.enterPriseDBName).GetBinByID(BinID, SessionHelper.RoomID, SessionHelper.CompanyID);
                }

                //objMaterialStagingDTO.BinID = BinID;
                //objMaterialStagingDTO.BinGUID = objBin.GUID;
                objMaterialStagingDTO.BinName = "";
                objMaterialStagingDTO.StagingName = StagingName;

                objMaterialStagingDTO.Room = SessionHelper.RoomID;
                objMaterialStagingDTO.CompanyID = SessionHelper.CompanyID;
                objMaterialStagingDTO.CreatedBy = SessionHelper.UserID;
                objMaterialStagingDTO.Description = "";
                objMaterialStagingDTO.IsArchived = false;
                objMaterialStagingDTO.IsDeleted = false;
                objMaterialStagingDTO.StagingLocationName = "";
                objMaterialStagingDTO.StagingStatus = 1;
                objMaterialStagingDTO.WhatWhereAction = "Staging Header Create from Credit";

                long MaterialStagingID = objMaterialStagingDAL.SaveMaterialStaging(objMaterialStagingDTO, SessionUserId);
                if (MaterialStagingID > 0)
                {
                    MaterialStagingDTO stagingInsertedData = objMaterialStagingDAL.GetRecord(MaterialStagingID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (stagingInsertedData != null)
                    {
                        return Json(new
                        {
                            MaterialStagingGuid = stagingInsertedData.GUID,
                            MaterialStagingID = MaterialStagingID,
                            Status = "Success"
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
            }

            return Json(new
            {
                MaterialStagingGuid = "",
                MaterialStagingID = "",
                Status = ""
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        public JsonResult GetUDFOptionByID(long UDFID)
        {

            List<UDFOptionsDTO> lstUDFData = new List<UDFOptionsDTO>();

            if (Session["PullMasterUDFOptions" + UDFID.ToString()] != null)
            {
                lstUDFData = (List<UDFOptionsDTO>)Session["PullMasterUDFOptions" + UDFID.ToString()];
            }
            else
            {
                using (UDFOptionDAL obj = new UDFOptionDAL(this.enterPriseDBName))
                {
                    //lstUDFData = obj.GetUDFOptionsByUDF(UDFID, SessionHelper.CompanyID).OrderBy(e => e.UDFOption).ToList();
                    lstUDFData = obj.GetUDFOptionsByUDFIDPlain(UDFID).OrderBy(e => e.UDFOption).ToList();
                    Session["PullMasterUDFOptions" + UDFID.ToString()] = lstUDFData;
                }
            }


            return Json(new { Status = true, Message = "success", UDFData = lstUDFData }, JsonRequestBehavior.AllowGet);
        }

        #region WI-5566

        /// <summary>
        /// Get Supplier Account Number For NewPull Grid
        /// </summary>
        /// <param name="ItemGuid"></param>
        /// <param name="NameStartWith"></param>
        /// <returns></returns>
        public JsonResult GetSupplierAccountNumbersforPull(Guid ItemGuid, string NameStartWith)
        {
            List<DTOForAutoComplete> returnKeyValList = new List<DTOForAutoComplete>();
            Int64 RoomID = SessionHelper.RoomID;
            Int64 CompanyID = SessionHelper.CompanyID;
            List<SupplierAccountDetailsDTO> lstSupplierAccountDetails = new List<SupplierAccountDetailsDTO>();
            try
            {
                lstSupplierAccountDetails = this.pullMasterDAL.GetAllSupplierAccountNumbers(ItemGuid, NameStartWith, RoomID, CompanyID);

                if (lstSupplierAccountDetails != null && lstSupplierAccountDetails.Count() > 0)
                {
                    foreach (var item in lstSupplierAccountDetails)
                    {
                        DTOForAutoComplete obj = new DTOForAutoComplete()
                        {
                            Value = item.AccountNo,
                            Key = Convert.ToString(item.GUID),
                            ID = item.SupplierID ?? 0,
                            GUID = item.GUID,
                            OtherInfo1 = Convert.ToString(item.AccountName)
                        };
                        returnKeyValList.Add(obj);
                    }
                }

                return Json(returnKeyValList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(returnKeyValList, JsonRequestBehavior.AllowGet);
            }
            finally
            {

            }
        }

        #endregion

        #region 3055

        public JsonResult UpdatePullQtyInPullHistory(Guid PullGUID, Guid ItemGuid, double OldPullQuantity, double NewPullQuantity, string PullCreditType, string callFrom = "singlepull")
        {
            string strWhatWhereAction = "EPQ" + DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss");

            string _returnmessage = ResMessage.SaveMessage;
            string _returnstatus = "OK";
            int IsCreditPullNothing = 0; // false means its PULL         

            #region take item data

            ItemMasterDTO oItemRecord = new ItemMasterDAL(this.enterPriseDBName).GetItemWithoutJoins(null, ItemGuid);

            #endregion

            #region take Pull Master data from pull guid

            PullMasterDAL objpullMasterDAL = this.pullMasterDAL;
            PullMasterViewDTO objoldPullMasterData = new PullMasterViewDTO();
            PullMasterViewDTO objnewPullMasterData = new PullMasterViewDTO();

            objoldPullMasterData = objpullMasterDAL.GetPullByGuidPlain(PullGUID);
            objoldPullMasterData.WhatWhereAction = strWhatWhereAction;
            objnewPullMasterData = objoldPullMasterData;

            //need to update oter fileds

            #endregion

            #region Check project spend or bin deleted

            BinMasterDTO objBinCheck = new BinMasterDAL(this.enterPriseDBName).GetBinByID(objoldPullMasterData.BinID ?? 0, SessionHelper.RoomID, SessionHelper.CompanyID);
            if (objBinCheck != null && objBinCheck.ID > 0 && (objBinCheck.IsDeleted.GetValueOrDefault(false) == true || objBinCheck.IsArchived.GetValueOrDefault(false) == true))
            { 
                return Json(new { Message = string.Format(ResPullMaster.DeletedSoPullUpdateNotAllowed, objBinCheck.BinNumber), Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
            else if (objBinCheck == null)
            {
                return Json(new { Message = string.Format(ResPullMaster.DeletedSoPullUpdateNotAllowed, objBinCheck.BinNumber), Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }

            ProjectMasterDAL objPrjMsgDAL = new ProjectMasterDAL(this.enterPriseDBName);
            ProjectMasterDTO objPrjMstDTO = new ProjectMasterDTO();
            objPrjMstDTO = objPrjMsgDAL.GetProjectMasterByGuidNormal(objoldPullMasterData.ProjectSpendGUID.GetValueOrDefault(Guid.Empty));
            if (objPrjMstDTO != null && objPrjMstDTO.ID > 0 && (objPrjMstDTO.IsDeleted.GetValueOrDefault(false) == true || objPrjMstDTO.IsArchived.GetValueOrDefault(false) == true))
            {
                return Json(new { Message = string.Format(ResPullMaster.DeletedSoPullUpdateNotAllowed, objPrjMstDTO.ProjectSpendName), Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }

            #endregion

            #region take Pull details data from pull guid

            PullDetailsDAL objPullDetails = new PullDetailsDAL(this.enterPriseDBName);
            List<PullDetailsDTO> lstloldPullDetailsDTO = new List<PullDetailsDTO>();

            lstloldPullDetailsDTO = objPullDetails.GetPullDetailsByPullGuid(PullGUID, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();

            #endregion

            if (objoldPullMasterData != null && lstloldPullDetailsDTO != null)
            {
                #region For Pull

                if (!string.IsNullOrWhiteSpace(PullCreditType) && PullCreditType.ToLower().Equals("pull"))
                {
                    IsCreditPullNothing = 2;
                    ///default pull qty validation
                    bool isBinlevelEnforce = false;
                    if (objoldPullMasterData.BinID != 0)
                    {
                        BinMasterDTO objBin = new BinMasterDAL(this.enterPriseDBName).GetBinByID(objoldPullMasterData.BinID ?? 0, SessionHelper.RoomID, SessionHelper.CompanyID);
                        if (objBin != null && objBin.IsEnforceDefaultPullQuantity.GetValueOrDefault(false) == true)
                        {
                            if (NewPullQuantity < objBin.DefaultPullQuantity || (decimal)NewPullQuantity % (decimal)objBin.DefaultPullQuantity != 0)
                            {  
                                return Json(new { Message = string.Format(ResPullMaster.LocationPullQtyMustBeDefaultPullQty, objBin.BinNumber, objBin.DefaultPullQuantity), Status = "Fail" }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                isBinlevelEnforce = true;
                            }
                        }
                    }

                    if (oItemRecord.PullQtyScanOverride && oItemRecord.DefaultPullQuantity > 0 && isBinlevelEnforce == false)
                    {
                        if (NewPullQuantity < oItemRecord.DefaultPullQuantity || (decimal)NewPullQuantity % (decimal)oItemRecord.DefaultPullQuantity != 0)
                        { 
                            return Json(new { Message = string.Format(ResPullMaster.PullQtyMustBeDefaultPullQty, oItemRecord.DefaultPullQuantity), Status = "Fail" }, JsonRequestBehavior.AllowGet);
                        }
                    }

                    bool IsPSLimitExceed = false;
                    string locationMSG = "";
                    long ModuleId = 0;

                    try
                    {
                        #region "Check Project Spend Condition"
                        bool IsProjecSpendAllowed = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowOverrideProjectSpendLimits, eTurnsWeb.Helper.SessionHelper.PermissionType.Approval);
                        #endregion

                        //need to update other fileds as required                            

                        string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);

                        double ItemCost = lstloldPullDetailsDTO.FirstOrDefault().ItemCost.GetValueOrDefault(0);
                        double ItemPrice = lstloldPullDetailsDTO.FirstOrDefault().ItemPrice.GetValueOrDefault(0);
                        bool isMorePull = false;
                        #region Case 1: New Quantity is grater than old quantity

                        if (NewPullQuantity > OldPullQuantity)
                        {
                            //need to update other fileds as required
                            objnewPullMasterData.PoolQuantity = (NewPullQuantity - OldPullQuantity);
                            objnewPullMasterData.TempPullQTY = (NewPullQuantity - OldPullQuantity);
                            isMorePull = true;
                        }

                        #endregion
                        #region Case 2: New Quantity is less than old quantity

                        else if (NewPullQuantity < OldPullQuantity)
                        {
                            //need to update other fileds as required
                            objnewPullMasterData.PoolQuantity = (objnewPullMasterData.PoolQuantity.GetValueOrDefault(0)) - (OldPullQuantity - NewPullQuantity);
                            objnewPullMasterData.TempPullQTY = objnewPullMasterData.PoolQuantity;
                            isMorePull = false;
                        }

                        #endregion

                        objpullMasterDAL.UpdatePullQtyInPullHistory(objnewPullMasterData, objoldPullMasterData, oItemRecord, IsCreditPullNothing, SessionHelper.RoomID, SessionHelper.CompanyID, isMorePull, ModuleId, ItemCost, ItemPrice, out locationMSG, IsProjecSpendAllowed, out IsPSLimitExceed, RoomDateFormat, SessionHelper.UserID,SessionHelper.EnterPriceID,ResourceHelper.CurrentCult.Name, AllowNegetive: false);

                        if (!string.IsNullOrEmpty(locationMSG))
                        {
                            return Json(new { Message = locationMSG, LocationMSG = locationMSG, PSLimitExceed = IsPSLimitExceed, Status = "Fail" }, JsonRequestBehavior.AllowGet);
                        }

                        
                        QBItemQOHProcess((Guid)objnewPullMasterData.ItemGUID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, "Pull Edit");

                    }
                    catch (Exception ex)
                    {
                        CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                        _returnmessage = string.Format(ResMessage.SaveErrorMsg, ex.Message);
                        _returnstatus = "fail";
                    }

                    //if (_returnstatus == "ok" && string.IsNullOrWhiteSpace(locationMSG) && ModuleId == 0 && (callFrom ?? string.Empty) == "singlepull")
                    //{
                    //    try
                    //    {
                    //        string pullGUIDs = "<DataGuids>" + Convert.ToString(objnewPullMasterData.GUID) + "</DataGuids>";
                    //        string eTurnsScheduleDBName = (Convert.ToString(ConfigurationManager.AppSettings["eTurnsScheduleDBName"]) ?? "eTurnsSchedule");
                    //        NotificationDAL objNotificationDAL = new NotificationDAL(this.enterPriseDBName);
                    //        List<NotificationDTO> lstNotification = objNotificationDAL.GetCurrentNotificationListByEventName("OPC", SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                    //        if (lstNotification != null && lstNotification.Count > 0)
                    //        {
                    //            objNotificationDAL.SendMailForImmediate(lstNotification, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.EnterPriceID, eTurnsScheduleDBName, pullGUIDs);
                    //        }
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                    //    }
                    //}
                }

                #endregion

                #region For MS Pull

                else if (!string.IsNullOrWhiteSpace(PullCreditType) && PullCreditType.ToLower().Equals("ms pull"))
                {
                    IsCreditPullNothing = 2;
                    ///default pull qty validation
                    bool isBinlevelEnforce = false;
                    if (objoldPullMasterData.BinID != 0)
                    {
                        BinMasterDTO objBin = new BinMasterDAL(this.enterPriseDBName).GetBinByID(objoldPullMasterData.BinID ?? 0, SessionHelper.RoomID, SessionHelper.CompanyID);
                        if (objBin != null && objBin.IsEnforceDefaultPullQuantity.GetValueOrDefault(false) == true)
                        {
                            if (NewPullQuantity < objBin.DefaultPullQuantity || (decimal)NewPullQuantity % (decimal)objBin.DefaultPullQuantity != 0)
                            {
                                return Json(new { Message = string.Format(ResPullMaster.LocationPullQtyMustBeDefaultPullQty, objBin.BinNumber, objBin.DefaultPullQuantity), Status = "Fail" }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                isBinlevelEnforce = true;
                            }
                        }
                    }

                    if (oItemRecord.PullQtyScanOverride && oItemRecord.DefaultPullQuantity > 0 && isBinlevelEnforce == false)
                    {
                        if (NewPullQuantity < oItemRecord.DefaultPullQuantity || (decimal)NewPullQuantity % (decimal)oItemRecord.DefaultPullQuantity != 0)
                        { 
                            return Json(new { Message = string.Format(ResPullMaster.PullQtyMustBeDefaultPullQty, oItemRecord.DefaultPullQuantity), Status = "Fail" }, JsonRequestBehavior.AllowGet);
                        }
                    }

                    bool IsPSLimitExceed = false;
                    string locationMSG = "";
                    long ModuleId = 0;

                    try
                    {
                        #region "Check Project Spend Condition"
                        bool IsProjecSpendAllowed = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowOverrideProjectSpendLimits, eTurnsWeb.Helper.SessionHelper.PermissionType.Approval);
                        #endregion

                        //need to update other fileds as required                            

                        string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);

                        double ItemCost = lstloldPullDetailsDTO.FirstOrDefault().ItemCost.GetValueOrDefault(0);
                        double ItemPrice = lstloldPullDetailsDTO.FirstOrDefault().ItemPrice.GetValueOrDefault(0);
                        bool isMorePull = false;
                        #region Case 1: New Quantity is grater than old quantity

                        if (NewPullQuantity > OldPullQuantity)
                        {
                            //need to update other fileds as required
                            objnewPullMasterData.PoolQuantity = (NewPullQuantity - OldPullQuantity);
                            objnewPullMasterData.TempPullQTY = (NewPullQuantity - OldPullQuantity);
                            isMorePull = true;
                        }

                        #endregion
                        #region Case 2: New Quantity is less than old quantity

                        else if (NewPullQuantity < OldPullQuantity)
                        {
                            //need to update other fileds as required
                            objnewPullMasterData.PoolQuantity = (objnewPullMasterData.PoolQuantity.GetValueOrDefault(0)) - (OldPullQuantity - NewPullQuantity);
                            objnewPullMasterData.TempPullQTY = objnewPullMasterData.PoolQuantity.GetValueOrDefault(0);
                            isMorePull = false;
                        }

                        #endregion

                        objpullMasterDAL.UpdatePullQtyInPullHistory(objnewPullMasterData, objoldPullMasterData, oItemRecord, IsCreditPullNothing, SessionHelper.RoomID, SessionHelper.CompanyID, isMorePull, ModuleId, ItemCost, ItemPrice, out locationMSG, IsProjecSpendAllowed, out IsPSLimitExceed, RoomDateFormat, SessionHelper.UserID, SessionHelper.EnterPriceID, ResourceHelper.CurrentCult.Name, AllowNegetive: false);

                        if (!string.IsNullOrEmpty(locationMSG))
                        {
                            return Json(new { Message = locationMSG, LocationMSG = locationMSG, PSLimitExceed = IsPSLimitExceed, Status = "Fail" }, JsonRequestBehavior.AllowGet);
                        }

                    }
                    catch (Exception ex)
                    {
                        CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                        _returnmessage = string.Format(ResMessage.SaveErrorMsg, ex.Message);
                        _returnstatus = "fail";
                    }

                    //if (_returnstatus == "ok" && string.IsNullOrWhiteSpace(locationMSG) && ModuleId == 0 && (callFrom ?? string.Empty) == "singlepull")
                    //{
                    //    try
                    //    {
                    //        string pullGUIDs = "<DataGuids>" + Convert.ToString(objnewPullMasterData.GUID) + "</DataGuids>";
                    //        string eTurnsScheduleDBName = (Convert.ToString(ConfigurationManager.AppSettings["eTurnsScheduleDBName"]) ?? "eTurnsSchedule");
                    //        NotificationDAL objNotificationDAL = new NotificationDAL(this.enterPriseDBName);
                    //        List<NotificationDTO> lstNotification = objNotificationDAL.GetCurrentNotificationListByEventName("OPC", SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                    //        if (lstNotification != null && lstNotification.Count > 0)
                    //        {
                    //            objNotificationDAL.SendMailForImmediate(lstNotification, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.EnterPriceID, eTurnsScheduleDBName, pullGUIDs);
                    //        }
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                    //    }
                    //}
                }

                #endregion

                #region For Credit

                else if (!string.IsNullOrWhiteSpace(PullCreditType) && PullCreditType.ToLower().Equals("credit"))
                {
                    string ErrorMessage = string.Empty;
                    PullMasterDAL pullDAL = this.pullMasterDAL;
                    CommonDAL objCommonDAL = this.commonDAL;
                    string columnList = "ID,RoomName,IsIgnoreCreditRule,IsProjectSpendMandatory";
                    RoomDTO objRoomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");
                    List<Guid> ItemGuids = new List<Guid>();
                    List<ItemLocationDetailsDTO> itemLocations = new List<ItemLocationDetailsDTO>();
                    ItemLocationDetailsDAL ildDAL = new ItemLocationDetailsDAL(this.enterPriseDBName);
                    PullDetailsDAL pullDetailsDAL = new PullDetailsDAL(this.enterPriseDBName);

                    bool isMoreCredit = false;
                    #region Case 1: New Quantity is grater than old quantity

                    if (NewPullQuantity > OldPullQuantity)
                    {
                        //need to update other fileds as required                        
                        isMoreCredit = true;
                    }
                    #endregion
                    #region Case 2: New Quantity is less than old quantity

                    else if (NewPullQuantity < OldPullQuantity)
                    {
                        isMoreCredit = false;
                    }

                    #endregion

                    #region  For More Credit

                    if (isMoreCredit)
                    {
                        #region Get Project name by guid

                        ProjectMasterDAL projDAL = new ProjectMasterDAL(this.enterPriseDBName);
                        ProjectMasterDTO projDTO = new ProjectMasterDTO();
                        if (objnewPullMasterData.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                            projDTO = projDAL.GetRecord(objnewPullMasterData.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

                        #endregion

                        ItemInfoToCredit itemToCredit = new ItemInfoToCredit();
                        itemToCredit.EditedFrom = objnewPullMasterData.EditedFrom;
                        itemToCredit.PullGUID = objnewPullMasterData.GUID;
                        itemToCredit.SupplierAccountGuid = objnewPullMasterData.SupplierAccountGuid;
                        itemToCredit.ProjectName = (projDTO != null ? projDTO.ProjectSpendName : null);
                        itemToCredit.PullOrderNumber = objnewPullMasterData.PullOrderNumber;
                        itemToCredit.WOGuid = objnewPullMasterData.WorkOrderDetailGUID;
                        itemToCredit.UDF1 = objnewPullMasterData.UDF1;
                        itemToCredit.UDF2 = objnewPullMasterData.UDF2;
                        itemToCredit.UDF3 = objnewPullMasterData.UDF3;
                        itemToCredit.UDF4 = objnewPullMasterData.UDF4;
                        itemToCredit.UDF5 = objnewPullMasterData.UDF5;

                        if (objoldPullMasterData.BinID != 0)
                        {
                            BinMasterDTO objBin = new BinMasterDAL(this.enterPriseDBName).GetBinByID(objnewPullMasterData.BinID ?? 0, SessionHelper.RoomID, SessionHelper.CompanyID);
                            if (objBin != null)
                                itemToCredit.Bin = objBin.BinNumber;
                        }

                        double ItemCost = lstloldPullDetailsDTO.FirstOrDefault().ItemCost.GetValueOrDefault(0);
                        double ItemPrice = lstloldPullDetailsDTO.FirstOrDefault().ItemPrice.GetValueOrDefault(0);

                        //need to update other fileds as required
                        itemToCredit.Quantity = (NewPullQuantity - OldPullQuantity);

                        itemToCredit.ItemGuid = oItemRecord.GUID;
                        itemToCredit.IsModelShow = false;
                        itemToCredit.ItemNumber = oItemRecord.ItemNumber;
                        itemToCredit.ItemType = oItemRecord.ItemType;

                        itemToCredit.PrevPullsToCredit = pullDAL.GetPreviousPulls(itemToCredit, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID,SessionHelper.EnterPriceID);

                        if (oItemRecord.SerialNumberTracking)
                        {
                            string lstSerialNumber = string.Empty;
                            foreach (var SerialItem in itemToCredit.PrevPullsToCredit)
                            {
                                string serailErrorMessage = objCommonDAL.CheckDuplicateSerialNumbers(SerialItem.Serial, 0, SessionHelper.RoomID, SessionHelper.CompanyID, itemToCredit.ItemGuid.GetValueOrDefault(Guid.Empty));

                                if (serailErrorMessage.ToLower().Trim() == "duplicate")
                                {
                                    if (lstSerialNumber != string.Empty)
                                        lstSerialNumber = lstSerialNumber + " , " + SerialItem.Serial;
                                    else
                                        lstSerialNumber = SerialItem.Serial;
                                }
                            }

                            if (lstSerialNumber != string.Empty)
                            {
                                return Json(new { Message = ResPullMaster.CreditTransactionDoneForSerial + " " + lstSerialNumber, Status = "Fail" }, JsonRequestBehavior.AllowGet);
                            }
                        }

                        /* WI-4693-Allow specified rooms to ignore credit rules */

                        if (!objRoomDTO.IsIgnoreCreditRule)
                        {
                            if (itemToCredit.PrevPullsToCredit == null || itemToCredit.PrevPullsToCredit.Count == 0)
                            {
                                itemToCredit.IsModelShow = true;
                                itemToCredit.ErrorMessage = string.Format(ResPullMaster.CreditQtyGreaterThanPreviousPullQty, itemToCredit.Quantity); 
                            }
                            if (itemToCredit.IsModelShow && !string.IsNullOrWhiteSpace(itemToCredit.ErrorMessage))
                            {
                                return Json(new { Message = itemToCredit.ErrorMessage, Status = "Fail" }, JsonRequestBehavior.AllowGet);
                            }

                            List<PullDetailsDTO> prepulls = pullDAL.GetPrevPull(itemToCredit, SessionHelper.RoomID, SessionHelper.CompanyID);
                            itemLocations = new List<ItemLocationDetailsDTO>();
                            foreach (var prePullItem in prepulls)
                            {
                                itemLocations.Add(pullDAL.ConvertPullDetailtoItemLocationDetail(prePullItem, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.RoomDateFormat));
                            }

                            List<CreditHistory> lstCreditGuids = new List<CreditHistory>();
                            long SessionUserId = SessionHelper.UserID;
                            if (ildDAL.ItemLocationDetailsEditForCreditPullnew(itemLocations, NewPullQuantity,OldPullQuantity, "Credit", SessionHelper.RoomDateFormat, out lstCreditGuids, SessionUserId,SessionHelper.EnterPriceID, "credit", objnewPullMasterData.WhatWhereAction))
                            {
                                pullDAL.UpdatePullRecordsForCreditQuantity(prepulls, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, lstCreditGuids);
                                if (itemToCredit != null && itemToCredit.WOGuid.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                                {
                                    WorkOrderLineItemsDAL objWOLDAL = new WorkOrderLineItemsDAL(this.enterPriseDBName);
                                    objWOLDAL.UpdateWOItemAndTotalCost(itemToCredit.WOGuid.GetValueOrDefault(Guid.Empty).ToString(), SessionHelper.RoomID, SessionHelper.CompanyID);
                                }
                            }
                        }
                        else
                        {
                            #region WI-4693-Allow specified rooms to ignore credit rules

                            List<PullDetailsDTO> prepulls = pullDAL.GetPrevPull(itemToCredit, SessionHelper.RoomID, SessionHelper.CompanyID);

                            double TotalAvailablePulls = prepulls.Sum(x => (x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0)));
                            double TotalRemainingCredit = (itemToCredit.Quantity - TotalAvailablePulls);
                            List<PullDetailsDTO> pulls = new List<PullDetailsDTO>();
                            if (TotalRemainingCredit > 0)
                            {
                                pulls = pullDAL.GetPrevPullForCreditEntry(prepulls, itemToCredit, TotalRemainingCredit, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.RoomDateFormat, SessionHelper.EnterPriceID, ResourceHelper.CurrentCult.Name);
                                prepulls.AddRange(pulls);
                            }
                            bool IsValid = true;
                            if (prepulls.Where(x => x.EditedFrom == "Fail").Count() > 0)
                            {
                                IsValid = false;
                            }

                            if (IsValid)
                            {
                                foreach (var prePullItem in prepulls)
                                {
                                    itemLocations.Add(pullDAL.ConvertPullDetailtoItemLocationDetailForCreditRule(prePullItem, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.RoomDateFormat));
                                }
                                long SessionUserId = SessionHelper.UserID;
                                List<CreditHistory> lstCreditGuids = new List<CreditHistory>();
                                if (ildDAL.ItemLocationDetailsEditForCreditPullnew(itemLocations,NewPullQuantity,OldPullQuantity, "Credit", SessionHelper.RoomDateFormat, out lstCreditGuids, SessionUserId,SessionHelper.EnterPriceID, "Credit", objnewPullMasterData.WhatWhereAction))
                                {
                                    pullDAL.UpdatePullRecordsForCreditQuantity(prepulls, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, lstCreditGuids);
                                    if (pulls != null && pulls.Count > 0)
                                    {
                                        List<PullDetailsDTO> lstPulls = new List<PullDetailsDTO>();
                                        lstPulls.AddRange(pulls);
                                        pullDAL.InsertintoCreditHistory(lstPulls, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "Pull Credit");
                                    }
                                    if (itemToCredit != null && itemToCredit.WOGuid.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                                    {
                                        WorkOrderLineItemsDAL objWOLDAL = new WorkOrderLineItemsDAL(this.enterPriseDBName);
                                        objWOLDAL.UpdateWOItemAndTotalCost(itemToCredit.WOGuid.GetValueOrDefault(Guid.Empty).ToString(), SessionHelper.RoomID, SessionHelper.CompanyID);
                                    }
                                }
                            }
                            else
                            {
                                ErrorMessage = ErrorMessage + prepulls[0].AddedFrom + Environment.NewLine;
                            }
                            #endregion
                        }

                        if (ItemGuids.IndexOf(itemToCredit.ItemGuid.GetValueOrDefault(Guid.Empty)) < 0)
                            ItemGuids.Add(itemToCredit.ItemGuid.GetValueOrDefault(Guid.Empty));

                        if (objRoomDTO.IsIgnoreCreditRule && !string.IsNullOrEmpty(ErrorMessage))
                            return Json(new { Message = ErrorMessage, Status = "Fail" }, JsonRequestBehavior.AllowGet);

                        #region "Update Ext Cost And Avg Cost"
                        if (ItemGuids.Count > 0)
                        {
                            long SessionUserId = SessionHelper.UserID;
                            ItemMasterDAL objItemDAL = new ItemMasterDAL(this.enterPriseDBName);
                            var enterpriseId=SessionHelper.EnterPriceID;
                            foreach (var guid in ItemGuids)
                            {
                                objItemDAL.UpdateItemCost(guid, SessionHelper.RoomID, SessionHelper.CompanyID, "Web", SessionUserId,enterpriseId);
                                objItemDAL.GetAndUpdateExtCostAndAvgCost(guid, SessionHelper.RoomID, SessionHelper.CompanyID);
                            }
                            objItemDAL = null;
                        }
                        #endregion
                    }

                    #endregion

                    #region For Less Credit

                    else
                    {
                        ItemLocationQTYDTO lstLocDTO = new ItemLocationQTYDTO();
                        ItemLocationQTYDTO lstLocDTO1 = new ItemLocationQTYDTO();
                        ItemLocationQTYDAL objLocQTY = new ItemLocationQTYDAL(this.enterPriseDBName);
                        ItemMasterDAL objItemDAL = new ItemMasterDAL(this.enterPriseDBName);
                        PullMasterViewDTO obj = new PullMasterViewDTO();
                        List<PullDetailsDTO> lstPullDetails = new List<PullDetailsDTO>();
                        List<PullCreditHistoryDTO> lstPullCreditHistory = new List<PullCreditHistoryDTO>();

                        objnewPullMasterData.PoolQuantity = (objnewPullMasterData.PoolQuantity.GetValueOrDefault(0)) - (OldPullQuantity - NewPullQuantity);
                        objnewPullMasterData.TempPullQTY = objnewPullMasterData.PoolQuantity.GetValueOrDefault(0);

                        lstPullDetails = pullDetailsDAL.GetPullDetailsByPullGuid(objnewPullMasterData.GUID, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
                        lstPullDetails.ForEach(t => t.EditedFrom = "Web");

                        lstPullCreditHistory = pullDetailsDAL.GetCreditHistoryDetailsByPullGuid(objnewPullMasterData.GUID, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();

                        #region "Bin Wise Quantity Check"
                        //lstLocDTO = objLocQTY.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => x.BinID == objoldPullMasterData.BinID && x.ItemGUID == oItemRecord.GUID).FirstOrDefault();
                        lstLocDTO = objLocQTY.GetItemLocationQTY(SessionHelper.RoomID, SessionHelper.CompanyID, objoldPullMasterData.BinID, Convert.ToString(oItemRecord.GUID)).FirstOrDefault();
                        lstLocDTO1 = ildDAL.GetItemQtyByLocation(objoldPullMasterData.BinID ?? 0, oItemRecord.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                        if (lstLocDTO == null && lstLocDTO1 != null && lstLocDTO1.Quantity > 0)
                        {
                            objLocQTY.Insert(lstLocDTO1, SessionHelper.UserID,SessionHelper.EnterPriceID);
                            //lstLocDTO = objLocQTY.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => x.BinID == objoldPullMasterData.BinID && x.ItemGUID == oItemRecord.GUID).FirstOrDefault();
                            lstLocDTO = objLocQTY.GetItemLocationQTY(SessionHelper.RoomID, SessionHelper.CompanyID, objoldPullMasterData.BinID, Convert.ToString(oItemRecord.GUID)).FirstOrDefault();
                        }
                        #endregion

                        #region "Item Location & Quantity  Wise Deduction"

                        #region "ItemLocation Deduction"

                        #region "LOt and other type logic"

                        Double takenQunatity = 0;

                        foreach (var itemPull in lstPullDetails)
                        {
                            PullCreditHistoryDTO pullCreditHistoryDTO = new PullCreditHistoryDTO();
                            pullCreditHistoryDTO = lstPullCreditHistory.Where(x => x.CreditDetailGuid == itemPull.GUID).FirstOrDefault();

                            bool IsNeedToEdit = false;
                            ItemLocationDetailsDTO objItemLocationDetailsDTO = ildDAL.GetItemLocationDetailsByLocationGuid(itemPull.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);

                            Double loopCurrentTakenCustomer = 0;
                            Double loopCurrentTakenConsignment = 0;

                            if (oItemRecord.Consignment)
                            {
                                #region "Consignment Credit and Pull"

                                if (itemPull.ConsignedQuantity > ((objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity))
                                {
                                    loopCurrentTakenConsignment = (objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity;

                                    objItemLocationDetailsDTO.ConsignedQuantity -= ((itemPull.ConsignedQuantity ?? 0) - loopCurrentTakenConsignment);

                                    itemPull.ConsignedQuantity = loopCurrentTakenConsignment;

                                    if (pullCreditHistoryDTO != null)
                                    {
                                        pullCreditHistoryDTO.CreditConsignedQuantity = loopCurrentTakenConsignment;
                                    }

                                    takenQunatity += loopCurrentTakenConsignment;

                                    IsNeedToEdit = true;
                                    goto Save;
                                }
                                else
                                {
                                    loopCurrentTakenConsignment = itemPull.ConsignedQuantity.GetValueOrDefault(0);
                                    itemPull.ConsignedQuantity = loopCurrentTakenConsignment;
                                    if (pullCreditHistoryDTO != null)
                                    {
                                        pullCreditHistoryDTO.CreditConsignedQuantity = loopCurrentTakenConsignment;
                                    }
                                    takenQunatity += itemPull.ConsignedQuantity.GetValueOrDefault(0);
                                    IsNeedToEdit = false;
                                }
                                #endregion
                            }
                            else
                            {
                                #region "Customreowned Credit and Pull"

                                if (itemPull.CustomerOwnedQuantity > ((objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity))
                                {
                                    loopCurrentTakenCustomer = (objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity;

                                    objItemLocationDetailsDTO.CustomerOwnedQuantity -= ((itemPull.CustomerOwnedQuantity ?? 0) - loopCurrentTakenCustomer);

                                    itemPull.CustomerOwnedQuantity = loopCurrentTakenCustomer;
                                    if (pullCreditHistoryDTO != null)
                                    {
                                        pullCreditHistoryDTO.CreditCustomerOwnedQuantity = loopCurrentTakenCustomer;
                                    }
                                    takenQunatity += loopCurrentTakenCustomer;
                                    IsNeedToEdit = true;
                                    goto Save;
                                }
                                else
                                {
                                    loopCurrentTakenCustomer = itemPull.CustomerOwnedQuantity.GetValueOrDefault(0);
                                    itemPull.CustomerOwnedQuantity = loopCurrentTakenCustomer;
                                    if (pullCreditHistoryDTO != null)
                                    {
                                        pullCreditHistoryDTO.CreditCustomerOwnedQuantity = loopCurrentTakenCustomer;
                                    }
                                    takenQunatity += itemPull.CustomerOwnedQuantity.GetValueOrDefault(0);
                                    IsNeedToEdit = false;
                                }
                                #endregion
                            }
                        Save:
                            if (IsNeedToEdit)
                            {
                                ildDAL.Edit(objItemLocationDetailsDTO);
                                itemPull.PullCredit = objnewPullMasterData.PullCredit;
                                obj = pullDAL.UpdatetoPullDetailForCredit(itemPull, pullCreditHistoryDTO, objnewPullMasterData.GUID, objnewPullMasterData.ProjectSpendGUID != null ? objnewPullMasterData.ProjectSpendGUID.Value : objnewPullMasterData.ProjectSpendGUID, itemPull.ItemCost, objnewPullMasterData.LastUpdatedBy, loopCurrentTakenCustomer, loopCurrentTakenConsignment, itemPull.ItemPrice);
                            }
                        }

                        #endregion

                        #endregion

                        #region "ItemLocation Quantity Deduction"

                        oItemRecord.OnHandQuantity = oItemRecord.OnHandQuantity - ((obj.ConsignedQuantity.GetValueOrDefault(0) + obj.CustomerOwnedQuantity.GetValueOrDefault(0)) - objnewPullMasterData.PoolQuantity.GetValueOrDefault(0));

                        if (oItemRecord.Consignment)
                        {
                            //Both's sum we have available.
                            if (lstLocDTO != null)
                            {
                                if (lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) <= 0)
                                {
                                    obj.ConsignedQuantity = objnewPullMasterData.TempPullQTY ?? 0;
                                    lstLocDTO.ConsignedQuantity = lstLocDTO.ConsignedQuantity.GetValueOrDefault(0) + (objnewPullMasterData.TempPullQTY ?? 0);
                                    lstLocDTO.Quantity = lstLocDTO.Quantity + (objnewPullMasterData.TempPullQTY ?? 0);
                                }
                                else if (lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) >= (objnewPullMasterData.TempPullQTY ?? 0))
                                {
                                    obj.CustomerOwnedQuantity = objnewPullMasterData.TempPullQTY ?? 0;
                                    lstLocDTO.CustomerOwnedQuantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) + (objnewPullMasterData.TempPullQTY ?? 0);
                                    lstLocDTO.Quantity = lstLocDTO.Quantity + (objnewPullMasterData.TempPullQTY ?? 0);
                                }
                                else if (lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) < (objnewPullMasterData.TempPullQTY ?? 0))
                                {
                                    double cstqty = (objnewPullMasterData.TempPullQTY ?? 0) + lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0);
                                    double consqty = cstqty;
                                    lstLocDTO.ConsignedQuantity = lstLocDTO.ConsignedQuantity + consqty;
                                    obj.CustomerOwnedQuantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0);
                                    obj.ConsignedQuantity = consqty;
                                    lstLocDTO.CustomerOwnedQuantity = 0;
                                    lstLocDTO.Quantity = lstLocDTO.Quantity + (obj.CustomerOwnedQuantity.GetValueOrDefault(0) + obj.ConsignedQuantity.GetValueOrDefault(0));
                                }
                            }
                        }
                        else
                        {
                            if (lstLocDTO != null)
                            {
                                lstLocDTO.CustomerOwnedQuantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) + objnewPullMasterData.TempPullQTY.GetValueOrDefault(0);
                                lstLocDTO.Quantity = lstLocDTO.Quantity + (objnewPullMasterData.TempPullQTY ?? 0);
                            }
                            obj.CustomerOwnedQuantity = objnewPullMasterData.TempPullQTY ?? 0;
                        }

                        #endregion

                        #region "Saving Location and QTY data"                       

                        oItemRecord.WhatWhereAction = objnewPullMasterData.PullCredit;
                        objItemDAL.Edit(oItemRecord, SessionHelper.UserID, SessionHelper.EnterPriceID);
                        List<ItemLocationQTYDTO> lstUpdate = new List<ItemLocationQTYDTO>();
                        if (lstLocDTO == null || lstLocDTO.ID == 0)
                        {
                            lstLocDTO = new ItemLocationQTYDTO();
                            lstLocDTO.ID = 0;
                            lstLocDTO.BinID = objnewPullMasterData.BinID.GetValueOrDefault(0);
                            lstLocDTO.Quantity = objnewPullMasterData.PoolQuantity.GetValueOrDefault();
                            lstLocDTO.CustomerOwnedQuantity = objnewPullMasterData.CustomerOwnedQuantity;
                            lstLocDTO.ConsignedQuantity = objnewPullMasterData.ConsignedQuantity;
                            lstLocDTO.LotNumber = objnewPullMasterData.LotNumber;
                            lstLocDTO.GUID = Guid.NewGuid();
                            lstLocDTO.ItemGUID = oItemRecord.GUID;
                            lstLocDTO.Created = DateTimeUtility.DateTimeNow;
                            lstLocDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                            lstLocDTO.CreatedBy = SessionHelper.UserID;
                            lstLocDTO.LastUpdatedBy = SessionHelper.UserID;
                            lstLocDTO.Room = SessionHelper.RoomID;
                            lstLocDTO.CompanyID = SessionHelper.CompanyID;
                            lstLocDTO.AddedFrom = "Web";
                            lstLocDTO.EditedFrom = "Web";
                            lstLocDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            lstLocDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                            objLocQTY.Insert(lstLocDTO, SessionHelper.UserID,SessionHelper.EnterPriceID);
                        }
                        lstUpdate.Add(lstLocDTO);
                        objLocQTY.Save(lstUpdate, SessionHelper.UserID,SessionHelper.EnterPriceID);
                        #endregion

                        #endregion

                        #region "Project Spend Quantity Update"

                        if (obj != null && (obj.CustomerOwnedQuantity.GetValueOrDefault(0) == 0 || obj.ConsignedQuantity.GetValueOrDefault(0) == 0))
                        {
                            List<PullDetailsDTO> lstPullDtl = pullDetailsDAL.GetPullDetailsByPullGuidPlain(obj.GUID, obj.Room.GetValueOrDefault(0), obj.CompanyID.GetValueOrDefault(0));
                            if (lstPullDtl != null && lstPullDtl.Count > 0)
                            {
                                double OldCreditCost = obj.PullCost ?? 0;
                                double OldCreditQuantity = obj.PoolQuantity ?? 0;

                                obj.CustomerOwnedQuantity = lstPullDtl.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0));
                                obj.ConsignedQuantity = lstPullDtl.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0));
                                obj.PoolQuantity = (
                                                    lstPullDtl.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0))
                                                        +
                                                    lstPullDtl.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0))
                                                    );
                                obj.PullCost = lstPullDtl.Sum(x => x.PoolQuantity.GetValueOrDefault(0) * x.ItemCost.GetValueOrDefault(0));
                                obj.PullPrice = lstPullDtl.Sum(x => x.PoolQuantity.GetValueOrDefault(0) * x.ItemPrice.GetValueOrDefault(0));
                                obj.WhatWhereAction = objnewPullMasterData.WhatWhereAction;

                                obj.ItemOnhandQty = oItemRecord.OnHandQuantity;
                                obj.ItemStageQty = oItemRecord.StagedQuantity;
                                obj.ItemLocationOnHandQty = 0;

                                ItemLocationQTYDTO objItemLocationQuantity = ildDAL.GetItemQtyByLocation(objoldPullMasterData.BinID ?? 0, oItemRecord.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                                if (objItemLocationQuantity != null && objItemLocationQuantity.BinID > 0)
                                {
                                    obj.ItemLocationOnHandQty = objItemLocationQuantity.CustomerOwnedQuantity.GetValueOrDefault(0) + objItemLocationQuantity.ConsignedQuantity.GetValueOrDefault(0);
                                }

                                pullDAL.EditForPullQty(obj);
                                pullDAL.InsertPullEditHistory(obj.GUID, obj.PoolQuantity.GetValueOrDefault(0), OldPullQuantity, objnewPullMasterData.WhatWhereAction);
                                //pullDAL.Edit(obj);

                                double DiffPullCost = (OldCreditCost - (obj.PullCost ?? 0));
                                double DiffPoolQuantity = (OldCreditQuantity - (obj.PoolQuantity ?? 0));

                                if (objnewPullMasterData.ProjectSpendGUID != null && objnewPullMasterData.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                                {
                                    pullDAL.UpdateProjectSpendWithCostEditCredit(oItemRecord, obj, DiffPullCost, DiffPoolQuantity, obj.ProjectSpendGUID.Value, SessionHelper.RoomID, SessionHelper.CompanyID);
                                }
                            }
                        }

                        #endregion

                        #region "Update Ext Cost And Avg Cost"
                        if (ItemGuids.Count > 0)
                        {
                            long SessionUserId = SessionHelper.UserID;
                            var enterpriseId = SessionHelper.EnterPriceID;
                            foreach (var guid in ItemGuids)
                            {
                                objItemDAL.UpdateItemCost(guid, SessionHelper.RoomID, SessionHelper.CompanyID, "Web", SessionUserId,enterpriseId);
                                objItemDAL.GetAndUpdateExtCostAndAvgCost(guid, SessionHelper.RoomID, SessionHelper.CompanyID);
                            }
                            objItemDAL = null;
                        }
                        #endregion

                        if (objnewPullMasterData != null && objnewPullMasterData.WorkOrderDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                        {
                            WorkOrderLineItemsDAL objWOLDAL = new WorkOrderLineItemsDAL(this.enterPriseDBName);
                            objWOLDAL.UpdateWOItemAndTotalCost(objnewPullMasterData.WorkOrderDetailGUID.GetValueOrDefault(Guid.Empty).ToString(), SessionHelper.RoomID, SessionHelper.CompanyID);
                        }

                        pullDAL.UpdateCumulativeOnHand(obj);
                    }

                    #endregion

                     
                    
                    QBItemQOHProcess((Guid)objnewPullMasterData.ItemGUID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, "Credit Edit");

                }

                #endregion

                #region For MS Credit

                else if (!string.IsNullOrWhiteSpace(PullCreditType) && PullCreditType.ToLower().Equals("ms credit"))
                {
                    string ErrorMessage = string.Empty;
                    PullMasterDAL pullDAL = this.pullMasterDAL;
                    CommonDAL objCommonDAL = this.commonDAL;
                    string columnList = "ID,RoomName,IsIgnoreCreditRule,IsProjectSpendMandatory";
                    RoomDTO objRoomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");
                    List<Guid> ItemGuids = new List<Guid>();
                    List<MaterialStagingPullDetailDTO> materialStagingPullDetailDTO = new List<MaterialStagingPullDetailDTO>();
                    MaterialStagingPullDetailDAL mspdDAL = new MaterialStagingPullDetailDAL(this.enterPriseDBName);
                    PullDetailsDAL pullDetailsDAL = new PullDetailsDAL(this.enterPriseDBName);

                    bool isMoreCredit = false;
                    #region Case 1: New Quantity is grater than old quantity

                    if (NewPullQuantity > OldPullQuantity)
                    {
                        //need to update other fileds as required                        
                        isMoreCredit = true;
                    }
                    #endregion
                    #region Case 2: New Quantity is less than old quantity

                    else if (NewPullQuantity < OldPullQuantity)
                    {
                        isMoreCredit = false;
                    }

                    #endregion

                    #region  For More Credit

                    if (isMoreCredit)
                    {
                        #region Get Project name by guid

                        ProjectMasterDAL projDAL = new ProjectMasterDAL(this.enterPriseDBName);
                        ProjectMasterDTO projDTO = new ProjectMasterDTO();
                        if (objnewPullMasterData.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                            projDTO = projDAL.GetRecord(objnewPullMasterData.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

                        #endregion

                        ItemInfoToMSCredit itemToCredit = new ItemInfoToMSCredit();
                        itemToCredit.EditedFrom = objnewPullMasterData.EditedFrom;
                        itemToCredit.PullGUID = objnewPullMasterData.GUID;
                        itemToCredit.SupplierAccountGuid = objnewPullMasterData.SupplierAccountGuid;
                        itemToCredit.ProjectName = (projDTO != null ? projDTO.ProjectSpendName : null);
                        itemToCredit.PullOrderNumber = objnewPullMasterData.PullOrderNumber;
                        itemToCredit.WOGuid = objnewPullMasterData.WorkOrderDetailGUID;
                        itemToCredit.UDF1 = objnewPullMasterData.UDF1;
                        itemToCredit.UDF2 = objnewPullMasterData.UDF2;
                        itemToCredit.UDF3 = objnewPullMasterData.UDF3;
                        itemToCredit.UDF4 = objnewPullMasterData.UDF4;
                        itemToCredit.UDF5 = objnewPullMasterData.UDF5;

                        if (objoldPullMasterData.BinID != 0)
                        {
                            BinMasterDTO objBin = new BinMasterDAL(this.enterPriseDBName).GetBinByID(objnewPullMasterData.BinID ?? 0, SessionHelper.RoomID, SessionHelper.CompanyID);
                            if (objBin != null)
                                itemToCredit.Bin = objBin.BinNumber;
                        }

                        double ItemCost = lstloldPullDetailsDTO.FirstOrDefault().ItemCost.GetValueOrDefault(0);
                        double ItemPrice = lstloldPullDetailsDTO.FirstOrDefault().ItemPrice.GetValueOrDefault(0);

                        //need to update other fileds as required
                        itemToCredit.Quantity = (NewPullQuantity - OldPullQuantity);

                        itemToCredit.ItemGuid = oItemRecord.GUID;
                        itemToCredit.IsModelShow = false;
                        itemToCredit.ItemNumber = oItemRecord.ItemNumber;
                        itemToCredit.ItemType = oItemRecord.ItemType;

                        itemToCredit.PrevPullsToMSCredit = pullDAL.GetPrevLoadMSPull(itemToCredit, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID, ResourceHelper.CurrentCult.Name);

                        /* WI-4693-Allow specified rooms to ignore credit rules */

                        if (!objRoomDTO.IsIgnoreCreditRule)
                        {
                            if (itemToCredit.PrevPullsToMSCredit == null || itemToCredit.PrevPullsToMSCredit.Count == 0)
                            {
                                itemToCredit.IsModelShow = true;
                                itemToCredit.ErrorMessage = string.Format(ResPullMaster.CreditQtyGreaterThanPreviousPullQty, itemToCredit.Quantity); 
                            }

                            if (itemToCredit.IsModelShow && !string.IsNullOrWhiteSpace(itemToCredit.ErrorMessage))
                            {
                                return Json(new { Message = itemToCredit.ErrorMessage, Status = "Fail" }, JsonRequestBehavior.AllowGet);
                            }

                            List<PullDetailsDTO> prepulls = pullDAL.GetPrevMSPull(itemToCredit, SessionHelper.RoomID, SessionHelper.CompanyID);

                            bool IsValid = true;

                            if (prepulls.Where(x => x.EditedFrom == "Fail").Count() > 0)
                            {
                                IsValid = false;
                            }

                            if (IsValid)
                            {
                                List<MaterialStagingPullDetailDTO> MSData = new List<MaterialStagingPullDetailDTO>();
                                ItemLocationDetailsDAL ildDAL = new ItemLocationDetailsDAL(this.enterPriseDBName);

                                foreach (var prePullItem in prepulls)
                                {
                                    if (itemToCredit.Bin == null && string.IsNullOrEmpty(itemToCredit.Bin))
                                    {
                                        prePullItem.CreditBinName = "[|EmptyStagingBin|]";
                                    }
                                    MSData.Add(pullDAL.ConvertPullDetailtoMaterialStaginPullDetail(prePullItem, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.RoomDateFormat));
                                }

                                List<CreditHistory> lstCreditGuids = new List<CreditHistory>();
                                long SessionUserId = SessionHelper.UserID;
                                if (ildDAL.MaterialStagingPoolDetailsEditForMSCreditPullnew(MSData,NewPullQuantity,OldPullQuantity, itemToCredit.PullGUID.GetValueOrDefault(Guid.Empty), "MS Credit", SessionHelper.RoomDateFormat, out lstCreditGuids, SessionUserId,SessionHelper.EnterPriceID, "ms credit", objnewPullMasterData.WhatWhereAction))
                                {
                                    pullDAL.UpdatePullRecordsForMSCreditQuantity(prepulls, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, lstCreditGuids);
                                }
                            }
                            else
                            {
                                ErrorMessage = ErrorMessage + prepulls[0].AddedFrom + Environment.NewLine;
                            }
                        }
                        else
                        {
                            #region WI-4693-Allow specified rooms to ignore credit rules

                            List<PullDetailsDTO> prepulls = pullDAL.GetPrevMSPull(itemToCredit, SessionHelper.RoomID, SessionHelper.CompanyID);

                            double TotalAvailablePulls = prepulls.Sum(x => (x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0)));
                            double TotalRemainingCredit = (itemToCredit.Quantity - TotalAvailablePulls);

                            List<PullDetailsDTO> pulls = new List<PullDetailsDTO>();
                            if (TotalRemainingCredit > 0)
                            {
                                pulls = pullDAL.GetPrevMSPullForCreditEntry(prepulls, itemToCredit, TotalRemainingCredit, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.RoomDateFormat, SessionHelper.CurrentTimeZone, SessionHelper.EnterPriceID, ResourceHelper.CurrentCult.Name);
                                prepulls.AddRange(pulls);
                            }
                            bool IsValid = true;

                            if (prepulls.Where(x => x.EditedFrom == "Fail").Count() > 0)
                            {
                                IsValid = false;
                            }

                            if (IsValid)
                            {
                                List<MaterialStagingPullDetailDTO> MSData = new List<MaterialStagingPullDetailDTO>();
                                ItemLocationDetailsDAL ildDAL = new ItemLocationDetailsDAL(this.enterPriseDBName);
                                foreach (var prePullItem in prepulls)
                                {
                                    if (itemToCredit.Bin == null && string.IsNullOrEmpty(itemToCredit.Bin))
                                    {
                                        prePullItem.CreditBinName = "[|EmptyStagingBin|]";
                                    }

                                    MSData.Add(pullDAL.ConvertPullDetailtoMaterialStaginPullDetailForCreditRule(prePullItem, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.RoomDateFormat, itemToCredit.QLGuid.GetValueOrDefault(Guid.Empty)));
                                }
                                List<CreditHistory> lstCreditGuids = new List<CreditHistory>();

                                if (ildDAL.MaterialStagingPoolDetailsEditForMSCreditPullnew(MSData,NewPullQuantity,OldPullQuantity, itemToCredit.PullGUID.GetValueOrDefault(Guid.Empty), "MS Credit", SessionHelper.RoomDateFormat, out lstCreditGuids, SessionHelper.UserID,SessionHelper.EnterPriceID, "ms credit", objnewPullMasterData.WhatWhereAction))
                                {
                                    pullDAL.UpdatePullRecordsForMSCreditQuantity(prepulls, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, lstCreditGuids);
                                    if (pulls != null && pulls.Count > 0)
                                    {
                                        List<PullDetailsDTO> lstPulls = new List<PullDetailsDTO>();
                                        lstPulls.AddRange(pulls);
                                        pullDAL.InsertintoCreditHistory(lstPulls, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "MS Pull Credit");
                                    }
                                }
                            }
                            else
                            {
                                ErrorMessage = ErrorMessage + prepulls[0].AddedFrom + Environment.NewLine;
                            }

                            #endregion
                        }

                        if (ItemGuids.IndexOf(itemToCredit.ItemGuid.GetValueOrDefault(Guid.Empty)) < 0)
                            ItemGuids.Add(itemToCredit.ItemGuid.GetValueOrDefault(Guid.Empty));

                        if (objRoomDTO.IsIgnoreCreditRule && !string.IsNullOrEmpty(ErrorMessage))
                            return Json(new { Message = ErrorMessage, Status = "Fail" }, JsonRequestBehavior.AllowGet);

                        //#region "Update Ext Cost And Avg Cost"
                        //if (ItemGuids.Count > 0)
                        //{
                        //    long SessionUserId = SessionHelper.UserID;
                        //    ItemMasterDAL objItemDAL = new ItemMasterDAL(this.enterPriseDBName);
                        //    foreach (var guid in ItemGuids)
                        //    {
                        //        objItemDAL.UpdateItemCost(guid, SessionHelper.RoomID, SessionHelper.CompanyID, "Web", SessionUserId);
                        //        objItemDAL.GetAndUpdateExtCostAndAvgCost(guid, SessionHelper.RoomID, SessionHelper.CompanyID);
                        //    }
                        //    objItemDAL = null;
                        //}
                        //#endregion
                    }

                    #endregion

                    #region For Less Credit

                    else
                    {
                        ItemMasterDAL objItemDAL = new ItemMasterDAL(this.enterPriseDBName);
                        PullMasterViewDTO obj = new PullMasterViewDTO();
                        List<PullDetailsDTO> lstPullDetails = new List<PullDetailsDTO>();
                        List<PullCreditHistoryDTO> lstPullCreditHistory = new List<PullCreditHistoryDTO>();
                        MaterialStagingPullDetailDAL objMaterialStagingPullDetailDAL = new MaterialStagingPullDetailDAL(this.enterPriseDBName);
                        MaterialStagingDetailDAL objMaterialStagingDetailDAL = new MaterialStagingDetailDAL(this.enterPriseDBName);

                        objnewPullMasterData.PoolQuantity = (objnewPullMasterData.PoolQuantity.GetValueOrDefault(0)) - (OldPullQuantity - NewPullQuantity);
                        objnewPullMasterData.TempPullQTY = objnewPullMasterData.PoolQuantity.GetValueOrDefault(0);

                        lstPullDetails = pullDetailsDAL.GetPullDetailsByPullGuid(objnewPullMasterData.GUID, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
                        lstPullDetails.ForEach(t => t.EditedFrom = "Web");

                        lstPullCreditHistory = pullDetailsDAL.GetCreditHistoryDetailsByPullGuid(objnewPullMasterData.GUID, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();

                        #region "Item Location & Quantity  Wise Deduction"

                        #region "ItemLocation Deduction"

                        #region "LOt and other type logic"

                        Double takenQunatity = 0;

                        foreach (var itemPull in lstPullDetails)
                        {
                            double OldpullQty = itemPull.PoolQuantity.GetValueOrDefault(0);
                            double NewpullQty = 0;

                            PullCreditHistoryDTO pullCreditHistoryDTO = new PullCreditHistoryDTO();
                            pullCreditHistoryDTO = lstPullCreditHistory.Where(x => x.CreditDetailGuid == itemPull.GUID).FirstOrDefault();

                            bool IsNeedToEdit = false;
                            MaterialStagingPullDetailDTO objMaterialStagingPullDetailDTO = objMaterialStagingPullDetailDAL.GetRecord(itemPull.MaterialStagingPullDetailGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);

                            Double loopCurrentTakenCustomer = 0;
                            Double loopCurrentTakenConsignment = 0;

                            if (oItemRecord.Consignment)
                            {
                                #region "Consignment Credit and Pull"

                                if (itemPull.ConsignedQuantity > ((objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity))
                                {
                                    loopCurrentTakenConsignment = (objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity;

                                    objMaterialStagingPullDetailDTO.ConsignedQuantity -= ((itemPull.ConsignedQuantity ?? 0) - loopCurrentTakenConsignment);

                                    itemPull.ConsignedQuantity = loopCurrentTakenConsignment;

                                    if (pullCreditHistoryDTO != null)
                                    {
                                        pullCreditHistoryDTO.CreditConsignedQuantity = loopCurrentTakenConsignment;
                                    }

                                    takenQunatity += loopCurrentTakenConsignment;

                                    IsNeedToEdit = true;
                                    goto Save;
                                }
                                else
                                {
                                    loopCurrentTakenConsignment = itemPull.ConsignedQuantity.GetValueOrDefault(0);
                                    itemPull.ConsignedQuantity = loopCurrentTakenConsignment;
                                    if (pullCreditHistoryDTO != null)
                                    {
                                        pullCreditHistoryDTO.CreditConsignedQuantity = loopCurrentTakenConsignment;
                                    }
                                    takenQunatity += itemPull.ConsignedQuantity.GetValueOrDefault(0);
                                    IsNeedToEdit = false;
                                }
                                #endregion
                            }
                            else
                            {
                                #region "Customreowned Credit and Pull"

                                if (itemPull.CustomerOwnedQuantity > ((objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity))
                                {
                                    loopCurrentTakenCustomer = (objnewPullMasterData.TempPullQTY ?? 0) - takenQunatity;

                                    objMaterialStagingPullDetailDTO.CustomerOwnedQuantity -= ((itemPull.CustomerOwnedQuantity ?? 0) - loopCurrentTakenCustomer);

                                    itemPull.CustomerOwnedQuantity = loopCurrentTakenCustomer;
                                    if (pullCreditHistoryDTO != null)
                                    {
                                        pullCreditHistoryDTO.CreditCustomerOwnedQuantity = loopCurrentTakenCustomer;
                                    }
                                    takenQunatity += loopCurrentTakenCustomer;
                                    IsNeedToEdit = true;
                                    goto Save;
                                }
                                else
                                {
                                    loopCurrentTakenCustomer = itemPull.CustomerOwnedQuantity.GetValueOrDefault(0);
                                    itemPull.CustomerOwnedQuantity = loopCurrentTakenCustomer;
                                    if (pullCreditHistoryDTO != null)
                                    {
                                        pullCreditHistoryDTO.CreditCustomerOwnedQuantity = loopCurrentTakenCustomer;
                                    }
                                    takenQunatity += itemPull.CustomerOwnedQuantity.GetValueOrDefault(0);
                                    IsNeedToEdit = false;
                                }
                                #endregion
                            }
                        Save:
                            if (IsNeedToEdit)
                            {
                                objMaterialStagingPullDetailDTO.PoolQuantity = (objMaterialStagingPullDetailDTO.ConsignedQuantity.GetValueOrDefault(0) + objMaterialStagingPullDetailDTO.CustomerOwnedQuantity.GetValueOrDefault(0));
                                objMaterialStagingPullDetailDAL.Edit(objMaterialStagingPullDetailDTO);
                                itemPull.PullCredit = objnewPullMasterData.PullCredit;
                                obj = pullDAL.UpdatetoPullDetailForCredit(itemPull, pullCreditHistoryDTO, objnewPullMasterData.GUID, objnewPullMasterData.ProjectSpendGUID != null ? objnewPullMasterData.ProjectSpendGUID.Value : objnewPullMasterData.ProjectSpendGUID, itemPull.ItemCost, objnewPullMasterData.LastUpdatedBy, loopCurrentTakenCustomer, loopCurrentTakenConsignment, itemPull.ItemPrice);

                                NewpullQty = (itemPull.CustomerOwnedQuantity.GetValueOrDefault(0) + itemPull.ConsignedQuantity.GetValueOrDefault(0));

                                //MaterialStagingDetailDTO objmsddto = objMaterialStagingDetailDAL.GetRecord(objMaterialStagingPullDetailDTO.MaterialStagingdtlGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                                MaterialStagingDetailDTO objmsddto = objMaterialStagingDetailDAL.GetMaterialStagingDetailByGUID(objMaterialStagingPullDetailDTO.MaterialStagingdtlGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);

                                if (objmsddto != null)
                                {
                                    objmsddto.Quantity = objmsddto.Quantity - (OldpullQty - NewpullQty);
                                    objMaterialStagingDetailDAL.Edit(objmsddto);
                                }
                            }
                        }

                        #endregion

                        #endregion

                        #region "ItemLocation Quantity Deduction"

                        if (oItemRecord.Consignment)
                        {
                            oItemRecord.StagedQuantity = oItemRecord.StagedQuantity - (objnewPullMasterData.ConsignedQuantity.GetValueOrDefault(0) - objnewPullMasterData.TempPullQTY);
                            obj.ConsignedQuantity = objnewPullMasterData.TempPullQTY ?? 0;
                        }
                        else
                        {
                            oItemRecord.StagedQuantity = oItemRecord.StagedQuantity - (objnewPullMasterData.CustomerOwnedQuantity.GetValueOrDefault(0) - objnewPullMasterData.TempPullQTY);
                            obj.CustomerOwnedQuantity = objnewPullMasterData.TempPullQTY ?? 0;
                        }

                        #endregion

                        #endregion

                        //Update started quantity...
                        objMaterialStagingPullDetailDAL.UpdateStagedQuantity(oItemRecord.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID,SessionHelper.EnterPriceID);
                        //Updated PS

                        #region "Project Spend Quantity Update"

                        if (obj != null && (obj.CustomerOwnedQuantity.GetValueOrDefault(0) == 0 || obj.ConsignedQuantity.GetValueOrDefault(0) == 0))
                        {
                            List<PullDetailsDTO> lstPullDtl = pullDetailsDAL.GetPullDetailsByPullGuidPlain(obj.GUID, obj.Room.GetValueOrDefault(0), obj.CompanyID.GetValueOrDefault(0));
                            if (lstPullDtl != null && lstPullDtl.Count > 0)
                            {
                                double OldCreditCost = obj.PullCost ?? 0;
                                double OldCreditQuantity = obj.PoolQuantity ?? 0;

                                obj.CustomerOwnedQuantity = lstPullDtl.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0));
                                obj.ConsignedQuantity = lstPullDtl.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0));
                                obj.PoolQuantity = (
                                                    lstPullDtl.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0))
                                                        +
                                                    lstPullDtl.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0))
                                                    );
                                obj.PullCost = lstPullDtl.Sum(x => x.PoolQuantity.GetValueOrDefault(0) * x.ItemCost.GetValueOrDefault(0));
                                obj.PullPrice = lstPullDtl.Sum(x => x.PoolQuantity.GetValueOrDefault(0) * x.ItemPrice.GetValueOrDefault(0));
                                obj.WhatWhereAction = objnewPullMasterData.WhatWhereAction;

                                obj.ItemOnhandQty = oItemRecord.OnHandQuantity;
                                obj.ItemStageQty = oItemRecord.StagedQuantity;
                                obj.ItemStageLocationQty = 0;

                                MaterialStagingPullDetailDTO objItemLocationStageQuantity = objMaterialStagingPullDetailDAL.GetItemStagingQtyByLocation(objoldPullMasterData.BinID ?? 0, oItemRecord.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                                if (objItemLocationStageQuantity != null && objItemLocationStageQuantity.StagingBinId > 0)
                                {
                                    obj.ItemStageLocationQty = objItemLocationStageQuantity.CustomerOwnedQuantity.GetValueOrDefault(0) + objItemLocationStageQuantity.ConsignedQuantity.GetValueOrDefault(0);
                                }

                                pullDAL.EditForPullQty(obj);
                                pullDAL.InsertPullEditHistory(obj.GUID, obj.PoolQuantity.GetValueOrDefault(0), OldPullQuantity, objnewPullMasterData.WhatWhereAction);
                                //pullDAL.Edit(obj);

                                double DiffPullCost = (OldCreditCost - (obj.PullCost ?? 0));
                                double DiffPoolQuantity = (OldCreditQuantity - (obj.PoolQuantity ?? 0));

                                if (objnewPullMasterData.ProjectSpendGUID != null && objnewPullMasterData.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                                {
                                    pullDAL.UpdateProjectSpendWithCostEditCredit(oItemRecord, obj, DiffPullCost, DiffPoolQuantity, obj.ProjectSpendGUID.Value, SessionHelper.RoomID, SessionHelper.CompanyID);
                                }
                            }
                        }

                        #endregion

                        pullDAL.UpdateCumulativeOnHand(obj);
                    }

                    #endregion
                }

                #endregion
            }
            else
            {
                _returnmessage = ResPullMaster.PullInfoNotAvailable; 
                _returnstatus = "Fail";
            }
            return Json(new { Message = _returnmessage, Status = _returnstatus }, JsonRequestBehavior.AllowGet);
        }

        private void QBItemQOHProcess(Guid ItemGUID, Int64 CompanyID, Int64 RoomID, long SessionUserId, string WhatWhereAction)
        {
            QuickBookItemDAL objQBItemDAL = new QuickBookItemDAL(SessionHelper.EnterPriseDBName);
            //objQBItemDAL.InsertQuickBookItem(ItemGUID, SessionHelper.EnterPriceID, CompanyID, RoomID, "Update", false, SessionUserId, "Web", null, WhatWhereAction);
        }

        public ActionResult PullLotSrSelectionForCreditEdit(Guid PullGUID, Guid ItemGuid, double OldCreditQuantity, double NewCreditQuantity, string PullCreditType)
        {
            string _returnmessage = ResMessage.SaveMessage;
            string _returnstatus = "";

            #region take item data

            ItemMasterDTO oItemRecord = new ItemMasterDAL(this.enterPriseDBName).GetItemWithoutJoins(null, ItemGuid);

            #endregion

            #region take Pull Master data from pull guid

            PullMasterDAL objpullMasterDAL = this.pullMasterDAL;
            PullMasterViewDTO objoldPullMasterData = new PullMasterViewDTO();
            PullMasterViewDTO objnewPullMasterData = new PullMasterViewDTO();

            objoldPullMasterData = objpullMasterDAL.GetPullByGuidPlain(PullGUID);
            objnewPullMasterData = objoldPullMasterData;

            //need to update oter fileds

            #endregion

            #region Check project spend or bin deleted

            BinMasterDTO objBinCheck = new BinMasterDAL(this.enterPriseDBName).GetBinByID(objoldPullMasterData.BinID ?? 0, SessionHelper.RoomID, SessionHelper.CompanyID);
            if (objBinCheck != null && objBinCheck.ID > 0 && (objBinCheck.IsDeleted.GetValueOrDefault(false) == true || objBinCheck.IsArchived.GetValueOrDefault(false) == true))
            {
                return Json(new { Message = string.Format(ResPullMaster.DeletedSoPullUpdateNotAllowed, objBinCheck.BinNumber), Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
            else if (objBinCheck == null)
            {
                return Json(new { Message = string.Format(ResPullMaster.DeletedSoPullUpdateNotAllowed, objBinCheck.BinNumber), Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }

            ProjectMasterDAL objPrjMsgDAL = new ProjectMasterDAL(this.enterPriseDBName);
            ProjectMasterDTO objPrjMstDTO = new ProjectMasterDTO();
            objPrjMstDTO = objPrjMsgDAL.GetProjectMasterByGuidNormal(objoldPullMasterData.ProjectSpendGUID.GetValueOrDefault(Guid.Empty));
            if (objPrjMstDTO != null && objPrjMstDTO.ID > 0 && (objPrjMstDTO.IsDeleted.GetValueOrDefault(false) == true || objPrjMstDTO.IsArchived.GetValueOrDefault(false) == true))
            { 
                return Json(new { Message = string.Format(ResPullMaster.DeletedSoPullUpdateNotAllowed, objPrjMstDTO.ProjectSpendName), Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }

            #endregion

            #region take Pull details data from pull guid

            PullDetailsDAL objPullDetails = new PullDetailsDAL(this.enterPriseDBName);
            List<PullDetailsDTO> lstloldPullDetailsDTO = new List<PullDetailsDTO>();

            lstloldPullDetailsDTO = objPullDetails.GetPullDetailsByPullGuid(PullGUID, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();

            #endregion

            if (objoldPullMasterData != null && lstloldPullDetailsDTO != null)
            {
                #region For Pull

                if (!string.IsNullOrWhiteSpace(PullCreditType) && PullCreditType.ToLower().Equals("credit"))
                {
                    try
                    {
                        //need to update other fileds as required                            

                        string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);

                        double ItemCost = lstloldPullDetailsDTO.FirstOrDefault().ItemCost.GetValueOrDefault(0);
                        double ItemPrice = lstloldPullDetailsDTO.FirstOrDefault().ItemPrice.GetValueOrDefault(0);

                        #region Case: New Quantity is grater than old quantity Or New Quantity is less than old quantity

                        if (NewCreditQuantity >= OldCreditQuantity
                            || NewCreditQuantity <= OldCreditQuantity)
                        {
                            //need to update other fileds as required
                            objnewPullMasterData.PoolQuantity = NewCreditQuantity;
                            objnewPullMasterData.TempPullQTY = NewCreditQuantity;

                            PullTransactionDAL objPullMasterDAL = new PullTransactionDAL(SessionHelper.EnterPriseDBName);
                            objnewPullMasterData = objPullMasterDAL.GetPullWithDetailsForEdit(objnewPullMasterData, SessionHelper.RoomID, SessionHelper.CompanyID);
                            return PartialView("PullLotSrSelectionForCreditEdit", objnewPullMasterData);
                        }

                        #endregion                        
                    }
                    catch (Exception ex)
                    {
                        CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                        _returnmessage = string.Format(ResMessage.SaveErrorMsg, ex.Message);
                        _returnstatus = "fail";
                    }
                    finally
                    {

                    }
                }

                #endregion
            }

            return PartialView("PullLotSrSelectionForCreditEdit", new PullMasterViewDTO());
        }

        public ActionResult PullLotSrSelectionForCreditEditPopup(JQueryDataTableParamModel param)
        {
            Guid ItemGUID = Guid.Empty;
            Guid PullGUID = Guid.Empty;
            Int64 BinID = 0;
            double PullQuantity = 0;
            double EnteredPullQuantity = 0;

            Guid.TryParse(Convert.ToString(Request["ItemGUID"]), out ItemGUID);
            Guid.TryParse(Convert.ToString(Request["PullGUID"]), out PullGUID);
            long.TryParse(Convert.ToString(Request["BinID"]), out BinID);
            double.TryParse(Convert.ToString(Request["PullQuantity"]), out PullQuantity);
            string InventoryConsuptionMethod = Convert.ToString(Request["InventoryConsuptionMethod"]);
            string CurrentLoaded = Convert.ToString(Request["CurrentLoaded"]);
            string CurrentDeletedLoaded = Convert.ToString(Request["CurrentDeletedLoaded"]);
            string ViewRight = Convert.ToString(Request["ViewRight"]);
            bool IsDeleteRowMode = Convert.ToBoolean(Request["IsDeleteRowMode"]);
            bool IsIgnoreCreditRule = Convert.ToBoolean(Request["IsIgnoreCreditRule"]);
            bool IsStagginLocation = false;
            EnteredPullQuantity = PullQuantity;

            string[] arrIds = new string[] { };

            if (!string.IsNullOrWhiteSpace(CurrentLoaded))
            {
                arrIds = CurrentLoaded.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }

            int TotalRecordCount = 0;
            PullTransactionDAL objPullDetails = new PullTransactionDAL(this.enterPriseDBName);
            List<ItemLocationLotSerialDTO> lstLotSrs = new List<ItemLocationLotSerialDTO>();
            Dictionary<string, double> dicSerialLots = new Dictionary<string, double>();

            List<ItemLocationLotSerialDTO> lstsetPulls = new List<ItemLocationLotSerialDTO>();

            string[] arrItem;

            List<ItemLocationLotSerialDTO> retlstLotSrs = new List<ItemLocationLotSerialDTO>();

            ItemMasterDTO oItem = null;
            BinMasterDTO objLocDTO = null;
            if (ItemGUID != Guid.Empty)
            {
                oItem = new ItemMasterDAL(this.enterPriseDBName).GetItemWithoutJoins(null, ItemGUID);
                objLocDTO = new BinMasterDAL(this.enterPriseDBName).GetBinByID(BinID, SessionHelper.RoomID, SessionHelper.CompanyID);
                if (objLocDTO != null && objLocDTO.ID > 0 && objLocDTO.IsStagingLocation)
                    IsStagginLocation = true;
            }

            if (oItem != null && oItem.ItemType == 4)
            {
                ItemLocationLotSerialDTO oLotSr = new ItemLocationLotSerialDTO();
                oLotSr.BinID = BinID;
                oLotSr.ID = BinID;
                oLotSr.BinNumber = string.Empty;
                oLotSr.ItemGUID = ItemGUID;
                oLotSr.LotOrSerailNumber = string.Empty;
                oLotSr.Expiration = string.Empty;
                oLotSr.PullQuantity = oItem.DefaultPullQuantity.GetValueOrDefault(0) > PullQuantity ? oItem.DefaultPullQuantity.GetValueOrDefault(0) : PullQuantity;
                oLotSr.LotSerialQuantity = PullQuantity;//oItem.DefaultPullQuantity.GetValueOrDefault(0);

                retlstLotSrs.Add(oLotSr);
            }
            else
            {
                if (arrIds.Count() > 0)
                {
                    string[] arrSerialLots = new string[arrIds.Count()];
                    for (int i = 0; i < arrIds.Count(); i++)
                    {
                        if ((oItem.SerialNumberTracking && !oItem.DateCodeTracking)
                            || (oItem.LotNumberTracking && !oItem.DateCodeTracking)
                            || !oItem.DateCodeTracking)
                        {
                            arrItem = new string[2];
                            arrItem[0] = arrIds[i].Substring(0, arrIds[i].LastIndexOf("_"));
                            arrItem[1] = arrIds[i].Replace(arrItem[0] + "_", "");
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
                            arrItem = arrIds[i].Split('_');
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
                                    objpull.ExpirationDate = Convert.ToDateTime(arrItem[1]);
                                    objpull.Expiration = Convert.ToString(arrItem[1]);
                                }
                                objpull.PullQuantity = Convert.ToDouble(arrItem[2]);

                                lstsetPulls.Add(objpull);
                            }
                        }
                        else if (!oItem.SerialNumberTracking && !oItem.DateCodeTracking && oItem.DateCodeTracking)
                        {
                            arrItem = arrIds[i].Split('_');
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
                                    objpull.ExpirationDate = Convert.ToDateTime(arrItem[0]);
                                    objpull.Expiration = Convert.ToString(arrItem[0]);
                                }
                                objpull.PullQuantity = Convert.ToDouble(arrItem[1]);

                                lstsetPulls.Add(objpull);
                            }
                        }
                        else
                        {
                            arrItem = arrIds[i].Split('_');
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
                                    objpull.ExpirationDate = Convert.ToDateTime(arrItem[0]);
                                    objpull.Expiration = Convert.ToString(arrItem[0]);
                                }
                                objpull.PullQuantity = Convert.ToDouble(arrItem[1]);

                                lstsetPulls.Add(objpull);
                            }
                        }
                    }

                    lstLotSrs = objPullDetails.GetItemLocationsWithLotSerialsForCreditEdit(ItemGUID, PullGUID, SessionHelper.RoomID, SessionHelper.CompanyID, PullQuantity, false, string.Empty, IsStagginLocation);

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
                    else
                        retlstLotSrs = objPullDetails.GetItemLocationsWithLotSerialsForCreditEdit(ItemGUID, PullGUID, SessionHelper.RoomID, SessionHelper.CompanyID, PullQuantity, true, string.Empty, IsStagginLocation);
                }

                foreach (ItemLocationLotSerialDTO item in retlstLotSrs)
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
                }
            }

            if (!(ViewRight == "NoRight" && (oItem.SerialNumberTracking || oItem.LotNumberTracking)))
                retlstLotSrs = retlstLotSrs.Where(x => x.PullQuantity > 0).ToList();

            if (PullQuantity > 0)
            {
                if (lstsetPulls != null && lstsetPulls.Count > 0)
                {
                    if (oItem.SerialNumberTracking && oItem.DateCodeTracking)
                    {
                        lstsetPulls = retlstLotSrs.Where(x => !lstsetPulls.Distinct().Any(e => e.SerialNumber == x.SerialNumber && Convert.ToDateTime(e.ExpirationDate).Date == Convert.ToDateTime(x.ExpirationDate).Date)).ToList();
                        //lstsetPulls = lstsetPulls.Distinct().Where(x => !retlstLotSrs.Any(e => e.SerialNumber == x.SerialNumber && Convert.ToDateTime(e.ExpirationDate).Date == Convert.ToDateTime(x.ExpirationDate).Date)).ToList();
                    }
                    else if (oItem.LotNumberTracking && oItem.DateCodeTracking)
                    {
                        lstsetPulls = retlstLotSrs.Where(x => !lstsetPulls.Distinct().Any(e => e.LotNumber == x.LotNumber && Convert.ToDateTime(e.ExpirationDate).Date == Convert.ToDateTime(x.ExpirationDate).Date)).ToList();
                        //lstsetPulls = lstsetPulls.Distinct().Where(x => !retlstLotSrs.Any(e => e.LotNumber == x.LotNumber && Convert.ToDateTime(e.ExpirationDate).Date == Convert.ToDateTime(x.ExpirationDate).Date)).ToList();
                    }
                    else if (oItem.SerialNumberTracking)
                    {
                        lstsetPulls = retlstLotSrs.Where(x => !lstsetPulls.Distinct().Any(e => e.SerialNumber == x.SerialNumber)).ToList();
                        //lstsetPulls = lstsetPulls.Distinct().Where(x => !retlstLotSrs.Any(e => e.SerialNumber == x.SerialNumber)).ToList();
                    }
                    else if (oItem.LotNumberTracking)
                    {
                        lstsetPulls = retlstLotSrs.Where(x => !lstsetPulls.Distinct().Any(e => e.LotNumber == x.LotNumber)).ToList();
                        //lstsetPulls = lstsetPulls.Distinct().Where(x => !retlstLotSrs.Any(e => e.LotNumber == x.LotNumber)).ToList();
                    }
                    else if (oItem.DateCodeTracking)
                    {
                        lstsetPulls = retlstLotSrs.Where(x => !lstsetPulls.Distinct().Any(e => Convert.ToDateTime(e.ExpirationDate).Date == Convert.ToDateTime(x.ExpirationDate).Date)).ToList();
                        //lstsetPulls = lstsetPulls.Distinct().Where(x => !retlstLotSrs.Any(e => Convert.ToDateTime(e.ExpirationDate).Date == Convert.ToDateTime(x.ExpirationDate).Date)).ToList();
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

            if (CurrentDeletedLoaded != "")
            {
                string[] arrDeletedIds = new string[] { };
                if (!string.IsNullOrWhiteSpace(CurrentDeletedLoaded))
                {
                    arrDeletedIds = CurrentDeletedLoaded.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                    if (arrDeletedIds.Count() > 0)
                    {
                        string[] arrSerialLots = new string[arrDeletedIds.Count()];
                        for (int i = 0; i < arrDeletedIds.Count(); i++)
                        {
                            PullQuantity += 1;
                            if ((oItem.SerialNumberTracking && !oItem.DateCodeTracking)
                                || (oItem.LotNumberTracking && !oItem.DateCodeTracking)
                                || !oItem.DateCodeTracking)
                            {
                                arrItem = new string[2];
                                arrItem[0] = arrDeletedIds[i].Substring(0, arrDeletedIds[i].LastIndexOf("_"));
                                arrItem[1] = arrDeletedIds[i].Replace(arrItem[0] + "_", "");
                                if (arrItem.Length > 1)
                                {
                                    if (oItem.SerialNumberTracking)
                                    {
                                        retlstLotSrs.RemoveAll(x => x.SerialNumber == arrItem[0]);
                                    }
                                    if (oItem.LotNumberTracking)
                                    {
                                        retlstLotSrs.RemoveAll(x => x.LotNumber == arrItem[0]);
                                    }
                                }
                            }
                            else if ((oItem.SerialNumberTracking && oItem.DateCodeTracking)
                                || (oItem.LotNumberTracking && oItem.DateCodeTracking))
                            {
                                arrItem = arrDeletedIds[i].Split('_');
                                if (arrItem.Length > 1)
                                {
                                    if (oItem.SerialNumberTracking && oItem.DateCodeTracking)
                                    {
                                        if (!string.IsNullOrWhiteSpace(arrItem[1]))
                                            retlstLotSrs.RemoveAll(x => x.SerialNumber == arrItem[0] && Convert.ToDateTime(x.ExpirationDate).Date == Convert.ToDateTime(arrItem[1]).Date);
                                        else
                                            retlstLotSrs.RemoveAll(x => x.SerialNumber == arrItem[0]);
                                    }
                                    if (oItem.LotNumberTracking && oItem.DateCodeTracking)
                                    {
                                        if (!string.IsNullOrWhiteSpace(arrItem[1]))
                                            retlstLotSrs.RemoveAll(x => x.LotNumber == arrItem[0] && Convert.ToDateTime(x.ExpirationDate).Date == Convert.ToDateTime(arrItem[1]).Date);
                                        else
                                            retlstLotSrs.RemoveAll(x => x.LotNumber == arrItem[0]);
                                    }
                                }
                            }
                            else if (!oItem.SerialNumberTracking && !oItem.DateCodeTracking && oItem.DateCodeTracking)
                            {
                                arrItem = arrDeletedIds[i].Split('_');
                                if (arrItem.Length > 1)
                                {
                                    if (oItem.DateCodeTracking)
                                    {
                                        if (!string.IsNullOrWhiteSpace(arrItem[0]))
                                            retlstLotSrs.RemoveAll(x => Convert.ToDateTime(x.ExpirationDate).Date == Convert.ToDateTime(arrItem[0]).Date);
                                    }
                                }
                            }
                            else
                            {
                                arrItem = arrDeletedIds[i].Split('_');
                                if (arrItem.Length > 1)
                                {
                                    if (oItem.SerialNumberTracking)
                                    {
                                        retlstLotSrs.RemoveAll(x => x.SerialNumber == arrItem[0]);
                                    }
                                    if (oItem.LotNumberTracking)
                                    {
                                        retlstLotSrs.RemoveAll(x => x.LotNumber == arrItem[0]);
                                    }
                                    if (oItem.DateCodeTracking)
                                    {
                                        if (!string.IsNullOrWhiteSpace(arrItem[0]))
                                            retlstLotSrs.RemoveAll(x => Convert.ToDateTime(x.ExpirationDate).Date == Convert.ToDateTime(arrItem[0]).Date);
                                    }
                                }
                            }
                        }
                    }
                }
            }

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

        #endregion

        public JsonResult GetProjectSpend(string ProjectSpend)
        {
            string ProjectGUID = string.Empty;
            if (!string.IsNullOrEmpty(ProjectSpend))
            {
                ProjectMasterDAL objProjDAL = new ProjectMasterDAL(SessionHelper.EnterPriseDBName);
                ProjectMasterDTO objDTO = objProjDAL.GetProjectspendByName(ProjectSpend, SessionHelper.RoomID, SessionHelper.CompanyID, false, false, null);
                if (objDTO != null)
                    ProjectGUID = Convert.ToString(objDTO.GUID);
            }

            var ObjReturn = new { vProjectID = ProjectGUID };

            return Json(ObjReturn, JsonRequestBehavior.AllowGet);

        }

    }

    public class PullItemsWithQuantity
    {
        public string ID { get; set; }
        public string ItemID { get; set; }
        public string ProjectID { get; set; }
        public string BinID { get; set; }
        public string PullCreditQuantity { get; set; }
        public string PullOrCredit { get; set; }
        public string ItemGUID { get; set; }
        public string TempPullQTY { get; set; }
        public string BinLocation { get; set; }
        public string KitDetailID { get; set; }
    }
}
