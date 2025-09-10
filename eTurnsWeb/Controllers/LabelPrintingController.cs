using Dynamite.Extensions;
using eTurns.DAL;
using eTurns.DAL.LabelPrintingDAL;
using eTurns.DTO;
using eTurns.DTO.LabelPrinting;
using eTurns.DTO.Resources;
using eTurnsWeb.Helper;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using ThoughtWorks.QRCode.Codec;
using System.Configuration;

namespace eTurnsWeb.Controllers
{
    [AuthorizeHelper]
    public class LabelPrintingController : eTurnsControllerBase
    {
        #region Private Property
        //CompanyConfigDTO objCompanyConfigDTO = null;
        //CompanyConfigDAL objCompanyConfigDAL = new CompanyConfigDAL(SessionHelper.EnterPriseDBName);
        RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
        eTurnsRegionInfo objeTurnsRegionInfo = null;

        private string _CostDecimalFormat = "N";
        private string _QtyDecimalFormat = "N";
        private string CostDecimalFormat
        {
            get
            {
                objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                if (objeTurnsRegionInfo != null && objeTurnsRegionInfo.CurrencyDecimalDigits > 0)
                {
                    return "N" + Convert.ToString(objeTurnsRegionInfo.CurrencyDecimalDigits);
                }

                return "N";
            }
        }
        private string QtyDecimalFormat
        {
            get
            {
                objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                if (objeTurnsRegionInfo != null)
                {
                    return "N" + Convert.ToString(objeTurnsRegionInfo.NumberDecimalDigits);
                }

                return "N";

            }
        }
        int CurrentIndex = 1;
        int CurrentRowIndex = 1;
        int CurrentIndexVS = 1;

        private const float TableCellBorderWidth = 0.00f;
        private const float BorderWidth = 0.00f;

        private IEnumerable<LabelFieldModuleTemplateDetailDTO> BaseLabelTemplates
        {
            get
            {
                IEnumerable<LabelFieldModuleTemplateDetailDTO> lstBaseLabelsDTO = null;
                if (Session["BaseLabelTemplates"] != null && ((List<LabelFieldModuleTemplateDetailDTO>)Session["BaseLabelTemplates"]).Count > 0)
                {
                    lstBaseLabelsDTO = (List<LabelFieldModuleTemplateDetailDTO>)Session["BaseLabelTemplates"];
                }
                else
                {
                    lstBaseLabelsDTO = new LabelFieldModuleTemplateDetailDAL(SessionHelper.EnterPriseDBName).GetBaseTemplateList();
                    Session["BaseLabelTemplates"] = lstBaseLabelsDTO;
                }

                return lstBaseLabelsDTO;
            }

        }

        private string DateFormatCSharp
        {
            get
            {
                //if (SessionHelper.CompanyConfig != null)
                //{
                //    if (!string.IsNullOrEmpty(SessionHelper.CompanyConfig.DateFormatCSharp))
                //        return SessionHelper.CompanyConfig.DateFormatCSharp;
                //}
                if (!string.IsNullOrEmpty(SessionHelper.DateTimeFormat))
                {
                    return SessionHelper.DateTimeFormat;
                }
                return "MM/dd/yyyy";
            }
        }

        #endregion

