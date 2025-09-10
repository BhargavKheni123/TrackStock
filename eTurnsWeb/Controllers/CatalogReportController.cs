using eTurns.DAL;
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
using System.Web.Mvc;
using System.Xml;
using ThoughtWorks.QRCode.Codec;

namespace eTurnsWeb.Controllers
{
    [AuthorizeHelper]
    public class CatalogReportController : eTurnsControllerBase
    {
        public ActionResult CatalogReportList()
        {
            return View();
        }

        public ActionResult CatalogReportTemplateMasterList()
        {
            return View();
        }

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult CatalogReportDetailListAjax(JQueryDataTableParamModel param)
        {
            CatalogReportDAL objDAL = null;
            IEnumerable<CatalogReportDetailDTO> DataFromDB = null;
            try
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
                if (string.IsNullOrEmpty(sortColumnName) || sortColumnName == "0" || sortColumnName == "undefined")
                    sortColumnName = "ID";

                if (sortDirection == "asc")
                    sortColumnName = sortColumnName + " asc";
                else
                    sortColumnName = sortColumnName + " desc";

                string searchQuery = string.Empty;

                int TotalRecordCount = 0;

                objDAL = new CatalogReportDAL(SessionHelper.EnterPriseDBName);
                TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
                //DataFromDB = objDAL.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.CompanyID, IsArchived, IsDeleted, SessionHelper.RoomDateFormat, CurrentTimeZone);
                DataFromDB = objDAL.GetPagedRecordsDB(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.CompanyID, IsArchived, IsDeleted, SessionHelper.RoomDateFormat, CurrentTimeZone);

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
                objDAL = null;
                DataFromDB = null;
            }
        }

