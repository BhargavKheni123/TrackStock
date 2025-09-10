using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsWeb.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;

namespace eTurnsWeb.Controllers
{
    [AuthorizeHelper]
    [NgRedirectActionFilter]
    public class NotificationController : Controller
    {
        XNamespace ns = XNamespace.Get("http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition");
        //XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));

        public ActionResult Notifications()
        {
            var isScheduleReport = SessionHelper.GetAdminPermission(SessionHelper.ModuleList.ScheduleReport);

            if (isScheduleReport)
            {
                if (IsAllowNewNotification())
                {
                    return View("NotificationsNew");
                }

                return View();
            }
            else 
            {
                return RedirectToAction("MyProfile", "Master");
            }           

        }

        public ActionResult NotificationConfigurationCreate()
        {
            ViewBag.forcopy = false;
            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
            NotificationDAL objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
            SFTPDAL objSFTPDAL = new SFTPDAL(SessionHelper.EnterPriseDBName);
            List<ReportBuilderDTO> lstReportBuilderDTO = objReportMasterDAL.GetReportList(SessionHelper.UserID).Where(x => ((x.CompanyID == SessionHelper.CompanyID || x.CompanyID == 0) && x.IsDeleted.GetValueOrDefault(false) == false && x.IsArchived.GetValueOrDefault(false) == false && x.ISEnterpriseReport.GetValueOrDefault(false) == false) || (x.ISEnterpriseReport.GetValueOrDefault(false) == true && x.IsDeleted.GetValueOrDefault(false) == false && x.IsArchived.GetValueOrDefault(false) == false)).OrderBy(x => x.ReportName).ToList();
            lstReportBuilderDTO = lstReportBuilderDTO.Where(x => x.ModuleName != "EnterpriseList").OrderBy(x => x.ReportName).ToList();
            List<SupplierMasterDTO> lstSuppliers = objSupplierMasterDAL.GetSupplierByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID, false).OrderBy(t => t.SupplierName).ToList();
            List<SelectListItem> lstLanguage = GetLanguage();
            List<FTPMasterDTO> lstFtps = objSFTPDAL.GetAllFtpForRoom(SessionHelper.RoomID, SessionHelper.CompanyID);

            if (SessionHelper.UserSupplierIds != null && SessionHelper.UserSupplierIds.Any())
                lstSuppliers = lstSuppliers.Where(x => SessionHelper.UserSupplierIds.Contains(x.ID)).ToList();

            ViewBag.DDLanguage = lstLanguage;
            ViewBag.lstReportList = lstReportBuilderDTO;
            ViewBag.Suppliers = lstSuppliers;
            ViewBag.FtpDetails = lstFtps;
            NotificationDTO objNotificationDTO = new NotificationDTO();
            objNotificationDTO.RoomID = SessionHelper.RoomID;
            objNotificationDTO.CompanyID = SessionHelper.CompanyID;
            objNotificationDTO.ScheduleFor = 5;
            objNotificationDTO.NotificationMode = 1;
            SchedulerDTO objSchedulerDTO = new SchedulerDTO();
            objSchedulerDTO.RoomId = SessionHelper.RoomID;
            objSchedulerDTO.CompanyId = SessionHelper.CompanyID;
            objSchedulerDTO.LoadSheduleFor = 5;
            objNotificationDTO.SchedulerParams = objSchedulerDTO;

            return PartialView("NotificationConfigurationAlert", objNotificationDTO);
        }

        public ActionResult NotificationConfigurationReportAlertCreate()
        {
            ViewBag.forcopy = false;
            NotificationDAL objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
            EmailTemplateDAL objEmailTemplateDAL = new EmailTemplateDAL(SessionHelper.EnterPriseDBName);
            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
            SFTPDAL objSFTPDAL = new SFTPDAL(SessionHelper.EnterPriseDBName);
            List<FTPMasterDTO> lstFtps = objSFTPDAL.GetAllFtpForRoom(SessionHelper.RoomID, SessionHelper.CompanyID);
            List<EmailTemplateDTO> lstEmailTemplates = objEmailTemplateDAL.GetAllEmailTemplateToNotify();
            List<ReportBuilderDTO> lstReportBuilderDTO = objReportMasterDAL.GetReportList(SessionHelper.UserID).Where(x => ((x.CompanyID == SessionHelper.CompanyID || x.CompanyID == 0) && x.IsDeleted.GetValueOrDefault(false) == false && x.IsArchived.GetValueOrDefault(false) == false && x.ISEnterpriseReport.GetValueOrDefault(false) == false) || (x.ISEnterpriseReport.GetValueOrDefault(false) == true && x.IsDeleted.GetValueOrDefault(false) == false && x.IsArchived.GetValueOrDefault(false) == false)).OrderBy(x => x.ReportName).ToList();
            lstReportBuilderDTO = lstReportBuilderDTO.Where(x => x.ModuleName != "EnterpriseList").OrderBy(x => x.ReportName).ToList();
            if (SessionHelper.RoleID >= 0)
            {
                lstReportBuilderDTO = lstReportBuilderDTO.Where(x => x.ReportName != "EnterpriseRoom" && x.ReportName != "EnterpriseUser").OrderBy(x => x.ReportName).ToList();
            }
            List<SupplierMasterDTO> lstSuppliers = objSupplierMasterDAL.GetSupplierByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID, false);
            List<SelectListItem> lstLanguage = GetLanguage();

            if (SessionHelper.UserSupplierIds != null && SessionHelper.UserSupplierIds.Any())
                lstSuppliers = lstSuppliers.Where(x => SessionHelper.UserSupplierIds.Contains(x.ID)).ToList();

            List<AlertReportDTO> lstAlertReports = new List<AlertReportDTO>();
            //string isSupplierRequired = string.Empty;

            foreach (var item in lstEmailTemplates)
            {
                if (!string.IsNullOrWhiteSpace(item.ResourceKeyName))
                {
                    string ResAlertName = "";
                    ResAlertName = ResourceHelper.GetAlertNameByResource(item.ResourceKeyName);
                    if (!string.IsNullOrWhiteSpace(ResAlertName))
                    {
                        item.TemplateName = ResAlertName;
                    }
                }

                //isSupplierRequired = item.IsSupplierRequired ? "_1":"_0";
                lstAlertReports.Add(new AlertReportDTO() { ID = item.ID + "_6", AlertReportName = item.TemplateName, ScheduleFor = 6 });
            }

            foreach (var item in lstReportBuilderDTO)
            {
                if (!string.IsNullOrWhiteSpace(item.ResourceKey))
                {
                    string ResMasterReportName = "";
                    ResMasterReportName = ResourceHelper.GetReportNameByResource(item.ResourceKey);
                    if (!string.IsNullOrWhiteSpace(ResMasterReportName))
                    {
                        item.ReportName = ResMasterReportName;
                    }
                }

                //isSupplierRequired = item.IsSupplierRequired ? "_1":"_0";
                lstAlertReports.Add(new AlertReportDTO() { ID = item.ID + "_5", AlertReportName = item.ReportName, ScheduleFor = 5 });
            }

            lstAlertReports = lstAlertReports.OrderBy(t => t.AlertReportName).ToList();
            ViewBag.AlertReport = lstAlertReports;
            ViewBag.DDLanguage = lstLanguage;
            ViewBag.lstReportList = lstReportBuilderDTO;
            ViewBag.Suppliers = lstSuppliers;
            ViewBag.EmailTemplates = lstEmailTemplates;
            ViewBag.FtpDetails = lstFtps;
            NotificationDTO objNotificationDTO = new NotificationDTO();
            DashboardDAL objDashboardDAL = new DashboardDAL(SessionHelper.EnterPriseDBName);
            DashboardParameterDTO objDashboardParameterDTO = new DashboardParameterDTO();
            objDashboardParameterDTO = objDashboardDAL.GetDashboardParameters(SessionHelper.RoomID, SessionHelper.CompanyID);

            if (objDashboardParameterDTO != null)
            {
                objNotificationDTO.MonthlyAverageUsage = objDashboardParameterDTO.MonthlyAverageUsage;
            }
            else
            {
                objNotificationDTO.MonthlyAverageUsage = 30;
            }

            SchedulerDTO objSchedulerDTO = new SchedulerDTO();
            objSchedulerDTO.RoomId = SessionHelper.RoomID;
            objSchedulerDTO.CompanyId = SessionHelper.CompanyID;
            objSchedulerDTO.LoadSheduleFor = 6;
            objNotificationDTO.SchedulerParams = objSchedulerDTO;
            objNotificationDTO.RoomID = SessionHelper.RoomID;
            objNotificationDTO.CompanyID = SessionHelper.CompanyID;
            objNotificationDTO.ScheduleFor = 6;
            objNotificationDTO.NotificationMode = 1;
            objNotificationDTO.IsSupplierRequired = true;

            if (IsAllowNewNotification())
            {
                return PartialView("NotificationConfigurationAlertNew", objNotificationDTO);
            }