        #region Listing Page Methods
        /// <summary>
        ///GET ALL: /Master/ 
        /// </summary>
        /// <returns></returns>
        public ActionResult TemplateConfigurationList()
        {
            return View();
        }

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult LabelFieldModuleTemplateDetailListAjax(JQueryDataTableParamModel param)
        {
            LabelFieldModuleTemplateDetailDAL controller = null;
            IEnumerable<LabelFieldModuleTemplateDetailDTO> DataFromDB = null;
            try
            {

                //int PageIndex = int.Parse(param.sEcho);
                //int PageSize = param.iDisplayLength;
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

                //if (sortDirection == "asc")
                //    sortColumnName = sortColumnName + " asc";
                //else
                //    sortColumnName = sortColumnName + " desc";


                if (!string.IsNullOrEmpty(sortColumnName))
                {
                    if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined"))
                        sortColumnName = "ID desc";
                }
                else
                    sortColumnName = "ID desc";

                string searchQuery = string.Empty;

                int TotalRecordCount = 0;
                string sSearch = System.Net.WebUtility.HtmlDecode(param.sSearch);
                controller = new LabelFieldModuleTemplateDetailDAL(SessionHelper.EnterPriseDBName);
                TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
                //DataFromDB = controller.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.CompanyID, SessionHelper.RoomID, IsArchived, IsDeleted, SessionHelper.RoomDateFormat, CurrentTimeZone);
                DataFromDB = controller.GetPagedRecordsDB(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, sSearch, sortColumnName, SessionHelper.CompanyID, SessionHelper.RoomID, IsArchived, IsDeleted, SessionHelper.RoomDateFormat, CurrentTimeZone);

                if (DataFromDB != null)
                {
                    DataFromDB.ToList().ForEach(t =>
                    {
                        t.CreatedDate = CommonUtility.ConvertDateByTimeZone(t.CreatedOn, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                        t.UpdatedDate = CommonUtility.ConvertDateByTimeZone(t.UpdatedOn, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                    });
                }
                ViewBag.TotalRecordCount = TotalRecordCount;

                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount,
                    aaData = DataFromDB
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                controller = null;
                DataFromDB = null;
            }
        }

        /// <summary>
        /// GetOrderNarrwSearchHTML
        /// </summary>
        /// <returns></returns>
        public ActionResult GetLPNarrwSearchHTML()
        {
            return PartialView("LabelPrintingNarrowSearch");
        }

        public JsonResult GetNarrowSearchData(bool IsDeleted, bool IsArchived)
        {
            CommonDAL objCommonCtrl = null;
            try
            {
                objCommonCtrl = new CommonDAL(SessionHelper.EnterPriseDBName);
                var tmpsupplierIds = new List<long>();
                NarrowSearchData objNarrowSearchData = objCommonCtrl.GetNarrowSearchDataFromCache("LabelPrinting", SessionHelper.CompanyID, SessionHelper.RoomID, IsArchived, IsDeleted, "", tmpsupplierIds, "");
                return Json(new { Success = true, Message = ResCommon.MsgDataSuccessfullyGet, Data = objNarrowSearchData }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Success = "fail", Message = ex.Message, Data = "" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                objCommonCtrl = null;
            }

        }

        /// <summary>
        /// Copy all base templates.
        /// </summary>
        /// <returns></returns>
        public JsonResult CopyAllBaseTemplate()
        {
            LabelFieldModuleTemplateDetailDAL objFMTDtlDAL = new LabelFieldModuleTemplateDetailDAL(SessionHelper.EnterPriseDBName);
            try
            {
                int RowAffected = objFMTDtlDAL.CopyAllBaseTemplate(SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.RoomID);
                string msg = string.Format(ResLabelPrinting.RowsCopied, RowAffected);

                return Json(new { Message = msg, Status = true, RowsCopied = RowAffected }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message, Status = false }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                objFMTDtlDAL = null;
            }
        }

        #endregion

        #region Add/Edit Methods

        /// <summary>
        /// CreateLabelConfiguration
        /// </summary>
        /// <returns></returns>
        public PartialViewResult CreateLabelConfiguration()
        {
            return PartialView();
        }

        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        public ActionResult LabelConfigurationCreate()
        {

            LabelFieldModuleTemplateDetailDTO objDTO = new LabelFieldModuleTemplateDetailDTO()
            {
                Name = "",
                CreatedOn = DateTimeUtility.DateTimeNow,
                CompanyID = SessionHelper.CompanyID,
                CreatedBy = SessionHelper.UserID,
                CreatedByName = SessionHelper.UserName,
                FeildIDs = "",
                FontSize = 9,
                IncludeBin = false,
                IncludeQuantity = false,
                IsArchived = false,
                IsDeleted = false,
                UpdatedBy = SessionHelper.UserID,
                UpdatedByName = SessionHelper.UserName,
                UpdatedOn = DateTimeUtility.DateTimeNow

            };

            LabelTemplateMasterDAL objTemplateDAL = new LabelTemplateMasterDAL(SessionHelper.EnterPriseDBName);
            LabelModuleMasterDAL objMoudleDAL = new LabelModuleMasterDAL(SessionHelper.EnterPriseDBName);
            LabelModuleFieldMasterDAL objMoudleFieldDAL = new LabelModuleFieldMasterDAL(SessionHelper.EnterPriseDBName);

            //List<LabelTemplateMasterDTO> lstLabelTemplates = objTemplateDAL.GetAllRecords(SessionHelper.CompanyID).ToList();
            List<LabelTemplateMasterDTO> lstLabelTemplates = objTemplateDAL.GetLabelTemplateMasterByCompanyID(SessionHelper.CompanyID).OrderBy(x => x.TemplateNameWithSize).ToList();
            List<LabelModuleMasterDTO> lstLabelModules = objMoudleDAL.GetAllLabelModuleMaster().ToList();

            //LabelTemplateMasterDTO tmpl73909 = lstLabelTemplates.FirstOrDefault(x => x.TemplateID == 73909);

            //lstLabelTemplates.RemoveAll(x => x.TemplateID == 73909);
            //if (tmpl73909 != null)
            //{
            //    lstLabelTemplates.Add(tmpl73909);
            //}

            lstLabelTemplates.Insert(0, new LabelTemplateMasterDTO() { TemplateID = -1, TemplateNameWithSize = ResLabelPrinting.DefaultDropdownText });
            lstLabelModules.Insert(0, new LabelModuleMasterDTO() { ID = -1, ModuleName = ResLabelPrinting.DefaultDropdownText });

            ViewBag.LabelTemplateList = lstLabelTemplates;
            ViewBag.LabelModuleList = lstLabelModules;
            ViewBag.BarcodeFontList = GetBarcodeFontList();
            ViewBag.BarcodePatternList = GetBarcodePatternsListByModuleID(0);
            ViewBag.TextFontList = GetTextFontList();
            objDTO.TextFont = "Verdana";

            return PartialView("CreateLabelConfiguration", objDTO);
        }

        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        public ActionResult TemplateConfigurationEdit(Int64 ID)
        {
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            if (IsDeleted || IsArchived)
            {
                ViewBag.ViewOnly = true;
            }
            //LabelFieldModuleTemplateDetailDTO objDTO = new LabelFieldModuleTemplateDetailDAL(SessionHelper.EnterPriseDBName).GetRecord(ID, SessionHelper.CompanyID, SessionHelper.RoomID);
            LabelFieldModuleTemplateDetailDTO objDTO = new LabelFieldModuleTemplateDetailDAL(SessionHelper.EnterPriseDBName).GetLabelFieldModuleTemplateDetailByID(ID, SessionHelper.CompanyID, SessionHelper.RoomID);

            LabelTemplateMasterDAL objTemplateDAL = new LabelTemplateMasterDAL(SessionHelper.EnterPriseDBName);
            LabelModuleMasterDAL objMoudleDAL = new LabelModuleMasterDAL(SessionHelper.EnterPriseDBName);
            LabelModuleFieldMasterDAL objMoudleFieldDAL = new LabelModuleFieldMasterDAL(SessionHelper.EnterPriseDBName);

            //List<LabelTemplateMasterDTO> lstLabelTemplates = objTemplateDAL.GetAllRecords(SessionHelper.CompanyID).ToList();
            List<LabelTemplateMasterDTO> lstLabelTemplates = objTemplateDAL.GetLabelTemplateMasterByCompanyID(SessionHelper.CompanyID).ToList();
            List<LabelModuleMasterDTO> lstLabelModules = objMoudleDAL.GetAllLabelModuleMaster().ToList();

            lstLabelTemplates.Insert(0, new LabelTemplateMasterDTO() { ID = -1, TemplateNameWithSize = ResLabelPrinting.DefaultDropdownText });
            lstLabelModules.Insert(0, new LabelModuleMasterDTO() { ID = -1, ModuleName = ResLabelPrinting.DefaultDropdownText });

            ViewBag.LabelTemplateList = lstLabelTemplates.OrderBy(T => T.TemplateName);
            ViewBag.LabelModuleList = lstLabelModules;
            ViewBag.BarcodeFontList = GetBarcodeFontList();
            ViewBag.BarcodePatternList = GetBarcodePatternsListByModuleID(objDTO.ModuleID);
            ViewBag.TextFontList = GetTextFontList();

            //LabelTemplateMasterDTO tmpl73909 = lstLabelTemplates.FirstOrDefault(x => x.TemplateID == 73909);
            LabelTemplateMasterDTO tmpl73909 = objTemplateDAL.GetLabelTemplateMasterByCompanyTemplateID(73909, SessionHelper.CompanyID);

            lstLabelTemplates.RemoveAll(x => x.TemplateID == 73909);
            if (tmpl73909 != null)
            {
                lstLabelTemplates.Add(tmpl73909);
            }

            if (BaseLabelTemplates.Select(x => x.Name.ToLower()).Contains(objDTO.Name.ToLower()))
            {
                objDTO.IsBaseLabelEdit = true;
                objDTO.BaseLabelTemplateName = objDTO.Name;
            }

            if (objDTO != null)
            {
                objDTO.CreatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.CreatedOn, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                objDTO.UpdatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.UpdatedOn, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
            }
            return PartialView("CreateLabelConfiguration", objDTO);
        }

        /// <summary>
        /// GetFieldsForModules
        /// </summary>
        /// <param name="DetailDTO"></param>
        /// <returns></returns>
        public PartialViewResult GetFieldsForModules(LabelFieldModuleTemplateDetailDTO DetailDTO)
        {
            LabelModuleMasterDAL objModuleDAL = null;
            LabelModuleMasterDTO labelModule = null;
            LabelModuleFieldMasterDAL objMoudleFieldDAL = null;
            LabelFieldModuleTemplateDetailDAL objDetailDAL = null;
            List<LabelModuleFieldMasterDTO> lstBarcodeKey = null;
            Int64 ModuleID = DetailDTO.ModuleID;
            try
            {

                objModuleDAL = new LabelModuleMasterDAL(SessionHelper.EnterPriseDBName);
                objMoudleFieldDAL = new LabelModuleFieldMasterDAL(SessionHelper.EnterPriseDBName);
                objDetailDAL = new LabelFieldModuleTemplateDetailDAL(SessionHelper.EnterPriseDBName);
                if (DetailDTO != null && DetailDTO.ID > 0)
                {
                    //DetailDTO = objDetailDAL.GetRecord(DetailDTO.ID, SessionHelper.CompanyID, SessionHelper.RoomID);
                    DetailDTO = objDetailDAL.GetLabelFieldModuleTemplateDetailByID(DetailDTO.ID, SessionHelper.CompanyID, SessionHelper.RoomID);
                    DetailDTO.ModuleID = ModuleID;
                }
                List<KeyValDTO> lstQtyField = new List<KeyValDTO>();
                lstQtyField.Insert(0, new KeyValDTO() { key = "", value = ResLabelPrinting.DefaultDropdownText });
                lstQtyField.Insert(1, new KeyValDTO() { key = "1", value = ResLabelPrinting.Hardcode1 });
                lstQtyField.Insert(2, new KeyValDTO() { key = "DefaultPullQuantity", value = ResLabelPrinting.DefaultPullQuantity });
                lstQtyField.Insert(3, new KeyValDTO() { key = "DefaultReorderQuantity", value = ResLabelPrinting.DefaultReorderQuantity });

                lstQtyField.Insert(4, new KeyValDTO() { key = "BinDefaultPullQuantity", value = ResLabelPrinting.BinDefaultPullQuantity });
                lstQtyField.Insert(5, new KeyValDTO() { key = "BinDefaultReorderQuantity", value = ResLabelPrinting.BinDefaultReorderQuantity });

                DetailDTO.lstQuantityFields = lstQtyField;

                if (ModuleID > 0)
                {
                    //labelModule = objModuleDAL.GetRecord(ModuleID);
                    labelModule = objModuleDAL.GetLabelModuleMasterByID(ModuleID);
                    DetailDTO.lstModuleFields = objMoudleFieldDAL.GetRecordsModueWise(ModuleID, SessionHelper.CompanyID, true).ToList();
                    if (DetailDTO.FeildIDs != null && DetailDTO.FeildIDs.Length > 0)
                    {
                        Int64[] ints = DetailDTO.FeildIDs.Split(',').Select(Int64.Parse).ToArray();
                        DetailDTO.lstSelectedModuleFields = DetailDTO.lstModuleFields.Where(x => ints.Contains(x.ID)).ToList();
                    }
                    lstBarcodeKey = DetailDTO.lstModuleFields.Where(x => x.IncludeInBarcode > 0).ToList();
                }
                else
                {
                    lstBarcodeKey = new List<LabelModuleFieldMasterDTO>();
                }

                lstBarcodeKey.Insert(0, new LabelModuleFieldMasterDTO() { ID = -1, FieldName = "", FieldDisplayName = ResLabelPrinting.DefaultDropdownText });
                DetailDTO.lstBarcodeKey = lstBarcodeKey;

                return PartialView("_FieldList", DetailDTO);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objModuleDAL = null;
                labelModule = null;
                objMoudleFieldDAL = null;
                objDetailDAL = null;
            }

        }

        public JsonResult LabelCopyTemplateConfiguration(string NewTemplateName, string OldTemplateName)
        {
            LabelFieldModuleTemplateDetailDAL objDetailDAL = new LabelFieldModuleTemplateDetailDAL(SessionHelper.EnterPriseDBName);
            LabelFieldModuleTemplateDetailDTO objTemplateDto = new LabelFieldModuleTemplateDetailDTO();
            //objTemplateDto = objDetailDAL.GetRecord(OldTemplateName.Trim(), SessionHelper.CompanyID, SessionHelper.RoomID);
            objTemplateDto = objDetailDAL.GetLabelFieldModuleTemplateDetailByName(OldTemplateName.Trim(), SessionHelper.CompanyID, SessionHelper.RoomID);
            try
            {
                if (objTemplateDto != null)
                {
                    objTemplateDto.ID = 0;
                    objTemplateDto.Name = NewTemplateName.Trim();
                    objTemplateDto.CompanyID = SessionHelper.CompanyID;
                    if (objTemplateDto.IncludeQuantity && string.IsNullOrWhiteSpace(objTemplateDto.QuantityField))
                    {
                        return Json(new { Message = ResLabelPrinting.ErrorMassageForQuantityField, Status = "fail" }, JsonRequestBehavior.AllowGet);
                    }
                    string strOK = objDetailDAL.DuplicateCheck(objTemplateDto.Name, objTemplateDto.ID, objTemplateDto.CompanyID, SessionHelper.RoomID);
                    if (strOK == "duplicate")
                    {
                        string message = string.Format(ResMessage.DuplicateMessage, ResLabelPrinting.UserDefineTemplateName, objTemplateDto.Name);
                        string status = "duplicate";
                        return Json(new { Message = message, Status = status, DTO = objTemplateDto }, JsonRequestBehavior.AllowGet);

                    }
                    if (objTemplateDto.arrFieldIds != null)
                        objTemplateDto.FeildIDs = string.Join(",", objTemplateDto.arrFieldIds);

                    if (objTemplateDto.ID <= 0)
                    {
                        objTemplateDto.UpdatedBy = SessionHelper.UserID;
                        objTemplateDto.CreatedBy = SessionHelper.UserID;
                        objTemplateDto.ID = objDetailDAL.Insert(objTemplateDto);
                        if (objTemplateDto.IsSelectedInModuleConfig)
                        {
                            objDetailDAL.SetAsDefaultTemplateForModule(objTemplateDto.ID, SessionHelper.CompanyID, SessionHelper.UserID, objTemplateDto.ModuleID, SessionHelper.RoomID);
                        }
                        eTurns.DAL.CacheHelper<IEnumerable<LabelFieldModuleTemplateDetailDTO>>.InvalidateCache();
                        return Json(new { Message = ResMessage.SaveMessage, Status = "ok", DTO = objTemplateDto }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Message = "fail", Status = "fail" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { Message = "fail", Status = "fail" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message, Status = "fail" }, JsonRequestBehavior.AllowGet);
            }


        }

        /// <summary>
        /// LabelConfigurationSave
        /// </summary>
        /// <param name="frmColl"></param>
        /// <returns></returns>
        public JsonResult LabelConfigurationSave(LabelFieldModuleTemplateDetailDTO frmColl)
        {
            LabelFieldModuleTemplateDetailDAL objDetailDAL = new LabelFieldModuleTemplateDetailDAL(SessionHelper.EnterPriseDBName);
            try
            {
                if (frmColl.IncludeQuantity && string.IsNullOrWhiteSpace(frmColl.QuantityField))
                {
                    return Json(new { Message = ResLabelPrinting.ErrorMassageForQuantityField, Status = "fail" }, JsonRequestBehavior.AllowGet);
                }
                string strOK = objDetailDAL.DuplicateCheck(frmColl.Name, frmColl.ID, frmColl.CompanyID, SessionHelper.RoomID);
                if (strOK == "duplicate")
                {
                    string message = string.Format(ResMessage.DuplicateMessage, ResLabelPrinting.UserDefineTemplateName, frmColl.Name);
                    string status = "duplicate";
                    return Json(new { Message = message, Status = status, DTO = frmColl }, JsonRequestBehavior.AllowGet);

                }

                //frmColl.CompanyID = SessionHelper.CompanyID;

                if (frmColl.IsSaveForEnterprise)
                {
                    frmColl.CompanyID = -1;
                }

                if (frmColl.arrFieldIds != null && frmColl.arrFieldIds.Length > 0)
                    frmColl.FeildIDs = string.Join(",", frmColl.arrFieldIds);

                if (frmColl.ID <= 0)
                {
                    frmColl.UpdatedBy = SessionHelper.UserID;
                    frmColl.CreatedBy = SessionHelper.UserID;
                    frmColl.ID = objDetailDAL.Insert(frmColl);
                }
                else
                {
                    frmColl.UpdatedBy = SessionHelper.UserID;
                    objDetailDAL.Edit(frmColl);
                }

                if (frmColl.IsSelectedInModuleConfig)
                {
                    objDetailDAL.SetAsDefaultTemplateForModule(frmColl.ID, SessionHelper.CompanyID, SessionHelper.UserID, frmColl.ModuleID, SessionHelper.RoomID);
                }
                else
                {
                    LabelModuleTemplateDetailDAL objMTDetailDAL = new LabelModuleTemplateDetailDAL(SessionHelper.EnterPriseDBName);
                    //IEnumerable<LabelModuleTemplateDetailDTO> lstDetailDTO = objMTDetailDAL.GetAllRecords(SessionHelper.CompanyID, SessionHelper.RoomID).Where(x => x.ModuleID == frmColl.ModuleID);
                    IEnumerable<LabelModuleTemplateDetailDTO> lstDetailDTO = objMTDetailDAL.GetLabelModuleTemplateDetailByRoomCompanyModuleID(SessionHelper.CompanyID, SessionHelper.RoomID, frmColl.ModuleID);
                    if (!(lstDetailDTO != null && lstDetailDTO.Count() > 0 && lstDetailDTO.FirstOrDefault().TemplateDetailID != frmColl.ID))
                    {
                        LabelFieldModuleTemplateDetailDAL objDAL = new LabelFieldModuleTemplateDetailDAL(SessionHelper.EnterPriseDBName);
                        //List<LabelFieldModuleTemplateDetailDTO> lstFMTDetails = objDAL.GetAllRecords(SessionHelper.CompanyID, SessionHelper.RoomID).Where(x => x.ModuleID == frmColl.ModuleID && x.IsDeleted == false && x.IsArchived == false).OrderBy(x => x.Name).ToList();
                        List<LabelFieldModuleTemplateDetailDTO> lstFMTDetails = objDAL.GetAllLabelFieldModuleTemplateDetail(SessionHelper.CompanyID, SessionHelper.RoomID, frmColl.ModuleID, false, false, string.Empty).OrderBy(x => x.Name).ToList();
                        Int64 templateID = objDAL.GetDefaultTemplateIDByModule(frmColl.ModuleID, SessionHelper.CompanyID, SessionHelper.RoomID);
                        if (templateID > 0)
                            objDetailDAL.SetAsDefaultTemplateForModule(templateID, SessionHelper.CompanyID, SessionHelper.UserID, frmColl.ModuleID, SessionHelper.RoomID);
                    }

                }


                return Json(new { Message = ResMessage.SaveMessage, Status = "ok", DTO = frmColl }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message, Status = "fail" }, JsonRequestBehavior.AllowGet);


            }
        }

        /// <summary>
        /// GetTemplateDetailByID
        /// </summary>
        /// <param name="TemplateID"></param>
        /// <returns></returns>
        public JsonResult GetTemplateDetailByID(Int64 TemplateID)
        {
            LabelTemplateMasterDAL objDAL = null;
            LabelTemplateMasterDTO objDTO = null;
            try
            {
                objDAL = new LabelTemplateMasterDAL(SessionHelper.EnterPriseDBName);
                //objDTO = objDAL.GetRecord(TemplateID, SessionHelper.CompanyID);
                objDTO = objDAL.GetLabelTemplateMasterByCompanyTemplateID(TemplateID, SessionHelper.CompanyID);
                return Json(new { Message = "ok", Success = "ok", TemplateDetail = objDTO }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message, Success = "fail", TemplateDetail = string.Empty }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                objDAL = null;
                objDTO = null;
            }

        }

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public JsonResult DeleteLabelFieldModuleTemplateDetailRecords(string ids)
        {
            try
            {
                LabelFieldModuleTemplateDetailDAL objDAL = new LabelFieldModuleTemplateDetailDAL(SessionHelper.EnterPriseDBName);
                var noRecordsSelected = ResLabelPrinting.NoRecordSelected;
                var ok = ResCommon.Ok;
                var defaultTemplateCantBeDelete = ResLabelPrinting.DefaultTemplateCantBeDelete;
                string strmsg = objDAL.DeleteSelectedRecords(ids, SessionHelper.UserID, SessionHelper.CompanyID, SessionHelper.RoomID, noRecordsSelected, ok, defaultTemplateCantBeDelete); 
                
                if (strmsg == noRecordsSelected)
                {
                    return Json(new { Message = strmsg, Status = "fail" }, JsonRequestBehavior.AllowGet);
                }
                else if (strmsg != ok)
                {
                    Int64[] intIDs = ids.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(Int64.Parse).ToArray();
                    if (intIDs.Length > 1)
                    {                        
                        return Json(new { Message = ResLabelPrinting.RowsNotDeleted, Status = "ok" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { Message = strmsg, Status = "fail" }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { Message = ResMessage.DeletedSuccessfully, Status = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message, Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// GetBarcodeFontList
        /// </summary>
        /// <returns></returns>
        public List<KeyValDTO> GetBarcodeFontList()
        {
            List<KeyValDTO> lstDTO = new List<KeyValDTO>();

            KeyValDTO objDTO = new KeyValDTO() { key = "128", value = ResLabelPrinting.Code128 }; 
            lstDTO.Add(objDTO);
            objDTO = new KeyValDTO() { key = "39", value = ResLabelPrinting.Code3of9 };
            lstDTO.Add(objDTO);
            objDTO = new KeyValDTO() { key = "93", value = ResLabelPrinting.Code93 };
            lstDTO.Add(objDTO);
            //lstDTO.Insert(0, new KeyValDTO() { key = "93", value = ResLabelPrinting.DefaultDropdownText });
            return lstDTO;
        }

        private List<KeyValDTO> GetBarcodePatternsListByModuleID(Int64 ModuleID)
        {
            List<KeyValDTO> lstDTO = new List<KeyValDTO>();
            if (ModuleID == 10)
            {
                KeyValDTO objDTO = new KeyValDTO() { key = "US", value = "#EItem#" };
                lstDTO.Add(objDTO);
                objDTO = new KeyValDTO() { key = "UK", value = "Item" };
                lstDTO.Add(objDTO);
                objDTO = new KeyValDTO() { key = "US01", value = "#Item" };
                lstDTO.Add(objDTO);
            }
            else
            {
                KeyValDTO objDTO = new KeyValDTO() { key = "US", value = "#IItem#BBin#QQty" };
                lstDTO.Add(objDTO);
                objDTO = new KeyValDTO() { key = "UK", value = "*Item+Bin+Qty*" };
                lstDTO.Add(objDTO);
                objDTO = new KeyValDTO() { key = "US01", value = "#Item@Bin<Qty" };
                lstDTO.Add(objDTO);
                objDTO = new KeyValDTO() { key = "UK01", value = "Item#" };
                lstDTO.Add(objDTO);
            }
            //lstDTO.Insert(0, new KeyValDTO() { key = "93", value = ResLabelPrinting.DefaultDropdownText });
            return lstDTO;

        }

        /// <summary>
        /// GetBarcodeFontList
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetBarcodePatternsByModuleID(Int64 ModuleID)
        {
            List<KeyValDTO> lstDTO = GetBarcodePatternsListByModuleID(ModuleID);
            return Json(new { BarcodePatterns = lstDTO }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// GetBarcodeFontList
        /// </summary>
        /// <returns></returns>
        public List<KeyValDTO> GetTextFontList()
        {
            List<KeyValDTO> lstDTO = new List<KeyValDTO>();
            KeyValDTO objDTO = new KeyValDTO() { key = "Verdana", value = "Verdana" };
            lstDTO.Add(objDTO);
            objDTO = new KeyValDTO() { key = "Calibri", value = "Calibri" };
            lstDTO.Add(objDTO);
            return lstDTO;
        }

        #endregion

        #region Module Wise Template View

        /// <summary>
        /// Module wise configuration
        /// </summary>
        /// <returns></returns>
        public PartialViewResult ModuleWiseConfiguration()
        {
            Dictionary<LabelModuleMasterDTO, List<LabelFieldModuleTemplateDetailDTO>> dict = null;
            LabelModuleMasterDAL objModuleDAL = null;
            LabelFieldModuleTemplateDetailDAL objDAL = null;
            IEnumerable<LabelModuleMasterDTO> lstModules = null;
            List<LabelFieldModuleTemplateDetailDTO> lstFMTDetails = null;

            try
            {
                dict = new Dictionary<LabelModuleMasterDTO, List<LabelFieldModuleTemplateDetailDTO>>();
                objModuleDAL = new LabelModuleMasterDAL(SessionHelper.EnterPriseDBName);
                objDAL = new LabelFieldModuleTemplateDetailDAL(SessionHelper.EnterPriseDBName);
                lstModules = objModuleDAL.GetAllLabelModuleMaster();

                foreach (var item in lstModules)
                {
                    //lstFMTDetails = objDAL.GetAllRecords(SessionHelper.CompanyID, SessionHelper.RoomID).Where(x => x.ModuleID == item.ID && x.IsDeleted == false && x.IsArchived == false).OrderBy(x => x.Name).ToList();
                    lstFMTDetails = objDAL.GetAllLabelFieldModuleTemplateDetail(SessionHelper.CompanyID, SessionHelper.RoomID, item.ID, false, false, string.Empty).OrderBy(x => x.Name).ToList();
                    lstFMTDetails.Insert(0, new LabelFieldModuleTemplateDetailDTO() { ID = -1, Name = ResLabelPrinting.DefaultDropdownText });
                    dict.Add(item, lstFMTDetails);
                }

                return PartialView(dict);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                dict = null;
                objModuleDAL = null;
                objDAL = null;
                lstModules = null;
                lstFMTDetails = null;
            }
        }

        public JsonResult GetModuleWiseTemplatedID()
        {
            LabelModuleTemplateDetailDAL objMTDetailDAL = null;
            List<LabelModuleTemplateDetailDTO> lstDetailDTO = null;
            try
            {
                objMTDetailDAL = new LabelModuleTemplateDetailDAL(SessionHelper.EnterPriseDBName);
                //lstDetailDTO = objMTDetailDAL.GetAllRecords(SessionHelper.CompanyID, SessionHelper.RoomID).ToList();
                lstDetailDTO = objMTDetailDAL.GetLabelModuleTemplateDetailByRoomIDCompanyID(SessionHelper.CompanyID, SessionHelper.RoomID).ToList();
                return Json(new { Message = "ok", Status = "ok", TemplateDetailList = lstDetailDTO }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message, Status = "fail", TemplateDetailList = "" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                objMTDetailDAL = null;
                lstDetailDTO = null;
            }

        }

        /// <summary>
        /// Save Module wise configuration
        /// </summary>
        /// <param name="frm"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LabelModuleTemplateSave(FormCollection frm)
        {
            LabelModuleMasterDAL objModuleDAL = null;
            LabelModuleTemplateDetailDAL objMTDetailDAL = null;
            LabelModuleTemplateDetailDTO objDetailDTO = null;
            LabelFieldModuleTemplateDetailDAL objLabelFieldModuleTemplateDetailDAL = null;
            IEnumerable<LabelModuleMasterDTO> lstModules = null;
            List<LabelModuleTemplateDetailDTO> lstDetailDTO = null;
            try
            {
                objModuleDAL = new LabelModuleMasterDAL(SessionHelper.EnterPriseDBName);
                objMTDetailDAL = new LabelModuleTemplateDetailDAL(SessionHelper.EnterPriseDBName);
                lstModules = objModuleDAL.GetAllLabelModuleMaster();
                //lstDetailDTO = objMTDetailDAL.GetAllRecords(SessionHelper.CompanyID, SessionHelper.RoomID).ToList();
                lstDetailDTO = objMTDetailDAL.GetLabelModuleTemplateDetailByRoomIDCompanyID(SessionHelper.CompanyID, SessionHelper.RoomID).ToList();
                objLabelFieldModuleTemplateDetailDAL = new LabelFieldModuleTemplateDetailDAL(SessionHelper.EnterPriseDBName);
                foreach (var item in lstModules)
                {
                    Int64 FMTDetailID = -1;
                    Int64.TryParse(frm["ddlModelTemplateDetail_" + item.ID], out FMTDetailID);

                    if (FMTDetailID > 0)
                    {
                        objLabelFieldModuleTemplateDetailDAL.SetAsDefaultTemplateForModule(FMTDetailID, SessionHelper.CompanyID, SessionHelper.UserID, item.ID, SessionHelper.RoomID);
                    }
                    else
                    {
                        if (lstDetailDTO != null && lstDetailDTO.Count > 0)
                        {
                            //objDetailDTO = lstDetailDTO.FirstOrDefault(x => x.ModuleID == item.ID);  
                            objDetailDTO = objMTDetailDAL.GetLabelModuleTemplateDetailByRoomCompanyModuleID(SessionHelper.CompanyID, SessionHelper.RoomID, item.ID).FirstOrDefault();
                            if (objDetailDTO != null && objDetailDTO.ID > 0)
                            {
                                objMTDetailDAL.Delete(item.ID, SessionHelper.UserID, SessionHelper.CompanyID, SessionHelper.RoomID);
                            }
                        }
                    }

                    /*objDetailDTO = null;
                    
                    if (lstDetailDTO != null && lstDetailDTO.Count > 0)
                    {
                        objDetailDTO = lstDetailDTO.FirstOrDefault(x => x.ModuleID == item.ID);
                    }

                    if (FMTDetailID > 0)
                    {
                        if (objDetailDTO != null && objDetailDTO.ID > 0)
                        {
                            objDetailDTO.TemplateDetailID = FMTDetailID;
                            objDetailDTO.UpdatedBy = SessionHelper.UserID;
                            objDetailDTO.ModuleID = item.ID;
                            objMTDetailDAL.Edit(objDetailDTO);
                        }
                        else
                        {
                            objDetailDTO = new LabelModuleTemplateDetailDTO()
                            {
                                CreatedBy = SessionHelper.UserID,
                                CompanyID = SessionHelper.CompanyID,
                                ModuleID = item.ID,
                                TemplateDetailID = FMTDetailID,
                                ID = 0,
                            };
                            objMTDetailDAL.Insert(objDetailDTO);
                        }

                    }
                    else
                    {
                        if (objDetailDTO != null && objDetailDTO.ID > 0)
                        {
                            objMTDetailDAL.Delete(item.ID, SessionHelper.UserID, SessionHelper.CompanyID);
                        }
                    }*/

                }
                return Json(new { Message = ResMessage.SaveMessage, Success = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message, Success = "fail" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                objModuleDAL = null;
                objMTDetailDAL = null;
                objDetailDTO = null;
                lstModules = null;
                lstDetailDTO = null;
            }
        }

        public JsonResult GetLabelHTMLByDetailID(Int64 ID)
        {
            LabelFieldModuleTemplateDetailDAL objMTDetailDAL = null;
            LabelFieldModuleTemplateDetailDTO detailDTO = null;

            try
            {
                objMTDetailDAL = new LabelFieldModuleTemplateDetailDAL(SessionHelper.EnterPriseDBName);
                //detailDTO = objMTDetailDAL.GetRecord(ID, SessionHelper.CompanyID, SessionHelper.RoomID);
                detailDTO = objMTDetailDAL.GetLabelFieldModuleTemplateDetailByID(ID, SessionHelper.CompanyID, SessionHelper.RoomID);
                return Json(new { Message = "ok", Status = "ok", HTMLString = detailDTO.LabelHTML }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message, Status = "fail", HTMLString = "" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                objMTDetailDAL = null;
                detailDTO = null;
            }
        }


        #endregion

        #region Generate Labels In PDF
        /// <summary>
        /// LoadLabelsInPDF
        /// </summary>
        /// <param name="ModuleID"></param>
        /// <param name="IDs"></param>
        /// <returns></returns>
        [HttpPost]
        public FileStreamResult GenerateLabelsPDF_LastOld(FormCollection frm)
        {
            Int64 ModuleID = int.Parse(frm["hdnModuleID"]);
            string IDs = frm["hdnIDs"];
            string ROTDIDs = frm["hdnROTDIds"];

            int NoOfCopy = 0;
            int.TryParse(frm["txtNoOfLableCopy"], out NoOfCopy);
            if (NoOfCopy <= 0)
            {
                NoOfCopy = 1;
            }
            int NoOfUsedLabel = 0;
            int.TryParse(frm["txtNoUsedLable"], out NoOfUsedLabel);

            string SortFields = frm["hdnSortField"];
            if (!string.IsNullOrEmpty(SortFields) && SortFields.ToLower().Contains("null"))
            {
                SortFields = string.Empty;
            }


            _CostDecimalFormat = CostDecimalFormat;
            _QtyDecimalFormat = QtyDecimalFormat;

            CurrentIndex = 1;
            LabelModuleTemplateDetailDAL objMTDetailDAL = null;
            LabelFieldModuleTemplateDetailDAL objFMTDetailDAL = null;
            LabelTemplateMasterDAL objTMDAL = null;
            LabelModuleTemplateDetailDTO objMTDetailDTO = null;
            LabelFieldModuleTemplateDetailDTO objFMTDetailDTO = null;
            LabelTemplateMasterDTO objTemplateDTO = null;
            LabelModuleMasterDAL objMMDAL = null;
            LabelModuleFieldMasterDAL objMFDAL = null;
            LabelModuleMasterDTO objMMDTO = null;

            MemoryStream fs = null;
            Document doc = null;
            PdfWriter pdfWriter = null;

            List<float> lstFL = null;
            PdfPTable table = null;
            Int64[] arrIDs = null;
            Dictionary<string, IElement>  supplierLogoDictionary = null;

            // Int64[] arrROTDIDs = null;
            try
            {
                fs = new MemoryStream();
                objMTDetailDAL = new LabelModuleTemplateDetailDAL(SessionHelper.EnterPriseDBName);
                objFMTDetailDAL = new LabelFieldModuleTemplateDetailDAL(SessionHelper.EnterPriseDBName);
                objTMDAL = new LabelTemplateMasterDAL(SessionHelper.EnterPriseDBName);
                objMMDAL = new LabelModuleMasterDAL(SessionHelper.EnterPriseDBName);
                objMFDAL = new LabelModuleFieldMasterDAL(SessionHelper.EnterPriseDBName);



                //objMTDetailDTO = objMTDetailDAL.GetAllRecords(SessionHelper.CompanyID, SessionHelper.RoomID).FirstOrDefault(x => x.ModuleID == ModuleID && x.CompanyID == SessionHelper.CompanyID && x.RoomID == SessionHelper.RoomID);
                objMTDetailDTO = objMTDetailDAL.GetLabelModuleTemplateDetailByRoomCompanyModuleID(SessionHelper.CompanyID, SessionHelper.RoomID, ModuleID).FirstOrDefault();

                //objFMTDetailDTO = objFMTDetailDAL.GetRecord(objMTDetailDTO.TemplateDetailID, SessionHelper.CompanyID, SessionHelper.RoomID);
                objFMTDetailDTO = objFMTDetailDAL.GetLabelFieldModuleTemplateDetailByID(objMTDetailDTO.TemplateDetailID, SessionHelper.CompanyID, SessionHelper.RoomID);
                objFMTDetailDTO.lstModuleFields = objMFDAL.GetRecordsModueWise(ModuleID, SessionHelper.CompanyID, true).ToList();
                //objTemplateDTO = objTMDAL.GetRecord(objFMTDetailDTO.TemplateID, SessionHelper.CompanyID);
                objTemplateDTO = objTMDAL.GetLabelTemplateMasterByCompanyTemplateID(objFMTDetailDTO.TemplateID, SessionHelper.CompanyID);

                //objMMDTO = objMMDAL.GetRecord(ModuleID);
                objMMDTO = objMMDAL.GetLabelModuleMasterByID(ModuleID);

                Int64[] ints = objFMTDetailDTO.FeildIDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(Int64.Parse).ToArray();


                objFMTDetailDTO.lstSelectedModuleFields = objFMTDetailDTO.lstModuleFields.Where(x => ints.Contains(x.ID)).ToList();

                float fltPageWidth = ConvertInchToPoint(objTemplateDTO.PageWidth);
                float fltPageHeight = ConvertInchToPoint(objTemplateDTO.PageHeight);
                doc = new Document(new Rectangle(fltPageWidth, fltPageHeight));
                float fltPageLeftMarging = ConvertInchToPoint(objTemplateDTO.PageMarginLeft);
                float fltPageRightMarging = ConvertInchToPoint(objTemplateDTO.PageMarginRight);
                float fltPageTopMarging = ConvertInchToPoint(objTemplateDTO.PageMarginTop);
                float fltPageBottomMarging = ConvertInchToPoint(objTemplateDTO.PageMarginBottom);
                doc.SetMargins(fltPageLeftMarging, fltPageRightMarging, fltPageTopMarging, fltPageBottomMarging);

                pdfWriter = PdfWriter.GetInstance(doc, fs);
                doc.Open();
                lstFL = new List<float>();
                float fltLabelWidth = 0;
                for (int i = 0; i < objTemplateDTO.NoOfColumns; i++)
                {
                    fltLabelWidth = ConvertInchToPoint(objTemplateDTO.LabelWidth);
                    lstFL.Add(fltLabelWidth);
                    if (objTemplateDTO.LabelSpacingHorizontal > 0 && i < objTemplateDTO.NoOfColumns - 1)
                    {
                        fltLabelWidth = ConvertInchToPoint(objTemplateDTO.LabelSpacingHorizontal);
                        lstFL.Add(fltLabelWidth);
                    }
                }

                table = new PdfPTable(lstFL.Count());
                table.WidthPercentage = 100f;
                table.HorizontalAlignment = 0;

                table.SetWidths(lstFL.ToArray<float>());
                table.DefaultCell.Border = 0;

                table.SkipFirstHeader = true;
                table.SkipLastFooter = true;

                arrIDs = IDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(Int64.Parse).ToArray();

                List<object> lstObjs = GetModuleWiseObjecList(objMMDTO, arrIDs, SortFields, objFMTDetailDTO, ROTDIDs, "");
                supplierLogoDictionary =  new Dictionary<string, IElement>();

                if (lstObjs != null & lstObjs.Count > 0)
                {
                    CurrentRowIndex = 1;
                    for (int i = 0; i < NoOfUsedLabel; i++)
                    {
                        if (objTemplateDTO.NoOfColumns > 1 && CurrentIndex % 2 == 0)
                        {
                            table.AddCell(GetTabelCell(objTemplateDTO));
                        }

                        PdfPCell cell = GetTabelCell(objTemplateDTO);

                        cell.AddElement(new Phrase(""));

                        table.AddCell(cell);

                        if (lstFL.Count < CurrentIndex)
                        {
                            if (objTemplateDTO.LabelSpacingVerticle > 0)
                            {
                                for (int k = 0; k < lstFL.Count; k++)
                                {
                                    table.AddCell(GetTabelCellForVerticleSpace(objTemplateDTO));
                                    if (lstFL.Count < CurrentIndexVS)
                                        CurrentIndexVS = 1;
                                }
                                CurrentIndexVS = 1;
                            }
                            CurrentIndex = 1;
                        }
                    }
                    CurrentRowIndex = 1;
                    foreach (object item in lstObjs)
                    {

                        for (int i = 0; i < NoOfCopy; i++)
                        {
                            if (objTemplateDTO.NoOfColumns > 1 && CurrentIndex % 2 == 0)
                            {
                                table.AddCell(GetTabelCell(objTemplateDTO));
                            }

                            PdfPCell cell = GetTabelCell(objTemplateDTO);
                            if (!string.IsNullOrEmpty(objFMTDetailDTO.LabelXML) && objFMTDetailDTO.LabelXML.Trim().Length > 0)
                            {
                                cell.AddElement(GetCellContentFromXMLNew(objTemplateDTO, item, objMMDTO, objFMTDetailDTO, supplierLogoDictionary: ref supplierLogoDictionary));
                            }
                            else
                            {
                                cell.AddElement(GetCellContent(objTemplateDTO, item, objMMDTO, objFMTDetailDTO));
                            }

                            table.AddCell(cell);

                            if (lstFL.Count < CurrentIndex)
                            {
                                if (CurrentRowIndex * objTemplateDTO.NoOfColumns != objTemplateDTO.NoOfLabelPerSheet)
                                {
                                    if (objTemplateDTO.LabelSpacingVerticle > 0)
                                    {
                                        for (int k = 0; k < lstFL.Count; k++)
                                        {
                                            table.AddCell(GetTabelCellForVerticleSpace(objTemplateDTO));
                                            if (lstFL.Count < CurrentIndexVS)
                                                CurrentIndexVS = 1;
                                        }
                                    }
                                }
                                else
                                    CurrentRowIndex = 1;

                                CurrentIndex = 1;
                                CurrentIndexVS = 1;
                                CurrentRowIndex += 1;
                            }
                        }
                    }
                }
                else
                {
                    table.AddCell(new PdfPCell(new Phrase(ResLabelPrinting.MsgNotDataFound)));
                }

                table.CompleteRow();
                doc.Add(table);

                pdfWriter.CloseStream = false;
                doc.Close();
                pdfWriter.Dispose();
                fs.Position = 0;
                return File(fs, "application/pdf");

            }
            //catch (Exception ex1)
            //{
            //    throw ex1;
            //}
            finally
            {
                //fs.Flush();

                //if (doc != null && doc.PageNumber != 0)
                //    doc.Dispose();

                //if (pdfWriter != null)
                //    pdfWriter.Dispose();

                //if (fs != null)
                //    fs.Dispose();

                fs = null;
                doc = null;
                pdfWriter = null;
                objMTDetailDAL = null;
                objFMTDetailDAL = null;
                objTMDAL = null;
                objMTDetailDTO = null;
                objFMTDetailDTO = null;
                objTemplateDTO = null;
                objMMDAL = null;
                objMFDAL = null;
                objMMDTO = null;

                lstFL = null;
                table = null;
                arrIDs = null;
            }




        }


        public JsonResult CheckLabelsBeforeGenerate(Int64 ModuleId)
        {
            LabelFieldModuleTemplateDetailDAL objLabelDetailDAL = null;
            LabelTemplateMasterDAL objTemplateMasterDAL = null;
            LabelModuleTemplateDetailDAL objModuleTemplateDAL = null;

            LabelModuleTemplateDetailDTO objModuleTemplateDTO = null;
            LabelFieldModuleTemplateDetailDTO objLabelDetailDTO = null;
            LabelTemplateMasterDTO objTemplateDTO = null;
            string message = string.Empty;

            try
            {
                objLabelDetailDAL = new LabelFieldModuleTemplateDetailDAL(SessionHelper.EnterPriseDBName);
                objTemplateMasterDAL = new LabelTemplateMasterDAL(SessionHelper.EnterPriseDBName);
                objModuleTemplateDAL = new LabelModuleTemplateDetailDAL(SessionHelper.EnterPriseDBName);

                //objModuleTemplateDTO = objModuleTemplateDAL.GetAllRecords(SessionHelper.CompanyID, SessionHelper.RoomID).FirstOrDefault(x => x.ModuleID == ModuleId && x.CompanyID == SessionHelper.CompanyID && x.RoomID == SessionHelper.RoomID);
                objModuleTemplateDTO = objModuleTemplateDAL.GetLabelModuleTemplateDetailByRoomCompanyModuleID(SessionHelper.CompanyID, SessionHelper.RoomID, ModuleId).FirstOrDefault();
                if (objModuleTemplateDTO != null)
                {
                    //objLabelDetailDTO = objLabelDetailDAL.GetRecord(objModuleTemplateDTO.TemplateDetailID, SessionHelper.CompanyID, SessionHelper.RoomID);
                    objLabelDetailDTO = objLabelDetailDAL.GetLabelFieldModuleTemplateDetailByID(objModuleTemplateDTO.TemplateDetailID, SessionHelper.CompanyID, SessionHelper.RoomID);
                    if (objLabelDetailDTO != null)
                    {
                        //objTemplateDTO = objTemplateMasterDAL.GetRecord(objLabelDetailDTO.TemplateID, SessionHelper.CompanyID);
                        objTemplateDTO = objTemplateMasterDAL.GetLabelTemplateMasterByCompanyTemplateID(objLabelDetailDTO.TemplateID, SessionHelper.CompanyID);
                        if (objTemplateDTO == null)
                            return Json(new { Status = false, Message = ResLabelPrinting.MasterTemplateNotFound }, JsonRequestBehavior.AllowGet); 
                    }
                    else
                        return Json(new { Status = false, Message = ResLabelPrinting.DesignLabelForModule }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { Status = false, Message = ResLabelPrinting.SelectLabelTemplateForModule }, JsonRequestBehavior.AllowGet);

                return Json(new { Status = true, Message = ResCommon.Success }, JsonRequestBehavior.AllowGet); 
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ResCommon.ErrorColon + " " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
            finally
            {

            }






        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public FileStreamResult GenerateLabelsPDF(FormCollection frm)
        {
            LabelFieldModuleTemplateDetailDAL objLabelDetailDAL = null;
            LabelTemplateMasterDAL objTemplateMasterDAL = null;
            LabelModuleTemplateDetailDAL objModuleTemplateDAL = null;
            LabelModuleMasterDAL objModuelMasterDAL = null;
            LabelModuleFieldMasterDAL objModuleFieldMasterDAL = null;

            LabelModuleTemplateDetailDTO objModuleTemplateDTO = null;
            LabelFieldModuleTemplateDetailDTO objLabelDetailDTO = null;
            LabelTemplateMasterDTO objTemplateDTO = null;
            LabelModuleMasterDTO objModuelMasterDTO = null;
            Document doc = null;
            MemoryStream fs = null;
            PdfWriter pdfWriter = null;
            PdfPTable table = null;
            List<object> lstObjects = null;
            List<object> lstObjs = null;
            List<float> lstFL = null;
            PdfPTable innerTable = null;
            Dictionary<string, IElement> supplierLogoDictionary = null;

            try
            {
                Int64 ModuleID = int.Parse(frm["hdnModuleID"]);
                string IDs = frm["hdnIDs"];
                string ROTDIDs = frm["hdnROTDIds"];
                string CallFrom = frm["hdnCallFrom"];
                int NoOfCopy = 0;
                int.TryParse(frm["txtNoOfLableCopy"], out NoOfCopy);
                if (NoOfCopy <= 0)
                {
                    NoOfCopy = 1;
                }
                int NoOfUsedLabel = 0;
                int.TryParse(frm["txtNoUsedLable"], out NoOfUsedLabel);

                string SortFields = frm["hdnSortField"];
                if (!string.IsNullOrEmpty(SortFields) && SortFields.ToLower().Contains("null"))
                    SortFields = string.Empty;


                objLabelDetailDAL = new LabelFieldModuleTemplateDetailDAL(SessionHelper.EnterPriseDBName);
                objTemplateMasterDAL = new LabelTemplateMasterDAL(SessionHelper.EnterPriseDBName);
                objModuleTemplateDAL = new LabelModuleTemplateDetailDAL(SessionHelper.EnterPriseDBName);
                objModuelMasterDAL = new LabelModuleMasterDAL(SessionHelper.EnterPriseDBName);
                objModuleFieldMasterDAL = new LabelModuleFieldMasterDAL(SessionHelper.EnterPriseDBName);

                //objModuleTemplateDTO = objModuleTemplateDAL.GetAllRecords(SessionHelper.CompanyID, SessionHelper.RoomID).FirstOrDefault(x => x.ModuleID == ModuleID && x.CompanyID == SessionHelper.CompanyID && x.RoomID == SessionHelper.RoomID);
                objModuleTemplateDTO = objModuleTemplateDAL.GetLabelModuleTemplateDetailByRoomCompanyModuleID(SessionHelper.CompanyID, SessionHelper.RoomID, ModuleID).FirstOrDefault();
                //objLabelDetailDTO = objLabelDetailDAL.GetRecord(objModuleTemplateDTO.TemplateDetailID, SessionHelper.CompanyID, SessionHelper.RoomID);
                objLabelDetailDTO = objLabelDetailDAL.GetLabelFieldModuleTemplateDetailByID(objModuleTemplateDTO.TemplateDetailID, SessionHelper.CompanyID, SessionHelper.RoomID);
                //objTemplateDTO = objTemplateMasterDAL.GetRecord(objLabelDetailDTO.TemplateID, SessionHelper.CompanyID);
                objTemplateDTO = objTemplateMasterDAL.GetLabelTemplateMasterByCompanyTemplateID(objLabelDetailDTO.TemplateID, SessionHelper.CompanyID);
                //objModuelMasterDTO = objModuelMasterDAL.GetRecord(ModuleID);
                objModuelMasterDTO = objModuelMasterDAL.GetLabelModuleMasterByID(ModuleID);

                Int64[] arrIDs = IDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(Int64.Parse).ToArray();
                objLabelDetailDTO.lstModuleFields = objModuleFieldMasterDAL.GetRecordsModueWise(ModuleID, SessionHelper.CompanyID, true).ToList();
                Int64[] ints = objLabelDetailDTO.FeildIDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(Int64.Parse).ToArray();
                objLabelDetailDTO.lstSelectedModuleFields = objLabelDetailDTO.lstModuleFields.Where(x => ints.Contains(x.ID)).ToList();

                _CostDecimalFormat = CostDecimalFormat;
                _QtyDecimalFormat = QtyDecimalFormat;

                lstObjects = GetModuleWiseObjecList(objModuelMasterDTO, arrIDs, SortFields, objLabelDetailDTO, ROTDIDs, CallFrom);
                lstObjs = new List<object>();
                foreach (var item in lstObjects)
                {
                    for (int c = 0; c < NoOfCopy; c++)
                    {
                        lstObjs.Add(item);
                    }
                }

                for (int i = 0; i < NoOfUsedLabel; i++)
                {
                    lstObjs.Insert(i, null);
                }

                float pageWidth = ConvertInchToPoint(objTemplateDTO.PageWidth);
                float pageHeight = ConvertInchToPoint(objTemplateDTO.PageHeight);
                float pageLeftMarging = ConvertInchToPoint(objTemplateDTO.PageMarginLeft);
                float pageRightMarging = ConvertInchToPoint(objTemplateDTO.PageMarginRight);
                float pageTopMarging = ConvertInchToPoint(objTemplateDTO.PageMarginTop);
                float pageBottomMarging = ConvertInchToPoint(objTemplateDTO.PageMarginBottom);

                doc = new Document(new Rectangle(pageWidth, pageHeight));
                doc.SetMargins(pageLeftMarging, pageRightMarging, pageTopMarging, pageBottomMarging);

                fs = new MemoryStream();
                pdfWriter = PdfWriter.GetInstance(doc, fs);
                //PageEventHelper pageEventHelper = new PageEventHelper(0, 0);
                //pdfWriter.PageEvent = pageEventHelper;
                doc.Open();

                lstFL = new List<float>();
                float labelWidth = ConvertInchToPoint(objTemplateDTO.LabelWidth);
                supplierLogoDictionary = new Dictionary<string, IElement>();

                for (int i = 0; i < objTemplateDTO.NoOfColumns; i++)
                {
                    float lblWidth = 0;
                    lblWidth = ConvertInchToPoint(objTemplateDTO.LabelWidth);
                    lstFL.Add(lblWidth);

                    if (objTemplateDTO.LabelSpacingHorizontal > 0 && i < objTemplateDTO.NoOfColumns - 1)
                    {
                        lblWidth = ConvertInchToPoint(objTemplateDTO.LabelSpacingHorizontal);
                        lstFL.Add(lblWidth);
                    }
                }

                int NoOfRowsInOnePage = objTemplateDTO.NoOfLabelPerSheet / objTemplateDTO.NoOfColumns;
                int NoOfCellsInOneRow = lstFL.Count();
                int NoOfLabelsInPageWithSpace = NoOfCellsInOneRow * NoOfRowsInOnePage;
                int NoOfPages = (int)Math.Ceiling((decimal)lstObjs.Count() / (decimal)objTemplateDTO.NoOfLabelPerSheet);
                int CurrentLabelIndexInAccrossAllPages = 1;

                if (NoOfPages <= 0)
                {
                    table = new PdfPTable(1);
                    PdfPCell tabelCell = GetLabelTableCell(objTemplateDTO);
                    tabelCell.AddElement(new Phrase(ResLabelPrinting.MsgNotDataFound));
                    table.AddCell(tabelCell);
                    doc.Add(table);
                }

                for (int k = 0; k < NoOfPages; k++)
                {
                    if (k > 0)
                        doc.NewPage();

                    table = new PdfPTable(lstFL.Count());
                    table.WidthPercentage = 100f;

                    table.SetWidths(lstFL.ToArray<float>());
                    table.DefaultCell.Border = 1;
                    table.SkipFirstHeader = true;
                    table.SkipLastFooter = true;
                    int currentLabelIndexInTabel = 1;
                    int currentRowIndex = 1;
                    int currentCellIndex = 1;

                    for (int i = 0; i < NoOfLabelsInPageWithSpace; i++)
                    {
                        PdfPCell tabelCell = GetLabelTableCell(objTemplateDTO);
                        if (lstFL[currentCellIndex - 1] == labelWidth)
                        {
                            if (lstObjs.Count() >= CurrentLabelIndexInAccrossAllPages && lstObjs[CurrentLabelIndexInAccrossAllPages - 1] != null)
                            {
                                innerTable = GetCellContentFromXMLNew(objTemplateDTO, lstObjs[CurrentLabelIndexInAccrossAllPages - 1], objModuelMasterDTO, objLabelDetailDTO,ref supplierLogoDictionary, pdfWriter, CallFrom);
                                tabelCell.AddElement(innerTable);
                            }
                            else
                            {
                                //tabelCell.AddElement(new Phrase("Label In Table: " + currentLabelIndexInTabel));
                                tabelCell.AddElement(new Phrase(""));
                            }

                            currentLabelIndexInTabel += 1;
                            CurrentLabelIndexInAccrossAllPages += 1;
                        }
                        table.AddCell(tabelCell);
                        if (currentCellIndex == lstFL.Count())
                        {
                            currentCellIndex = 1;
                            table.CompleteRow();
                            currentRowIndex += 1;
                            AddVerticalSpaceInLabelsRow(objTemplateDTO, table, NoOfCellsInOneRow, currentRowIndex, NoOfRowsInOnePage);
                        }
                        else
                        {
                            currentCellIndex += 1;
                        }

                    }
                    //table.CompleteRow();
                    doc.Add(table);

                    //if (CurrentLabelIndexInAccrossAllPages < lstObjs.Count)
                    //{
                    //    doc.NewPage();
                    //}
                }
                pdfWriter.CloseStream = false;
                doc.Close();
                pdfWriter.Dispose();
                fs.Position = 0;
                //string strFileName = DateTime.Now.ToString("yyyyMMdd_HHmmssfff") + "_" + Guid.NewGuid().ToString() + ".pdf";
                //return File(fs, "application/pdf", strFileName);
                return File(fs, "application/pdf");
            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                throw ex;
            }
            finally
            {

                fs = null;
                doc = null;
                pdfWriter = null;
                table = null;
                innerTable = null;

                objLabelDetailDAL = null;
                objTemplateMasterDAL = null;
                objModuleTemplateDAL = null;
                objModuelMasterDAL = null;
                objModuleFieldMasterDAL = null;
                objModuleTemplateDTO = null;
                objLabelDetailDTO = null;
                objTemplateDTO = null;
                objModuelMasterDTO = null;
                lstObjects = null;
                lstObjs = null;
                lstFL = null;

            }
        }

        private void AddVerticalSpaceInLabelsRow(LabelTemplateMasterDTO objTemplateDTO, PdfPTable table, int NoOfCellsInOneRows, int CurrentRowIndex, int NoOfRowsInOnePage)
        {

            if (objTemplateDTO.LabelSpacingVerticle > 0 && CurrentRowIndex <= NoOfRowsInOnePage)
            {
                for (int k = 0; k < NoOfCellsInOneRows; k++)
                {
                    PdfPCell tabelVSpaceCell = new PdfPCell();
                    tabelVSpaceCell.Border = 0;
                    tabelVSpaceCell.BorderWidthTop = 0.005F;
                    tabelVSpaceCell.BorderWidthLeft = 0.005F;
                    tabelVSpaceCell.BorderWidthRight = 0.005F;
                    tabelVSpaceCell.BorderWidthBottom = 0.005F;
                    tabelVSpaceCell.BorderColor = BaseColor.WHITE;
                    tabelVSpaceCell.FixedHeight = ConvertInchToPoint(objTemplateDTO.LabelSpacingVerticle);
                    tabelVSpaceCell.VerticalAlignment = Element.ALIGN_TOP;
                    tabelVSpaceCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(tabelVSpaceCell);
                }
            }
        }

        private PdfPCell GetLabelTableCell(LabelTemplateMasterDTO objTemplateDTO)
        {
            PdfPCell tabelCell = new PdfPCell();
            tabelCell.PaddingLeft = ConvertInchToPoint(objTemplateDTO.LabelPaddingLeft);
            tabelCell.PaddingRight = ConvertInchToPoint(objTemplateDTO.LabelPaddingRight);
            tabelCell.PaddingTop = ConvertInchToPoint(objTemplateDTO.LabelPaddingTop);
            tabelCell.PaddingBottom = ConvertInchToPoint(objTemplateDTO.LabelPaddingBottom);
            tabelCell.Border = 0;
            tabelCell.BorderWidthTop = 0.00F;
            tabelCell.BorderWidthLeft = 0.00F;
            tabelCell.BorderWidthRight = 0.00F;
            tabelCell.BorderWidthBottom = 0.00F;
            tabelCell.BorderColor = BaseColor.RED;
            tabelCell.FixedHeight = ConvertInchToPoint(objTemplateDTO.LabelHeight);
            tabelCell.VerticalAlignment = Element.ALIGN_TOP;
            tabelCell.HorizontalAlignment = Element.ALIGN_LEFT;
            return tabelCell;
        }

        /// <summary>
        /// LoadLabelsInPDF
        /// </summary>
        /// <param name="ModuleID"></param>
        /// <param name="IDs"></param>
        /// <returns></returns>
        public FileStreamResult LoadLabelsInPDF(Int64 ModuleID, string IDs)
        {
            CurrentIndex = 1;
            LabelModuleTemplateDetailDAL objMTDetailDAL = null;
            LabelFieldModuleTemplateDetailDAL objFMTDetailDAL = null;
            LabelTemplateMasterDAL objTMDAL = null;
            LabelModuleTemplateDetailDTO objMTDetailDTO = null;
            LabelFieldModuleTemplateDetailDTO objFMTDetailDTO = null;
            LabelTemplateMasterDTO objTemplateDTO = null;
            LabelModuleMasterDAL objMMDAL = null;
            LabelModuleFieldMasterDAL objMFDAL = null;
            LabelModuleMasterDTO objMMDTO = null;

            MemoryStream fs = null;
            Document doc = null;
            PdfWriter pdfWriter = null;

            List<float> lstFL = null;
            PdfPTable table = null;
            Int64[] arrIDs = null;
            Dictionary<string, IElement> supplierLogoDictionary = null;
            try
            {
                fs = new MemoryStream();
                objMTDetailDAL = new LabelModuleTemplateDetailDAL(SessionHelper.EnterPriseDBName);
                objFMTDetailDAL = new LabelFieldModuleTemplateDetailDAL(SessionHelper.EnterPriseDBName);
                objTMDAL = new LabelTemplateMasterDAL(SessionHelper.EnterPriseDBName);
                objMMDAL = new LabelModuleMasterDAL(SessionHelper.EnterPriseDBName);
                objMFDAL = new LabelModuleFieldMasterDAL(SessionHelper.EnterPriseDBName);



                //objMTDetailDTO = objMTDetailDAL.GetAllRecords(SessionHelper.CompanyID, SessionHelper.RoomID).FirstOrDefault(x => x.ModuleID == ModuleID && x.CompanyID == SessionHelper.CompanyID && x.RoomID == SessionHelper.RoomID);
                objMTDetailDTO = objMTDetailDAL.GetLabelModuleTemplateDetailByRoomCompanyModuleID(SessionHelper.CompanyID, SessionHelper.RoomID, ModuleID).FirstOrDefault();

                //objFMTDetailDTO = objFMTDetailDAL.GetRecord(objMTDetailDTO.TemplateDetailID, SessionHelper.CompanyID, SessionHelper.RoomID);
                objFMTDetailDTO = objFMTDetailDAL.GetLabelFieldModuleTemplateDetailByID(objMTDetailDTO.TemplateDetailID, SessionHelper.CompanyID, SessionHelper.RoomID);
                objFMTDetailDTO.lstModuleFields = objMFDAL.GetRecordsModueWise(ModuleID, SessionHelper.CompanyID, true).ToList();
                //objTemplateDTO = objTMDAL.GetRecord(objFMTDetailDTO.TemplateID, SessionHelper.CompanyID);
                objTemplateDTO = objTMDAL.GetLabelTemplateMasterByCompanyTemplateID(objFMTDetailDTO.TemplateID, SessionHelper.CompanyID);

                //objMMDTO = objMMDAL.GetRecord(ModuleID);
                objMMDTO = objMMDAL.GetLabelModuleMasterByID(ModuleID);

                Int64[] ints = objFMTDetailDTO.FeildIDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(Int64.Parse).ToArray();


                objFMTDetailDTO.lstSelectedModuleFields = objFMTDetailDTO.lstModuleFields.Where(x => ints.Contains(x.ID)).ToList();


                doc = new Document(new Rectangle(ConvertInchToPoint(objTemplateDTO.PageWidth), ConvertInchToPoint(objTemplateDTO.PageHeight)));
                doc.SetMargins(ConvertInchToPoint(objTemplateDTO.PageMarginLeft), ConvertInchToPoint(objTemplateDTO.PageMarginRight), ConvertInchToPoint(objTemplateDTO.PageMarginTop), ConvertInchToPoint(objTemplateDTO.PageMarginBottom));

                pdfWriter = PdfWriter.GetInstance(doc, fs);
                doc.Open();
                lstFL = new List<float>();

                for (int i = 0; i < objTemplateDTO.NoOfColumns; i++)
                {
                    lstFL.Add(ConvertInchToPoint(objTemplateDTO.LabelWidth));
                    if (objTemplateDTO.LabelSpacingHorizontal > 0 && i < objTemplateDTO.NoOfColumns - 1)
                        lstFL.Add(ConvertInchToPoint(objTemplateDTO.LabelSpacingHorizontal));
                }

                table = new PdfPTable(lstFL.Count());
                table.WidthPercentage = 100f;
                table.HorizontalAlignment = 0;

                table.SetWidths(lstFL.ToArray<float>());
                table.DefaultCell.Border = 0;

                table.SkipFirstHeader = true;
                table.SkipLastFooter = true;

                arrIDs = IDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(Int64.Parse).ToArray();

                List<object> lstObjs = GetModuleWiseObjecList(objMMDTO, arrIDs, "", objFMTDetailDTO, "", "");
                supplierLogoDictionary = new Dictionary<string, IElement>();

                if (lstObjs != null & lstObjs.Count > 0)
                {
                    foreach (object item in lstObjs)
                    {
                        if (objTemplateDTO.NoOfColumns > 1 && CurrentIndex % 2 == 0)
                        {
                            table.AddCell(GetTabelCell(objTemplateDTO));
                        }

                        PdfPCell cell = GetTabelCell(objTemplateDTO);
                        if (!string.IsNullOrEmpty(objFMTDetailDTO.LabelXML) && objFMTDetailDTO.LabelXML.Trim().Length > 0)
                        {
                            cell.AddElement(GetCellContentFromXMLNew(objTemplateDTO, item, objMMDTO, objFMTDetailDTO, supplierLogoDictionary: ref supplierLogoDictionary));
                        }
                        else
                        {
                            cell.AddElement(GetCellContent(objTemplateDTO, item, objMMDTO, objFMTDetailDTO));
                        }
                        table.AddCell(cell);

                        if (lstFL.Count < CurrentIndex)
                        {
                            CurrentIndex = 1;
                        }
                    }
                }
                else
                {
                    table.AddCell(new PdfPCell(new Phrase(ResLabelPrinting.MsgNotDataFound)));
                }

                table.CompleteRow();
                doc.Add(table);

                pdfWriter.CloseStream = false;
                doc.Close();
                pdfWriter.Dispose();
                fs.Position = 0;
                return File(fs, "application/pdf");

            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                //fs.Flush();
                //fs.Dispose();
                doc.Dispose();
                pdfWriter.Dispose();
                fs = null;
                doc = null;
                pdfWriter = null;

                objMTDetailDAL = null;
                objFMTDetailDAL = null;
                objTMDAL = null;
                objMTDetailDTO = null;
                objFMTDetailDTO = null;
                objTemplateDTO = null;
                objMMDAL = null;
                objMFDAL = null;
                objMMDTO = null;

                lstFL = null;
                table = null;
                arrIDs = null;
            }




        }

        /// <summary>
        /// GetTabelCell
        /// </summary>
        /// <param name="objLabelSize"></param>
        /// <returns></returns>
        private PdfPCell GetTabelCell(LabelTemplateMasterDTO objLabelSize)
        {
            PdfPCell tabelCell = new PdfPCell();

            tabelCell.Border = 0;
            tabelCell.BorderWidthTop = TableCellBorderWidth;
            tabelCell.BorderWidthLeft = TableCellBorderWidth;
            tabelCell.BorderWidthRight = TableCellBorderWidth;
            tabelCell.BorderWidthBottom = TableCellBorderWidth;
            tabelCell.BorderColor = BaseColor.RED;

            //tabelCell.PaddingLeft = (float)objLabelSize.LabelPaddingLeft * 0.72f;
            //tabelCell.PaddingRight = (float)objLabelSize.LabelPaddingRight * 0.72f;
            //tabelCell.PaddingTop = (float)objLabelSize.LabelPaddingTop * 0.72f;
            //tabelCell.PaddingBottom = (float)objLabelSize.LabelPaddingBottom * 0.72f;
            tabelCell.PaddingLeft = ConvertInchToPoint(objLabelSize.LabelPaddingLeft);
            tabelCell.PaddingRight = ConvertInchToPoint(objLabelSize.LabelPaddingRight);
            tabelCell.PaddingTop = ConvertInchToPoint(objLabelSize.LabelPaddingTop);
            tabelCell.PaddingBottom = ConvertInchToPoint(objLabelSize.LabelPaddingBottom);

            tabelCell.FixedHeight = ConvertInchToPoint(objLabelSize.LabelHeight);

            tabelCell.VerticalAlignment = Element.ALIGN_TOP;
            tabelCell.HorizontalAlignment = Element.ALIGN_LEFT;
            CurrentIndex += 1;
            return tabelCell;

        }

        /// <summary>
        /// GetTabelCell
        /// </summary>
        /// <param name="objLabelSize"></param>
        /// <returns></returns>
        private PdfPCell GetTabelCellForVerticleSpace(LabelTemplateMasterDTO objLabelSize)
        {
            PdfPCell tabelCell = new PdfPCell();

            tabelCell.Border = 0;
            tabelCell.BorderWidthTop = TableCellBorderWidth;
            tabelCell.BorderWidthLeft = TableCellBorderWidth;
            tabelCell.BorderWidthRight = TableCellBorderWidth;
            tabelCell.BorderWidthBottom = TableCellBorderWidth;
            tabelCell.BorderColor = BaseColor.GREEN;

            tabelCell.FixedHeight = ConvertInchToPoint(objLabelSize.LabelSpacingVerticle);

            tabelCell.VerticalAlignment = Element.ALIGN_TOP;
            tabelCell.HorizontalAlignment = Element.ALIGN_LEFT;
            CurrentIndexVS += 1;
            return tabelCell;

        }


        /// <summary>
        /// GetContentCell
        /// </summary>
        /// <param name="e"></param>
        /// <param name="Colspan"></param>
        /// <returns></returns>
        private PdfPCell GetContentCell(IElement e, int Colspan)
        {
            PdfPCell cell = new PdfPCell();
            cell.VerticalAlignment = Element.ALIGN_TOP;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.MinimumHeight = 2;
            cell.Padding = 0.0f;
            cell.PaddingTop = 0.0f;
            cell.PaddingLeft = 0.0f;
            cell.PaddingRight = 0.0f;
            cell.PaddingBottom = 0.0f;
            cell.UseBorderPadding = true;
            cell.UseAscender = true;
            cell.Colspan = Colspan;
            cell.Border = 0;
            cell.BorderWidthTop = 0.0f;
            cell.BorderWidthLeft = 0.0f;
            cell.BorderWidthRight = 0.0f;
            cell.BorderWidthBottom = 0.0f;
            cell.BorderColor = BaseColor.RED;

            cell.AddElement(e);

            return cell;
        }

        /// <summary>
        /// GetBarcode
        /// </summary>
        /// <param name="barcodeString"></param>
        public byte[] GetBarcode(LabelTemplateMasterDTO objSizeDTO, string strBarcode, int width, int Height, bool IncludeLabel)
        {
            BarcodeLib.Barcode b = new BarcodeLib.Barcode();
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.Drawing.Image img = null;
            try
            {
                b.Alignment = BarcodeLib.AlignmentPositions.LEFT;
                BarcodeLib.TYPE type = BarcodeLib.TYPE.CODE128;
                b.IncludeLabel = IncludeLabel;
                //b.RotateFlipType = (RotateFlipType)Enum.Parse(typeof(RotateFlipType), "RotateNoneFlipNone", true);
                b.LabelPosition = BarcodeLib.LabelPositions.BOTTOMLEFT;
                img = b.Encode(type, strBarcode, System.Drawing.Color.Black, System.Drawing.Color.White, width, Height);
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                return ms.ToArray();

            }
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            finally
            {
                b.Dispose();
                ms.Dispose();
                if (img != null)
                    img.Dispose();
            }
        }

        /// <summary>
        /// GetCellContent
        /// </summary>
        /// <param name="objLabelSize"></param>
        /// <param name="item"></param>
        /// <param name="objMMDTO"></param>
        /// <param name="objLFMTDetailDTO"></param>
        /// <returns></returns>
        private PdfPTable GetCellContent(LabelTemplateMasterDTO objLabelSize, object item, LabelModuleMasterDTO objMMDTO, LabelFieldModuleTemplateDetailDTO objLFMTDetailDTO)
        {
            List<float> lstFL = new List<float>();

            float fwidth = ConvertInchToPoint(objLabelSize.LabelWidth);
            //float leftrightpading = ((float)objLabelSize.LabelPaddingLeft * 0.72f) + ((float)objLabelSize.LabelPaddingRight * 0.72f);
            float leftrightpading = (ConvertInchToPoint(objLabelSize.LabelPaddingLeft) + ConvertInchToPoint(objLabelSize.LabelPaddingRight));
            int intWidth = (int)(fwidth - leftrightpading);

            if (objLabelSize.NoOfColumns <= 3)
            {
                lstFL.Add((intWidth * 20) / 100);
                lstFL.Add((intWidth * 30) / 100);
                lstFL.Add((intWidth * 20) / 100);
                lstFL.Add((intWidth * 30) / 100);
            }
            else
            {
                lstFL.Add((intWidth * 30) / 100);
                lstFL.Add((intWidth * 70) / 100);
            }

            Font verdanaBold = FontFactory.GetFont("Verdana", BaseFont.CP1252, true, (float)objLFMTDetailDTO.FontSize, Font.BOLD);
            Font verdanaNormal = FontFactory.GetFont("Verdana", BaseFont.CP1252, true, (float)objLFMTDetailDTO.FontSize, Font.NORMAL);

            PdfPTable table = new PdfPTable(lstFL.Count());

            table.SkipFirstHeader = true;
            table.SkipLastFooter = true;
            table.WidthPercentage = 100f;
            table.HorizontalAlignment = 0;
            table.DefaultCell.Border = 0;

            table.SetWidths(lstFL.ToArray<float>());

            string BarcodeString = "";
            PropertyInfo info = null;
            object value = null;
            PdfPCell cell = null;

            string barcodefield = objLFMTDetailDTO.lstModuleFields.FirstOrDefault(x => x.ID == objLFMTDetailDTO.BarcodeKey).FieldName;
            info = item.GetType().GetProperty(barcodefield);
            value = info.GetValue(item, null);
            BarcodeString = "#" + value;
            if (objMMDTO.ModuleDTOName == "InventoryLabelDTO"
                    || objMMDTO.ModuleDTOName == "OrderLabelDTO"
                    || objMMDTO.ModuleDTOName == "ReceiveLabelDTO"
                    || objMMDTO.ModuleDTOName == "StagingLabelDTO"
                    || objMMDTO.ModuleDTOName == "TransferLabelDTO")
            {

                if (objLFMTDetailDTO.IncludeBin)
                {
                    info = item.GetType().GetProperty("BinNumber");
                    string strBin = Convert.ToString(info.GetValue(item, null));
                    if (!string.IsNullOrEmpty(strBin))
                    {
                        BarcodeString += "@" + strBin;
                    }
                }

                if (objLFMTDetailDTO.IncludeQuantity)
                {
                    if (objLFMTDetailDTO.QuantityField != "1")
                    {
                        info = item.GetType().GetProperty(objLFMTDetailDTO.QuantityField);
                        value = info.GetValue(item, null);
                    }
                    else
                        value = objLFMTDetailDTO.QuantityField;

                    BarcodeString += "<" + Convert.ToString(value);
                }
            }

            byte[] b = GetBarcode(objLabelSize, BarcodeString, intWidth, 30, false);
            iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(b);
            img.SetDpi(96, 96);
            if (img.Width > (ConvertInchToPoint(objLabelSize.LabelWidth)))
                img.ScaleToFitLineWhenOverflow = true;

            cell = GetContentCell(img, lstFL.Count);
            cell.FixedHeight = 30;
            table.AddCell(cell);
            foreach (var fl in objLFMTDetailDTO.lstSelectedModuleFields)
            {
                info = item.GetType().GetProperty(fl.FieldName);
                value = info.GetValue(item, null);
                cell = GetContentCell(new Chunk(fl.FieldDisplayName, verdanaNormal), 1);
                cell.FixedHeight = (float)(objLFMTDetailDTO.FontSize * 2);
                table.AddCell(cell);
                cell = GetContentCell(new Chunk((string)value, verdanaNormal), 1);
                cell.FixedHeight = (float)(objLFMTDetailDTO.FontSize * 2);
                table.AddCell(cell);
            }

            return table;
        }

        /// <summary>
        /// GetCellContent
        /// </summary>
        /// <param name="objLabelSize"></param>
        /// <param name="item"></param>
        /// <param name="objMMDTO"></param>
        /// <param name="objLFMTDetailDTO"></param>
        /// <returns></returns>
        private PdfPTable GetCellContentFromXMLNew(LabelTemplateMasterDTO objLabelSize, object item, LabelModuleMasterDTO objMMDTO, 
            LabelFieldModuleTemplateDetailDTO objLFMTDetailDTO, ref Dictionary<string, IElement> supplierLogoDictionary, PdfWriter cb = null, string CallFrom = "" )
        {

            XmlDocument objDoc = new XmlDocument();
            objDoc.LoadXml(objLFMTDetailDTO.LabelXML);
            int rowCount = objDoc.SelectNodes("mytable/row").Count;
            //List<float> lstCellWithed = null;
            int MaxNoOfCols = 0;

            if (rowCount > 0)
            {
                //lstCellWithed = new List<float>();

                foreach (XmlElement row in objDoc.SelectNodes("mytable/row"))
                {
                    int NoOfColsInCurrRow = 0;
                    foreach (XmlElement child in row.SelectNodes("column"))
                    {
                        int ColSpan = 1;
                        string strColSpan = Convert.ToString(child.GetAttribute("colspan"));
                        int.TryParse(strColSpan, out ColSpan);
                        NoOfColsInCurrRow += ColSpan;
                        //string strwidth = Convert.ToString(child.GetAttribute("width")).Replace("px", "");
                        //float width = 0;
                        //float.TryParse(strwidth, out width);
                        //lstCellWithed.Add(ConvertPixelToPoint(width));
                    }

                    if (NoOfColsInCurrRow > MaxNoOfCols)
                        MaxNoOfCols = NoOfColsInCurrRow;
                }
            }

            //if (lstCellWithed == null || lstCellWithed.Count <= 0)
            //    return null;

            if (MaxNoOfCols <= 0)
                return null;

            float fwidth = ConvertInchToPoint(objLabelSize.LabelWidth);
            float leftrightpading = ConvertInchToPoint(objLabelSize.LabelPaddingLeft) + ConvertInchToPoint(objLabelSize.LabelPaddingRight);
            int intWidth = (int)(fwidth - leftrightpading);

            PdfPTable InnerTable = new PdfPTable(MaxNoOfCols);
            InnerTable.WidthPercentage = 100f;

            InnerTable.TotalWidth = (float)intWidth;
            InnerTable.LockedWidth = true;

            InnerTable.HorizontalAlignment = 0;
            InnerTable.DefaultCell.Border = 0;

            //InnerTable.SetWidths(lstCellWithed.ToArray<float>());

            InnerTable.SkipFirstHeader = true;
            InnerTable.SkipLastFooter = true;

            string strfName = Convert.ToString(ConfigurationManager.AppSettings["BaseFilePath"]) + @"\fonts\ARIALUNI.TTF";
            //if (!string.IsNullOrEmpty(objLFMTDetailDTO.TextFont))
            //    strfName = objLFMTDetailDTO.TextFont;

            Font cellFont = FontFactory.GetFont(strfName, BaseFont.IDENTITY_H, true, (float)objLFMTDetailDTO.FontSize, Font.NORMAL);
            long SessionUserId = SessionHelper.UserID;
            

            foreach (XmlElement row in objDoc.SelectNodes("mytable/row"))
            {
                foreach (XmlElement child in row.SelectNodes("column"))
                {
                    #region GetAndSet Attribute of child from xml

                    float fontSize = 0, fltheight = 0, flwidth = 0, imgWidth = 0, imgHeight = 0;
                    int RowSpan = 1, ColSpan = 1;

                    IElement cellContent = null;

                    string strfontstyle = Convert.ToString(child.GetAttribute("fontstyle"));
                    string strfontsize = Convert.ToString(child.GetAttribute("fontSize"));
                    string strRowSpan = Convert.ToString(child.GetAttribute("rowspan"));
                    string strColSpan = Convert.ToString(child.GetAttribute("colspan"));
                    string strheight = Convert.ToString(child.GetAttribute("height")).Replace("px", "");
                    string strflwidth = Convert.ToString(child.GetAttribute("width")).Replace("px", "");
                    string strImgHeight = Convert.ToString(child.GetAttribute("ImageHeight")).Replace("px", "");
                    string strImgWidth = Convert.ToString(child.GetAttribute("ImageWidth")).Replace("px", "");
                    string strCellalignment = Convert.ToString(child.GetAttribute("cellalignment")).Replace("px", "");
                    int alignment = Element.ALIGN_LEFT;
                    switch (strCellalignment.ToLower().Trim())
                    {
                        case "right":
                            alignment = Element.ALIGN_RIGHT;
                            break;
                        case "center":
                            alignment = Element.ALIGN_CENTER;
                            break;
                        case "justify":
                            alignment = Element.ALIGN_JUSTIFIED;
                            break;
                        default:
                            alignment = Element.ALIGN_LEFT;
                            break;
                    }

                    int.TryParse(strRowSpan, out RowSpan);
                    int.TryParse(strColSpan, out ColSpan);
                    float.TryParse(strfontsize, out fontSize);
                    float.TryParse(strheight, out fltheight);
                    float.TryParse(strflwidth, out flwidth);
                    float.TryParse(strImgHeight, out imgHeight);
                    float.TryParse(strImgWidth, out imgWidth);
                    int qrCodeSize = (int)imgWidth;
                    if (strfontstyle.ToUpper().Equals("B"))
                        cellFont.SetStyle(1);
                    else if (strfontstyle.ToUpper().Equals("I"))
                        cellFont.SetStyle(2);
                    else if (strfontstyle.ToUpper().Equals("BI"))
                        cellFont.SetStyle(3);
                    else
                        cellFont.SetStyle(0);

                    if (RowSpan <= 0)
                        RowSpan = 1;

                    if (ColSpan <= 0)
                        ColSpan = 1;

                    if (fontSize > 0)
                        cellFont.Size = fontSize;
                    if (flwidth > 0)
                        flwidth = ConvertPixelToPoint(flwidth);

                    if (fltheight > 0)
                        fltheight = ConvertPixelToPoint(fltheight);

                    if (imgWidth > 0)
                        imgWidth = ConvertPixelToPoint(imgWidth);

                    if (imgHeight > 0)
                        imgHeight = ConvertPixelToPoint(imgHeight);

                    #endregion

                    if (Convert.ToString(child.InnerText).Trim().Equals("Barcode_Row"))
                    {
                        string optionBarcodeKey = "";
                        string uniqueIdentifier = GetPropertyValueFromObject("ItemUniqueNumber", item);
                        if (string.IsNullOrEmpty(uniqueIdentifier) &&
                            (objMMDTO.ModuleDTOName == "InventoryLabelDTO"
                                || objMMDTO.ModuleDTOName == "OrderLabelDTO"
                                || objMMDTO.ModuleDTOName == "ReceiveLabelDTO"
                                || objMMDTO.ModuleDTOName == "StagingLabelDTO"
                                || objMMDTO.ModuleDTOName == "TransferLabelDTO"))
                        {
                            string strItemID = GetPropertyValueFromObject("ID", item);
                            Int64 itemID = 0;
                            Int64.TryParse(strItemID, out itemID);
                            if (itemID > 0)
                            {
                                ItemMasterDAL objItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                                ItemMasterDTO objItemDTO = objItemDAL.GetItemWithoutJoins(itemID, null);
                                objItemDAL.Edit(objItemDTO, SessionUserId, SessionHelper.EnterPriceID);
                                objItemDTO = objItemDAL.GetItemWithoutJoins(itemID, null);
                                uniqueIdentifier = objItemDTO.ItemUniqueNumber;
                                PropertyInfo info = item.GetType().GetProperty("ItemUniqueNumber");
                                info.SetValue(item, uniqueIdentifier, null);
                            }
                        }

                        string BarcodeString = GetBarcodeStringNew(objLFMTDetailDTO, objMMDTO, item, false);

                        if (string.IsNullOrEmpty(BarcodeString) || string.IsNullOrWhiteSpace(BarcodeString))
                            BarcodeString = optionBarcodeKey;

                        //TODO: comment and get image by new function is issue WI-2665 "The Zebra barcodes are not very crisp and users are having trouble scanning because of this"
                        //byte[] b = GetBarcodeNew(objLFMTDetailDTO, objMMDTO, item, objLabelSize, BarcodeString, intWidth, (int)(fltheight), false, Convert.ToString(child.GetAttribute("BarcodeFont")));
                        iTextSharp.text.Image img = GetBarcodeImage(objLFMTDetailDTO, objMMDTO, item, objLabelSize, BarcodeString, (int)(intWidth), (int)(fltheight), false, Convert.ToString(child.GetAttribute("BarcodeFont")), cb);
                        if (img != null)
                        {
                            //
                            img.ScaleToFit((flwidth-3), fltheight);
                            //img.ScaleToFit(flwidth, fltheight);
                            //img.ScaleToFit(intWidth, fltheight);
                            cellContent = img;
                        }

                    }
                    else if (Convert.ToString(child.InnerText).Trim().Equals("SrLtBarcode_Row") && CallFrom == "ItemSerailLotBarcode")
                    {
                        string SerialLotBarcodeString = GetPropertyValueFromObject("SerialOrLotNumber", item);
                        iTextSharp.text.Image img = GetBarcodeImage(objLFMTDetailDTO, objMMDTO, item, objLabelSize, SerialLotBarcodeString, (int)(intWidth), (int)(fltheight), false, Convert.ToString(child.GetAttribute("BarcodeFont")), cb);
                        if (img != null)
                        {
                            img.ScaleToFit((flwidth - 3), fltheight);//img.ScaleToFit(intWidth, fltheight);
                            //img.SetDpi(300, 300);
                            cellContent = img;
                        }
                    }
                    else if (Convert.ToString(child.InnerText).Trim().Equals("SrLtQRcode_Cell") && CallFrom == "ItemSerailLotBarcode")
                    {
                        string SerialLotBarcodeString = GetPropertyValueFromObject("SerialOrLotNumber", item);
                        int scale = (int)(qrCodeSize / 34);

                        byte[] b = GetQRcode(objLFMTDetailDTO, objMMDTO, item, objLabelSize, SerialLotBarcodeString, scale);
                        if (b != null)
                        {
                            iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(b);
                            img.ScaleToFit(fltheight, fltheight);
                            img.SetDpi(300, 300);
                            cellContent = img;
                        }
                    }
                    else if (Convert.ToString(child.InnerText).Trim().Equals("QRcode_Cell"))
                    {
                        string optionBarcodeKey = "";
                        string uniqueIdentifier = GetPropertyValueFromObject("ItemUniqueNumber", item);
                        if (string.IsNullOrEmpty(uniqueIdentifier) &&
                            (objMMDTO.ModuleDTOName == "InventoryLabelDTO"
                                || objMMDTO.ModuleDTOName == "OrderLabelDTO"
                                || objMMDTO.ModuleDTOName == "ReceiveLabelDTO"
                                || objMMDTO.ModuleDTOName == "StagingLabelDTO"
                                || objMMDTO.ModuleDTOName == "TransferLabelDTO"))
                        {
                            string strItemID = GetPropertyValueFromObject("ID", item);
                            Int64 itemID = 0;
                            Int64.TryParse(strItemID, out itemID);
                            if (itemID > 0)
                            {
                                ItemMasterDAL objItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                                ItemMasterDTO objItemDTO = objItemDAL.GetItemWithoutJoins(itemID, null);
                                objItemDAL.Edit(objItemDTO, SessionUserId, SessionHelper.EnterPriceID);
                                objItemDTO = objItemDAL.GetItemWithoutJoins(itemID, null);
                                uniqueIdentifier = objItemDTO.ItemUniqueNumber;
                                PropertyInfo info = item.GetType().GetProperty("ItemUniqueNumber");
                                info.SetValue(item, uniqueIdentifier, null);
                            }
                        }

                        string BarcodeString = GetBarcodeStringNew(objLFMTDetailDTO, objMMDTO, item, false);

                        if (string.IsNullOrEmpty(BarcodeString) || string.IsNullOrWhiteSpace(BarcodeString))
                            BarcodeString = optionBarcodeKey;
                        int scale = (int)(qrCodeSize / 34);

                        //BarcodeQRCode qrCode = new BarcodeQRCode(BarcodeString, qrCodeSize, qrCodeSize, null);
                        //iTextSharp.text.Image img = qrCode.GetImage();
                        //img.ScaleToFit(ConvertPointToPixel(intWidth), ConvertPointToPixel(intWidth));
                        //cellContent = img;

                        byte[] b = GetQRcode(objLFMTDetailDTO, objMMDTO, item, objLabelSize, BarcodeString, scale);
                        if (b != null)
                        {
                            iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(b);
                            img.ScaleToFit(fltheight, fltheight);
                            img.SetDpi(300, 300);
                            cellContent = img;
                        }
                    }
                    else if (Convert.ToString(child.InnerText).Trim().Equals("ItemImage_Cell"))
                    {
                        PropertyInfo info = item.GetType().GetProperty("ImagePath");
                        object objPath = info.GetValue(item, null);
                        if (!string.IsNullOrEmpty(Convert.ToString(objPath)))
                        {
                            string imgPath = Convert.ToString(objPath); //Server.MapPath("..\\InventoryPhoto\\" + SessionHelper.CompanyID + "\\" + Convert.ToString(objPath).Trim());
                            cellContent = GetImageFromRelativePath(imgPath, imgWidth, imgHeight);
                        }
                    }
                    else if (Convert.ToString(child.InnerText).Trim().Equals("AssetImage_Cell"))
                    {
                        PropertyInfo info = item.GetType().GetProperty("AssetImage");
                        object objPath = info.GetValue(item, null);
                        if (!string.IsNullOrEmpty(Convert.ToString(objPath)))
                        {
                            string imgPath = Convert.ToString(objPath); //Server.MapPath("..\\InventoryPhoto\\" + SessionHelper.CompanyID + "\\" + Convert.ToString(objPath).Trim());
                            cellContent = GetImageFromRelativePath(imgPath, imgWidth, imgHeight);
                        }
                    }
                    else if (Convert.ToString(child.InnerText).Trim().Equals("ToolImage_Cell"))
                    {
                        PropertyInfo info = item.GetType().GetProperty("ToolImage");
                        object objPath = info.GetValue(item, null);
                        if (!string.IsNullOrEmpty(Convert.ToString(objPath)))
                        {
                            string imgPath = Convert.ToString(objPath); //Server.MapPath("..\\InventoryPhoto\\" + SessionHelper.CompanyID + "\\" + Convert.ToString(objPath).Trim());
                            cellContent = GetImageFromRelativePath(imgPath, imgWidth, imgHeight);
                        }
                    }
                    else if (Convert.ToString(child.InnerText).Trim().Equals("SupplierImage_Cell"))
                    {
                        PropertyInfo info = item.GetType().GetProperty("SupplierLogo");
                        object objPath = info.GetValue(item, null);
                        if (!string.IsNullOrEmpty(Convert.ToString(objPath)))
                        {
                            string imgPath = Convert.ToString(objPath);// Server.MapPath("..\\Uploads\\" + SessionHelper.CompanyID + "\\" + Convert.ToString(objPath).Trim());
                            
                            if(supplierLogoDictionary.ContainsKey(imgPath))
                            {
                                cellContent = supplierLogoDictionary[imgPath];
                            }
                            else
                            {
                                cellContent = GetImageFromRelativePath(imgPath, imgWidth, imgHeight);
                                supplierLogoDictionary.Add(imgPath, cellContent);
                            }
                            
                        }
                    }
                    else if (Convert.ToString(child.InnerText).Trim().Equals("EnterPriseLogo_Cell"))
                    {
                        if (!string.IsNullOrEmpty(SessionHelper.EnterpriseLogoUrl))
                        {
                            string imgPath = Server.MapPath("..\\Uploads\\EnterpriseLogos\\" + SessionHelper.EnterPriceID + "\\" + SessionHelper.EnterpriseLogoUrl);
                            cellContent = GetImageFromRelativePath(imgPath, imgWidth, imgHeight);

                        }
                    }
                    else if (Convert.ToString(child.InnerText).Trim().Equals("CompanyLogo_Cell"))
                    {
                        if (!string.IsNullOrEmpty(SessionHelper.CompanyLogoUrl))
                        {
                            string imgPath = Server.MapPath("..\\Uploads\\CompanyLogos\\" + SessionHelper.CompanyID + "\\" + SessionHelper.CompanyLogoUrl);
                            cellContent = GetImageFromRelativePath(imgPath, imgWidth, imgHeight);
                        }
                    }
                    else
                    {
                        string cellString = GetStringForCell(objLFMTDetailDTO, child, item);
                        //cellContent = new Phrase(cellString, cellFont);
                        cellContent = new Paragraph(cellString, cellFont);
                        ((Paragraph)cellContent).Alignment = alignment;
                    }

                    PdfPCell InnerTableCell = new PdfPCell();
                    InnerTableCell.AddElement(cellContent);
                    InnerTableCell.Colspan = ColSpan;
                    InnerTableCell.Rowspan = RowSpan;
                    InnerTableCell.VerticalAlignment = Element.ALIGN_TOP;
                    InnerTableCell.UseBorderPadding = true;
                    InnerTableCell.UseAscender = true;
                    InnerTableCell.Padding = 0;
                    InnerTableCell.PaddingBottom = 0;
                    InnerTableCell.PaddingLeft = 0;
                    InnerTableCell.PaddingTop = 0f;
                    InnerTableCell.PaddingRight = 0;
                    InnerTableCell.Border = 1;
                    InnerTableCell.BorderWidth = 0.5f;
                    InnerTableCell.BorderWidthBottom = BorderWidth;
                    InnerTableCell.BorderWidthLeft = BorderWidth;
                    InnerTableCell.BorderWidthRight = BorderWidth;
                    InnerTableCell.BorderWidthTop = BorderWidth;
                    InnerTableCell.BorderColor = BaseColor.BLUE;
                    InnerTableCell.FixedHeight = fltheight;

                    //InnerTableCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    InnerTableCell.HorizontalAlignment = alignment;

                    InnerTable.AddCell(InnerTableCell);

                }
            }

            InnerTable.CompleteRow();
            return InnerTable;

        }

        #region NewFunctions

        private string GetBarcodeStringNew(LabelFieldModuleTemplateDetailDTO objLFMTDetailDTO, LabelModuleMasterDTO objMMDTO, object item, bool IsOptionalBarcode)
        {
            List<string> lstBarcodes = null;
            string barcodeString = string.Empty;
            string barcodefield = objLFMTDetailDTO.lstModuleFields.FirstOrDefault(x => x.ID == objLFMTDetailDTO.BarcodeKey).FieldName;
            if (IsOptionalBarcode)
                barcodefield = "ItemUniqueNumber";

            if (objLFMTDetailDTO.BarcodePattern == "UK")
                lstBarcodes = GetListOfBarcodeStringForUKOnly(objLFMTDetailDTO, objMMDTO, item, barcodefield);
            else if (objLFMTDetailDTO.BarcodePattern == "US")
                lstBarcodes = GetListOfBarcodeStringForUS(objLFMTDetailDTO, objMMDTO, item, barcodefield);
            else if (objLFMTDetailDTO.BarcodePattern == "US01")
                lstBarcodes = GetListOfBarcodeStringForUS01(objLFMTDetailDTO, objMMDTO, item, barcodefield);
            else if (objLFMTDetailDTO.BarcodePattern == "UK01")
            {
                lstBarcodes = GetListOfBarcodeStringForUK01(objLFMTDetailDTO, objMMDTO, item, barcodefield);
            }

            barcodeString = ConcateBarcodeString(lstBarcodes);

            return barcodeString;
        }

        /// <summary>
        /// Create List for UK Barcode String 
        /// </summary>
        /// <param name="objLFMTDetailDTO"></param>
        /// <param name="objMMDTO"></param>
        /// <param name="item"></param>
        /// <param name="PrimaryBarcodeField"></param>
        /// <returns></returns>
        private List<string> GetListOfBarcodeStringForUS(LabelFieldModuleTemplateDetailDTO objLFMTDetailDTO, LabelModuleMasterDTO objMMDTO, object item, string PrimaryBarcodeField)
        {
            List<string> lstReturns = new List<string>();
            string BarcodeString = string.Empty;
            bool isIncludeBinQty = true;

            string strPrimaryBarcodeString = GetPropertyValueFromObject(PrimaryBarcodeField, item);
            if (string.IsNullOrWhiteSpace(strPrimaryBarcodeString))
            {
                strPrimaryBarcodeString = GetPropertyValueFromObject("ItemUniqueNumber", item);
            }
            string strBin = GetPropertyValueFromObject("BinNumber", item);
            if (objLFMTDetailDTO.BarcodeFont == "39" && !string.IsNullOrEmpty(strPrimaryBarcodeString))
            {
                lstReturns.Add(strPrimaryBarcodeString.ToUpper());
                return lstReturns;
            }

            if (objMMDTO.ModuleDTOName == "InventoryLabelDTO")
            {
                string strItemType = GetPropertyValueFromObject("ItemType", item);
                if (!string.IsNullOrEmpty(strItemType) && strItemType == "4")
                    isIncludeBinQty = false;
            }

            if (objMMDTO.ModuleDTOName == "AssetLabelDTO")
                BarcodeString = "#A";
            else if (objMMDTO.ModuleDTOName == "ToolLabelDTO")
                BarcodeString = "#T";
            else if (objMMDTO.ModuleDTOName == "TechnicianLabelDTO")
                BarcodeString = "#E";
            else if (objMMDTO.ModuleDTOName == "LocationLabelDTO")
                BarcodeString = "#B";
            else if (objMMDTO.ModuleDTOName == "WorkOrderLabelDTO")
                BarcodeString = "#W";
            else
                BarcodeString = "#I";

            lstReturns.Add(BarcodeString + strPrimaryBarcodeString);

            if (objLFMTDetailDTO.IncludeBin && isIncludeBinQty && !string.IsNullOrEmpty(strBin))
                lstReturns.Add("#B" + strBin);

            if (objLFMTDetailDTO.IncludeQuantity && isIncludeBinQty)
            {
                if (objLFMTDetailDTO.QuantityField == "1")
                    lstReturns.Add("#Q1");
                else
                {
                    string strQty = GetPropertyValueFromObject(objLFMTDetailDTO.QuantityField, item);
                    string[] arrQty = strQty.Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    strQty = arrQty[0].Replace(",", "");
                    if (!string.IsNullOrEmpty(strQty))
                        lstReturns.Add("#Q" + strQty);
                }
            }

            return lstReturns;
        }

        /// <summary>
        /// Create List for UK Barcode String 
        /// </summary>
        /// <param name="objLFMTDetailDTO"></param>
        /// <param name="objMMDTO"></param>
        /// <param name="item"></param>
        /// <param name="PrimaryBarcodeField"></param>
        /// <returns></returns>
        private List<string> GetListOfBarcodeStringForUS01(LabelFieldModuleTemplateDetailDTO objLFMTDetailDTO, LabelModuleMasterDTO objMMDTO, object item, string PrimaryBarcodeField)
        {
            List<string> lstReturns = new List<string>();
            string BarcodeString = string.Empty;
            bool isIncludeBinQty = true;

            string strPrimaryBarcodeString = GetPropertyValueFromObject(PrimaryBarcodeField, item);
            if (string.IsNullOrWhiteSpace(strPrimaryBarcodeString))
            {
                strPrimaryBarcodeString = GetPropertyValueFromObject("ItemUniqueNumber", item);
            }
            string strBin = GetPropertyValueFromObject("BinNumber", item);
            if (objLFMTDetailDTO.BarcodeFont == "39" && !string.IsNullOrEmpty(strPrimaryBarcodeString))
            {
                lstReturns.Add(strPrimaryBarcodeString.ToUpper());
                return lstReturns;
            }

            if (objMMDTO.ModuleDTOName == "InventoryLabelDTO")
            {
                string strItemType = GetPropertyValueFromObject("ItemType", item);
                if (!string.IsNullOrEmpty(strItemType) && strItemType == "4")
                    isIncludeBinQty = false;
            }

            if (objMMDTO.ModuleDTOName == "AssetLabelDTO")
                BarcodeString = "#";
            else if (objMMDTO.ModuleDTOName == "ToolLabelDTO")
                BarcodeString = "#";
            else if (objMMDTO.ModuleDTOName == "TechnicianLabelDTO")
                BarcodeString = "#";
            else if (objMMDTO.ModuleDTOName == "LocationLabelDTO")
                BarcodeString = "@B";
            else
                BarcodeString = "#";


            lstReturns.Add(BarcodeString + strPrimaryBarcodeString);

            if (objLFMTDetailDTO.IncludeBin && isIncludeBinQty && !string.IsNullOrEmpty(strBin))
                lstReturns.Add("@" + strBin);

            if (objLFMTDetailDTO.IncludeQuantity && isIncludeBinQty)
            {
                if (objLFMTDetailDTO.QuantityField == "1")
                    lstReturns.Add("<1");
                else
                {
                    string strQty = GetPropertyValueFromObject(objLFMTDetailDTO.QuantityField, item);
                    string[] arrQty = strQty.Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    strQty = arrQty[0].Replace(",", "");
                    if (!string.IsNullOrEmpty(strQty))
                        lstReturns.Add("<" + strQty);
                }
            }

            return lstReturns;
        }


        /// <summary>
        /// Create List for Other than UK Barcode String 
        /// </summary>
        /// <param name="objLFMTDetailDTO"></param>
        /// <param name="objMMDTO"></param>
        /// <param name="item"></param>
        /// <param name="PrimaryBarcodeField"></param>
        /// <returns></returns>
        private List<string> GetListOfBarcodeStringForUKOnly(LabelFieldModuleTemplateDetailDTO objLFMTDetailDTO, LabelModuleMasterDTO objMMDTO, object item, string PrimaryBarcodeField)
        {
            List<string> lstReturns = new List<string>();
            string BarcodeString = string.Empty;
            bool isIncludeBinQty = true;

            string strPrimaryBarcodeString = GetPropertyValueFromObject(PrimaryBarcodeField, item);
            if (string.IsNullOrWhiteSpace(strPrimaryBarcodeString))
            {
                strPrimaryBarcodeString = GetPropertyValueFromObject("ItemUniqueNumber", item);
            }
            string strBin = GetPropertyValueFromObject("BinNumber", item);

            if (objMMDTO.ModuleDTOName == "InventoryLabelDTO")
            {
                string strItemType = GetPropertyValueFromObject("ItemType", item);
                if (!string.IsNullOrEmpty(strItemType) && strItemType == "4")
                    isIncludeBinQty = false;
            }

            lstReturns.Add(strPrimaryBarcodeString);

            if (objLFMTDetailDTO.IncludeBin && isIncludeBinQty && !string.IsNullOrEmpty(strBin))
                lstReturns.Add("+" + strBin.ToUpper());

            if (objLFMTDetailDTO.IncludeQuantity && isIncludeBinQty)
            {
                if (objLFMTDetailDTO.QuantityField == "1")
                    lstReturns.Add("+1");
                else
                {
                    string strQty = GetPropertyValueFromObject(objLFMTDetailDTO.QuantityField, item);
                    string[] arrQty = strQty.Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    lstReturns.Add("+" + arrQty[0].Replace(",", ""));
                }

            }

            if (objLFMTDetailDTO.BarcodeFont != "39" && objMMDTO.ModuleDTOName != "TechnicianLabelDTO")
            {
                lstReturns.Insert(0, "*");
                lstReturns.Add("*");
            }


            return lstReturns;
        }

        /// <summary>
        /// Create List for Other than UK Barcode String 
        /// </summary>
        /// <param name="objLFMTDetailDTO"></param>
        /// <param name="objMMDTO"></param>
        /// <param name="item"></param>
        /// <param name="PrimaryBarcodeField"></param>
        /// <returns></returns>
        private List<string> GetListOfBarcodeStringForUK01(LabelFieldModuleTemplateDetailDTO objLFMTDetailDTO, LabelModuleMasterDTO objMMDTO, object item, string PrimaryBarcodeField)
        {
            List<string> lstReturns = new List<string>();
            string strPrimaryBarcodeString = GetPropertyValueFromObject(PrimaryBarcodeField, item);

            if (string.IsNullOrWhiteSpace(strPrimaryBarcodeString))
            {
                strPrimaryBarcodeString = GetPropertyValueFromObject("ItemUniqueNumber", item);
            }
            

            lstReturns.Add(strPrimaryBarcodeString);

            //if (objLFMTDetailDTO.BarcodeFont != "39" && objMMDTO.ModuleDTOName != "TechnicianLabelDTO")
            //{
            //    lstReturns.Insert(0, "*");
            //    lstReturns.Add("*");
            //}


            return lstReturns;
        }

        /// <summary>
        /// Get Property Value From Object
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private string GetPropertyValueFromObject(string propertyName, object item)
        {
            PropertyInfo info = null;
            object value = null;
            string BarcodeString = string.Empty;
            info = item.GetType().GetProperty(propertyName);
            if (info != null)
            {
                value = info.GetValue(item, null);

                return Convert.ToString(value);
            }
            return "";
        }

        /// <summary>
        /// Concate String To Generate Barcode 
        /// </summary>
        /// <param name="arrStringForBarcode"></param>
        /// <returns></returns>
        private string ConcateBarcodeString(List<string> stringsForBarcode)
        {
            string strReturnString = string.Empty;
            if (stringsForBarcode.Count > 0)
            {
                foreach (var strItem in stringsForBarcode)
                {
                    strReturnString += strItem;
                }
            }
            return strReturnString;
        }

        private byte[] GetBarcodeNew(LabelFieldModuleTemplateDetailDTO objLFMTDetailDTO, LabelModuleMasterDTO objMMDTO, object item, LabelTemplateMasterDTO objSizeDTO, string strBarcode, int width, int Height, bool IncludeLabel, string strBarcodeType)
        {
            BarcodeLib.Barcode b = new BarcodeLib.Barcode();
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.Drawing.Image img = null;
            BarcodeLib.TYPE type = BarcodeLib.TYPE.CODE128B;

            try
            {
                b.Alignment = BarcodeLib.AlignmentPositions.LEFT;
                if (Convert.ToString(strBarcodeType).Trim().Equals("39"))
                    type = BarcodeLib.TYPE.CODE39Extended;
                else if (Convert.ToString(strBarcodeType).Contains("93"))
                    type = BarcodeLib.TYPE.CODE93;

                b.IncludeLabel = IncludeLabel;
                //b.RotateFlipType = (RotateFlipType)Enum.Parse(typeof(RotateFlipType), "RotateNoneFlipNone", true);
                b.LabelPosition = BarcodeLib.LabelPositions.BOTTOMLEFT;

                width = (int)ConvertPointToPixel((int)(width));
                Height = (int)ConvertPointToPixel((int)(Height));


                img = b.Encode(type, strBarcode, System.Drawing.Color.Black, System.Drawing.Color.White, width, Height);
                if (img != null)
                {
                    img.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                    return ms.ToArray();
                }
                return null;

            }
            catch (Exception ex)
            {
                string OptionalKeyWhenImageLarge = GetBarcodeStringNew(objLFMTDetailDTO, objMMDTO, item, true);

                if (!string.IsNullOrEmpty(OptionalKeyWhenImageLarge) && ex.Message.Contains("EGENERATE_IMAGE-2: Image size specified not large enough to draw image."))
                {
                    try
                    {
                        img = b.Encode(type, OptionalKeyWhenImageLarge, System.Drawing.Color.Black, System.Drawing.Color.White, width, Height);
                        if (img != null)
                        {
                            //TODO: updte format Gif to Png for issue WI-2665 "The Zebra barcodes are not very crisp and users are having trouble scanning because of this"
                            img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            return ms.ToArray();
                        }
                    }
                    catch (Exception exInner)
                    {
                        objLFMTDetailDTO.IncludeBin = false;
                        objLFMTDetailDTO.IncludeQuantity = false;
                        OptionalKeyWhenImageLarge = GetBarcodeStringNew(objLFMTDetailDTO, objMMDTO, item, true);

                        if (!string.IsNullOrEmpty(OptionalKeyWhenImageLarge) && exInner.Message.Contains("EGENERATE_IMAGE-2: Image size specified not large enough to draw image."))
                        {
                            try
                            {
                                //TODO: updte format Gif to Png for issue WI-2665 "The Zebra barcodes are not very crisp and users are having trouble scanning because of this"
                                img = b.Encode(type, OptionalKeyWhenImageLarge, System.Drawing.Color.Black, System.Drawing.Color.White, width, Height);
                                if (img != null)
                                {
                                    img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                    return ms.ToArray();
                                }
                            }
                            catch (Exception)
                            {
                                return null;
                            }

                        }
                        return null;
                    }

                }

                else if (!string.IsNullOrEmpty(OptionalKeyWhenImageLarge) && ex.Message.Contains("EENCODE-1: Input data not allowed to be blank."))
                {
                    try
                    {
                        img = b.Encode(type, OptionalKeyWhenImageLarge, System.Drawing.Color.Black, System.Drawing.Color.White, width, Height);
                        if (img != null)
                        {
                            img.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                            return ms.ToArray();
                        }
                    }
                    catch (Exception exInner)
                    {
                        objLFMTDetailDTO.IncludeBin = false;
                        objLFMTDetailDTO.IncludeQuantity = false;
                        OptionalKeyWhenImageLarge = GetBarcodeStringNew(objLFMTDetailDTO, objMMDTO, item, true);

                        if (!string.IsNullOrEmpty(OptionalKeyWhenImageLarge) && exInner.Message.Contains("EGENERATE_IMAGE-2: Image size specified not large enough to draw image."))
                        {
                            try
                            {
                                img = b.Encode(type, OptionalKeyWhenImageLarge, System.Drawing.Color.Black, System.Drawing.Color.White, width, Height);
                                if (img != null)
                                {
                                    img.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                                    return ms.ToArray();
                                }
                            }
                            catch (Exception)
                            {
                                return null;
                            }

                        }
                        return null;
                    }
                }

                return null;
                //throw ex;
            }
            finally
            {
                b.Dispose();
                ms.Dispose();
                if (img != null)
                    img.Dispose();
            }
        }

        private string StringEncode128(string text)
        {
            const int Ascii_FNC1 = 102;
            const int Ascii_STARTB = 104;

            int CharAscii = 0;
            for (int CharPos = 0; CharPos < text.Length; ++CharPos)
            {
                string letter = text.Substring(CharPos, 1);
                CharAscii = (int)Convert.ToChar(letter);
                if (CharAscii > 127 && CharAscii != Ascii_FNC1)
                {
                    throw new Exception(text + "|" + CharAscii);
                }
            }

            StringBuilder outstring = new StringBuilder();
            outstring.Append((char)(Ascii_STARTB));

            for (int CharPos = 0; CharPos < text.Length; ++CharPos)
            {
                string letter = text.Substring(CharPos, 1);
                CharAscii = (int)Convert.ToChar(letter) - 32;
                outstring.Append((char)(CharAscii));
            }

            return outstring.ToString();
        }
        private iTextSharp.text.Image GetBarcodeImageByType(string strBarcode, BarcodeLib.TYPE type, PdfWriter cb)
        {
            Image barcodeImage = null;
            if (type == BarcodeLib.TYPE.CODE128B)
            {
                Barcode128 code128 = new Barcode128();
                code128.CodeType = Barcode.CODE128_RAW;
                //code128.Code = strBarcode;
                code128.ChecksumText = true;
                code128.GenerateChecksum = true;
                code128.StartStopText = true;
                code128.Code = (StringEncode128(strBarcode) + "\uffff");

                barcodeImage = code128.CreateImageWithBarcode(cb.DirectContent, BaseColor.BLACK, BaseColor.WHITE);
            }
            else if (type == BarcodeLib.TYPE.CODE39Extended)
            {
                Barcode39 code39 = new Barcode39();
                code39.Code = strBarcode;
                code39.Extended = true;
                barcodeImage = code39.CreateImageWithBarcode(cb.DirectContent, BaseColor.BLACK, BaseColor.WHITE);
            }

            return barcodeImage;
        }

        private iTextSharp.text.Image GetBarcodeImage(LabelFieldModuleTemplateDetailDTO objLFMTDetailDTO, LabelModuleMasterDTO objMMDTO, object item, LabelTemplateMasterDTO objSizeDTO, string strBarcode, int width, int Height, bool IncludeLabel, string strBarcodeType, PdfWriter cb)
        {
            BarcodeLib.Barcode b = new BarcodeLib.Barcode();
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.Drawing.Image img = null;
            BarcodeLib.TYPE type = BarcodeLib.TYPE.CODE128B;
            int widthPixel = (int)ConvertPointToPixel((int)(width));
            int HeightPixel = (int)ConvertPointToPixel((int)(Height));

            try
            {
                b.Alignment = BarcodeLib.AlignmentPositions.LEFT;
                if (Convert.ToString(strBarcodeType).Trim().Equals("39"))
                    type = BarcodeLib.TYPE.CODE39Extended;
                else if (Convert.ToString(strBarcodeType).Contains("93"))
                    type = BarcodeLib.TYPE.CODE93;

                b.IncludeLabel = IncludeLabel;
                b.LabelPosition = BarcodeLib.LabelPositions.BOTTOMLEFT;
                img = b.Encode(type, strBarcode, System.Drawing.Color.Black, System.Drawing.Color.White, widthPixel, HeightPixel);

                if (img != null)
                {
                    Image barcodeImage = null;
                    if (Convert.ToString(strBarcodeType).Contains("93") || objSizeDTO.LabelType == 1)
                    {
                        img.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                        barcodeImage = iTextSharp.text.Image.GetInstance(ms.ToArray());
                    }
                    else
                    {
                        barcodeImage = GetBarcodeImageByType(strBarcode, type, cb);
                    }

                    return barcodeImage;
                }

                return null;

            }
            catch (Exception ex)
            {
                string OptionalKeyWhenImageLarge = GetBarcodeStringNew(objLFMTDetailDTO, objMMDTO, item, true);

                if (!string.IsNullOrEmpty(OptionalKeyWhenImageLarge) && ex.Message.Contains("EGENERATE_IMAGE-2: Image size specified not large enough to draw image."))
                {
                    try
                    {
                        img = b.Encode(type, OptionalKeyWhenImageLarge, System.Drawing.Color.Black, System.Drawing.Color.White, widthPixel, HeightPixel);
                        
                        if (img != null)
                        {
                            Image barcodeImage = null;
                            if (Convert.ToString(strBarcodeType).Contains("93") || objSizeDTO.LabelType == 1)
                            {
                                img.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                                barcodeImage = iTextSharp.text.Image.GetInstance(ms.ToArray());
                            }
                            else
                            {
                                barcodeImage = GetBarcodeImageByType(OptionalKeyWhenImageLarge, type, cb);
                            }

                            return barcodeImage;

                        }
                    }
                    catch (Exception exInner)
                    {
                        bool IsIncludeBin = objLFMTDetailDTO.IncludeBin;
                        bool IsIncludeQty = objLFMTDetailDTO.IncludeQuantity;
                        objLFMTDetailDTO.IncludeBin = false;
                        objLFMTDetailDTO.IncludeQuantity = false;
                        OptionalKeyWhenImageLarge = GetBarcodeStringNew(objLFMTDetailDTO, objMMDTO, item, true);
                        objLFMTDetailDTO.IncludeBin = IsIncludeBin;
                        objLFMTDetailDTO.IncludeQuantity = IsIncludeQty;

                        if (!string.IsNullOrEmpty(OptionalKeyWhenImageLarge) && exInner.Message.Contains("EGENERATE_IMAGE-2: Image size specified not large enough to draw image."))
                        {
                            try
                            {
                                img = b.Encode(type, OptionalKeyWhenImageLarge, System.Drawing.Color.Black, System.Drawing.Color.White, widthPixel, HeightPixel);
                                if (img != null)
                                {
                                    Image barcodeImage = null;
                                    if (Convert.ToString(strBarcodeType).Contains("93") || objSizeDTO.LabelType == 1)
                                    {
                                        img.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                                        barcodeImage = iTextSharp.text.Image.GetInstance(ms.ToArray());
                                    }
                                    else
                                    {
                                        barcodeImage = GetBarcodeImageByType(OptionalKeyWhenImageLarge, type, cb);
                                    }
                                    return barcodeImage;
                                }
                            }
                            catch (Exception)
                            {
                                return null;
                            }

                        }
                        return null;
                    }

                }

                else if (!string.IsNullOrEmpty(OptionalKeyWhenImageLarge) && ex.Message.Contains("EENCODE-1: Input data not allowed to be blank."))
                {
                    try
                    {
                        img = b.Encode(type, OptionalKeyWhenImageLarge, System.Drawing.Color.Black, System.Drawing.Color.White, widthPixel, HeightPixel);
                        if (img != null)
                        {
                            Image barcodeImage = null;
                            if (Convert.ToString(strBarcodeType).Contains("93") || objSizeDTO.LabelType == 1)
                            {
                                img.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                                barcodeImage = iTextSharp.text.Image.GetInstance(ms.ToArray());
                            }
                            else
                            {
                                barcodeImage = GetBarcodeImageByType(OptionalKeyWhenImageLarge, type, cb);
                            }

                            return barcodeImage;

                        }
                    }
                    catch (Exception exInner)
                    {
                        bool IsIncludeBin = objLFMTDetailDTO.IncludeBin;
                        bool IsIncludeQty = objLFMTDetailDTO.IncludeQuantity;
                        objLFMTDetailDTO.IncludeBin = false;
                        objLFMTDetailDTO.IncludeQuantity = false;
                        OptionalKeyWhenImageLarge = GetBarcodeStringNew(objLFMTDetailDTO, objMMDTO, item, true);
                        objLFMTDetailDTO.IncludeBin = IsIncludeBin;
                        objLFMTDetailDTO.IncludeQuantity = IsIncludeQty;

                        if (!string.IsNullOrEmpty(OptionalKeyWhenImageLarge) && exInner.Message.Contains("EGENERATE_IMAGE-2: Image size specified not large enough to draw image."))
                        {
                            try
                            {
                                img = b.Encode(type, OptionalKeyWhenImageLarge, System.Drawing.Color.Black, System.Drawing.Color.White, widthPixel, HeightPixel);
                                if (img != null)
                                {
                                    Image barcodeImage = null;
                                    if (Convert.ToString(strBarcodeType).Contains("93") || objSizeDTO.LabelType == 1)
                                    {
                                        img.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                                        barcodeImage = iTextSharp.text.Image.GetInstance(ms.ToArray());
                                    }
                                    else
                                    {
                                        barcodeImage = GetBarcodeImageByType(OptionalKeyWhenImageLarge, type, cb);
                                    }
                                    return barcodeImage;
                                }
                            }
                            catch (Exception)
                            {
                                return null;
                            }

                        }
                        return null;
                    }
                }

                return null;
                //throw ex;
            }
            finally
            {
                b.Dispose();
                ms.Dispose();
                if (img != null)
                    img.Dispose();
            }
        }


        private byte[] GetQRcode(LabelFieldModuleTemplateDetailDTO objLFMTDetailDTO, LabelModuleMasterDTO objMMDTO, object item, LabelTemplateMasterDTO objSizeDTO, string strBarcode, int scale)
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.Drawing.Image img = null;


            try
            {
                String encoding = "Byte";
                if (encoding == "Byte")
                {
                    qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
                }
                else if (encoding == "AlphaNumeric")
                {
                    qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.ALPHA_NUMERIC;
                }
                else if (encoding == "Numeric")
                {
                    qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.NUMERIC;
                }

                qrCodeEncoder.QRCodeScale = scale;


                int version = 7;
                qrCodeEncoder.QRCodeVersion = version;


                string errorCorrect = "M";
                if (errorCorrect == "L")
                    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.L;
                else if (errorCorrect == "M")
                    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
                else if (errorCorrect == "Q")
                    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.Q;
                else if (errorCorrect == "H")
                    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H;

                String data = strBarcode;
                img = qrCodeEncoder.Encode(data);

                if (img != null)
                {
                    //TODO: updte format Gif to Png for issue WI-2665 "The Zebra barcodes are not very crisp and users are having trouble scanning because of this"
                    img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return ms.ToArray();
                }
                return null;

            }
            catch
            {
                return null;
            }
            finally
            {
                qrCodeEncoder = null;
                ms.Dispose();
                if (img != null)
                    img.Dispose();
            }
        }



        /// <summary>
        /// GetUnFitBarcodeItems
        /// </summary>
        /// <param name="objLFMTDetailDTO"></param>
        /// <param name="objMMDTO"></param>
        /// <param name="item"></param>
        /// <param name="objSizeDTO"></param>
        /// <param name="strBarcode"></param>
        /// <param name="width"></param>
        /// <param name="Height"></param>
        /// <param name="IncludeLabel"></param>
        /// <param name="strBarcodeType"></param>
        /// <returns></returns>
        private string GetUnFitBarcodeItems(LabelFieldModuleTemplateDetailDTO objLFMTDetailDTO, LabelModuleMasterDTO objMMDTO, object item, LabelTemplateMasterDTO objSizeDTO, string strBarcode, int width, int Height, bool IncludeLabel, string strBarcodeType)
        {
            BarcodeLib.Barcode b = new BarcodeLib.Barcode();
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.Drawing.Image img = null;
            BarcodeLib.TYPE type = BarcodeLib.TYPE.CODE128B;

            try
            {
                b.Alignment = BarcodeLib.AlignmentPositions.LEFT;
                if (Convert.ToString(strBarcodeType).Trim().Equals("39"))
                    type = BarcodeLib.TYPE.CODE39Extended;
                else if (Convert.ToString(strBarcodeType).Contains("93"))
                    type = BarcodeLib.TYPE.CODE93;

                b.IncludeLabel = IncludeLabel;
                b.LabelPosition = BarcodeLib.LabelPositions.BOTTOMLEFT;
                width = (int)ConvertPointToPixel(width);
                Height = (int)ConvertPointToPixel(Height);

                img = b.Encode(type, strBarcode, System.Drawing.Color.Black, System.Drawing.Color.White, width, Height);

            }
            catch (Exception ex)
            {
                string OptionalKeyWhenImageLarge = GetBarcodeStringNew(objLFMTDetailDTO, objMMDTO, item, true);

                if (!string.IsNullOrEmpty(OptionalKeyWhenImageLarge) && ex.Message.Contains("EGENERATE_IMAGE-2: Image size specified not large enough to draw image."))
                {
                    try
                    {
                        img = b.Encode(type, OptionalKeyWhenImageLarge, System.Drawing.Color.Black, System.Drawing.Color.White, width, Height);
                        if (img != null)
                            return string.Empty;
                    }
                    catch (Exception exInner)
                    {
                        if (exInner.Message.Contains("EGENERATE_IMAGE-2: Image size specified not large enough to draw image."))
                        {
                            return strBarcode;
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

            return string.Empty;
        }


        /// <summary>
        /// GetModuleWiseObjecList
        /// </summary>
        /// <param name="objMMDTO"></param>
        /// <param name="IDs"></param>
        /// <returns></returns>
        public List<object> GetModuleObjectListForUnfit(LabelModuleMasterDTO objMMDTO, LabelFieldModuleTemplateDetailDTO objLFMTDetailDTO)
        {
            List<object> objectList = null;

            if (objMMDTO.ModuleDTOName == "AssetLabelDTO")
            {

                AssetMasterDAL objAssetDAL = new AssetMasterDAL(SessionHelper.EnterPriseDBName);
                IEnumerable<AssetMasterDTO> lstAssetDTO = objAssetDAL.GetAllAssetsByRoom(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                lstAssetDTO = lstAssetDTO.OrderBy(x => x.AssetName);
                List<AssetLabelDTO> objLableDTOList = new List<AssetLabelDTO>();
                foreach (var item in lstAssetDTO)
                {
                    objLableDTOList.Add(GetAssetLabelDTO(item));
                }
                objectList = objLableDTOList.Cast<object>().ToList();
            }
            else if (objMMDTO.ModuleDTOName == "InventoryLabelDTO")
            {
                ItemMasterDAL objDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                IEnumerable<ItemMasterDTO> lstMasterDTO = objDAL.GetActiveItemsLimitedFieldsByRoom(SessionHelper.RoomID, SessionHelper.CompanyID);
                List<ItemMasterDTO> lstItems = lstMasterDTO.ToList<ItemMasterDTO>();
                BinMasterDAL objBinDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                ItemLocationQTYDAL objAPI = new ItemLocationQTYDAL(SessionHelper.EnterPriseDBName);
                List<ItemMasterDTO> ItemListBin = new List<ItemMasterDTO>();

                foreach (var item in lstItems)
                {
                    List<ItemLocationQTYDTO> lstItemLocationQty = objAPI.GetBinsByItemDB(item.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, "BinNumber Asc", false);
                    for (int i = 0; i < lstItemLocationQty.Count; i++)
                    {
                        if (i == 0)
                        {
                            item.BinID = lstItemLocationQty[i].BinID;
                            item.BinNumber = lstItemLocationQty[i].BinNumber;
                        }
                        else
                        {
                            BinMasterDTO objBinDTO = objBinDAL.GetBinByID(lstItemLocationQty[i].BinID, SessionHelper.RoomID, SessionHelper.CompanyID);
                            //BinMasterDTO objBinDTO = objBinDAL.GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false,Guid.Empty, lstItemLocationQty[i].BinID, null,null).FirstOrDefault();
                            ItemMasterDTO obj = GetItemMasterDTOFromBinDTO(objBinDTO, item);
                            ItemListBin.Add(obj);
                        }
                    }
                }

                foreach (var item in ItemListBin)
                {
                    lstItems.Add(item);
                }

                lstMasterDTO = lstItems.OrderBy(x => x.ItemNumber);

                List<InventoryLabelDTO> objLableDTOList = new List<InventoryLabelDTO>();
                foreach (var item in lstMasterDTO)
                {
                    if (!string.IsNullOrEmpty(item.BinNumber))
                        item.BinNumber = item.BinNumber.Replace("[|EmptyStagingBin|]", "");
                    objLableDTOList.Add(GetInvertoryLabelDTO(item));
                }

                objectList = objLableDTOList.Cast<object>().ToList();

            }
            else if (objMMDTO.ModuleDTOName == "KittingLabelDTO")
            {
                KitMasterDAL objDAL = new KitMasterDAL(SessionHelper.EnterPriseDBName);
                TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
                IEnumerable<KitMasterDTO> lstMasterDTO = objDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, SessionHelper.UserSupplierIds, CurrentTimeZone);
                lstMasterDTO = lstMasterDTO.OrderBy(x => x.KitPartNumber);

                List<KittingLabelDTO> objLableDTOList = new List<KittingLabelDTO>();
                foreach (var item in lstMasterDTO)
                {
                    objLableDTOList.Add(GetKittingLabelDTO(item));
                }
                objectList = objLableDTOList.Cast<object>().ToList();
            }
            else if (objMMDTO.ModuleDTOName == "OrderLabelDTO")
            {
                OrderMasterDAL objDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
                IEnumerable<OrderMasterDTO> lstMasterDTO = objDAL.GetAllOrdersByRoomNormal(SessionHelper.RoomID, SessionHelper.CompanyID);
                lstMasterDTO = lstMasterDTO.OrderBy(x => x.OrderNumber);

                List<OrderLabelDTO> objLableDTOList = new List<OrderLabelDTO>();
                foreach (var item in lstMasterDTO)
                {
                    List<OrderLabelDTO> lstOrdLblDTO = GetOrderLabelDTO(item);
                    foreach (var lblDTO in lstOrdLblDTO)
                    {
                        if (!string.IsNullOrEmpty(lblDTO.BinNumber))
                            lblDTO.BinNumber = lblDTO.BinNumber.Replace("[|EmptyStagingBin|]", "");
                        objLableDTOList.Add(lblDTO);
                    }
                }
                objectList = objLableDTOList.Cast<object>().ToList();
            }
            else if (objMMDTO.ModuleDTOName == "QuickListLabelDTO")
            {
                QuickListDAL objDAL = new QuickListDAL(SessionHelper.EnterPriseDBName);
                IEnumerable<QuickListMasterDTO> lstMasterDTO = objDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                lstMasterDTO = lstMasterDTO.OrderBy(x => x.Name);
                List<QuickListLabelDTO> objLableDTOList = new List<QuickListLabelDTO>();
                foreach (var item in lstMasterDTO)
                {
                    objLableDTOList.Add(GetQuickListLabelDTO(item));
                }
                objectList = objLableDTOList.Cast<object>().ToList();

            }
            else if (objMMDTO.ModuleDTOName == "ReceiveLabelDTO")
            {
                int totlCnt = 0;
                ReceivedOrderTransferDetailDAL objROTDDAL = new ReceivedOrderTransferDetailDAL(SessionHelper.EnterPriseDBName);
                ReceiveOrderDetailsDAL objDAL = new ReceiveOrderDetailsDAL(SessionHelper.EnterPriseDBName);
                IEnumerable<ReceivableItemDTO> lstDetailDTO = null;

                List<ReceiveLabelDTO> objLableDTOList = new List<ReceiveLabelDTO>();
                var tmpsupplierIds = new List<long>();
                string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
                TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
                lstDetailDTO = objDAL.GetALLReceiveListByPaging(0, int.MaxValue, out totlCnt, "", "ItemNumber ASC", SessionHelper.RoomID, SessionHelper.CompanyID, false, false, "4,5,6,7,8", tmpsupplierIds, RoomDateFormat, CurrentTimeZone);
                lstDetailDTO = lstDetailDTO.OrderBy(x => x.ItemNumber);
                foreach (var item in lstDetailDTO)
                {
                    if (!string.IsNullOrEmpty(item.ReceiveBinName))
                        item.ReceiveBinName = item.ReceiveBinName.Replace("[|EmptyStagingBin|]", "");
                    objLableDTOList.Add(GetReceiveLabelDTO(item));
                }

                objectList = objLableDTOList.Cast<object>().ToList();
            }
            else if (objMMDTO.ModuleDTOName == "StagingLabelDTO")
            {
                MaterialStagingDAL objDAL = new MaterialStagingDAL(SessionHelper.EnterPriseDBName);
                //IEnumerable<MaterialStagingDTO> lstMasterDTO = objDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                IEnumerable<MaterialStagingDTO> lstMasterDTO = objDAL.GetMaterialStaging(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, string.Empty, null);
                List<StagingLabelDTO> objLableDTOList = new List<StagingLabelDTO>();
                foreach (var item in lstMasterDTO)
                {
                    List<StagingLabelDTO> lstOrdLblDTO = GetStagingLabelDTO(item);

                    foreach (var lblDTO in lstOrdLblDTO)
                    {
                        if (!string.IsNullOrEmpty(lblDTO.BinNumber))
                            lblDTO.BinNumber = lblDTO.BinNumber.Replace("[|EmptyStagingBin|]", "");
                        objLableDTOList.Add(lblDTO);
                    }
                }

                objLableDTOList = objLableDTOList.OrderBy(x => x.ItemNumber).ToList();

                objectList = objLableDTOList.Cast<object>().ToList();
            }
            else if (objMMDTO.ModuleDTOName == "ToolLabelDTO")
            {
                ToolMasterDAL objToolDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
                IEnumerable<ToolMasterDTO> lstToolDTO = objToolDAL.GetToolByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(x => x.ToolName);
                List<ToolLabelDTO> objLableDTOList = new List<ToolLabelDTO>();
                foreach (var item in lstToolDTO)
                {
                    objLableDTOList.Add(GetToolLabelDTO(item));
                }
                objectList = objLableDTOList.Cast<object>().ToList();
            }
            else if (objMMDTO.ModuleDTOName == "TransferLabelDTO")
            {
                TransferMasterDAL objDAL = new TransferMasterDAL(SessionHelper.EnterPriseDBName);
                var lstMasterDTO = objDAL.GetAllTransfersByRoomNormal(SessionHelper.RoomID, SessionHelper.CompanyID);
                List<TransferLabelDTO> objLableDTOList = new List<TransferLabelDTO>();

                foreach (var item in lstMasterDTO)
                {
                    List<TransferLabelDTO> lstOrdLblDTO = GetTransferLabelDTO(item);

                    foreach (var lblDTO in lstOrdLblDTO)
                    {
                        if (!string.IsNullOrEmpty(lblDTO.BinNumber))
                            lblDTO.BinNumber = lblDTO.BinNumber.Replace("[|EmptyStagingBin|]", "");
                        objLableDTOList.Add(lblDTO);
                    }
                }

                objLableDTOList = objLableDTOList.OrderBy(x => x.ItemNumber).ToList();
                objectList = objLableDTOList.Cast<object>().ToList();

            }
            else if (objMMDTO.ModuleDTOName == "TechnicianLabelDTO")
            {
                TechnicialMasterDAL objDAL = new TechnicialMasterDAL(SessionHelper.EnterPriseDBName);
                IEnumerable<TechnicianMasterDTO> lstMasterDTO = objDAL.GetTechnicianByRoomIDPlain(SessionHelper.RoomID, SessionHelper.CompanyID);
                List<TechnicianLabelDTO> objLableDTOList = new List<TechnicianLabelDTO>();
                foreach (var item in lstMasterDTO)
                {
                    TechnicianLabelDTO lstOrdLblDTO = GetTechnicianLabelDTO(item);
                    objLableDTOList.Add(lstOrdLblDTO);
                }
                objLableDTOList = objLableDTOList.OrderBy(x => x.TechnicianName).ToList();
                objectList = objLableDTOList.Cast<object>().ToList();

            }

            return objectList;
        }

        /// <summary>
        /// Get List Un-fit Barcode Items 
        /// which are not fit into the label width
        /// </summary>
        /// <param name="objLabelDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetListUnfitBarcodeItems(LabelFieldModuleTemplateDetailDTO objLabelDetail)
        {
            List<string> lstBarcodeString = null;
            LabelModuleTemplateDetailDAL objMTDetailDAL = null;
            LabelTemplateMasterDAL objTMDAL = null;
            LabelModuleTemplateDetailDTO objMTDetailDTO = null;
            LabelTemplateMasterDTO objTemplateDTO = null;
            LabelModuleMasterDAL objMMDAL = null;
            LabelModuleMasterDTO objMMDTO = null;
            LabelModuleFieldMasterDAL objMFMDAL = null;

            try
            {
                lstBarcodeString = new List<string>();
                objMTDetailDAL = new LabelModuleTemplateDetailDAL(SessionHelper.EnterPriseDBName);
                objTMDAL = new LabelTemplateMasterDAL(SessionHelper.EnterPriseDBName);
                objMMDAL = new LabelModuleMasterDAL(SessionHelper.EnterPriseDBName);
                objMFMDAL = new LabelModuleFieldMasterDAL(SessionHelper.EnterPriseDBName);

                //objMTDetailDTO = objMTDetailDAL.GetAllRecords(SessionHelper.CompanyID, SessionHelper.RoomID).FirstOrDefault(x => x.ModuleID == objLabelDetail.ModuleID && x.CompanyID == SessionHelper.CompanyID && x.RoomID == SessionHelper.RoomID);
                objMTDetailDTO = objMTDetailDAL.GetLabelModuleTemplateDetailByRoomCompanyModuleID(SessionHelper.CompanyID, SessionHelper.RoomID, objLabelDetail.ModuleID).FirstOrDefault();
                //objTemplateDTO = objTMDAL.GetRecord(objLabelDetail.TemplateID, SessionHelper.CompanyID);
                objTemplateDTO = objTMDAL.GetLabelTemplateMasterByCompanyTemplateID(objLabelDetail.TemplateID, SessionHelper.CompanyID);
                //objMMDTO = objMMDAL.GetRecord(objLabelDetail.ModuleID);
                objMMDTO = objMMDAL.GetLabelModuleMasterByID(objLabelDetail.ModuleID);

                long[] arrIDs = new long[1];
                arrIDs[0] = 0;
                long SessionUserId = SessionHelper.UserID;
                List<object> lstObjs = GetModuleObjectListForUnfit(objMMDTO, objLabelDetail);
                if (lstObjs != null & lstObjs.Count > 0)
                {
                    float fwidth = ConvertInchToPoint(objTemplateDTO.LabelWidth);
                    float leftrightpading = ConvertInchToPoint(objTemplateDTO.LabelPaddingLeft) + ConvertInchToPoint(objTemplateDTO.LabelPaddingRight);
                    int intWidth = (int)(fwidth - leftrightpading);

                    foreach (object item in lstObjs)
                    {
                        string optionBarcodeKey = "";
                        string uniqueIdentifier = GetPropertyValueFromObject("ItemUniqueNumber", item);
                        if (string.IsNullOrEmpty(uniqueIdentifier) &&
                            (objMMDTO.ModuleDTOName == "InventoryLabelDTO"
                                || objMMDTO.ModuleDTOName == "OrderLabelDTO"
                                || objMMDTO.ModuleDTOName == "ReceiveLabelDTO"
                                || objMMDTO.ModuleDTOName == "StagingLabelDTO"
                                || objMMDTO.ModuleDTOName == "TransferLabelDTO"))
                        {
                            string strItemID = GetPropertyValueFromObject("ID", item);
                            Int64 itemID = 0;
                            Int64.TryParse(strItemID, out itemID);
                            if (itemID > 0)
                            {
                                ItemMasterDAL objItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                                ItemMasterDTO objItemDTO = objItemDAL.GetItemWithoutJoins(itemID, null);
                                objItemDAL.Edit(objItemDTO, SessionUserId, SessionHelper.EnterPriceID);
                                objItemDTO = objItemDAL.GetItemWithoutJoins(itemID, null);
                                uniqueIdentifier = objItemDTO.ItemUniqueNumber;
                                PropertyInfo info = item.GetType().GetProperty("ItemUniqueNumber");
                                info.SetValue(item, uniqueIdentifier, null);
                            }
                        }
                        objLabelDetail.lstModuleFields = objMFMDAL.GetRecordsModueWise(objLabelDetail.ModuleID, SessionHelper.CompanyID, true).ToList();

                        string BarcodeString = GetBarcodeStringNew(objLabelDetail, objMMDTO, item, false);

                        if (string.IsNullOrEmpty(BarcodeString) || string.IsNullOrWhiteSpace(BarcodeString))
                            BarcodeString = optionBarcodeKey;

                        string strunfitItem = GetUnFitBarcodeItems(objLabelDetail, objMMDTO, item, objTemplateDTO, BarcodeString, intWidth, (int)(30), false, objLabelDetail.BarcodeFont);
                        if (!string.IsNullOrEmpty(strunfitItem))
                            lstBarcodeString.Add(strunfitItem);
                    }
                }

                return Json(new { Message = "Success", Status = true, Items = lstBarcodeString }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message, Status = false, Items = lstBarcodeString }, JsonRequestBehavior.AllowGet);
            }

            finally
            {
                objMTDetailDAL = null;

                objTMDAL = null;
                objMTDetailDTO = null;

                objTemplateDTO = null;
                objMMDAL = null;

                objMMDTO = null;
            }

        }


        #endregion



        /// <summary>
        /// GetStringForCell
        /// </summary>
        /// <param name="objLFMTDetailDTO"></param>
        /// <param name="child"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private string GetStringForCell(LabelFieldModuleTemplateDetailDTO objLFMTDetailDTO, XmlElement child, object item)
        {
            LabelModuleFieldMasterDTO fl = null;
            Int64 FldID = 0;
            PropertyInfo info = null;
            object value = null;

            string fldid = Convert.ToString(child.GetAttribute("FieldID"));
            Int64.TryParse(fldid, out FldID);
            if (FldID > 0)
                fl = objLFMTDetailDTO.lstSelectedModuleFields.FirstOrDefault(x => x.ID == FldID);


            if (fl != null && fl.ID == FldID)
            {
                info = item.GetType().GetProperty(fl.FieldName);
                if (info != null)
                    value = info.GetValue(item, null);

                if (value == null)
                    value = "";

                return fl.FieldDisplayName + ": " + Convert.ToString(value.ToString());
            }
            else
            {
                LabelModuleFieldMasterDAL lblMdlfldMstDAL = new LabelModuleFieldMasterDAL(SessionHelper.EnterPriseDBName);
                fl = lblMdlfldMstDAL.GetRecordsModueWise(objLFMTDetailDTO.ModuleID, objLFMTDetailDTO.CompanyID, true).FirstOrDefault(x => x.ID == FldID);
                if (fl != null && fl.ID == FldID)
                {
                    info = item.GetType().GetProperty(fl.FieldName);
                    if (info != null)
                        value = info.GetValue(item, null);

                    if (value == null)
                        value = "";

                    return Convert.ToString(value.ToString());
                }
            }

            return string.Empty;
        }

        private List<object> GetObjectListForAssetModule(Int64[] IDs, string SortField)
        {
            AssetMasterDAL objAssetDAL = null;
            IEnumerable<AssetMasterDTO> lstAssetDTO = null;
            List<AssetLabelDTO> objLableDTOList = null;
            List<object> objectList = null;
            try
            {
                objAssetDAL = new AssetMasterDAL(SessionHelper.EnterPriseDBName);
                lstAssetDTO = objAssetDAL.GetAllAssetsByRoom(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                if (!(IDs.Length == 1 && IDs[0] == 0))
                    lstAssetDTO = lstAssetDTO.Where(x => IDs.Contains(x.ID));

                /*
                if (!string.IsNullOrEmpty(SortField))
                    lstAssetDTO = lstAssetDTO.OrderBy(SortField);
                    */

                List<AssetMasterDTO> finalList = new List<AssetMasterDTO>();
                for (int i = 0; i < IDs.Length; i++)
                {
                    var vList = lstAssetDTO.Where(x => x.ID == IDs[i]).FirstOrDefault();
                    if (vList != null)
                    {
                        finalList.Add(vList);
                    }
                }

                objLableDTOList = new List<AssetLabelDTO>();
                foreach (var item in finalList)
                {
                    objLableDTOList.Add(GetAssetLabelDTO(item));
                }
                objectList = objLableDTOList.Cast<object>().ToList();
                return objectList;
            }
            finally
            {
                objAssetDAL = null;
                lstAssetDTO = null;
                objLableDTOList = null;
            }
        }

        private List<object> GetObjectListForItemModule(Int64[] IDs, string SortField)
        {
            List<object> objectList = null;
            ItemMasterDAL objDAL = null;
            IEnumerable<ItemMasterDTO> lstMasterDTO = null;
            List<ItemMasterDTO> lstItems = null;
            List<ItemMasterDTO> lstItemsTemp = null;
            Dictionary<string, List<string>> dicItemsBin = null;
            BinMasterDAL objBinDAL = null;
            ItemLocationQTYDAL objAPI = null;
            List<ItemMasterDTO> ItemListBin = null;
            ItemMasterDTO obj = null;
            List<InventoryLabelDTO> objLableDTOList = null;
            try
            {

                objDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                if (!(IDs.Length == 1 && IDs[0] == 0))
                {
                    lstMasterDTO = objDAL.GetAllItemsWithJoinsByIDs(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, string.Join(",", IDs));
                }
                else
                {
                    lstMasterDTO = objDAL.GetAllItemsWithJoins(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, null);
                }
                dicItemsBin = new Dictionary<string, List<string>>();

                if (!(IDs.Length == 1 && IDs[0] == 0))
                {
                    lstItems = lstMasterDTO.Where(x => IDs.Contains(x.ID)).ToList();
                }
                objBinDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                objAPI = new ItemLocationQTYDAL(SessionHelper.EnterPriseDBName);
                ItemListBin = new List<ItemMasterDTO>();
                foreach (var item in lstItems)
                {
                    //List<ItemLocationQTYDTO> lstItemLocationQty = objAPI.GetBinsByItemDB(item.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, "BinNumber Asc", false);
                    List<BinMasterDTO> lstItemLocationQty = objBinDAL.GetItemLocations(item.GUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (lstItemLocationQty != null && lstItemLocationQty.Count > 0)
                    {
                        lstItemLocationQty = lstItemLocationQty.Where(x => x.IsStagingLocation == false).ToList();
                    }
                    string ItemImageExternalURL = item.ItemImageExternalURL;
                    string ImagePath = item.ImagePath;
                    string ImageType = item.ImageType;
                    for (int i = 0; i < lstItemLocationQty.Count; i++)
                    {
                        if (item.DefaultLocation.GetValueOrDefault(0) == lstItemLocationQty[i].ID)
                        {
                            /*item.BinID = lstItemLocationQty[i].BinID;*/
                            item.BinID = lstItemLocationQty[i].ID;
                            item.BinNumber = lstItemLocationQty[i].BinNumber;

                            if (lstItemLocationQty[i].DefaultReorderQuantity.HasValue && lstItemLocationQty[i].DefaultReorderQuantity.GetValueOrDefault(0) > 0)
                            {
                                item.BinDefaultReorderQuantity = lstItemLocationQty[i].DefaultReorderQuantity.GetValueOrDefault(0);
                            }
                            else
                            {
                                item.BinDefaultReorderQuantity = item.DefaultReorderQuantity.GetValueOrDefault(0);
                            }

                            if (lstItemLocationQty[i].DefaultPullQuantity.HasValue && lstItemLocationQty[i].DefaultPullQuantity.GetValueOrDefault(0) > 0)
                            {
                                item.BinDefaultPullQuantity = lstItemLocationQty[i].DefaultPullQuantity.GetValueOrDefault(0);
                            }
                            else
                            {
                                item.BinDefaultPullQuantity = item.DefaultReorderQuantity.GetValueOrDefault(0);
                            }

                            //item.BinDefaultReorderQuantity = lstItemLocationQty[i].DefaultReorderQuantity.GetValueOrDefault(0);
                            //item.BinDefaultPullQuantity = lstItemLocationQty[i].DefaultPullQuantity.GetValueOrDefault(0);
                            
                            if (!item.IsItemLevelMinMaxQtyRequired.GetValueOrDefault(false))
                            {
                                /*
                                objBinDTO = objBinDAL.GetRecord(lstItemLocationQty[i].BinID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                                //objBinDTO = objBinDAL.GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false,Guid.Empty, lstItemLocationQty[i].BinID, null,null).FirstOrDefault();
                                item.CriticalQuantity = objBinDTO.CriticalQuantity.GetValueOrDefault(0);
                                item.MinimumQuantity = objBinDTO.MinimumQuantity.GetValueOrDefault(0);
                                item.MaximumQuantity = objBinDTO.MaximumQuantity.GetValueOrDefault(0);
                                */

                                item.CriticalQuantity = lstItemLocationQty[i].CriticalQuantity.GetValueOrDefault(0);
                                item.MinimumQuantity = lstItemLocationQty[i].MinimumQuantity.GetValueOrDefault(0);
                                item.MaximumQuantity = lstItemLocationQty[i].MaximumQuantity.GetValueOrDefault(0);



                            }
                        }
                        else
                        {
                            /*
                            objBinDTO = objBinDAL.GetRecord(lstItemLocationQty[i].BinID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                            //objBinDTO = objBinDAL.GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false,Guid.Empty, lstItemLocationQty[i].BinID, null,null).FirstOrDefault();
                            obj = GetItemMasterDTOFromBinDTO(objBinDTO, item);
                            */

                            //objBinDTO = objBinDAL.GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false,Guid.Empty, lstItemLocationQty[i].BinID, null,null).FirstOrDefault();
                            obj = GetItemMasterDTOFromBinDTO(lstItemLocationQty[i], item);

                            if (!(obj.BinDefaultReorderQuantity.HasValue && obj.BinDefaultReorderQuantity.GetValueOrDefault(0) > 0))
                            {
                                obj.BinDefaultReorderQuantity = item.DefaultReorderQuantity.GetValueOrDefault(0);
                            }

                            if (!(obj.BinDefaultPullQuantity.HasValue && obj.BinDefaultPullQuantity.GetValueOrDefault(0) > 0))
                            {
                                obj.BinDefaultPullQuantity = item.DefaultPullQuantity.GetValueOrDefault(0);
                            }
                            obj.ImageType = ImageType;
                            obj.ItemImageExternalURL = ItemImageExternalURL;
                            obj.ImagePath = ImagePath;
                            ItemListBin.Add(obj);
                        }
                    }
                }

                //foreach (var item in ItemListBin)
                //{
                //    lstItems.Add(item);
                //}
                lstItems.AddRange(ItemListBin);


                if (Session["NSItemLocation"] != null && !string.IsNullOrWhiteSpace(Convert.ToString(Session["NSItemLocation"])))
                {
                    string itemGUIDs = string.Join(",", lstItems.Select(x => x.GUID.ToString()));
                    List<BinMasterDTO> itemBinFromParent = objBinDAL.GetBinMasterFromParentID(Convert.ToString(Session["NSItemLocation"]), itemGUIDs);

                    lstItemsTemp = new List<ItemMasterDTO>();
                    foreach (var item in lstItems)
                    {
                        var isItemAndBin = itemBinFromParent.Where(x => x.ID == item.BinID.GetValueOrDefault(0)).ToList();
                        if (isItemAndBin != null && isItemAndBin.Count > 0)
                        {
                            lstItemsTemp.Add(item);
                        }
                    }

                    if (lstItemsTemp.Count > 0)
                    {
                        lstItems = new List<ItemMasterDTO>();
                        foreach (var item in lstItemsTemp)
                        {
                            lstItems.Add(item);
                        }
                    }

                }

                lstMasterDTO = lstItems;
                /*
                if (!string.IsNullOrEmpty(SortField) && !SortField.ToLower().Contains("null"))
                {
                    lstMasterDTO = lstItems.OrderBy(SortField);
                }
                */

                List<ItemMasterDTO> finalList = new List<ItemMasterDTO>();
                for (int i = 0; i < IDs.Length; i++)
                {
                    var vItems = lstMasterDTO.Where(x => x.ID == IDs[i]).ToList();
                    if (vItems != null && vItems.Count > 0)
                    {
                        finalList.AddRange(vItems);
                        //foreach (ItemMasterDTO item in vItems)
                        //{
                        //    finalList.Add(item);
                        //}
                    }
                }
                finalList.FindAll(x => x.BinNumber == "[|EmptyStagingBin|]").ForEach(x => x.BinNumber = "");
                objLableDTOList = new List<InventoryLabelDTO>();
                MaterialStagingDetailDAL objMSDAL = new MaterialStagingDetailDAL(SessionHelper.EnterPriseDBName);
                List<MaterialStagingDetailDTO> objStagingDTO = null;

                var SupplierIDs = finalList.Select(x => x.SupplierID).Distinct().ToArray();
                SupplierMasterDAL supplierDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
                string strSupplierIDs = string.Join(",", SupplierIDs);
                var AllSupplierslist = supplierDAL.GetSupplierByIDsPlain(strSupplierIDs, SessionHelper.RoomID, SessionHelper.CompanyID);

                foreach (var item in finalList)
                {
                    //if (!string.IsNullOrEmpty(item.BinNumber))
                    //    item.BinNumber = item.BinNumber.Replace("[|EmptyStagingBin|]", "");
                    objStagingDTO = objMSDAL.GetStagLimitedInfo(item.GUID, item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0));
                    objLableDTOList.Add(GetInvertoryLabelDTOLimited(item, objStagingDTO, AllSupplierslist));
                }
                objectList = objLableDTOList.Cast<object>().ToList();
                return objectList;

            }
            finally
            {
                objDAL = null;
                lstMasterDTO = null;
                lstItems = null;
                lstItemsTemp = null;
                dicItemsBin = null;
                objBinDAL = null;
                objAPI = null;
                ItemListBin = null;
                obj = null;
                objLableDTOList = null;

            }
        }
        private List<object> GetObjectListForItemModule_Bins(Int64[] IDs, string SortField, string ROTDIds)
        {
            List<object> objectList = null;
            ItemMasterDAL objDAL = null;
            IEnumerable<ItemMasterDTO> lstMasterDTO = null;
            List<ItemMasterDTO> lstItems = null;
            List<ItemMasterDTO> lstItemsTemp = null;
            Dictionary<string, List<string>> dicItemsBin = null;

            BinMasterDAL objBinDAL = null;
            ItemLocationQTYDAL objAPI = null;
            List<ItemMasterDTO> ItemListBin = null;
            List<ItemsBins> lstItemLocationQty = null;
            BinMasterDTO objBinDTO = null;
            ItemMasterDTO obj = null;
            List<InventoryLabelDTO> objLableDTOList = null;
            try
            {
                objDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                if (!(IDs.Length == 1 && IDs[0] == 0))
                {
                    string ItemIDs = String.Join(",", IDs);
                    lstMasterDTO = objDAL.GetAllItemsWithJoinsByIDs(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ItemIDs);
                }
                else
                {
                    lstMasterDTO = objDAL.GetAllItemsWithJoins(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, null);
                }
                dicItemsBin = new Dictionary<string, List<string>>();

                if (!(IDs.Length == 1 && IDs[0] == 0))
                {
                    lstItems = lstMasterDTO.Where(x => IDs.Contains(x.ID)).ToList();

                }

                string[] strItemBin = ROTDIds.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var stritem in strItemBin)
                {
                    string[] itembinid = stritem.Split(new char[1] { '#' }, StringSplitOptions.RemoveEmptyEntries);
                    if (dicItemsBin.Keys.Contains(itembinid[0]))
                    {
                        List<string> lstBins = dicItemsBin[itembinid[0]];
                        if (lstBins != null)
                            lstBins.Add(itembinid[1]);
                        dicItemsBin[itembinid[0]] = lstBins;
                    }
                    else
                    {
                        List<string> lstBins = new List<string>();
                        lstBins.Add(itembinid[1]);
                        dicItemsBin.Add(itembinid[0], lstBins);
                    }
                }

                objBinDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                objAPI = new ItemLocationQTYDAL(SessionHelper.EnterPriseDBName);
                ItemListBin = new List<ItemMasterDTO>();

                foreach (var item in lstItems)
                {
                    lstItemLocationQty = objBinDAL.GetItemsBinForLabels(item.GUID.ToString());
                    for (int i = 0; i < lstItemLocationQty.Count; i++)
                    {
                        if (i == 0)
                        {
                            item.BinID = lstItemLocationQty[i].BinID;
                            item.BinNumber = lstItemLocationQty[i].BinNumber;
                            objBinDTO = objBinDAL.GetBinByID(lstItemLocationQty[i].BinID, SessionHelper.RoomID, SessionHelper.CompanyID);
                            
                            if (objBinDTO.DefaultReorderQuantity.HasValue && objBinDTO.DefaultReorderQuantity.GetValueOrDefault(0) > 0)
                            {
                                item.BinDefaultReorderQuantity = objBinDTO.DefaultReorderQuantity.GetValueOrDefault(0);
                            }
                            else
                            {
                                item.BinDefaultReorderQuantity = item.DefaultReorderQuantity.GetValueOrDefault(0);
                            }

                            if (objBinDTO.DefaultPullQuantity.HasValue && objBinDTO.DefaultPullQuantity.GetValueOrDefault(0) > 0)
                            {
                                item.BinDefaultPullQuantity = objBinDTO.DefaultPullQuantity.GetValueOrDefault(0);
                            }
                            else
                            {
                                item.BinDefaultPullQuantity = item.DefaultReorderQuantity.GetValueOrDefault(0);
                            }

                            if (!item.IsItemLevelMinMaxQtyRequired.GetValueOrDefault(false))
                            {
                                //objBinDTO = objBinDAL.GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false,Guid.Empty, lstItemLocationQty[i].BinID, null,null).FirstOrDefault();
                                item.CriticalQuantity = objBinDTO.CriticalQuantity.GetValueOrDefault(0);
                                item.MinimumQuantity = objBinDTO.MinimumQuantity.GetValueOrDefault(0);
                                item.MaximumQuantity = objBinDTO.MaximumQuantity.GetValueOrDefault(0);
                                
                                
                            }
                        }
                        else
                        {
                            objBinDTO = objBinDAL.GetBinByID(lstItemLocationQty[i].BinID, SessionHelper.RoomID, SessionHelper.CompanyID);
                            //objBinDTO = objBinDAL.GetItemLocation( SessionHelper.RoomID, SessionHelper.CompanyID, false, false,Guid.Empty, lstItemLocationQty[i].BinID,null,null).FirstOrDefault();
                            obj = GetItemMasterDTOFromBinDTO(objBinDTO, item);
                            
                            if (objBinDTO.DefaultReorderQuantity.HasValue && objBinDTO.DefaultReorderQuantity.GetValueOrDefault(0) > 0)
                            {
                                obj.BinDefaultReorderQuantity = objBinDTO.DefaultReorderQuantity.GetValueOrDefault(0);
                            }
                            else
                            {
                                obj.BinDefaultReorderQuantity = item.DefaultReorderQuantity.GetValueOrDefault(0);
                            }

                            if (objBinDTO.DefaultPullQuantity.HasValue && objBinDTO.DefaultPullQuantity.GetValueOrDefault(0) > 0)
                            {
                                obj.BinDefaultPullQuantity = objBinDTO.DefaultPullQuantity.GetValueOrDefault(0);
                            }
                            else
                            {
                                obj.BinDefaultPullQuantity = item.DefaultPullQuantity.GetValueOrDefault(0);
                            }
                            ItemListBin.Add(obj);
                        }
                    }
                }

                //foreach (var item in ItemListBin)
                //{
                //    lstItems.Add(item);
                //}

                if (ItemListBin != null)
                {
                    lstItems = lstItems.Concat(ItemListBin).ToList();
                }

                lstItemsTemp = new List<ItemMasterDTO>();
                foreach (var item in lstItems)
                {
                    List<string> lstbinIds = dicItemsBin[item.ID.ToString()];
                    if (lstbinIds.Contains(item.BinID.ToString()))
                    {
                        lstItemsTemp.Add(item);
                    }
                }
                lstItems = new List<ItemMasterDTO>();
                if (lstItemsTemp.Count > 0)
                {
                    //    foreach (var item in lstItemsTemp)
                    //    {
                    //        lstItems.Add(item);
                    //    }

                    lstItems = lstItemsTemp;
                }

                lstMasterDTO = lstItems;
                /*
                if (!string.IsNullOrEmpty(SortField) && !SortField.ToLower().Contains("null"))
                {
                    lstMasterDTO = lstItems.OrderBy(SortField);
                }
                */

                List<ItemMasterDTO> finalList = new List<ItemMasterDTO>();
                for (int i = 0; i < strItemBin.Length; i++)
                {

                    long itemID = Convert.ToInt64(strItemBin[i].Split('#')[0]);
                    long binID = Convert.ToInt64(strItemBin[i].Split('#')[1]);
                    //var vItems = lstMasterDTO.Where(x => x.ID == itemID && x.BinID == binID).ToList();
                    List<ItemMasterDTO> vItems = lstMasterDTO.Where(x => x.ID == itemID && x.BinID == binID).ToList();
                    if (vItems != null && vItems.Count > 0)
                    {
                        //foreach (ItemMasterDTO item in vItems)
                        //{
                        //    finalList.Add(item);
                        //}
                        finalList = finalList.Concat(vItems).ToList();
                    }
                }

                objLableDTOList = new List<InventoryLabelDTO>();
                foreach (var item in finalList)
                {
                    if (!string.IsNullOrEmpty(item.BinNumber))
                        item.BinNumber = item.BinNumber.Replace("[|EmptyStagingBin|]", "");
                    objLableDTOList.Add(GetInvertoryLabelDTO(item));
                }
                objectList = objLableDTOList.Cast<object>().ToList();
                return objectList;

            }
            finally
            {
                objDAL = null;
                lstMasterDTO = null;
                lstItems = null;
                lstItemsTemp = null;
                dicItemsBin = null;
                objBinDAL = null;
                objAPI = null;
                ItemListBin = null;
                lstItemLocationQty = null;
                objBinDTO = null;
                obj = null;
                objLableDTOList = null;
            }

        }

        private List<object> GetObjectListForItemModule_SerialLot(Int64[] IDs, string SortField, string ROTDIds)
        {
            List<object> objectList = null;
            ItemMasterDAL objDAL = null;
            IEnumerable<ItemMasterDTO> lstMasterDTO = null;
            List<ItemMasterDTO> lstItems = null;

            BinMasterDAL objBinDAL = null;

            BinMasterDTO objBinDTO = null;
            ItemMasterDTO obj = null;
            List<InventoryLabelDTO> objLableDTOList = null;
            ItemLocationDetailsDAL ilDAL = null;
            List<ItemLocationDetailsDTO> lstItemLocationDetails = null;
            InventoryLabelDTO invLabelDTO = null;
            try
            {
                objDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                lstMasterDTO = objDAL.GetAllItemsWithJoins(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, null);

                if (!(IDs.Length == 1 && IDs[0] == 0))
                    lstItems = lstMasterDTO.Where(x => IDs.Contains(x.ID)).ToList();

                string[] strLocationDetailIDs = ROTDIds.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                ilDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                lstItemLocationDetails = ilDAL.GetRecordsByIDs(ROTDIds, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
                objLableDTOList = new List<InventoryLabelDTO>();
                objBinDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                for (int i = 0; i < lstItemLocationDetails.Count; i++)
                {
                    invLabelDTO = null;
                    ItemMasterDTO item = lstItems.FirstOrDefault(x => x.GUID == lstItemLocationDetails[i].ItemGUID.GetValueOrDefault(Guid.Empty));
                    if (i == 0)
                    {
                        item.BinID = lstItemLocationDetails[i].BinID;
                        item.BinNumber = lstItemLocationDetails[i].BinNumber;
                        if (!item.IsItemLevelMinMaxQtyRequired.GetValueOrDefault(false))
                        {
                            objBinDTO = objBinDAL.GetBinByID(lstItemLocationDetails[i].BinID.GetValueOrDefault(0), SessionHelper.RoomID, SessionHelper.CompanyID);
                            //objBinDTO = objBinDAL.GetItemLocation( SessionHelper.RoomID, SessionHelper.CompanyID, false, false,Guid.Empty, lstItemLocationDetails[i].BinID.GetValueOrDefault(0),null,null).FirstOrDefault();
                            item.CriticalQuantity = objBinDTO.CriticalQuantity.GetValueOrDefault(0);
                            item.MinimumQuantity = objBinDTO.MinimumQuantity.GetValueOrDefault(0);
                            item.MaximumQuantity = objBinDTO.MaximumQuantity.GetValueOrDefault(0);
                        }
                        invLabelDTO = GetInvertoryLabelDTO(item);
                    }
                    else
                    {
                        objBinDTO = objBinDAL.GetBinByID(lstItemLocationDetails[i].BinID.GetValueOrDefault(0), SessionHelper.RoomID, SessionHelper.CompanyID);
                        //objBinDTO = objBinDAL.GetItemLocation( SessionHelper.RoomID, SessionHelper.CompanyID, false, false,Guid.Empty, lstItemLocationDetails[i].BinID.GetValueOrDefault(0),null,null).FirstOrDefault();
                        obj = GetItemMasterDTOFromBinDTO(objBinDTO, item);
                        invLabelDTO = GetInvertoryLabelDTO(obj);
                    }

                    if (!string.IsNullOrEmpty(invLabelDTO.BinNumber))
                        invLabelDTO.BinNumber = invLabelDTO.BinNumber.Replace("[|EmptyStagingBin|]", "");

                    if (item.SerialNumberTracking)
                    {
                        invLabelDTO.SerialNumberTracking = "Yes";
                        invLabelDTO.ItemTrackingType = "Serial";
                        invLabelDTO.SerialOrLotNumber = lstItemLocationDetails[i].SerialNumber;
                    }
                    else if (item.LotNumberTracking)
                    {
                        invLabelDTO.LotNumberTracking = "Yes";
                        invLabelDTO.ItemTrackingType = "Lot";
                        invLabelDTO.SerialOrLotNumber = lstItemLocationDetails[i].LotNumber;
                    }

                    if (item.DateCodeTracking)
                    {
                        invLabelDTO.DateCodeTracking = "Yes";
                        invLabelDTO.ExpirationDate = lstItemLocationDetails[i].ExpirationStr;
                    }
                    if (!item.SerialNumberTracking)
                    {
                        invLabelDTO.SerialNumberTracking = "No";
                    }
                    if (!item.LotNumberTracking)
                    {
                        invLabelDTO.LotNumberTracking = "No";
                    }
                    if (!item.DateCodeTracking)
                    {
                        invLabelDTO.DateCodeTracking = "No";
                    }
                    objLableDTOList.Add(invLabelDTO);

                }

                objectList = objLableDTOList.Cast<object>().ToList();
                return objectList;

            }
            finally
            {
                objDAL = null;
                lstMasterDTO = null;
                lstItems = null;
                objBinDAL = null;

                objBinDTO = null;
                obj = null;
                objLableDTOList = null;
            }

        }

        /// <summary>
        /// GetModuleWiseObjecList
        /// </summary>
        /// <param name="objMMDTO"></param>
        /// <param name="IDs"></param>
        /// <returns></returns>
        public List<object> GetModuleWiseObjecList(LabelModuleMasterDTO objMMDTO, Int64[] IDs, string SortField, LabelFieldModuleTemplateDetailDTO objLFMTDetailDTO, string ROTDIds, string CallFrom)
        {
            List<object> objectList = null;

            if (objMMDTO.ModuleDTOName == "AssetLabelDTO")
            {
                objectList = GetObjectListForAssetModule(IDs, SortField);
            }
            else if (objMMDTO.ModuleDTOName == "InventoryLabelDTO")
            {
                if (!string.IsNullOrEmpty(ROTDIds) && CallFrom == "ItemsBinsBarcode")
                    objectList = GetObjectListForItemModule_Bins(IDs, SortField, ROTDIds);
                else if (!string.IsNullOrEmpty(ROTDIds) && CallFrom == "ItemSerailLotBarcode")
                    objectList = GetObjectListForItemModule_SerialLot(IDs, SortField, ROTDIds);
                else
                    objectList = GetObjectListForItemModule(IDs, SortField);


                #region Inverntory code 

                //ItemMasterDAL objDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                //IEnumerable<ItemMasterDTO> lstMasterDTO = objDAL.GetAllItemsWithJoins(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, null);
                //List<ItemMasterDTO> lstItems = null;
                //List<ItemMasterDTO> lstItemsTemp = null;
                //Dictionary<string, List<string>> dicItemsBin = new Dictionary<string, List<string>>();
                //if (!(IDs.Length == 1 && IDs[0] == 0))
                //{
                //    lstItems = lstMasterDTO.Where(x => IDs.Contains(x.ID)).ToList();
                //}

                //if (ROTDIds != null && ROTDIds.Length > 0 && CallFrom = "ItemsBinsBarcode")
                //{
                //    string[] strItemBin = ROTDIds.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                //    foreach (var stritem in strItemBin)
                //    {
                //        string[] itembinid = stritem.Split(new char[1] { '#' }, StringSplitOptions.RemoveEmptyEntries);
                //        if (dicItemsBin.Keys.Contains(itembinid[0]))
                //        {
                //            List<string> lstBins = dicItemsBin[itembinid[0]];
                //            if (lstBins != null)
                //                lstBins.Add(itembinid[1]);
                //            dicItemsBin[itembinid[0]] = lstBins;
                //        }
                //        else
                //        {
                //            List<string> lstBins = new List<string>();
                //            lstBins.Add(itembinid[1]);
                //            dicItemsBin.Add(itembinid[0], lstBins);
                //        }
                //    }
                //}


                ////Int64[] SelectedIds = objLFMTDetailDTO.lstSelectedModuleFields.Select(x => x.ID).ToArray();


                ////if (1 == 0 && (SelectedIds.Contains(217) || SelectedIds.Contains(219) || SelectedIds.Contains(218)))
                ////{
                ////IEnumerable<ItemMasterDTO> lstItemMasterMinMaxItemLevel = lstItems;//.Where(x => !x.IsItemLevelMinMaxQtyRequired.GetValueOrDefault(false));

                //BinMasterDAL objBinDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                //ItemLocationQTYDAL objAPI = new ItemLocationQTYDAL(SessionHelper.EnterPriseDBName);
                //List<ItemMasterDTO> ItemListBin = new List<ItemMasterDTO>();

                //foreach (var item in lstItems)
                //{
                //    if (!string.IsNullOrEmpty(ROTDIds) && dicItemsBin.Count > 0)
                //    {
                //        List<ItemsBins> lstItemLocationQty = objBinDAL.GetItemsBinForLabels(item.GUID.ToString());
                //        for (int i = 0; i < lstItemLocationQty.Count; i++)
                //        {

                //            if (i == 0)
                //            {
                //                item.BinID = lstItemLocationQty[i].BinID;
                //                item.BinNumber = lstItemLocationQty[i].BinNumber;
                //                if (!item.IsItemLevelMinMaxQtyRequired.GetValueOrDefault(false))
                //                {
                //                    BinMasterDTO objBinDTO = objBinDAL.GetRecord(lstItemLocationQty[i].BinID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                //                    item.CriticalQuantity = objBinDTO.CriticalQuantity.GetValueOrDefault(0);
                //                    item.MinimumQuantity = objBinDTO.MinimumQuantity.GetValueOrDefault(0);
                //                    item.MaximumQuantity = objBinDTO.MaximumQuantity.GetValueOrDefault(0);
                //                }
                //            }
                //            else
                //            {
                //                BinMasterDTO objBinDTO = objBinDAL.GetRecord(lstItemLocationQty[i].BinID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                //                ItemMasterDTO obj = GetItemMasterDTOFromBinDTO(objBinDTO, item);
                //                ItemListBin.Add(obj);
                //            }
                //        }

                //    }
                //    else
                //    {
                //        List<ItemLocationQTYDTO> lstItemLocationQty = objAPI.GetBinsByItemDB(item.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, "BinNumber Asc", false);
                //        for (int i = 0; i < lstItemLocationQty.Count; i++)
                //        {

                //            if (i == 0)
                //            {
                //                item.BinID = lstItemLocationQty[i].BinID;
                //                item.BinNumber = lstItemLocationQty[i].BinNumber;
                //                if (!item.IsItemLevelMinMaxQtyRequired.GetValueOrDefault(false))
                //                {
                //                    BinMasterDTO objBinDTO = objBinDAL.GetRecord(lstItemLocationQty[i].BinID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                //                    item.CriticalQuantity = objBinDTO.CriticalQuantity.GetValueOrDefault(0);
                //                    item.MinimumQuantity = objBinDTO.MinimumQuantity.GetValueOrDefault(0);
                //                    item.MaximumQuantity = objBinDTO.MaximumQuantity.GetValueOrDefault(0);
                //                }
                //            }
                //            else
                //            {
                //                BinMasterDTO objBinDTO = objBinDAL.GetRecord(lstItemLocationQty[i].BinID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                //                ItemMasterDTO obj = GetItemMasterDTOFromBinDTO(objBinDTO, item);
                //                ItemListBin.Add(obj);
                //            }

                //        }
                //    }

                //}


                //foreach (var item in ItemListBin)
                //{
                //    lstItems.Add(item);
                //}

                //if (!string.IsNullOrEmpty(ROTDIds) && dicItemsBin.Count > 0)
                //{
                //    lstItemsTemp = new List<ItemMasterDTO>();
                //    foreach (var item in lstItems)
                //    {
                //        List<string> lstbinIds = dicItemsBin[item.ID.ToString()];
                //        if (lstbinIds.Contains(item.BinID.ToString()))
                //        {
                //            lstItemsTemp.Add(item);
                //        }
                //    }
                //    lstItems = new List<ItemMasterDTO>();
                //    if (lstItemsTemp.Count > 0)
                //    {
                //        foreach (var item in lstItemsTemp)
                //        {
                //            lstItems.Add(item);
                //        }
                //    }
                //}

                //lstMasterDTO = lstItems;
                //if (!string.IsNullOrEmpty(SortField) && !SortField.ToLower().Contains("null"))
                //{
                //    lstMasterDTO = lstItems.OrderBy(SortField);
                //}

                //List<InventoryLabelDTO> objLableDTOList = new List<InventoryLabelDTO>();
                //foreach (var item in lstMasterDTO)
                //{
                //    if (!string.IsNullOrEmpty(item.BinNumber))
                //        item.BinNumber = item.BinNumber.Replace("[|EmptyStagingBin|]", "");
                //    objLableDTOList.Add(GetInvertoryLabelDTO(item));
                //}
                //objectList = objLableDTOList.Cast<object>().ToList();
                #endregion

            }
            else if (objMMDTO.ModuleDTOName == "KittingLabelDTO")
            {
                //ItemMasterDAL objDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                //IEnumerable<ItemMasterDTO> lstMasterDTO = objDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                KitMasterDAL objDAL = new KitMasterDAL(SessionHelper.EnterPriseDBName);
                TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
                IEnumerable<KitMasterDTO> lstMasterDTO = objDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, SessionHelper.UserSupplierIds, CurrentTimeZone);
                if (!(IDs.Length == 1 && IDs[0] == 0))
                {
                    lstMasterDTO = lstMasterDTO.Where(x => IDs.Contains(x.ID));
                }

                /*
                if (!string.IsNullOrEmpty(SortField))
                {
                    lstMasterDTO = lstMasterDTO.OrderBy(SortField);
                }
                */

                List<KitMasterDTO> finalList = new List<KitMasterDTO>();
                for (int i = 0; i < IDs.Length; i++)
                {
                    var vList = lstMasterDTO.Where(x => x.ID == IDs[i]).FirstOrDefault();
                    if (vList != null)
                    {
                        finalList.Add(vList);
                    }
                }


                List<KittingLabelDTO> objLableDTOList = new List<KittingLabelDTO>();
                foreach (var item in finalList)
                {
                    objLableDTOList.Add(GetKittingLabelDTO(item));
                }
                objectList = objLableDTOList.Cast<object>().ToList();
            }
            else if (objMMDTO.ModuleDTOName == "OrderLabelDTO")
            {
                OrderMasterDAL objDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
                IEnumerable<OrderMasterDTO> lstMasterDTO;
                string orderIds = string.Empty;

                if (IDs.Any())
                {
                    orderIds = string.Join(",", IDs);
                }
                
                if (Request.UrlReferrer != null)
                {
                    if (Request.UrlReferrer.AbsolutePath.Contains("ReturnOrderList"))
                    {
                        lstMasterDTO = objDAL.GetOrdersByOrderTypeAndIdsNormal(SessionHelper.RoomID, SessionHelper.CompanyID, OrderType.RuturnOrder, orderIds);
                    }
                    else
                    {
                        lstMasterDTO = objDAL.GetOrdersByOrderTypeAndIdsNormal(SessionHelper.RoomID, SessionHelper.CompanyID, OrderType.Order, orderIds);
                    }
                }
                else
                {
                    var actionName = Convert.ToString(Session["ActionName"]);
                    if (!string.IsNullOrEmpty(actionName) && !string.IsNullOrWhiteSpace(actionName) && actionName.ToLower() == "returnorderlist")
                    {
                        lstMasterDTO = objDAL.GetOrdersByOrderTypeAndIdsNormal(SessionHelper.RoomID, SessionHelper.CompanyID, OrderType.RuturnOrder, orderIds);
                    }
                    else
                    {
                        lstMasterDTO = objDAL.GetOrdersByOrderTypeAndIdsNormal(SessionHelper.RoomID, SessionHelper.CompanyID, OrderType.Order, orderIds);
                    }
                }
                

                if (!(IDs.Length == 1 && IDs[0] == 0))
                {
                    lstMasterDTO = lstMasterDTO.Where(x => IDs.Contains(x.ID));
                }

                /*
                if (!string.IsNullOrEmpty(SortField))
                {
                    lstMasterDTO = lstMasterDTO.OrderBy(SortField);
                }
                */

                List<OrderMasterDTO> finalList = new List<OrderMasterDTO>();
                for (int i = 0; i < IDs.Length; i++)
                {
                    var vList = lstMasterDTO.Where(x => x.ID == IDs[i]).FirstOrDefault();
                    if (vList != null)
                    {
                        finalList.Add(vList);
                    }
                }



                List<OrderLabelDTO> objLableDTOList = new List<OrderLabelDTO>();
                foreach (var item in finalList)
                {
                    List<OrderLabelDTO> lstOrdLblDTO = GetOrderLabelDTO(item);
                    foreach (var lblDTO in lstOrdLblDTO)
                    {
                        if (!string.IsNullOrEmpty(lblDTO.BinNumber))
                            lblDTO.BinNumber = lblDTO.BinNumber.Replace("[|EmptyStagingBin|]", "");
                        objLableDTOList.Add(lblDTO);
                    }
                }
                objectList = objLableDTOList.Cast<object>().ToList();
            }
            else if (objMMDTO.ModuleDTOName == "QuickListLabelDTO")
            {
                QuickListDAL objDAL = new QuickListDAL(SessionHelper.EnterPriseDBName);
                IEnumerable<QuickListMasterDTO> lstMasterDTO = objDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                if (!(IDs.Length == 1 && IDs[0] == 0))
                {
                    lstMasterDTO = lstMasterDTO.Where(x => IDs.Contains(x.ID));
                }

                /*
                if (!string.IsNullOrEmpty(SortField))
                {
                    lstMasterDTO = lstMasterDTO.OrderBy(SortField);
                }
                */

                List<QuickListMasterDTO> finalList = new List<QuickListMasterDTO>();
                for (int i = 0; i < IDs.Length; i++)
                {
                    var vList = lstMasterDTO.Where(x => x.ID == IDs[i]).FirstOrDefault();
                    if (vList != null)
                    {
                        finalList.Add(vList);
                    }
                }


                List<QuickListLabelDTO> objLableDTOList = new List<QuickListLabelDTO>();
                foreach (var item in finalList)
                {
                    objLableDTOList.Add(GetQuickListLabelDTO(item));
                }
                objectList = objLableDTOList.Cast<object>().ToList();

            }
            else if (objMMDTO.ModuleDTOName == "ReceiveLabelDTO")
            {
                //OrderDetailsDAL objDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
                //IEnumerable<OrderDetailsDTO> lstDetailDTO = objDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => x.IsDeleted == false && x.IsArchived == false);
                int totlCnt = 0;
                ReceivedOrderTransferDetailDAL objROTDDAL = new ReceivedOrderTransferDetailDAL(SessionHelper.EnterPriseDBName);
                ReceiveOrderDetailsDAL objDAL = new ReceiveOrderDetailsDAL(SessionHelper.EnterPriseDBName);
                IEnumerable<ReceivableItemDTO> lstDetailDTO = null;
                IEnumerable<ReceivedOrderTransferDetailDTO> lstROTDDetailDTO = null;
                List<ReceiveLabelDTO> objLableDTOList = new List<ReceiveLabelDTO>();
                Int64[] arrROTDIDs = null;
                if (!string.IsNullOrEmpty(ROTDIds))
                {
                    lstROTDDetailDTO = objROTDDAL.GetROTDByIDsPlain(ROTDIds, SessionHelper.RoomID, SessionHelper.CompanyID);

                    if (!(ROTDIds.Length == 1 && ROTDIds[0] == 0))
                    {
                        //lstROTDDetailDTO = lstROTDDetailDTO.Where(x => IDs.Contains(x.ID));
                        arrROTDIDs = ROTDIds.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(Int64.Parse).ToArray();
                        if (arrROTDIDs != null && arrROTDIDs.Length > 0)
                            lstROTDDetailDTO = lstROTDDetailDTO.Where(x => arrROTDIDs.Contains(x.ID));
                    }

                    //if (!string.IsNullOrEmpty(SortField))
                    //{
                    //    SortField = SortField.Replace("OrderNumber ", "OrderNumber_ForSorting ");
                    //    lstDetailDTO = lstDetailDTO.OrderBy(SortField);
                    //}

                    foreach (var item in lstROTDDetailDTO)
                    {
                        if (!string.IsNullOrEmpty(item.BinNumber))
                            item.BinNumber = item.BinNumber.Replace("[|EmptyStagingBin|]", "");
                        objLableDTOList.Add(GetReceivedLabelDTO(item));
                    }
                }
                else
                {
                    var tmpsupplierIds = new List<long>();
                    string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
                    TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
                    lstDetailDTO = objDAL.GetALLReceiveListByPaging(0, int.MaxValue, out totlCnt, "", SortField, SessionHelper.RoomID, SessionHelper.CompanyID, false, false, "4,5,6,7,8", tmpsupplierIds, RoomDateFormat, CurrentTimeZone);

                    if (!(IDs.Length == 1 && IDs[0] == 0))
                        lstDetailDTO = lstDetailDTO.Where(x => IDs.Contains(x.OrderDetailID));

                    Guid[] orderDetailGuids = lstDetailDTO.Select(x => x.OrderDetailGUID).ToArray<Guid>();
                    lstROTDDetailDTO = objROTDDAL.GetROTDByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID);

                    if (!(ROTDIds.Length == 1 && ROTDIds[0] == 0))
                        lstROTDDetailDTO = lstROTDDetailDTO.Where(x => orderDetailGuids.Contains(x.OrderDetailGUID.GetValueOrDefault(Guid.Empty)));
                    else
                        lstROTDDetailDTO = null;

                    if (lstROTDDetailDTO != null && lstROTDDetailDTO.Count() > 0)
                    {
                        foreach (var item in lstROTDDetailDTO)
                            objLableDTOList.Add(GetReceivedLabelDTO(item));
                    }
                    else
                    {
                        //if (!string.IsNullOrEmpty(SortField))
                        //{
                        //    SortField = SortField.Replace("OrderNumber ", "OrderNumber_ForSorting ");
                        //    lstDetailDTO = lstDetailDTO.OrderBy(SortField);
                        //}

                        foreach (var item in lstDetailDTO)
                        {
                            if (!string.IsNullOrEmpty(item.ReceiveBinName))
                                item.ReceiveBinName = item.ReceiveBinName.Replace("[|EmptyStagingBin|]", "");
                            objLableDTOList.Add(GetReceiveLabelDTO(item));
                        }
                    }
                }
                objectList = objLableDTOList.Cast<object>().ToList();
            }
            else if (objMMDTO.ModuleDTOName == "StagingLabelDTO")
            {
                MaterialStagingDAL objDAL = new MaterialStagingDAL(SessionHelper.EnterPriseDBName);
                //IEnumerable<MaterialStagingDTO> lstMasterDTO = objDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                IEnumerable<MaterialStagingDTO> lstMasterDTO = objDAL.GetMaterialStaging(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, string.Empty, null);
                if (!(IDs.Length == 1 && IDs[0] == 0))
                {
                    lstMasterDTO = lstMasterDTO.Where(x => IDs.Contains(x.ID));
                }

                /*
                if (!string.IsNullOrEmpty(SortField))
                {
                    lstMasterDTO = lstMasterDTO.OrderBy(SortField);
                }
                */

                List<MaterialStagingDTO> finalList = new List<MaterialStagingDTO>();
                for (int i = 0; i < IDs.Length; i++)
                {
                    var vList = lstMasterDTO.Where(x => x.ID == IDs[i]).FirstOrDefault();
                    if (vList != null)
                    {
                        finalList.Add(vList);
                    }
                }

                List<StagingLabelDTO> objLableDTOList = new List<StagingLabelDTO>();
                foreach (var item in finalList)
                {
                    List<StagingLabelDTO> lstOrdLblDTO = GetStagingLabelDTO(item);
                    foreach (var lblDTO in lstOrdLblDTO)
                    {
                        if (!string.IsNullOrEmpty(lblDTO.BinNumber))
                            lblDTO.BinNumber = lblDTO.BinNumber.Replace("[|EmptyStagingBin|]", "");
                        objLableDTOList.Add(lblDTO);
                    }
                }
                objectList = objLableDTOList.Cast<object>().ToList();
            }
            else if (objMMDTO.ModuleDTOName == "ToolLabelDTO")
            {
                ToolMasterDAL objToolDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
                List<ToolMasterDTO> lstToolDTO = new List<ToolMasterDTO>();

                string toolIds = string.Empty;

                if (IDs.Any())
                {
                    toolIds = string.Join(",", IDs);
                }

                if (!(IDs.Length == 1 && IDs[0] == 0))
                {
                    lstToolDTO = objToolDAL.GetToolByIDsNormal(toolIds, SessionHelper.RoomID, SessionHelper.CompanyID);
                    //lstToolDTO = lstToolDTO.Where(x => IDs.Contains(x.ID));
                }


                List<ToolMasterDTO> finalList = new List<ToolMasterDTO>();
                for (int i = 0; i < IDs.Length; i++)
                {
                    var vList = lstToolDTO.Where(x => x.ID == IDs[i]).FirstOrDefault();
                    if (vList != null)
                    {
                        finalList.Add(vList);
                    }
                }

                List<ToolLabelDTO> objLableDTOList = new List<ToolLabelDTO>();
                foreach (var item in finalList)
                {
                    objLableDTOList.Add(GetToolLabelDTO(item));
                }
                objectList = objLableDTOList.Cast<object>().ToList();
            }
            else if (objMMDTO.ModuleDTOName == "TransferLabelDTO")
            {
                TransferMasterDAL objDAL = new TransferMasterDAL(SessionHelper.EnterPriseDBName);
                List<TransferMasterDTO> lstMasterDTO = new List<TransferMasterDTO>();
                
                if (IDs != null && IDs.Any() && IDs.Count() > 0)
                {
                    lstMasterDTO = objDAL.GetTransfersByIdsNormal(SessionHelper.RoomID, SessionHelper.CompanyID, IDs);
                }
                
                if (!(IDs.Length == 1 && IDs[0] == 0))
                {
                    lstMasterDTO = lstMasterDTO.Where(x => IDs.Contains(x.ID)).ToList();
                }

                /*
                if (!string.IsNullOrEmpty(SortField))
                {
                    lstMasterDTO = lstMasterDTO.OrderBy(SortField);
                }
                */

                List<TransferMasterDTO> finalList = new List<TransferMasterDTO>();
                for (int i = 0; i < IDs.Length; i++)
                {
                    var vList = lstMasterDTO.Where(x => x.ID == IDs[i]).FirstOrDefault();
                    if (vList != null)
                    {
                        finalList.Add(vList);
                    }
                }

                List<TransferLabelDTO> objLableDTOList = new List<TransferLabelDTO>();
                foreach (var item in finalList)
                {
                    List<TransferLabelDTO> lstOrdLblDTO = GetTransferLabelDTO(item);
                    foreach (var lblDTO in lstOrdLblDTO)
                    {
                        if (!string.IsNullOrEmpty(lblDTO.BinNumber))
                            lblDTO.BinNumber = lblDTO.BinNumber.Replace("[|EmptyStagingBin|]", "");
                        objLableDTOList.Add(lblDTO);
                    }
                }
                objectList = objLableDTOList.Cast<object>().ToList();

            }
            else if (objMMDTO.ModuleDTOName == "TechnicianLabelDTO")
            {
                TechnicialMasterDAL objDAL = new TechnicialMasterDAL(SessionHelper.EnterPriseDBName);
                IEnumerable<TechnicianMasterDTO> lstMasterDTO = objDAL.GetTechnicianByRoomIDPlain(SessionHelper.RoomID, SessionHelper.CompanyID);

                if (!(IDs.Length == 1 && IDs[0] == 0))
                {
                    lstMasterDTO = lstMasterDTO.Where(x => IDs.Contains(x.ID));
                }

                /*
                if (!string.IsNullOrEmpty(SortField))
                {
                    lstMasterDTO = lstMasterDTO.OrderBy(SortField);
                }
                */

                List<TechnicianMasterDTO> finalList = new List<TechnicianMasterDTO>();
                for (int i = 0; i < IDs.Length; i++)
                {
                    var vList = lstMasterDTO.Where(x => x.ID == IDs[i]).FirstOrDefault();
                    if (vList != null)
                    {
                        finalList.Add(vList);
                    }
                }

                List<TechnicianLabelDTO> objLableDTOList = new List<TechnicianLabelDTO>();
                foreach (var item in finalList)
                {
                    TechnicianLabelDTO lstOrdLblDTO = GetTechnicianLabelDTO(item);
                    objLableDTOList.Add(lstOrdLblDTO);
                }
                objLableDTOList = objLableDTOList.OrderBy(x => x.TechnicianName).ToList();
                objectList = objLableDTOList.Cast<object>().ToList();

            }
            else if (objMMDTO.ModuleDTOName == "LocationLabelDTO")
            {
                BinMasterDAL objDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                //IEnumerable<BinMasterDTO> lstMasterDTO = objDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                IEnumerable<BinMasterDTO> lstMasterDTO = null;

                if (!(IDs.Length == 1 && IDs[0] == 0))
                {
                    string IDcsv = string.Join(",", IDs);
                    lstMasterDTO = objDAL.GetBinMasterbyIds(SessionHelper.RoomID, SessionHelper.CompanyID, IDcsv);
                    //lstMasterDTO = lstMasterDTO.Where(x => IDs.Contains(x.ID));
                }

                /*
                if (!string.IsNullOrEmpty(SortField))
                {
                    lstMasterDTO = lstMasterDTO.OrderBy(SortField);
                }
                */

                List<BinMasterDTO> finalList = new List<BinMasterDTO>();
                for (int i = 0; i < IDs.Length; i++)
                {
                    var vList = lstMasterDTO.Where(x => x.ID == IDs[i]).FirstOrDefault();
                    if (vList != null)
                    {
                        finalList.Add(vList);
                    }
                }

                List<LocationLabelDTO> objLableDTOList = new List<LocationLabelDTO>();
                foreach (var item in finalList)
                {
                    LocationLabelDTO lstOrdLblDTO = GetLocationLabelDTO(item);
                    objLableDTOList.Add(lstOrdLblDTO);
                }
                objLableDTOList = objLableDTOList.OrderBy(x => x.Location).ToList();
                objectList = objLableDTOList.Cast<object>().ToList();

            }
            else if (objMMDTO.ModuleDTOName == "WorkOrderLabelDTO")
            {
                WorkOrderDAL objDAL = new WorkOrderDAL(SessionHelper.EnterPriseDBName);
                //IEnumerable<WorkOrderDTO> lstMasterDTO = objDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                List<WorkOrderDTO> lstMasterDTO = objDAL.GetWorkOrdersByIdsJoins(string.Join(",", IDs), SessionHelper.RoomID, SessionHelper.CompanyID);
                //if (!(IDs.Length == 1 && IDs[0] == 0))
                //{
                //    lstMasterDTO = lstMasterDTO.Where(x => IDs.Contains(x.ID));
                //}

                /*
                if (!string.IsNullOrEmpty(SortField))
                {
                    lstMasterDTO = lstMasterDTO.OrderBy(SortField);
                }
                */

                List<WorkOrderDTO> finalList = new List<WorkOrderDTO>();
                for (int i = 0; i < IDs.Length; i++)
                {
                    var vList = lstMasterDTO.Where(x => x.ID == IDs[i]).FirstOrDefault();
                    if (vList != null)
                    {
                        finalList.Add(vList);
                    }
                }

                List<WorkOrderLabelDTO> objLableDTOList = new List<WorkOrderLabelDTO>();
                foreach (var item in finalList)
                {
                    List<WorkOrderLabelDTO> lstWOrdLblDTO = GetWorkOrderLabelDTO(item);
                    foreach (var lblDTO in lstWOrdLblDTO)
                    {
                        if (!string.IsNullOrEmpty(lblDTO.BinNumber))
                            lblDTO.BinNumber = lblDTO.BinNumber.Replace("[|EmptyStagingBin|]", "");
                        objLableDTOList.Add(lblDTO);
                    }
                    //objLableDTOList.Add(lstOrdLblDTO);
                }
                //objLableDTOList = objLableDTOList.OrderBy(x => x.Name).ToList();
                objectList = objLableDTOList.Cast<object>().ToList();

            }
            return objectList;
        }

        /// <summary>
        /// Convert Pixel To Point
        /// </summary>
        /// <param name="ValueInPixel"></param>
        /// <returns></returns>
        private float ConvertPixelToPoint(float ValueInPixel)
        {
            return (ValueInPixel * 72) / 96;

        }

        /// <summary>
        /// Convert Inch To Point float
        /// </summary>
        /// <param name="ValueInInch"></param>
        /// <returns></returns>
        private float ConvertInchToPoint(float ValueInInch)
        {
            //return ValueInInch * 72;
            return iTextSharp.text.Utilities.InchesToPoints((float)ValueInInch);
        }

        /// <summary>
        /// Convert Pixel To Point
        /// </summary>
        /// <param name="ValueInPixel"></param>
        /// <returns></returns>
        private float ConvertPointToPixel(float ValueInPoint)
        {
            return (ValueInPoint * 96) / 72;
            //return iTextSharp.text.Utilities.p((float)ValueInInch) * 72);
        }
        /// <summary>
        /// Convert Inch To Point double
        /// </summary>
        /// <param name="ValueInInch"></param>
        /// <returns></returns>
        private float ConvertInchToPoint(double ValueInInch)
        {
            // return (float)(ValueInInch * 72);
            return iTextSharp.text.Utilities.InchesToPoints((float)ValueInInch);
        }

        /// <summary>
        /// Get image from relative path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="imgWidth"></param>
        /// <param name="imgHeigth"></param>
        /// <returns></returns>
        private iTextSharp.text.Image GetImageFromRelativePath(string path, float imgWidth, float imgHeigth)
        {
            byte[] imageBytes = null;
            System.Net.WebClient webClient = null;
            try
            {
                if (!string.IsNullOrEmpty(path))
                {
                    if (path.Contains("http://") || path.Contains("https://"))
                    {
                        try
                        {
                            webClient = new System.Net.WebClient();
                            imageBytes = webClient.DownloadData(path);
                        }
                        catch (Exception)
                        {
                        }
                    }
                    else if (System.IO.File.Exists(path))
                    {
                        if (path.LastIndexOf(".svg") == (path.Length - 4))
                        {
                            var svgDocument = Svg.SvgDocument.Open(path);
                            using (var smallBitmap = svgDocument.Draw())
                            {
                                var width = smallBitmap.Width;
                                var height = smallBitmap.Height;
                                using (var bitmap = svgDocument.Draw(width, height))
                                {
                                    path = path.Remove(path.LastIndexOf(".svg"), 4) + ".png";
                                    bitmap.Save(path, System.Drawing.Imaging.ImageFormat.Png);
                                }
                            }
                        }

                        imageBytes = System.IO.File.ReadAllBytes(path);
                    }
                }

                try
                {
                    if (imageBytes != null)
                    {
                        iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(imageBytes);
                        //Image img = Image.GetInstance(path);
                        img.SetDpi(300, 300);
                        //img.Width = imgWidth;
                        img.ScaleToFitHeight = true;
                        img.SpacingAfter = 0;
                        img.SpacingBefore = 0;
                        img.ScaleToFit(imgWidth, imgHeigth - 2);
                        return img;
                    }
                    return null;
                }
                catch
                {
                    return null;
                }
            }
            finally
            {
                imageBytes = null;
                webClient = null;
            }

        }

        /// <summary>
        /// Method to resize, convert and save the image.
        /// </summary>
        /// <param name="image">Bitmap image.</param>
        /// <param name="maxWidth">resize width.</param>
        /// <param name="maxHeight">resize height.</param>
        /// <param name="quality">quality setting value.</param>
        /// <param name="filePath">file path.</param>      
        public iTextSharp.text.Image GetImageFromRelativePath(string path, int imgWidth, int imgHeigth)
        {
            System.Drawing.Image image = null;
            byte[] imgdata = System.IO.File.ReadAllBytes(path);
            MemoryStream ms = new MemoryStream(imgdata);
            image = System.Drawing.Image.FromStream(ms);


            // Get the image's original width and height
            int originalWidth = image.Width;
            int originalHeight = image.Height;

            // To preserve the aspect ratio
            float ratioX = (float)imgWidth / (float)originalWidth;
            float ratioY = (float)imgHeigth / (float)originalHeight;
            float ratio = Math.Min(ratioX, ratioY);

            // New width and height based on aspect ratio
            int newWidth = (int)(originalWidth * ratio);
            int newHeight = (int)(originalHeight * ratio);

            newWidth = imgWidth;
            newHeight = imgHeigth;

            // Convert other formats (including CMYK) to RGB.
            System.Drawing.Bitmap newImage = new System.Drawing.Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb);

            // Draws the image in the specified size with quality mode set to HighQuality
            using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(newImage))
            {
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            // Get an ImageCodecInfo object that represents the JPEG codec.
            System.Drawing.Imaging.ImageCodecInfo imageCodecInfo = this.GetEncoderInfo(System.Drawing.Imaging.ImageFormat.Jpeg);

            // Create an Encoder object for the Quality parameter.
            System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.Quality;

            // Create an EncoderParameters object. 
            EncoderParameters encoderParameters = new EncoderParameters(1);

            // Save the image as a JPEG file with quality level.
            EncoderParameter encoderParameter = new EncoderParameter(encoder, 1);
            encoderParameters.Param[0] = encoderParameter;
            ms = new MemoryStream();
            newImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(ms.ToArray());
            img.ScaleToFitHeight = true;
            img.SpacingAfter = 0;
            img.SpacingBefore = 0;
            img.ScaleToFit(imgWidth, imgHeigth);
            return img;
            //newImage.Save(filePath, imageCodecInfo, encoderParameters);
        }

        /// <summary>
        /// Method to get encoder infor for given image format.
        /// </summary>
        /// <param name="format">Image format</param>
        /// <returns>image codec info.</returns>
        private ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders().SingleOrDefault(c => c.FormatID == format.Guid);
        }

        /// <summary>
        /// GetBarcode
        /// </summary>
        /// <param name="barcodeString"></param>
        private byte[] GetBarcode(LabelTemplateMasterDTO objSizeDTO, string strBarcode, int width, int Height, bool IncludeLabel, string strBarcodeType, string OptionalKeyWhenImageLarge)
        {
            BarcodeLib.Barcode b = new BarcodeLib.Barcode();
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.Drawing.Image img = null;
            BarcodeLib.TYPE type = BarcodeLib.TYPE.CODE128B;

            try
            {
                b.Alignment = BarcodeLib.AlignmentPositions.LEFT;
                if (Convert.ToString(strBarcodeType).Trim().Equals("39"))
                    type = BarcodeLib.TYPE.CODE39Extended;
                else if (Convert.ToString(strBarcodeType).Contains("93"))
                    type = BarcodeLib.TYPE.CODE93;

                b.IncludeLabel = IncludeLabel;
                //b.RotateFlipType = (RotateFlipType)Enum.Parse(typeof(RotateFlipType), "RotateNoneFlipNone", true);
                b.LabelPosition = BarcodeLib.LabelPositions.BOTTOMLEFT;
                width = (int)ConvertPointToPixel(width);
                Height = (int)ConvertPointToPixel(Height);

                img = b.Encode(type, strBarcode, System.Drawing.Color.Black, System.Drawing.Color.White, width, Height);

                if (img != null)
                {
                    img.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                    return ms.ToArray();
                }
                return null;

            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(OptionalKeyWhenImageLarge) && ex.Message.Contains("EGENERATE_IMAGE-2: Image size specified not large enough to draw image."))
                {
                    try
                    {
                        if (strBarcode.LastIndexOf("#") > 1)
                        {
                            string s = strBarcode;
                            int start = s.IndexOf("#") + 1;
                            int end = s.IndexOf("#", start);
                            string result = s.Remove(0, end);
                        }

                        img = b.Encode(type, OptionalKeyWhenImageLarge, System.Drawing.Color.Black, System.Drawing.Color.White, width, Height);
                        if (img != null)
                        {
                            img.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                            return ms.ToArray();
                        }
                    }
                    catch (Exception)
                    {

                        return null;
                    }

                }
                return null;
                //throw ex;
            }
            finally
            {
                b.Dispose();
                ms.Dispose();
                if (img != null)
                    img.Dispose();
            }
        }

        public System.Drawing.Bitmap Crop(System.Drawing.Bitmap bmp)
        {
            int w = bmp.Width;
            int h = bmp.Height;

            Func<int, bool> allWhiteRow = row =>
            {
                for (int i = 0; i < w; ++i)
                    if (bmp.GetPixel(i, row).R != 255)
                        return false;
                return true;
            };

            Func<int, bool> allWhiteColumn = col =>
            {
                for (int i = 0; i < h; ++i)
                    if (bmp.GetPixel(col, i).R != 255)
                        return false;
                return true;
            };

            int topmost = 0;
            for (int row = 0; row < h; ++row)
            {
                if (allWhiteRow(row))
                    topmost = row;
                else break;
            }

            int bottommost = 0;
            for (int row = h - 1; row >= 0; --row)
            {
                if (allWhiteRow(row))
                    bottommost = row;
                else break;
            }

            int leftmost = 0, rightmost = 0;
            for (int col = 0; col < w; ++col)
            {
                if (allWhiteColumn(col))
                    leftmost = col;
                else
                    break;
            }

            for (int col = w - 1; col >= 0; --col)
            {
                if (allWhiteColumn(col))
                    rightmost = col;
                else
                    break;
            }

            if (rightmost == 0) rightmost = w; // As reached left
            if (bottommost == 0) bottommost = h; // As reached top.

            int croppedWidth = rightmost - leftmost;
            int croppedHeight = bottommost - topmost;

            if (croppedWidth == 0) // No border on left or right
            {
                leftmost = 0;
                croppedWidth = w;
            }

            if (croppedHeight == 0) // No border on top or bottom
            {
                topmost = 0;
                croppedHeight = h;
            }

            try
            {
                var target = new System.Drawing.Bitmap(croppedWidth, croppedHeight);
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(target))
                {
                    g.DrawImage(bmp,
                      new System.Drawing.RectangleF(0, 0, croppedWidth, croppedHeight),
                      new System.Drawing.RectangleF(leftmost, topmost, croppedWidth, croppedHeight),
                      System.Drawing.GraphicsUnit.Pixel);
                }
                return target;
            }
            catch (Exception ex)
            {
                throw new Exception(
                  string.Format(ResLabelPrinting.CropImageException, topmost, bottommost, leftmost, rightmost, croppedWidth, croppedHeight),
                  ex);
            }
        }

        #region DTO To Label DTO Conversion Function

        public AssetLabelDTO GetAssetLabelDTO(AssetMasterDTO objMasterDTO)
        {
            AssetLabelDTO objLabelDTO = new AssetLabelDTO();
            objLabelDTO.AssetName = objMasterDTO.AssetName;
            objLabelDTO.Description = objMasterDTO.Description;
            objLabelDTO.Make = objMasterDTO.Make;
            objLabelDTO.Model = objMasterDTO.Model;
            objLabelDTO.SerialNumber = objMasterDTO.Serial;
            objLabelDTO.PurchaseDate = objMasterDTO.PurchaseDate != null ? objMasterDTO.PurchaseDate.GetValueOrDefault(DateTime.MinValue).ToString(DateFormatCSharp) : "";
            objLabelDTO.PurchasePrice = objMasterDTO.PurchasePrice.GetValueOrDefault(0).ToString(_CostDecimalFormat);
            objLabelDTO.DepreciatedValue = objMasterDTO.DepreciatedValue.GetValueOrDefault(0).ToString(_CostDecimalFormat);
            objLabelDTO.SuggestedMaintenanceDate = objMasterDTO.SuggestedMaintenanceDate != null ? objMasterDTO.SuggestedMaintenanceDate.GetValueOrDefault(DateTime.MinValue).ToString(DateFormatCSharp) : "";
            objLabelDTO.Category = objMasterDTO.AssetCategory;
            objLabelDTO.UDF1 = objMasterDTO.UDF1;
            objLabelDTO.UDF2 = objMasterDTO.UDF2;
            objLabelDTO.UDF3 = objMasterDTO.UDF3;
            objLabelDTO.UDF4 = objMasterDTO.UDF4;
            objLabelDTO.UDF5 = objMasterDTO.UDF5;
            objLabelDTO.ID = objMasterDTO.ID.ToString();
            objLabelDTO.AssetImage = GetAssetImagePath(objMasterDTO.ID);// objMasterDTO.ImagePath;
            //objLabelDTO.ImageType = objMasterDTO.ImageType.ToString();
            objLabelDTO.ImageType = objMasterDTO.ImageType ?? string.Empty;

            return objLabelDTO;
        }

        public InventoryLabelDTO GetInvertoryLabelDTO(ItemMasterDTO objMasterDTO)
        {
            InventoryLabelDTO objLabelDTO = new InventoryLabelDTO();
            //IEnumerable<MaterialStagingDetailDTO> objStagingDTO = new MaterialStagingDetailDAL(SessionHelper.EnterPriseDBName).GetStagingLocationByItem(objMasterDTO.GUID, objMasterDTO.Room.GetValueOrDefault(0), objMasterDTO.CompanyID.GetValueOrDefault(0)).Where(x => x.Quantity > 0);
            MaterialStagingDetailDAL objMSDAL = new MaterialStagingDetailDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<MaterialStagingDetailDTO> objStagingDTO = objMSDAL.GetStagingLocationByItemWithQty(objMasterDTO.GUID, objMasterDTO.Room.GetValueOrDefault(0), objMasterDTO.CompanyID.GetValueOrDefault(0));

            string stagingLocation = string.Empty;
            if (objStagingDTO != null && objStagingDTO.Count() > 0)
            {
                stagingLocation = objStagingDTO.FirstOrDefault().MaterialStagingName + " - " + objStagingDTO.FirstOrDefault().StagingBinName;
            }

            objLabelDTO.BinNumber = objMasterDTO.DefaultLocationName;
            objLabelDTO.Category = objMasterDTO.CategoryName;
            if (!string.IsNullOrWhiteSpace(objMasterDTO.BinNumber))
                objLabelDTO.BinNumber = objMasterDTO.BinNumber;

            objLabelDTO.Cost = objMasterDTO.Cost.GetValueOrDefault(0).ToString(_CostDecimalFormat);
            objLabelDTO.ItemCriticalQuantity = objMasterDTO.CriticalQuantity.GetValueOrDefault(0).ToString(_QtyDecimalFormat);
            objLabelDTO.Description = objMasterDTO.Description;
            objLabelDTO.GLAccount = objMasterDTO.GLAccount;
            objLabelDTO.ItemNumber = objMasterDTO.ItemNumber;
            objLabelDTO.LongDescription = objMasterDTO.LongDescription;
            objLabelDTO.ManufacturerNumber = objMasterDTO.ManufacturerNumber;
            objLabelDTO.Manufacturer = objMasterDTO.ManufacturerName;
            objLabelDTO.ItemMaximumQuantity = objMasterDTO.MaximumQuantity.GetValueOrDefault(0).ToString(_QtyDecimalFormat);
            objLabelDTO.ItemMinimumQuantity = objMasterDTO.MinimumQuantity.GetValueOrDefault(0).ToString(_QtyDecimalFormat);
            //objLabelDTO.PackageQuantity = objMasterDTO.PackingQuantity.GetValueOrDefault(0).ToString(_QtyDecimalFormat);
            objLabelDTO.Picture = "";
            objLabelDTO.SellPrice = objMasterDTO.SellPrice.GetValueOrDefault(0).ToString(_CostDecimalFormat);
            objLabelDTO.Staging = objMasterDTO.StagedQuantity.GetValueOrDefault(0).ToString(_QtyDecimalFormat);
            objLabelDTO.Supplier = objMasterDTO.SupplierName;
            objLabelDTO.SupplierNumber = objMasterDTO.SupplierPartNo;
            //objLabelDTO.Taxable = objMasterDTO.Taxable ? "True" : "False";
            objLabelDTO.UDF1 = objMasterDTO.UDF1;
            objLabelDTO.UDF2 = objMasterDTO.UDF2;
            objLabelDTO.UDF3 = objMasterDTO.UDF3;
            objLabelDTO.UDF4 = objMasterDTO.UDF4;
            objLabelDTO.UDF5 = objMasterDTO.UDF5;
            objLabelDTO.ID = objMasterDTO.ID.ToString();
            objLabelDTO.DefaultPullQuantity = objMasterDTO.DefaultPullQuantity.GetValueOrDefault(0).ToString(_QtyDecimalFormat);
            objLabelDTO.DefaultReorderQuantity = objMasterDTO.DefaultReorderQuantity.GetValueOrDefault(0).ToString(_QtyDecimalFormat);
            objLabelDTO.BinDefaultPullQuantity = objMasterDTO.BinDefaultPullQuantity.GetValueOrDefault(0).ToString(_QtyDecimalFormat);
            objLabelDTO.BinDefaultReorderQuantity = objMasterDTO.BinDefaultReorderQuantity.GetValueOrDefault(0).ToString(_QtyDecimalFormat);
            objLabelDTO.HardCodeQuantity = "1";
            objLabelDTO.ItemUniqueNumber = objMasterDTO.ItemUniqueNumber;
            objLabelDTO.ImagePath = GetImageLogoPath(objMasterDTO.ID);// objMasterDTO.ImagePath;
            objLabelDTO.StagingLocation = stagingLocation;
            objLabelDTO.UNSPSC = objMasterDTO.UNSPSC;
            objLabelDTO.ItemType = objMasterDTO.ItemType.ToString();
            objLabelDTO.UPCNumber = objMasterDTO.UPC;
            objLabelDTO.UOM = objMasterDTO.Unit;
            objLabelDTO.SupplierLogo = GetSupplierLogoPath(objMasterDTO.SupplierID.GetValueOrDefault(0));
            objLabelDTO.ItemTrackingType = "";
            if (objMasterDTO.SerialNumberTracking)
                objLabelDTO.ItemTrackingType = "Serial";
            else if (objMasterDTO.LotNumberTracking)
                objLabelDTO.ItemTrackingType = "Lot";
            objLabelDTO.CostUOM = objMasterDTO.CostUOMName;
            objLabelDTO.UDF6 = objMasterDTO.UDF6;
            objLabelDTO.UDF7 = objMasterDTO.UDF7;
            objLabelDTO.UDF8 = objMasterDTO.UDF8;
            objLabelDTO.UDF9 = objMasterDTO.UDF9;
            objLabelDTO.UDF10 = objMasterDTO.UDF10;

            if (objMasterDTO.SerialNumberTracking)
            {
                objLabelDTO.SerialNumberTracking = "Yes";
            }
            else
            {
                objLabelDTO.SerialNumberTracking = "No";
            }
            if (objMasterDTO.LotNumberTracking)
            {
                objLabelDTO.LotNumberTracking = "Yes";
            }
            else
            {
                objLabelDTO.LotNumberTracking = "No";
            }
            if (objMasterDTO.DateCodeTracking)
            {
                objLabelDTO.DateCodeTracking = "Yes";
            }
            else
            {
                objLabelDTO.DateCodeTracking = "No";
            }

            return objLabelDTO;
        }

        public InventoryLabelDTO GetInvertoryLabelDTOLimited(ItemMasterDTO objMasterDTO, List<MaterialStagingDetailDTO> objStagingDTO, List<SupplierMasterDTO> supplierlist)
        {
            InventoryLabelDTO objLabelDTO = new InventoryLabelDTO();
            //IEnumerable<MaterialStagingDetailDTO> objStagingDTO = new MaterialStagingDetailDAL(SessionHelper.EnterPriseDBName).GetStagingLocationByItem(objMasterDTO.GUID, objMasterDTO.Room.GetValueOrDefault(0), objMasterDTO.CompanyID.GetValueOrDefault(0)).Where(x => x.Quantity > 0);

            string stagingLocation = string.Empty;
            if (objStagingDTO != null && objStagingDTO.Count() > 0)
            {
                stagingLocation = objStagingDTO.FirstOrDefault().MaterialStagingName + " - " + objStagingDTO.FirstOrDefault().StagingBinName;
            }

            objLabelDTO.BinNumber = objMasterDTO.DefaultLocationName;
            objLabelDTO.Category = objMasterDTO.CategoryName;
            if (!string.IsNullOrWhiteSpace(objMasterDTO.BinNumber))
                objLabelDTO.BinNumber = objMasterDTO.BinNumber;

            objLabelDTO.Cost = objMasterDTO.Cost.GetValueOrDefault(0).ToString(_CostDecimalFormat);
            objLabelDTO.ItemCriticalQuantity = objMasterDTO.CriticalQuantity.GetValueOrDefault(0).ToString(_QtyDecimalFormat);
            objLabelDTO.Description = objMasterDTO.Description;
            objLabelDTO.GLAccount = objMasterDTO.GLAccount;
            objLabelDTO.ItemNumber = objMasterDTO.ItemNumber;
            objLabelDTO.LongDescription = objMasterDTO.LongDescription;
            objLabelDTO.ManufacturerNumber = objMasterDTO.ManufacturerNumber;
            objLabelDTO.Manufacturer = objMasterDTO.ManufacturerName;
            objLabelDTO.ItemMaximumQuantity = objMasterDTO.MaximumQuantity.GetValueOrDefault(0).ToString(_QtyDecimalFormat);
            objLabelDTO.ItemMinimumQuantity = objMasterDTO.MinimumQuantity.GetValueOrDefault(0).ToString(_QtyDecimalFormat);
            //objLabelDTO.PackageQuantity = objMasterDTO.PackingQuantity.GetValueOrDefault(0).ToString(_QtyDecimalFormat);
            objLabelDTO.Picture = "";
            objLabelDTO.SellPrice = objMasterDTO.SellPrice.GetValueOrDefault(0).ToString(_CostDecimalFormat);
            objLabelDTO.Staging = objMasterDTO.StagedQuantity.GetValueOrDefault(0).ToString(_QtyDecimalFormat);
            objLabelDTO.Supplier = objMasterDTO.SupplierName;
            objLabelDTO.SupplierNumber = objMasterDTO.SupplierPartNo;
            //objLabelDTO.Taxable = objMasterDTO.Taxable ? "True" : "False";
            objLabelDTO.UDF1 = objMasterDTO.UDF1;
            objLabelDTO.UDF2 = objMasterDTO.UDF2;
            objLabelDTO.UDF3 = objMasterDTO.UDF3;
            objLabelDTO.UDF4 = objMasterDTO.UDF4;
            objLabelDTO.UDF5 = objMasterDTO.UDF5;
            objLabelDTO.ID = objMasterDTO.ID.ToString();
            objLabelDTO.DefaultPullQuantity = objMasterDTO.DefaultPullQuantity.GetValueOrDefault(0).ToString(_QtyDecimalFormat);
            objLabelDTO.DefaultReorderQuantity = objMasterDTO.DefaultReorderQuantity.GetValueOrDefault(0).ToString(_QtyDecimalFormat);
            objLabelDTO.BinDefaultPullQuantity = objMasterDTO.BinDefaultPullQuantity.GetValueOrDefault(0).ToString(_QtyDecimalFormat);
            objLabelDTO.BinDefaultReorderQuantity = objMasterDTO.BinDefaultReorderQuantity.GetValueOrDefault(0).ToString(_QtyDecimalFormat);
            objLabelDTO.HardCodeQuantity = "1";
            objLabelDTO.ItemUniqueNumber = objMasterDTO.ItemUniqueNumber;
            if ((!string.IsNullOrEmpty(objMasterDTO.ImageType)) && objMasterDTO.ImageType.Trim().ToLower() == "imagepath")
            {
                objLabelDTO.ImagePath = Server.MapPath("..\\Uploads\\InventoryPhoto\\" + SessionHelper.EnterPriceID + "\\" + SessionHelper.CompanyID + "\\" + SessionHelper.RoomID + "\\" + objMasterDTO.ID + "\\" + objMasterDTO.ImagePath);//GetImageLogoPath(objMasterDTO.ID);// objMasterDTO.ImagePath;
            }
            else if ((!string.IsNullOrEmpty(objMasterDTO.ImageType)) && objMasterDTO.ImageType.Trim().ToLower() == "externalimage")
            {
                objLabelDTO.ImagePath = objMasterDTO.ItemImageExternalURL;
            }

            objLabelDTO.StagingLocation = stagingLocation;
            objLabelDTO.UNSPSC = objMasterDTO.UNSPSC;
            objLabelDTO.ItemType = objMasterDTO.ItemType.ToString();
            objLabelDTO.UPCNumber = objMasterDTO.UPC;
            objLabelDTO.UOM = objMasterDTO.Unit;
            var itemSupplier = supplierlist.Find(x => x.ID == objMasterDTO.SupplierID.GetValueOrDefault(0));
            if (itemSupplier != null && !string.IsNullOrEmpty(itemSupplier.ImageType))
            {
                if (itemSupplier.ImageType.Trim().ToLower() == "imagepath")
                {
                    objLabelDTO.SupplierLogo = Server.MapPath("..\\Uploads\\SupplierLogos\\" + SessionHelper.EnterPriceID + "\\" + SessionHelper.CompanyID + "\\" + SessionHelper.RoomID + "\\" + objMasterDTO.SupplierID.GetValueOrDefault(0) + "\\" + itemSupplier.SupplierImage);
                }
                else if (itemSupplier.ImageType.Trim().ToLower() == "externalimage")
                {
                    objLabelDTO.SupplierLogo = itemSupplier.ImageExternalURL;
                }

            }
            //objLabelDTO.SupplierLogo = GetSupplierLogoPath(objMasterDTO.SupplierID.GetValueOrDefault(0));
            objLabelDTO.ItemTrackingType = "";
            if (objMasterDTO.SerialNumberTracking)
                objLabelDTO.ItemTrackingType = "Serial";
            else if (objMasterDTO.LotNumberTracking)
                objLabelDTO.ItemTrackingType = "Lot";
            objLabelDTO.CostUOM = objMasterDTO.CostUOMName;
            objLabelDTO.UDF6 = objMasterDTO.UDF6;
            objLabelDTO.UDF7 = objMasterDTO.UDF7;
            objLabelDTO.UDF8 = objMasterDTO.UDF8;
            objLabelDTO.UDF9 = objMasterDTO.UDF9;
            objLabelDTO.UDF10 = objMasterDTO.UDF10;

            if (objMasterDTO.SerialNumberTracking)
            {
                objLabelDTO.SerialNumberTracking = "Yes";
            }
            else
            {
                objLabelDTO.SerialNumberTracking = "No";
            }
            if (objMasterDTO.LotNumberTracking)
            {
                objLabelDTO.LotNumberTracking = "Yes";
            }
            else
            {
                objLabelDTO.LotNumberTracking = "No";
            }
            if (objMasterDTO.DateCodeTracking)
            {
                objLabelDTO.DateCodeTracking = "Yes";
            }
            else
            {
                objLabelDTO.DateCodeTracking = "No";
            }

            return objLabelDTO;
        }


        public KittingLabelDTO GetKittingLabelDTO(KitMasterDTO objMasterDTO)
        {
            KittingLabelDTO objLabelDTO = new KittingLabelDTO();
            objLabelDTO.KitPartNumber = objMasterDTO.KitPartNumber;
            objLabelDTO.Description = objMasterDTO.Description;
            objLabelDTO.KitMaximumQuantity = objMasterDTO.MaximumKitQuantity.GetValueOrDefault(0).ToString(_QtyDecimalFormat);
            objLabelDTO.KitMaximumQuantity = objMasterDTO.MinimumKitQuantity.GetValueOrDefault(0).ToString(_QtyDecimalFormat);
            objLabelDTO.UDF1 = objMasterDTO.UDF1;
            objLabelDTO.UDF2 = objMasterDTO.UDF2;
            objLabelDTO.UDF3 = objMasterDTO.UDF3;
            objLabelDTO.UDF4 = objMasterDTO.UDF4;
            objLabelDTO.UDF5 = objMasterDTO.UDF5;
            objLabelDTO.ID = objMasterDTO.ID.ToString();

            return objLabelDTO;
        }

        public List<OrderLabelDTO> GetOrderLabelDTO(OrderMasterDTO objMasterDTO)
        {
            List<OrderLabelDTO> lstLabelDTO = new List<OrderLabelDTO>();
            OrderDetailsDAL objDetailDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
            List<OrderDetailsDTO> objOrderDetailDTO = objDetailDAL.GetOrderDetailByOrderGUIDFullWithSupplierFilter(objMasterDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID,false,SessionHelper.UserSupplierIds);

            foreach (var item in objOrderDetailDTO)
            {
                OrderLabelDTO objLabelDTO = new OrderLabelDTO();
                objLabelDTO.BinNumber = item.BinName;
                objLabelDTO.Cost = item.Cost.GetValueOrDefault(0).ToString(_CostDecimalFormat);
                objLabelDTO.GLAccount = item.GLAccount;
                objLabelDTO.Manufacturer = item.Manufacturer;
                objLabelDTO.ManufacturerNumber = item.ManufacturerNumber;
                objLabelDTO.ItemMaximumQuantity = item.MaximumQuantity.ToString(_QtyDecimalFormat);
                objLabelDTO.ItemMinimumQuantity = item.MinimumQuantity.ToString(_QtyDecimalFormat);
                objLabelDTO.ItemNumber = item.ItemNumber;
                objLabelDTO.PackageQuantity = item.PackingQuantity.GetValueOrDefault(0).ToString(_QtyDecimalFormat);
                objLabelDTO.RequestedQuantity = item.RequestedQuantity.GetValueOrDefault(0).ToString(_QtyDecimalFormat);
                objLabelDTO.SellPrice = item.SellPrice.GetValueOrDefault(0).ToString(_CostDecimalFormat);
                objLabelDTO.Supplier = item.SupplierName;
                objLabelDTO.SupplierNumber = item.SupplierPartNo;
                objLabelDTO.UOM = item.Unit;
                objLabelDTO.StagingName = objMasterDTO.StagingName;
                objLabelDTO.ReleaseNumber = objMasterDTO.ReleaseNumber;
                objLabelDTO.OrderNumber = objMasterDTO.OrderNumber;
                objLabelDTO.Description = objMasterDTO.Comment;
                objLabelDTO.UDF1 = objMasterDTO.UDF1;
                objLabelDTO.UDF2 = objMasterDTO.UDF2;
                objLabelDTO.UDF3 = objMasterDTO.UDF3;
                objLabelDTO.UDF4 = objMasterDTO.UDF4;
                objLabelDTO.UDF5 = objMasterDTO.UDF5;
                objLabelDTO.ID = objMasterDTO.ID.ToString();
                objLabelDTO.DefaultPullQuantity = item.DefaultPullQuantity.ToString(_QtyDecimalFormat);
                objLabelDTO.DefaultReorderQuantity = item.DefaultReorderQuantity.ToString(_QtyDecimalFormat);
                objLabelDTO.HardCodeQuantity = "1";
                objLabelDTO.ImagePath = GetImageLogoPath(item.ItemGUID);//item.ImagePath;
                objLabelDTO.ItemUniqueNumber = item.ItemUniqueNumber;
                objLabelDTO.UPCNumber = item.UPC;
                objLabelDTO.UOM = item.Unit;
                objLabelDTO.SupplierLogo = GetSupplierLogoPath(item.SupplierID.GetValueOrDefault(0));
                lstLabelDTO.Add(objLabelDTO);
            }

            return lstLabelDTO;
        }


        public ReceiveLabelDTO GetReceivedLabelDTO(ReceivedOrderTransferDetailDTO item)
        {
            OrderMasterDAL objMasterDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);

            ItemMasterDTO objItemDTO = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemWithMasterTableJoins(null, item.ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
            ReceiveLabelDTO objLabelDTO = new ReceiveLabelDTO();
            objLabelDTO.BinNumber = item.BinNumber;
            objLabelDTO.Cost = item.Cost.GetValueOrDefault(0).ToString(_CostDecimalFormat);
            objLabelDTO.GLAccount = objItemDTO.GLAccount;
            objLabelDTO.Manufacturer = objItemDTO.ManufacturerName;
            objLabelDTO.ManufacturerNumber = objItemDTO.ManufacturerNumber;
            objLabelDTO.ItemMaximumQuantity = objItemDTO.MaximumQuantity.GetValueOrDefault(0).ToString(_QtyDecimalFormat);
            objLabelDTO.ItemMinimumQuantity = objItemDTO.MinimumQuantity.GetValueOrDefault(0).ToString(_QtyDecimalFormat);
            objLabelDTO.ItemNumber = item.ItemNumber;
            //objLabelDTO.PackageQuantity = item.PackingQuantity.ToString(_QtyDecimalFormat);
            objLabelDTO.QuantityReceived = (item.CustomerOwnedQuantity.GetValueOrDefault(0) + item.ConsignedQuantity.GetValueOrDefault(0)).ToString(_QtyDecimalFormat);
            objLabelDTO.SellPrice = objItemDTO.SellPrice.GetValueOrDefault(0).ToString(_CostDecimalFormat);
            objLabelDTO.Supplier = objItemDTO.SupplierName;
            objLabelDTO.SupplierNumber = objItemDTO.SupplierPartNo;
            objLabelDTO.UOM = objItemDTO.Unit;
            objLabelDTO.Description = objItemDTO.Description;
            //objLabelDTO.UDF1 = item.UDF1;
            //objLabelDTO.UDF2 = item.UDF2;
            //objLabelDTO.UDF3 = item.UDF3;
            //objLabelDTO.UDF4 = item.UDF4;
            //objLabelDTO.UDF5 = item.UDF5;
            objLabelDTO.UDF1 = objItemDTO.UDF1;
            objLabelDTO.UDF2 = objItemDTO.UDF2;
            objLabelDTO.UDF3 = objItemDTO.UDF3;
            objLabelDTO.UDF4 = objItemDTO.UDF4;
            objLabelDTO.UDF5 = objItemDTO.UDF5;

            objLabelDTO.DateCode = Convert.ToString(item.ExpirationDate);
            objLabelDTO.LotNumber = item.LotNumber;
            objLabelDTO.SerialNumber = item.SerialNumber;
            objLabelDTO.PurchaseOrTransfer = objItemDTO.IsPurchase ? "Yes" : "No";

            objLabelDTO.ReceivedDate = FnCommon.ConvertDateByTimeZone(item.ReceivedDate, true, false); //Convert.ToString(item.ReceivedDate);

            objLabelDTO.DefaultPullQuantity = objItemDTO.DefaultPullQuantity.GetValueOrDefault(0).ToString(_QtyDecimalFormat);
            objLabelDTO.DefaultReorderQuantity = objItemDTO.DefaultReorderQuantity.GetValueOrDefault(0).ToString(_QtyDecimalFormat);
            objLabelDTO.HardCodeQuantity = "1";
            objLabelDTO.ImagePath = GetImageLogoPath(objItemDTO.ID);// objItemDTO.ImagePath;
            objLabelDTO.ItemUniqueNumber = objItemDTO.ItemUniqueNumber;
            objLabelDTO.UPCNumber = objItemDTO.UPC;
            objLabelDTO.UOM = objItemDTO.Unit;

            return objLabelDTO;
        }


        public ReceiveLabelDTO GetReceiveLabelDTO(ReceivableItemDTO item)
        {
            OrderMasterDAL objMasterDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
            OrderMasterDTO objMasterDTO = objMasterDAL.GetOrderByGuidPlain(item.OrderGUID);
            ReceivedOrderTransferDetailDAL ROTDDL = new ReceivedOrderTransferDetailDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<ReceivedOrderTransferDetailDTO> lstOrdTrfDtlDTO = ROTDDL.GetROTDByOrderDetailGUIDPlain(item.OrderDetailGUID, SessionHelper.RoomID, SessionHelper.CompanyID).OrderByDescending(x => x.ID).ToList();
            ItemMasterDTO objItemDTO = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemWithMasterTableJoins(null, item.ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
            ReceiveLabelDTO objLabelDTO = new ReceiveLabelDTO();
            objLabelDTO.BinNumber = item.ReceiveBinName;
            objLabelDTO.Cost = item.ItemCost.ToString(_CostDecimalFormat);
            objLabelDTO.GLAccount = objItemDTO.GLAccount;
            objLabelDTO.Manufacturer = item.Manufacturer;
            objLabelDTO.ManufacturerNumber = item.ManufacturerNumber;
            objLabelDTO.ItemMaximumQuantity = objItemDTO.MaximumQuantity.GetValueOrDefault(0).ToString(_QtyDecimalFormat);
            objLabelDTO.ItemMinimumQuantity = objItemDTO.MinimumQuantity.GetValueOrDefault(0).ToString(_QtyDecimalFormat);
            objLabelDTO.ItemNumber = item.ItemNumber;
            objLabelDTO.PackageQuantity = item.PackingQuantity.ToString(_QtyDecimalFormat);
            objLabelDTO.QuantityReceived = item.ReceivedQuantity.ToString(_QtyDecimalFormat);
            objLabelDTO.SellPrice = objItemDTO.SellPrice.GetValueOrDefault(0).ToString(_CostDecimalFormat);
            objLabelDTO.Supplier = item.SupplierName;
            objLabelDTO.SupplierNumber = item.SupplierPartNumber;
            objLabelDTO.UOM = item.UnitName;
            objLabelDTO.Description = item.ItemDescription;

            objLabelDTO.UDF1 = objItemDTO.UDF1;
            objLabelDTO.UDF2 = objItemDTO.UDF2;
            objLabelDTO.UDF3 = objItemDTO.UDF3;
            objLabelDTO.UDF4 = objItemDTO.UDF4;
            objLabelDTO.UDF5 = objItemDTO.UDF5;

            objLabelDTO.PurchaseOrTransfer = objItemDTO.IsPurchase ? "Yes" : "No";
            if (lstOrdTrfDtlDTO != null && lstOrdTrfDtlDTO.Count() > 0)
            {
                objLabelDTO.ReceivedDate = lstOrdTrfDtlDTO.FirstOrDefault().Received;
            }

            objLabelDTO.ID = objMasterDTO.ID.ToString();
            objLabelDTO.DefaultPullQuantity = objItemDTO.DefaultPullQuantity.GetValueOrDefault(0).ToString(_QtyDecimalFormat);
            objLabelDTO.DefaultReorderQuantity = objItemDTO.DefaultReorderQuantity.GetValueOrDefault(0).ToString(_QtyDecimalFormat);
            objLabelDTO.HardCodeQuantity = "1";
            objLabelDTO.ImagePath = GetImageLogoPath(objItemDTO.ID);// objItemDTO.ImagePath;
            objLabelDTO.ItemUniqueNumber = objItemDTO.ItemUniqueNumber;
            objLabelDTO.UPCNumber = objItemDTO.UPC;
            objLabelDTO.UOM = objItemDTO.Unit;
            return objLabelDTO;
        }

        public QuickListLabelDTO GetQuickListLabelDTO(QuickListMasterDTO objMasterDTO)
        {
            QuickListLabelDTO objLabelDTO = new QuickListLabelDTO();

            objLabelDTO.Comment = objMasterDTO.Comment;
            objLabelDTO.QuickListName = objMasterDTO.Name;
            objLabelDTO.UDF1 = objMasterDTO.UDF1;
            objLabelDTO.UDF2 = objMasterDTO.UDF2;
            objLabelDTO.UDF3 = objMasterDTO.UDF3;
            objLabelDTO.UDF4 = objMasterDTO.UDF4;
            objLabelDTO.UDF5 = objMasterDTO.UDF5;
            objLabelDTO.ID = objMasterDTO.ID.ToString();


            return objLabelDTO;
        }

        public ToolLabelDTO GetToolLabelDTO(ToolMasterDTO objMasterDTO)
        {
            ToolLabelDTO objLabelDTO = new ToolLabelDTO();
            objLabelDTO.ToolName = objMasterDTO.ToolName;
            objLabelDTO.Description = objMasterDTO.Description;
            objLabelDTO.SerialNumber = objMasterDTO.Serial;
            objLabelDTO.Category = objMasterDTO.ToolCategory;
            objLabelDTO.Location = objMasterDTO.Location;
            objLabelDTO.GroupOfTool = objMasterDTO.IsGroupOfItems.GetValueOrDefault(0) == 0 ? "No" : "Yes";
            objLabelDTO.Quantity = objMasterDTO.Quantity.ToString(_QtyDecimalFormat);
            objLabelDTO.Cost = objMasterDTO.Cost.HasValue ? objMasterDTO.Cost.Value.ToString(_CostDecimalFormat) : string.Empty;
            objLabelDTO.UDF1 = objMasterDTO.UDF1;
            objLabelDTO.UDF2 = objMasterDTO.UDF2;
            objLabelDTO.UDF3 = objMasterDTO.UDF3;
            objLabelDTO.UDF4 = objMasterDTO.UDF4;
            objLabelDTO.UDF5 = objMasterDTO.UDF5;
            objLabelDTO.ID = objMasterDTO.ID.ToString();
            objLabelDTO.ToolImage = GetToolImagePath(objMasterDTO.ID, objMasterDTO.GUID);// objMasterDTO.ImagePath;
            objLabelDTO.ImageType = objMasterDTO.ImageType;

            return objLabelDTO;
        }

        public List<StagingLabelDTO> GetStagingLabelDTO(MaterialStagingDTO objMasterDTO)
        {
            List<StagingLabelDTO> lstLabelDTO = new List<StagingLabelDTO>();
            MaterialStagingDetailDAL objDetailDAL = new MaterialStagingDetailDAL(SessionHelper.EnterPriseDBName);
            //IEnumerable<MaterialStagingDetailDTO> lstDetailDTO = objDetailDAL.GetAllRecords(objMasterDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID);
            IEnumerable<MaterialStagingDetailDTO> lstDetailDTO = objDetailDAL.GetMaterialStagingDetailbyItemGUIDStagingBINID(Convert.ToString(objMasterDTO.GUID), string.Empty, null, SessionHelper.RoomID, SessionHelper.CompanyID, null, null);

            foreach (var item in lstDetailDTO)
            {
                StagingLabelDTO objLabelDTO = new StagingLabelDTO();
                item.ItemDetail = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemWithMasterTableJoins(null, item.ItemGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                if (item.ItemDetail != null && SessionHelper.UserSupplierIds != null && SessionHelper.UserSupplierIds.Count >0)
                {
                    if(!SessionHelper.UserSupplierIds.Contains(item.ItemDetail.SupplierID ?? 0))
                    {
                        continue;
                    }
                }
                objLabelDTO.BinNumber = item.BinName;
                objLabelDTO.Location = item.StagingBinName;
                objLabelDTO.Cost = item.ItemDetail.Cost.GetValueOrDefault(0).ToString(_CostDecimalFormat);
                objLabelDTO.GLAccount = item.ItemDetail.GLAccount;
                objLabelDTO.Manufacturer = item.ItemDetail.ManufacturerName;
                objLabelDTO.ManufacturerNumber = item.ItemDetail.ManufacturerNumber;
                objLabelDTO.ItemMaximumQuantity = item.ItemDetail.MaximumQuantity.GetValueOrDefault(0).ToString(_QtyDecimalFormat);
                objLabelDTO.ItemMinimumQuantity = item.ItemDetail.MinimumQuantity.GetValueOrDefault(0).ToString(_QtyDecimalFormat);
                objLabelDTO.ItemNumber = item.ItemDetail.ItemNumber;
                objLabelDTO.PackageQuantity = item.ItemDetail.PackingQuantity.GetValueOrDefault(0).ToString(_QtyDecimalFormat);
                objLabelDTO.SellPrice = item.ItemDetail.SellPrice.GetValueOrDefault(0).ToString(_CostDecimalFormat);
                objLabelDTO.Supplier = item.ItemDetail.SupplierName;
                objLabelDTO.SupplierNumber = item.ItemDetail.SupplierPartNo;
                objLabelDTO.UOM = item.ItemDetail.Unit;
                objLabelDTO.StagingName = objMasterDTO.StagingName;
                objLabelDTO.Description = item.ItemDetail.Description;
                objLabelDTO.UDF1 = objMasterDTO.UDF1;
                objLabelDTO.UDF2 = objMasterDTO.UDF2;
                objLabelDTO.UDF3 = objMasterDTO.UDF3;
                objLabelDTO.UDF4 = objMasterDTO.UDF4;
                objLabelDTO.UDF5 = objMasterDTO.UDF5;
                objLabelDTO.ID = objMasterDTO.ID.ToString();
                objLabelDTO.Comment = objMasterDTO.Description;
                objLabelDTO.DefaultPullQuantity = item.ItemDetail.DefaultPullQuantity.GetValueOrDefault(0).ToString(_QtyDecimalFormat);
                objLabelDTO.DefaultReorderQuantity = item.ItemDetail.DefaultReorderQuantity.GetValueOrDefault(0).ToString(_QtyDecimalFormat);
                objLabelDTO.HardCodeQuantity = "1";
                objLabelDTO.ImagePath = GetImageLogoPath(item.ItemDetail.ID); //item.ItemDetail.ImagePath;
                objLabelDTO.SupplierLogo = GetSupplierLogoPath(item.ItemDetail.SupplierID.GetValueOrDefault());
                objLabelDTO.ItemUniqueNumber = item.ItemDetail.ItemUniqueNumber;
                objLabelDTO.UPCNumber = item.ItemDetail.UPC;
                objLabelDTO.UOM = item.ItemDetail.Unit;

                lstLabelDTO.Add(objLabelDTO);
            }



            return lstLabelDTO;
        }

        public List<TransferLabelDTO> GetTransferLabelDTO(TransferMasterDTO objMasterDTO)
        {
            List<TransferLabelDTO> lstLabelDTO = new List<TransferLabelDTO>();
            TransferDetailDAL objDetailDAL = new TransferDetailDAL(SessionHelper.EnterPriseDBName);
            TransferInOutQtyDetailDAL InOutQtyDetaiDAL = new TransferInOutQtyDetailDAL(SessionHelper.EnterPriseDBName);
            string ExpirationDates = string.Empty;
            string SerialNumbers = string.Empty;
            string LotNumbers = string.Empty;
            IEnumerable<TransferDetailDTO> lstDetailDTO = objDetailDAL.GetTransferedRecord(objMasterDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false, SessionHelper.UserSupplierIds);
            foreach (var item in lstDetailDTO)
            {
                TransferLabelDTO objLabelDTO = new TransferLabelDTO();
                item.ItemDetail = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemWithMasterTableJoins(null, item.ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                objLabelDTO.BinNumber = item.BinName;
                objLabelDTO.Cost = item.ItemDetail.Cost.GetValueOrDefault(0).ToString(_CostDecimalFormat);
                objLabelDTO.GLAccount = item.ItemDetail.GLAccount;
                objLabelDTO.Manufacturer = item.ItemDetail.ManufacturerName;
                objLabelDTO.ManufacturerNumber = item.ItemDetail.ManufacturerNumber;
                objLabelDTO.ItemMaximumQuantity = item.ItemDetail.MaximumQuantity.GetValueOrDefault(0).ToString(_QtyDecimalFormat);
                objLabelDTO.ItemMinimumQuantity = item.ItemDetail.MinimumQuantity.GetValueOrDefault(0).ToString(_QtyDecimalFormat);
                objLabelDTO.ItemNumber = item.ItemDetail.ItemNumber;
                objLabelDTO.PackageQuantity = item.ItemDetail.PackingQuantity.GetValueOrDefault(0).ToString(_QtyDecimalFormat);
                objLabelDTO.SellPrice = item.ItemDetail.SellPrice.GetValueOrDefault(0).ToString(_CostDecimalFormat);
                objLabelDTO.Supplier = item.ItemDetail.SupplierName;
                objLabelDTO.SupplierNumber = item.ItemDetail.SupplierPartNo;
                objLabelDTO.UOM = item.ItemDetail.Unit;
                objLabelDTO.StagingName = objMasterDTO.StagingName;
                objLabelDTO.Description = item.ItemDetail.Description;
                objLabelDTO.UDF1 = objMasterDTO.UDF1;
                objLabelDTO.UDF2 = objMasterDTO.UDF2;
                objLabelDTO.UDF3 = objMasterDTO.UDF3;
                objLabelDTO.UDF4 = objMasterDTO.UDF4;
                objLabelDTO.UDF5 = objMasterDTO.UDF5;
                objLabelDTO.ID = objMasterDTO.ID.ToString();
                objLabelDTO.Description = objMasterDTO.Comment;
                objLabelDTO.TransferNumber = objMasterDTO.TransferNumber;
                objLabelDTO.ImagePath = GetImageLogoPath(item.ItemDetail.ID); //item.ItemDetail.ImagePath;
                objLabelDTO.SupplierLogo = GetSupplierLogoPath(item.ItemDetail.SupplierID.GetValueOrDefault());
                objLabelDTO.ItemUniqueNumber = item.ItemDetail.ItemUniqueNumber;
                objLabelDTO.UPCNumber = item.ItemDetail.UPC;
                objLabelDTO.UOM = item.ItemDetail.Unit;
                objLabelDTO.RepleneshingStockRoomName = objMasterDTO.ReplenishingRoomName;
                objLabelDTO.RequestingStockRoomName = objMasterDTO.RequestingRoomName;
                objLabelDTO.Replenishinglocation = item.BinName;
                objLabelDTO.Requestinglocation = item.DestinationBin;
                objLabelDTO.ItemUDF1 = item.ItemDetail.UDF1;
                objLabelDTO.ItemUDF2 = item.ItemDetail.UDF2;
                objLabelDTO.ItemUDF3 = item.ItemDetail.UDF3;
                objLabelDTO.ItemUDF4 = item.ItemDetail.UDF4;
                objLabelDTO.ItemUDF5 = item.ItemDetail.UDF5;
                objLabelDTO.ItemUDF6 = item.ItemDetail.UDF6;
                objLabelDTO.ItemUDF7 = item.ItemDetail.UDF7;
                objLabelDTO.ItemUDF8 = item.ItemDetail.UDF8;
                objLabelDTO.ItemUDF9 = item.ItemDetail.UDF9;
                objLabelDTO.ItemUDF10 = item.ItemDetail.UDF10;
                var InOutQtyDetails = InOutQtyDetaiDAL.GetTransferInOutQtyDetail(item.GUID);
                foreach (var itemQtyDetail in InOutQtyDetails)
                {
                    objLabelDTO.Requestinglocation = itemQtyDetail.BinNumber;
                    if (itemQtyDetail.ExpirationDate != null)
                        ExpirationDates += itemQtyDetail.ExpirationDate.Value.Date.ToShortDateString() + ", ";

                    if (!string.IsNullOrEmpty(itemQtyDetail.SerialNumber))
                        SerialNumbers += itemQtyDetail.SerialNumber + ", ";

                    if (!string.IsNullOrEmpty(itemQtyDetail.LotNumber))
                        LotNumbers += itemQtyDetail.LotNumber + ", ";
 
                }
                if(!string.IsNullOrEmpty(ExpirationDates))
                objLabelDTO.ExpirationDateItem = ExpirationDates.Substring(0, ExpirationDates.Length - 2);
                if (!string.IsNullOrEmpty(SerialNumbers))
                    objLabelDTO.SerialNumberOfItem = SerialNumbers.Substring(0, SerialNumbers.Length - 2);
                if (!string.IsNullOrEmpty(LotNumbers))
                    objLabelDTO.LotNumberOfItem = LotNumbers.Substring(0, LotNumbers.Length - 2);
                if (InOutQtyDetails!= null && InOutQtyDetails.Count() > 0)
                objLabelDTO.Quantitytransferred = Convert.ToString(InOutQtyDetails.FirstOrDefault().TotalQty);

                lstLabelDTO.Add(objLabelDTO);
                ExpirationDates = string.Empty;
                SerialNumbers = string.Empty;
                LotNumbers = string.Empty;
            }



            return lstLabelDTO;
        }

        public TechnicianLabelDTO GetTechnicianLabelDTO(TechnicianMasterDTO objMasterDTO)
        {
            TechnicianLabelDTO objLabelDTO = new TechnicianLabelDTO();
            objLabelDTO.TechnicianName = objMasterDTO.Technician;
            objLabelDTO.TechnicianCode = objMasterDTO.TechnicianCode;
            objLabelDTO.TechnicianCodeName = objMasterDTO.TechnicianCode + "-" + objMasterDTO.Technician;
            objLabelDTO.UDF1 = objMasterDTO.UDF1;
            objLabelDTO.UDF2 = objMasterDTO.UDF2;
            objLabelDTO.UDF3 = objMasterDTO.UDF3;
            objLabelDTO.UDF4 = objMasterDTO.UDF4;
            objLabelDTO.UDF5 = objMasterDTO.UDF5;
            objLabelDTO.ID = objMasterDTO.ID.ToString();

            return objLabelDTO;
        }

        public LocationLabelDTO GetLocationLabelDTO(BinMasterDTO objMasterDTO)
        {
            LocationLabelDTO objLabelDTO = new LocationLabelDTO();
            objLabelDTO.Location = objMasterDTO.BinNumber;
            objLabelDTO.ID = objMasterDTO.ID.ToString();
            objLabelDTO.IsStagingLocation = objMasterDTO.IsStagingLocation == true ? "Yes" : "No";
            return objLabelDTO;
        }


        public List<WorkOrderLabelDTO> GetWorkOrderLabelDTO(WorkOrderDTO objMasterDTO)
        {
            //WorkOrderLabelDTO objLabelDTO = new WorkOrderLabelDTO();
            //objLabelDTO.Name = objMasterDTO.WOName;
            //objLabelDTO.ID = objMasterDTO.ID.ToString();
            //objLabelDTO.Asset = objMasterDTO.AssetName;
            //objLabelDTO.Tool = objMasterDTO.ToolName;
            //objLabelDTO.UDF1 = objMasterDTO.UDF1;
            //objLabelDTO.UDF2 = objMasterDTO.UDF2;
            //objLabelDTO.UDF3 = objMasterDTO.UDF3;
            //objLabelDTO.UDF4 = objMasterDTO.UDF4;
            //objLabelDTO.UDF5 = objMasterDTO.UDF5;
            //objLabelDTO.ProjectSpend = objMasterDTO.ProjectSpendName;
            //objLabelDTO.Technician = objMasterDTO.Technician;
            //objLabelDTO.Customer = objMasterDTO.Customer;
            //objLabelDTO.Description = objMasterDTO.Description;
            //objLabelDTO.GUID = objMasterDTO.GUID.ToString();


            //return objLabelDTO;

            List<WorkOrderLabelDTO> lstLabelDTO = new List<WorkOrderLabelDTO>();
            PullMasterDAL objDetailDAL = new PullMasterDAL(SessionHelper.EnterPriseDBName);
            List<PullMasterViewDTO> objWorkOrderDetailDTO = objDetailDAL.GetWorkOrderLineItemsByWOGUID(objMasterDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID);

            foreach (var item in objWorkOrderDetailDTO)
            {
                WorkOrderLabelDTO objLabelDTO = new WorkOrderLabelDTO();
                objLabelDTO.Name = objMasterDTO.WOName;
                objLabelDTO.ID = objMasterDTO.ID.ToString();
                objLabelDTO.Asset = objMasterDTO.AssetName;
                objLabelDTO.Tool = objMasterDTO.ToolName;
                objLabelDTO.UDF1 = objMasterDTO.UDF1;
                objLabelDTO.UDF2 = objMasterDTO.UDF2;
                objLabelDTO.UDF3 = objMasterDTO.UDF3;
                objLabelDTO.UDF4 = objMasterDTO.UDF4;
                objLabelDTO.UDF5 = objMasterDTO.UDF5;
                objLabelDTO.ProjectSpend = objMasterDTO.ProjectSpendName;
                objLabelDTO.Technician = objMasterDTO.Technician;
                objLabelDTO.Customer = objMasterDTO.Customer;
                objLabelDTO.Description = objMasterDTO.Description;
                objLabelDTO.GUID = objMasterDTO.GUID.ToString();


                objLabelDTO.ItemNumber = item.ItemNumber;
                objLabelDTO.BinNumber = item.BinNumber;
                objLabelDTO.PoolQuantity = item.PoolQuantity.GetValueOrDefault(0).ToString(_QtyDecimalFormat);

                objLabelDTO.PULLCost = item.PullCost.GetValueOrDefault(0).ToString(_CostDecimalFormat);
                objLabelDTO.PullPrice = item.PullPrice.GetValueOrDefault(0).ToString(_CostDecimalFormat);
                objLabelDTO.ItemMaximumQuantity = item.MaximumQuantity.ToString(_QtyDecimalFormat);
                objLabelDTO.ItemMinimumQuantity = item.MinimumQuantity.ToString(_QtyDecimalFormat);

                objLabelDTO.GLAccount = item.GLAccount;
                objLabelDTO.Manufacturer = item.Manufacturer;
                objLabelDTO.ManufacturerNumber = item.ManufacturerNumber;
                objLabelDTO.Supplier = item.SupplierName;
                objLabelDTO.SupplierNumber = item.SupplierPartNo;
               
                objLabelDTO.UOM = item.Unit;
                objLabelDTO.ItemImage = GetImageLogoPath(item.ItemGUID.GetValueOrDefault(Guid.Empty));
                objLabelDTO.ImagePath = GetImageLogoPath(item.ItemGUID.GetValueOrDefault(Guid.Empty));
                objLabelDTO.SupplierLogo = GetSupplierLogoPath(item.SupplierID.GetValueOrDefault(0));
                lstLabelDTO.Add(objLabelDTO);
            }

            return lstLabelDTO;
        }


        public ItemMasterDTO GetItemMasterDTOFromBinDTO(BinMasterDTO objMasterDTO, ItemMasterDTO objItemDTO)
        {
            ItemMasterDTO objLabelDTO = new ItemMasterDTO()
            {
                Action = objItemDTO.Action,
                ID = objItemDTO.ID,
                AppendedBarcodeString = objItemDTO.AppendedBarcodeString,
                AverageCost = objItemDTO.AverageCost,
                AverageUsage = objItemDTO.AverageUsage,
                BinID = objMasterDTO.ID,
                BinNumber = objItemDTO.BinNumber,
                BondedInventory = objItemDTO.BondedInventory,
                CategoryColor = objItemDTO.CategoryColor,
                CategoryID = objItemDTO.CategoryID,
                CategoryName = objItemDTO.CategoryName,
                CompanyID = objItemDTO.CompanyID,
                ConsignedQuantity = objItemDTO.ConsignedQuantity,
                Consignment = objItemDTO.Consignment,
                Cost = objItemDTO.Cost,
                CostUOMID = objItemDTO.CostUOMID,
                CostUOMName = objItemDTO.CostUOMName,
                CountConsignedQuantity = objItemDTO.CountConsignedQuantity,
                CountCustomerOwnedQuantity = objItemDTO.CountCustomerOwnedQuantity,
                CountLineItemDescriptionEntry = objItemDTO.CountLineItemDescriptionEntry,
                Created = objItemDTO.Created,
                CreatedBy = objItemDTO.CreatedBy,
                CreatedByName = objItemDTO.CreatedByName,
                CriticalQuantity = objItemDTO.CriticalQuantity,
                CustomerOwnedQuantity = objItemDTO.CustomerOwnedQuantity,
                DateCodeTracking = objItemDTO.DateCodeTracking,
                DefaultLocation = objItemDTO.DefaultLocation,
                DefaultLocationName = objItemDTO.DefaultLocationName,
                DefaultPullQuantity = objItemDTO.DefaultPullQuantity,
                ExtendedCost = objItemDTO.ExtendedCost,
                DefaultReorderQuantity = objItemDTO.DefaultReorderQuantity,
                Description = objItemDTO.Description,
                GLAccount = objItemDTO.GLAccount,
                GLAccountID = objItemDTO.GLAccountID,
                GUID = objItemDTO.GUID,
                HistoryID = objItemDTO.HistoryID,
                ImagePath = GetImageLogoPath(objItemDTO.ID), //objItemDTO.ImagePath,
                InTransitquantity = objItemDTO.InTransitquantity,
                InventoryClassification = objItemDTO.InventoryClassification,
                InventoryClassificationName = objItemDTO.InventoryClassificationName,
                InventryLocation = objItemDTO.InventryLocation,
                IsArchived = objItemDTO.IsArchived,
                IsAutoInventoryClassification = objItemDTO.IsAutoInventoryClassification,
                IsBOMItem = objItemDTO.IsBOMItem,
                IsBuildBreak = objItemDTO.IsBuildBreak,
                IsDeleted = objItemDTO.IsDeleted,
                IsEnforceDefaultReorderQuantity = objItemDTO.IsEnforceDefaultReorderQuantity,
                IsItemLevelMinMaxQtyRequired = objItemDTO.IsItemLevelMinMaxQtyRequired,
                IsLotSerialExpiryCost = objItemDTO.IsLotSerialExpiryCost,
                IsPurchase = objItemDTO.IsPurchase,
                IsTransfer = objItemDTO.IsTransfer,
                ItemLocations = objItemDTO.ItemLocations,
                ItemNumber = objItemDTO.ItemNumber,
                ItemsLocations = objItemDTO.ItemsLocations,
                ItemType = objItemDTO.ItemType,
                ItemTypeName = objItemDTO.ItemTypeName,
                ItemUDF1 = objItemDTO.ItemUDF1,
                ItemUDF2 = objItemDTO.ItemUDF2,
                ItemUDF3 = objItemDTO.ItemUDF3,
                ItemUDF4 = objItemDTO.ItemUDF4,
                ItemUDF5 = objItemDTO.ItemUDF5,
                ItemUniqueNumber = objItemDTO.ItemUniqueNumber,
                LastUpdatedBy = objItemDTO.LastUpdatedBy,
                LeadTimeInDays = objItemDTO.LeadTimeInDays,
                Link1 = objItemDTO.Link1,
                Link2 = objItemDTO.Link2,
                LongDescription = objItemDTO.LongDescription,
                LotNumberTracking = objItemDTO.LotNumberTracking,
                lstItemLocationQTY = objItemDTO.lstItemLocationQTY,
                ManufacturerID = objItemDTO.ManufacturerID,
                ManufacturerName = objItemDTO.ManufacturerName,
                ManufacturerNumber = objItemDTO.ManufacturerNumber,
                Markup = objItemDTO.Markup,
                MaximumQuantity = objItemDTO.MaximumQuantity,
                MinimumQuantity = objItemDTO.MinimumQuantity,
                MonthValue = objItemDTO.MonthValue,
                OnHandQuantity = objItemDTO.OnHandQuantity,
                OnOrderQuantity = objItemDTO.OnOrderQuantity,
                OnReturnQuantity = objItemDTO.OnReturnQuantity,
                OnTransferQuantity = objItemDTO.OnTransferQuantity,
                PackingQuantity = objItemDTO.PackingQuantity,
                //lstItemLocations = objItemDTO.ItemLocations,
                PricePerTerm = objItemDTO.PricePerTerm,
                PullQtyScanOverride = objItemDTO.PullQtyScanOverride,
                QuickListGUID = objItemDTO.QuickListGUID,
                QuickListItemQTY = objItemDTO.QuickListItemQTY,
                QuickListName = objItemDTO.QuickListName,
                Reason = objItemDTO.Reason,
                RefBomI = objItemDTO.RefBomI,
                RefBomId = objItemDTO.RefBomId,
                RequisitionedQuantity = objItemDTO.RequisitionedQuantity,
                Room = objItemDTO.Room,
                RoomName = objItemDTO.RoomName,
                RownumberCost = objItemDTO.RownumberCost,
                SellPrice = objItemDTO.SellPrice,
                SerialNumberTracking = objItemDTO.SerialNumberTracking,
                StagedQuantity = objItemDTO.StagedQuantity,
                Status = objItemDTO.Status,
                StockOutCount = objItemDTO.StockOutCount,
                SuggestedOrderQuantity = objItemDTO.SuggestedOrderQuantity,
                SuggestedTransferQuantity = objItemDTO.SuggestedTransferQuantity,
                SupplierID = objItemDTO.SupplierID,
                SupplierName = objItemDTO.SupplierName,
                SupplierPartNo = objItemDTO.SupplierPartNo,
                Taxable = objItemDTO.Taxable,
                Trend = objItemDTO.Trend,
                TrendingSetting = objItemDTO.TrendingSetting,
                Turns = objItemDTO.Turns,
                UDF1 = objItemDTO.UDF1,
                UDF2 = objItemDTO.UDF2,
                UDF3 = objItemDTO.UDF3,
                UDF4 = objItemDTO.UDF4,
                UDF5 = objItemDTO.UDF5,
                UDF6 = objItemDTO.UDF6,
                UDF7 = objItemDTO.UDF7,
                UDF8 = objItemDTO.UDF8,
                UDF9 = objItemDTO.UDF9,
                UDF10 = objItemDTO.UDF10,
                Unit = objItemDTO.Unit,
                UOMID = objItemDTO.UOMID,
                UNSPSC = objItemDTO.UNSPSC,
                UPC = objItemDTO.UPC,
                Updated = objItemDTO.Updated,
                UpdatedByName = objItemDTO.UpdatedByName,
                WeightPerPiece = objItemDTO.WeightPerPiece,
                WhatWhereAction = objItemDTO.WhatWhereAction,
                xmlItemLocations = objItemDTO.xmlItemLocations,
                OnOrderInTransitQuantity = objItemDTO.OnOrderInTransitQuantity,
                BinDefaultReorderQuantity =  objMasterDTO.DefaultReorderQuantity,
                BinDefaultPullQuantity = objMasterDTO.DefaultPullQuantity

            };



            objLabelDTO.BinNumber = objMasterDTO.BinNumber;
            objLabelDTO.DefaultLocationName = objMasterDTO.BinNumber;
            if (!objItemDTO.IsItemLevelMinMaxQtyRequired.GetValueOrDefault(false))
            {
                objLabelDTO.CriticalQuantity = objMasterDTO.CriticalQuantity.GetValueOrDefault(0);
                objLabelDTO.MaximumQuantity = objMasterDTO.MaximumQuantity.GetValueOrDefault(0);
                objLabelDTO.MinimumQuantity = objMasterDTO.MinimumQuantity.GetValueOrDefault(0);
            }
            return objLabelDTO;
        }


        public string GetSupplierLogoPath(Int64 SupplierID)
        {
            SupplierMasterDAL supplierDAL = null;
            SupplierMasterDTO supplier = null;
            string strPath = string.Empty;
            try
            {
                if (SupplierID > 0)
                {
                    supplierDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
                    supplier = supplierDAL.GetSupplierByIDPlain(SupplierID);
                    if (!string.IsNullOrEmpty(supplier.ImageType))
                    {
                        if (supplier.ImageType.Trim().ToLower() == "imagepath")
                        {
                            strPath = Server.MapPath("..\\Uploads\\SupplierLogos\\" + SessionHelper.EnterPriceID + "\\" + SessionHelper.CompanyID + "\\" + SessionHelper.RoomID + "\\" + SupplierID + "\\" + supplier.SupplierImage);
                        }
                        else if (supplier.ImageType.Trim().ToLower() == "externalimage")
                        {
                            strPath = supplier.ImageExternalURL;
                        }

                    }
                }
            }
            finally
            {
                supplierDAL = null;
                supplier = null;
            }



            return strPath;
        }

        public string GetImageLogoPath(Int64 ItemID)
        {
            ItemMasterDAL itemDAL = null;
            ItemMasterDTO item = null;
            string strPath = string.Empty;
            try
            {
                if (ItemID > 0)
                {
                    itemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                    item = itemDAL.GetItemWithoutJoins(ItemID, null);
                    if (!string.IsNullOrEmpty(item.ImageType))
                    {
                        if (item.ImageType.Trim().ToLower() == "imagepath")
                        {
                            strPath = item.ImagePath;
                            Int64 EnterpriseId = SessionHelper.EnterPriceID;
                            Int64 CompanyID = SessionHelper.CompanyID;
                            Int64 RoomID = SessionHelper.RoomID;
                            strPath = Server.MapPath("..\\Uploads\\InventoryPhoto\\" + EnterpriseId + "\\" + CompanyID + "\\" + RoomID + "\\" + item.ID + "\\" + strPath);
                        }
                        else if (item.ImageType.Trim().ToLower() == "externalimage")
                        {
                            strPath = item.ItemImageExternalURL;
                        }

                    }
                }
            }
            finally
            {
                itemDAL = null;
                item = null;
            }



            return strPath;
        }


        public string GetImageLogoPath(Guid? ItemGUID)
        {
            ItemMasterDAL itemDAL = null;
            ItemMasterDTO item = null;
            string strPath = string.Empty;
            try
            {
                if (ItemGUID != null)
                {
                    itemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                    item = itemDAL.GetItemWithoutJoins(null, ItemGUID.GetValueOrDefault(Guid.Empty));
                    if (!string.IsNullOrEmpty(item.ImageType))
                    {
                        if (item.ImageType.Trim().ToLower() == "imagepath")
                        {
                            strPath = item.ImagePath;
                            Int64 EnterpriseId = SessionHelper.EnterPriceID;
                            Int64 CompanyID = SessionHelper.CompanyID;
                            Int64 RoomID = SessionHelper.RoomID;
                            strPath = Server.MapPath("..\\Uploads\\InventoryPhoto\\" + EnterpriseId + "\\" + CompanyID + "\\" + RoomID + "\\" + item.ID + "\\" + strPath);
                        }
                        else if (item.ImageType.Trim().ToLower() == "externalimage")
                        {
                            strPath = item.ItemImageExternalURL;
                        }

                    }
                }
            }
            finally
            {
                itemDAL = null;
                item = null;
            }



            return strPath;
        }


        public string GetAssetImagePath(Int64 AssetID)
        {
            AssetMasterDAL itemDAL = null;
            AssetMasterDTO item = null;
            string strPath = string.Empty;
            try
            {
                if (AssetID > 0)
                {
                    itemDAL = new AssetMasterDAL(SessionHelper.EnterPriseDBName);
                    item = itemDAL.GetAssetById(AssetID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                    if (!string.IsNullOrEmpty(item.ImageType))
                    {
                        if (item.ImageType.Trim().ToLower() == "imagepath")
                        {
                            strPath = item.ImagePath;
                            Int64 EnterpriseId = SessionHelper.EnterPriceID;
                            Int64 CompanyID = SessionHelper.CompanyID;
                            Int64 RoomID = SessionHelper.RoomID;
                            strPath = Server.MapPath("..\\Uploads\\AssetPhoto\\" + EnterpriseId + "\\" + CompanyID + "\\" + RoomID + "\\" + item.ID + "\\" + strPath);
                        }
                        else if (item.ImageType.Trim().ToLower() == "externalimage")
                        {
                            strPath = item.AssetImageExternalURL;
                        }

                    }
                }
            }
            finally
            {
                itemDAL = null;
                item = null;
            }



            return strPath;
        }


        public string GetToolImagePath(Int64 ToolID, Guid ToolGuid)
        {
            ToolMasterDAL itemDAL = null;
            ToolMasterDTO item = null;
            string strPath = string.Empty;
            try
            {
                if (ToolID > 0)
                {
                    itemDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
                    item = itemDAL.GetToolByIDPlain(ToolID);
                    if (!string.IsNullOrEmpty(item.ImageType))
                    {
                        if (item.ImageType.Trim().ToLower() == "imagepath")
                        {
                            strPath = item.ImagePath;
                            Int64 EnterpriseId = SessionHelper.EnterPriceID;
                            Int64 CompanyID = SessionHelper.CompanyID;
                            Int64 RoomID = SessionHelper.RoomID;
                            strPath = Server.MapPath("..\\Uploads\\ToolPhoto\\" + EnterpriseId + "\\" + CompanyID + "\\" + RoomID + "\\" + item.ID + "\\" + strPath);
                        }
                        else if (item.ImageType.Trim().ToLower() == "externalimage")
                        {
                            strPath = item.ToolImageExternalURL;
                        }

                    }
                }
            }
            finally
            {
                itemDAL = null;
                item = null;
            }



            return strPath;
        }

        #endregion

        [HttpPost]
        public JsonResult SetGenerateLabelsPDFData(string IDs, string ModuleID, string SortField, string ROTDIds, string ROTDSortField, string CallFrom, string NoOfLableCopy, string NoUsedLable, string ActionName)
        {
            Session["IDs"] = IDs;
            Session["ModuleID"] = ModuleID;
            Session["SortField"] = SortField;
            Session["ROTDIds"] = ROTDIds;
            Session["ROTDSortField"] = ROTDSortField;
            Session["CallFrom"] = CallFrom;
            Session["NoOfLableCopy"] = NoOfLableCopy;
            Session["NoUsedLable"] = NoUsedLable;
            Session["ActionName"] = ActionName;

            return Json(new { Status = true, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GenerateLabelsPDFNew()
        {
            if (Session["IDs"] == null && Session["ModuleID"] == null && Session["SortField"] == null && Session["ROTDIds"] == null &&
                Session["ROTDSortField"] == null && Session["CallFrom"] == null && Session["NoOfLableCopy"] == null && Session["NoUsedLable"] == null)
            {
                return RedirectToAction("MyProfile", "Master");
            }

            LabelFieldModuleTemplateDetailDAL objLabelDetailDAL = null;
            LabelTemplateMasterDAL objTemplateMasterDAL = null;
            LabelModuleTemplateDetailDAL objModuleTemplateDAL = null;
            LabelModuleMasterDAL objModuelMasterDAL = null;
            LabelModuleFieldMasterDAL objModuleFieldMasterDAL = null;

            LabelModuleTemplateDetailDTO objModuleTemplateDTO = null;
            LabelFieldModuleTemplateDetailDTO objLabelDetailDTO = null;
            LabelTemplateMasterDTO objTemplateDTO = null;
            LabelModuleMasterDTO objModuelMasterDTO = null;
            Document doc = null;
            MemoryStream fs = null;
            PdfWriter pdfWriter = null;
            PdfPTable table = null;
            List<object> lstObjects = null;
            List<object> lstObjs = null;
            List<float> lstFL = null;
            PdfPTable innerTable = null;
            Dictionary<string, IElement> supplierLogoDictionary = null;

            try
            {
                //Int64 ModuleID = int.Parse(frm["hdnModuleID"]);
                Int64 ModuleID = 0;
                if (!string.IsNullOrWhiteSpace(Convert.ToString(Session["ModuleID"])))
                    ModuleID = Convert.ToInt64(Session["ModuleID"]);

                //string IDs = frm["hdnIDs"];
                string IDs = Convert.ToString(Session["IDs"]);
                //string ROTDIDs = frm["hdnROTDIds"];
                string ROTDIDs = Convert.ToString(Session["ROTDIds"]);
                //string CallFrom = frm["hdnCallFrom"];
                string CallFrom = Convert.ToString(Session["CallFrom"]);

                int NoOfCopy = 0;
                //int.TryParse(frm["txtNoOfLableCopy"], out NoOfCopy);
                int.TryParse(Convert.ToString(Session["NoOfLableCopy"]), out NoOfCopy);
                if (NoOfCopy <= 0)
                {
                    NoOfCopy = 1;
                }
                int NoOfUsedLabel = 0;
                //int.TryParse(frm["txtNoUsedLable"], out NoOfUsedLabel);
                if (!string.IsNullOrWhiteSpace(Convert.ToString(Session["NoUsedLable"])))
                    NoOfUsedLabel = Convert.ToInt32(Session["NoUsedLable"]);

                //string SortFields = frm["hdnSortField"];
                string SortFields = Convert.ToString(Session["SortField"]);
                if (!string.IsNullOrEmpty(SortFields) && SortFields.ToLower().Contains("null"))
                    SortFields = string.Empty;


                objLabelDetailDAL = new LabelFieldModuleTemplateDetailDAL(SessionHelper.EnterPriseDBName);
                objTemplateMasterDAL = new LabelTemplateMasterDAL(SessionHelper.EnterPriseDBName);
                objModuleTemplateDAL = new LabelModuleTemplateDetailDAL(SessionHelper.EnterPriseDBName);
                objModuelMasterDAL = new LabelModuleMasterDAL(SessionHelper.EnterPriseDBName);
                objModuleFieldMasterDAL = new LabelModuleFieldMasterDAL(SessionHelper.EnterPriseDBName);

                //objModuleTemplateDTO = objModuleTemplateDAL.GetAllRecords(SessionHelper.CompanyID, SessionHelper.RoomID).FirstOrDefault(x => x.ModuleID == ModuleID && x.CompanyID == SessionHelper.CompanyID && x.RoomID == SessionHelper.RoomID);
                objModuleTemplateDTO = objModuleTemplateDAL.GetAllRecordByModuleID(SessionHelper.CompanyID, SessionHelper.RoomID, ModuleID);
                //objLabelDetailDTO = objLabelDetailDAL.GetRecord(objModuleTemplateDTO.TemplateDetailID, SessionHelper.CompanyID, SessionHelper.RoomID);
                objLabelDetailDTO = objLabelDetailDAL.GetLabelFieldModuleTemplateDetailByID(objModuleTemplateDTO.TemplateDetailID, SessionHelper.CompanyID, SessionHelper.RoomID);
                //objTemplateDTO = objTemplateMasterDAL.GetRecord(objLabelDetailDTO.TemplateID, SessionHelper.CompanyID);
                objTemplateDTO = objTemplateMasterDAL.GetLabelTemplateMasterByCompanyTemplateID(objLabelDetailDTO.TemplateID, SessionHelper.CompanyID);
                //objModuelMasterDTO = objModuelMasterDAL.GetRecord(ModuleID);
                objModuelMasterDTO = objModuelMasterDAL.GetLabelModuleMasterByID(ModuleID);

                Int64[] arrIDs = IDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(Int64.Parse).ToArray();
                objLabelDetailDTO.lstModuleFields = objModuleFieldMasterDAL.GetRecordsModueWise(ModuleID, SessionHelper.CompanyID, true).ToList();
                Int64[] ints = objLabelDetailDTO.FeildIDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(Int64.Parse).ToArray();
                objLabelDetailDTO.lstSelectedModuleFields = objLabelDetailDTO.lstModuleFields.Where(x => ints.Contains(x.ID)).ToList();

                _CostDecimalFormat = CostDecimalFormat;
                _QtyDecimalFormat = QtyDecimalFormat;

                lstObjects = GetModuleWiseObjecList(objModuelMasterDTO, arrIDs, SortFields, objLabelDetailDTO, ROTDIDs, CallFrom);
                lstObjs = new List<object>();
                foreach (var item in lstObjects)
                {
                    for (int c = 0; c < NoOfCopy; c++)
                    {
                        lstObjs.Add(item);
                    }
                }

                for (int i = 0; i < NoOfUsedLabel; i++)
                {
                    lstObjs.Insert(i, null);
                }

                float pageWidth = ConvertInchToPoint(objTemplateDTO.PageWidth);
                float pageHeight = ConvertInchToPoint(objTemplateDTO.PageHeight);
                float pageLeftMarging = ConvertInchToPoint(objTemplateDTO.PageMarginLeft);
                float pageRightMarging = ConvertInchToPoint(objTemplateDTO.PageMarginRight);
                float pageTopMarging = ConvertInchToPoint(objTemplateDTO.PageMarginTop);
                float pageBottomMarging = ConvertInchToPoint(objTemplateDTO.PageMarginBottom);

                doc = new Document(new Rectangle(pageWidth, pageHeight));
                doc.SetMargins(pageLeftMarging, pageRightMarging, pageTopMarging, pageBottomMarging);

                fs = new MemoryStream();
                pdfWriter = PdfWriter.GetInstance(doc, fs);
                //PageEventHelper pageEventHelper = new PageEventHelper(0, 0);
                //pdfWriter.PageEvent = pageEventHelper;
                doc.Open();

                lstFL = new List<float>();
                float labelWidth = ConvertInchToPoint(objTemplateDTO.LabelWidth);

                supplierLogoDictionary = new Dictionary<string, IElement>();

                for (int i = 0; i < objTemplateDTO.NoOfColumns; i++)
                {
                    float lblWidth = 0;
                    lblWidth = ConvertInchToPoint(objTemplateDTO.LabelWidth);
                    lstFL.Add(lblWidth);

                    if (objTemplateDTO.LabelSpacingHorizontal > 0 && i < objTemplateDTO.NoOfColumns - 1)
                    {
                        lblWidth = ConvertInchToPoint(objTemplateDTO.LabelSpacingHorizontal);
                        lstFL.Add(lblWidth);
                    }
                }

                int NoOfRowsInOnePage = objTemplateDTO.NoOfLabelPerSheet / objTemplateDTO.NoOfColumns;
                int NoOfCellsInOneRow = lstFL.Count();
                int NoOfLabelsInPageWithSpace = NoOfCellsInOneRow * NoOfRowsInOnePage;
                int NoOfPages = (int)Math.Ceiling((decimal)lstObjs.Count() / (decimal)objTemplateDTO.NoOfLabelPerSheet);
                int CurrentLabelIndexInAccrossAllPages = 1;
                supplierLogoDictionary = new Dictionary<string, IElement>();

                if (NoOfPages <= 0)
                {
                    table = new PdfPTable(1);
                    PdfPCell tabelCell = GetLabelTableCell(objTemplateDTO);
                    tabelCell.AddElement(new Phrase(ResLabelPrinting.MsgNotDataFound));
                    table.AddCell(tabelCell);
                    doc.Add(table);
                }

                for (int k = 0; k < NoOfPages; k++)
                {
                    if (k > 0)
                        doc.NewPage();

                    table = new PdfPTable(lstFL.Count());
                    table.WidthPercentage = 100f;

                    table.SetWidths(lstFL.ToArray<float>());
                    table.DefaultCell.Border = 1;
                    table.SkipFirstHeader = true;
                    table.SkipLastFooter = true;
                    int currentLabelIndexInTabel = 1;
                    int currentRowIndex = 1;
                    int currentCellIndex = 1;

                    for (int i = 0; i < NoOfLabelsInPageWithSpace; i++)
                    {
                        PdfPCell tabelCell = GetLabelTableCell(objTemplateDTO);
                        if (lstFL[currentCellIndex - 1] == labelWidth)
                        {
                            if (lstObjs.Count() >= CurrentLabelIndexInAccrossAllPages && lstObjs[CurrentLabelIndexInAccrossAllPages - 1] != null)
                            {
                                innerTable = GetCellContentFromXMLNew(objTemplateDTO, lstObjs[CurrentLabelIndexInAccrossAllPages - 1], objModuelMasterDTO, objLabelDetailDTO, ref supplierLogoDictionary, pdfWriter, CallFrom);
                                tabelCell.AddElement(innerTable);
                            }
                            else
                            {
                                //tabelCell.AddElement(new Phrase("Label In Table: " + currentLabelIndexInTabel));
                                tabelCell.AddElement(new Phrase(""));
                            }

                            currentLabelIndexInTabel += 1;
                            CurrentLabelIndexInAccrossAllPages += 1;
                        }
                        table.AddCell(tabelCell);
                        if (currentCellIndex == lstFL.Count())
                        {
                            currentCellIndex = 1;
                            table.CompleteRow();
                            currentRowIndex += 1;
                            AddVerticalSpaceInLabelsRow(objTemplateDTO, table, NoOfCellsInOneRow, currentRowIndex, NoOfRowsInOnePage);
                        }
                        else
                        {
                            currentCellIndex += 1;
                        }

                    }
                    //table.CompleteRow();
                    doc.Add(table);

                    //if (CurrentLabelIndexInAccrossAllPages < lstObjs.Count)
                    //{
                    //    doc.NewPage();
                    //}
                }
                pdfWriter.CloseStream = false;
                doc.Close();
                pdfWriter.Dispose();
                fs.Position = 0;
                //string strFileName = DateTime.Now.ToString("yyyyMMdd_HHmmssfff") + "_" + Guid.NewGuid().ToString() + ".pdf";
                //return File(fs, "application/pdf", strFileName);
                return File(fs, "application/pdf");
            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                return Content("<script language='javascript' type='text/javascript'>alert('Something Is Wrong');</script>");
                //throw ex;
            }
            finally
            {

                fs = null;
                doc = null;
                pdfWriter = null;
                table = null;
                innerTable = null;

                objLabelDetailDAL = null;
                objTemplateMasterDAL = null;
                objModuleTemplateDAL = null;
                objModuelMasterDAL = null;
                objModuleFieldMasterDAL = null;
                objModuleTemplateDTO = null;
                objLabelDetailDTO = null;
                objTemplateDTO = null;
                objModuelMasterDTO = null;
                lstObjects = null;
                lstObjs = null;
                lstFL = null;

            }
        }

        #endregion

        #region Avery Template Master Editing

        public ActionResult LabelTemplateMasterList()
        {
            return View();
        }

        public ActionResult AjaxLabelTemplateList(JQueryDataTableParamModel param)
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
            //if (string.IsNullOrEmpty(sortColumnName) || sortColumnName == "0" || sortColumnName == "undefined")
            //    sortColumnName = "Name asc";

            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined"))
                    sortColumnName = "TemplateName desc";
            }
            else
                sortColumnName = "TemplateName desc";

            //if (sortDirection == "asc")
            //    sortColumnName = sortColumnName + " asc";
            //else
            //    sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;

            //BinMasterController controller = new BinMasterController();
            LabelTemplateMasterDAL objDAL = new LabelTemplateMasterDAL(SessionHelper.EnterPriseDBName);
            //IEnumerable<LabelTemplateMasterDTO> DataFromDB = objDAL.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted);
            IEnumerable<LabelTemplateMasterDTO> DataFromDB = objDAL.GetPagedRecordsDB(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted);
            ViewBag.TotalRecordCount = TotalRecordCount;
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = DataFromDB
            }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult UpdateDataInBaseTemplateMasterTable(LabelTemplateMasterDTO[] objDTO)
        {
            LabelTemplateMasterDAL objDAL = null;
            try
            {
                objDAL = new LabelTemplateMasterDAL(SessionHelper.EnterPriseDBName);
                if (objDTO != null && objDTO.Length > 0)
                {
                    foreach (var item in objDTO)
                    {
                        objDAL.EditInBaseDB(item);
                    }
                }
                return Json(new { Massage = ResCommon.MsgDataSaved, Status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objDAL = null;
            }

        }

        [HttpPost]
        public JsonResult UpdateDataInCurrentDBTemplateMasterTable(LabelTemplateMasterDTO[] objDTO)
        {
            LabelTemplateMasterDAL objDAL = null;
            try
            {
                objDAL = new LabelTemplateMasterDAL(SessionHelper.EnterPriseDBName);
                if (objDTO != null && objDTO.Length > 0)
                {
                    foreach (var item in objDTO)
                    {
                        objDAL.EditInCurrentDB(item);
                    }
                }
                return Json(new { Massage = ResCommon.MsgDataSaved, Status = true }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                objDAL = null;
            }
        }

        [HttpPost]
        public JsonResult UpdateAllEnterpriseTemplateMasterTable()
        {
            LabelTemplateMasterDAL objDAL = null;
            try
            {
                objDAL = new LabelTemplateMasterDAL(SessionHelper.EnterPriseDBName);
                objDAL.EditInAllEnterprise();
                return Json(new { Massage = ResCommon.MsgDataSaved, Status = true }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objDAL = null;
            }

        }

        [HttpPost]
        public JsonResult UpdateTemplateMasterDataInCurrentCompany(LabelTemplateMasterDTO[] objDTO)
        {
            LabelTemplateMasterDAL objDAL = null;
            try
            {
                objDAL = new LabelTemplateMasterDAL(SessionHelper.EnterPriseDBName);
                if (objDTO != null && objDTO.Length > 0)
                {
                    foreach (var item in objDTO)
                    {
                        objDAL.EditInCurrentDB(item);
                    }
                }
                return Json(new { Massage = ResCommon.MsgDataSaved, Status = true }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                objDAL = null;
            }
        }


        #endregion

        #region Commented Code

        /// <summary>
        /// GetPropertyList
        /// </summary>
        /// <param name="testClass"></param>
        /// <returns></returns>
        //public Dictionary<string, string> GetPropertyList(object testClass)
        //{

        //    PropertyInfo[] aryProperties = testClass.GetType().GetProperties();
        //    Dictionary<string, string> propertyList = new Dictionary<string, string>();
        //    foreach (PropertyInfo property in aryProperties)
        //    {
        //        propertyList.Add(property.Name, property.Name);
        //    }

        //    return propertyList;
        //}

        /// <summary>
        /// GetCellContent
        /// </summary>
        /// <param name="objLabelSize"></param>
        /// <param name="item"></param>
        /// <param name="objMMDTO"></param>
        /// <returns></returns>
        //private PdfPTable GetCellContent(LabelTemplateMasterDTO objLabelSize, object item, LabelModuleMasterDTO objMMDTO, LabelFieldModuleTemplateDetailDTO objLFMTDetailDTO)
        //{
        //    List<float> lstFL = new List<float>();

        //    float fwidth = (float)objLabelSize.LabelWidth * basePoints;
        //    float leftrightpading = ((float)objLabelSize.LabelPaddingLeft * 0.72f) + ((float)objLabelSize.LabelPaddingRight * 0.72f);
        //    int intWidth = (int)(fwidth - leftrightpading);

        //    if (objLabelSize.NoOfColumns <= 3)
        //    {
        //        lstFL.Add((intWidth * 20) / 100);
        //        lstFL.Add((intWidth * 30) / 100);
        //        lstFL.Add((intWidth * 20) / 100);
        //        lstFL.Add((intWidth * 30) / 100);
        //    }
        //    else
        //    {
        //        lstFL.Add((intWidth * 30) / 100);
        //        lstFL.Add((intWidth * 70) / 100);
        //    }

        //    //BaseFont baseFont = BaseFont.CreateFont(BaseFont.CP1252, BaseFont.CP1252, false);
        //    //Font fb = new Font(baseFont,(int)objLFMTDetailDTO.FontSize,Font.BOLD);
        //    //Font fn = new Font(baseFont, (int)objLFMTDetailDTO.FontSize, Font.NORMAL);

        //    Font verdanaBold = FontFactory.GetFont("Verdana", (float)objLFMTDetailDTO.FontSize, Font.BOLD);
        //    Font verdanaNormal = FontFactory.GetFont("Verdana", (float)objLFMTDetailDTO.FontSize, Font.NORMAL);

        //    PdfPTable table = new PdfPTable(lstFL.Count());

        //    table.SkipFirstHeader = true;
        //    table.SkipLastFooter = true;
        //    table.WidthPercentage = 100f;
        //    table.HorizontalAlignment = 0;
        //    table.DefaultCell.Border = 1;
        //    table.SetWidths(lstFL.ToArray<float>());

        //    string BarcodeString = "";

        //    if (objMMDTO.ModuleDTOName == "AssetLabelDTO" || objMMDTO.ModuleDTOName == "KittingLabelDTO" || )
        //    {
        //        PropertyInfo info = null;
        //        object value = null;
        //        PdfPCell cell = null;

        //        string barcodefield = objLFMTDetailDTO.lstModuleFields.FirstOrDefault(x => x.ID == objLFMTDetailDTO.BarcodeKey).FieldName;
        //        info = item.GetType().GetProperty(barcodefield);
        //        value = info.GetValue(item, null);
        //        BarcodeString = "#" + value;
        //        byte[] b = GetBarcode(objLabelSize, BarcodeString, intWidth, 30, false);
        //        iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(b);
        //        img.SetDpi(96, 96);
        //        if (img.Width > (objLabelSize.LabelWidth * basePoints))
        //            img.ScaleToFitLineWhenOverflow = true;

        //        cell = GetContentCell(img, lstFL.Count);
        //        cell.FixedHeight = 30;
        //        table.AddCell(cell);
        //        foreach (var fl in objLFMTDetailDTO.lstSelectedModuleFields)
        //        {
        //            info = item.GetType().GetProperty(fl.FieldName);
        //            value = info.GetValue(item, null);
        //            cell = GetContentCell(new Chunk(fl.FieldDisplayName, verdanaNormal), 1);
        //            cell.FixedHeight = (float)(objLFMTDetailDTO.FontSize * 2);
        //            table.AddCell(cell);
        //            cell = GetContentCell(new Chunk((string)value, verdanaNormal), 1);
        //            cell.FixedHeight = (float)(objLFMTDetailDTO.FontSize * 2);
        //            table.AddCell(cell);
        //        }
        //    }
        //    else if (objMMDTO.ModuleDTOName == "InventoryLabelDTO")
        //    {
        //        PropertyInfo info = null;
        //        object value = null;
        //        PdfPCell cell = null;

        //        string barcodefield = objLFMTDetailDTO.lstModuleFields.FirstOrDefault(x => x.ID == objLFMTDetailDTO.BarcodeKey).FieldName;
        //        info = item.GetType().GetProperty(barcodefield);
        //        value = info.GetValue(item, null);
        //        BarcodeString = "#" + value;
        //        if (objLFMTDetailDTO.IncludeBin && !string.IsNullOrEmpty(((InventoryLabelDTO)item).BinNumber))
        //        {
        //            BarcodeString += "@" + ((InventoryLabelDTO)item).BinNumber;
        //        }

        //        if (objLFMTDetailDTO.IncludeQuantity)
        //        {
        //            if (objLFMTDetailDTO.QuantityField != "1")
        //            {
        //                info = item.GetType().GetProperty(objLFMTDetailDTO.QuantityField);
        //                value = info.GetValue((InventoryLabelDTO)item, null);
        //            }
        //            else
        //            {
        //                value = objLFMTDetailDTO.QuantityField;
        //            }
        //            BarcodeString += "<" + value;
        //        }
        //        byte[] b = GetBarcode(objLabelSize, BarcodeString, intWidth, 30, false);
        //        iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(b);
        //        img.SetDpi(96, 96);
        //        if (img.Width > (objLabelSize.LabelWidth * basePoints))
        //            img.ScaleToFitLineWhenOverflow = true;
        //        cell = GetContentCell(img, lstFL.Count);
        //        cell.FixedHeight = 30;
        //        table.AddCell(cell);
        //        foreach (var fl in objLFMTDetailDTO.lstSelectedModuleFields)
        //        {
        //            info = item.GetType().GetProperty(fl.FieldName);
        //            value = info.GetValue(item, null);
        //            cell = GetContentCell(new Chunk(fl.FieldDisplayName, verdanaNormal), 1);
        //            cell.FixedHeight = (float)(objLFMTDetailDTO.FontSize * 2);
        //            table.AddCell(cell);
        //            cell = GetContentCell(new Chunk((string)value, verdanaNormal), 1);
        //            cell.FixedHeight = (float)(objLFMTDetailDTO.FontSize * 2);
        //            table.AddCell(cell);
        //        }

        //    }
        //    else if (objMMDTO.ModuleDTOName == "KittingLabelDTO")
        //    {
        //        PropertyInfo info = null;
        //        object value = null;
        //        PdfPCell cell = null;

        //        string barcodefield = objLFMTDetailDTO.lstModuleFields.FirstOrDefault(x => x.ID == objLFMTDetailDTO.BarcodeKey).FieldName;
        //        info = item.GetType().GetProperty(barcodefield);
        //        value = info.GetValue(item, null);
        //        BarcodeString = "#" + value;

        //        byte[] b = GetBarcode(objLabelSize, BarcodeString, intWidth, 30, false);
        //        iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(b);
        //        img.SetDpi(96, 96);
        //        if (img.Width > (objLabelSize.LabelWidth * basePoints))
        //            img.ScaleToFitLineWhenOverflow = true;
        //        cell = GetContentCell(img, lstFL.Count);
        //        cell.FixedHeight = 30;
        //        table.AddCell(cell);
        //        foreach (var fl in objLFMTDetailDTO.lstSelectedModuleFields)
        //        {
        //            info = item.GetType().GetProperty(fl.FieldName);
        //            value = info.GetValue(item, null);
        //            cell = GetContentCell(new Chunk(fl.FieldDisplayName, verdanaNormal), 1);
        //            cell.FixedHeight = (float)(objLFMTDetailDTO.FontSize * 2);
        //            table.AddCell(cell);
        //            cell = GetContentCell(new Chunk((string)value, verdanaNormal), 1);
        //            cell.FixedHeight = (float)(objLFMTDetailDTO.FontSize * 2);
        //            table.AddCell(cell);
        //        }
        //    }
        //    else if (objMMDTO.ModuleDTOName == "OrderLabelDTO")
        //    {

        //    }
        //    else if (objMMDTO.ModuleDTOName == "QuickListLabelDTO")
        //    {
        //    }
        //    else if (objMMDTO.ModuleDTOName == "ReceiveLabelDTO")
        //    {
        //    }
        //    else if (objMMDTO.ModuleDTOName == "StagingLabelDTO")
        //    {
        //    }
        //    else if (objMMDTO.ModuleDTOName == "ToolLabelDTO")
        //    {
        //    }
        //    else if (objMMDTO.ModuleDTOName == "TransferLabelDTO")
        //    {
        //    }

        //    return table;
        //}

        /// <summary>
        /// Configuration
        /// </summary>
        /// <returns></returns>
        //public ActionResult Configuration()
        //{
        //    LabelTemplateMasterDAL objTemplateDAL = new LabelTemplateMasterDAL(SessionHelper.EnterPriseDBName);
        //    LabelModuleMasterDAL objMoudleDAL = new LabelModuleMasterDAL(SessionHelper.EnterPriseDBName);

        //    ViewBag.LabelTemplateList = objTemplateDAL.GetAllRecords().ToList();
        //    ViewBag.LabelModuleList = objMoudleDAL.GetAllRecords().ToList();

        //    return View();
        //}

        /// <summary>
        /// fsfsfd
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        //[HttpPost]
        //public JsonResult SaveDefaultFieldsTemplate(FormCollection formCollection)
        //{
        //    LabelFieldModuleTemplateDetailDAL objDetailDAL = new LabelFieldModuleTemplateDetailDAL(SessionHelper.EnterPriseDBName);
        //    if (!string.IsNullOrEmpty(Convert.ToString(formCollection["hdnDetailID"])))
        //    {
        //        objDetailDAL.DeleteRecords(Convert.ToString(formCollection["hdnDetailID"]), SessionHelper.UserID, SessionHelper.CompanyID);

        //    }
        //    LabelFieldModuleTemplateDetailDTO objDetailDTO = new LabelFieldModuleTemplateDetailDTO()
        //        {
        //            ModuleID = Int64.Parse(Convert.ToString(formCollection["ddlLabelModuleList"])),
        //            CompanyID = SessionHelper.CompanyID,
        //            TemplateID = Int64.Parse(Convert.ToString(formCollection["ddlLabelTemplateList"])),
        //            FeildIDs = Convert.ToString(formCollection["chkField"]),
        //            CreatedBy = SessionHelper.UserID,
        //            UpdatedBy = SessionHelper.UserID,
        //            CreatedOn = DateTime.Now,
        //            UpdatedOn = DateTime.Now,
        //        };

        //    objDetailDAL.Insert(objDetailDTO);

        //    return Json(new { Massage = "success", Status = "success" }, JsonRequestBehavior.AllowGet);
        //}

        /// <summary>
        /// /sfsdf
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        //private PdfPCell GetContentCell(IElement e)
        //{
        //    PdfPCell cell = new PdfPCell();
        //    cell.VerticalAlignment = Element.ALIGN_TOP;
        //    cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //    cell.Border = 1;
        //    cell.Padding = 0.5f;
        //    cell.UseBorderPadding = true;
        //    cell.BorderWidthTop = 1f;
        //    cell.BorderWidthLeft = 1f;
        //    cell.BorderWidthRight = 1f;
        //    cell.BorderWidthBottom = 1f;
        //    cell.BorderColor = BaseColor.RED;

        //    cell.AddElement(e);
        //    return cell;
        //}

        /// <summary>
        /// GetCellContent
        /// </summary>
        /// <param name="objLabelSize"></param>
        /// <param name="item"></param>
        /// <param name="objMMDTO"></param>
        /// <param name="objLFMTDetailDTO"></param>
        /// <returns></returns>
        //private PdfPTable GetCellContentFromXML(LabelTemplateMasterDTO objLabelSize, object item, LabelModuleMasterDTO objMMDTO, LabelFieldModuleTemplateDetailDTO objLFMTDetailDTO)
        //{
        //    float fwidth = ConvertInchToPoint(objLabelSize.LabelWidth);
        //    //float leftrightpading = ((float)objLabelSize.LabelPaddingLeft * 0.72f) + ((float)objLabelSize.LabelPaddingRight * 0.72f);
        //    float leftrightpading = ConvertInchToPoint(objLabelSize.LabelPaddingLeft) + ConvertInchToPoint(objLabelSize.LabelPaddingRight);
        //    int intWidth = (int)(fwidth - leftrightpading);

        //    string strfName = "Verdana";
        //    if (!string.IsNullOrEmpty(objLFMTDetailDTO.TextFont))
        //        strfName = objLFMTDetailDTO.TextFont;

        //    string BarcodeString = "";
        //    PropertyInfo info = null;
        //    object value = null;
        //    PdfPCell cell = null;

        //    XmlDocument objDoc = new XmlDocument();
        //    objDoc.LoadXml(objLFMTDetailDTO.LabelXML);

        //    int rowCount = objDoc.SelectNodes("mytable/row").Count;
        //    PdfPTable table = new PdfPTable(1);
        //    table.WidthPercentage = 100f;
        //    table.HorizontalAlignment = 0;
        //    table.DefaultCell.Border = 0;
        //    table.SkipFirstHeader = true;
        //    table.SkipLastFooter = true;
        //    table.SpacingAfter = 0;
        //    table.SpacingBefore = 0;

        //    foreach (XmlElement row in objDoc.SelectNodes("mytable/row"))
        //    {
        //        PdfPCell rowCell = new PdfPCell();
        //        rowCell.Border = 0;
        //        rowCell.Padding = 0;
        //        rowCell.UseBorderPadding = true;
        //        rowCell.VerticalAlignment = Element.ALIGN_TOP;
        //        rowCell.HorizontalAlignment = Element.ALIGN_LEFT;
        //        rowCell.UseAscender = true;
        //        rowCell.BorderColor = BaseColor.YELLOW;
        //        int noOfColms = row.SelectNodes("column").Count;

        //        if (noOfColms > 0)
        //        {
        //            PdfPTable prowTable = new PdfPTable(noOfColms);
        //            prowTable.WidthPercentage = 100f;
        //            prowTable.HorizontalAlignment = 0;
        //            prowTable.DefaultCell.Border = 1;
        //            prowTable.SkipFirstHeader = true;
        //            prowTable.SkipLastFooter = true;
        //            prowTable.SpacingAfter = 0;
        //            prowTable.SpacingBefore = 0;

        //            foreach (XmlElement child in row.SelectNodes("column"))
        //            {
        //                float fltheight = 0;
        //                string strheight = child.GetAttribute("height");
        //                if (!string.IsNullOrEmpty(strheight))
        //                    strheight = strheight.Replace("px", "");

        //                int RowSpan = 1;
        //                string strRowSpan = child.GetAttribute("rowspan");
        //                int.TryParse(strRowSpan, out RowSpan);
        //                if (RowSpan <= 0)
        //                    RowSpan = 1;

        //                int ColSpan = 1;
        //                string strColSpan = child.GetAttribute("colspan");
        //                int.TryParse(strColSpan, out ColSpan);
        //                if (ColSpan <= 0)
        //                    ColSpan = 1;




        //                float.TryParse(strheight, out fltheight);

        //                if (!child.InnerText.ToLower().Contains("barcode") &&
        //                    !child.InnerText.ToLower().Contains("ItemImage") &&
        //                    !child.InnerText.ToLower().Contains("EnerPriceLogo") &&
        //                    !child.InnerText.ToLower().Contains("CompanyLogo"))
        //                {
        //                    Font textFont = FontFactory.GetFont(strfName, (float)objLFMTDetailDTO.FontSize, Font.NORMAL);

        //                    string fontBold = Convert.ToString(child.GetAttribute("font-weight"));
        //                    string fontItalic = Convert.ToString(child.GetAttribute("font-style"));
        //                    if (fontBold.ToLower().Contains("bold") && fontItalic.ToLower().Contains("italic"))
        //                        textFont.SetStyle(3);
        //                    else if (fontBold.ToLower().Contains("bold") && !fontItalic.ToLower().Contains("italic"))
        //                        textFont.SetStyle(1);
        //                    else if (!fontBold.ToLower().Contains("bold") && fontItalic.ToLower().Contains("italic"))
        //                        textFont.SetStyle(2);
        //                    else
        //                        textFont.SetStyle(0);

        //                    float fontSize = 0;
        //                    string strfontsize = child.GetAttribute("fontSize");
        //                    float.TryParse(strfontsize, out fontSize);
        //                    if (fontSize > 0)
        //                        textFont.Size = fontSize;

        //                    Int64 FldID = 0;
        //                    string fldid = child.GetAttribute("FieldID");

        //                    Int64.TryParse(fldid, out FldID);

        //                    LabelModuleFieldMasterDTO fl = objLFMTDetailDTO.lstSelectedModuleFields.FirstOrDefault(x => x.ID == FldID);
        //                    if (fl != null && fl.ID == FldID)
        //                    {
        //                        info = item.GetType().GetProperty(fl.FieldName);
        //                        value = info.GetValue(item, null);
        //                        if (value == null)
        //                            value = "";
        //                        Chunk chk = new Chunk(fl.FieldDisplayName + ": " + Convert.ToString(value.ToString()), textFont);
        //                        cell = GetContentCell(chk, 1);
        //                        cell.FixedHeight = (float)(objLFMTDetailDTO.FontSize * 1.5);
        //                        if (fltheight > 0)
        //                        {
        //                            cell.FixedHeight = ((fltheight * 72) / 96);
        //                        }
        //                        cell.Rowspan = RowSpan;
        //                        cell.Colspan = ColSpan;
        //                        prowTable.AddCell(cell);
        //                    }
        //                    else
        //                    {
        //                        cell = GetContentCell(new Chunk("", textFont), 1);
        //                        cell.FixedHeight = (float)(objLFMTDetailDTO.FontSize * 2);
        //                        if (fltheight > 0)
        //                        {
        //                            cell.FixedHeight = ((fltheight * 72) / 96);
        //                        }
        //                        cell.Rowspan = RowSpan;
        //                        cell.Colspan = ColSpan;
        //                        prowTable.AddCell(cell);
        //                    }

        //                }
        //                else if (child.InnerText.ToLower().Contains("barcode"))
        //                {
        //                    string barcodefield = objLFMTDetailDTO.lstModuleFields.FirstOrDefault(x => x.ID == objLFMTDetailDTO.BarcodeKey).FieldName;
        //                    info = item.GetType().GetProperty(barcodefield);
        //                    value = info.GetValue(item, null);
        //                    BarcodeString = "#" + value;
        //                    if (objMMDTO.ModuleDTOName == "InventoryLabelDTO"
        //                            || objMMDTO.ModuleDTOName == "OrderLabelDTO"
        //                            || objMMDTO.ModuleDTOName == "ReceiveLabelDTO"
        //                            || objMMDTO.ModuleDTOName == "StagingLabelDTO"
        //                            || objMMDTO.ModuleDTOName == "TransferLabelDTO")
        //                    {

        //                        if (objLFMTDetailDTO.IncludeBin)
        //                        {
        //                            info = item.GetType().GetProperty("BinNumber");
        //                            string strBin = Convert.ToString(info.GetValue(item, null));
        //                            if (!string.IsNullOrEmpty(strBin))
        //                            {
        //                                BarcodeString += "#B" + strBin;
        //                            }
        //                        }

        //                        if (objLFMTDetailDTO.IncludeQuantity)
        //                        {
        //                            if (objLFMTDetailDTO.QuantityField != "1")
        //                            {
        //                                info = item.GetType().GetProperty(objLFMTDetailDTO.QuantityField);
        //                                value = info.GetValue(item, null);
        //                            }
        //                            else
        //                                value = objLFMTDetailDTO.QuantityField;

        //                            BarcodeString += "#Q" + Convert.ToString(value);
        //                        }

        //                    }
        //                    int height = 25;
        //                    if (fltheight > 0)
        //                    {
        //                        height = (int)((fltheight * 72) / 96);
        //                        height = height - 2;
        //                    }

        //                    byte[] b = GetBarcode(objLabelSize, BarcodeString, intWidth, height, false);
        //                    iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(b);
        //                    //img.BorderColor = BaseColor.BLUE;
        //                    //img.BorderWidthTop = 0;

        //                    img.SetDpi(300, 300);
        //                    if (img.Width > (ConvertInchToPoint(objLabelSize.LabelWidth)))
        //                    {
        //                        img.ScaleToFitLineWhenOverflow = true;
        //                    }
        //                    cell = GetContentCell(img, noOfColms);
        //                    cell.PaddingTop = -3.0f;
        //                    cell.PaddingLeft = 0.0f;
        //                    cell.PaddingRight = 0.0f;
        //                    cell.PaddingBottom = 2.5f;
        //                    cell.UseBorderPadding = false;
        //                    //cell.UseAscender = false;
        //                    //cell.FixedHeight = 25f;

        //                    //if (fltheight > 0)
        //                    //{
        //                    //    cell.FixedHeight = ((fltheight * 72) / 96);
        //                    //}
        //                    prowTable.AddCell(cell);
        //                }
        //            }
        //            prowTable.CompleteRow();
        //            rowCell.AddElement(prowTable);
        //            table.AddCell(rowCell);
        //        }

        //    }

        //    return table;

        //}

        #endregion
    }
}