        public ActionResult AddEditCatalog()
        {
            CatalogReportDAL objDal = new CatalogReportDAL(SessionHelper.EnterPriseDBName);
            CatalogReportDetailDTO objDetail = null;
            CatalogReportTemplateMasterDAL objTemplateDAL = new CatalogReportTemplateMasterDAL(SessionHelper.EnterPriseDBName);
            //List<CatalogReportTemplateMasterDTO> lstCatalogReportTemplates = objTemplateDAL.GetAllRecords(SessionHelper.CompanyID).ToList();
            List<CatalogReportTemplateMasterDTO> lstCatalogReportTemplates = objTemplateDAL.GetCatalogReportTemplateMasterByCompanyID(SessionHelper.CompanyID).ToList();

            objDetail = new CatalogReportDetailDTO();
            objDetail.FontSize = 10;
            objDetail.lstModuleFields = GetCatalogReportFields();
            objDetail.CompanyID = SessionHelper.CompanyID;
            objDetail.CreatedBy = SessionHelper.UserID;
            objDetail.UpdatedBy = SessionHelper.UserID;
            objDetail.CreatedOn = DateTimeUtility.DateTimeNow;
            objDetail.UpdatedOn = DateTimeUtility.DateTimeNow;
            ViewBag.TextFontList = GetTextFontList();
            ViewBag.BarcodeFontList = GetBarcodeFontList();
            ViewBag.BarcodePatternList = GetBarcodePatternList();
            ViewBag.lstBarcodeKey = GetBarcodeKeyList();
            ViewBag.lstQuantityFields = GetQuantityFieldsList();

            //ViewBag.NoOfItemsOnPage = GetNoOfItemsList();

            ViewBag.CatalogReportTemplateList = lstCatalogReportTemplates;

            return PartialView(objDetail);
        }
        /// <summary>
        /// GetTemplateDetailByID
        /// </summary>
        /// <param name="TemplateID"></param>
        /// <returns></returns>
        public JsonResult GetTemplateDetailByID(Int64 TemplateID)
        {
            CatalogReportTemplateMasterDAL objDAL = null;
            CatalogReportTemplateMasterDTO objDTO = null;
            try
            {
                objDAL = new CatalogReportTemplateMasterDAL(SessionHelper.EnterPriseDBName);
                //objDTO = objDAL.GetRecord(TemplateID);
                objDTO = objDAL.GetCatalogReportTemplateMasterByTemplateID(TemplateID);
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
        /// GetOrderNarrwSearchHTML
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCRNarrwSearchHTML()
        {
            return PartialView();
        }
        public JsonResult GetNarrowSearchData(bool IsDeleted, bool IsArchived)
        {
            CommonDAL objCommonCtrl = null;
            try
            {
                objCommonCtrl = new CommonDAL(SessionHelper.EnterPriseDBName);
                var tmpsupplierIds = new List<long>();
                NarrowSearchData objNarrowSearchData = objCommonCtrl.GetNarrowSearchDataFromCache("CatalogReport", SessionHelper.CompanyID, SessionHelper.RoomID, IsArchived, IsDeleted, "", tmpsupplierIds, "");
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

        public ActionResult EditCatalogDetail(Int64 ID)
        {
            CatalogReportDAL objDal = new CatalogReportDAL(SessionHelper.EnterPriseDBName);
            CatalogReportTemplateMasterDAL objTemplateDAL = new CatalogReportTemplateMasterDAL(SessionHelper.EnterPriseDBName);
            //List<CatalogReportTemplateMasterDTO> lstCatalogReportTemplates = objTemplateDAL.GetAllRecords(SessionHelper.CompanyID).ToList();
            List<CatalogReportTemplateMasterDTO> lstCatalogReportTemplates = objTemplateDAL.GetCatalogReportTemplateMasterByCompanyID(SessionHelper.CompanyID).ToList();

            CatalogReportDetailDTO objDetail = null;
            ViewBag.TextFontList = GetTextFontList();
            ViewBag.BarcodeFontList = GetBarcodeFontList();
            ViewBag.BarcodePatternList = GetBarcodePatternList();

            ViewBag.lstBarcodeKey = GetBarcodeKeyList();
            ViewBag.lstQuantityFields = GetQuantityFieldsList();


            //ViewBag.NoOfItemsOnPage = GetNoOfItemsList();
            ViewBag.CatalogReportTemplateList = lstCatalogReportTemplates;

            //objDetail = objDal.GetRecord(ID, SessionHelper.CompanyID);
            objDetail = objDal.GetCatalogReportDetailByID(ID, SessionHelper.CompanyID);
            objDetail.lstModuleFields = GetCatalogReportFields();
            if (!string.IsNullOrEmpty(objDetail.SelectedFields) && objDetail.SelectedFields.Length > 0)
            {
                objDetail.arrFieldIds = objDetail.SelectedFields.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (objDetail.arrFieldIds != null && objDetail.arrFieldIds.Length > 0)
                {
                    List<KeyValDTO> lsttempKeyValDTO = null;
                    lsttempKeyValDTO = (from p in objDetail.arrFieldIds
                                        select new KeyValDTO
                                        {
                                            key = p,
                                            value = p
                                        }).ToList();

                    objDetail.lstSelectedModuleFields = lsttempKeyValDTO;

                }
            }

            if (objDetail.FontSize <= 0)
                objDetail.FontSize = 10;

            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            if (IsDeleted || IsArchived)
            {
                ViewBag.ViewOnly = true;
            }

            if(objDetail.CreatedOn != null)
                objDetail.CreatedDate = CommonUtility.ConvertDateByTimeZone(objDetail.CreatedOn, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
            if (objDetail.UpdatedOn != null)
                objDetail.UpdatedDate = CommonUtility.ConvertDateByTimeZone(objDetail.UpdatedOn, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);

            return PartialView("AddEditCatalog", objDetail);
        }

        public JsonResult CatalogReportSave(CatalogReportDetailDTO frmColl)
        {

            CatalogReportDAL objDAL = new CatalogReportDAL(SessionHelper.EnterPriseDBName);
            if (frmColl.IncludeQuantity && string.IsNullOrWhiteSpace(frmColl.QuantityField))
            {
                return Json(new { Message = ResLabelPrinting.ErrorMassageForQuantityField, Status = "fail" , DTO = frmColl }, JsonRequestBehavior.AllowGet);
            }
            string strOK = objDAL.DuplicateCheck(frmColl.Name, frmColl.ID, frmColl.CompanyID);
            if (strOK == "duplicate")
            {
                string message = string.Format(ResMessage.DuplicateMessage, ResLabelPrinting.UserDefineTemplateName, frmColl.Name);
                string status = "duplicate";
                return Json(new { Message = message, Status = status, DTO = frmColl }, JsonRequestBehavior.AllowGet);

            }

            frmColl.UpdatedBy = SessionHelper.UserID;
            frmColl.UpdatedOn = DateTimeUtility.DateTimeNow;
            frmColl.CompanyID = SessionHelper.CompanyID;

            if (frmColl.arrFieldIds != null && frmColl.arrFieldIds.Length > 0)
                frmColl.SelectedFields = string.Join(",", frmColl.arrFieldIds);

            if (frmColl.ID <= 0)
            {
                frmColl.CreatedOn = DateTimeUtility.DateTimeNow;
                frmColl.CreatedBy = SessionHelper.UserID;
                objDAL.Insert(frmColl);
            }
            else
            {
                objDAL.Edit(frmColl);
            }

            return Json(new { Message = ResMessage.SaveMessage, Status = "ok", DTO = frmColl }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteCatalogReportDetailRecords(string ids)
        {
            try
            {
                CatalogReportDAL objDAL = new CatalogReportDAL(SessionHelper.EnterPriseDBName);
                string strmsg = objDAL.DeleteRecords(ids, SessionHelper.UserID, SessionHelper.CompanyID,SessionHelper.RoomID,SessionHelper.UserID,SessionHelper.EnterPriceID);
                return Json(new { Message = ResMessage.DeletedSuccessfully, Status = "ok" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                //return ex.Message;
                return Json(new { Message = ex.Message, Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        private List<KeyValDTO> GetNoOfItemsList()
        {
            List<KeyValDTO> lstNoOfItems = new List<KeyValDTO>();
            lstNoOfItems.Add(new KeyValDTO() { key = "1", value = "1 Item" });
            lstNoOfItems.Add(new KeyValDTO() { key = "2", value = "2 Items" });
            lstNoOfItems.Add(new KeyValDTO() { key = "3", value = "3 Items" });
            lstNoOfItems.Add(new KeyValDTO() { key = "4", value = "4 Items" });
            lstNoOfItems.Add(new KeyValDTO() { key = "5", value = "5 Items" });
            lstNoOfItems.Add(new KeyValDTO() { key = "6", value = "6 Items" });
            lstNoOfItems.Add(new KeyValDTO() { key = "7", value = "7 Items" });
            lstNoOfItems.Add(new KeyValDTO() { key = "8", value = "8 Items" });
            lstNoOfItems.Add(new KeyValDTO() { key = "9", value = "9 Items" });
            lstNoOfItems.Add(new KeyValDTO() { key = "10", value = "10 Items" });
            return lstNoOfItems;
        }


        private List<KeyValDTO> GetCatalogReportFields()
        {
            CatalogReportFieldsDTO objCatalogFields = new CatalogReportFieldsDTO();
            PropertyInfo[] props = objCatalogFields.GetType().GetProperties();
            List<KeyValDTO> lsttempKeyValDTO = null;
            lsttempKeyValDTO = (from p in props
                                where p.PropertyType.FullName.Contains("Int64") == false
                                && p.PropertyType.FullName.ToLower().Contains("system.guid") == false
                                && p.Name.ToLower().Contains("itemuniquenumber") == false
                                && p.Name.ToLower() != "inventoryclassification"
                                && p.Name.ToLower().Contains("image") == false
                                select new KeyValDTO
                                {
                                    key = p.Name,
                                    value = GetResourceValue(p.Name),
                                }).ToList();
            lsttempKeyValDTO.Add(new KeyValDTO() { key = "ItemImage", value = eTurns.DTO.Resources.ResourceHelper.GetResourceValue("ItemImage", "Res_RPT_CatalogReportFields") });//.OrderBy(x => x.value).ToList();

            lsttempKeyValDTO = lsttempKeyValDTO.OrderBy(x => x.value).ToList();
            return lsttempKeyValDTO;
        }
        private string GetResourceValue(string Name)
        {
            if (!string.IsNullOrWhiteSpace(Name))
            {
                string Prefix = string.Empty;
                if (Name.ToLower().Contains("udf"))
                {
                    if (Name.ToLower().Contains("udf10"))
                    {
                        Prefix = Name.Replace("UDF10", string.Empty);
                    }
                    else if (Name.ToLower().Contains("udf"))
                    {
                        Prefix = Name.Replace("UDF1", string.Empty).Replace("UDF2", string.Empty).Replace("UDF3", string.Empty).Replace("UDF4", string.Empty).Replace("UDF5", string.Empty).Replace("UDF6", string.Empty).Replace("UDF7", string.Empty).Replace("UDF8", string.Empty).Replace("UDF9", string.Empty);
                    }

                    if (!string.IsNullOrWhiteSpace(Prefix))
                    {
                        Name = Name.Replace(Prefix, string.Empty);
                    }
                }

                string ResourceValue = eTurns.DTO.Resources.ResourceHelper.GetResourceValue(Name, "Res_RPT_CatalogReportFields", (Name.Contains("UDF")) ? true : false);
                if (!string.IsNullOrWhiteSpace(Prefix))
                {
                    ResourceValue = Prefix + ResourceValue;
                }
                return ResourceValue;
            }
            else
            {
                return Name;
            }

        }
        /// <summary>
        /// GetBarcodeFontList
        /// </summary>
        /// <returns></returns>
        private List<KeyValDTO> GetTextFontList()
        {
            List<KeyValDTO> lstDTO = new List<KeyValDTO>();
            KeyValDTO objDTO = new KeyValDTO() { key = "Calibri", value = "Calibri" };
            lstDTO.Add(objDTO);
            objDTO = new KeyValDTO() { key = "Verdana", value = "Verdana" };
            lstDTO.Add(objDTO);
            return lstDTO;
        }

        /// <summary>
        /// GetBarcodeFontList
        /// </summary>
        /// <returns></returns>
        private List<KeyValDTO> GetBarcodePatternList()
        {
            List<KeyValDTO> lstDTO = new List<KeyValDTO>();

            KeyValDTO objDTO = new KeyValDTO() { key = "US", value = "#IItem#BBin#QQty" };
            lstDTO.Add(objDTO);
            objDTO = new KeyValDTO() { key = "UK", value = "*Item+Bin+Qty*" };
            lstDTO.Add(objDTO);
            objDTO = new KeyValDTO() { key = "US01", value = "#Item@Bin<Qty" };
            lstDTO.Add(objDTO);
            //lstDTO.Insert(0, new KeyValDTO() { key = "93", value = ResLabelPrinting.DefaultDropdownText });
            return lstDTO;
        }

        /// <summary>
        /// GetBarcodeFontList
        /// </summary>
        /// <returns></returns>
        private List<KeyValDTO> GetBarcodeFontList()
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

        private List<LabelModuleFieldMasterDTO> GetBarcodeKeyList()
        {
            long ModuleID = 2; // 2 = Inventory
            eTurns.DAL.LabelPrintingDAL.LabelModuleFieldMasterDAL objMoudleFieldDAL = new eTurns.DAL.LabelPrintingDAL.LabelModuleFieldMasterDAL(SessionHelper.EnterPriseDBName);
            List<LabelModuleFieldMasterDTO> lstModuleFields = objMoudleFieldDAL.GetRecordsModueWise(ModuleID, SessionHelper.CompanyID, true).ToList();


            if (lstModuleFields != null && lstModuleFields.Count > 0)
            {
                lstModuleFields = lstModuleFields.Where(x => x.IncludeInBarcode > 0).ToList();
            }
            else
            {
                lstModuleFields = new List<LabelModuleFieldMasterDTO>();
            }

            lstModuleFields.Insert(0, new LabelModuleFieldMasterDTO() { ID = -1, FieldName = "", FieldDisplayName = ResLabelPrinting.DefaultDropdownText });

            return lstModuleFields;
        }

        private List<KeyValDTO> GetQuantityFieldsList()
        {
            List<KeyValDTO> lstQtyField = new List<KeyValDTO>();
            lstQtyField.Insert(0, new KeyValDTO() { key = "", value = ResLabelPrinting.DefaultDropdownText });
            lstQtyField.Insert(1, new KeyValDTO() { key = "1", value = ResLabelPrinting.Hardcode1 });
            lstQtyField.Insert(2, new KeyValDTO() { key = "DefaultPullQuantity", value = ResLabelPrinting.DefaultPullQuantity });
            lstQtyField.Insert(2, new KeyValDTO() { key = "DefaultReorderQuantity", value = ResLabelPrinting.DefaultReorderQuantity });
            lstQtyField.Insert(3, new KeyValDTO() { key = "BinDefaultPullQuantity", value = ResLabelPrinting.BinDefaultPullQuantity });
            lstQtyField.Insert(4, new KeyValDTO() { key = "BinDefaultReorderQuantity", value = ResLabelPrinting.BinDefaultReorderQuantity });

            return lstQtyField;
        }

        public ActionResult AjaxCatalogReportTemplateList(JQueryDataTableParamModel param)
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
            CatalogReportTemplateMasterDAL objDAL = new CatalogReportTemplateMasterDAL(SessionHelper.EnterPriseDBName);
            //IEnumerable<CatalogReportTemplateMasterDTO> DataFromDB = objDAL.GetMasterDataPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted);
            IEnumerable<CatalogReportTemplateMasterDTO> DataFromDB = objDAL.GetMasterDataPagedRecordsDB(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted);
            
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
        public JsonResult UpdateDataInBaseTemplateMasterTable(CatalogReportTemplateMasterDTO[] objDTO)
        {
            CatalogReportTemplateMasterDAL objDAL = null;
            try
            {
                objDAL = new CatalogReportTemplateMasterDAL(SessionHelper.EnterPriseDBName);
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
        public JsonResult UpdateDataInCurrentDBTemplateMasterTable(CatalogReportTemplateMasterDTO[] objDTO)
        {
            CatalogReportTemplateMasterDAL objDAL = null;
            try
            {
                objDAL = new CatalogReportTemplateMasterDAL(SessionHelper.EnterPriseDBName);
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
            CatalogReportTemplateMasterDAL objDAL = null;
            try
            {
                objDAL = new CatalogReportTemplateMasterDAL(SessionHelper.EnterPriseDBName);
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
        public JsonResult UpdateTemplateMasterDataInCurrentCompany(CatalogReportTemplateMasterDTO[] objDTO)
        {
            CatalogReportTemplateMasterDAL objDAL = null;
            try
            {
                objDAL = new CatalogReportTemplateMasterDAL(SessionHelper.EnterPriseDBName);
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


        public JsonResult GetSortByOptions(string OptionFor)
        {
            List<KeyValDTO> lsttempKeyValDTO = new List<KeyValDTO>();
            if (!string.IsNullOrEmpty(OptionFor) && OptionFor.Trim() == "Location")
            {


                lsttempKeyValDTO.Add(new KeyValDTO() { key = "ItemNumber", value = GetResourceValue("ItemNumber") });
                lsttempKeyValDTO.Add(new KeyValDTO() { key = "SupplierName", value = GetResourceValue("SupplierName") });
                lsttempKeyValDTO.Add(new KeyValDTO() { key = "SupplierPartNo", value = GetResourceValue("SupplierPartNo") });
                lsttempKeyValDTO.Add(new KeyValDTO() { key = "CategoryName", value = GetResourceValue("CategoryName") });
                lsttempKeyValDTO.Add(new KeyValDTO() { key = "DefaultLocationName", value = GetResourceValue("DefaultLocationName") });
                lsttempKeyValDTO.Add(new KeyValDTO() { key = "ManufacturerName", value = GetResourceValue("ManufacturerName") });
                lsttempKeyValDTO.Add(new KeyValDTO() { key = "ManufacturerNumber", value = GetResourceValue("ManufacturerNumber") });
                lsttempKeyValDTO.Add(new KeyValDTO() { key = "UNSPSC", value = GetResourceValue("UNSPSC") });
                lsttempKeyValDTO.Add(new KeyValDTO() { key = "ItemBlanketPO", value = GetResourceValue("ItemBlanketPO") });
                lsttempKeyValDTO.Add(new KeyValDTO() { key = "ItemUDF1", value = GetResourceValue("ItemUDF1") });
                lsttempKeyValDTO.Add(new KeyValDTO() { key = "ItemUDF2", value = GetResourceValue("ItemUDF2") });
                lsttempKeyValDTO.Add(new KeyValDTO() { key = "ItemUDF3", value = GetResourceValue("ItemUDF3") });
                lsttempKeyValDTO.Add(new KeyValDTO() { key = "ItemUDF4", value = GetResourceValue("ItemUDF4") });
                lsttempKeyValDTO.Add(new KeyValDTO() { key = "ItemUDF5", value = GetResourceValue("ItemUDF5") });
                lsttempKeyValDTO.Add(new KeyValDTO() { key = "ItemUDF6", value = GetResourceValue("ItemUDF6") });
                lsttempKeyValDTO.Add(new KeyValDTO() { key = "ItemUDF7", value = GetResourceValue("ItemUDF7") });
                lsttempKeyValDTO.Add(new KeyValDTO() { key = "ItemUDF8", value = GetResourceValue("ItemUDF8") });
                lsttempKeyValDTO.Add(new KeyValDTO() { key = "ItemUDF9", value = GetResourceValue("ItemUDF9") });
                lsttempKeyValDTO.Add(new KeyValDTO() { key = "ItemUDF10", value = GetResourceValue("ItemUDF10") });
                lsttempKeyValDTO.Add(new KeyValDTO() { key = "InventoryClassificationName", value = GetResourceValue("InventoryClassificationName") });
                lsttempKeyValDTO.Add(new KeyValDTO() { key = "ItemDescription", value = ResItemMaster.Description });
                lsttempKeyValDTO.Add(new KeyValDTO() { key = "ItemCost", value = ResRoomMaster.LastCost });
                lsttempKeyValDTO.Add(new KeyValDTO() { key = "CostUOMName", value = ResItemMaster.CostUOMName });
                lsttempKeyValDTO.Add(new KeyValDTO() { key = "ItemCreatedByName", value = ResCommon.CreatedBy });
                lsttempKeyValDTO.Add(new KeyValDTO() { key = "CriticalQuantity", value = ResCommon.Critical });
                lsttempKeyValDTO.Add(new KeyValDTO() { key = "MinimumQuantity", value = ResCommon.Minimum });
                lsttempKeyValDTO.Add(new KeyValDTO() { key = "MaximumQuantity", value = ResCommon.Maximum });
                lsttempKeyValDTO.Add(new KeyValDTO() { key = "BinLocation", value = ResCommon.BinLocation });

                lsttempKeyValDTO = lsttempKeyValDTO.OrderBy(x => x.value).ToList();
            }
            else
            {
                //lsttempKeyValDTO.Add(new KeyValDTO() { key = "ItemNumber", value = "Item #" });
                //lsttempKeyValDTO.Add(new KeyValDTO() { key = "DefaultLocationName", value = "Bin" });

                lsttempKeyValDTO.Add(new KeyValDTO() { key = "CategoryName", value = GetResourceValue("CategoryName") });
                lsttempKeyValDTO.Add(new KeyValDTO() { key = "DefaultLocationName", value = GetResourceValue("DefaultLocationName") });
                lsttempKeyValDTO.Add(new KeyValDTO() { key = "ItemNumber", value = GetResourceValue("ItemNumber") });
                lsttempKeyValDTO.Add(new KeyValDTO() { key = "ManufacturerName", value = GetResourceValue("ManufacturerName") });
                lsttempKeyValDTO.Add(new KeyValDTO() { key = "ManufacturerNumber", value = GetResourceValue("ManufacturerNumber") });
                lsttempKeyValDTO.Add(new KeyValDTO() { key = "SupplierName", value = GetResourceValue("SupplierName") });
                lsttempKeyValDTO.Add(new KeyValDTO() { key = "SupplierPartNo", value = GetResourceValue("SupplierPartNo") });
                lsttempKeyValDTO.Add(new KeyValDTO() { key = "ItemDescription", value = ResItemMaster.Description });
                lsttempKeyValDTO.Add(new KeyValDTO() { key = "ItemCost", value = ResRoomMaster.LastCost });
                lsttempKeyValDTO.Add(new KeyValDTO() { key = "CostUOMName", value = ResItemMaster.CostUOMName });
                lsttempKeyValDTO.Add(new KeyValDTO() { key = "ItemCreatedByName", value = ResCommon.CreatedBy });
                lsttempKeyValDTO.Add(new KeyValDTO() { key = "CriticalQuantity", value = ResCommon.Critical });
                lsttempKeyValDTO.Add(new KeyValDTO() { key = "MinimumQuantity", value = ResCommon.Minimum });
                lsttempKeyValDTO.Add(new KeyValDTO() { key = "MaximumQuantity", value = ResCommon.Maximum });
                lsttempKeyValDTO.Add(new KeyValDTO() { key = "BinLocation", value = ResCommon.BinLocation });

                lsttempKeyValDTO = lsttempKeyValDTO.OrderBy(e => e.value).ToList();
            }

            return Json(new { OptionData = lsttempKeyValDTO }, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public JsonResult UpdateDataInBaseTemplateMasterTable(CatalogReportTemplateMasterDTO[] objDTO)
        //{
        //    CatalogReportTemplateMasterDAL objDAL = null;
        //    try
        //    {
        //        objDAL = new CatalogReportTemplateMasterDAL(SessionHelper.EnterPriseDBName);
        //        if (objDTO != null && objDTO.Length > 0)
        //        {
        //            foreach (var item in objDTO)
        //            {
        //                objDAL.EditInBaseDB(item);
        //            }
        //        }
        //        return Json(new { Massage = "Data Saved", Status = true }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        objDAL = null;
        //    }

        //}


        #region Generate CatalogReport


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

        private const float TableCellBorderWidth = 0.25f;
        private const float BorderWidth = 0.25f;

        CatalogReportTemplateMasterDTO objTemplateDTO = new CatalogReportTemplateMasterDTO();

        [HttpPost]
        public FileStreamResult GenerateCatalogReport(FormCollection frm)
        {
            CatalogReportDAL objDal = new CatalogReportDAL(SessionHelper.EnterPriseDBName);
            CatalogReportDetailDTO objDetail = null;
            Int64 ID = 0;
            Int64.TryParse(frm["hdnCatalogReportID"], out ID);
            bool isInActiveItems = false;
            CatalogReportDTO objCRDTO = new CatalogReportDTO();
            string ItemIDs = "";
            string callFrom = "CR";

            callFrom = frm["hdncallFrom"] ?? "CR";

            if (ID == 0 && callFrom == "ItemList")
            {
                objDetail = objDal.GetDefaultCatalogReport(SessionHelper.CompanyID);
                ItemIDs = frm["hdnSelectedItemIDs"];
                isInActiveItems = false;
            }
            else
            {
                isInActiveItems = Convert.ToBoolean(frm["chkIncludeInactiveItems"].Split(',')[0]);
                ItemIDs = "";
                //objDetail = objDal.GetRecord(ID, SessionHelper.CompanyID);
                objDetail = objDal.GetCatalogReportDetailByID(ID, SessionHelper.CompanyID);
            }





            string groupName = frm["ddlGroupName"];
            if (!string.IsNullOrEmpty(groupName))
            {
                if (groupName == "Manufacturer")
                    groupName = "ManufacturerName";
                else if (groupName == "Manufacturer#")
                    groupName = "ManufacturerNumber";
                else if (groupName == "Supplier")
                    groupName = "SupplierName";
                else if (groupName == "Supplier#")
                    groupName = "SupplierPartNo";
                else if (groupName == "Category")
                    groupName = "CategoryName";
                else if (groupName == "Category")
                    groupName = "CategoryName";
                else if (groupName == "Location")
                    groupName = "DefaultLocationName";
                else if (groupName == "BinNumber")
                    groupName = "BinNumber";
            }

            string sortField = frm["ddlSort"];
            string sortorderby = frm["ddlSortAscDesc"];

            if (string.IsNullOrEmpty(sortField))
                sortField = "ItemNumber";
            else
            {
                if (sortField == "DefaultLocationName" && groupName == "BinNumber")
                {
                    sortField = "BinNumber";
                }

                sortField = sortField + " " + sortorderby;
            }


            Dictionary<string, List<CatalogReportFieldsDTO>> dicCatalogs = new Dictionary<string, List<CatalogReportFieldsDTO>>();

            if (!string.IsNullOrEmpty(groupName))
            {
                IEnumerable<string> lstGrpList = objDal.GetListByGrouping(groupName, "", SessionHelper.RoomID, ItemIDs);
                foreach (var item in lstGrpList)
                {
                    List<CatalogReportFieldsDTO> lstItems = objDal.GetItemsByGroupNameAndId(item, groupName, "", SessionHelper.RoomID, sortField, isInActiveItems, ItemIDs: ItemIDs).ToList();
                    dicCatalogs.Add(item, lstItems);
                }
            }
            else
            {
                List<CatalogReportFieldsDTO> lstItems = objDal.GetItemsByGroupNameAndId(SessionHelper.RoomID, sortField, isInActiveItems, ItemIDs: ItemIDs).ToList();
                //dicCatalogs.Add(" ", lstItems);
                if (lstItems != null && lstItems.Count > 0)
                    dicCatalogs.Add(" ", lstItems);
            }

            return GenerateLabelsPDF(frm["ddlGroupName"], dicCatalogs, objDetail);
            //  return Json(new { Message = "Success", Status = true }, JsonRequestBehavior.AllowGet);
        }
        private PdfPTable GetHeaderTable(string GroupByName, string keyValue, CatalogReportDetailDTO objFMTDetailDTO)
        {
            PdfPTable tableHead = null;
            tableHead = new PdfPTable(1) { WidthPercentage = 100 };
            tableHead.SetWidths(new float[] { 100 });
            tableHead.AddCell(GetHeaderCell(ResCommon.RoomColon + " " + SessionHelper.RoomName, objFMTDetailDTO, BaseColor.BLUE, BaseColor.WHITE));

            PdfPCell cellSpace = new PdfPCell();
            cellSpace.FixedHeight = ConvertInchToPoint(0.05);
            cellSpace.Border = 0;

            tableHead.AddCell(cellSpace);
            if (!string.IsNullOrEmpty(GroupByName))
            {
                tableHead.AddCell(GetHeaderCell(GroupByName + ": " + keyValue, objFMTDetailDTO, BaseColor.GRAY, BaseColor.WHITE));
                tableHead.AddCell(cellSpace);
            }
            tableHead.CompleteRow();
            return tableHead;
        }
        private PdfPCell GetHeaderCell(string cellText, CatalogReportDetailDTO objFMTDetailDTO, BaseColor bgcolor, BaseColor fontColor)
        {
            PdfPCell tabelCell = new PdfPCell();

            tabelCell.Border = 0;
            tabelCell.BorderWidthTop = 0.00F;
            tabelCell.BorderWidthLeft = 0.00F;
            tabelCell.BorderWidthRight = 0.00F;
            tabelCell.BorderWidthBottom = 0.00F;
            tabelCell.BorderColor = BaseColor.BLACK;

            tabelCell.PaddingLeft = ConvertInchToPoint(0.10F);
            tabelCell.PaddingRight = ConvertInchToPoint(0.10F);
            tabelCell.PaddingTop = ConvertInchToPoint(0.010F);
            tabelCell.PaddingBottom = ConvertInchToPoint(0.040F);

            tabelCell.FixedHeight = ConvertInchToPoint(0.30F);
            tabelCell.BackgroundColor = bgcolor;

            tabelCell.VerticalAlignment = Element.ALIGN_TOP;
            tabelCell.HorizontalAlignment = Element.ALIGN_LEFT;

            var font = FontFactory.GetFont(objFMTDetailDTO.TextFont, Convert.ToSingle(objFMTDetailDTO.FontSize), Font.BOLD, fontColor);
            Chunk ch = new Chunk(cellText, font);
            ch.SetLocalDestination(cellText);
            ch.SetGenericTag(cellText);
            ch.SetLocalGoto(cellText);

            Paragraph p = new Paragraph(ch);
            tabelCell.AddElement(p);

            return tabelCell;

        }
        private void GetIndexTable(Dictionary<string, List<CatalogReportFieldsDTO>> objDATA, CatalogReportDetailDTO objFMTDetailDTO, CatalogReportTemplateMasterDTO TemplateDTO, ref PdfWriter writer, ref Document doc)
        {
            Dictionary<string, int> entries = GetIndexArray(objDATA, objFMTDetailDTO, TemplateDTO);
            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
            Font fIdxName = new Font(bf, 11, 1, BaseColor.BLUE);
            Font f = new Font(bf, 9);


            PdfPTable tbl2 = null;
            int idx = 1;
            Chunk dottedLine = new Chunk(new iTextSharp.text.pdf.draw.DottedLineSeparator());
            Paragraph p;

            foreach (var item in entries)
            {
                if (idx == 1)
                {
                    if (tbl2 != null)
                    {
                        tbl2.CompleteRow();
                        doc.Add(tbl2);
                    }
                    doc.NewPage();

                    var tblTitle = new PdfPTable(1) { WidthPercentage = 100 };
                    tblTitle.SetWidths(new float[] { 100 });
                    var titleCell = new PdfPCell(new Phrase("  " + ResLabelPrinting.TableOfContents + "  ", fIdxName)); 
                    titleCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    titleCell.Border = 0;
                    tblTitle.AddCell(titleCell);
                    tblTitle.CompleteRow();
                    doc.Add(tblTitle);
                    tbl2 = new PdfPTable(2) { WidthPercentage = 100 };
                    tbl2.SetWidths(new float[] { 95, 5 });

                }
                Chunk ch = new Chunk(item.Key, f);
                float magnifyOpt = 100.0F;
                PdfDestination magnify = new PdfDestination(PdfDestination.XYZ, -1, -1, magnifyOpt / 100);
                PdfAction pdfAc = PdfAction.GotoLocalPage(item.Value, magnify, writer);
                ch.SetAction(pdfAc);
                p = new Paragraph(ch);
                p.Add(dottedLine);

                var Cell01 = new PdfPCell(p);
                Cell01.AddElement(p);
                Cell01.HorizontalAlignment = Element.ALIGN_LEFT;
                Cell01.Border = 0;
                Cell01.VerticalAlignment = Element.ALIGN_TOP;
                Cell01.FixedHeight = 20.0F;
                tbl2.AddCell(Cell01);


                var Cell02 = new PdfPCell(new Phrase(item.Value.ToString(), f));
                Cell02.HorizontalAlignment = Element.ALIGN_RIGHT;
                Cell02.Border = 0;
                Cell02.VerticalAlignment = Element.ALIGN_BOTTOM;
                Cell02.FixedHeight = 20.0F;
                tbl2.AddCell(Cell02);
                tbl2.CompleteRow();
                idx += 1;

                if (idx > 36)
                    idx = 1;

            }
            if (tbl2 != null)
            {
                tbl2.CompleteRow();
                doc.Add(tbl2);
            }
        }
        private Dictionary<string, int> GetIndexArray(Dictionary<string, List<CatalogReportFieldsDTO>> objDATA, CatalogReportDetailDTO objCatalogReportData, CatalogReportTemplateMasterDTO TemplateDTO)
        {
            Dictionary<string, int> entries = new Dictionary<string, int>();
            Dictionary<string, int> returnIndexes = new Dictionary<string, int>();
            int index = 2;
            //int nofp = 1;
            foreach (var item in objDATA)
            {
                if (item.Value != null && item.Value.Count > 0)
                {
                    entries.Add(item.Key.ToString(), index);
                    int nofp = (int)(Math.Ceiling((double)item.Value.Count / (double)TemplateDTO.NoOfLabelPerSheet));
                    index += nofp;

                }
            }
            int Indx = (int)Math.Ceiling((double)entries.Count / (double)36);

            foreach (var item in entries)
            {
                if (Indx > 1)
                {
                    returnIndexes.Add(item.Key.ToString(), ((item.Value + Indx) - 1));
                }
                else
                {
                    returnIndexes.Add(item.Key, item.Value);

                }
            }

            return returnIndexes;
        }
        private PdfPTable GetIndexTable(string GroupByName, Dictionary<string, List<CatalogReportFieldsDTO>> objDATA, CatalogReportDetailDTO objFMTDetailDTO, CatalogReportTemplateMasterDTO TemplateDTO, PdfWriter writer)
        {
            Dictionary<string, int> entries = GetIndexArray(objDATA, objFMTDetailDTO, TemplateDTO);
            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
            Font fIdxName = new Font(bf, 11, 1, BaseColor.BLUE);
            Font f = new Font(bf, 9);

            var tbl2 = new PdfPTable(2) { WidthPercentage = 99 };
            tbl2.SetWidths(new float[] { 94, 5 });

            var titleCell = new PdfPCell(new Phrase("  " + ResLabelPrinting.TableOfContents + "  ", fIdxName));
            titleCell.HorizontalAlignment = Element.ALIGN_CENTER;
            titleCell.Border = 0;
            titleCell.Colspan = 2;
            tbl2.AddCell(titleCell);
            tbl2.CompleteRow();

            Chunk dottedLine = new Chunk(new iTextSharp.text.pdf.draw.DottedLineSeparator());
            Paragraph p;

            foreach (var item in entries)
            {
                Chunk ch = new Chunk(item.Key, f);

                float magnifyOpt = 100.0F;
                PdfDestination magnify = new PdfDestination(PdfDestination.XYZ, -1, -1, magnifyOpt / 100);
                PdfAction pdfAc = PdfAction.GotoLocalPage(item.Value, magnify, writer);
                ch.SetAction(pdfAc);
                p = new Paragraph(ch);
                p.Add(dottedLine);

                var Cell01 = new PdfPCell(p);
                Cell01.AddElement(p);
                Cell01.HorizontalAlignment = Element.ALIGN_LEFT;
                Cell01.Border = 0;
                Cell01.VerticalAlignment = Element.ALIGN_TOP;
                Cell01.FixedHeight = 20.0F;
                tbl2.AddCell(Cell01);


                var Cell02 = new PdfPCell(new Phrase(item.Value.ToString(), f));
                Cell02.HorizontalAlignment = Element.ALIGN_RIGHT;
                Cell02.Border = 0;
                Cell02.VerticalAlignment = Element.ALIGN_BOTTOM;
                Cell02.FixedHeight = 20.0F;
                tbl2.AddCell(Cell02);
                tbl2.CompleteRow();
            }

            return tbl2;
        }
        public FileStreamResult GenerateLabelsPDF(string GroupByName, Dictionary<string, List<CatalogReportFieldsDTO>> objDATA, CatalogReportDetailDTO objFMTDetailDTO)
        {

            _CostDecimalFormat = CostDecimalFormat;
            _QtyDecimalFormat = QtyDecimalFormat;

            CurrentIndex = 1;
            CatalogReportTemplateMasterDAL objTMDAL = null;
            CatalogReportTemplateMasterDTO objTemplateDTO = null;
            MemoryStream fs = null;
            Document doc = null;
            PdfWriter pdfWriter = null;
            List<float> lstFL = null;
            PdfPTable table = null;
            try
            {
                fs = new MemoryStream();
                objTMDAL = new CatalogReportTemplateMasterDAL(SessionHelper.EnterPriseDBName);
                //objTemplateDTO = objTMDAL.GetRecord(objFMTDetailDTO.TemplateID, SessionHelper.CompanyID);
                objTemplateDTO = objTMDAL.GetCatalogReportTemplateMasterByCompanyTemplateID(objFMTDetailDTO.TemplateID, SessionHelper.CompanyID);

                objFMTDetailDTO.lstModuleFields = GetCatalogReportFields();
                string[] ints = objFMTDetailDTO.SelectedFields.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                List<KeyValDTO> lstKeyVal = new List<KeyValDTO>();
                if (ints != null && ints.Length > 0)
                {
                    foreach (var item in ints)
                    {
                        KeyValDTO keyVal = objFMTDetailDTO.lstModuleFields.FirstOrDefault(x => x.key == item);
                        lstKeyVal.Add(keyVal);
                    }
                }

                objFMTDetailDTO.lstSelectedModuleFields = lstKeyVal;

                float fltPageWidth = ConvertInchToPoint(objTemplateDTO.PageWidth);
                float fltPageHeight = ConvertInchToPoint(objTemplateDTO.PageHeight);
                float fltPageLeftMarging = ConvertInchToPoint(objTemplateDTO.PageMarginLeft);
                float fltPageRightMarging = ConvertInchToPoint(objTemplateDTO.PageMarginRight);
                float fltPageTopMarging = ConvertInchToPoint(objTemplateDTO.PageMarginTop);
                float fltPageBottomMarging = ConvertInchToPoint(objTemplateDTO.PageMarginBottom);

                //int IndexPagesCount = GetNoOfIndexPages(objDATA, objFMTDetailDTO, objTemplateDTO);
                int NoOfCell = 0;

                doc = new Document(new Rectangle(fltPageWidth, fltPageHeight));
                doc.SetMargins(fltPageLeftMarging, fltPageRightMarging, fltPageTopMarging, fltPageBottomMarging);

                pdfWriter = PdfWriter.GetInstance(doc, fs);
                PageEventHelper pageEventHelper = new PageEventHelper(0, 1);
                pdfWriter.PageEvent = pageEventHelper;
                doc.Open();

                if (!string.IsNullOrEmpty(GroupByName))
                    GetIndexTable(objDATA, objFMTDetailDTO, objTemplateDTO, ref pdfWriter, ref doc);

                //PdfPTable IndxTable = GetIndexTable(GroupByName, objDATA, objFMTDetailDTO, objTemplateDTO, pdfWriter);
                //IndxTable.CompleteRow();
                // doc.Add(IndxTable);

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

                if (objDATA != null && objDATA.Count > 0)
                {
                    CurrentRowIndex = 1;
                    NoOfCell = 1;
                    foreach (var dicItem in objDATA)
                    {
                        List<CatalogReportFieldsDTO> lstObjs = objDATA[dicItem.Key];
                        if (lstObjs == null & lstObjs.Count <= 0)
                            continue;

                        CurrentRowIndex = 1;
                        NoOfCell = 1;
                        CurrentIndex = 1;
                        foreach (CatalogReportFieldsDTO item in lstObjs)
                        {
                            if (NoOfCell == 1)
                            {
                                if (table != null)
                                {
                                    table.CompleteRow();
                                    doc.Add(table);
                                }
                                doc.NewPage();
                                pageEventHelper.onGenericTag(pdfWriter, doc, new Rectangle(100, 20), dicItem.Key);
                                doc.Add(GetHeaderTable(GroupByName, dicItem.Key, objFMTDetailDTO));

                                table = new PdfPTable(lstFL.Count());
                                table.WidthPercentage = 100f;
                                table.HorizontalAlignment = 0;

                                table.SetWidths(lstFL.ToArray<float>());
                                table.DefaultCell.Border = 0;

                                table.SkipFirstHeader = true;
                                table.SkipLastFooter = true;
                                NoOfCell = 1;
                            }

                            if (objTemplateDTO.NoOfColumns > 1 && CurrentIndex % 2 == 0)
                            {
                                table.AddCell(GetHorizontalSpaceTabelCell(objTemplateDTO));
                            }

                            PdfPCell cell = GetTabelCell(objTemplateDTO);

                            cell.AddElement(GetCellContentFromXMLNew(objTemplateDTO, item, objFMTDetailDTO));
                            //cell.AddElement(new Phrase("NoOfCell=" + NoOfCell));
                            table.AddCell(cell);

                            NoOfCell += 1;
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
                                }

                                if (CurrentRowIndex * objTemplateDTO.NoOfColumns == objTemplateDTO.NoOfLabelPerSheet)
                                    CurrentRowIndex = 1;


                                CurrentIndex = 1;
                                CurrentIndexVS = 1;
                                CurrentRowIndex += 1;
                            }

                            if (NoOfCell > objTemplateDTO.NoOfLabelPerSheet)
                            {
                                NoOfCell = 1;
                            }

                        }
                    }
                }
                else
                {
                    table = new PdfPTable(1);
                    table.AddCell(new PdfPCell(new Phrase(ResLabelPrinting.MsgNotDataFound)));
                }
                if (table != null)
                {
                    table.CompleteRow();

                    doc.Add(table);
                }


                pdfWriter.CloseStream = false;
                doc.Close();
                pdfWriter.Dispose();
                fs.Position = 0;
                return File(fs, "application/pdf");


            }
            finally
            {
                fs = null;
                pdfWriter.Flush();
                if (pdfWriter != null)
                    pdfWriter.Dispose();
                doc.Dispose();
                doc = null;

                pdfWriter = null;
                objTMDAL = null;
                objFMTDetailDTO = null;
                objTemplateDTO = null;
                lstFL = null;
                table = null;
            }

        }


        //public FileStreamResult GenerateLabelsPDF(Dictionary<string, List<CatalogReportFieldsDTO>> objDATA, CatalogReportDetailDTO objCatalogReportData)
        //{
        //    _CostDecimalFormat = CostDecimalFormat;
        //    _QtyDecimalFormat = QtyDecimalFormat;
        //    CurrentIndex = 1;

        //    MemoryStream fs = null;
        //    MemoryStream fsIndex = null;
        //    Document doc = null;
        //    PdfWriter pdfWriter = null;
        //    PdfWriter pdfWriterIndex = null;

        //    List<float> lstFL = null;
        //    PdfPTable table = null;
        //    Dictionary<string, int> dicGroupPages = new Dictionary<string, int>();
        //    // Int64[] arrROTDIDs = null;
        //    try
        //    {

        //        CatalogReportTemplateMasterDAL objTMDAL = new CatalogReportTemplateMasterDAL(SessionHelper.EnterPriseDBName);
        //        fs = new MemoryStream();

        //        objTemplateDTO = objTMDAL.GetRecord(objCatalogReportData.TemplateID, SessionHelper.CompanyID);
        //        int IndexPagesCount = GetNoOfIndexPages(objDATA, objCatalogReportData, objTemplateDTO);


        //        double fltLabelHeight = objTemplateDTO.LabelHeight;
        //        float fltPageWidth = ConvertInchToPoint(objTemplateDTO.PageWidth);
        //        float fltPageHeight = ConvertInchToPoint(objTemplateDTO.PageHeight);
        //        doc = new Document(new Rectangle(fltPageWidth, fltPageHeight));
        //        float fltPageLeftMarging = ConvertInchToPoint(objTemplateDTO.PageMarginLeft);
        //        float fltPageRightMarging = ConvertInchToPoint(objTemplateDTO.PageMarginRight);
        //        float fltPageTopMarging = ConvertInchToPoint(objTemplateDTO.PageMarginTop);
        //        float fltPageBottomMarging = ConvertInchToPoint(objTemplateDTO.PageMarginBottom);
        //        doc.SetMargins(fltPageLeftMarging, fltPageRightMarging, fltPageTopMarging, fltPageBottomMarging);

        //        pdfWriter = PdfWriter.GetInstance(doc, fs);
        //        PageEventHelper pageEventHelper = new PageEventHelper(IndexPagesCount, 0);
        //        pdfWriter.PageEvent = pageEventHelper;

        //        doc.Open();



        //        lstFL = new List<float>();
        //        float fltLabelWidth = 0;
        //        for (int i = 0; i < objTemplateDTO.NoOfColumns; i++)
        //        {
        //            fltLabelWidth = ConvertInchToPoint(objTemplateDTO.LabelWidth);
        //            lstFL.Add(fltLabelWidth);
        //            if (objTemplateDTO.LabelSpacingHorizontal > 0 && i < objTemplateDTO.NoOfColumns - 1)
        //            {
        //                fltLabelWidth = ConvertInchToPoint(objTemplateDTO.LabelSpacingHorizontal);
        //                lstFL.Add(fltLabelWidth);
        //            }
        //        }

        //        if (objDATA != null & objDATA.Count > 0)
        //        {
        //            foreach (var dicItem in objDATA)
        //            {
        //                List<CatalogReportFieldsDTO> lstObjs = objDATA[dicItem.Key];
        //                if (lstObjs.Count <= 0)
        //                    continue;

        //                //for (int i = 0; i < 2; i++)
        //                //{
        //                //    objTemplateDTO.LabelHeight = 0.5F;
        //                //    PdfPCell cell = GetTabelCell(objTemplateDTO);
        //                //    cell.AddElement(new Phrase(SessionHelper.RoomName));
        //                //    table.AddCell(cell);
        //                //}

        //                //pageEventHelper.OnGenericTag(pdfWriter, doc, new Rectangle(100, 20), dicItem.Key.ToString() + " " + lstObjs.FirstOrDefault().SupplierName);
        //                CurrentRowIndex = 1;
        //                foreach (CatalogReportFieldsDTO item in lstObjs)
        //                {
        //                    if (CurrentRowIndex == 1)
        //                    {
        //                        table = new PdfPTable(lstFL.Count());
        //                        table.WidthPercentage = 100f;
        //                        table.HorizontalAlignment = 0;

        //                        table.SetWidths(lstFL.ToArray<float>());
        //                        table.DefaultCell.Border = 0;

        //                        table.SkipFirstHeader = true;
        //                        table.SkipLastFooter = true;
        //                        doc.NewPage();
        //                        pageEventHelper.onGenericTag(pdfWriter, doc, new Rectangle(100, 20), dicItem.Key);

        //                        for (int i = 0; i < 2; i++)
        //                        {
        //                            PdfPCell cellHead = null;

        //                            cellHead = GetTabelCell(objTemplateDTO);
        //                            cellHead.PaddingLeft = ConvertInchToPoint(0.10);
        //                            cellHead.PaddingRight = ConvertInchToPoint(0);
        //                            cellHead.PaddingTop = ConvertInchToPoint(0);
        //                            cellHead.PaddingBottom = ConvertInchToPoint(0.06);

        //                            cellHead.FixedHeight = ConvertInchToPoint(0.3);

        //                            if (i == 0)
        //                            {
        //                                var redFont = FontFactory.GetFont(objCatalogReportData.TextFont, Convert.ToSingle(objCatalogReportData.FontSize), Font.BOLD, BaseColor.WHITE);
        //                                cellHead.AddElement(new Phrase(SessionHelper.RoomName, redFont));
        //                                cellHead.BackgroundColor = CMYKColor.BLUE;
        //                                cellHead.Border = 0;
        //                                table.AddCell(cellHead);

        //                                cellHead = new PdfPCell();
        //                                cellHead.FixedHeight = ConvertInchToPoint(0.05);
        //                                cellHead.Border = 0;
        //                                cellHead.BorderWidthTop = 0;
        //                                cellHead.BorderWidthLeft = 0;
        //                                cellHead.BorderWidthRight = 0;
        //                                cellHead.BorderWidthBottom = 0;
        //                                table.AddCell(cellHead);
        //                            }
        //                            else
        //                            {
        //                                var redFont = FontFactory.GetFont(objCatalogReportData.TextFont, Convert.ToSingle(objCatalogReportData.FontSize), Font.BOLD, BaseColor.BLACK);
        //                                Anchor target = new Anchor(dicItem.Key, redFont);
        //                                target.Name = dicItem.Key;
        //                                //target.Reference = "#" + dicItem.Key;
        //                                Paragraph p3 = new Paragraph();
        //                                p3.Add(target);

        //                                //cellHead.AddElement(new Phrase(dicItem.Key, redFont));
        //                                cellHead.AddElement(p3);
        //                                cellHead.Border = 0;
        //                                cellHead.BackgroundColor = BaseColor.LIGHT_GRAY;
        //                                table.AddCell(cellHead);
        //                            }

        //                        }
        //                    }

        //                    objTemplateDTO.LabelHeight = fltLabelHeight;
        //                    PdfPCell cell = GetTabelCell(objTemplateDTO);
        //                    if (!string.IsNullOrEmpty(objCatalogReportData.LabelXML) && objCatalogReportData.LabelXML.Trim().Length > 0)
        //                    {
        //                        cell.AddElement(GetCellContentFromXMLNew(objTemplateDTO, item, objCatalogReportData));
        //                        //cell.AddElement(new Phrase(CurrentRowIndex.ToString()));
        //                        PdfPCell cellHead = new PdfPCell();
        //                        cellHead.FixedHeight = ConvertInchToPoint(0.06);
        //                        cellHead.Border = 0;
        //                        cellHead.BorderWidthTop = 0;
        //                        cellHead.BorderWidthLeft = 0;
        //                        cellHead.BorderWidthRight = 0;
        //                        cellHead.BorderWidthBottom = 0;
        //                        table.AddCell(cellHead);
        //                    }

        //                    table.AddCell(cell);

        //                    if (objTemplateDTO.NoOfLabelPerSheet <= CurrentRowIndex)
        //                        CurrentRowIndex = 1;
        //                    else
        //                        CurrentRowIndex += 1;

        //                    if (CurrentRowIndex == 1)
        //                    {
        //                        table.CompleteRow();
        //                        doc.Add(table);
        //                    }
        //                }

        //                table.CompleteRow();
        //                doc.Add(table);


        //            }
        //        }
        //        else
        //        {
        //            table.AddCell(new PdfPCell(new Phrase("No Data Found")));
        //        }

        //        table.CompleteRow();
        //        //doc.Add(table);
        //        pdfWriter.CloseStream = false;
        //        doc.Close();

        //        //-----------------------------------

        //        Dictionary<string, int> entries = pageEventHelper.getTOC();
        //        Document docIndex = new Document(new Rectangle(fltPageWidth, fltPageHeight));
        //        fsIndex = new MemoryStream();
        //        docIndex.SetMargins(fltPageLeftMarging, fltPageRightMarging, fltPageTopMarging, fltPageBottomMarging);
        //        PdfReader rdr = new PdfReader(fs.ToArray());

        //        pdfWriterIndex = PdfWriter.GetInstance(docIndex, fsIndex);
        //        PageEventHelper pageIndexEventHelper = new PageEventHelper(0, rdr.NumberOfPages);
        //        pdfWriterIndex.PageEvent = pageIndexEventHelper;

        //        docIndex.Open();
        //        docIndex.NewPage();
        //        //doc.NewPage();


        //        pageEventHelper = null;

        //        BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
        //        Font f = new Font(bf, 9);


        //        Font fIdxName = new Font(bf, 11, 1, BaseColor.BLUE);


        //        Font fTitle = new Font(bf, 12, 2, BaseColor.BLUE);

        //        var tbl1 = new PdfPTable(1) { WidthPercentage = 100 };
        //        tbl1.SetWidths(new float[] { 100 });
        //        var Cell01 = new PdfPCell(new Phrase("  Table of Contents  ", fIdxName));
        //        Cell01.HorizontalAlignment = Element.ALIGN_CENTER;
        //        Cell01.Border = 0;
        //        tbl1.AddCell(Cell01);
        //        tbl1.CompleteRow();
        //        docIndex.Add(tbl1);
        //        var tbl2 = new PdfPTable(2) { WidthPercentage = 100 };
        //        tbl2.SetWidths(new float[] { 95, 5 });

        //        Chunk dottedLine = new Chunk(new iTextSharp.text.pdf.draw.DottedLineSeparator());
        //        Paragraph p;
        //        foreach (var item in entries)
        //        {
        //            Chunk ch = new Chunk(item.Key, f);
        //            p = new Paragraph(ch);
        //            p.Add(dottedLine);
        //            //ch.SetAction(PdfAction.GotoLocalPage(item.Key, false));
        //            //ch.SetAction(PdfAction.GotoLocalPage(item.Value, new PdfDestination(0), pdfWriter));

        //            var Cell02 = new PdfPCell(p);

        //            Cell02.HorizontalAlignment = Element.ALIGN_LEFT;
        //            Cell02.Border = 0;
        //            Cell02.FixedHeight = 6F;
        //            tbl2.AddCell(Cell02);

        //            var Cell04 = new PdfPCell(new Phrase(item.Value.ToString(), f));
        //            Cell04.HorizontalAlignment = Element.ALIGN_RIGHT;
        //            Cell04.Border = 0;
        //            tbl2.AddCell(Cell04);
        //            tbl1.CompleteRow();


        //        }

        //        docIndex.Add(tbl2);
        //        pdfWriterIndex.CloseStream = false;
        //        pdfWriter.CloseStream = false;
        //        doc.Close();

        //        docIndex.Close();
        //        fsIndex.Position = 0;
        //        fs.Position = 0;
        //        MemoryStream streamMerge = new MemoryStream();
        //        Document docMerge = new Document(new Rectangle(fltPageWidth, fltPageHeight));
        //        docMerge.SetMargins(fltPageLeftMarging, fltPageRightMarging, fltPageTopMarging, fltPageBottomMarging);
        //        PdfCopy pdf = new PdfCopy(docMerge, streamMerge);
        //        pdf.Open();
        //        docMerge.Open();

        //        PdfReader reader = null;
        //        PdfImportedPage page = null;
        //        reader = new PdfReader(fsIndex.ToArray());
        //        for (int i = 0; i < reader.NumberOfPages; i++)
        //        {
        //            page = pdf.GetImportedPage(reader, i + 1);
        //            pdf.AddPage(page);
        //        }
        //        pdf.FreeReader(reader);
        //        reader.Close();

        //        reader = null;
        //        page = null;

        //        reader = new PdfReader(fs.ToArray());
        //        for (int i = 0; i < reader.NumberOfPages; i++)
        //        {
        //            page = pdf.GetImportedPage(reader, i + 1);
        //            pdf.AddPage(page);
        //        }
        //        pdf.FreeReader(reader);
        //        reader.Close();

        //        pdf.CloseStream = false;
        //        pdf.Close();
        //        pdf.Dispose();
        //        pdfWriterIndex.Dispose();
        //        pdfWriter.Dispose();
        //        docMerge.Close();
        //        streamMerge.Position = 0;



        //        return File(streamMerge, "application/pdf");

        //    }
        //    finally
        //    {

        //        fs = null;
        //        doc = null;
        //        pdfWriter = null;


        //        lstFL = null;
        //        table = null;
        //    }

        //}


        //private int GetNoOfIndexPages(Dictionary<string, List<CatalogReportFieldsDTO>> objDATA, CatalogReportDetailDTO objCatalogReportData, CatalogReportTemplateMasterDTO TemplateDTO, ref PdfDocument doc)
        //{
        //    Dictionary<string, int> entries = GetIndexArray(objDATA, objCatalogReportData, TemplateDTO);

        //    int NoOfIndexPages = 0;
        //    MemoryStream fsIndex = null;
        //    PdfWriter pdfWriterIndex = null;
        //    float fltPageWidth = ConvertInchToPoint(TemplateDTO.PageWidth);
        //    float fltPageHeight = ConvertInchToPoint(TemplateDTO.PageHeight);
        //    float fltPageLeftMarging = ConvertInchToPoint(TemplateDTO.PageMarginLeft);
        //    float fltPageRightMarging = ConvertInchToPoint(TemplateDTO.PageMarginRight);
        //    float fltPageTopMarging = ConvertInchToPoint(TemplateDTO.PageMarginTop);
        //    float fltPageBottomMarging = ConvertInchToPoint(TemplateDTO.PageMarginBottom);

        //    //START============ Count PDF Inex Pages ==================
        //    Document docIndex = new Document(new Rectangle(fltPageWidth, fltPageHeight));
        //    docIndex.SetMargins(fltPageLeftMarging, fltPageRightMarging, fltPageTopMarging, fltPageBottomMarging);
        //    fsIndex = new MemoryStream();
        //    pdfWriterIndex = PdfWriter.GetInstance(docIndex, fsIndex);
        //    docIndex.Open();
        //    docIndex.NewPage();

        //    BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
        //    Font f = new Font(bf, 9);
        //    Font fIdxName = new Font(bf, 11, 1, BaseColor.BLUE);
        //    Font fTitle = new Font(bf, 12, 2, BaseColor.BLUE);
        //    var tbl1 = new PdfPTable(1) { WidthPercentage = 100 };
        //    tbl1.SetWidths(new float[] { 100 });
        //    var Cell01 = new PdfPCell(new Phrase("  Table of Contents  ", fIdxName));
        //    Cell01.HorizontalAlignment = Element.ALIGN_CENTER;
        //    Cell01.Border = 0;
        //    tbl1.AddCell(Cell01);
        //    tbl1.CompleteRow();
        //    docIndex.Add(tbl1);
        //    var tbl2 = new PdfPTable(2) { WidthPercentage = 100 };
        //    tbl2.SetWidths(new float[] { 95, 5 });
        //    int pageIndex = 1;
        //    Chunk dottedLine = new Chunk(new iTextSharp.text.pdf.draw.DottedLineSeparator());
        //    Paragraph p;
        //    foreach (var item in entries)
        //    {
        //        Chunk ch = new Chunk(item.Key, f);
        //        p = new Paragraph(ch);
        //        p.Add(dottedLine);
        //        var Cell02 = new PdfPCell(p);
        //        Cell02.HorizontalAlignment = Element.ALIGN_LEFT;
        //        Cell02.Border = 0;
        //        Cell02.FixedHeight = 6F;
        //        tbl2.AddCell(Cell02);
        //        var Cell04 = new PdfPCell(new Phrase(item.Value.ToString(), f));
        //        Cell04.HorizontalAlignment = Element.ALIGN_RIGHT;
        //        Cell04.Border = 0;
        //        tbl2.AddCell(Cell04);
        //        tbl1.CompleteRow();
        //        pageIndex += 1;

        //        if (pageIndex > 30)
        //        {
        //            doc.NewPage();
        //            pageIndex = 1;
        //        }

        //    }

        //    docIndex.Add(tbl2);
        //    pdfWriterIndex.CloseStream = false;
        //    docIndex.Close();
        //    fsIndex.Position = 0;
        //    PdfReader reader = new PdfReader(fsIndex.ToArray());
        //    NoOfIndexPages = reader.NumberOfPages;

        //    return NoOfIndexPages;

        //    //END============ Count PDF Inex Pages ==================
        //}


        /// <summary>
        /// GetCellContent
        /// </summary>
        /// <param name="objLabelSize"></param>
        /// <param name="item"></param>
        /// <param name="objMMDTO"></param>
        /// <param name="objLFMTDetailDTO"></param>
        /// <returns></returns>
        private PdfPTable GetCellContentFromXMLNew(CatalogReportTemplateMasterDTO objLabelSize, CatalogReportFieldsDTO item, CatalogReportDetailDTO objLFMTDetailDTO)
        {

            XmlDocument objDoc = new XmlDocument();
            
            if (objLFMTDetailDTO == null || string.IsNullOrEmpty(objLFMTDetailDTO.LabelXML) || string.IsNullOrWhiteSpace(objLFMTDetailDTO.LabelXML))
            {
                return new PdfPTable(1);
            }
            
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

            PdfPTable InnerTable = null;
            PdfPCell InnerTableCell = null;
            Font cellFont = null;

            try
            {
                InnerTable = new PdfPTable(MaxNoOfCols);
                InnerTable.WidthPercentage = 100f;
                InnerTable.TotalWidth = (float)intWidth;
                InnerTable.LockedWidth = true;

                InnerTable.HorizontalAlignment = 0;
                InnerTable.DefaultCell.Border = 0;

                //InnerTable.SetWidths(lstCellWithed.ToArray<float>());

                InnerTable.SkipFirstHeader = true;
                InnerTable.SkipLastFooter = true;

                string strfName = "Verdana";
                if (!string.IsNullOrEmpty(objLFMTDetailDTO.TextFont))
                    strfName = objLFMTDetailDTO.TextFont;

                cellFont = FontFactory.GetFont(strfName, BaseFont.CP1252, true, (float)objLFMTDetailDTO.FontSize, Font.NORMAL);


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

                            string BarcodeString = GetBarcodeStringNew(objLFMTDetailDTO, item, false);

                            if (string.IsNullOrEmpty(BarcodeString) || string.IsNullOrWhiteSpace(BarcodeString))
                                BarcodeString = optionBarcodeKey;

                            // optionBarcodeKey = GetBarcodeStringNew(objLFMTDetailDTO, objMMDTO, item, true);

                            // byte[] b = GetBarcode(objLabelSize, BarcodeString, intWidth, (int)(fltheight), false, Convert.ToString(child.GetAttribute("BarcodeFont")), optionBarcodeKey);
                            byte[] b = GetBarcodeNew(objLFMTDetailDTO, item, objLabelSize, BarcodeString, (int)flwidth, (int)(fltheight), false, Convert.ToString(child.GetAttribute("BarcodeFont")));
                            if (b != null)
                            {
                                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(b);
                                img.SetDpi(300, 300);
                                cellContent = img;
                                ((iTextSharp.text.Image)cellContent).Alignment = Element.ALIGN_CENTER;
                                img.Alignment = Element.ALIGN_CENTER;
                            }
                        }
                        else if (Convert.ToString(child.InnerText).Trim().Equals("QRcode_Cell"))
                        {
                            //string optionBarcodeKey = "";
                            string uniqueIdentifier = GetPropertyValueFromObject("ItemUniqueNumber", item);


                            string BarcodeString = GetBarcodeStringNew(objLFMTDetailDTO, item, false);

                            if (string.IsNullOrEmpty(BarcodeString) || string.IsNullOrWhiteSpace(BarcodeString))
                                BarcodeString = uniqueIdentifier;

                            int scale = (int)(qrCodeSize / 34);
                            byte[] b = GetQRcode(objLFMTDetailDTO, item, objLabelSize, BarcodeString, scale);
                            if (b != null)
                            {
                                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(b);
                                img.SetDpi(300, 300);
                                img.ScaleToFitHeight = true;
                                img.ScaleToFit(fltheight - 1, fltheight - 1);
                                cellContent = img;
                            }
                        }
                        else if (Convert.ToString(child.InnerText).Trim().Equals("ItemImage_Cell"))
                        {
                            string imgPath = GetItemImagePath(item);

                            if (!string.IsNullOrEmpty(Convert.ToString(imgPath)))
                                cellContent = GetImageFromRelativePath(imgPath, imgWidth, imgHeight);
                        }
                        else if (Convert.ToString(child.InnerText).Trim().Equals("SupplierImage_Cell"))
                        {
                            string imgPath = GetSupplierLogoPath(item);

                            if (!string.IsNullOrEmpty(Convert.ToString(imgPath)))
                                cellContent = GetImageFromRelativePath(imgPath, imgWidth, imgHeight);
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

                        InnerTableCell = new PdfPCell();
                        InnerTableCell.AddElement(cellContent);
                        InnerTableCell.Colspan = ColSpan;
                        InnerTableCell.Rowspan = RowSpan;
                        InnerTableCell.VerticalAlignment = Element.ALIGN_MIDDLE;

                        InnerTableCell.UseBorderPadding = true;
                        InnerTableCell.UseAscender = true;
                        InnerTableCell.Padding = 0.25F;
                        InnerTableCell.PaddingBottom = 0.25F;
                        InnerTableCell.PaddingLeft = 0.25F;
                        InnerTableCell.PaddingTop = 0.25F;
                        InnerTableCell.PaddingRight = 0.25F;
                        InnerTableCell.Border = 0;
                        InnerTableCell.BorderWidth = 0;
                        InnerTableCell.BorderWidthBottom = 0;
                        InnerTableCell.BorderWidthLeft = 0;
                        InnerTableCell.BorderWidthRight = 0;
                        InnerTableCell.BorderWidthTop = 0;
                        InnerTableCell.BorderColor = BaseColor.BLUE;
                        InnerTableCell.FixedHeight = fltheight + 0.01F;

                        //InnerTableCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        InnerTableCell.HorizontalAlignment = alignment;
                        if (Convert.ToString(child.InnerText).Trim().Equals("Barcode_Row"))
                        {
                            InnerTableCell.HorizontalAlignment = Element.ALIGN_CENTER;
                            InnerTableCell.PaddingLeft = 5;
                        }

                        InnerTable.AddCell(InnerTableCell);

                    }
                }

                InnerTable.CompleteRow();
                return InnerTable;
            }
            finally
            {
                cellFont = null;
                InnerTable = null;
                InnerTableCell = null;
            }
        }


        public string GetSupplierLogoPath(CatalogReportFieldsDTO item)
        {

            string strPath = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(item.SupplierImageType))
                {
                    if (item.SupplierImageType.Trim().ToLower() == "imagepath" && !string.IsNullOrEmpty(item.SupplierImagePath) && !item.SupplierImagePath.Contains("http://") && !item.SupplierImagePath.Contains("https://"))
                    {
                        strPath = item.SupplierImagePath;
                        Int64 EnterpriseId = SessionHelper.EnterPriceID;
                        Int64 CompanyID = SessionHelper.CompanyID;
                        Int64 RoomID = SessionHelper.RoomID;
                        Int64 SupplierId = item.SupplierID ?? 0;
                        strPath = Server.MapPath("..\\Uploads\\SupplierLogos\\" + EnterpriseId + "\\" + CompanyID + "\\" + RoomID + "\\" + SupplierId + "\\" + strPath);
                    }
                    else if (item.SupplierImageType.Trim().ToLower() == "externalimage")
                    {
                        strPath = item.SupplierImageExternalURL;
                    }

                }

            }
            catch
            {
                return string.Empty;
            }
            finally
            {
            }

            return strPath;
        }

        public string GetItemImagePath(CatalogReportFieldsDTO item)
        {
            string strPath = string.Empty;
            try
            {

                if (!string.IsNullOrEmpty(item.ItemImageType))
                {
                    if (item.ItemImageType.Trim().ToLower() == "imagepath" && !string.IsNullOrEmpty(item.ItemImagePath) && !item.ItemImagePath.Contains("http://") && !item.ItemImagePath.Contains("https://"))
                    {
                        strPath = item.ItemImagePath;
                        Int64 EnterpriseId = SessionHelper.EnterPriceID;
                        Int64 CompanyID = SessionHelper.CompanyID;
                        Int64 RoomID = SessionHelper.RoomID;

                        strPath = Server.MapPath("..\\Uploads\\InventoryPhoto\\" + EnterpriseId + "\\" + CompanyID + "\\" + RoomID + "\\" + item.ItemID + "\\" + strPath);
                    }
                    else if (item.ItemImageType.Trim().ToLower() == "externalimage")
                    {
                        strPath = item.ItemImageExternalURL;
                    }

                }

            }
            catch
            {
                return string.Empty;
            }
            finally
            {
                item = null;
            }



            return strPath;
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
            iTextSharp.text.Image img = null;
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
                        catch
                        {
                        }
                    }
                    else if (System.IO.File.Exists(path))
                        imageBytes = System.IO.File.ReadAllBytes(path);
                }

                if (imageBytes != null)
                {
                    img = iTextSharp.text.Image.GetInstance(imageBytes);
                    if (img.Width > 800 || img.Height > 600)
                    {
                        int tempImageWidth = (int)imgWidth;
                        int tempImageHeight = (int)imgHeigth;

                        //if (tempImageWidth < 800 || tempImageHeight < 600)
                        //{
                        //    tempImageWidth = tempImageWidth * 3;
                        //    tempImageHeight = tempImageHeight * 3;
                        //}

                        img = GetResizedImageFromByteArray(imageBytes, tempImageWidth, tempImageHeight);
                    }
                    //img = iTextSharp.text.Image.GetInstance(imageBytes);
                    //img.ScaleAbsolute(imgWidth, imgHeigth - 2);
                    img.SetDpi(300, 300);
                    ////img.Width = imgWidth;
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
            finally
            {
                imageBytes = null;
                webClient = null;
                img = null;
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
        public iTextSharp.text.Image GetResizedImageFromByteArray(byte[] imgdata, int imgWidth, int imgHeigth)
        {
            System.Drawing.Image image = null;
            System.Drawing.Bitmap newImage = null;
            try
            {
                using (MemoryStream ms = new MemoryStream(imgdata))
                {
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
                    newImage = new System.Drawing.Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb);

                    // Draws the image in the specified size with quality mode set to HighQuality
                    using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(newImage))
                    {
                        graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
                        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        graphics.DrawImage(image, 0, 0, newWidth, newHeight);
                    }

                    // Get an ImageCodecInfo object that represents the JPEG codec.
                    System.Drawing.Imaging.ImageCodecInfo imageCodecInfo = this.GetEncoderInfo(System.Drawing.Imaging.ImageFormat.Png);

                    // Create an Encoder object for the Quality parameter.
                    Encoder encoder = Encoder.Quality;

                    // Create an EncoderParameters object. 
                    EncoderParameters encoderParameters = new EncoderParameters(1);

                    // Save the image as a JPEG file with quality level.
                    EncoderParameter encoderParameter = new EncoderParameter(encoder, 1);
                    encoderParameters.Param[0] = encoderParameter;
                    using (MemoryStream msNew = new MemoryStream())
                    {

                        // ms = new MemoryStream();
                        newImage.Save(msNew, System.Drawing.Imaging.ImageFormat.Png);
                        iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(msNew.ToArray());
                        img.ScaleToFitHeight = true;
                        img.SpacingAfter = 0;
                        img.SpacingBefore = 0;
                        img.ScaleToFit(imgWidth, imgHeigth);
                        return img;
                    }
                }
            }
            finally
            {
                image = null;
                newImage = null;

            }


            //newImage.Save(filePath, imageCodecInfo, encoderParameters);
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
            Encoder encoder = Encoder.Quality;

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


        private byte[] GetQRcode(CatalogReportDetailDTO objLFMTDetailDTO, CatalogReportFieldsDTO item, CatalogReportTemplateMasterDTO objSizeDTO, string strBarcode, int scale)
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

        private byte[] GetBarcodeNew(CatalogReportDetailDTO objLFMTDetailDTO, CatalogReportFieldsDTO item, CatalogReportTemplateMasterDTO objSizeDTO, string strBarcode, int width, int Height, bool IncludeLabel, string strBarcodeType)
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
                string OptionalKeyWhenImageLarge = GetBarcodeStringNew(objLFMTDetailDTO, item, true);

                if (!string.IsNullOrEmpty(OptionalKeyWhenImageLarge) && ex.Message.Contains("EGENERATE_IMAGE-2: Image size specified not large enough to draw image."))
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
                        OptionalKeyWhenImageLarge = GetBarcodeStringNew(objLFMTDetailDTO, item, true);

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
                        OptionalKeyWhenImageLarge = GetBarcodeStringNew(objLFMTDetailDTO, item, true);

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

        private string GetBarcodeStringNew(CatalogReportDetailDTO objLFMTDetailDTO, CatalogReportFieldsDTO item, bool IsOptionalBarcode)
        {
            List<string> lstBarcodes = null;
            string barcodeString = string.Empty;
            string barcodefield = "ItemUniqueNumber";
            List<LabelModuleFieldMasterDTO> barcodeKeyList = GetBarcodeKeyList();
            if (barcodeKeyList != null && barcodeKeyList.Count > 0)
            {
                LabelModuleFieldMasterDTO objDTO = barcodeKeyList.Where(x => x.ID == Convert.ToInt64(objLFMTDetailDTO.BarcodeKey)).FirstOrDefault();
                if (objDTO != null)
                {
                    string _fieldName = objDTO.FieldName;
                    if (objDTO.FieldName == "SupplierNumber")
                    {
                        _fieldName = "SupplierPartNo";

                    }
                    else if (objDTO.FieldName == "UPCNumber")
                    {
                        _fieldName = "UPC";
                    }

                    var barcodef = objLFMTDetailDTO.lstModuleFields.FirstOrDefault(x => x.key == _fieldName);
                    if (barcodef != null)
                    {
                        barcodefield = barcodef.key;
                    }
                }
            }

            if (IsOptionalBarcode)
                barcodefield = "ItemUniqueNumber";

            if (objLFMTDetailDTO.BarcodePattern == "UK")
                lstBarcodes = GetListOfBarcodeStringForUKOnly(objLFMTDetailDTO, item, barcodefield);
            else if (objLFMTDetailDTO.BarcodePattern == "US")
                lstBarcodes = GetListOfBarcodeStringForUS(objLFMTDetailDTO, item, barcodefield);
            else if (objLFMTDetailDTO.BarcodePattern == "US01")
                lstBarcodes = GetListOfBarcodeStringForUS01(objLFMTDetailDTO, item, barcodefield);

            barcodeString = ConcateBarcodeString(lstBarcodes);

            return barcodeString;
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

        /// <summary>
        /// Create List for UK Barcode String 
        /// </summary>
        /// <param name="objLFMTDetailDTO"></param>
        /// <param name="objMMDTO"></param>
        /// <param name="item"></param>
        /// <param name="PrimaryBarcodeField"></param>
        /// <returns></returns>
        private List<string> GetListOfBarcodeStringForUS(CatalogReportDetailDTO objLFMTDetailDTO, CatalogReportFieldsDTO item, string PrimaryBarcodeField)
        {
            List<string> lstReturns = new List<string>();
            string BarcodeString = string.Empty;
            bool isIncludeBinQty = true;

            string strPrimaryBarcodeString = GetPropertyValueFromObject(PrimaryBarcodeField, item);
            string strBin = GetPropertyValueFromObject("BinNumber", item);
            if (objLFMTDetailDTO.BarcodeFont == "39" && !string.IsNullOrEmpty(strPrimaryBarcodeString))
            {
                lstReturns.Add(strPrimaryBarcodeString.ToUpper());
                return lstReturns;
            }

            // BarcodeString = "#I";

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
        private List<string> GetListOfBarcodeStringForUS01(CatalogReportDetailDTO objLFMTDetailDTO, CatalogReportFieldsDTO item, string PrimaryBarcodeField)
        {
            List<string> lstReturns = new List<string>();
            string BarcodeString = string.Empty;
            bool isIncludeBinQty = true;

            string strPrimaryBarcodeString = GetPropertyValueFromObject(PrimaryBarcodeField, item);
            string strBin = GetPropertyValueFromObject("BinNumber", item);
            if (objLFMTDetailDTO.BarcodeFont == "39" && !string.IsNullOrEmpty(strPrimaryBarcodeString))
            {
                lstReturns.Add(strPrimaryBarcodeString.ToUpper());
                return lstReturns;
            }


            //   BarcodeString = "#";

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
        private List<string> GetListOfBarcodeStringForUKOnly(CatalogReportDetailDTO objLFMTDetailDTO, CatalogReportFieldsDTO item, string PrimaryBarcodeField)
        {
            List<string> lstReturns = new List<string>();
            string BarcodeString = string.Empty;
            bool isIncludeBinQty = true;

            string strPrimaryBarcodeString = GetPropertyValueFromObject(PrimaryBarcodeField, item);
            string strBin = GetPropertyValueFromObject("BinNumber", item);


            string strItemType = GetPropertyValueFromObject("ItemType", item);
            if (!string.IsNullOrEmpty(strItemType) && strItemType == "4")
                isIncludeBinQty = false;

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

            if (objLFMTDetailDTO.BarcodeFont != "39")
            {
                lstReturns.Insert(0, "*");
                lstReturns.Add("*");
            }


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
        /// GetStringForCell
        /// </summary>
        /// <param name="objLFMTDetailDTO"></param>
        /// <param name="child"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private string GetStringForCell(CatalogReportDetailDTO objLFMTDetailDTO, XmlElement child, CatalogReportFieldsDTO item)
        {


            PropertyInfo info = null;
            object value = null;


            info = item.GetType().GetProperty(child.GetAttribute("FieldID").Trim());
            if (info != null)
                value = info.GetValue(item, null);

            if (value == null)
                value = "";
            string strLable = child.InnerText.Trim();
            if (!objLFMTDetailDTO.SelectedFields.Contains(child.GetAttribute("FieldID").Trim()))
                strLable = "";
            else if (!string.IsNullOrEmpty(strLable) && strLable.Trim().Length > 0)
            {
                if (!strLable.Contains("UDF"))
                {
                    strLable = eTurns.DTO.Resources.ResourceHelper.GetResourceValue(strLable, "Res_RPT_CatalogReportFields") + ": ";
                }
                else
                {

                    strLable = eTurns.DTO.Resources.ResourceHelper.GetResourceValue(strLable, "Res_RPT_CatalogReportFields", true) + ": ";

                }
            }

            return strLable + Convert.ToString(value.ToString());


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
            return ValueInInch * 72;
        }

        /// <summary>
        /// Convert Pixel To Point
        /// </summary>
        /// <param name="ValueInPixel"></param>
        /// <returns></returns>
        private float ConvertPointToPixel(float ValueInPoint)
        {
            return (ValueInPoint * 96) / 72;
        }
        /// <summary>
        /// Convert Inch To Point double
        /// </summary>
        /// <param name="ValueInInch"></param>
        /// <returns></returns>
        private float ConvertInchToPoint(double ValueInInch)
        {
            return (float)(ValueInInch * 72);
        }

        /// <summary>
        /// GetTabelCell
        /// </summary>
        /// <param name="objLabelSize"></param>
        /// <returns></returns>
        private PdfPCell GetHorizontalSpaceTabelCell(CatalogReportTemplateMasterDTO objLabelSize)
        {
            PdfPCell tabelCell = new PdfPCell();

            tabelCell.Border = 0;
            tabelCell.BorderWidthTop = 0;
            tabelCell.BorderWidthLeft = 0;
            tabelCell.BorderWidthRight = 0;
            tabelCell.BorderWidthBottom = 0;
            tabelCell.BorderColor = BaseColor.BLACK;

            tabelCell.PaddingLeft = ConvertInchToPoint(0);
            tabelCell.PaddingRight = ConvertInchToPoint(0);
            tabelCell.PaddingTop = ConvertInchToPoint(0);
            tabelCell.PaddingBottom = ConvertInchToPoint(0);

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
        private PdfPCell GetTabelCell(CatalogReportTemplateMasterDTO objLabelSize)
        {
            PdfPCell tabelCell = new PdfPCell();

            tabelCell.Border = 0;
            tabelCell.BorderWidthTop = TableCellBorderWidth;
            tabelCell.BorderWidthLeft = TableCellBorderWidth;
            tabelCell.BorderWidthRight = TableCellBorderWidth;
            tabelCell.BorderWidthBottom = TableCellBorderWidth;
            tabelCell.BorderColor = BaseColor.BLACK;

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
        private PdfPCell GetTabelCellForVerticleSpace(CatalogReportTemplateMasterDTO objLabelSize)
        {
            PdfPCell tabelCell = new PdfPCell();

            tabelCell.Border = 0;
            tabelCell.BorderWidthTop = 0;
            tabelCell.BorderWidthLeft = 0;
            tabelCell.BorderWidthRight = 0;
            tabelCell.BorderWidthBottom = 0;
            tabelCell.BorderColor = BaseColor.GREEN;

            tabelCell.FixedHeight = ConvertInchToPoint(objLabelSize.LabelSpacingVerticle);

            tabelCell.VerticalAlignment = Element.ALIGN_TOP;
            tabelCell.HorizontalAlignment = Element.ALIGN_LEFT;
            CurrentIndexVS += 1;
            return tabelCell;

        }

        #endregion

        public JsonResult SetGenerateCatalogReportData(string CatalogID, string ItemIDs, string CallFrom, string GroupName, string Sort, string SortAscDesc, string IncludeInactiveItems)
        {
            Session["CatalogID"] = CatalogID;
            string strBinIDs = string.Empty;
            if (CallFrom.ToLower() == "itembinmasterlist" && CatalogID == "0" && ItemIDs.Contains("#"))
            {
                string[] arrItemBinID = ItemIDs.Split(new char[1] { '#' }, StringSplitOptions.RemoveEmptyEntries);
                if (arrItemBinID != null && arrItemBinID.Length > 0)
                {
                    ItemIDs = arrItemBinID[0];
                    strBinIDs = arrItemBinID[1];
                    Session["ItemIDs"] = ItemIDs;
                    Session["ItemBinIDs"] = strBinIDs;
                }
                Session["CallFrom"] = "ItemList";
            }
            else
            {
                Session["ItemIDs"] = ItemIDs;
                Session["ItemBinIDs"] = strBinIDs;
                Session["CallFrom"] = CallFrom;
            }

            Session["GroupName"] = GroupName;
            Session["Sort"] = Sort;
            Session["SortAscDesc"] = SortAscDesc;
            Session["IncludeInactiveItems"] = IncludeInactiveItems;

            return Json(new { Status = true, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GenerateCatalogReportNew()
        {
            if (Session["CatalogID"] == null && Session["ItemIDs"] == null && Session["CallFrom"] == null && Session["GroupName"] == null &&
                Session["Sort"] == null && Session["SortAscDesc"] == null && Session["IncludeInactiveItems"] == null)
            {
                return RedirectToAction("MyProfile", "Master");
            }

            CatalogReportDAL objDal = new CatalogReportDAL(SessionHelper.EnterPriseDBName);
            CatalogReportDetailDTO objDetail = null;
            Int64 ID = 0;
            //Int64.TryParse(frm["hdnCatalogReportID"], out ID);
            if (!string.IsNullOrWhiteSpace(Convert.ToString(Session["CatalogID"])))
                Int64.TryParse(Convert.ToString(Session["CatalogID"]), out ID);
            bool isInActiveItems = false;
            CatalogReportDTO objCRDTO = new CatalogReportDTO();
            string ItemIDs = "";
            string BinIDs = "";
            string callFrom = "CR";

            //callFrom = frm["hdncallFrom"] ?? "CR";
            if (!string.IsNullOrWhiteSpace(Convert.ToString(Session["CallFrom"])))
                callFrom = Convert.ToString(Session["CallFrom"]);

            if (!string.IsNullOrWhiteSpace(Convert.ToString(Session["ItemBinIDs"])))
                BinIDs = Convert.ToString(Session["ItemBinIDs"]);

            if (ID == 0 && callFrom == "ItemList")
            {
                objDetail = objDal.GetDefaultCatalogReport(SessionHelper.CompanyID);
                //ItemIDs = frm["hdnSelectedItemIDs"];
                ItemIDs = Convert.ToString(Session["ItemIDs"]);
                isInActiveItems = false;
            }
            else
            {
                //isInActiveItems = Convert.ToBoolean(frm["chkIncludeInactiveItems"].Split(',')[0]);
                isInActiveItems = Convert.ToBoolean(Session["IncludeInactiveItems"]);
                ItemIDs = "";
                //objDetail = objDal.GetRecord(ID, SessionHelper.CompanyID);
                objDetail = objDal.GetCatalogReportDetailByID(ID, SessionHelper.CompanyID);
            }





            //string groupName = frm["ddlGroupName"];
            string groupName = Convert.ToString(Session["GroupName"]);
            if (!string.IsNullOrEmpty(groupName))
            {
                if (groupName == "Manufacturer")
                    groupName = "ManufacturerName";
                else if (groupName == "Manufacturer#")
                    groupName = "ManufacturerNumber";
                else if (groupName == "Supplier")
                    groupName = "SupplierName";
                else if (groupName == "Supplier#")
                    groupName = "SupplierPartNo";
                else if (groupName == "Category")
                    groupName = "CategoryName";
                else if (groupName == "Category")
                    groupName = "CategoryName";
                else if (groupName == "Location")
                    groupName = "DefaultLocationName";
                else if (groupName == "BinNumber")
                    groupName = "BinNumber";
            }

            //string sortField = frm["ddlSort"];
            string sortField = Convert.ToString(Session["Sort"]);
            //string sortorderby = frm["ddlSortAscDesc"];
            string sortorderby = Convert.ToString(Session["SortAscDesc"]);

            if (string.IsNullOrEmpty(sortField))
                sortField = "ItemNumber";
            else
            {
                if (sortField == "DefaultLocationName" && groupName == "BinNumber")
                {
                    sortField = "BinNumber";
                }

                sortField = sortField + " " + sortorderby;
            }


            Dictionary<string, List<CatalogReportFieldsDTO>> dicCatalogs = new Dictionary<string, List<CatalogReportFieldsDTO>>();

            if (!string.IsNullOrEmpty(groupName))
            {
                IEnumerable<string> lstGrpList = objDal.GetListByGrouping(groupName, "", SessionHelper.RoomID, ItemIDs);
                foreach (var item in lstGrpList)
                {
                    List<CatalogReportFieldsDTO> lstItems = objDal.GetItemsByGroupNameAndId(item, groupName, "", SessionHelper.RoomID, sortField, isInActiveItems, ItemIDs: ItemIDs, ItemBinIDs: BinIDs).ToList();
                    dicCatalogs.Add(item, lstItems);
                }
            }
            else
            {
                List<CatalogReportFieldsDTO> lstItems = objDal.GetItemsByGroupNameAndId(SessionHelper.RoomID, sortField, isInActiveItems, ItemIDs: ItemIDs, ItemBinIDs: BinIDs).ToList();
                //dicCatalogs.Add(" ", lstItems);
                if (lstItems != null && lstItems.Count > 0)
                    dicCatalogs.Add(" ", lstItems);
            }

            //return GenerateLabelsPDF(frm["ddlGroupName"], dicCatalogs, objDetail);
            return GenerateLabelsPDF(Convert.ToString(Session["GroupName"]), dicCatalogs, objDetail);
        }


    }

    public class PageEventHelper : PdfPageEventHelper
    {
        int NoIndexPages = 0;
        int TotalPages = 0;
        public PageEventHelper(int indPages, int totalPages) : base()
        {
            NoIndexPages = indPages;
            TotalPages = totalPages;
        }

        protected Dictionary<String, int> toc = new Dictionary<string, int>();
        PdfContentByte cb;
        PdfTemplate template;
        BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            cb = writer.DirectContent;
            template = cb.CreateTemplate(50, 50);
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);

            int pageN = writer.PageNumber + NoIndexPages;
            String text = string.Format(ResCommon.PageOf, pageN.ToString()) + " ";
            iTextSharp.text.Rectangle pageSize = document.PageSize;
            cb.SetRGBColorFill(100, 100, 100);
            cb.BeginText();
            cb.SetFontAndSize(bf, 8F);
            cb.SetTextMatrix(document.LeftMargin, pageSize.GetBottom(document.BottomMargin) - 10);
            cb.ShowText(text);
            float len = cb.GetEffectiveStringWidth(text, true);
            cb.EndText();
            cb.AddTemplate(template, document.LeftMargin + len, pageSize.GetBottom(document.BottomMargin) - 10);
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);
            template.BeginText();
            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
            template.SetFontAndSize(bf, 8F);
            template.SetTextMatrix(0, 0);
            template.ShowText("" + ((writer.PageNumber + NoIndexPages + TotalPages) - 1));
            template.EndText();
        }

        public void onGenericTag(PdfWriter writer, Document document, Rectangle rect, String text)
        {
            if (!toc.ContainsKey(text))
            {
                toc.Add(text, (writer.PageNumber + NoIndexPages));
                //writer.AddNamedDestination(name, writer.PageNumber, new PdfDestination(PdfDestination.FITH, rect.GetTop(1)));

            }
        }
        public Dictionary<string, int> getTOC()
        {
            return toc;
        }
    }



}