            return PartialView("NotificationConfigurationAlert", objNotificationDTO);
        }

        public ActionResult NotificationConfigurationAlertCreate()
        {
            ViewBag.forcopy = false;
            NotificationDAL objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
            EmailTemplateDAL objEmailTemplateDAL = new EmailTemplateDAL(SessionHelper.EnterPriseDBName);
            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
            SFTPDAL objSFTPDAL = new SFTPDAL(SessionHelper.EnterPriseDBName);
            List<FTPMasterDTO> lstFtps = objSFTPDAL.GetAllFtpForRoom(SessionHelper.RoomID, SessionHelper.CompanyID);
            List<EmailTemplateDTO> lstEmailTemplates = objEmailTemplateDAL.GetAllEmailTemplateToNotify();
            List<ReportBuilderDTO> lstReportBuilderDTO = objReportMasterDAL.GetReportList(SessionHelper.UserID).Where(x => ((x.CompanyID == SessionHelper.CompanyID || x.CompanyID == 0) && x.IsDeleted.GetValueOrDefault(false) == false && x.IsArchived.GetValueOrDefault(false) == false && x.ISEnterpriseReport.GetValueOrDefault(false) == false) || (x.ISEnterpriseReport.GetValueOrDefault(false) == true && x.IsDeleted.GetValueOrDefault(false) == false && x.IsArchived.GetValueOrDefault(false) == false)).OrderBy(x => x.ReportName).ToList();
            lstReportBuilderDTO = lstReportBuilderDTO.Where(x => x.ModuleName != "EnterpriseList").OrderBy(x => x.ReportName).ToList();
            List<SupplierMasterDTO> lstSuppliers = objSupplierMasterDAL.GetSupplierByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID, false).OrderBy(t => t.SupplierName).ToList();
            List<SelectListItem> lstLanguage = GetLanguage();

            if (SessionHelper.UserSupplierIds != null && SessionHelper.UserSupplierIds.Any())
                lstSuppliers = lstSuppliers.Where(x => SessionHelper.UserSupplierIds.Contains(x.ID)).ToList();

            ViewBag.DDLanguage = lstLanguage;
            ViewBag.lstReportList = lstReportBuilderDTO;
            ViewBag.Suppliers = lstSuppliers;
            ViewBag.EmailTemplates = lstEmailTemplates;
            ViewBag.FtpDetails = lstFtps;
            NotificationDTO objNotificationDTO = new NotificationDTO();


            SchedulerDTO objSchedulerDTO = new SchedulerDTO();
            objSchedulerDTO.RoomId = SessionHelper.RoomID;
            objSchedulerDTO.CompanyId = SessionHelper.CompanyID;
            objSchedulerDTO.LoadSheduleFor = 6;
            objNotificationDTO.SchedulerParams = objSchedulerDTO;
            objNotificationDTO.RoomID = SessionHelper.RoomID;
            objNotificationDTO.CompanyID = SessionHelper.CompanyID;
            objNotificationDTO.ScheduleFor = 6;
            objNotificationDTO.NotificationMode = 1;
            return PartialView("NotificationConfigurationAlert", objNotificationDTO);
        }

        public ActionResult NotificationConfigurationAlertEdit(long ID, bool? forcopy)
        {
            ViewBag.forcopy = forcopy;
            EmailTemplateDAL objEmailTemplateDAL = new EmailTemplateDAL(SessionHelper.EnterPriseDBName);
            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
            NotificationDAL objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
            SFTPDAL objSFTPDAL = new SFTPDAL(SessionHelper.EnterPriseDBName);
            List<EmailTemplateDTO> lstEmailTemplates = objEmailTemplateDAL.GetAllEmailTemplateToNotify();
            List<ReportBuilderDTO> lstReportBuilderDTO = objReportMasterDAL.GetReportList(SessionHelper.UserID).Where(x => ((x.CompanyID == SessionHelper.CompanyID || x.CompanyID == 0) && x.IsDeleted.GetValueOrDefault(false) == false && x.IsArchived.GetValueOrDefault(false) == false && x.ISEnterpriseReport.GetValueOrDefault(false) == false) || (x.ISEnterpriseReport.GetValueOrDefault(false) == true && x.IsDeleted.GetValueOrDefault(false) == false && x.IsArchived.GetValueOrDefault(false) == false)).OrderBy(x => x.ReportName).ToList();
            lstReportBuilderDTO = lstReportBuilderDTO.Where(x => x.ModuleName != "EnterpriseList").OrderBy(x => x.ReportName).ToList();
            if (SessionHelper.RoleID >= 0 && lstReportBuilderDTO != null)
            {
                lstReportBuilderDTO = lstReportBuilderDTO.Where(x => x.ReportName != "EnterpriseRoom" && x.ReportName != "EnterpriseUser").OrderBy(x => x.ReportName).ToList();
            }
            List<SupplierMasterDTO> lstSuppliers = objSupplierMasterDAL.GetSupplierByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID, false).ToList();
            List<SelectListItem> lstLanguage = GetLanguage();
            List<FTPMasterDTO> lstFtps = objSFTPDAL.GetAllFtpForRoom(SessionHelper.RoomID, SessionHelper.CompanyID);

            if (SessionHelper.UserSupplierIds != null && SessionHelper.UserSupplierIds.Any())
                lstSuppliers = lstSuppliers.Where(x => SessionHelper.UserSupplierIds.Contains(x.ID)).ToList();

            ViewBag.DDLanguage = lstLanguage;
            ViewBag.lstReportList = lstReportBuilderDTO;
            ViewBag.Suppliers = lstSuppliers;
            ViewBag.EmailTemplates = lstEmailTemplates;
            ViewBag.FtpDetails = lstFtps;

            NotificationDTO objNotificationDTO = objNotificationDAL.GetNotifiactionByID(ID);
            List<AlertReportDTO> lstAlertReports = new List<AlertReportDTO>();
            //string isSupplierRequired = string.Empty;

            foreach (var item in lstEmailTemplates)
            {
                if (!string.IsNullOrWhiteSpace(item.ResourceKeyName))
                {
                    string ResAlertName = "";
                    ResAlertName = ResourceHelper.GetAlertNameByResource(item.ResourceKeyName);
                    if (!string.IsNullOrWhiteSpace(ResAlertName))
                    {
                        item.TemplateName = ResAlertName;
                    }                    
                }

                //isSupplierRequired = item.IsSupplierRequired ? "_1":"_0";
                lstAlertReports.Add(new AlertReportDTO() { ID = item.ID + "_6", AlertReportName = item.TemplateName, ScheduleFor = 6 });
            }

            foreach (var item in lstReportBuilderDTO)
            {
                if (!string.IsNullOrWhiteSpace(item.ResourceKey))
                {
                    string ResMasterReportName = "";
                    ResMasterReportName = ResourceHelper.GetReportNameByResource(item.ResourceKey);
                    if (!string.IsNullOrWhiteSpace(ResMasterReportName))
                    {
                        item.ReportName = ResMasterReportName;
                    }
                }

                //isSupplierRequired = item.IsSupplierRequired ? "_1":"_0";
                lstAlertReports.Add(new AlertReportDTO() { ID = item.ID + "_5", AlertReportName = item.ReportName, ScheduleFor = 5 });
            }

            lstAlertReports = lstAlertReports.OrderBy(t => t.AlertReportName).ToList();
            ViewBag.AlertReport = lstAlertReports;
            
            if (objNotificationDTO == null)
            {
                objNotificationDTO = new NotificationDTO();
                SchedulerDTO objSchedulerDTO = new SchedulerDTO();
                objSchedulerDTO.RoomId = SessionHelper.RoomID;
                objSchedulerDTO.CompanyId = SessionHelper.CompanyID;
                objSchedulerDTO.LoadSheduleFor = 6;
                objNotificationDTO.SchedulerParams = objSchedulerDTO;
                objNotificationDTO.RoomID = SessionHelper.RoomID;
                objNotificationDTO.CompanyID = SessionHelper.CompanyID;
                objNotificationDTO.ScheduleFor = 6;
                objNotificationDTO.IsSupplierRequired = true;
            }
            else
            { 
                //if (objNotificationDTO.ScheduleFor == 5 && objNotificationDTO.ReportMasterDTO != null && objNotificationDTO.ReportMasterDTO.ID > 0)
                //{ 
                //     objNotificationDTO.IsSupplierRequired = objNotificationDTO.ReportMasterDTO.IsSupplierRequired;   
                //}
                //else if (objNotificationDTO.ScheduleFor == 6 && objNotificationDTO.EmailTemplateDetail != null && objNotificationDTO.EmailTemplateDetail.ID > 0)
                //{
                //    objNotificationDTO.IsSupplierRequired = objNotificationDTO.EmailTemplateDetail.IsSupplierRequired;   
                //}
            }

            //isSupplierRequired = objNotificationDTO.IsSupplierRequired ? "_1":"_0";

            if (objNotificationDTO.ScheduleFor == 5)
            {
                objNotificationDTO.NotificationName = objNotificationDTO.ReportID + "_5";
            }
            else if (objNotificationDTO.ScheduleFor == 6)
            {
                objNotificationDTO.NotificationName = objNotificationDTO.EmailTemplateID + "_6";
            }

            if(objNotificationDTO != null && !string.IsNullOrWhiteSpace(objNotificationDTO.ParentReportName))
            {
                ViewBag.HelpDocReportName = objNotificationDTO.ParentReportName;
            }

            NotificationHelper objNotificationHelper = new NotificationHelper();
            objNotificationHelper.SetReportParamsNotification(ref objNotificationDTO);


            if (IsAllowNewNotification())
            {
                return PartialView("NotificationConfigurationAlertNew", objNotificationDTO);
            }

            return PartialView("NotificationConfigurationAlert", objNotificationDTO);
        }

        [HttpPost]
        public JsonResult GetEmailTemplateDetails(long TemplateId, long NotificationID, int ScheduleFor, long RptID)
        {
            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            long ReportID = 0;
            long ReportMasterID = 0;
            bool isInStockReport = false;
            EmailTemplateDAL objEmailTemplateDAL = new EmailTemplateDAL(SessionHelper.EnterPriseDBName);
            string ItemType = "EmailTemplate";

            NotificationDTO oNotificationDTO = new NotificationDTO();
            string parentReportName = string.Empty;

            if (ScheduleFor == 5)
            {
                ItemType = "Report";
                ReportID = TemplateId;
                TemplateId = 3;


                if (NotificationID > 0)
                {
                    oNotificationDTO = new NotificationDAL(SessionHelper.EnterPriseDBName).GetNotifiactionByID(NotificationID);
                    if (oNotificationDTO != null && oNotificationDTO.ReportID.HasValue)
                        ReportMasterID = oNotificationDTO.ReportID.Value;
                }
                else
                {
                    ReportMasterID = ReportID;
                }


                ReportBuilderDTO oReportBuilderDTO = objReportMasterDAL.GetParentReportMasterByReportID(ReportMasterID);
                if (oReportBuilderDTO != null)
                {
                    var reportName = oReportBuilderDTO.ReportName;

                    isInStockReport = (!string.IsNullOrEmpty(reportName) && !string.IsNullOrWhiteSpace(reportName) && (reportName.ToLower() == "instock" || reportName.ToLower() == "instock margin"
                        || reportName.ToLower() == "instock with qoh"))
                    ? true : false;
                    //if (oReportBuilderDTO.ReportFileName != null && (oReportBuilderDTO.ReportFileName.ToLower() == "rpt_instock.rdlc" || oReportBuilderDTO.ReportFileName.ToLower() == "rpt_suggestedorders.rdlc" || oReportBuilderDTO.ReportFileName.ToLower() == "rpt_instockbybinwithqty.rdlc" || oReportBuilderDTO.ReportFileName.ToLower() == "rpt_instock_margin.rdlc"))
                    //{
                    //    isInStockReport = true;
                    //}

                    parentReportName = oReportBuilderDTO.ReportName;

                }
            }
            List<EmailTemplateDetailDTO> lstTemplatedtls = new List<EmailTemplateDetailDTO>();
            if (TemplateId > 0)
            {
                lstTemplatedtls = objEmailTemplateDAL.GetEmailTemplateDetails(TemplateId, NotificationID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, ReportID, ScheduleFor);
            }

            ReportMasterDTO objReportMasterDTO = objReportMasterDAL.GetReportSigleRecord(RptID, TemplateId, ItemType);
            if (objReportMasterDTO != null && !string.IsNullOrWhiteSpace(objReportMasterDTO.AllowedAttahmentReports))
            {
                objReportMasterDTO.AllowedAttahmentReportsWithChild = objReportMasterDTO.AllowedAttahmentReports + "," + objReportMasterDAL.GetChildReportIDs(objReportMasterDTO.AllowedAttahmentReports, SessionHelper.CompanyID);
            }



            return Json(new { EmailTemplateDetails = lstTemplatedtls, IsInStockReport = isInStockReport, ParentReportName = parentReportName, AlertReportConfig = objReportMasterDTO });
        }
        [HttpPost]
        public JsonResult GetTokenReportWise(long TemplateId, long ReportID, int ScheduleFor, bool isBothApplay)
        {
            List<KeyValDTO> lstTokenList = new List<KeyValDTO>();
            try
            {
                NotificationDAL objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
                if (isBothApplay)
                {
                    if (ScheduleFor == 6)
                    {
                        lstTokenList = objNotificationDAL.GetAlertToken(TemplateId, ReportID);
                    }
                }
                else
                {
                    if (ReportID > 0)
                    {
                        NotificationDTO oNotificationDTO = new NotificationDAL(SessionHelper.EnterPriseDBName).GetNotifiactionByID(ReportID);
                        TemplateId = oNotificationDTO.ReportID ?? 0;
                        if (ScheduleFor == 6 && oNotificationDTO != null)
                        {
                            TemplateId = oNotificationDTO.EmailTemplateID ?? 0;
                        }
                    }
                    if (ScheduleFor == 5)
                    {
                        lstTokenList = objNotificationDAL.GetReportToken(TemplateId);
                        // TemplateId = 3;
                    }
                    if (ScheduleFor == 6)
                    {
                        lstTokenList = objNotificationDAL.GetAlertToken(TemplateId);
                        // TemplateId = 3;
                    }
                }


                lstTokenList.Insert(0, new KeyValDTO() { key = string.Empty, value = string.Empty });
            }
            catch (Exception)
            {
                return Json(new { lstSortList = lstTokenList });
            }
            return Json(new { lstSortList = lstTokenList });
        }
        [HttpPost]
        public JsonResult GetSortFieldsReportWise(long TemplateId, long NotificationID, int ScheduleFor)
        {
            NotificationDTO objNotificationDTO = new NotificationDTO();
            try
            {

                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                long ReportID = 0;
                // if (ScheduleFor == 5)
                {
                    ReportID = TemplateId;
                    TemplateId = 3;
                }
                if (NotificationID > 0)
                {
                    NotificationDTO oNotificationDTO = new NotificationDAL(SessionHelper.EnterPriseDBName).GetNotifiactionByID(NotificationID);
                    objNotificationDTO = oNotificationDTO;
                    if (oNotificationDTO != null && oNotificationDTO.ReportID.HasValue)
                        ReportID = oNotificationDTO.ReportID.Value;
                    else
                    {
                        ReportBuilderDTO oReportBuilderDTO = objReportMasterDAL.GetReportList().Where(x => x.ID == ReportID).FirstOrDefault();
                        objNotificationDTO.ReportMasterDTO = oReportBuilderDTO;
                    }
                }
                else
                {
                    ReportBuilderDTO oReportBuilderDTO = objReportMasterDAL.GetReportList().Where(x => x.ID == ReportID).FirstOrDefault();
                    objNotificationDTO.ReportMasterDTO = oReportBuilderDTO;
                    if (objNotificationDTO.SortSequence == null && oReportBuilderDTO.SortColumns != null)
                    {
                        objNotificationDTO.SortSequence = oReportBuilderDTO.SortColumns;
                    }
                }

                ReportBuilderDTO objReportBuilderDTO = objReportMasterDAL.GetReportList().Where(x => x.ID == ReportID).FirstOrDefault();
                
                if (objReportBuilderDTO == null)
                {
                    return Json(new { lstSortList = new List<KeyValDTO>(), lstSortOrder = new List<KeyValDTO>(), objNotificationDTO = objNotificationDTO });
                }
                string ReportFile = objReportBuilderDTO.ReportFileName;
                string SubReportFile = objReportBuilderDTO.SubReportFileName;
                string Resfile = objReportBuilderDTO.MasterReportResFile;
                ReportPerameters objRptParameters = new ReportPerameters();
                objRptParameters.ModuleName = objReportBuilderDTO.ModuleName;
                string rdlPath = string.Empty;
                string rdlSubPath = string.Empty;
                string RDLCBaseFilePath = CommonUtility.RDLCBaseFilePath;
                if (objReportBuilderDTO.ParentID > 0)
                {
                    if (objReportBuilderDTO.ISEnterpriseReport.GetValueOrDefault(false))
                    {
                        rdlPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/EnterpriseReport" + @"\\" + ReportFile;
                        rdlSubPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/EnterpriseReport" + @"\\" + SubReportFile;
                    }
                    else
                    {
                        rdlPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID + @"\\" + ReportFile;
                        rdlSubPath = RDLCBaseFilePath + "/"+ SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID + @"\\" + SubReportFile;
                    }
                }
                else
                {
                    rdlPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/BaseReport" + @"\\" + ReportFile;
                    rdlSubPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/BaseReport" + @"\\" + SubReportFile;
                }

                objRptParameters.Id = ReportID;
                objRptParameters.ReportFileName = ReportFile;



                XDocument doc = XDocument.Load(rdlPath);
                List<XElement> lstSortingFields = new List<XElement>();
                List<XElement> lstSortingRows = new List<XElement>();

                lstSortingRows = doc.Descendants(ns + "Tablix").Descendants(ns + "TablixRow").ToList();

                lstSortingFields = lstSortingRows[1].Descendants(ns + "TablixCell").ToList();
                if (!string.IsNullOrEmpty(SubReportFile))
                {
                    XDocument docsub = XDocument.Load(rdlSubPath);
                    lstSortingRows = docsub.Descendants(ns + "Tablix").Descendants(ns + "TablixRow").ToList();
                    lstSortingFields = lstSortingRows[1].Descendants(ns + "TablixCell").ToList();
                }
                List<KeyValDTO> lstSortList = new List<KeyValDTO>();
                foreach (XElement cl in lstSortingFields)
                {
                    string tdval = string.Empty;
                    tdval = Convert.ToString(cl.Descendants(ns + "Value").FirstOrDefault().Value);

                    if (tdval.Contains("=Fields!") && tdval.Contains(".Value") && tdval.Replace("=Fields!", "").Replace(".Value", "") != "Total")
                    {
                        KeyValDTO obj = new KeyValDTO();
                        obj.key = tdval.Replace("=Fields!", "").Replace(".Value", "");
                        obj.value = GetField(tdval.Replace("=Fields!", "").Replace(".Value", ""), Resfile);
                        lstSortList.Add(obj);
                    }
                }

                if (lstSortList.Count <= 0 && string.IsNullOrEmpty(SubReportFile))
                {
                    lstSortingFields = lstSortingRows[2].Descendants(ns + "TablixCell").ToList();
                    foreach (XElement cl in lstSortingFields)
                    {
                        string tdval = string.Empty;
                        tdval = Convert.ToString(cl.Descendants(ns + "Value").FirstOrDefault().Value);

                        if (tdval.Contains("=Fields!") && tdval.Contains(".Value") && tdval.Replace("=Fields!", "").Replace(".Value", "") != "Total")
                        {
                            KeyValDTO obj = new KeyValDTO();
                            obj.key = tdval.Replace("=Fields!", "").Replace(".Value", "");
                            obj.value = GetField(tdval.Replace("=Fields!", "").Replace(".Value", ""), Resfile);
                            lstSortList.Add(obj);
                        }
                    }
                }

                lstSortList.Insert(0, new KeyValDTO() { key = string.Empty, value = string.Empty });
                objNotificationDTO.SortFields = lstSortList;

                List<KeyValDTO> lstSortOrder = new List<KeyValDTO>();
                lstSortOrder.Insert(0, new KeyValDTO() { key = "asc", value = "asc" });
                lstSortOrder.Insert(1, new KeyValDTO() { key = "desc", value = "desc" });
                objNotificationDTO.SortOrders = lstSortOrder;


                if (!string.IsNullOrEmpty(objNotificationDTO.SortSequence))
                {
                    string[] CurrentSorting = objNotificationDTO.SortSequence.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < CurrentSorting.Length; i++)
                    {
                        string item = CurrentSorting[i];
                        if (i == 0)
                        {
                            objNotificationDTO.SortFieldFirstValue = item.Replace(" asc", "").Replace(" desc", "");
                            objNotificationDTO.SortFieldFirstOrder = "asc";
                            if (item.ToLower().Contains(" desc"))
                            {
                                objNotificationDTO.SortFieldFirstOrder = "desc";
                            }
                        }
                        else if (i == 1)
                        {
                            objNotificationDTO.SortFieldSecondValue = item.Replace(" asc", "").Replace(" desc", "");
                            objNotificationDTO.SortFieldSecondOrder = "asc";
                            if (item.ToLower().Contains(" desc"))
                            {
                                objNotificationDTO.SortFieldSecondOrder = "desc";
                            }
                        }
                        else if (i == 2)
                        {
                            objNotificationDTO.SortFieldThirdValue = item.Replace(" asc", "").Replace(" desc", "");
                            objNotificationDTO.SortFieldThirdOrder = "asc";
                            if (item.ToLower().Contains(" desc"))
                            {
                                objNotificationDTO.SortFieldThirdOrder = "desc";
                            }
                        }
                        else if (i == 3)
                        {
                            objNotificationDTO.SortFieldFourthValue = item.Replace(" asc", "").Replace(" desc", "");
                            objNotificationDTO.SortFieldFourthOrder = "asc";
                            if (item.ToLower().Contains(" desc"))
                            {
                                objNotificationDTO.SortFieldFourthOrder = "desc";
                            }
                        }
                        else if (i == 4)
                        {
                            objNotificationDTO.SortFieldFifthValue = item.Replace(" asc", "").Replace(" desc", "");
                            objNotificationDTO.SortFieldFifthOrder = "asc";
                            if (item.ToLower().Contains(" desc"))
                            {
                                objNotificationDTO.SortFieldFifthOrder = "desc";
                            }
                        }
                    }
                }

                return Json(new { lstSortList = lstSortList, lstSortOrder = lstSortOrder, objNotificationDTO = objNotificationDTO });
            }
            catch (Exception)
            {
                return Json(new { lstSortList = new List<KeyValDTO>(), lstSortOrder = new List<KeyValDTO>(), objNotificationDTO = objNotificationDTO });
            }
        }
        public string GetField(string Key, string FileName)
        {
            string KeyVal = string.Empty;

            if (Key.ToLower().Contains("udf"))
            {
                KeyVal = ResourceHelper.GetResourceValue(Key, FileName, true);
            }
            else
            {
                KeyVal = ResourceHelper.GetResourceValue(Key, FileName);

            }

            return KeyVal;
        }
        public ActionResult EmailConfigurationTemplate()
        {
            List<SelectListItem> lstLanguage = GetLanguage();
            ViewBag.DDLanguage = lstLanguage;
            //List<SelectListItem> lstEmailTemp = GetEmailTemplateList(lstLanguage[0].Value);
            EmailTemplateDAL objEmailTemplate = new EmailTemplateDAL(SessionHelper.EnterPriseDBName);
            List<EmailTemplateDTO> lstEmailTemp = objEmailTemplate.GetAllEmailTemplate();
            ViewBag.DDLEmailTemplate = lstEmailTemp;
            return View();
        }

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
                    if (ResourceHelper.CurrentCult.Name == item.Culture)
                    {
                        obj.Selected = true;
                    }
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

        public ActionResult NotificationListAjax(JQueryDataTableParamModel param)
        {
            Guid ICGUID = Guid.Empty;
            Guid.TryParse(Convert.ToString(Request["IcGuid"]), out ICGUID);

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

            // set the default column sorting here, if first time then required to set 
            if (sortColumnName == "0" || sortColumnName == "undefined" || string.IsNullOrEmpty(sortColumnName))
                sortColumnName = "ID desc";

            if (sortColumnName.Contains("EmailSubject"))
            {
                sortColumnName = sortColumnName.Replace("EmailSubject", "MailSubject");
            }

            int TotalRecordCount = 0;
            InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            //bool IsArchived = false;
            //bool IsDeleted = false;
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            NotificationDAL objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);

            //ResourceHelper.CurrentCult.Name 

            List<NotificationDTO> DataFromDB = objNotificationDAL.GetPagedNotifications(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, SessionHelper.UserID, ResourceHelper.CurrentCult.Name);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = DataFromDB
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult NotificationSave(NotificationDTO objNotificationDTO, SchedulerDTO objSchedulerDTO)
        {
            string message = "";
            string status = "";
            NotificationDAL objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
            ModelState.Clear();
            string SortSequence = string.Empty;
            string XMLValue = string.Empty;
            if (objNotificationDTO != null)
            {
                //if (objNotificationDTO.ScheduleFor == 5)
                //{
                if (!string.IsNullOrWhiteSpace(objNotificationDTO.CompanyIds))
                {
                    objNotificationDTO.CompanyIds = objNotificationDTO.CompanyIds.TrimEnd(',');
                }
                else
                {
                    objNotificationDTO.CompanyIds = Convert.ToString(SessionHelper.CompanyID);
                }
                if (!string.IsNullOrWhiteSpace(objNotificationDTO.RoomIds))
                {
                    objNotificationDTO.RoomIds = objNotificationDTO.RoomIds.TrimEnd(',');
                }
                else
                {
                    objNotificationDTO.RoomIds = Convert.ToString(SessionHelper.RoomID);
                }
                //}
                //else
                //{
                //    objNotificationDTO.CompanyIds = string.Empty;
                //    objNotificationDTO.RoomIds = string.Empty;
                //}
            }
            if (objNotificationDTO != null && objNotificationDTO.SortFieldFirstValue != null && objNotificationDTO.SortFieldFirstOrder != null && !string.IsNullOrWhiteSpace(objNotificationDTO.SortFieldFirstValue) && !string.IsNullOrWhiteSpace(objNotificationDTO.SortFieldFirstOrder))
            {
                if (SortSequence != null && !string.IsNullOrWhiteSpace(SortSequence))
                {
                    SortSequence = SortSequence + "," + objNotificationDTO.SortFieldFirstValue + " " + objNotificationDTO.SortFieldFirstOrder;
                }
                else
                {
                    SortSequence = objNotificationDTO.SortFieldFirstValue + " " + objNotificationDTO.SortFieldFirstOrder;
                }
            }
            if (objNotificationDTO != null && objNotificationDTO.SortFieldSecondValue != null && objNotificationDTO.SortFieldSecondOrder != null && !string.IsNullOrWhiteSpace(objNotificationDTO.SortFieldSecondValue) && !string.IsNullOrWhiteSpace(objNotificationDTO.SortFieldSecondOrder))
            {
                if (SortSequence != null && !string.IsNullOrWhiteSpace(SortSequence))
                {
                    SortSequence = SortSequence + "," + objNotificationDTO.SortFieldSecondValue + " " + objNotificationDTO.SortFieldSecondOrder;
                }
                else
                {
                    SortSequence = objNotificationDTO.SortFieldSecondValue + " " + objNotificationDTO.SortFieldSecondOrder;
                }
            }
            if (objNotificationDTO != null && objNotificationDTO.SortFieldThirdValue != null && objNotificationDTO.SortFieldThirdOrder != null && !string.IsNullOrWhiteSpace(objNotificationDTO.SortFieldThirdValue) && !string.IsNullOrWhiteSpace(objNotificationDTO.SortFieldThirdOrder))
            {
                if (SortSequence != null && !string.IsNullOrWhiteSpace(SortSequence))
                {
                    SortSequence = SortSequence + "," + objNotificationDTO.SortFieldThirdValue + " " + objNotificationDTO.SortFieldThirdOrder;
                }
                else
                {
                    SortSequence = objNotificationDTO.SortFieldThirdValue + " " + objNotificationDTO.SortFieldThirdOrder;
                }
            }
            if (objNotificationDTO != null && objNotificationDTO.SortFieldFourthValue != null && objNotificationDTO.SortFieldFourthOrder != null && !string.IsNullOrWhiteSpace(objNotificationDTO.SortFieldFourthValue) && !string.IsNullOrWhiteSpace(objNotificationDTO.SortFieldFourthOrder))
            {
                if (SortSequence != null && !string.IsNullOrWhiteSpace(SortSequence))
                {
                    SortSequence = SortSequence + "," + objNotificationDTO.SortFieldFourthValue + " " + objNotificationDTO.SortFieldFourthOrder;
                }
                else
                {
                    SortSequence = objNotificationDTO.SortFieldFourthValue + " " + objNotificationDTO.SortFieldFourthOrder;
                }
            }
            if (objNotificationDTO != null && objNotificationDTO.SortFieldFifthValue != null && objNotificationDTO.SortFieldFifthOrder != null && !string.IsNullOrWhiteSpace(objNotificationDTO.SortFieldFifthValue) && !string.IsNullOrWhiteSpace(objNotificationDTO.SortFieldFifthOrder))
            {
                if (SortSequence != null && !string.IsNullOrWhiteSpace(SortSequence))
                {
                    SortSequence = SortSequence + "," + objNotificationDTO.SortFieldFifthValue + " " + objNotificationDTO.SortFieldFifthOrder;
                }
                else
                {
                    SortSequence = objNotificationDTO.SortFieldFifthValue + " " + objNotificationDTO.SortFieldFifthOrder;
                }
            }
            if (objNotificationDTO != null)
            {

                XMLValue = XMLValue + "<HideHeader>" + Convert.ToString(objNotificationDTO.HideHeader) + "</HideHeader>";
                XMLValue = XMLValue + "<ShowSignature>" + Convert.ToString(objNotificationDTO.ShowSignature) + "</ShowSignature>";
                XMLValue = XMLValue + "<SortSequence>" + Convert.ToString(SortSequence) + "</SortSequence>";
                XMLValue = XMLValue + "<Status>";
                Int64 count = 1;
                Int64 Ordercount = 1;
                if (objNotificationDTO.Status != null)
                {
                    foreach (string s in objNotificationDTO.Status.Split(','))
                    {
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            XMLValue = XMLValue + "<Status" + count + ">" + Convert.ToString(s) + "</Status" + count + ">";
                            count++;
                        }
                    }
                }
                XMLValue = XMLValue + "</Status>";
                XMLValue = XMLValue + "<WOStatus>";
                count = 1;
                if (objNotificationDTO.WOStatus != null)
                {
                    foreach (string s in objNotificationDTO.WOStatus.Split(','))
                    {
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            XMLValue = XMLValue + "<WOStatus" + count + ">" + Convert.ToString(s) + "</WOStatus" + count + ">";
                            count++;
                        }
                    }
                }
                XMLValue = XMLValue + "</WOStatus>";
                XMLValue = XMLValue + "<OrderStatus>";
                Ordercount = 1;
                if (objNotificationDTO.OrderStatus != null)
                {
                    foreach (string s in objNotificationDTO.OrderStatus.Split(','))
                    {
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            XMLValue = XMLValue + "<OrderStatus" + Ordercount + ">" + Convert.ToString(s) + "</OrderStatus" + Ordercount + ">";
                            Ordercount++;
                        }
                    }
                }
                XMLValue = XMLValue + "</OrderStatus>";

                XMLValue = XMLValue + "<OnlyExpiredItems>" + Convert.ToString(objNotificationDTO.OnlyExpiredItems ? "Yes" : string.Empty) + "</OnlyExpiredItems>";
                XMLValue = XMLValue + "<DaysUntilItemExpires>" + Convert.ToString(objNotificationDTO.DaysUntilItemExpires) + "</DaysUntilItemExpires>";
                XMLValue = XMLValue + "<DaysToApproveOrder>" + Convert.ToString(objNotificationDTO.DaysToApproveOrder) + "</DaysToApproveOrder>";
                if (objNotificationDTO.ProjectExpirationDate != null)
                {
                    objNotificationDTO.ProjectExpirationDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(objNotificationDTO.ProjectExpirationDate, (SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString("yyyy-MM-dd HH:mm:ss");
                    XMLValue = XMLValue + "<ProjectExpirationDate>" + Convert.ToString(objNotificationDTO.ProjectExpirationDate) + "</ProjectExpirationDate>";
                }
                else
                {
                    XMLValue = XMLValue + "<ProjectExpirationDate></ProjectExpirationDate>";
                }
                XMLValue = XMLValue + "<OnlyAvailableTools>" + Convert.ToString(objNotificationDTO.OnlyAvailableTools ? "Yes" : string.Empty) + "</OnlyAvailableTools>";
                XMLValue = XMLValue + "<QuantityType>";
                count = 1;
                if (objNotificationDTO.QtyType != null)
                {
                    foreach (string s in objNotificationDTO.QtyType.Split(','))
                    {
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            XMLValue = XMLValue + "<Type" + count + ">" + Convert.ToString(s) + "</Type" + count + ">";
                            count++;
                        }
                    }
                }
                XMLValue = XMLValue + "</QuantityType>";

                XMLValue = XMLValue + "<ActionCodes>";
                count = 1;
                if (objNotificationDTO.ActionCodes != null)
                {
                    foreach (string s in objNotificationDTO.ActionCodes.Split(','))
                    {
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            XMLValue = XMLValue + "<Code" + count + ">" + Convert.ToString(s) + "</Code" + count + ">";
                            count++;
                        }
                    }
                }
                XMLValue = XMLValue + "</ActionCodes>";
                XMLValue = XMLValue + "<CountAppliedFilter>" + Convert.ToString(objNotificationDTO.CountAppliedFilter) + "</CountAppliedFilter>";
                XMLValue = XMLValue + "<UsageType>" + Convert.ToString(objNotificationDTO.UsageType) + "</UsageType>";
                XMLValue = XMLValue + "<IsAllowedZeroPullUsage>" + Convert.ToString(objNotificationDTO.IsAllowedZeroPullUsage ? "Yes" : string.Empty) + "</IsAllowedZeroPullUsage>";
                XMLValue = XMLValue + "<FilterQOH>";
                count = 1;
                if (objNotificationDTO.FilterQOH != null)
                {
                    foreach (string s in objNotificationDTO.FilterQOH.Split(','))
                    {
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            XMLValue = XMLValue + "<FQOH" + count + ">" + Convert.ToString(s) + "</FQOH" + count + ">";
                            count++;
                        }
                    }
                }
                XMLValue = XMLValue + "</FilterQOH>";


                XMLValue = XMLValue + "<MonthlyAverageUsage>" + Convert.ToString(objNotificationDTO.MonthlyAverageUsage ?? 30) + "</MonthlyAverageUsage>";
                string _strInStockOnlyExpItems = "";
                if (objNotificationDTO.OnlyExpirationItems == true)
                {
                    _strInStockOnlyExpItems = "Yes";
                }
                else
                {
                    _strInStockOnlyExpItems = "No";

                }


                XMLValue = XMLValue + "<ItemStatus>";
                count = 1;
                if (objNotificationDTO.ItemStatus != null)
                {
                    foreach (string s in objNotificationDTO.ItemStatus.Split(','))
                    {
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            XMLValue = XMLValue + "<IStatus" + count + ">" + Convert.ToString(s) + "</IStatus" + count + ">";
                            count++;
                        }
                    }
                }
                XMLValue = XMLValue + "</ItemStatus>";

                XMLValue = XMLValue + "<OnlyExpirationItems>" + _strInStockOnlyExpItems + "</OnlyExpirationItems>";

                XMLValue = XMLValue + "<ReportRange>" + (objNotificationDTO.ReportRange ?? string.Empty) + "</ReportRange>";

                XMLValue = XMLValue + "<IsSelectAllRangeData>" + (objNotificationDTO.SelectAllRangeData.ToString()) + "</IsSelectAllRangeData>";

                XMLValue = XMLValue + "<RangeData>";
                count = 1;
                if (objNotificationDTO.ReportRangeData != null && !objNotificationDTO.SelectAllRangeData)
                {
                    foreach (string s in objNotificationDTO.ReportRangeData.Split(','))
                    {
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            XMLValue = XMLValue + "<DataId" + count + ">" + Convert.ToString(s) + "</DataId" + count + ">";
                            count++;
                        }
                    }
                }
                XMLValue = XMLValue + "</RangeData>";

                XMLValue = XMLValue + "<CartType>";
                count = 1;
                if (objNotificationDTO.CartType != null)
                {
                    foreach (string s in objNotificationDTO.CartType.Split(','))
                    {
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            XMLValue = XMLValue + "<CType" + count + ">" + Convert.ToString(s) + "</CType" + count + ">";
                            count++;
                        }
                    }
                }
                XMLValue = XMLValue + "</CartType>";

                XMLValue = XMLValue + "<MoveType>";
                count = 1;
                if (objNotificationDTO.MoveType != null)
                {
                    foreach (string s in objNotificationDTO.MoveType.Split(','))
                    {
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            XMLValue = XMLValue + "<CType" + count + ">" + Convert.ToString(s) + "</CType" + count + ">";
                            count++;
                        }
                    }
                }
                XMLValue = XMLValue + "</MoveType>";

                XMLValue = XMLValue + "<IsIncludeStockouttool>" + Convert.ToString(objNotificationDTO.IsIncludeStockouttool ? "Yes" : string.Empty) + "</IsIncludeStockouttool>";
                XMLValue = XMLValue + "<ExcludeZeroOrdQty>" + Convert.ToString(objNotificationDTO.ExcludeZeroOrdQty ? "Yes" : string.Empty) + "</ExcludeZeroOrdQty>";
                if (objNotificationDTO.AllCheckedOutTools != null)
                {
                    XMLValue = XMLValue + "<AllCheckedOutTools>" + Convert.ToString(objNotificationDTO.AllCheckedOutTools ? "true" : string.Empty) + "</AllCheckedOutTools>";
                }

            }
            //if (!string.IsNullOrWhiteSpace(XMLValue))
            //{

            //    XmlDocument xmldoc = new XmlDocument();
            //    xmldoc.LoadXml("<Data>"+XMLValue+"</Data>");
            //    XmlNodeList nodeList = xmldoc.SelectNodes("/Data/Status");
            //    string Short_Fall = string.Empty;
            //    foreach (XmlNode node in nodeList)
            //    {
            //        for (int i = 1; i <= node.ChildNodes.Count;i++)
            //        {
            //            Short_Fall = node["Status"+i].InnerText;
            //        }
            //    }
            //}
            if (!ModelState.IsValid)
            {
                message = string.Join("; ", ModelState.Values
                                            .SelectMany(x => x.Errors)
                                            .Select(x => x.ErrorMessage));
                status = "fail";
            }
            else if (objNotificationDTO.ScheduleFor == 6 && !string.IsNullOrEmpty(objNotificationDTO.AttachmentReportIDs) &&
                !string.IsNullOrWhiteSpace(objNotificationDTO.AttachmentReportIDs) && string.IsNullOrEmpty(objNotificationDTO.AttachmentTypes))
            {
                message = ResMessage.AttachmentTypeIsMandatoryWithAttachmentReport;//"Please Select Attachment Report";
                status = "fail";
            }
            else
            {
                try
                {
                    string strOK = objNotificationDAL.IsDuplicateSchedule(objSchedulerDTO.ScheduleID, objSchedulerDTO.ScheduleName, SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (strOK == "duplicate")
                    {
                        message = string.Format(ResMessage.DuplicateMessage, ResSchedulerReportList.ScheduleName, objSchedulerDTO.ScheduleName); // "Room \"" + objDTO.RoomName + "\" already exist! Try with Another!";
                        status = "duplicate";
                    }
                    else
                    {

                        objNotificationDTO.RoomID = SessionHelper.RoomID;
                        objNotificationDTO.CompanyID = SessionHelper.CompanyID;
                        objNotificationDTO.CreatedBy = SessionHelper.UserID;
                        objNotificationDTO.UpdatedBy = SessionHelper.UserID;
                        objNotificationDTO.IsDeleted = false;
                        objNotificationDTO.SortSequence = SortSequence;
                        objNotificationDTO.XMLValue = XMLValue;
                        if (!string.IsNullOrWhiteSpace(objNotificationDTO.EmailTemplateDetails))
                        {
                            objNotificationDTO.EmailTemplateDetails = objNotificationDTO.EmailTemplateDetails;
                        }
                        if (objSchedulerDTO != null)
                        {
                            objSchedulerDTO.RoomId = SessionHelper.RoomID;
                            objSchedulerDTO.CompanyId = SessionHelper.CompanyID;
                            objSchedulerDTO.Created = DateTimeUtility.DateTimeNow;
                            objSchedulerDTO.Updated = DateTimeUtility.DateTimeNow;
                            objSchedulerDTO.CreatedBy = SessionHelper.UserID;
                            objSchedulerDTO.LastUpdatedBy = SessionHelper.UserID;

                            List<EmailTemplateDetailDTO> lstEmailDetisl = new List<EmailTemplateDetailDTO>();
                            if (!string.IsNullOrWhiteSpace(objSchedulerDTO.EmailTemplateDetails))
                            {
                                lstEmailDetisl = Newtonsoft.Json.JsonConvert.DeserializeObject<List<EmailTemplateDetailDTO>>(objSchedulerDTO.EmailTemplateDetails);
                                lstEmailDetisl.ForEach(t =>
                                {
                                    t.MailBodyText = HttpUtility.UrlDecode(t.MailBodyText);
                                });
                            }
                            objNotificationDTO.SchedulerParams = objSchedulerDTO;
                            objNotificationDTO.AttachmentReportIDs = objSchedulerDTO.AttachmentReportIDs;
                            objNotificationDTO.AttachmentTypes = objSchedulerDTO.AttachmentTypes;
                            objNotificationDTO.CompanyID = SessionHelper.CompanyID;
                            objNotificationDTO.Created = DateTimeUtility.DateTimeNow;
                            objNotificationDTO.CreatedBy = SessionHelper.UserID;
                            objNotificationDTO.lstEmailTemplateDtls = lstEmailDetisl;

                        }
                        if (!string.IsNullOrEmpty(objNotificationDTO.NotificationName))
                        {
                            string[] arrids = objNotificationDTO.NotificationName.Split('_');
                            objNotificationDTO.ScheduleFor = Convert.ToInt32(arrids[1]);
                            if (objNotificationDTO.ScheduleFor == 5)
                            {
                                objNotificationDTO.Reports = arrids[0];
                                objNotificationDTO.EmailTemplates = string.Empty;
                            }
                            if (objNotificationDTO.ScheduleFor == 6)
                            {
                                objNotificationDTO.Reports = string.Empty;
                                objNotificationDTO.EmailTemplates = arrids[0];
                            }
                        }
                        if (objNotificationDTO.ID == 0)
                        {
                            objNotificationDAL.SaveEmailScheduleSetup(objNotificationDTO);
                            message = ResMessage.SaveMessage;
                            status = "ok";
                        }
                        else
                        {
                            objNotificationDAL.UpdateEmailScheduleSetup(objNotificationDTO);
                            message = ResMessage.SaveMessage;
                            status = "ok";
                        }
                    }
                }
                catch (Exception ex)
                {
                    message = ResMessage.SaveErrorMsg + (Convert.ToString(ex.InnerException) ?? string.Empty);
                    status = "fail";
                }
            }
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);

        }

        //public JsonResult GetEmailTemplateDetails(long EmailTemplateId)
        //{
        //    NotificationDAL objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);

        //    return Json(null);
        //}

        public JsonResult GetAllSchedules(string BinNumber)
        {
            NotificationDAL objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
            List<SchedulerDTO> lstAllSchedule = new List<SchedulerDTO>();
            lstAllSchedule = objNotificationDAL.GetAllSchedules(SessionHelper.RoomID, SessionHelper.CompanyID, BinNumber);
            return Json(lstAllSchedule, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult LoadScheduleParams(string ScheduleName, short LoadSheduleFor, long? NotificationId)
        {
            SchedulerDTO objSchedulerDTO = null;
            NotificationDAL objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
            NotificationDTO objNotificationDTO = new NotificationDTO();
            if ((NotificationId ?? 0) > 0)
            {
                objNotificationDTO = objNotificationDAL.GetNotifiactionByID(NotificationId ?? 0);

            }
            if (!string.IsNullOrWhiteSpace(ScheduleName))
            {
                objSchedulerDTO = objNotificationDAL.GetScheduleByName(SessionHelper.RoomID, SessionHelper.CompanyID, ScheduleName);
                
                if (objNotificationDTO.ID > 0 && objSchedulerDTO != null)
                {
                    eTurns.DAL.RegionSettingDAL objRegionSettingDAL = new eTurns.DAL.RegionSettingDAL(SessionHelper.EnterPriseDBName);
                    objSchedulerDTO.NextRunDateTime = eTurnsWeb.Helper.CommonUtility.ConvertDateByTimeZone(objNotificationDTO.NextRunDate, eTurnsWeb.Helper.SessionHelper.CurrentTimeZone, eTurnsWeb.Helper.SessionHelper.DateTimeFormat, eTurnsWeb.Helper.SessionHelper.RoomCulture, true);
                    objSchedulerDTO.ScheduleRunTime = objNotificationDTO.ScheduleTime.HasValue ? objNotificationDTO.ScheduleTime.Value.ToString(@"hh\:mm") : "00:00";
                    //objSchedulerDTO.ScheduleRunTime = (eTurnsWeb.Helper.CommonUtility.ConvertDateByTimeZonedt(objNotificationDTO.ScheduleRunDateTime, eTurnsWeb.Helper.SessionHelper.CurrentTimeZone, eTurnsWeb.Helper.SessionHelper.DateTimeFormat, eTurnsWeb.Helper.SessionHelper.RoomCulture) ?? DateTime.MinValue).ToString("HH:mm");
                }
            }

            if (objSchedulerDTO == null)
            {
                objSchedulerDTO = new SchedulerDTO();
                objSchedulerDTO.RoomId = SessionHelper.RoomID;
                objSchedulerDTO.CompanyId = SessionHelper.CompanyID;
                objSchedulerDTO.LoadSheduleFor = LoadSheduleFor;

            }
            else
            {
                objSchedulerDTO.LoadSheduleFor = LoadSheduleFor;
            }

            if (IsAllowNewNotification())
            {
                return PartialView("SchedulerInfoNew", objSchedulerDTO);
            }

            return PartialView("SchedulerInfo", objSchedulerDTO);
        }


        public JsonResult DeleteNotification(string ids)
        {
            try
            {
                string response = string.Empty;
                CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                response = objCommonDAL.DeleteModulewise(ids, "NotificationMaster", false, SessionHelper.UserID, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID);
                return Json(new { Message = response, Status = "ok" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { Message = "", Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult NotificationHistory(Int64 ID)
        {
            ViewBag.NotificationID = ID;
            return PartialView("_NotificationHistory");

        }
        public ActionResult NotificationListAjax_ChangeLogListAjax(JQueryDataTableParamModel param)
        {
            Guid ICGUID = Guid.Empty;
            Guid.TryParse(Convert.ToString(Request["IcGuid"]), out ICGUID);

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
            Int32 NotificationID = Convert.ToInt32(Request["NotificationID"]);
            // set the default column sorting here, if first time then required to set 
            if (sortColumnName == "0" || sortColumnName == "undefined" || string.IsNullOrEmpty(sortColumnName))
                sortColumnName = "ID desc";

            if (sortColumnName.Contains("EmailSubject"))
            {
                sortColumnName = sortColumnName.Replace("EmailSubject", "MailSubject");
            }

            int TotalRecordCount = 0;
            InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            bool IsArchived = false;
            bool IsDeleted = false;
            NotificationDAL objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);

            //ResourceHelper.CurrentCult.Name 

            List<NotificationDTO> DataFromDB = objNotificationDAL.GetPagedNotifications_ChangeLog(NotificationID, param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, SessionHelper.UserID, ResourceHelper.CurrentCult.Name);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = DataFromDB
            }, JsonRequestBehavior.AllowGet);
        }

        private bool IsAllowNewNotification()
        {
            //if (Settinfile.Element("IsNewNotification") != null)
            if (SiteSettingHelper.IsNewNotification != string.Empty) 
            {
                string _strIsNewNotification = SiteSettingHelper.IsNewNotification; // Settinfile.Element("IsNewNotification").Value;
                if ((_strIsNewNotification ?? string.Empty).ToLower() == "yes")
                {
                    return true;
                }
                else
                {
                    try
                    {
                        //if (Settinfile.Element("AllowEnterpriseRoomForNN") != null)
                        if (SiteSettingHelper.AllowEnterpriseRoomForNN  != string.Empty)
                        {
                            string entRoomList = SiteSettingHelper.AllowEnterpriseRoomForNN; //Settinfile.Element("AllowEnterpriseRoomForNN").Value;
                            if (!string.IsNullOrWhiteSpace(entRoomList))
                            {
                                List<string> entAndRoomList = entRoomList.Split('|').ToList();
                                string entAndRoom = entAndRoomList.Where(x => x.Contains(SessionHelper.EnterPriceID.ToString() + "-")).FirstOrDefault();
                                if (!string.IsNullOrWhiteSpace(entAndRoom))
                                {
                                    string entRoom = entAndRoom.Split('-')[1];
                                    if (!string.IsNullOrEmpty(entRoom))
                                    {
                                        List<string> roomList = entRoom.Split(',').ToList();
                                        if (roomList != null && roomList.Count > 0)
                                        {
                                            string isRoomAvail = roomList.Where(x => x == SessionHelper.RoomID.ToString()).FirstOrDefault();
                                            if (!string.IsNullOrWhiteSpace(isRoomAvail))
                                            {
                                                return true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return true;
                                    }

                                }

                            }
                        }
                    }
                    catch
                    {
                        return false;

                    }
                }
            }

            return false;
        }

        #region [NewNot]
        public ActionResult NotificationList()
        {
            return View();
        }

        public ActionResult NotificationCreate()
        {
            ViewBag.forcopy = false;
            NotificationDAL objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
            EmailTemplateDAL objEmailTemplateDAL = new EmailTemplateDAL(SessionHelper.EnterPriseDBName);
            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
            SFTPDAL objSFTPDAL = new SFTPDAL(SessionHelper.EnterPriseDBName);
            List<FTPMasterDTO> lstFtps = objSFTPDAL.GetAllFtpForRoom(SessionHelper.RoomID, SessionHelper.CompanyID);
            List<EmailTemplateDTO> lstEmailTemplates = objEmailTemplateDAL.GetAllEmailTemplateToNotify();
            List<ReportBuilderDTO> lstReportBuilderDTO = objReportMasterDAL.GetReportList(SessionHelper.UserID).Where(x => ((x.CompanyID == SessionHelper.CompanyID || x.CompanyID == 0) && x.IsDeleted.GetValueOrDefault(false) == false && x.IsArchived.GetValueOrDefault(false) == false && x.ISEnterpriseReport.GetValueOrDefault(false) == false) || (x.ISEnterpriseReport.GetValueOrDefault(false) == true && x.IsDeleted.GetValueOrDefault(false) == false && x.IsArchived.GetValueOrDefault(false) == false)).OrderBy(x => x.ReportName).ToList();
            lstReportBuilderDTO = lstReportBuilderDTO.Where(x => x.ModuleName != "EnterpriseList").OrderBy(x => x.ReportName).ToList();
            List<SupplierMasterDTO> lstSuppliers = objSupplierMasterDAL.GetSupplierByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID, false);
            List<SelectListItem> lstLanguage = GetLanguage();

            if (SessionHelper.UserSupplierIds != null && SessionHelper.UserSupplierIds.Any())
                lstSuppliers = lstSuppliers.Where(x => SessionHelper.UserSupplierIds.Contains(x.ID)).ToList();

            List<AlertReportDTO> lstAlertReports = new List<AlertReportDTO>();

            foreach (var item in lstEmailTemplates)
            {
                lstAlertReports.Add(new AlertReportDTO() { ID = item.ID + "_6", AlertReportName = item.TemplateName, ScheduleFor = 6 });
            }

            foreach (var item in lstReportBuilderDTO)
            {
                lstAlertReports.Add(new AlertReportDTO() { ID = item.ID + "_5", AlertReportName = item.ReportName, ScheduleFor = 5 });
            }

            lstAlertReports = lstAlertReports.OrderBy(t => t.AlertReportName).ToList();
            ViewBag.AlertReport = lstAlertReports;
            ViewBag.DDLanguage = lstLanguage;
            ViewBag.lstReportList = lstReportBuilderDTO;
            ViewBag.Suppliers = lstSuppliers;
            ViewBag.EmailTemplates = lstEmailTemplates;
            ViewBag.FtpDetails = lstFtps;
            NotificationDTO objNotificationDTO = new NotificationDTO();


            SchedulerDTO objSchedulerDTO = new SchedulerDTO();
            objSchedulerDTO.RoomId = SessionHelper.RoomID;
            objSchedulerDTO.CompanyId = SessionHelper.CompanyID;
            objSchedulerDTO.LoadSheduleFor = 6;
            objNotificationDTO.SchedulerParams = objSchedulerDTO;
            objNotificationDTO.RoomID = SessionHelper.RoomID;
            objNotificationDTO.CompanyID = SessionHelper.CompanyID;
            objNotificationDTO.ScheduleFor = 6;
            objNotificationDTO.NotificationMode = 1;
            return PartialView("_CreateNotification", objNotificationDTO);
        }


        public ActionResult NotificationEdit(long ID, bool? forcopy)
        {
            ViewBag.forcopy = forcopy;
            EmailTemplateDAL objEmailTemplateDAL = new EmailTemplateDAL(SessionHelper.EnterPriseDBName);
            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
            NotificationDAL objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
            SFTPDAL objSFTPDAL = new SFTPDAL(SessionHelper.EnterPriseDBName);
            List<EmailTemplateDTO> lstEmailTemplates = objEmailTemplateDAL.GetAllEmailTemplateToNotify();
            List<ReportBuilderDTO> lstReportBuilderDTO = objReportMasterDAL.GetReportList(SessionHelper.UserID).Where(x => ((x.CompanyID == SessionHelper.CompanyID || x.CompanyID == 0) && x.IsDeleted.GetValueOrDefault(false) == false && x.IsArchived.GetValueOrDefault(false) == false && x.ISEnterpriseReport.GetValueOrDefault(false) == false) || (x.ISEnterpriseReport.GetValueOrDefault(false) == true && x.IsDeleted.GetValueOrDefault(false) == false && x.IsArchived.GetValueOrDefault(false) == false)).OrderBy(x => x.ReportName).ToList();
            lstReportBuilderDTO = lstReportBuilderDTO.Where(x => x.ModuleName != "EnterpriseList").OrderBy(x => x.ReportName).ToList();
            List<SupplierMasterDTO> lstSuppliers = objSupplierMasterDAL.GetSupplierByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID, false).ToList();
            List<SelectListItem> lstLanguage = GetLanguage();
            List<FTPMasterDTO> lstFtps = objSFTPDAL.GetAllFtpForRoom(SessionHelper.RoomID, SessionHelper.CompanyID);

            if (SessionHelper.UserSupplierIds != null && SessionHelper.UserSupplierIds.Any())
                lstSuppliers = lstSuppliers.Where(x => SessionHelper.UserSupplierIds.Contains(x.ID)).ToList();

            ViewBag.DDLanguage = lstLanguage;
            ViewBag.lstReportList = lstReportBuilderDTO;
            ViewBag.Suppliers = lstSuppliers;
            ViewBag.EmailTemplates = lstEmailTemplates;
            ViewBag.FtpDetails = lstFtps;

            NotificationDTO objNotificationDTO = objNotificationDAL.GetNotifiactionByID(ID);
            List<AlertReportDTO> lstAlertReports = new List<AlertReportDTO>();
            foreach (var item in lstEmailTemplates)
            {
                lstAlertReports.Add(new AlertReportDTO() { ID = item.ID + "_6", AlertReportName = item.TemplateName, ScheduleFor = 6 });
            }
            foreach (var item in lstReportBuilderDTO)
            {
                lstAlertReports.Add(new AlertReportDTO() { ID = item.ID + "_5", AlertReportName = item.ReportName, ScheduleFor = 5 });
            }
            lstAlertReports = lstAlertReports.OrderBy(t => t.AlertReportName).ToList();
            ViewBag.AlertReport = lstAlertReports;
            if (objNotificationDTO == null)
            {
                objNotificationDTO = new NotificationDTO();
                SchedulerDTO objSchedulerDTO = new SchedulerDTO();
                objSchedulerDTO.RoomId = SessionHelper.RoomID;
                objSchedulerDTO.CompanyId = SessionHelper.CompanyID;
                objSchedulerDTO.LoadSheduleFor = 6;
                objNotificationDTO.SchedulerParams = objSchedulerDTO;
                objNotificationDTO.RoomID = SessionHelper.RoomID;
                objNotificationDTO.CompanyID = SessionHelper.CompanyID;
                objNotificationDTO.ScheduleFor = 6;
            }
            if (objNotificationDTO.ScheduleFor == 5)
            {
                objNotificationDTO.NotificationName = objNotificationDTO.ReportID + "_5";
            }
            else if (objNotificationDTO.ScheduleFor == 6)
            {
                objNotificationDTO.NotificationName = objNotificationDTO.EmailTemplateID + "_6";
            }
            string RequisitionStatus = string.Empty;
            string WOStatus = string.Empty;
            string OrderStatus = string.Empty;
            string QtyType = string.Empty;
            string ActionCodes = string.Empty;
            string DaysUntilItemExpires = string.Empty;
            string DaysToApproveOrder = string.Empty;
            string ProjectExpirationDate = string.Empty;
            string _FilterQOH = string.Empty;
            string _MonthlyAverageUsage = string.Empty;
            string _ReportRange = string.Empty;
            string CartType = string.Empty;
            string MoveType = string.Empty;
            string IsIncludeStockouttool = string.Empty;
            if (objNotificationDTO != null && (objNotificationDTO.XMLValue) != null && (!string.IsNullOrWhiteSpace(objNotificationDTO.XMLValue)))
            {
                if ((objNotificationDTO.ReportMasterDTO != null && !string.IsNullOrEmpty(objNotificationDTO.ReportMasterDTO.ModuleName)
                    && objNotificationDTO.ReportMasterDTO.ModuleName.Trim().ToLower() == "tool")
                   || (objNotificationDTO.AttachedReportMasterDTO != null && objNotificationDTO.AttachedReportMasterDTO.ModuleName != null
                    && (objNotificationDTO.AttachedReportMasterDTO.ModuleName.Trim().ToLower() == "tool"))
                    && objNotificationDTO.XMLValue.ToLower().IndexOf("onlyavailabletools") >= 0
                    )
                {
                    XDocument xDoc = XDocument.Parse("<Data>" + objNotificationDTO.XMLValue + "</Data>");
                    if (xDoc.Descendants("OnlyAvailableTools").FirstOrDefault() != null && xDoc.Descendants("OnlyAvailableTools").FirstOrDefault().Value == "Yes")
                        objNotificationDTO.OnlyAvailableTools = true;
                }
                if (objNotificationDTO.XMLValue.ToLower().IndexOf("status") >= 0)
                {
                    if ((objNotificationDTO != null && objNotificationDTO.ReportName != null && (objNotificationDTO.ReportName.Trim().ToLower() == "requisition" || objNotificationDTO.ReportName.Trim().ToLower() == "requisition with lineitems"))
                    || (objNotificationDTO != null && objNotificationDTO.ReportMasterDTO != null && objNotificationDTO.ReportMasterDTO.ModuleName != null && (objNotificationDTO.ReportMasterDTO.ModuleName.Trim().ToLower() == "consume_requisition" || objNotificationDTO.ReportMasterDTO.ModuleName.Trim().ToLower() == "range-consume_requisition"))
                    || (objNotificationDTO != null && objNotificationDTO.AttachedReportMasterDTO != null && objNotificationDTO.AttachedReportMasterDTO.ModuleName != null && (objNotificationDTO.AttachedReportMasterDTO.ModuleName.Trim().ToLower() == "consume_requisition" || objNotificationDTO.AttachedReportMasterDTO.ModuleName.Trim().ToLower() == "range-consume_requisition")))
                    {
                        XmlDocument xmldoc = new XmlDocument();
                        xmldoc.LoadXml("<Data>" + objNotificationDTO.XMLValue + "</Data>");
                        XmlNodeList nodeList = xmldoc.SelectNodes("/Data/Status");
                        if (nodeList != null && nodeList.Count > 0)
                        {
                            foreach (XmlNode node in nodeList)
                            {
                                for (int i = 1; i <= node.ChildNodes.Count; i++)
                                {
                                    if (!string.IsNullOrWhiteSpace(node["Status" + i].InnerText))
                                    {
                                        if (!string.IsNullOrWhiteSpace(RequisitionStatus))
                                        {
                                            RequisitionStatus += "," + node["Status" + i].InnerText;
                                        }
                                        else
                                        {
                                            RequisitionStatus += node["Status" + i].InnerText;
                                        }
                                    }
                                }
                            }
                        }
                        nodeList = xmldoc.SelectNodes("/Data/WOStatus");
                        if (nodeList != null && nodeList.Count > 0)
                        {
                            foreach (XmlNode node in nodeList)
                            {
                                for (int i = 1; i <= node.ChildNodes.Count; i++)
                                {
                                    if (!string.IsNullOrWhiteSpace(node["WOStatus" + i].InnerText))
                                    {
                                        if (!string.IsNullOrWhiteSpace(WOStatus))
                                        {
                                            WOStatus += "," + node["WOStatus" + i].InnerText;
                                        }
                                        else
                                        {
                                            WOStatus += node["WOStatus" + i].InnerText;
                                        }
                                    }
                                }
                            }
                        }
                        nodeList = xmldoc.SelectNodes("/Data/OrderStatus");
                        if (nodeList != null && nodeList.Count > 0)
                        {
                            foreach (XmlNode node in nodeList)
                            {
                                for (int i = 1; i <= node.ChildNodes.Count; i++)
                                {
                                    if (!string.IsNullOrWhiteSpace(node["OrderStatus" + i].InnerText))
                                    {
                                        if (!string.IsNullOrWhiteSpace(OrderStatus))
                                        {
                                            OrderStatus += "," + node["OrderStatus" + i].InnerText;
                                        }
                                        else
                                        {
                                            OrderStatus += node["OrderStatus" + i].InnerText;
                                        }
                                    }
                                }
                            }
                        }
                        objNotificationDTO.Status = RequisitionStatus;
                        objNotificationDTO.WOStatus = WOStatus;
                        objNotificationDTO.OrderStatus = OrderStatus;
                    }
                }
                if (objNotificationDTO.XMLValue.ToLower().IndexOf("orderstatus") >= 0)
                {
                    if ((objNotificationDTO != null && objNotificationDTO.ReportName != null && objNotificationDTO.ReportName.Trim().ToLower() == "order") ||
                    (objNotificationDTO != null && objNotificationDTO.ReportMasterDTO != null && objNotificationDTO.ReportMasterDTO.ModuleName != null && objNotificationDTO.ReportMasterDTO.ModuleName.Trim().ToLower() == "order")
                    || (objNotificationDTO != null && objNotificationDTO.AttachedReportMasterDTO != null && objNotificationDTO.AttachedReportMasterDTO.ModuleName != null && objNotificationDTO.AttachedReportMasterDTO.ModuleName.Trim().ToLower() == "order"))
                    {
                        XmlDocument xmldoc = new XmlDocument();
                        xmldoc.LoadXml("<Data>" + objNotificationDTO.XMLValue + "</Data>");
                        XmlNodeList nodeList = xmldoc.SelectNodes("/Data/OrderStatus");
                        if (nodeList != null && nodeList.Count > 0)
                        {
                            foreach (XmlNode node in nodeList)
                            {
                                for (int i = 1; i <= node.ChildNodes.Count; i++)
                                {
                                    if (!string.IsNullOrWhiteSpace(node["OrderStatus" + i].InnerText))
                                    {
                                        if (!string.IsNullOrWhiteSpace(OrderStatus))
                                        {
                                            OrderStatus += "," + node["OrderStatus" + i].InnerText;
                                        }
                                        else
                                        {
                                            OrderStatus += node["OrderStatus" + i].InnerText;
                                        }
                                    }
                                }
                            }
                        }
                        objNotificationDTO.Status = RequisitionStatus;
                        objNotificationDTO.WOStatus = WOStatus;
                        objNotificationDTO.OrderStatus = OrderStatus;
                    }
                }
                if (objNotificationDTO.XMLValue.ToLower().IndexOf("wostatus") >= 0)
                {
                    if ((objNotificationDTO != null && objNotificationDTO.ReportName != null && objNotificationDTO.ReportName.Trim().ToLower() == "work order") ||
                    (objNotificationDTO != null && objNotificationDTO.ReportMasterDTO != null && objNotificationDTO.ReportMasterDTO.ModuleName != null && objNotificationDTO.ReportMasterDTO.ModuleName.Trim().ToLower() == "workorder")
                    || (objNotificationDTO != null && objNotificationDTO.AttachedReportMasterDTO != null && objNotificationDTO.AttachedReportMasterDTO.ModuleName != null && objNotificationDTO.AttachedReportMasterDTO.ModuleName.Trim().ToLower() == "workorder"))

                    {
                        XmlDocument xmldoc = new XmlDocument();
                        xmldoc.LoadXml("<Data>" + objNotificationDTO.XMLValue + "</Data>");
                        XmlNodeList nodeList = xmldoc.SelectNodes("/Data/WOStatus");
                        if (nodeList != null && nodeList.Count > 0)
                        {
                            foreach (XmlNode node in nodeList)
                            {
                                for (int i = 1; i <= node.ChildNodes.Count; i++)
                                {
                                    if (!string.IsNullOrWhiteSpace(node["WOStatus" + i].InnerText))
                                    {
                                        if (!string.IsNullOrWhiteSpace(WOStatus))
                                        {
                                            WOStatus += "," + node["WOStatus" + i].InnerText;
                                        }
                                        else
                                        {
                                            WOStatus += node["WOStatus" + i].InnerText;
                                        }
                                    }
                                }
                            }
                        }
                        objNotificationDTO.Status = RequisitionStatus;
                        objNotificationDTO.WOStatus = WOStatus;
                        objNotificationDTO.OrderStatus = OrderStatus;
                    }
                }
                if ((objNotificationDTO != null && objNotificationDTO.ReportMasterDTO != null && objNotificationDTO.ReportMasterDTO.ModuleName != null && (objNotificationDTO.ReportMasterDTO.ModuleName.Trim().ToLower() == "expiringitems"))
                   || (objNotificationDTO != null && objNotificationDTO.AttachedReportMasterDTO != null && objNotificationDTO.AttachedReportMasterDTO.ModuleName != null && (objNotificationDTO.AttachedReportMasterDTO.ModuleName.Trim().ToLower() == "expiringitems")))
                {
                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.LoadXml("<Data>" + objNotificationDTO.XMLValue + "</Data>");
                    XmlNodeList nodeList = xmldoc.SelectNodes("/Data/OnlyExpiredItems");
                    if (nodeList != null && nodeList.Count > 0)
                    {
                        RequisitionStatus = nodeList[0].InnerText;
                    }
                    nodeList = xmldoc.SelectNodes("/Data/DaysUntilItemExpires");
                    if (nodeList != null && nodeList.Count > 0)
                    {
                        DaysUntilItemExpires = nodeList[0].InnerText;
                    }
                    nodeList = xmldoc.SelectNodes("/Data/DaysToApproveOrder");
                    if (nodeList != null && nodeList.Count > 0)
                    {
                        DaysToApproveOrder = nodeList[0].InnerText;
                    }
                    nodeList = xmldoc.SelectNodes("/Data/ProjectExpirationDate");
                    if (nodeList != null && nodeList.Count > 0)
                    {
                        ProjectExpirationDate = nodeList[0].InnerText;
                    }
                    objNotificationDTO.OnlyExpiredItems = RequisitionStatus == "Yes" ? true : false;
                    objNotificationDTO.DaysUntilItemExpires = DaysUntilItemExpires;
                    objNotificationDTO.DaysToApproveOrder = DaysToApproveOrder;
                    if (!string.IsNullOrWhiteSpace(ProjectExpirationDate))
                    {
                        objNotificationDTO.ProjectExpirationDate = FnCommon.ConvertDateByTimeZone(Convert.ToDateTime(ProjectExpirationDate), true).Split(' ')[0];// ProjectExpirationDate.ToString(SessionHelper.RoomDateFormat);
                    }
                    // objNotificationDTO.ProjectExpirationDate = ProjectExpirationDate;
                    //objNotificationDTO.WOStatus = WOStatus;
                }
                if (objNotificationDTO.XMLValue.ToLower().IndexOf("quantitytype") >= 0)
                {
                    if ((objNotificationDTO != null && objNotificationDTO.ReportName != null && (objNotificationDTO.ReportName.Trim().ToLower() == "pull" || objNotificationDTO.ReportName.Trim().ToLower() == "pull summary" || objNotificationDTO.ReportName.Trim().ToLower() == "pull summary by quarter")) ||
                    (objNotificationDTO != null && objNotificationDTO.ReportMasterDTO != null && objNotificationDTO.ReportMasterDTO.ModuleName != null && (objNotificationDTO.ReportName.Trim().ToLower() == "pull" || objNotificationDTO.ReportName.Trim().ToLower() == "pull summary" || objNotificationDTO.ReportName.Trim().ToLower() == "pull summary by quarter"))
                    || (objNotificationDTO != null && objNotificationDTO.AttachedReportMasterDTO != null && objNotificationDTO.AttachedReportMasterDTO.ModuleName != null && ((objNotificationDTO.AttachedReportMasterDTO.ModuleName.Trim().ToLower() == "consume_pull") || (objNotificationDTO.AttachedReportMasterDTO.ModuleName.Trim().ToLower() == "not consume_pull"))))

                    {
                        XmlDocument xmldoc = new XmlDocument();
                        xmldoc.LoadXml("<Data>" + objNotificationDTO.XMLValue + "</Data>");
                        XmlNodeList nodeList = xmldoc.SelectNodes("/Data/QuantityType");
                        if (nodeList != null && nodeList.Count > 0)
                        {
                            foreach (XmlNode node in nodeList)
                            {
                                for (int i = 1; i <= node.ChildNodes.Count; i++)
                                {
                                    if (!string.IsNullOrWhiteSpace(node["Type" + i].InnerText))
                                    {
                                        if (!string.IsNullOrWhiteSpace(QtyType))
                                        {
                                            QtyType += "," + node["Type" + i].InnerText;
                                        }
                                        else
                                        {
                                            QtyType += node["Type" + i].InnerText;
                                        }
                                    }
                                }
                            }
                        }

                        objNotificationDTO.QtyType = QtyType;
                    }
                }

                if (objNotificationDTO != null && objNotificationDTO.XMLValue.ToLower().IndexOf("actioncodes") >= 0)
                {

                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.LoadXml("<Data>" + objNotificationDTO.XMLValue + "</Data>");
                    XmlNodeList nodeList = xmldoc.SelectNodes("/Data/ActionCodes");
                    if (nodeList != null && nodeList.Count > 0)
                    {
                        foreach (XmlNode node in nodeList)
                        {
                            for (int i = 1; i <= node.ChildNodes.Count; i++)
                            {
                                if (!string.IsNullOrWhiteSpace(node["Code" + i].InnerText))
                                {
                                    if (!string.IsNullOrWhiteSpace(ActionCodes))
                                    {
                                        ActionCodes += "," + node["Code" + i].InnerText;
                                    }
                                    else
                                    {
                                        ActionCodes += node["Code" + i].InnerText;
                                    }
                                }
                            }
                        }
                    }

                    objNotificationDTO.ActionCodes = ActionCodes;
                }



                if (objNotificationDTO != null && objNotificationDTO.XMLValue.ToLower().IndexOf("filterqoh") >= 0)
                {

                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.LoadXml("<Data>" + objNotificationDTO.XMLValue + "</Data>");
                    XmlNodeList nodeList = xmldoc.SelectNodes("/Data/FilterQOH");
                    if (nodeList != null && nodeList.Count > 0)
                    {
                        foreach (XmlNode node in nodeList)
                        {
                            for (int i = 1; i <= node.ChildNodes.Count; i++)
                            {
                                if (!string.IsNullOrWhiteSpace(node["FQOH" + i].InnerText))
                                {
                                    if (!string.IsNullOrWhiteSpace(_FilterQOH))
                                    {
                                        _FilterQOH += "," + node["FQOH" + i].InnerText;
                                    }
                                    else
                                    {
                                        _FilterQOH += node["FQOH" + i].InnerText;
                                    }
                                }
                            }
                        }
                    }

                    objNotificationDTO.FilterQOH = _FilterQOH;
                }

                if (objNotificationDTO != null && objNotificationDTO.XMLValue.ToLower().IndexOf("monthlyaverageusage") >= 0)
                {
                    XDocument xDoc = XDocument.Parse("<Data>" + objNotificationDTO.XMLValue + "</Data>");
                    long _tempMonthlyAvgUse = 30;
                    long.TryParse(xDoc.Descendants("MonthlyAverageUsage").FirstOrDefault().Value, out _tempMonthlyAvgUse);
                    objNotificationDTO.MonthlyAverageUsage = _tempMonthlyAvgUse;
                }

                if (objNotificationDTO != null && objNotificationDTO.XMLValue.ToLower().IndexOf("reportrange") >= 0)
                {
                    XDocument xDoc = XDocument.Parse("<Data>" + objNotificationDTO.XMLValue + "</Data>");
                    objNotificationDTO.ReportRange = Convert.ToString(xDoc.Descendants("ReportRange").FirstOrDefault().Value);
                }

                if (objNotificationDTO.XMLValue.ToLower().IndexOf("carttype") >= 0)
                {
                    if (objNotificationDTO != null && objNotificationDTO.ReportName != null
                        && objNotificationDTO.AttachedReportMasterDTO.ModuleName.Trim().ToLower() == "cart")
                    {
                        XmlDocument xmldoc = new XmlDocument();
                        xmldoc.LoadXml("<Data>" + objNotificationDTO.XMLValue + "</Data>");
                        XmlNodeList nodeList = xmldoc.SelectNodes("/Data/CartType");
                        if (nodeList != null && nodeList.Count > 0)
                        {
                            foreach (XmlNode node in nodeList)
                            {
                                for (int i = 1; i <= node.ChildNodes.Count; i++)
                                {
                                    if (!string.IsNullOrWhiteSpace(node["CType" + i].InnerText))
                                    {
                                        if (!string.IsNullOrWhiteSpace(CartType))
                                        {
                                            CartType += "," + node["CType" + i].InnerText;
                                        }
                                        else
                                        {
                                            CartType += node["CType" + i].InnerText;
                                        }
                                    }
                                }
                            }
                        }

                        objNotificationDTO.CartType = CartType;
                    }
                }

                if ((objNotificationDTO.ReportMasterDTO != null && !string.IsNullOrEmpty(objNotificationDTO.ReportMasterDTO.ModuleName)
                    && objNotificationDTO.ReportMasterDTO.ModuleName.Trim().ToLower() == "ToolInStock".ToLower())
                    && objNotificationDTO.XMLValue.ToLower().IndexOf("isincludestockouttool") >= 0
                    )
                {
                    XDocument xDoc = XDocument.Parse("<Data>" + objNotificationDTO.XMLValue + "</Data>");
                    if (xDoc.Descendants("IsIncludeStockouttool").FirstOrDefault() != null && xDoc.Descendants("IsIncludeStockouttool").FirstOrDefault().Value == "Yes")
                        objNotificationDTO.IsIncludeStockouttool = true;
                }

                if (objNotificationDTO.XMLValue.ToLower().IndexOf("excludezeroordqty") >= 0)
                {
                    XDocument xDoc = XDocument.Parse("<Data>" + objNotificationDTO.XMLValue + "</Data>");
                    if (xDoc.Descendants("ExcludeZeroOrdQty").FirstOrDefault() != null && xDoc.Descendants("ExcludeZeroOrdQty").FirstOrDefault().Value == "Yes")
                        objNotificationDTO.ExcludeZeroOrdQty = true;
                }

                if (objNotificationDTO.XMLValue.ToLower().IndexOf("movetype") >= 0)
                {
                    if (objNotificationDTO != null && objNotificationDTO.ReportName != null
                        && objNotificationDTO.AttachedReportMasterDTO.ModuleName.Trim().ToLower() == "movematerial")
                    {
                        XmlDocument xmldoc = new XmlDocument();
                        xmldoc.LoadXml("<Data>" + objNotificationDTO.XMLValue + "</Data>");
                        XmlNodeList nodeList = xmldoc.SelectNodes("/Data/MoveType");
                        if (nodeList != null && nodeList.Count > 0)
                        {
                            foreach (XmlNode node in nodeList)
                            {
                                for (int i = 1; i <= node.ChildNodes.Count; i++)
                                {
                                    if (!string.IsNullOrWhiteSpace(node["CType" + i].InnerText))
                                    {
                                        if (!string.IsNullOrWhiteSpace(MoveType))
                                        {
                                            MoveType += "," + node["CType" + i].InnerText;
                                        }
                                        else
                                        {
                                            MoveType += node["CType" + i].InnerText;
                                        }
                                    }
                                }
                            }
                        }

                        objNotificationDTO.MoveType = MoveType;
                    }
                }

                if (objNotificationDTO.XMLValue.ToLower().IndexOf("allcheckedouttools") >= 0)
                {
                    XDocument xDoc = XDocument.Parse("<Data>" + objNotificationDTO.XMLValue + "</Data>");
                    if (xDoc.Descendants("AllCheckedOutTools").FirstOrDefault() != null && xDoc.Descendants("AllCheckedOutTools").FirstOrDefault().Value == "true")
                        objNotificationDTO.AllCheckedOutTools = true;
                }
            }
            return PartialView("_CreateNotification", objNotificationDTO);
        }

        public ActionResult NotificationAjax(JQueryDataTableParamModel param)
        {
            Guid ICGUID = Guid.Empty;
            Guid.TryParse(Convert.ToString(Request["IcGuid"]), out ICGUID);

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

            // set the default column sorting here, if first time then required to set 
            if (sortColumnName == "0" || sortColumnName == "undefined" || string.IsNullOrEmpty(sortColumnName))
                sortColumnName = "ID desc";

            if (sortColumnName.Contains("EmailSubject"))
            {
                sortColumnName = sortColumnName.Replace("EmailSubject", "MailSubject");
            }

            int TotalRecordCount = 0;
            InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            bool IsArchived = false;
            bool IsDeleted = false;
            NotificationDAL objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);

            //ResourceHelper.CurrentCult.Name 

            List<NotificationDTO> DataFromDB = objNotificationDAL.GetPagedNotifications(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, SessionHelper.UserID, ResourceHelper.CurrentCult.Name);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = DataFromDB
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult NotificationSingleSave(NotificationDTO objNotificationDTO, SchedulerDTO objSchedulerDTO)
        {
            string message = "";
            string status = "";
            NotificationDAL objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
            ModelState.Clear();
            string SortSequence = string.Empty;
            string XMLValue = string.Empty;
            if (objNotificationDTO != null)
            {
                //if (objNotificationDTO.ScheduleFor == 5)
                //{
                if (!string.IsNullOrWhiteSpace(objNotificationDTO.CompanyIds))
                {
                    objNotificationDTO.CompanyIds = objNotificationDTO.CompanyIds.TrimEnd(',');
                }
                else
                {
                    objNotificationDTO.CompanyIds = Convert.ToString(SessionHelper.CompanyID);
                }
                if (!string.IsNullOrWhiteSpace(objNotificationDTO.RoomIds))
                {
                    objNotificationDTO.RoomIds = objNotificationDTO.RoomIds.TrimEnd(',');
                }
                else
                {
                    objNotificationDTO.RoomIds = Convert.ToString(SessionHelper.RoomID);
                }
                //}
                //else
                //{
                //    objNotificationDTO.CompanyIds = string.Empty;
                //    objNotificationDTO.RoomIds = string.Empty;
                //}
            }
            if (objNotificationDTO != null && objNotificationDTO.SortFieldFirstValue != null && objNotificationDTO.SortFieldFirstOrder != null && !string.IsNullOrWhiteSpace(objNotificationDTO.SortFieldFirstValue) && !string.IsNullOrWhiteSpace(objNotificationDTO.SortFieldFirstOrder))
            {
                if (SortSequence != null && !string.IsNullOrWhiteSpace(SortSequence))
                {
                    SortSequence = SortSequence + "," + objNotificationDTO.SortFieldFirstValue + " " + objNotificationDTO.SortFieldFirstOrder;
                }
                else
                {
                    SortSequence = objNotificationDTO.SortFieldFirstValue + " " + objNotificationDTO.SortFieldFirstOrder;
                }
            }
            if (objNotificationDTO != null && objNotificationDTO.SortFieldSecondValue != null && objNotificationDTO.SortFieldSecondOrder != null && !string.IsNullOrWhiteSpace(objNotificationDTO.SortFieldSecondValue) && !string.IsNullOrWhiteSpace(objNotificationDTO.SortFieldSecondOrder))
            {
                if (SortSequence != null && !string.IsNullOrWhiteSpace(SortSequence))
                {
                    SortSequence = SortSequence + "," + objNotificationDTO.SortFieldSecondValue + " " + objNotificationDTO.SortFieldSecondOrder;
                }
                else
                {
                    SortSequence = objNotificationDTO.SortFieldSecondValue + " " + objNotificationDTO.SortFieldSecondOrder;
                }
            }
            if (objNotificationDTO != null && objNotificationDTO.SortFieldThirdValue != null && objNotificationDTO.SortFieldThirdOrder != null && !string.IsNullOrWhiteSpace(objNotificationDTO.SortFieldThirdValue) && !string.IsNullOrWhiteSpace(objNotificationDTO.SortFieldThirdOrder))
            {
                if (SortSequence != null && !string.IsNullOrWhiteSpace(SortSequence))
                {
                    SortSequence = SortSequence + "," + objNotificationDTO.SortFieldThirdValue + " " + objNotificationDTO.SortFieldThirdOrder;
                }
                else
                {
                    SortSequence = objNotificationDTO.SortFieldThirdValue + " " + objNotificationDTO.SortFieldThirdOrder;
                }
            }
            if (objNotificationDTO != null && objNotificationDTO.SortFieldFourthValue != null && objNotificationDTO.SortFieldFourthOrder != null && !string.IsNullOrWhiteSpace(objNotificationDTO.SortFieldFourthValue) && !string.IsNullOrWhiteSpace(objNotificationDTO.SortFieldFourthOrder))
            {
                if (SortSequence != null && !string.IsNullOrWhiteSpace(SortSequence))
                {
                    SortSequence = SortSequence + "," + objNotificationDTO.SortFieldFourthValue + " " + objNotificationDTO.SortFieldFourthOrder;
                }
                else
                {
                    SortSequence = objNotificationDTO.SortFieldFourthValue + " " + objNotificationDTO.SortFieldFourthOrder;
                }
            }
            if (objNotificationDTO != null && objNotificationDTO.SortFieldFifthValue != null && objNotificationDTO.SortFieldFifthOrder != null && !string.IsNullOrWhiteSpace(objNotificationDTO.SortFieldFifthValue) && !string.IsNullOrWhiteSpace(objNotificationDTO.SortFieldFifthOrder))
            {
                if (SortSequence != null && !string.IsNullOrWhiteSpace(SortSequence))
                {
                    SortSequence = SortSequence + "," + objNotificationDTO.SortFieldFifthValue + " " + objNotificationDTO.SortFieldFifthOrder;
                }
                else
                {
                    SortSequence = objNotificationDTO.SortFieldFifthValue + " " + objNotificationDTO.SortFieldFifthOrder;
                }
            }
            if (objNotificationDTO != null)
            {

                XMLValue = XMLValue + "<HideHeader>" + Convert.ToString(objNotificationDTO.HideHeader) + "</HideHeader>";
                XMLValue = XMLValue + "<ShowSignature>" + Convert.ToString(objNotificationDTO.ShowSignature) + "</ShowSignature>";
                XMLValue = XMLValue + "<SortSequence>" + Convert.ToString(SortSequence) + "</SortSequence>";
                XMLValue = XMLValue + "<Status>";
                Int64 count = 1;
                Int64 ordercount = 1;
                if (objNotificationDTO.Status != null)
                {
                    foreach (string s in objNotificationDTO.Status.Split(','))
                    {
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            XMLValue = XMLValue + "<Status" + count + ">" + Convert.ToString(s) + "</Status" + count + ">";
                            count++;
                        }
                    }
                }
                XMLValue = XMLValue + "</Status>";
                XMLValue = XMLValue + "<WOStatus>";
                count = 1;
                if (objNotificationDTO.WOStatus != null)
                {
                    foreach (string s in objNotificationDTO.WOStatus.Split(','))
                    {
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            XMLValue = XMLValue + "<WOStatus" + count + ">" + Convert.ToString(s) + "</WOStatus" + count + ">";
                            count++;
                        }
                    }
                }
                XMLValue = XMLValue + "</WOStatus>";
                XMLValue = XMLValue + "<OrderStatus>";
                ordercount = 1;
                if (objNotificationDTO.OrderStatus != null)
                {
                    foreach (string s in objNotificationDTO.OrderStatus.Split(','))
                    {
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            XMLValue = XMLValue + "<OrderStatus" + ordercount + ">" + Convert.ToString(s) + "</OrderStatus" + ordercount + ">";
                            ordercount++;
                        }
                    }
                }
                XMLValue = XMLValue + "</OrderStatus>";
                XMLValue = XMLValue + "<OnlyExpiredItems>" + Convert.ToString(objNotificationDTO.OnlyExpiredItems ? "Yes" : string.Empty) + "</OnlyExpiredItems>";
                XMLValue = XMLValue + "<DaysUntilItemExpires>" + Convert.ToString(objNotificationDTO.DaysUntilItemExpires) + "</DaysUntilItemExpires>";
                XMLValue = XMLValue + "<DaysToApproveOrder>" + Convert.ToString(objNotificationDTO.DaysToApproveOrder) + "</DaysToApproveOrder>";
                if (objNotificationDTO.ProjectExpirationDate != null)
                {
                    objNotificationDTO.ProjectExpirationDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(objNotificationDTO.ProjectExpirationDate, (SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString("yyyy-MM-dd HH:mm:ss");
                    XMLValue = XMLValue + "<ProjectExpirationDate>" + Convert.ToString(objNotificationDTO.ProjectExpirationDate) + "</ProjectExpirationDate>";
                }
                else
                {
                    XMLValue = XMLValue + "<ProjectExpirationDate></ProjectExpirationDate>";
                }
                XMLValue = XMLValue + "<OnlyAvailableTools>" + Convert.ToString(objNotificationDTO.OnlyAvailableTools ? "Yes" : string.Empty) + "</OnlyAvailableTools>";
                XMLValue = XMLValue + "<QuantityType>";
                count = 1;
                if (objNotificationDTO.QtyType != null)
                {
                    foreach (string s in objNotificationDTO.QtyType.Split(','))
                    {
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            XMLValue = XMLValue + "<Type" + count + ">" + Convert.ToString(s) + "</Type" + count + ">";
                            count++;
                        }
                    }
                }
                XMLValue = XMLValue + "</QuantityType>";

                XMLValue = XMLValue + "<CartType>";
                count = 1;
                if (objNotificationDTO.CartType != null)
                {
                    foreach (string s in objNotificationDTO.CartType.Split(','))
                    {
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            XMLValue = XMLValue + "<CType" + count + ">" + Convert.ToString(s) + "</CType" + count + ">";
                            count++;
                        }
                    }
                }
                XMLValue = XMLValue + "</CartType>";

                XMLValue = XMLValue + "<IsIncludeStockouttool>" + Convert.ToString(objNotificationDTO.IsIncludeStockouttool ? "Yes" : string.Empty) + "</IsIncludeStockouttool>";

                XMLValue = XMLValue + "<MoveType>";
                count = 1;
                if (objNotificationDTO.MoveType != null)
                {
                    foreach (string s in objNotificationDTO.MoveType.Split(','))
                    {
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            XMLValue = XMLValue + "<CType" + count + ">" + Convert.ToString(s) + "</CType" + count + ">";
                            count++;
                        }
                    }
                }
                XMLValue = XMLValue + "</MoveType>";
            }
            //if (!string.IsNullOrWhiteSpace(XMLValue))
            //{

            //    XmlDocument xmldoc = new XmlDocument();
            //    xmldoc.LoadXml("<Data>"+XMLValue+"</Data>");
            //    XmlNodeList nodeList = xmldoc.SelectNodes("/Data/Status");
            //    string Short_Fall = string.Empty;
            //    foreach (XmlNode node in nodeList)
            //    {
            //        for (int i = 1; i <= node.ChildNodes.Count;i++)
            //        {
            //            Short_Fall = node["Status"+i].InnerText;
            //        }
            //    }
            //}
            if (!ModelState.IsValid)
            {
                message = string.Join("; ", ModelState.Values
                                            .SelectMany(x => x.Errors)
                                            .Select(x => x.ErrorMessage));
                status = "fail";
            }
            else if (objNotificationDTO.ScheduleFor == 6 && !string.IsNullOrEmpty(objNotificationDTO.AttachmentReportIDs) && 
                    !string.IsNullOrWhiteSpace(objNotificationDTO.AttachmentReportIDs) && string.IsNullOrEmpty(objNotificationDTO.AttachmentTypes))
            {
                message = ResMessage.AttachmentTypeIsMandatoryWithAttachmentReport;//"Please Select Attachment Report";
                status = "fail";
            }
            else
            {
                try
                {
                    string strOK = objNotificationDAL.IsDuplicateSchedule(objSchedulerDTO.ScheduleID, objSchedulerDTO.ScheduleName, SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (strOK == "duplicate")
                    {
                        message = string.Format(ResMessage.DuplicateMessage, ResSchedulerReportList.ScheduleName, objSchedulerDTO.ScheduleName); // "Room \"" + objDTO.RoomName + "\" already exist! Try with Another!";
                        status = "duplicate";
                    }
                    else
                    {

                        objNotificationDTO.RoomID = SessionHelper.RoomID;
                        objNotificationDTO.CompanyID = SessionHelper.CompanyID;
                        objNotificationDTO.CreatedBy = SessionHelper.UserID;
                        objNotificationDTO.UpdatedBy = SessionHelper.UserID;
                        objNotificationDTO.IsDeleted = false;
                        objNotificationDTO.SortSequence = SortSequence;
                        objNotificationDTO.XMLValue = XMLValue;
                        if (!string.IsNullOrWhiteSpace(objNotificationDTO.EmailTemplateDetails))
                        {
                            objNotificationDTO.EmailTemplateDetails = objNotificationDTO.EmailTemplateDetails;
                        }
                        if (objSchedulerDTO != null)
                        {
                            objSchedulerDTO.RoomId = SessionHelper.RoomID;
                            objSchedulerDTO.CompanyId = SessionHelper.CompanyID;
                            objSchedulerDTO.Created = DateTimeUtility.DateTimeNow;
                            objSchedulerDTO.Updated = DateTimeUtility.DateTimeNow;
                            objSchedulerDTO.CreatedBy = SessionHelper.UserID;
                            objSchedulerDTO.LastUpdatedBy = SessionHelper.UserID;

                            List<EmailTemplateDetailDTO> lstEmailDetisl = new List<EmailTemplateDetailDTO>();
                            if (!string.IsNullOrWhiteSpace(objSchedulerDTO.EmailTemplateDetails))
                            {
                                lstEmailDetisl = Newtonsoft.Json.JsonConvert.DeserializeObject<List<EmailTemplateDetailDTO>>(objSchedulerDTO.EmailTemplateDetails);
                                lstEmailDetisl.ForEach(t =>
                                {
                                    t.MailBodyText = HttpUtility.UrlDecode(t.MailBodyText);
                                });
                            }
                            objNotificationDTO.SchedulerParams = objSchedulerDTO;
                            objNotificationDTO.AttachmentReportIDs = objSchedulerDTO.AttachmentReportIDs;
                            objNotificationDTO.AttachmentTypes = objSchedulerDTO.AttachmentTypes;
                            objNotificationDTO.CompanyID = SessionHelper.CompanyID;
                            objNotificationDTO.Created = DateTimeUtility.DateTimeNow;
                            objNotificationDTO.CreatedBy = SessionHelper.UserID;
                            objNotificationDTO.lstEmailTemplateDtls = lstEmailDetisl;

                        }
                        if (!string.IsNullOrEmpty(objNotificationDTO.NotificationName))
                        {
                            string[] arrids = objNotificationDTO.NotificationName.Split('_');
                            objNotificationDTO.ScheduleFor = Convert.ToInt32(arrids[1]);
                            if (objNotificationDTO.ScheduleFor == 5)
                            {
                                objNotificationDTO.Reports = arrids[0];
                                objNotificationDTO.EmailTemplates = string.Empty;
                            }
                            if (objNotificationDTO.ScheduleFor == 6)
                            {
                                objNotificationDTO.Reports = string.Empty;
                                objNotificationDTO.EmailTemplates = arrids[0];
                            }
                        }
                        if (objNotificationDTO.ID == 0)
                        {
                            objNotificationDAL.SaveEmailScheduleSetup(objNotificationDTO);
                            message = ResMessage.SaveMessage;
                            status = "ok";
                        }
                        else
                        {
                            objNotificationDAL.UpdateEmailScheduleSetup(objNotificationDTO);
                            message = ResMessage.SaveMessage;
                            status = "ok";
                        }
                    }
                }
                catch (Exception ex)
                {
                    message = ResMessage.SaveErrorMsg + (Convert.ToString(ex.InnerException) ?? string.Empty);
                    status = "fail";
                }
            }
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetReportsandAlert()
        {
            EmailTemplateDAL objEmailTemplateDAL = new EmailTemplateDAL(SessionHelper.EnterPriseDBName);
            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            List<EmailTemplateDTO> lstEmailTemplates = objEmailTemplateDAL.GetAllEmailTemplateToNotify();
            List<ReportBuilderDTO> lstReportBuilderDTO = objReportMasterDAL.GetReportList(SessionHelper.UserID).Where(x => ((x.CompanyID == SessionHelper.CompanyID || x.CompanyID == 0) && x.IsDeleted.GetValueOrDefault(false) == false && x.IsArchived.GetValueOrDefault(false) == false && x.ISEnterpriseReport.GetValueOrDefault(false) == false) || (x.ISEnterpriseReport.GetValueOrDefault(false) == true && x.IsDeleted.GetValueOrDefault(false) == false && x.IsArchived.GetValueOrDefault(false) == false)).OrderBy(x => x.ReportName).ToList();
            lstReportBuilderDTO = lstReportBuilderDTO.Where(x => x.ModuleName != "EnterpriseList").OrderBy(x => x.ReportName).ToList();
            List<AlertReportDTO> lstAlertReports = new List<AlertReportDTO>();
            foreach (var item in lstEmailTemplates)
            {
                lstAlertReports.Add(new AlertReportDTO() { ID = item.ID + "_6", AlertReportName = item.TemplateName, ScheduleFor = 6 });
            }
            foreach (var item in lstReportBuilderDTO)
            {
                lstAlertReports.Add(new AlertReportDTO() { ID = item.ID + "_5", AlertReportName = item.ReportName, ScheduleFor = 5 });
            }
            lstAlertReports = lstAlertReports.OrderBy(t => t.AlertReportName).ToList();

            return PartialView("_NotificationList", lstAlertReports);

        }

        #endregion
    }
}
