using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Helper;
using eTurns.DTO.Resources;
using eTurnsMaster.DAL;
using eTurnsWeb.BAL;
using eTurnsWeb.Helper;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
//using System.Net.Mail;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace eTurnsWeb.Controllers
{
    [AuthorizeHelper]
    public partial class ReportBuilderController : eTurnsControllerBase
    {
        //
        // GET: /ReportBuilder/
        bool IsReportDelete = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Reports, eTurnsWeb.Helper.SessionHelper.PermissionType.Delete);
        eTurnsRegionInfo rsInfo = SessionHelper.eTurnsRegionInfoProp;
        eTurns.DAL.AlertMail amDAL = new eTurns.DAL.AlertMail();

        XNamespace ns = XNamespace.Get("http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition");
        XNamespace nsrd = XNamespace.Get("http://schemas.microsoft.com/SQLServer/reporting/reportdesigner");
        long ParentID = 0;
        string SubReportResFile = "";
        string strSubTablix = string.Empty;
        string rdlPath = string.Empty;
        bool IsRoomGridReportCommon = false;
        string RoomDBName = string.Empty;
        EnterpriseDTO objEntDTO = null;

        //XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));
        bool isRunWithReportConnection
        {
            get
            {
                string _strRunWithReportConnection = "No";
                //if (Settinfile.Element("RunWithReportConnection") != null)
                //{
                //    _strRunWithReportConnection = Convert.ToString(Settinfile.Element("RunWithReportConnection").Value);
                //}

                if (SiteSettingHelper.RunWithReportConnection != string.Empty)
                {
                    _strRunWithReportConnection = SiteSettingHelper.RunWithReportConnection;
                }

                if (!string.IsNullOrEmpty(_strRunWithReportConnection) && _strRunWithReportConnection.ToLower() == "yes" && ReportAppIntent == "ReadOnly")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        //isRunWithReportConnection



        public ActionResult CreateTemplate(long? id)
        {
            var isCustomizeReport = SessionHelper.GetAdminPermission(SessionHelper.ModuleList.CustomizeReport);

            if (isCustomizeReport)
            {
                ReportBuilderDTO objReportBuilderDTO = new ReportBuilderDTO();
                List<ReportBuilderDTO> lstReportBuilderDTO = GetReportList();
                if (SessionHelper.RoleID >= 0 && lstReportBuilderDTO != null)
                {
                    lstReportBuilderDTO = lstReportBuilderDTO.Where(x => x.ReportName != "EnterpriseRoom" && x.ReportName != "EnterpriseUser").OrderBy(x => x.ReportName).ToList();
                }
                lstReportBuilderDTO = lstReportBuilderDTO.Where(x => string.IsNullOrEmpty(x.CombineReportID)).ToList();
                ViewBag.ReportType = lstReportBuilderDTO;

                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                ReportBuilderDTO oReportBuilderDTO = null;
                ViewBag.ReportID = id.GetValueOrDefault(0);
                ViewBag.ParentID = 0;
                if (id.GetValueOrDefault(0) > 0)
                {
                    oReportBuilderDTO = objReportMasterDAL.GetParentReportMasterByReportID(id.GetValueOrDefault(0));
                    if (oReportBuilderDTO != null)
                    {
                        ViewBag.ParentID = oReportBuilderDTO.ID;
                    }
                    else
                    {
                        ViewBag.ParentID = id.GetValueOrDefault(0);
                    }
                }

                objReportBuilderDTO.IsPrivate = false;
                return View(objReportBuilderDTO);
            }
            else
            {
                return RedirectToAction("MyProfile", "Master");
            }

        }
        public ActionResult ViewReports()
        {
            var isViewReport = SessionHelper.GetAdminPermission(SessionHelper.ModuleList.ViewReport);

            if (isViewReport)
            {
                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                List<ReportBuilderDTO> objReportBuilderDTO = objReportMasterDAL.GetReportList(SessionHelper.UserID).Where(x => ((x.CompanyID == SessionHelper.CompanyID || x.CompanyID == 0) && x.IsDeleted.GetValueOrDefault(false) == false && x.IsArchived.GetValueOrDefault(false) == false && x.ISEnterpriseReport.GetValueOrDefault(false) == false) || (x.ISEnterpriseReport.GetValueOrDefault(false) == true && x.IsDeleted.GetValueOrDefault(false) == false && x.IsArchived.GetValueOrDefault(false) == false)).ToList();
                //List<ReportBuilderDTO> objReportBuilderDTO = objReportMasterDAL.GetReportList(SessionHelper.UserID).Where(x => ((x.CompanyID == SessionHelper.CompanyID || x.CompanyID == 0) && !x.IsDeleted.GetValueOrDefault(false) && !x.IsArchived.GetValueOrDefault(false))).ToList();
                objReportBuilderDTO = objReportBuilderDTO.Where(x => !x.IsDeleted.GetValueOrDefault(false) && !x.IsArchived.GetValueOrDefault(false) && x.ModuleName != "EnterpriseList" && x.ModuleName != "UsersList").OrderBy(x => x.ReportName).ToList();
                if (SessionHelper.RoleID >= 0 && objReportBuilderDTO != null)
                {
                    List<ReportBuilderDTO> lstParentID = objReportBuilderDTO.Where(x => x.ReportName == "EnterpriseRoom" || x.ReportName == "EnterpriseUser").OrderBy(x => x.ReportName).ToList();
                    objReportBuilderDTO = objReportBuilderDTO.Where(x => x.ReportName != "EnterpriseRoom" && x.ReportName != "EnterpriseUser").OrderBy(x => x.ReportName).ToList();
                    objReportBuilderDTO = objReportBuilderDTO.Where(x => lstParentID.Select(l => l.ID).ToList<long>().Contains(x.ParentID ?? 0) == false).ToList();
                }
                return View(objReportBuilderDTO);
            }
            else
            {
                return RedirectToAction("MyProfile", "Master");
            }
        }
        public ActionResult ReportSetting()
        {
            return View();
        }
        public ActionResult ScheduleReports()
        {
            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            List<ReportBuilderDTO> objReportBuilderDTO = objReportMasterDAL.GetReportList().Where(x => x.CompanyID == SessionHelper.CompanyID || x.CompanyID == 0).ToList();
            return View(objReportBuilderDTO);
        }
        public List<ReportBuilderDTO> GetReportList()
        {
            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            List<ReportBuilderDTO> objReportBuilderDTO = new List<ReportBuilderDTO>();
            objReportBuilderDTO = objReportMasterDAL.GetReportList(0, 0, SessionHelper.UserID);
            objReportBuilderDTO = objReportBuilderDTO.Where(x => x.ModuleName != "EnterpriseList" && x.ModuleName != "UsersList").OrderBy(x => x.ReportName).ToList();
            return objReportBuilderDTO;

        }
        public JsonResult GetChildReportList(long ParentId)
        {
            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            List<ReportBuilderDTO> objReportBuilderDTO = new List<ReportBuilderDTO>();
            objReportBuilderDTO = objReportMasterDAL.GetChildReport(ParentId, SessionHelper.CompanyID, SessionHelper.UserID);
            return Json(new { Status = true, Message = "ok", ListChildReport = objReportBuilderDTO }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetReportDetailByID(string ID)
        {
            int _currentNotificationCount = 0;
            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            ReportBuilderDTO objReportBuilderDTO = new ReportBuilderDTO();
            objReportBuilderDTO = objReportMasterDAL.GetReportDetail(Convert.ToInt64(ID));

            if (objReportBuilderDTO.ISEnterpriseReport.GetValueOrDefault(false) == true)
            {
                List<NotificationDTO> listNotification = new NotificationDAL(SessionHelper.EnterPriseDBName).GetNotificationByReportID(SessionHelper.CompanyID, SessionHelper.RoomID, Convert.ToInt64(ID));

                if (listNotification != null && listNotification.Count > 0)
                {
                    _currentNotificationCount = listNotification.Count;
                }
            }
            //return Json(new { Status = true, Message = "ok", ReportName = objReportBuilderDTO.ReportFileName, SubReportName = objReportBuilderDTO.SubReportFileName, ReportType = objReportBuilderDTO.ReportType, Isprivate = objReportBuilderDTO.IsPrivate, IsBaseReport = objReportBuilderDTO.IsBaseReport, MasterReportResFile = objReportBuilderDTO.MasterReportResFile, SubReportresFile = objReportBuilderDTO.SubReportResFile, IsIncludeTotal = objReportBuilderDTO.IsIncludeTotal, IsIncludeGrandTotal = objReportBuilderDTO.IsIncludeGrandTotal }, JsonRequestBehavior.AllowGet);
            return Json(new { Status = true, Message = "ok", ReportBuilderDTO = objReportBuilderDTO, CurrentNotificationCount = _currentNotificationCount }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetScheduleNamebyReportID(Int64 ReportID)
        {
            string strScheduleName = string.Empty;
            if (ReportID > 0)
            {
                ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                strScheduleName = objDAL.GetScheduleNamebyReportID(ReportID, SessionHelper.RoomID, SessionHelper.CompanyID);
                return Json(new { Message = ResReportMaster.ReportSchedule, Status = true, ScheduleName = strScheduleName }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Message = ResReportMaster.ReportSchedule, Status = false, ScheduleName = strScheduleName }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteReportByID(long ReportID)
        {
            if (ReportID > 0)
            {
                ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                objDAL.DeleteReport(ReportID, SessionHelper.UserID);

                #region For Default Print option as par module WI-4440

                objDAL.UpdateDefaultReportAfterReportDeleted(ReportID, null, null);

                #endregion

                return Json(new { Message = string.Format(ResCommon.MsgDeletedSuccessfully, ResLayout.Report), Status = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Message = ResReportMaster.NoProperDataToDelete, Status = false }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteReportByIDs(string ReportIDs)
        {
            if (!string.IsNullOrWhiteSpace(ReportIDs))
            {
                ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                List<string> lstReportIDs = ReportIDs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (lstReportIDs != null && lstReportIDs.Count > 0)
                {
                    foreach (var ReportID in lstReportIDs)
                    {
                        long repid = 0;
                        if (long.TryParse(ReportID, out repid))
                        {
                            objDAL.DeleteReport(repid, SessionHelper.UserID);
                        }

                    }
                }

                return Json(new { Message = string.Format(ResCommon.MsgDeletedSuccessfully, ResourceHelper.GetReportModuleResource("Report")), Status = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Message = ResReportMaster.NoProperDataToDelete, Status = false }, JsonRequestBehavior.AllowGet);
        }

        public string GetFullPath(string path)
        {
            string rdlPath = string.Empty;
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);

            }
            rdlPath = path;

            return rdlPath;
        }
        private List<CommonDTO> GetSheduleItemTypeOptions()
        {
            List<CommonDTO> ItemType = new List<CommonDTO>();
            ItemType.Add(new CommonDTO() { PageName = "ItemMaster.rdlc", Text = "Item" });
            ItemType.Add(new CommonDTO() { PageName = "RPT_Orders.rdlc", Text = "Order" });


            return ItemType;
        }
        public JsonResult GetReportDetail(string ReportName, string IsPrivate, string ResourceFile, long ReportId)
        {
            bool hasTotalField = true;
            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            ReportBuilderDTO objReportBuilderDTO = new ReportBuilderDTO();
            objReportBuilderDTO = objReportMasterDAL.GetReportDetail(ReportId);

            #region WI-6317 Please add the PullID to the Audit Trail reports

            string ParentReportName = ReportName;
            Int64 ParentReportID = ReportId;

            ReportBuilderDTO oReportBuilderParentDTO = objReportMasterDAL.GetParentReportMasterByReportID(ReportId);
            if (oReportBuilderParentDTO != null)
            {
                ParentReportName = oReportBuilderParentDTO.ReportName;
                ParentReportID = oReportBuilderParentDTO.ID;
            }

            List<string> IgnoreColumnList = new List<string>();
            if (ParentReportName.Equals("Audit Trail"))
            {
                IgnoreColumnList = objReportMasterDAL.GetReportIgnoreColumnListByReportName(ParentReportName);
            }
            else if (ParentReportName == "Requisition")
            {
                IgnoreColumnList.Add("ItemImageExternalURL");
                IgnoreColumnList.Add("ImageType");
            }

            #endregion

            string rdlPath = string.Empty;
            string RDLCBaseFilePath = CommonUtility.RDLCBaseFilePath;
            if (objReportBuilderDTO.ParentID > 0)
            {
                if (objReportBuilderDTO.ISEnterpriseReport.GetValueOrDefault(false))
                {
                    rdlPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/EnterpriseReport" + @"\\" + ReportName;
                }
                else
                {
                    rdlPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID + @"\\" + ReportName;
                }
            }
            else
            {
                rdlPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/BaseReport" + @"\\" + ReportName;
            }

            XDocument doc = XDocument.Load(rdlPath);
            string connString = doc.Descendants(ns + "ConnectString").FirstOrDefault().Value;

            string ReportHeight = string.Empty;
            string ReportWidth = string.Empty;
            string ReportPageSetUpType = string.Empty;

            //ReportWidth = doc.Descendants(ns + "PageWidth").First().Value;
            //if (doc.Descendants(ns + "PageHeight") != null && doc.Descendants(ns + "PageHeight").Count() > 0)
            //{
            //    ReportHeight = doc.Descendants(ns + "PageHeight").First().Value;
            //}


            if (doc.Descendants(ns + "PageHeight").Count() > 0)
                ReportHeight = doc.Descendants(ns + "PageHeight").First().Value;
            else
            {
                if (double.Parse(doc.Root.Element(ns + "Width").Value.Replace("in", "")) > 9)
                {
                    doc.Descendants(ns + "Page").FirstOrDefault().Add(new XElement(ns + "PageHeight", "8.5in"));
                    ReportHeight = "8.5in";
                }
                else
                {
                    doc.Descendants(ns + "Page").FirstOrDefault().Add(new XElement(ns + "PageHeight", "11in"));
                    ReportHeight = "11in";
                }
            }

            if (doc.Descendants(ns + "PageWidth").Count() > 0)
                ReportWidth = doc.Descendants(ns + "PageWidth").First().Value;
            else
            {
                if (double.Parse(doc.Root.Element(ns + "Width").Value.Replace("in", "")) > 9)
                {
                    doc.Descendants(ns + "Page").FirstOrDefault().Add(new XElement(ns + "PageWidth", "11in"));
                    ReportWidth = "11in";
                }
                else
                {
                    doc.Descendants(ns + "Page").FirstOrDefault().Add(new XElement(ns + "PageWidth", "8.5in"));
                    ReportWidth = "8.5in";
                }
            }

            ReportPageSetUpType = GetReportPageSetUp(ReportHeight.ToLower().Replace("in", ""), ReportWidth.ToLower().Replace("in", ""));



            List<XElement> lstDSFields = doc.Descendants(ns + "Field").ToList();
            if (lstDSFields != null && lstDSFields.Count > 0)
            {
                //lstDSFields = lstDSFields.OrderBy(e => e.Attribute("Name").Value).ToList();
                if (lstDSFields.Descendants(ns + "DataField").FirstOrDefault(x => x.Value == "Total") == null)
                {
                    hasTotalField = false;
                }

                if (ParentReportName.Equals("Audit Trail"))
                {
                    List<XElement> IEnuDSFields = lstDSFields;
                    if (IgnoreColumnList.Count > 0)
                    {
                        foreach (string ColumName in IgnoreColumnList)
                        {
                            if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                            {
                                IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains(ColumName))).ToList();
                            }
                        }
                        if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                        {
                            lstDSFields.Remove();
                            lstDSFields = IEnuDSFields.OrderBy(e => e.Attribute("Name").Value).ToList();
                        }
                    }
                }
                else if (ParentReportName == "Requisition")
                {
                    List<XElement> IEnuDSFields = lstDSFields;
                    if (IgnoreColumnList.Count > 0)
                    {
                        foreach (string ColumName in IgnoreColumnList)
                        {
                            if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                            {
                                IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains(ColumName))).ToList();
                            }
                        }
                        if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                        {
                            lstDSFields.Remove();
                            lstDSFields = IEnuDSFields.OrderBy(e => e.Attribute("Name").Value).ToList();
                        }
                    }
                }
                else
                {
                    List<XElement> IEnuDSFields = IEnuDSFields = lstDSFields.Where(x => !(x.Attribute("Name").Value.Contains("Total"))).ToList();

                    if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                        IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("CostDecimalPoint"))).ToList();
                    if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                        IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("QuantityDecimalPoint"))).ToList();
                    if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                        IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("RoomInfo"))).ToList();

                    if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                        IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("CompanyInfo"))).ToList();
                    if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                        IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("CurrentDateTime"))).ToList();
                    if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                        IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("ID")) || x.Attribute("Name").Value.Trim().ToLower() == "uniqueid").ToList();
                    if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                        IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("Guid"))).ToList();
                    if (ResourceFile.ToLower().Trim() != "resreportroom")
                    {
                        if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                            IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("Tax1Rate"))).ToList();

                        if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                            IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("Tax2Rate"))).ToList();

                        if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                            IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("Tax1Name"))).ToList();

                        if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                            IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("Tax2Name"))).ToList();


                        if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                            IEnuDSFields = IEnuDSFields.Where(x => x.Descendants(nsrd + "TypeName").FirstOrDefault() != null && !(x.Descendants(nsrd + "TypeName").FirstOrDefault().Value.Contains("System.Int64") && !(x.Attribute("Name").Value.Contains("RowNumber")))).ToList();
                    }
                    if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                        IEnuDSFields = IEnuDSFields.Where(x => x.Descendants(nsrd + "TypeName").FirstOrDefault() != null && !(x.Descendants(nsrd + "TypeName").FirstOrDefault().Value.Contains("System.Guid"))).ToList();

                    if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                    {
                        lstDSFields.Remove();
                        lstDSFields = IEnuDSFields.OrderBy(e => e.Attribute("Name").Value).ToList();
                    }
                }
            }
            List<string> arrFields = new List<string>();
            string strFields = string.Empty;
            List<object> NgFields = new List<object>();
            List<XElement> lstColumns = doc.Descendants(ns + "TablixColumn").ToList();
            List<XElement> lstReportField = doc.Descendants(ns + "TablixRow").ToList();
            double sumTotalColumnsWidth = lstColumns.Select(x => x.Value.Replace("in", "")).Sum(x => double.Parse(x, System.Globalization.NumberStyles.AllowDecimalPoint));
            string strHeaderFields = string.Empty;
            List<object> NgHeaderFields = new List<object>();
            if (lstReportField != null && lstReportField.Count > 0)
            {

                List<XElement> lstFieldTop = lstReportField[0].Descendants(ns + "TablixCell").ToList();
                List<XElement> lstFieldBottom = lstReportField[1].Descendants(ns + "TablixCell").ToList();
                int cellcount = lstFieldBottom.Count;
                if (objReportBuilderDTO.IsIncludeTotal == true)
                {
                    cellcount = cellcount - 1;
                }
                string tablixcelltop = string.Empty;
                string tablixcellBottom = string.Empty;

                for (int i = 0; i <= cellcount - 1; i++)
                {
                    string strcolumnwidth = lstColumns[i].Value;

                    string fontstyle = string.Empty;
                    string fontweight = string.Empty;
                    string Textalign = string.Empty;

                    if (lstFieldTop[i].Descendants(ns + "FontStyle").Any())
                    {
                        fontstyle = lstFieldTop[i].Descendants(ns + "FontStyle").FirstOrDefault().Value;
                    }
                    if (lstFieldTop[i].Descendants(ns + "FontWeight").Any())
                    {
                        fontweight = lstFieldTop[i].Descendants(ns + "FontWeight").FirstOrDefault().Value;
                    }
                    if (lstFieldTop[i].Descendants(ns + "TextAlign").Any())
                    {
                        Textalign = lstFieldTop[i].Descendants(ns + "TextAlign").FirstOrDefault().Value;
                    }

                    if (lstFieldTop[i].Descendants(ns + "Value").FirstOrDefault() != null
                        && !(string.IsNullOrEmpty(lstFieldTop[i].Descendants(ns + "Value").FirstOrDefault().Value.TrimEnd())))
                    {
                        tablixcelltop = Convert.ToString(lstFieldTop[i].Descendants(ns + "Value").FirstOrDefault().Value);
                        tablixcellBottom = Convert.ToString(lstFieldBottom[i].Descendants(ns + "Value").FirstOrDefault().Value).Replace("=Fields!", "").Replace("=Parameters!BarcodeURL.Value+Fields!", "").Replace(".Value", "");
                        strHeaderFields += "<td id='tddropline_" + i + "' " + GetTDStyleWithWidth(Textalign, ConvertInchToPx(strcolumnwidth)) + "><div class='divLineDrag'>&nbsp;</div><span class='ChangeText' " + GetSpanStyle(fontstyle, fontweight) + " >" + GetField(tablixcelltop, ResourceFile) + "</span><input type='hidden' value='" + tablixcellBottom.Replace("=Parameters!BarcodeURL.Value+Fields!", "").Replace(".Value", "") + "' id='hdnlineitem_" + (i + 1) + "' /><img id='imgdelete' style='float: right;' alt='Remove' src='../../Content/images/deletereport_icon.png'  onclick='RemovelineItem(this)'></img></td>";

                        NgHeaderFields.Add(new { Index = i, StyleTextAlign = Textalign, StyleWidth = ConvertInchToPx(strcolumnwidth), TDStyleWithWidth = GetTDStyleWithWidth(Textalign, ConvertInchToPx(strcolumnwidth)), FontStyle = fontstyle, FontWeight = fontweight, SpanStyle = GetSpanStyle(fontstyle, fontweight), Field = GetField(tablixcelltop, ResourceFile), CellBottom = tablixcellBottom.Replace("=Parameters!BarcodeURL.Value+Fields!", "").Replace(".Value", "") });
                    }
                    else
                    {
                        strHeaderFields += "<td id='tddropline_" + i + "' " + GetTDStyleWithWidth(Textalign, ConvertInchToPx(strcolumnwidth)) + "><div class='divLineDrag'>&nbsp;</div><span class='ChangeText' " + GetSpanStyle(fontstyle, fontweight) + " >" + "" + "</span><input type='hidden' value='" + "" + "' id='hdnlineitem_" + (i + 1) + "' /><img id='imgdelete' style='float: right;' alt='Remove' src='../../Content/images/deletereport_icon.png'  onclick='RemoveBlanklineItem(this)'></img></td>";

                        NgHeaderFields.Add(new { Index = i, TDStyleWithWidth = GetTDStyleWithWidth(Textalign, ConvertInchToPx(strcolumnwidth)), SpanStyle = GetSpanStyle(fontstyle, fontweight), Field = "", CellBottom = "" });
                    }
                }

                if (lstDSFields != null && lstDSFields.Count > 0)
                {
                    lstDSFields = lstDSFields.OrderBy(x => GetField(x.Descendants(ns + "DataField").FirstOrDefault().Value, ResourceFile)).ToList();
                    //if (lstDSFields.Descendants(ns + "DataField").FirstOrDefault(x => x.Value == "Total") == null)
                    //{
                    //    hasTotalField = false;
                    //}

                    //foreach (XElement item in lstDSFields)
                    //{
                    //    bool exist = false;
                    //    string strright = (item.Descendants(ns + "DataField").FirstOrDefault().Value);
                    //    foreach (XElement itemhead in lstFieldBottom)
                    //    {
                    //        if (itemhead.Descendants(ns + "Value").FirstOrDefault() != null
                    //            && !(string.IsNullOrEmpty(itemhead.Descendants(ns + "Value").FirstOrDefault().Value.TrimEnd()))
                    //            )
                    //        {
                    //            string strleft = (itemhead.Descendants(ns + "Value").FirstOrDefault().Value.Replace("=Fields!", "").Replace(".Value", ""));


                    //            if (!string.IsNullOrEmpty(strleft.Trim()) && !string.IsNullOrEmpty(strright.Trim()) && (strleft).Trim().ToLower() == (strright).Trim().ToLower())
                    //            {
                    //                exist = true;
                    //            }
                    //            else if ((itemhead.Descendants(ns + "Value").FirstOrDefault().Value.Replace("=Parameters!BarcodeURL.Value+Fields!", "").Replace(".Value", "")) == (item.Descendants(ns + "DataField").FirstOrDefault().Value))
                    //            {
                    //                exist = true;
                    //            }
                    //        }
                    //        if (exist == false)
                    //        {
                    //            strFields += "<tr id='trdrag_Bottom'><td class='TdDragLineItem'><input type='hidden' value='" + item.Descendants(ns + "DataField").FirstOrDefault().Value + "' id='hdnlineitem' />" + GetField(item.Descendants(ns + "DataField").FirstOrDefault().Value, ResourceFile) + "</td></tr>";
                    //        }
                    //    }
                    //}



                    foreach (XElement item in lstDSFields)
                    {
                        bool exist = false;
                        string strright = (item.Descendants(ns + "DataField").FirstOrDefault().Value);
                        foreach (XElement itemhead in lstFieldBottom)
                        {
                            string strleft = (itemhead.Descendants(ns + "Value").FirstOrDefault().Value.Replace("=Fields!", "").Replace(".Value", ""));


                            if (!string.IsNullOrEmpty(strleft.Trim()) && !string.IsNullOrEmpty(strright.Trim()) && (strleft).Trim().ToLower() == (strright).Trim().ToLower())
                            {
                                exist = true;
                            }
                            else if ((itemhead.Descendants(ns + "Value").FirstOrDefault().Value.Replace("=Parameters!BarcodeURL.Value+Fields!", "").Replace(".Value", "")) == (item.Descendants(ns + "DataField").FirstOrDefault().Value))
                            {
                                exist = true;
                            }
                        }
                        if (exist == false)
                        {
                            strFields += "<tr id='trdrag_Bottom'><td class='TdDragLineItem'><input type='hidden' value='" + item.Descendants(ns + "DataField").FirstOrDefault().Value + "' id='hdnlineitem' />" + GetField(item.Descendants(ns + "DataField").FirstOrDefault().Value, ResourceFile) + "</td></tr>";
                            NgFields.Add(new { DataField = item.Descendants(ns + "DataField").FirstOrDefault().Value, DataFieldDisplay = GetField(item.Descendants(ns + "DataField").FirstOrDefault().Value, ResourceFile) });
                        }
                    }
                }


            }
            return Json(new { Status = true, Message = "ok", FieldLIs = strFields, NgFieldLIs = NgFields, ReportField = strHeaderFields, NgReportField = NgHeaderFields, ReportPageSetUpType = ReportPageSetUpType, HasTotal = hasTotalField, ReportTableWidth = sumTotalColumnsWidth }, JsonRequestBehavior.AllowGet);
        }
        public string GetReportPageSetUp(string Rheight, string Rwidth)
        {
            string ReportPageSetUp = "1";
            double Height = 0;
            double Width = 0;
            try
            {
                if (!string.IsNullOrWhiteSpace(Rheight) && !string.IsNullOrWhiteSpace(Rwidth))
                {
                    Height = double.Parse(Rheight);
                    Width = double.Parse(Rwidth);
                    if (Width > Height)
                    {
                        ReportPageSetUp = "2";
                    }
                }
            }
            catch (Exception)
            {

            }

            return ReportPageSetUp;
        }
        public string GetSpanStyle(string fontstyle, string fontweight)
        {
            string retStyle = string.Empty;
            retStyle = "style='";
            if (!string.IsNullOrEmpty(fontstyle))
            {
                retStyle += "font-style:" + fontstyle + "; ";
            }
            if (!string.IsNullOrEmpty(fontweight))
            {
                retStyle += "font-weight:" + fontweight + "; ";
            }
            retStyle += "'";
            return retStyle;
        }
        public string GetTDStyleWithWidth(string TextAlign, string width, bool IsVisible = true)
        {
            string retStyle = string.Empty;
            retStyle = "style='";
            if (!string.IsNullOrEmpty(TextAlign))
            {
                retStyle += "text-align:" + TextAlign + "; ";
            }
            if (!string.IsNullOrEmpty(width))
            {
                retStyle += "width:" + width + "; ";

            }
            if (!IsVisible)
            {
                retStyle += "display:none";
            }
            retStyle += "'";
            return retStyle;
        }

        public string GetTDStyle(string TextAlign)
        {
            string retStyle = string.Empty;
            retStyle = "style='";
            if (!string.IsNullOrEmpty(TextAlign))
            {
                retStyle += "text-align:" + TextAlign + ";";
            }

            retStyle += "'";
            return retStyle;
        }
        public string GetTDStyleForHeader(string TextAlign, string TDHeight)
        {
            string retStyle = string.Empty;
            retStyle = "style='";
            if (!string.IsNullOrEmpty(TDHeight))
            {
                retStyle += "height:" + TDHeight + ";";
            }
            if (!string.IsNullOrEmpty(TextAlign))
            {
                retStyle += "text-align:" + TextAlign + ";";
            }

            retStyle += "'";
            return retStyle;
        }
        public JsonResult GetVerticalReportDetail(string ReportName, string IsPrivate, string ResourceFile, long ReportId)
        {
            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            ReportBuilderDTO objReportBuilderDTO = new ReportBuilderDTO();
            objReportBuilderDTO = objReportMasterDAL.GetReportDetail(ReportId);

            string rdlPath = string.Empty;
            string RDLCBaseFilePath = CommonUtility.RDLCBaseFilePath;
            if (objReportBuilderDTO.ParentID > 0)
            {
                if (objReportBuilderDTO.ISEnterpriseReport.GetValueOrDefault(false))
                {
                    rdlPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/EnterpriseReport" + @"\\" + ReportName;
                }
                else
                {
                    rdlPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID + @"\\" + ReportName;
                }
            }
            else
            {
                rdlPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/BaseReport" + @"\\" + ReportName;
            }

            #region WI-7309 Add Requisition ID # field when customizing the Requisition report (screenshots)

            string ParentReportName = ReportName;
            Int64 ParentReportID = ReportId;

            ReportBuilderDTO oReportBuilderParentDTO = objReportMasterDAL.GetParentReportMasterByReportID(ReportId);
            if (oReportBuilderParentDTO != null)
            {
                ParentReportName = oReportBuilderParentDTO.ReportName;
                ParentReportID = oReportBuilderParentDTO.ID;
            }
            #endregion
            XDocument doc = XDocument.Load(rdlPath);
            XNamespace ns = XNamespace.Get("http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition");
            XNamespace nsrd = XNamespace.Get("http://schemas.microsoft.com/SQLServer/reporting/reportdesigner");
            string connString = doc.Descendants(ns + "ConnectString").FirstOrDefault().Value;
            List<XElement> lstMaterFields = doc.Descendants(ns + "Field").ToList();
            if (lstMaterFields != null && lstMaterFields.Count > 0)
            {
                //lstMaterFields = lstMaterFields.OrderBy(e => e.Attribute("Name").Value).ToList();
                IEnumerable<XElement> IEnuDSFields = lstMaterFields.Where(x => !(x.Attribute("Name").Value.Contains("Total")));

                #region New Code to remove CostDecimalPoint,QuantityDecimalPoint,RoomInfo,ComapnyInfo,Int64,Guid

                if (IEnuDSFields != null)
                    IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("CostDecimalPoint"))).ToList();
                if (IEnuDSFields != null)
                    IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("QuantityDecimalPoint"))).ToList();
                if (IEnuDSFields != null)
                    IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("RoomInfo"))).ToList();
                if (IEnuDSFields != null)
                    IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("CompanyInfo"))).ToList();
                if (IEnuDSFields != null)
                    IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("CurrentDateTime"))).ToList();
                if (IEnuDSFields != null)
                {
                    if (ParentReportName == "Requisition")
                    {
                        IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value == "ID")).ToList();
                    }
                    else
                    {
                        IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("ID"))).ToList();
                    }
                }
                if (IEnuDSFields != null)
                    IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("Guid"))).ToList();
                if (IEnuDSFields != null)
                    IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("Tax1Rate"))).ToList();
                if (IEnuDSFields != null)
                    IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("Tax2Rate"))).ToList();
                if (IEnuDSFields != null)
                    IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("Tax1Name"))).ToList();
                if (IEnuDSFields != null)
                    IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("Tax2Name"))).ToList();

                #endregion
                if (ParentReportName == "Requisition")
                {
                    IEnuDSFields = IEnuDSFields.Where(x => (!(x.Descendants(nsrd + "TypeName").FirstOrDefault().Value.Contains("System.Int64") && !(x.Attribute("Name").Value.Contains("RowNumber")))) || (x.Attribute("Name").Value.Contains("RequisitionID")));
                }
                else
                {
                    IEnuDSFields = IEnuDSFields.Where(x => !(x.Descendants(nsrd + "TypeName").FirstOrDefault().Value.Contains("System.Int64") && !(x.Attribute("Name").Value.Contains("RowNumber"))));
                }
                IEnuDSFields = IEnuDSFields.Where(x => !(x.Descendants(nsrd + "TypeName").FirstOrDefault().Value.Contains("System.Guid")));
                lstMaterFields.Remove();
                lstMaterFields = IEnuDSFields.OrderBy(e => e.Attribute("Name").Value).ToList();
            }
            //
            lstMaterFields = lstMaterFields.OrderBy(x => x.Value).ToList();
            string ReportHeight = string.Empty;
            string ReportWidth = string.Empty;
            string ReportPageSetUpType = string.Empty;

            if (doc.Descendants(ns + "PageHeight").Count() > 0)
                ReportHeight = doc.Descendants(ns + "PageHeight").First().Value;
            else
            {
                if (double.Parse(doc.Root.Element(ns + "Width").Value.Replace("in", "")) > 9)
                {
                    doc.Descendants(ns + "Page").FirstOrDefault().Add(new XElement(ns + "PageHeight", "8.5in"));
                    ReportHeight = "8.5in";
                }
                else
                {
                    doc.Descendants(ns + "Page").FirstOrDefault().Add(new XElement(ns + "PageHeight", "11in"));
                    ReportHeight = "11in";
                }
            }

            if (doc.Descendants(ns + "PageWidth").Count() > 0)
                ReportWidth = doc.Descendants(ns + "PageWidth").First().Value;
            else
            {
                if (double.Parse(doc.Root.Element(ns + "Width").Value.Replace("in", "")) > 9)
                {
                    doc.Descendants(ns + "Page").FirstOrDefault().Add(new XElement(ns + "PageWidth", "11in"));
                    ReportWidth = "11in";
                }
                else
                {
                    doc.Descendants(ns + "Page").FirstOrDefault().Add(new XElement(ns + "PageWidth", "8.5in"));
                    ReportWidth = "8.5in";
                }
            }

            ReportPageSetUpType = GetReportPageSetUp(ReportHeight.ToLower().Replace("in", ""), ReportWidth.ToLower().Replace("in", ""));


            List<string> arrFields = new List<string>();
            string strFields = string.Empty;

            lstMaterFields = lstMaterFields.OrderBy(x => GetField(x.Descendants(ns + "DataField").FirstOrDefault().Value, ResourceFile)).ToList();

            List<XElement> lstExsistingReportField = new List<XElement>();
            List<XElement> lstReportField = doc.Descendants(ns + "TablixRow").ToList();
            foreach (XElement ReportField in lstReportField)
            {
                lstExsistingReportField.AddRange(ReportField.Descendants(ns + "TablixCell").ToList());
            }
            foreach (XElement item in lstMaterFields)
            {
                bool exist = false;
                string strright = (item.Descendants(ns + "DataField").FirstOrDefault().Value);
                foreach (XElement itemhead in lstExsistingReportField)
                {
                    if (itemhead.Descendants(ns + "Value").FirstOrDefault() != null)
                    {
                        string strleft = (itemhead.Descendants(ns + "Value").FirstOrDefault().Value.Replace("=Fields!", "").Replace(".Value", ""));

                        if (!string.IsNullOrEmpty(strleft.Trim()) && !string.IsNullOrEmpty(strright.Trim()) && (strleft).Trim().ToLower() == (strright).Trim().ToLower())
                        {
                            exist = true;
                        }
                        else if ((itemhead.Descendants(ns + "Value").FirstOrDefault().Value.Replace("=Parameters!BarcodeURL.Value+Fields!", "").Replace(".Value", "")) == (item.Descendants(ns + "DataField").FirstOrDefault().Value))
                        {
                            exist = true;
                        }
                    }
                }
                if (exist == false)
                {
                    strFields += "<tr class='dragtr'><td><span id='trdrag_Header' class='DragThis'>" + GetField(item.Descendants(ns + "DataField").FirstOrDefault().Value, ResourceFile) + "</span><input type='hidden' id='hdnmasterfield' value=" + item.Descendants(ns + "DataField").FirstOrDefault().Value + " /></td></tr>";
                }

                //strFields += "<tr class='dragtr'><td><span id='trdrag_Header' class='DragThis'>" + GetField(item.Descendants(ns + "DataField").FirstOrDefault().Value, ResourceFile) + "</span><input type='hidden' id='hdnmasterfield' value=" + item.Descendants(ns + "DataField").FirstOrDefault().Value + " /></td></tr>";
            }

            List<XElement> lstTablixRow = doc.Descendants(ns + "TablixRow").ToList();
            List<XElement> lstTablixRowGrouping = doc.Descendants(ns + "Tablix").Descendants(ns + "TablixRowHierarchy").ToList();
            string strHeaderLeftFields = string.Empty;
            // string strHeaderLeftFieldsedit = string.Empty; 
            string strHeaderRightFields = string.Empty;
            //string strHeaderRightFieldsedit = string.Empty; ;
            string strSubReportsParameter = string.Empty;
            string TablixRows = string.Empty;
            string RowGroup = string.Empty;
            bool IsRoomDetail = false;
            if (lstTablixRow != null && lstTablixRow.Count > 0)
            {
                int cnt = 0;
                int cnttr = 0;
                foreach (XElement tr in lstTablixRow)
                {
                    cnttr += 1;
                    List<XElement> lstTablixCell = new List<XElement>();
                    string TablixRow = string.Empty;
                    lstTablixCell = tr.Descendants(ns + "TablixCell").ToList();

                    if (lstTablixCell != null && lstTablixCell.Count > 0)
                    {
                        if ((lstTablixCell[0].Descendants(ns + "Textbox").Any()) && (lstTablixCell[0].Descendants(ns + "Textbox").Last().FirstAttribute.Value == "TextboxRoom"))
                        {
                            IsRoomDetail = true;
                            continue;
                        }


                        string tdval = string.Empty;

                        TablixRow = "<tr id='trdrop_" + cnttr + "' class='connectedSortable'>";
                        foreach (XElement td in lstTablixCell)
                        {
                            cnt += 1;
                            string TablixCell = string.Empty;
                            string TDHeight = ConvertInchToPx(tr.Descendants(ns + "Height").FirstOrDefault().Value);
                            if (td.Descendants(ns + "TextRun").Any())
                            {
                                string fontstyle = string.Empty;
                                string fontweight = string.Empty;
                                string Textalign = string.Empty;

                                if (td.Descendants(ns + "FontStyle").Any())
                                {
                                    fontstyle = td.Descendants(ns + "FontStyle").FirstOrDefault().Value;
                                }
                                if (td.Descendants(ns + "FontWeight").Any())
                                {
                                    fontweight = td.Descendants(ns + "FontWeight").FirstOrDefault().Value;
                                }
                                if (td.Descendants(ns + "TextAlign").Any())
                                {
                                    Textalign = td.Descendants(ns + "TextAlign").FirstOrDefault().Value;
                                }


                                tdval = Convert.ToString(td.Descendants(ns + "Value").FirstOrDefault().Value);

                                if (tdval.Contains("=Fields!") && tdval.Contains(".Value"))
                                {
                                    //strHeaderLeftFields += "<li class='ui-state-default' style=''>" + tdval.Replace("=Fields!", "").Replace(".Value", "") + "<img id='imgdelete' alt='Remove' src='../../Content/images/delete.png' style='display: block; float: right; margin-top: -2px; vertical-align: middle;' onclick='Removeli(this)'></li>";                                   
                                    //strHeaderRightFields += "<li class='ui-state-default' style=''>" + tdval.Replace("=Fields!", "").Replace(".Value", "") + "<img id='imgdelete' alt='Remove' src='../../Content/images/delete.png' style='display: block; float: right; margin-top: -2px; vertical-align: middle;' onclick='Removeli(this)'></li>";

                                    TablixCell += "<td id='tddrop_" + cnttr + "_" + cnt + "' class='notSelected' " + GetTDStyleForHeader(Textalign, TDHeight) + "><input type='hidden' id='hdn' value='" + tdval.Replace("=Fields!", "").Replace(".Value", "") + "' /><span class='NotChangeText' " + GetSpanStyle(fontstyle, fontweight) + ">" + GetField(tdval.Replace("=Fields!", "").Replace(".Value", ""), ResourceFile) + "</span><img id='imgdelete' alt='Remove' src='../../Content/images/deletereport_icon.png'  onclick='Removeli(this)'></img></td>";


                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(tdval))
                                    {
                                        TablixCell += "<td id='tddrop_" + cnttr + "_" + cnt + "' class='notSelected' " + GetTDStyleForHeader(Textalign, TDHeight) + ">&nbsp;<input type='hidden' id='hdn' value='" + tdval.Replace("=Fields!", "").Replace(".Value", "") + "' /><span class='ChangeText'  " + GetSpanStyle(fontstyle, fontweight) + ">" + GetField(tdval.Replace("=Fields!", "").Replace(".Value", ""), ResourceFile) + "</span></td>";
                                    }
                                    else
                                    {
                                        TablixCell += "<td id='tddrop_" + cnttr + "_" + cnt + "' class='notSelected' " + GetTDStyleForHeader(Textalign, TDHeight) + "><input type='hidden' id='hdn' value='" + tdval.Replace("=Fields!", "").Replace(".Value", "") + "' /><span class='ChangeText' " + GetSpanStyle(fontstyle, fontweight) + ">" + GetField(tdval.Replace("=Fields!", "").Replace(".Value", ""), ResourceFile) + "</span></td>";
                                    }
                                }


                            }
                            else if (td.Descendants(ns + "Subreport").Any())
                            {
                                TablixRow = string.Empty;

                                tdval = td.ToString().Replace("<", "&lt;").Replace(">", "&gt;").Replace("xmlns=" + "\"" + ns.ToString() + "\"", "");
                                TablixCell += "<td id='tdsubreport'><span style='display:none;' class='subreport'>'" + tdval + "'</span><input type='hidden' id='hdn' value='' /></td>";
                            }
                            else if (td.Descendants(ns + "Image").Any())
                            {
                                tdval = td.ToString().Replace("<", "&lt;").Replace(">", "&gt;").Replace("xmlns=" + "\"" + ns.ToString() + "\"", "");
                                //tdval = HttpContext.Server.HtmlEncode(td.ToString());
                                TablixCell += "<td id='tddrop_" + cnttr + "_" + cnt + "' class='notSelected'><input type='hidden' id='hdn' value='' /><span style='display:none;' class='NotChangeText'>'" + tdval + "'</span><img id='imgdelete' alt='Remove' src='../../Content/images/deletereport_icon.png'  onclick='Removeli(this)'></img><img id='imgImage' alt='Image' src='../../Content/images/SampleBarcodeImage.png' style='display: block;height: 12px;width: 75px; float: left; margin-top: -2px; vertical-align: middle;' ></img></td>";
                            }
                            else
                            {
                                TablixCell += "<td id='tddrop_" + cnttr + "_" + cnt + "' class='notSelected' >&nbsp;<input type='hidden' id='hdn' value='' /></td>";
                            }
                            TablixRow += TablixCell;
                        }
                        if (!string.IsNullOrEmpty(TablixRow))
                            TablixRow += "</tr>";

                        TablixRows += TablixRow;


                    }
                    if (lstTablixRowGrouping != null && lstTablixRowGrouping.Count > 0)
                    {
                        RowGroup = lstTablixRowGrouping[0].ToString().Replace("<", "&lt;").Replace(">", "&gt;").Replace("xmlns=" + "\"" + ns.ToString() + "\"", "");
                        RowGroup = "<span id=spRowGroup' style='display:none;'>'" + RowGroup + "'</span>";
                    }

                }
            }

            return Json(new { Status = true, Message = "ok", FieldLIs = strFields, ReportLeftField = TablixRows, ReportRightField = strHeaderRightFields, SubReportParam = strSubReportsParameter, RowGrouping = RowGroup, ReportPageSetUpType = ReportPageSetUpType, IsRoomDetail = IsRoomDetail }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetGroupingReportDetail(long ReportId)
        {
            bool hasTotalField = true;
            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            ReportBuilderDTO objReportBuilderDTO = new ReportBuilderDTO();
            objReportBuilderDTO = objReportMasterDAL.GetReportDetail(ReportId);
            List<ReportGroupMasterDTO> lstReportGroupMasterDTO = null;
            if (objReportBuilderDTO.ParentID.GetValueOrDefault(0) > 0)
                lstReportGroupMasterDTO = objReportMasterDAL.GetreportGroupFieldList(objReportBuilderDTO.ParentID.GetValueOrDefault(0));
            else
                lstReportGroupMasterDTO = objReportMasterDAL.GetreportGroupFieldList(ReportId);

            string ReportName = string.Empty;
            string ReportResFile = string.Empty;
            ReportName = objReportBuilderDTO.ReportFileName;
            ReportResFile = objReportBuilderDTO.MasterReportResFile;
            string rdlPath = string.Empty;
            string RDLCBaseFilePath = CommonUtility.RDLCBaseFilePath;
            if (objReportBuilderDTO.ParentID > 0)
            {
                if (objReportBuilderDTO.ISEnterpriseReport.GetValueOrDefault(false))
                {
                    rdlPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/EnterpriseReport" + @"\\" + ReportName;
                }
                else
                {
                    rdlPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID + @"\\" + ReportName;
                }
            }
            else
            {
                rdlPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/BaseReport" + @"\\" + ReportName;
            }

            XDocument doc = XDocument.Load(rdlPath);
            XNamespace ns = XNamespace.Get("http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition");
            XNamespace nsrd = XNamespace.Get("http://schemas.microsoft.com/SQLServer/reporting/reportdesigner");

            List<XElement> lstMaterFields = doc.Descendants(ns + "Field").ToList();

            string strFields = string.Empty;
            List<object> NgFields = new List<object>();
            string DatasetFields = string.Empty;

            string ReportHeight = string.Empty;
            string ReportWidth = string.Empty;
            string ReportPageSetUpType = string.Empty;

            ReportHeight = doc.Descendants(ns + "PageHeight").First().Value;
            ReportWidth = doc.Descendants(ns + "PageWidth").First().Value;
            ReportPageSetUpType = GetReportPageSetUp(ReportHeight.ToLower().Replace("in", ""), ReportWidth.ToLower().Replace("in", ""));
            List<XElement> lstTablixRow = doc.Descendants(ns + "TablixRow").ToList();
            List<XElement> lstTablixRowGrouping = doc.Descendants(ns + "Tablix").Descendants(ns + "TablixRowHierarchy").Descendants(ns + "GroupExpression").ToList();

            List<XElement> lstGroupCSS = doc.Descendants(ns + "Tablix").Descendants(ns + "TablixRowHierarchy").Descendants(ns + "TablixMember").ToList()[0].Descendants(ns + "TablixHeader").ToList();

            List<XElement> lstDSFields = doc.Descendants(ns + "Field").ToList();

            if (lstDSFields.Descendants(ns + "DataField").FirstOrDefault(x => x.Value == "Total") == null)
            {
                hasTotalField = false;
            }

            #region New Code to remove CostDecimalPoint,QuantityDecimalPoint,RoomInfo,ComapnyInfo,Int64,Guid

            if (lstDSFields != null && lstDSFields.Count > 0)
            {
                List<XElement> IEnuDSFields = IEnuDSFields = lstDSFields.Where(x => !(x.Attribute("Name").Value.Contains("Total"))).ToList();

                if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                    IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("CostDecimalPoint"))).ToList();
                if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                    IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("QuantityDecimalPoint"))).ToList();
                if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                    IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("RoomInfo"))).ToList();
                if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                    IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("CompanyInfo"))).ToList();
                if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                    IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("CurrentDateTime"))).ToList();
                if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                    IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("ID"))).ToList();
                if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                    IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("Guid"))).ToList();

                if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                    IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("Tax1Rate"))).ToList();

                if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                    IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("Tax2Rate"))).ToList();

                if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                    IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("Tax1Name"))).ToList();

                if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                    IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("Tax2Name"))).ToList();



                if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                    IEnuDSFields = IEnuDSFields.Where(x => x.Descendants(nsrd + "TypeName").FirstOrDefault() != null && !(x.Descendants(nsrd + "TypeName").FirstOrDefault().Value.Contains("System.Int64") && !(x.Attribute("Name").Value.Contains("RowNumber")))).ToList();

                if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                    IEnuDSFields = IEnuDSFields.Where(x => x.Descendants(nsrd + "TypeName").FirstOrDefault() != null && !(x.Descendants(nsrd + "TypeName").FirstOrDefault().Value.Contains("System.Guid"))).ToList();

                if (objReportBuilderDTO != null && (!string.IsNullOrEmpty(objReportBuilderDTO.MasterReportResFile)
                    && objReportBuilderDTO.MasterReportResFile.ToLower() == "res_rpt_pullsummarybyquarter"))
                {
                    if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                        IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Equals("Month1"))
                                && !(x.Attribute("Name").Value.Equals("Month2"))
                                && !(x.Attribute("Name").Value.Equals("Month3"))
                                && !(x.Attribute("Name").Value.Equals("Month4"))
                                && !(x.Attribute("Name").Value.Equals("Month5"))
                                && !(x.Attribute("Name").Value.Equals("Month6"))
                                && !(x.Attribute("Name").Value.Equals("Month7"))
                                && !(x.Attribute("Name").Value.Equals("Month8"))
                                && !(x.Attribute("Name").Value.Equals("Month9"))
                                && !(x.Attribute("Name").Value.Equals("Month10"))
                                && !(x.Attribute("Name").Value.Equals("Month11"))
                                && !(x.Attribute("Name").Value.Equals("Month12"))).ToList();
                }

                if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                {
                    lstDSFields.Remove();
                    lstDSFields = IEnuDSFields.OrderBy(e => e.Attribute("Name").Value).ToList();
                }


            }

            #endregion

            List<string> arrFields = new List<string>();

            List<XElement> lstColumns = doc.Descendants(ns + "TablixColumn").ToList();
            List<XElement> lstReportField = doc.Descendants(ns + "TablixRow").ToList();
            double sumTotalColumnsWidth = lstColumns.Select(x => x.Value.Replace("in", "")).Sum(x => double.Parse(x, System.Globalization.NumberStyles.AllowDecimalPoint));

            List<KeyValSelectDTO> lstGrouplist = new List<KeyValSelectDTO>();
            List<KeyValSelectDTO> lstRemainingGrlist = new List<KeyValSelectDTO>();
            string strHeaderFields = string.Empty;
            List<object> NgHeaderFields = new List<object>();
            if (lstReportField != null && lstReportField.Count > 0)
            {

                List<XElement> lstFieldTop = lstReportField[0].Descendants(ns + "TablixCell").ToList();
                List<XElement> lstFieldBottom = lstReportField.Count > 1 ? lstReportField[1].Descendants(ns + "TablixCell").ToList()
                                                                        : lstReportField[0].Descendants(ns + "TablixCell").ToList();
                int cellcount = lstFieldBottom.Count;

                if (objReportBuilderDTO.IsIncludeTotal == true)
                {
                    cellcount = cellcount - 1;
                }

                string tablixcelltop = string.Empty;
                string tablixcellBottom = string.Empty;
                int rowGroupCount = lstTablixRowGrouping.Count;
                int tdcount = 0;
                for (int i = 0; i <= rowGroupCount - 1; i++)
                {

                    string strcolumnwidth = lstGroupCSS[i].Descendants(ns + "Size").FirstOrDefault().Value;
                    string fontstyle = string.Empty;
                    string fontfamily = string.Empty;
                    string fontweight = string.Empty;
                    string Textalign = string.Empty;

                    if (lstGroupCSS[i].Descendants(ns + "FontStyle").Any())
                    {
                        fontstyle = lstGroupCSS[i].Descendants(ns + "FontStyle").FirstOrDefault().Value;
                    }
                    if (lstGroupCSS[i].Descendants(ns + "FontFamily").Any())
                    {
                        fontfamily = lstGroupCSS[i].Descendants(ns + "FontFamily").FirstOrDefault().Value;
                    }
                    if (lstGroupCSS[i].Descendants(ns + "FontWeight").Any())
                    {
                        fontweight = lstGroupCSS[i].Descendants(ns + "FontWeight").FirstOrDefault().Value;
                    }
                    if (lstGroupCSS[i].Descendants(ns + "TextAlign").Any())
                    {
                        Textalign = lstGroupCSS[i].Descendants(ns + "TextAlign").FirstOrDefault().Value;
                    }
                    KeyValSelectDTO objKeyValDTO = new KeyValSelectDTO();

                    tablixcellBottom = Convert.ToString(lstTablixRowGrouping[i].Value).Replace("=Fields!", "").Replace("=Parameters!BarcodeURL.Value+Fields!", "").Replace(".Value", "");
                    strHeaderFields += "<td id='tddropline_" + tdcount + "' onmousemove='doResize(this,event)'  onmouseover='doResize(this,event)' onmouseout='doneResizing()'  " + GetTDStyleWithWidth(Textalign, ConvertInchToPx(strcolumnwidth), false) + "'><div class='divLineDrag'>&nbsp;</div><span class='ChangeText'  " + GetSpanStyle(fontstyle, fontweight) + ">" + GetField(tablixcellBottom, ReportResFile) + "</span><input type='hidden' value='" + tablixcellBottom.Replace("=Parameters!BarcodeURL.Value+Fields!", "").Replace(".Value", "") + "' id='hdnFieldName' /><input type='hidden' value='Group' id='hdnFieldtype' /><img id='imgdelete' style='float: right;' alt='Remove' src='../../Content/images/deletereport_icon.png'  onclick='RemoveGrouplineItem(this)'></img></td>";
                    NgHeaderFields.Add(new { FieldType = "Group", Index = tdcount, StyleTextAlign = Textalign, StyleWidth = ConvertInchToPx(strcolumnwidth), TDStyleDisplay = "None", TDStyleWithWidth = GetTDStyleWithWidth(Textalign, ConvertInchToPx(strcolumnwidth), false), FontStyle = fontstyle, FontWeight = fontweight, SpanStyle = GetSpanStyle(fontstyle, fontweight), Field = GetField(tablixcellBottom, ReportResFile), CellBottom = tablixcellBottom.Replace("=Parameters!BarcodeURL.Value+Fields!", "").Replace(".Value", "") });
                    objKeyValDTO.key = tablixcellBottom;
                    objKeyValDTO.value = GetField(tablixcellBottom, ReportResFile);
                    objKeyValDTO.IsSelect = true;
                    lstGrouplist.Add(objKeyValDTO);
                    //tablixcellBottom = Convert.ToString(lstFieldBottom[i].Descendants(ns + "Value").FirstOrDefault().Value).Replace("=Fields!", "").Replace("=Parameters!BarcodeURL.Value+Fields!", "").Replace(".Value", "");
                    //strHeaderFields += "<td id='tddropline_" + i + "' onmousemove='doResize(this,event)'  onmouseover='doResize(this,event)' onmouseout='doneResizing()' " + GetTDStyleWithWidth(Textalign, ConvertInchToPx(strcolumnwidth)) + ";'><div class='divLineDrag'>&nbsp;</div><span class='ChangeText' " + GetSpanStyle(fontstyle, fontweight) + " >" + GetField(tablixcelltop, ReportResFile) + "</span><input type='hidden' value='" + tablixcellBottom.Replace("=Parameters!BarcodeURL.Value+Fields!", "").Replace(".Value", "") + "' id='hdnlineitem_" + (i + 1) + "' /><img id='imgdelete' style='float: right;' alt='Remove' src='../../Content/images/deletereport_icon.png'  onclick='RemovelineItem(this)'></img></td>";
                    tdcount += 1;
                }
                for (int i = 0; i <= cellcount - 1; i++)
                {

                    string strcolumnwidth = lstColumns[i].Value;

                    string fontstyle = string.Empty;
                    string fontweight = string.Empty;
                    string Textalign = string.Empty;
                    string fontfamily = string.Empty;

                    if (lstFieldTop[i].Descendants(ns + "FontStyle").Any())
                    {
                        fontstyle = lstFieldTop[i].Descendants(ns + "FontStyle").FirstOrDefault().Value;
                    }
                    if (lstFieldTop[i].Descendants(ns + "FontFamily").Any())
                    {
                        fontfamily = lstFieldTop[i].Descendants(ns + "FontFamily").FirstOrDefault().Value;
                    }

                    if (lstFieldTop[i].Descendants(ns + "FontWeight").Any())
                    {
                        fontweight = lstFieldTop[i].Descendants(ns + "FontWeight").FirstOrDefault().Value;
                    }
                    if (lstFieldTop[i].Descendants(ns + "TextAlign").Any())
                    {
                        Textalign = lstFieldTop[i].Descendants(ns + "TextAlign").FirstOrDefault().Value;
                    }
                    if (lstFieldTop[i].Descendants(ns + "Value").FirstOrDefault() != null)
                    {
                        tablixcelltop = Convert.ToString(lstFieldTop[i].Descendants(ns + "Value").FirstOrDefault().Value);
                        tablixcellBottom = Convert.ToString(lstFieldBottom[i].Descendants(ns + "Value").FirstOrDefault().Value).Replace("=Fields!", "").Replace("=Parameters!BarcodeURL.Value+Fields!", "").Replace(".Value", "");
                    }
                    strHeaderFields += "<td id='tddropline_" + tdcount + "' onmousemove='doResize(this,event)'  onmouseover='doResize(this,event)' onmouseout='doneResizing()' " + GetTDStyleWithWidth(Textalign, ConvertInchToPx(strcolumnwidth)) + ";'><div class='divLineDrag'>&nbsp;</div><span class='ChangeText' " + GetSpanStyle(fontstyle, fontweight) + " >" + GetPrefixField(tablixcelltop, ReportResFile) + "</span><input type='hidden' value='" + tablixcellBottom.Replace("=Parameters!BarcodeURL.Value+Fields!", "").Replace(".Value", "") + "' id='hdnFieldName' /><input type='hidden' value='Normal' id='hdnFieldtype' /><img id='imgdelete' style='float: right;' alt='Remove' src='../../Content/images/deletereport_icon.png'  onclick='RemoveGrouplineItem(this)'></img></td>";
                    NgHeaderFields.Add(new { FieldType = "Normal", Index = tdcount, StyleTextAlign = Textalign, StyleWidth = ConvertInchToPx(strcolumnwidth), TDStyleDisplay = "", TDStyleWithWidth = GetTDStyleWithWidth(Textalign, ConvertInchToPx(strcolumnwidth), false), FontStyle = fontstyle, FontWeight = fontweight, SpanStyle = GetSpanStyle(fontstyle, fontweight), Field = GetPrefixField(tablixcelltop, ReportResFile), CellBottom = tablixcellBottom.Replace("=Parameters!BarcodeURL.Value+Fields!", "").Replace(".Value", "") });
                    tdcount += 1;
                    if (lstReportGroupMasterDTO.Any(x => x.FieldName == tablixcellBottom))
                    {
                        KeyValSelectDTO objKeyValDTO = new KeyValSelectDTO();
                        //objKeyValDTO.key = tablixcellBottom;
                        //objKeyValDTO.value = GetField(tablixcellBottom, ReportResFile);
                        //objKeyValDTO.IsSelect = false;
                        //lstGrouplist.Add(objKeyValDTO);
                    }
                    else
                    {
                        KeyValSelectDTO objKeyValDTO = new KeyValSelectDTO();
                        objKeyValDTO.key = tablixcellBottom;
                        objKeyValDTO.value = GetPrefixField(tablixcellBottom, ReportResFile);
                        objKeyValDTO.IsSelect = false;
                        if (lstGrouplist.FindIndex(x => x.key == objKeyValDTO.key && x.value == objKeyValDTO.value) < 0)
                            lstRemainingGrlist.Add(objKeyValDTO);
                    }
                }

                if (lstDSFields != null && lstDSFields.Count > 0)
                {
                    lstDSFields = lstDSFields.OrderBy(x => GetPrefixField(x.Descendants(ns + "DataField").FirstOrDefault().Value, ReportResFile)).ToList();
                    foreach (XElement item in lstDSFields)
                    {
                        bool exist = false;
                        foreach (XElement itemhead in lstFieldBottom)
                        {
                            if ((itemhead.Descendants(ns + "Value").FirstOrDefault().Value.Replace("=Fields!", "").Replace(".Value", "")) == (item.Descendants(ns + "DataField").FirstOrDefault().Value))
                            {
                                exist = true;
                            }
                            else if ((itemhead.Descendants(ns + "Value").FirstOrDefault().Value.Replace("=Parameters!BarcodeURL.Value+Fields!", "").Replace(".Value", "")) == (item.Descendants(ns + "DataField").FirstOrDefault().Value))
                            {
                                exist = true;
                            }
                        }
                        if (exist == false)
                        {
                            //strFields += "<tr id='trdrag'><td class='TdDragLineItem'><input type='hidden' value='" + item.Descendants(ns + "DataField").FirstOrDefault().Value + "' id='hdnlineitem' />" + GetField(item.Descendants(ns + "DataField").FirstOrDefault().Value, ResourceFile) + "</td></tr>";
                            DatasetFields = item.Descendants(ns + "DataField").FirstOrDefault().Value;
                            if (lstReportGroupMasterDTO.Any(x => x.FieldName == DatasetFields))
                            {
                                strFields += "<tr id='trdrag'><td class='TdDragLineItem'>" + GetPrefixField(DatasetFields, ReportResFile) + "<input type='hidden' id='hdnFieldName' value='" + DatasetFields + "' /><input type='hidden' id='hdnFieldtype' value='Group' /></td></tr>";
                                NgFields.Add(new { DataField = DatasetFields, DataFieldDisplay = GetPrefixField(DatasetFields, ReportResFile), FieldType = "Group" });
                            }
                            else
                            {
                                strFields += "<tr id='trdrag'><td class='TdDragLineItem'>" + GetPrefixField(DatasetFields, ReportResFile) + "<input type='hidden' id='hdnFieldName' value='" + DatasetFields + "' /><input type='hidden' id='hdnFieldtype' value='Normal' /></td></tr>";
                                NgFields.Add(new { DataField = DatasetFields, DataFieldDisplay = GetPrefixField(DatasetFields, ReportResFile), FieldType = "Normal" });
                            }
                        }
                    }
                }


            }
            ViewBag.lstReportGroupMaster = lstReportGroupMasterDTO;

            return Json(new { Status = true, Message = "ok", FieldLIs = strFields, NgFieldLIs = NgFields, ReportField = strHeaderFields, NgReportField = NgHeaderFields, Grouplist = lstGrouplist, RemainingGrouplist = lstRemainingGrlist, DBGroupList = lstReportGroupMasterDTO, ReportPageSetUpType = ReportPageSetUpType, HasTotal = hasTotalField, ReportTableWidth = sumTotalColumnsWidth }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveXML(string ReportName, string strrdlXML, string SubReportName, string strSubrdlXML, long ParentId, long ChildID, string SortColumns, string ReportType, string IsPrivate, string Saveas, string Report, string MasterReportResrfile, string SubReportResFile, bool IsIncludeTotal, bool IsIncludeGrandTotal, bool IsIncludeSubTotal, string PageType, string EmailAddress, string ModuleName, string IsEnterprise, string SetAsDefaultPrintReport, bool? IsIncludeTax1, bool? IsIncludeTax2, bool isEdit, bool? HideHeader, bool? ShowSignature, bool? SetAsDefaultPrintReportForAllRoom)
        {
            ReportMasterDAL obj = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            string reportAppIntent = "ReadOnly";
            string strmsg = string.Empty;

            if (obj.CheckReportExist(Saveas, SessionHelper.CompanyID, ChildID))
            {
                strmsg = ResReportMaster.ReportNameUsedByOtherUser;
            }
            else
            {

                string rdlPath = string.Empty;
                string ParentrdlPath = string.Empty;
                string RDLCBaseFilePath = CommonUtility.RDLCBaseFilePath;
                if (string.IsNullOrEmpty(Saveas) || ChildID > 0)
                {
                    if (Convert.ToBoolean(IsEnterprise))
                    {
                        rdlPath = GetFullPath(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/EnterpriseReport") + @"\\" + ReportName;
                        if (!System.IO.File.Exists(rdlPath))
                        {
                            //rdlPath = GetFullPath(Server.MapPath(@"/RDLC_Reports/" + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID)) + @"\\" + ReportName;
                            string rdlpathFrom = GetFullPath(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID) + @"\\" + ReportName;
                            if (System.IO.File.Exists(rdlpathFrom))
                                System.IO.File.Copy(rdlpathFrom, rdlPath);
                            else
                            {
                                strmsg = ResReportMaster.ReportFileDoesNotExist;
                                return Json(new { Status = false, Message = strmsg }, JsonRequestBehavior.AllowGet);
                            }

                        }
                        if (ParentId > 0)
                        {
                            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                            ReportBuilderDTO objReportBuilderDTO = new ReportBuilderDTO();
                            objReportBuilderDTO = objReportMasterDAL.GetReportDetail(Convert.ToInt64(ParentId));
                            ParentrdlPath = GetFullPath(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/BaseReport") + @"\\" + objReportBuilderDTO.ReportFileName;

                            if (objReportBuilderDTO != null && !string.IsNullOrEmpty(objReportBuilderDTO.ReportAppIntent) && !string.IsNullOrWhiteSpace(objReportBuilderDTO.ReportAppIntent))
                            {
                                reportAppIntent = objReportBuilderDTO.ReportAppIntent;
                            }
                        }
                    }
                    else
                    {
                        rdlPath = GetFullPath(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID) + @"\\" + ReportName;

                        if (!System.IO.File.Exists(rdlPath))
                        {
                            rdlPath = GetFullPath(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/EnterpriseReport") + @"\\" + ReportName;
                        }

                        if (ParentId > 0)
                        {
                            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                            ReportBuilderDTO objReportBuilderDTO = new ReportBuilderDTO();
                            objReportBuilderDTO = objReportMasterDAL.GetReportDetail(Convert.ToInt64(ParentId));
                            ParentrdlPath = GetFullPath(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/BaseReport") + @"\\" + objReportBuilderDTO.ReportFileName;

                            if (objReportBuilderDTO != null && !string.IsNullOrEmpty(objReportBuilderDTO.ReportAppIntent) && !string.IsNullOrWhiteSpace(objReportBuilderDTO.ReportAppIntent))
                            {
                                reportAppIntent = objReportBuilderDTO.ReportAppIntent;
                            }
                        }
                    }
                }
                else
                {
                    rdlPath = GetFullPath(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/BaseReport") + @"\\" + ReportName;
                    if (ParentId > 0)
                    {
                        ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                        ReportBuilderDTO objReportBuilderDTO = new ReportBuilderDTO();
                        objReportBuilderDTO = objReportMasterDAL.GetReportDetail(Convert.ToInt64(ParentId));
                        ParentrdlPath = GetFullPath(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/BaseReport") + @"\\" + objReportBuilderDTO.ReportFileName;

                        if (objReportBuilderDTO != null && !string.IsNullOrEmpty(objReportBuilderDTO.ReportAppIntent) && !string.IsNullOrWhiteSpace(objReportBuilderDTO.ReportAppIntent))
                        {
                            reportAppIntent = objReportBuilderDTO.ReportAppIntent;
                        }
                    }
                }

                #region "Main Report"
                XDocument doc = XDocument.Load(rdlPath);


                XNamespace ns = XNamespace.Get("http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition");
                XNamespace nsrd = XNamespace.Get("http://schemas.microsoft.com/SQLServer/reporting/reportdesigner");

                doc.Descendants(ns + "Tablix").First().Value = strrdlXML;

                string strColumsn = strrdlXML.Substring(0, strrdlXML.IndexOf("<TablixRows>"));
                strColumsn = strColumsn.Remove(0, strrdlXML.IndexOf("<TablixColumns>"));
                XDocument xdtemp = XDocument.Parse(strColumsn);
                List<XElement> lstWidths = xdtemp.Descendants("Width").ToList();
                lstWidths.ForEach(x => { x.Value = x.Value.Replace("in", ""); });
                double totWidth = lstWidths.Sum(x => double.Parse(x.Value));

                double pagewdth = totWidth;// + 0.50;
                if (!string.IsNullOrEmpty(ParentrdlPath))
                {
                    XDocument docBase = XDocument.Load(ParentrdlPath);
                    IEnumerable<XElement> lstDSParentFields = docBase.Descendants(ns + "Page").Descendants(ns + "PageHeader");
                    XDocument docChild = XDocument.Load(rdlPath);
                    IEnumerable<XElement> lstDSChildFields = doc.Descendants(ns + "Page").Descendants(ns + "PageHeader");
                    doc.Descendants(ns + "Page").Descendants(ns + "PageHeader").FirstOrDefault().ReplaceWith(lstDSParentFields.ToList());
                    lstDSParentFields = docBase.Descendants(ns + "ReportParameters");
                    doc.Descendants(ns + "ReportParameters").FirstOrDefault().ReplaceWith(lstDSParentFields.ToList());
                    //doc.Save(rdlPath);
                    //  doc.Descendants(ns + "Page").Descendants(ns + "PageHeader"). = ParentReportdoc.Descendants(ns + "Page").Descendants(ns + "PageHeader");
                }

                if (doc.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Textbox").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "TotalPages") != null)
                    doc.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Textbox").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "TotalPages").Descendants(ns + "Width").FirstOrDefault().Value = "1.35in";

                if (doc.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Image").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "eTurnsLogo2") != null)
                    doc.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Image").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "eTurnsLogo2").Descendants(ns + "Width").FirstOrDefault().Value = "1.35in";

                doc.Root.Element(ns + "Width").Value = (pagewdth + 0.05).ToString("N2") + "in";

                if (PageType == "2")
                {
                    if (pagewdth < 10.40)
                        doc.Root.Element(ns + "Width").Value = "10.40in";

                    if (doc.Descendants(ns + "PageHeight").Count() <= 0)
                        doc.Descendants(ns + "Page").FirstOrDefault().Add(new XElement(ns + "PageHeight", "8.5in"));
                    else
                        doc.Descendants(ns + "PageHeight").First().Value = "8.5in";


                    if (doc.Descendants(ns + "PageWidth").Count() <= 0)
                        doc.Descendants(ns + "Page").FirstOrDefault().Add(new XElement(ns + "PageWidth", "11in"));
                    else
                        doc.Descendants(ns + "PageWidth").First().Value = "11in";



                    // pagewdth = 10.20;
                    if (doc.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").
                        Descendants(ns + "Textbox").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "TotalPages") != null)
                    {
                        doc.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Textbox").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "TotalPages").Descendants(ns + "Left").FirstOrDefault().Value = "9.10in";
                    }

                    if (doc.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").
                            Descendants(ns + "Image").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "eTurnsLogo2") != null)
                    {
                        doc.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Image").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "eTurnsLogo2").Descendants(ns + "Left").FirstOrDefault().Value = "9.10in";
                    }
                    if (doc.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").
                                Descendants(ns + "Textbox").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "ReportTitle") != null)
                    {
                        doc.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Textbox").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "ReportTitle").Descendants(ns + "Paragraphs").Descendants(ns + "Paragraph").Descendants(ns + "TextRuns").Descendants(ns + "TextRun").Descendants(ns + "Value").FirstOrDefault().Value = Saveas;
                        doc.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Textbox").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "ReportTitle").Descendants(ns + "Width").FirstOrDefault().Value = "5.0in";
                        doc.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Textbox").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "ReportTitle").Descendants(ns + "Left").FirstOrDefault().Value = "2.80in";
                    }

                }
                else if (PageType == "1")
                {
                    if (pagewdth < 7.90)
                        doc.Root.Element(ns + "Width").Value = "07.90in";

                    //doc.Descendants(ns + "PageHeight").First().Value = "11in";
                    // doc.Descendants(ns + "PageWidth").First().Value = "8.5in";

                    if (doc.Descendants(ns + "PageHeight").Count() <= 0)
                        doc.Descendants(ns + "Page").FirstOrDefault().Add(new XElement(ns + "PageHeight", "11in"));
                    else
                        doc.Descendants(ns + "PageHeight").First().Value = "11in";


                    if (doc.Descendants(ns + "PageWidth").Count() <= 0)
                        doc.Descendants(ns + "Page").FirstOrDefault().Add(new XElement(ns + "PageWidth", "8.5in"));
                    else
                        doc.Descendants(ns + "PageWidth").First().Value = "8.5in";

                    //pagewdth = 7.90;

                    if (doc.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").
                        Descendants(ns + "Textbox").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "TotalPages") != null)
                    {
                        doc.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Textbox").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "TotalPages").Descendants(ns + "Left").FirstOrDefault().Value = "6.60in";
                    }

                    if (doc.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").
                            Descendants(ns + "Image").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "eTurnsLogo2") != null)
                    {
                        doc.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Image").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "eTurnsLogo2").Descendants(ns + "Left").FirstOrDefault().Value = "6.60in";
                    }
                    if (doc.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").
                                Descendants(ns + "Textbox").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "ReportTitle") != null)
                    {
                        doc.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Textbox").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "ReportTitle").Descendants(ns + "Paragraphs").Descendants(ns + "Paragraph").Descendants(ns + "TextRuns").Descendants(ns + "TextRun").Descendants(ns + "Value").FirstOrDefault().Value = Saveas;
                        doc.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Textbox").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "ReportTitle").Descendants(ns + "Width").FirstOrDefault().Value = "3.0in";
                        doc.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Textbox").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "ReportTitle").Descendants(ns + "Left").FirstOrDefault().Value = "2.50in";
                    }

                }

                string ReportHeight = string.Empty;
                string ReportWidth = string.Empty;
                string ReportPageSetUpType = string.Empty;

                ReportWidth = doc.Descendants(ns + "PageWidth").First().Value.Replace("in", "");
                if (doc.Descendants(ns + "PageHeight") != null && doc.Descendants(ns + "PageHeight").Count() > 0)
                {
                    ReportHeight = doc.Descendants(ns + "PageHeight").First().Value.Replace("in", "");
                }

                ReportPageSetUpType = GetReportPageSetUp(ReportHeight.ToLower().Replace("in", ""), ReportWidth.ToLower().Replace("in", ""));


                #region "Page Header"
                try
                {
                    if (ReportPageSetUpType != PageType)
                    {
                        List<XElement> lstPageHeaderItem = new List<XElement>();
                        lstPageHeaderItem = doc.Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Left").ToList();
                        if (lstPageHeaderItem != null && lstPageHeaderItem.Count > 0 && pagewdth > 0)
                        {
                            int HeaderItemCnt = lstPageHeaderItem.Count;
                            double Cellcount = Convert.ToDouble(Math.Round((pagewdth / HeaderItemCnt), 2));
                            double InctCellcount = 0.09;
                            for (int i = 0; i < HeaderItemCnt; i++)
                            {
                                if (i == (HeaderItemCnt - 1))
                                {
                                    string ItemWidth = doc.Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Width").ToList()[i].Value.ToLower().Replace("in", "");
                                    double clacwidth = Math.Round(pagewdth - (Convert.ToDouble(ItemWidth) - 0.5), 2);
                                    if (clacwidth <= 0)
                                    {
                                        clacwidth = 0.10;
                                    }
                                    doc.Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Left").ToList()[i].Value = Convert.ToString(clacwidth) + "in";
                                }
                                else
                                {
                                    doc.Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Left").ToList()[i].Value = Convert.ToString(InctCellcount) + "in";
                                }

                                InctCellcount += Cellcount;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
                #endregion
                #region "Page Footer"
                try
                {

                    if (GetReportPageSetUp(ReportHeight, ReportWidth) != PageType)
                    {
                        List<XElement> lstPageFooterItem = new List<XElement>();
                        lstPageFooterItem = doc.Descendants(ns + "PageFooter").Descendants(ns + "ReportItems").Descendants(ns + "Left").ToList();
                        if (lstPageFooterItem != null && lstPageFooterItem.Count > 0 && pagewdth > 0)
                        {
                            int FooterItemCnt = lstPageFooterItem.Count;
                            double Cellcount = Convert.ToDouble(Math.Round((pagewdth / FooterItemCnt), 2));
                            double InctCellcount = 0.09;
                            for (int i = 0; i < FooterItemCnt; i++)
                            {
                                if (i == (FooterItemCnt - 1))
                                {
                                    string ItemWidth = doc.Descendants(ns + "PageFooter").Descendants(ns + "ReportItems").Descendants(ns + "Width").ToList()[i].Value.ToLower().Replace("in", "");
                                    double clacwidth = Math.Round(pagewdth - (Convert.ToDouble(ItemWidth) - 0.5), 2);
                                    doc.Descendants(ns + "PageFooter").Descendants(ns + "ReportItems").Descendants(ns + "Left").ToList()[i].Value = Convert.ToString(clacwidth) + "in";
                                }
                                else
                                {
                                    doc.Descendants(ns + "PageFooter").Descendants(ns + "ReportItems").Descendants(ns + "Left").ToList()[i].Value = Convert.ToString(InctCellcount) + "in";
                                }

                                InctCellcount += Cellcount;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
                #endregion
                string strdoc = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
                strdoc += doc.ToString().Replace("&gt;", ">");
                strdoc = strdoc.ToString().Replace("&lt;", "<");
                if ((!string.IsNullOrEmpty(SubReportName)) && (!string.IsNullOrEmpty(strSubrdlXML)) && (!string.IsNullOrEmpty(Saveas)))
                {
                    TextReader tr = new StringReader(strdoc);
                    doc = XDocument.Load(tr);
                    doc.Descendants(ns + "Tablix").Descendants(ns + "Subreport").FirstOrDefault().Attribute("Name").Value = "SubRPT_" + Saveas;
                    doc.Descendants(ns + "Tablix").Descendants(ns + "ReportName").FirstOrDefault().Value = "SubRPT_" + Saveas;
                    strdoc = string.Empty;
                    strdoc = doc.ToString();
                }

                if (!string.IsNullOrEmpty(Saveas))
                {

                    if (Convert.ToBoolean(IsEnterprise))
                    {
                        rdlPath = GetFullPath(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/EnterpriseReport") + @"\\RPT_" + Saveas + ".rdlc";
                    }
                    else
                    {
                        rdlPath = GetFullPath(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID) + @"\\RPT_" + Saveas + ".rdlc";
                    }
                }
                System.IO.File.WriteAllText(rdlPath, strdoc);

                #region For Item Received Receivable Report

                ReportMaster objRPTDTO = new ReportMaster();
                objRPTDTO = obj.GetReportIDNew("Item Received Receivable");

                if (objRPTDTO != null && !string.IsNullOrWhiteSpace(objRPTDTO.CombineReportID))
                {
                    string[] ReportIDs = objRPTDTO.CombineReportID.Split(',');
                    bool isReceivedReport = false;
                    bool isReceivableReport = false;
                    if (ReportIDs.Length > 0)
                    {
                        for (int i = 0; i < ReportIDs.Length; i++)
                        {
                            ReportBuilderDTO reportBuilderDTO = obj.GetParentReportMasterByReportID(Convert.ToInt32(ReportIDs[i]));
                            if (reportBuilderDTO != null && reportBuilderDTO.ID > 0
                                && Convert.ToInt32(ReportIDs[i]) == ChildID)
                            {
                                if (reportBuilderDTO.ReportName.ToLower().Equals("received items"))
                                {
                                    isReceivedReport = true;
                                }
                                if (reportBuilderDTO.ReportName.ToLower().Equals("receivable items"))
                                {
                                    isReceivableReport = true;
                                }
                            }
                        }
                    }
                    if (isReceivedReport || isReceivableReport)
                    {
                        string ReportPathTobUpdate = "";
                        ReportPathTobUpdate = GetFullPath(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/BaseReport") + @"\\" + "RPT_ItemReceivedReceivable.rdlc";
                        if (System.IO.File.Exists(ReportPathTobUpdate)
                            && !string.IsNullOrWhiteSpace(strrdlXML))
                        {
                            XDocument docActual = XDocument.Load(ReportPathTobUpdate);
                            XDocument docUpdated = XDocument.Load(ReportPathTobUpdate);
                            foreach (var tablix in docUpdated.Descendants(ns + "Tablix").ToList())
                            {
                                if (tablix != null)
                                {
                                    if (isReceivedReport)
                                    {
                                        if (tablix.LastAttribute.Value.Equals("Tablix1"))
                                        {
                                            tablix.Value = strrdlXML;
                                            //(docUpdated.Descendants(ns + "Tablix").ToList())[0].Value = strrdlXML;
                                        }
                                    }
                                    if (isReceivableReport)
                                    {
                                        if (tablix.LastAttribute.Value.Equals("Tablix2"))
                                        {
                                            tablix.Value = strrdlXML;
                                            //XElement xElerptTitle = doc.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Textbox").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "ReportTitle");
                                            //(docUpdated.Descendants(ns + "Tablix").ToList())[1].Value = strrdlXML;                                            
                                        }
                                    }
                                }
                            }
                            if (!string.IsNullOrEmpty(ReportPathTobUpdate))
                            {
                                XDocument docBase = XDocument.Load(ReportPathTobUpdate);
                                IEnumerable<XElement> lstDSParentFields = docBase.Descendants(ns + "Page").Descendants(ns + "PageHeader");
                                XDocument docChild = XDocument.Load(ReportPathTobUpdate);
                                IEnumerable<XElement> lstDSChildFields = docUpdated.Descendants(ns + "Page").Descendants(ns + "PageHeader");
                                docUpdated.Descendants(ns + "Page").Descendants(ns + "PageHeader").FirstOrDefault().ReplaceWith(lstDSParentFields.ToList());
                                lstDSParentFields = docBase.Descendants(ns + "ReportParameters");
                                docUpdated.Descendants(ns + "ReportParameters").FirstOrDefault().ReplaceWith(lstDSParentFields.ToList());
                            }

                            if (!string.IsNullOrWhiteSpace(ReportPathTobUpdate))
                            {
                                if (docUpdated.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Textbox").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "TotalPages") != null)
                                    docUpdated.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Textbox").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "TotalPages").Descendants(ns + "Width").FirstOrDefault().Value = "1.35in";

                                if (docUpdated.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Image").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "eTurnsLogo2") != null)
                                    docUpdated.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Image").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "eTurnsLogo2").Descendants(ns + "Width").FirstOrDefault().Value = "1.35in";

                                docUpdated.Root.Element(ns + "Width").Value = (pagewdth + 0.05).ToString("N2") + "in";

                                if (PageType == "2")
                                {
                                    if (pagewdth < 10.40)
                                        docUpdated.Root.Element(ns + "Width").Value = "10.40in";

                                    if (docUpdated.Descendants(ns + "PageHeight").Count() <= 0)
                                        docUpdated.Descendants(ns + "Page").FirstOrDefault().Add(new XElement(ns + "PageHeight", "8.5in"));
                                    else
                                        docUpdated.Descendants(ns + "PageHeight").First().Value = "8.5in";


                                    if (docUpdated.Descendants(ns + "PageWidth").Count() <= 0)
                                        docUpdated.Descendants(ns + "Page").FirstOrDefault().Add(new XElement(ns + "PageWidth", "11in"));
                                    else
                                        docUpdated.Descendants(ns + "PageWidth").First().Value = "11in";

                                    // pagewdth = 10.20;
                                    if (docUpdated.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").
                                        Descendants(ns + "Textbox").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "TotalPages") != null)
                                    {
                                        docUpdated.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Textbox").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "TotalPages").Descendants(ns + "Left").FirstOrDefault().Value = "9.10in";
                                    }

                                    if (docUpdated.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").
                                            Descendants(ns + "Image").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "eTurnsLogo2") != null)
                                    {
                                        docUpdated.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Image").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "eTurnsLogo2").Descendants(ns + "Left").FirstOrDefault().Value = "9.10in";
                                    }
                                    //if (docUpdated.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").
                                    //            Descendants(ns + "Textbox").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "ReportTitle") != null)
                                    //{
                                    //    docUpdated.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Textbox").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "ReportTitle").Descendants(ns + "Paragraphs").Descendants(ns + "Paragraph").Descendants(ns + "TextRuns").Descendants(ns + "TextRun").Descendants(ns + "Value").FirstOrDefault().Value = Saveas;
                                    //    docUpdated.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Textbox").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "ReportTitle").Descendants(ns + "Width").FirstOrDefault().Value = "5.0in";
                                    //    docUpdated.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Textbox").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "ReportTitle").Descendants(ns + "Left").FirstOrDefault().Value = "2.80in";
                                    //}

                                }
                                else if (PageType == "1")
                                {
                                    if (pagewdth < 7.90)
                                        docUpdated.Root.Element(ns + "Width").Value = "07.90in";

                                    if (docUpdated.Descendants(ns + "PageHeight").Count() <= 0)
                                        docUpdated.Descendants(ns + "Page").FirstOrDefault().Add(new XElement(ns + "PageHeight", "11in"));
                                    else
                                        docUpdated.Descendants(ns + "PageHeight").First().Value = "11in";


                                    if (docUpdated.Descendants(ns + "PageWidth").Count() <= 0)
                                        docUpdated.Descendants(ns + "Page").FirstOrDefault().Add(new XElement(ns + "PageWidth", "8.5in"));
                                    else
                                        docUpdated.Descendants(ns + "PageWidth").First().Value = "8.5in";

                                    if (docUpdated.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").
                                        Descendants(ns + "Textbox").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "TotalPages") != null)
                                    {
                                        docUpdated.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Textbox").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "TotalPages").Descendants(ns + "Left").FirstOrDefault().Value = "6.60in";
                                    }

                                    if (docUpdated.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").
                                            Descendants(ns + "Image").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "eTurnsLogo2") != null)
                                    {
                                        docUpdated.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Image").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "eTurnsLogo2").Descendants(ns + "Left").FirstOrDefault().Value = "6.60in";
                                    }
                                    //if (docUpdated.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").
                                    //            Descendants(ns + "Textbox").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "ReportTitle") != null)
                                    //{
                                    //    docUpdated.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Textbox").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "ReportTitle").Descendants(ns + "Paragraphs").Descendants(ns + "Paragraph").Descendants(ns + "TextRuns").Descendants(ns + "TextRun").Descendants(ns + "Value").FirstOrDefault().Value = Saveas;
                                    //    docUpdated.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Textbox").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "ReportTitle").Descendants(ns + "Width").FirstOrDefault().Value = "3.0in";
                                    //    docUpdated.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Textbox").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "ReportTitle").Descendants(ns + "Left").FirstOrDefault().Value = "2.50in";
                                    //}
                                }

                                string ReportHeight1 = string.Empty;
                                string ReportWidth1 = string.Empty;
                                string ReportPageSetUpType1 = string.Empty;

                                ReportWidth1 = docActual.Descendants(ns + "PageWidth").First().Value.Replace("in", "");
                                if (docUpdated.Descendants(ns + "PageHeight") != null && docUpdated.Descendants(ns + "PageHeight").Count() > 0)
                                {
                                    ReportHeight1 = docActual.Descendants(ns + "PageHeight").First().Value.Replace("in", "");
                                }

                                ReportPageSetUpType1 = GetReportPageSetUp(ReportHeight1.ToLower().Replace("in", ""), ReportWidth1.ToLower().Replace("in", ""));

                                //#region "Page Header"
                                try
                                {
                                    if (ReportPageSetUpType1 != PageType)
                                    {
                                        List<XElement> lstPageHeaderItem = new List<XElement>();
                                        lstPageHeaderItem = docUpdated.Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Left").ToList();
                                        if (lstPageHeaderItem != null && lstPageHeaderItem.Count > 0 && pagewdth > 0)
                                        {
                                            int HeaderItemCnt = lstPageHeaderItem.Count;
                                            double Cellcount = Convert.ToDouble(Math.Round((pagewdth / HeaderItemCnt), 2));
                                            double InctCellcount = 0.09;
                                            for (int i = 0; i < HeaderItemCnt; i++)
                                            {
                                                if (i == (HeaderItemCnt - 1))
                                                {
                                                    string ItemWidth = docUpdated.Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Width").ToList()[i].Value.ToLower().Replace("in", "");
                                                    double clacwidth = Math.Round(pagewdth - (Convert.ToDouble(ItemWidth) - 0.5), 2);
                                                    if (clacwidth <= 0)
                                                    {
                                                        clacwidth = 0.10;
                                                    }
                                                    docUpdated.Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Left").ToList()[i].Value = Convert.ToString(clacwidth) + "in";
                                                }
                                                else
                                                {
                                                    docUpdated.Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Left").ToList()[i].Value = Convert.ToString(InctCellcount) + "in";
                                                }

                                                InctCellcount += Cellcount;
                                            }
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                }
                                //#endregion
                                //#region "Page Footer"
                                try
                                {
                                    if (GetReportPageSetUp(ReportHeight1, ReportWidth1) != PageType)
                                    {
                                        List<XElement> lstPageFooterItem = new List<XElement>();
                                        lstPageFooterItem = docUpdated.Descendants(ns + "PageFooter").Descendants(ns + "ReportItems").Descendants(ns + "Left").ToList();
                                        if (lstPageFooterItem != null && lstPageFooterItem.Count > 0 && pagewdth > 0)
                                        {
                                            int FooterItemCnt = lstPageFooterItem.Count;
                                            double Cellcount = Convert.ToDouble(Math.Round((pagewdth / FooterItemCnt), 2));
                                            double InctCellcount = 0.09;
                                            for (int i = 0; i < FooterItemCnt; i++)
                                            {
                                                if (i == (FooterItemCnt - 1))
                                                {
                                                    string ItemWidth = docUpdated.Descendants(ns + "PageFooter").Descendants(ns + "ReportItems").Descendants(ns + "Width").ToList()[i].Value.ToLower().Replace("in", "");
                                                    double clacwidth = Math.Round(pagewdth - (Convert.ToDouble(ItemWidth) - 0.5), 2);
                                                    docUpdated.Descendants(ns + "PageFooter").Descendants(ns + "ReportItems").Descendants(ns + "Left").ToList()[i].Value = Convert.ToString(clacwidth) + "in";
                                                }
                                                else
                                                {
                                                    docUpdated.Descendants(ns + "PageFooter").Descendants(ns + "ReportItems").Descendants(ns + "Left").ToList()[i].Value = Convert.ToString(InctCellcount) + "in";
                                                }

                                                InctCellcount += Cellcount;
                                            }
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                }
                                //#endregion

                                string strdocUpdated = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
                                strdocUpdated += docUpdated.ToString().Replace("&gt;", ">");
                                strdocUpdated = strdocUpdated.ToString().Replace("&lt;", "<");
                                TextReader tr = new StringReader(strdocUpdated);
                                XDocument doc1 = XDocument.Load(tr);
                                bool isTopNodeFound = false;
                                bool isPageBreakNodeFound = false;
                                if ((doc1.Descendants(ns + "Tablix").ToList())[1] != null)
                                {
                                    foreach (var texbox in (doc1.Descendants(ns + "Tablix").ToList())[1].Descendants(ns + "Textbox").ToList())
                                    {
                                        var texboxlist1 = (doc1.Descendants(ns + "Tablix").ToList())[0].Descendants(ns + "Textbox").ToList();
                                        if (texboxlist1.Where(x => x.FirstAttribute.Value.ToString().Trim().Equals(texbox.FirstAttribute.Value.ToString().Trim())).Count() > 0)
                                        {
                                            texbox.FirstAttribute.Value = texbox.FirstAttribute.Value + "XYZ";
                                        }
                                    }
                                    foreach (var grouplst in (doc1.Descendants(ns + "Tablix").ToList())[1].Descendants(ns + "Group").ToList())
                                    {
                                        var grouplst1 = (doc1.Descendants(ns + "Tablix").ToList())[0].Descendants(ns + "Group").ToList();
                                        if (grouplst1.Where(x => x.FirstAttribute.Value.ToString().Trim().Equals(grouplst.FirstAttribute.Value.ToString().Trim())).Count() > 0)
                                        {
                                            grouplst.FirstAttribute.Value = grouplst.FirstAttribute.Value + "XYZ";
                                        }
                                    }
                                    foreach (var nd in (doc1.Descendants(ns + "Tablix").ToList())[1].Nodes().ToList())
                                    {
                                        string nodeName = ((System.Xml.Linq.XElement)nd).Name.LocalName;
                                        if (nodeName == "Top")
                                        {
                                            isTopNodeFound = true;
                                            ((System.Xml.Linq.XElement)nd).Value = "1.22916in";
                                        }
                                        else if (nodeName == "DataSetName")
                                        {
                                            ((System.Xml.Linq.XElement)nd).Value = "DS_ReceivablesItems";
                                        }
                                        else if (nodeName == "PageBreak")
                                        {
                                            isPageBreakNodeFound = true;
                                            ((System.Xml.Linq.XElement)nd).Value = "<BreakLocation>Start</BreakLocation>";
                                        }
                                    }
                                    if (!isTopNodeFound)
                                    {
                                        (doc1.Descendants(ns + "Tablix").ToList())[1].Add(new XElement(ns + "Top", "1.22916in"));
                                    }
                                    if (!isPageBreakNodeFound)
                                    {
                                        (doc1.Descendants(ns + "Tablix").ToList())[1].Add(new XElement(ns + "PageBreak", "<BreakLocation>Start</BreakLocation>"));
                                    }
                                }
                                //strdocUpdated = doc1.ToString();
                                strdocUpdated = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
                                strdocUpdated += doc1.ToString().Replace("&gt;", ">");
                                strdocUpdated = strdocUpdated.ToString().Replace("&lt;", "<");
                                System.IO.File.WriteAllText(ReportPathTobUpdate, strdocUpdated);
                            }
                        }
                    }
                }

                #endregion

                ReportBuilderDTO objDTO = new ReportBuilderDTO();
                objDTO.ID = Convert.ToInt64(ParentId);
                if (ChildID > 0)
                {
                    objDTO.ID = ChildID;
                }
                objDTO.SortColumns = SortColumns;
                objDTO.IsPrivate = Convert.ToBoolean(IsPrivate);

                objDTO.HideHeader = Convert.ToBoolean(HideHeader ?? false);
                objDTO.ShowSignature = Convert.ToBoolean(ShowSignature ?? false);

                objDTO.ISEnterpriseReport = Convert.ToBoolean(IsEnterprise);
                objDTO.ReportFileName = "RPT_" + Saveas + ".rdlc";
                objDTO.IsIncludeTotal = IsIncludeTotal;
                objDTO.IsIncludeGrandTotal = IsIncludeGrandTotal;
                objDTO.IsIncludeTax1 = IsIncludeTax1;
                objDTO.IsIncludeTax2 = IsIncludeTax2;

                objDTO.IsIncludeSubTotal = IsIncludeSubTotal;
                objDTO.ToEmailAddress = EmailAddress;//.Trim();
                objDTO.ModuleName = ModuleName.Trim();
                objDTO.LastUpdatedBy = SessionHelper.UserID;

                if (!string.IsNullOrEmpty(SetAsDefaultPrintReport))
                    objDTO.SetAsDefaultPrintReport = bool.Parse(SetAsDefaultPrintReport);

                if (Convert.ToBoolean(IsPrivate) == true)
                {
                    objDTO.PrivateUserID = SessionHelper.UserID;
                }

                objDTO.CompanyID = SessionHelper.CompanyID;
                objDTO.RoomID = SessionHelper.RoomID;

                if (Convert.ToBoolean(IsEnterprise) == true)
                {
                    objDTO.PrivateUserID = 0;
                    objDTO.CompanyID = 0;
                    objDTO.RoomID = 0;
                    objDTO.ISEnterpriseReport = true;
                }
                objDTO.ReportName = Saveas;
                objDTO.ReportAppIntent = reportAppIntent;

                if (!isEdit)
                {
                    objDTO.ReportFileName = "RPT_" + Saveas + ".rdlc";
                    if (!string.IsNullOrEmpty(SubReportName))
                    {
                        objDTO.SubReportFileName = "SubRPT_" + Saveas + ".rdlc";
                    }
                    objDTO.ParentID = ParentId;
                    if (ChildID > 0)
                    {
                        objDTO.ParentID = ChildID;
                    }

                    objDTO.ID = 0;

                    objDTO.MasterReportResFile = MasterReportResrfile;
                    objDTO.SubReportResFile = SubReportResFile;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.ReportType = Convert.ToInt32(Report);

                    Int64 InsertedReportID = obj.Insert(objDTO);

                    if (InsertedReportID > 0 && objDTO.SetAsDefaultPrintReport.GetValueOrDefault(false) == true)
                    {
                        string ModName = ModuleName;
                        if (ModuleName == "Company")
                            ModName = "AdminCompany";
                        else if (ModuleName == "Enterprises List")
                            ModName = "AdminEnterprise";
                        else if (ModuleName == "Inventory Count" || ModuleName == "CountMaster")
                            ModName = "InventoryCount";
                        else if (ModuleName == "Item" || ModuleName == "Item List" || ModuleName == "ItemList"
                                || ModuleName == "ItemListingGroup" || ModuleName == "Items"
                                || ModuleName == "InStockByBin" || ModuleName == "Item-Discrepency"
                                || ModuleName == "Staging" || ModuleName == "InStockByBinMargin"
                                || ModuleName == "InStockWithQOH" || ModuleName == "ExpiringItems"
                                || ModuleName == "InStockByActivity" || ModuleName == "InventoryDailyHistory"
                                || ModuleName == "InventoryReconciliation" || ModuleName == "ATTSummary"
                                || ModuleName == "Stock Out Item"
                                || ModuleName == "InventoryDailyHistoryWithDateRange"
                                | ModuleName == "Inventory Stock Out"
                                || ModuleName == "ItemsWithSuppliers")
                            ModName = "InventoryItem";
                        else if (ModuleName == "Kit")
                            ModName = "InventoryKit";
                        else if (ModuleName == "Order" || ModuleName == "Order Grouping"
                                 || ModuleName == "Order List" || ModuleName == "Order"
                                 || ModuleName == "Order Summary" || ModuleName == "Replenish_Order"
                                 || ModuleName == "UnfulFilledOrders" || ModuleName == "ClosedOrder"
                                 || ModuleName == "OrderMasterList")
                            ModName = "ReplenishOrder";
                        else if (ModuleName == "Project Spend" || ModuleName == "ProjectSpend")
                            ModName = "ConsumeProjectSpend";
                        else if (ModuleName == "Pull" || ModuleName == "Consume_Pull" || ModuleName == "WOPullSummary")
                            ModName = "ConsumePull";
                        else if (ModuleName == "Pull Completed" || ModuleName == "Pull Incomplete" || ModuleName == "Pull No Header" || ModuleName == "Pull Summary" || ModuleName == "Pull Summary By ConsignedPO" || ModuleName == "Pull Summary by Quarter")
                            ModName = "Consume_Pull";
                        else if (ModuleName == "Requisition" || ModuleName == "Consume_Requisition"
                                 || ModuleName == "Range-Consume_Requisition" || ModuleName == "ReqItemSummary")
                            ModName = "ConsumeRequisition";
                        else if (ModuleName == "Return Order" || ModuleName == "ReturnOrder")
                            ModName = "ReplenishReturnOrder";
                        else if (ModuleName == "Room")
                            ModName = "AdminRoom";
                        else if (ModuleName == "Suggested Orders" || ModuleName == "Cart"
                                 || ModuleName == "SuggOrderOfExpDate")
                            ModName = "ReplenishCart";
                        else if (ModuleName == "Tools")
                            ModName = "Tool";
                        else if (ModuleName == "Transfer" || ModuleName == "TransferdItems")
                            ModName = "ReplenishTransfer";
                        else if (ModuleName == "Users")
                            ModName = "AdminUser";
                        else if (ModuleName == "Work Order" || ModuleName == "WorkOrder" || ModuleName == "WorkorderList")
                            ModName = "ConsumeWorkOrder";

                        ReportMasterDAL objReportDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                        if (SetAsDefaultPrintReportForAllRoom.GetValueOrDefault(false) == true)
                        {
                            if (objDTO.ISEnterpriseReport.GetValueOrDefault(false) == true)
                            {
                                objReportDAL.InsertDefaultReportForAllRoom(InsertedReportID, ModName, 0, 0, 0);
                            }
                            else if (objDTO.ISEnterpriseReport.GetValueOrDefault(false) == false)
                            {
                                objReportDAL.InsertDefaultReportForAllRoom(InsertedReportID, ModName, 0, SessionHelper.CompanyID, SessionHelper.RoomID);
                            }
                        }
                        else
                        {
                            objReportDAL.UpdateDefaultReportBasedonCustomiseReport(InsertedReportID, ModName, SessionHelper.CompanyID, SessionHelper.RoomID);
                        }
                    }
                }
                else
                {
                    string ModName = ModuleName;
                    objDTO.ReportType = Convert.ToInt32(Report);

                    #region For Default Print option as par module WI-4440

                    ReportMasterDAL objReportDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                    ReportBuilderDTO objDefaultReport = new ReportBuilderDTO();
                    objDefaultReport = objReportDAL.GetReportDetail(Convert.ToInt64(objDTO.ID));

                    if (ModuleName == "Company")
                        ModName = "AdminCompany";
                    else if (ModuleName == "Enterprises List")
                        ModName = "AdminEnterprise";
                    else if (ModuleName == "Inventory Count" || ModuleName == "CountMaster")
                        ModName = "InventoryCount";
                    else if (ModuleName == "Item" || ModuleName == "Item List" || ModuleName == "ItemList"
                            || ModuleName == "ItemListingGroup" || ModuleName == "Items"
                            || ModuleName == "InStockByBin" || ModuleName == "Item-Discrepency"
                            || ModuleName == "Staging" || ModuleName == "InStockByBinMargin"
                            || ModuleName == "InStockWithQOH" || ModuleName == "ExpiringItems"
                            || ModuleName == "InStockByActivity" || ModuleName == "InventoryDailyHistory"
                            || ModuleName == "InventoryReconciliation" || ModuleName == "ATTSummary"
                            || ModuleName == "Stock Out Item"
                            || ModuleName == "InventoryDailyHistoryWithDateRange"
                            || ModuleName == "Inventory Stock Out"
                            || ModuleName == "ItemsWithSuppliers")
                        ModName = "InventoryItem";
                    else if (ModuleName == "Kit")
                        ModName = "InventoryKit";
                    else if (ModuleName == "Order" || ModuleName == "Order Grouping"
                             || ModuleName == "Order List" || ModuleName == "Order"
                             || ModuleName == "Order Summary" || ModuleName == "Replenish_Order"
                             || ModuleName == "UnfulFilledOrders" || ModuleName == "ClosedOrder"
                             || ModuleName == "OrderMasterList")
                        ModName = "ReplenishOrder";
                    else if (ModuleName == "Project Spend" || ModuleName == "ProjectSpend")
                        ModName = "ConsumeProjectSpend";
                    else if (ModuleName == "Pull" || ModuleName == "Consume_Pull" || ModuleName == "WOPullSummary")
                        ModName = "ConsumePull";
                    else if (ModuleName == "Pull Completed" || ModuleName == "Pull Incomplete" || ModuleName == "Pull No Header" || ModuleName == "Pull Summary" || ModuleName == "Pull Summary By ConsignedPO" || ModuleName == "Pull Summary by Quarter")
                        ModName = "Consume_Pull";
                    else if (ModuleName == "Requisition" || ModuleName == "Consume_Requisition"
                             || ModuleName == "Range-Consume_Requisition" || ModuleName == "ReqItemSummary")
                        ModName = "ConsumeRequisition";
                    else if (ModuleName == "Return Order" || ModuleName == "ReturnOrder")
                        ModName = "ReplenishReturnOrder";
                    else if (ModuleName == "Room")
                        ModName = "AdminRoom";
                    else if (ModuleName == "Suggested Orders" || ModuleName == "Cart"
                             || ModuleName == "SuggOrderOfExpDate")
                        ModName = "ReplenishCart";
                    else if (ModuleName == "Tools")
                        ModName = "Tool";
                    else if (ModuleName == "Transfer" || ModuleName == "TransferdItems")
                        ModName = "ReplenishTransfer";
                    else if (ModuleName == "Users")
                        ModName = "AdminUser";
                    else if (ModuleName == "Work Order" || ModuleName == "WorkOrder" || ModuleName == "WorkorderList")
                        ModName = "ConsumeWorkOrder";

                    if (objDefaultReport.SetAsDefaultPrintReport.GetValueOrDefault(false) == true && objDTO.SetAsDefaultPrintReport.GetValueOrDefault(false) == false)
                    {
                        if (!objDTO.ISEnterpriseReport.GetValueOrDefault(false))
                        {
                            objReportDAL.UpdateDefaultReportAfterReportDeleted(objDTO.ID, null, null);
                        }
                        else
                        {
                            objReportDAL.UpdateDefaultReportAfterReportDeleted(objDTO.ID, SessionHelper.CompanyID, SessionHelper.RoomID);
                        }

                    }
                    else if (objDTO.SetAsDefaultPrintReport.GetValueOrDefault(false) == true)
                    {
                        if (SetAsDefaultPrintReportForAllRoom.GetValueOrDefault(false) == true)
                        {
                            if (objDTO.ISEnterpriseReport.GetValueOrDefault(false) == true)
                            {
                                objReportDAL.InsertDefaultReportForAllRoom(objDTO.ID, ModName, 0, 0, 0);
                            }
                            else if (objDTO.ISEnterpriseReport.GetValueOrDefault(false) == false)
                            {
                                objReportDAL.InsertDefaultReportForAllRoom(objDTO.ID, ModName, 0, SessionHelper.CompanyID, SessionHelper.RoomID);
                            }
                        }
                        else
                        {
                            objReportDAL.UpdateDefaultReportBasedonCustomiseReport(objDTO.ID, ModName, SessionHelper.CompanyID, SessionHelper.RoomID);
                        }
                    }
                    else
                    {
                        objReportDAL.UpdateDefaultReportBasedonCustomiseReport(objDTO.ID, ModName, SessionHelper.CompanyID, SessionHelper.RoomID);
                    }
                    #endregion

                    if (objDefaultReport != null
                        && objDefaultReport.ID > 0
                        && !string.IsNullOrWhiteSpace(Saveas)
                        && !string.IsNullOrWhiteSpace(objDefaultReport.SubReportFileName))
                    {
                        string newSubreportFileName = "SubRPT_" + Saveas + ".rdlc";
                        if (!newSubreportFileName.Equals(objDefaultReport.SubReportFileName))
                        {
                            objDTO.SubReportFileName = newSubreportFileName;
                        }
                    }

                    EditReportMaster(objDTO, ModName);
                }


                #endregion

                if ((!string.IsNullOrEmpty(SubReportName)) && (!string.IsNullOrEmpty(strSubrdlXML)))
                {
                    #region "Sub Report"
                    string SubrdlPath = string.Empty;
                    if (string.IsNullOrEmpty(Saveas) || ChildID > 0)
                    {
                        if (Convert.ToBoolean(IsEnterprise))
                        {
                            SubrdlPath = GetFullPath(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/EnterpriseReport") + @"\\" + SubReportName;

                            if (!System.IO.File.Exists(SubrdlPath))
                            {
                                //rdlPath = GetFullPath(Server.MapPath(@"/RDLC_Reports/" + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID)) + @"\\" + ReportName;
                                string rdlpathFrom = GetFullPath(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID) + @"\\" + SubReportName;
                                if (System.IO.File.Exists(rdlpathFrom))
                                    System.IO.File.Copy(rdlpathFrom, SubrdlPath);
                                else
                                {
                                    strmsg = ResReportMaster.SubReportFileDoesNotExist;
                                    return Json(new { Status = false, Message = strmsg }, JsonRequestBehavior.AllowGet);
                                }
                            }

                        }
                        else
                        {
                            SubrdlPath = GetFullPath(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID) + @"\\" + SubReportName;
                            string sourceSubrdlPath = GetFullPath(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/EnterpriseReport") + @"\\" + SubReportName;
                            if (!System.IO.File.Exists(SubrdlPath))
                            {
                                System.IO.File.Copy(sourceSubrdlPath, SubrdlPath);
                            }
                        }
                    }
                    else
                    {
                        SubrdlPath = GetFullPath(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/BaseReport") + @"\\" + SubReportName;
                    }
                    XDocument Subdoc = XDocument.Load(SubrdlPath);
                    XNamespace Subns = XNamespace.Get("http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition");
                    XNamespace Subnsrd = XNamespace.Get("http://schemas.microsoft.com/SQLServer/reporting/reportdesigner");

                    Subdoc.Descendants(ns + "Tablix").First().Value = strSubrdlXML;

                    Subdoc.Root.Element(ns + "Width").Value = (pagewdth + 0.05).ToString("N2") + "in";
                    if (PageType == "2")
                    {
                        if (Subdoc.Descendants(ns + "PageHeight").Count() > 0)
                        {
                            Subdoc.Descendants(ns + "PageHeight").First().Value = "8.5in";
                            Subdoc.Descendants(ns + "PageWidth").First().Value = "11in";
                            // Subdoc.Root.Element(ns + "Width").Value = "10.05in";
                            if (pagewdth < 10.40)
                                Subdoc.Root.Element(ns + "Width").Value = "10.40in";
                        }
                        else
                        {
                            Subdoc.Descendants(ns + "Page").FirstOrDefault().Add(new XElement(ns + "PageHeight", "8.5in"));
                            Subdoc.Descendants(ns + "Page").FirstOrDefault().Add(new XElement(ns + "PageWidth", "11in"));
                            if (pagewdth < 10.40)
                                Subdoc.Root.Element(ns + "Width").Value = "10.40in";
                        }
                    }
                    else if (PageType == "1")
                    {
                        if (Subdoc.Descendants(ns + "PageHeight").Count() > 0)
                        {
                            Subdoc.Descendants(ns + "PageHeight").First().Value = "11in";
                            Subdoc.Descendants(ns + "PageWidth").First().Value = "8.5in";
                            //Subdoc.Root.Element(ns + "Width").Value = "8.00in";
                            if (pagewdth < 7.90)
                                Subdoc.Root.Element(ns + "Width").Value = "07.90in";
                        }
                        else
                        {
                            Subdoc.Descendants(ns + "Page").FirstOrDefault().Add(new XElement(ns + "PageHeight", "11in"));
                            Subdoc.Descendants(ns + "Page").FirstOrDefault().Add(new XElement(ns + "PageWidth", "8.5in"));
                            if (pagewdth < 10.40)
                                Subdoc.Root.Element(ns + "Width").Value = "7.90in";
                        }

                    }
                    string strSubdoc = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
                    strSubdoc += Subdoc.ToString().Replace("&gt;", ">");
                    strSubdoc = strSubdoc.ToString().Replace("&lt;", "<");
                    if (!string.IsNullOrEmpty(Saveas))
                    {
                        if (Convert.ToBoolean(IsEnterprise))
                        {
                            SubrdlPath = GetFullPath(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/EnterpriseReport") + @"\\SubRPT_" + Saveas + ".rdlc";
                        }
                        else
                        {
                            SubrdlPath = GetFullPath(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID) + @"\\SubRPT_" + Saveas + ".rdlc";
                        }
                    }
                    System.IO.File.WriteAllText(SubrdlPath, strSubdoc);
                    #endregion
                }
                strmsg = ResOrder.msgSavedSuccessfully;
            }
            return Json(new { Status = true, Message = strmsg }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ScheduleSave(SchedulerDTO objSchedulerDTO, string ReportIds)
        {
            string[] arrReportId = ReportIds.Split(',');
            objSchedulerDTO.CompanyId = SessionHelper.CompanyID;
            objSchedulerDTO.CreatedBy = SessionHelper.UserID;
            objSchedulerDTO.LastUpdatedBy = SessionHelper.UserID;
            objSchedulerDTO.ScheduledBy = SessionHelper.UserID;
            bool Optype = false;
            if (objSchedulerDTO != null && objSchedulerDTO.ScheduleID == 0)
            {
                Optype = true;
            }

            foreach (string str in arrReportId)
            {
                if (Optype)
                {
                    objSchedulerDTO.ScheduleID = 0;
                }
                SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
                objSchedulerDTO.ReportID = Convert.ToInt64(str);
                objSupplierMasterDAL.SaveSupplierSchedule(objSchedulerDTO);
            }


            return Json(new { message = "ok" });
        }

        public ActionResult ReportScheduleList()
        {
            return View();
        }

        public ActionResult GetUTCStartAndEndDate(string StartDate, string EndDate, string StartTime, string EndTime)
        {
            var startDateUTC = string.Empty;
            var endDateUTC = string.Empty;

            if (StartTime != null && StartTime.Length > 0)
            {
                string[] Hours_Minutes = StartTime.Split(':');
                int TotalSeconds = 0;

                if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                {
                    TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                    TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                }
                startDateUTC = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(StartDate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString("yyyy-MM-dd");
            }
            else
            {
                startDateUTC = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(StartDate, (SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString("yyyy-MM-dd"); //"yyyy-MM-dd HH:mm:ss"
            }

            if (EndTime != null && EndTime.Length > 0)
            {
                string[] Hours_MinutesEndDate = EndTime.Split(':');
                int TotalSecondsEndDate = 0;

                if (Hours_MinutesEndDate != null && Hours_MinutesEndDate.Length > 0)
                {
                    TotalSecondsEndDate = Convert.ToInt32(Hours_MinutesEndDate[0]) * 60 * 60;
                    TotalSecondsEndDate += Convert.ToInt32(Hours_MinutesEndDate[1]) * 60;
                }
                endDateUTC = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(EndDate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(TotalSecondsEndDate), SessionHelper.CurrentTimeZone).ToString("yyyy-MM-dd");
            }
            else
            {
                endDateUTC = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(EndDate, (SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString("yyyy-MM-dd"); //"yyyy-MM-dd HH:mm:ss"
            }

            return Json(new { StartDateUTC = startDateUTC, EndDateUTC = endDateUTC });

        }

        public ActionResult ReportSorting(Int64 ReportID)
        {
            if (ReportID <= 0)
            {
                return PartialView(null);
            }

            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            ReportBuilderDTO objReportBuilderDTO = objReportMasterDAL.GetReportList().Where(x => x.ID == ReportID).FirstOrDefault();
            string ReportFile = objReportBuilderDTO.ReportFileName;
            string Resfile = objReportBuilderDTO.MasterReportResFile;
            ReportPerameters objRptParameters = new ReportPerameters();
            if (objReportBuilderDTO.ReportType == 2)
            {
                ReportFile = objReportBuilderDTO.SubReportFileName;
            }
            string rdlPath = string.Empty;
            string RDLCBaseFilePath = CommonUtility.RDLCBaseFilePath;
            if (objReportBuilderDTO.ParentID > 0)
            {
                if (objReportBuilderDTO.ISEnterpriseReport.GetValueOrDefault(false))
                {
                    rdlPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/EnterpriseReport" + @"\\" + ReportFile;
                }
                else
                {
                    rdlPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID + @"\\" + ReportFile;
                }
            }
            else
            {
                rdlPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/BaseReport" + @"\\" + ReportFile;
            }
            objRptParameters.ReportFileName = ReportFile;


            XDocument doc = XDocument.Load(rdlPath);

            List<XElement> lstFields = doc.Descendants(ns + "Field").ToList();

            List<XElement> lstSortingRows = doc.Descendants(ns + "Tablix").Descendants(ns + "TablixRow").ToList();
            List<XElement> lstSortingFields = lstSortingRows.Count > 1 ? lstSortingRows[1].Descendants(ns + "TablixCell").ToList()
                                                                    : lstSortingRows[0].Descendants(ns + "TablixCell").ToList();

            List<KeyValDTO> lstSortList = new List<KeyValDTO>();
            foreach (XElement cl in lstSortingFields)
            {
                string tdval = string.Empty;
                if (cl.Descendants(ns + "Value").FirstOrDefault() != null
                    && !(string.IsNullOrEmpty(cl.Descendants(ns + "Value").FirstOrDefault().Value.TrimEnd())))
                    tdval = Convert.ToString(cl.Descendants(ns + "Value").FirstOrDefault().Value);

                if (tdval.Contains("=Fields!") && tdval.Contains(".Value") && tdval.Replace("=Fields!", "").Replace(".Value", "") != "Total")
                {
                    KeyValDTO obj = new KeyValDTO();
                    obj.key = tdval.Replace("=Fields!", "").Replace(".Value", "");
                    obj.value = GetField(tdval.Replace("=Fields!", "").Replace(".Value", ""), Resfile);
                    lstSortList.Add(obj);
                }
            }
            //List<string> arrFields = new List<string>();
            //string strFields = string.Empty;


            //foreach (XElement item in lstFields)
            //{

            //}

            lstSortList.Insert(0, new KeyValDTO() { key = string.Empty, value = string.Empty });
            objRptParameters.SortFields = lstSortList;

            List<KeyValDTO> lstSortOrder = new List<KeyValDTO>();
            lstSortOrder.Insert(0, new KeyValDTO() { key = "asc", value = ResReportMaster.Asc });
            lstSortOrder.Insert(1, new KeyValDTO() { key = "desc", value = ResReportMaster.Desc });
            objRptParameters.SortOrders = lstSortOrder;


            if (!string.IsNullOrEmpty(objReportBuilderDTO.SortColumns))
            {
                string[] CurrentSorting = objReportBuilderDTO.SortColumns.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < CurrentSorting.Length; i++)
                {
                    string item = CurrentSorting[i];
                    if (i == 0)
                    {
                        objRptParameters.SortFieldFirstValue = item.Replace(" asc", "").Replace(" desc", "");
                        objRptParameters.SortFieldFirstOrder = "asc";
                        if (item.ToLower().Contains(" desc"))
                        {
                            objRptParameters.SortFieldFirstOrder = "desc";
                        }
                    }
                    else if (i == 1)
                    {
                        objRptParameters.SortFieldSecondValue = item.Replace(" asc", "").Replace(" desc", "");
                        objRptParameters.SortFieldSecondOrder = "asc";
                        if (item.ToLower().Contains(" desc"))
                        {
                            objRptParameters.SortFieldSecondOrder = "desc";
                        }
                    }
                    else if (i == 2)
                    {
                        objRptParameters.SortFieldThirdValue = item.Replace(" asc", "").Replace(" desc", "");
                        objRptParameters.SortFieldThirdOrder = "asc";
                        if (item.ToLower().Contains(" desc"))
                        {
                            objRptParameters.SortFieldThirdOrder = "desc";
                        }
                    }
                    else if (i == 3)
                    {
                        objRptParameters.SortFieldFourthValue = item.Replace(" asc", "").Replace(" desc", "");
                        objRptParameters.SortFieldFourthOrder = "asc";
                        if (item.ToLower().Contains(" desc"))
                        {
                            objRptParameters.SortFieldFourthOrder = "desc";
                        }
                    }
                    else if (i == 4)
                    {
                        objRptParameters.SortFieldFifthValue = item.Replace(" asc", "").Replace(" desc", "");
                        objRptParameters.SortFieldFifthOrder = "asc";
                        if (item.ToLower().Contains(" desc"))
                        {
                            objRptParameters.SortFieldFifthOrder = "desc";
                        }
                    }
                }
            }


            return PartialView(objRptParameters);
        }
        public ActionResult ViewReportPerameters(Int64 ReportID)
        {
            if (ReportID <= 0)
            {
                return PartialView(null);
            }
            ReportPerameters objRptParameters = ReportParameters(ReportID);
            return PartialView(objRptParameters);
        }

        private ReportPerameters ReportParameters(long ReportID)
        {
            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            ReportBuilderDTO objReportBuilderDTO = objReportMasterDAL.GetReportList().Where(x => x.ID == ReportID).FirstOrDefault();



            string ReportFile = objReportBuilderDTO.ReportFileName;
            string SubReportFile = objReportBuilderDTO.SubReportFileName;
            string Resfile = objReportBuilderDTO.MasterReportResFile;
            ReportPerameters objRptParameters = new ReportPerameters();
            objRptParameters.ModuleName = objReportBuilderDTO.ModuleName;

            ReportMasterDTO objParentDTO = objReportMasterDAL.GetParentReportSigleRecord(ReportID, 0, "Report");
            if (objParentDTO != null && objParentDTO.ID > 0)
            {
                objRptParameters.ParentReportName = objParentDTO.ReportName;
            }
            else
            {
                objRptParameters.ParentReportName = objReportBuilderDTO.ReportName;
            }

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
                    rdlSubPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID + @"\\" + SubReportFile;
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

            List<XElement> lstFields = doc.Descendants(ns + "Field").ToList();

            List<XElement> lstSortingRows = new List<XElement>();
            List<XElement> lstSortingFields = new List<XElement>();

            lstSortingRows = doc.Descendants(ns + "Tablix").Descendants(ns + "TablixRow").ToList();

            lstSortingFields = lstSortingRows.Count > 1 ? lstSortingRows[1].Descendants(ns + "TablixCell").ToList()
                                : lstSortingRows[0].Descendants(ns + "TablixCell").ToList();
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
                if (cl.Descendants(ns + "Value").FirstOrDefault() != null
                    && !(string.IsNullOrEmpty(cl.Descendants(ns + "Value").FirstOrDefault().Value.TrimEnd())))
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
                    if (cl.Descendants(ns + "Value").FirstOrDefault() != null
                         && !(string.IsNullOrEmpty(cl.Descendants(ns + "Value").FirstOrDefault().Value.TrimEnd())))
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
            objRptParameters.SortFields = lstSortList;

            List<KeyValDTO> lstSortOrder = new List<KeyValDTO>();
            lstSortOrder.Insert(0, new KeyValDTO() { key = "asc", value = "asc" });
            lstSortOrder.Insert(1, new KeyValDTO() { key = "desc", value = "desc" });
            objRptParameters.SortOrders = lstSortOrder;


            if (!string.IsNullOrEmpty(objReportBuilderDTO.SortColumns))
            {
                string[] CurrentSorting = objReportBuilderDTO.SortColumns.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < CurrentSorting.Length; i++)
                {
                    string item = CurrentSorting[i];
                    if (i == 0)
                    {
                        objRptParameters.SortFieldFirstValue = item.Replace(" asc", "").Replace(" desc", "");
                        objRptParameters.SortFieldFirstOrder = "asc";
                        if (item.ToLower().Contains(" desc"))
                        {
                            objRptParameters.SortFieldFirstOrder = "desc";
                        }
                    }
                    else if (i == 1)
                    {
                        objRptParameters.SortFieldSecondValue = item.Replace(" asc", "").Replace(" desc", "");
                        objRptParameters.SortFieldSecondOrder = "asc";
                        if (item.ToLower().Contains(" desc"))
                        {
                            objRptParameters.SortFieldSecondOrder = "desc";
                        }
                    }
                    else if (i == 2)
                    {
                        objRptParameters.SortFieldThirdValue = item.Replace(" asc", "").Replace(" desc", "");
                        objRptParameters.SortFieldThirdOrder = "asc";
                        if (item.ToLower().Contains(" desc"))
                        {
                            objRptParameters.SortFieldThirdOrder = "desc";
                        }
                    }
                    else if (i == 3)
                    {
                        objRptParameters.SortFieldFourthValue = item.Replace(" asc", "").Replace(" desc", "");
                        objRptParameters.SortFieldFourthOrder = "asc";
                        if (item.ToLower().Contains(" desc"))
                        {
                            objRptParameters.SortFieldFourthOrder = "desc";
                        }
                    }
                    else if (i == 4)
                    {
                        objRptParameters.SortFieldFifthValue = item.Replace(" asc", "").Replace(" desc", "");
                        objRptParameters.SortFieldFifthOrder = "asc";
                        if (item.ToLower().Contains(" desc"))
                        {
                            objRptParameters.SortFieldFifthOrder = "desc";
                        }
                    }
                }
            }

            IEnumerable<XElement> lstParametes = doc.Descendants(ns + "ReportParameter");

            foreach (XElement item in lstParametes)
            {
                if (!string.IsNullOrEmpty(item.Value))
                {
                    if (item.Attribute("Name").Value.ToLower().Contains("startdate"))
                    {
                        objRptParameters.HasStartDate = true;
                    }
                    if (item.Attribute("Name").Value.ToLower().Contains("enddate"))
                    {
                        objRptParameters.HasEndDate = true;
                    }
                    if (item.Value.ToLower().Contains("roomids"))
                    {
                        objRptParameters.HasRoomIds = true;
                        List<KeyValDTO> lstList = new List<KeyValDTO>();

                        if (SessionHelper.UserType <= 2)
                        {
                            EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
                            List<RoomDTO> data = objEnterpriseMasterDAL.GetRoomsByCompany(SessionHelper.CompanyList).OrderBy(t => t.RoomName).ToList();
                            //data = data.Where(t => (t.IsDeleted ?? false) == false).ToList();

                            foreach (RoomDTO room in data)
                            {
                                KeyValDTO obj = new KeyValDTO();
                                obj.key = room.ID.ToString();
                                obj.value = room.RoomName;// +"(" + room.CompanyName + ")";
                                lstList.Add(obj);
                            }
                        }
                        else
                        {
                            foreach (RoomDTO room in SessionHelper.RoomList)
                            {
                                KeyValDTO obj = new KeyValDTO();
                                obj.key = room.ID.ToString();
                                obj.value = room.RoomName;// +"(" + room.CompanyName + ")";
                                lstList.Add(obj);
                            }
                        }
                        objRptParameters.RoomList = lstList;
                        ViewBag.RoomList = lstList;
                    }
                    if (item.Value.ToLower().Contains("companyids"))
                    {
                        objRptParameters.HasCompanyIds = true;

                        objRptParameters.HasRoomIds = true;
                        List<KeyValDTO> lstList = new List<KeyValDTO>();
                        foreach (CompanyMasterDTO comp in SessionHelper.CompanyList)
                        {
                            KeyValDTO obj = new KeyValDTO();
                            obj.key = comp.ID.ToString();
                            obj.value = comp.Name;
                            lstList.Add(obj);
                        }

                        objRptParameters.CompanyList = lstList;
                        ViewBag.CompanyList = lstList;
                    }
                }

            }


            if (objRptParameters.HasStartDate || objRptParameters.HasEndDate)
            {
                RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
                if (objRptParameters.HasStartDate)
                {
                    objRptParameters.StartDate = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                    objRptParameters.StartDateStr = objRptParameters.StartDate.ToString(SessionHelper.RoomDateFormat, SessionHelper.RoomCulture);
                    objRptParameters.StartTime = "00:00";
                }
                if (objRptParameters.HasEndDate)
                {
                    objRptParameters.EndDate = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                    objRptParameters.EndDateStr = objRptParameters.EndDate.ToString(SessionHelper.RoomDateFormat, SessionHelper.RoomCulture);
                    objRptParameters.EndTime = "23:59";
                }
            }

            objRptParameters.IsUserCanDelete = false;
            if ((!objReportBuilderDTO.IsBaseReport) && (IsReportDelete))
            {
                if (objReportBuilderDTO.ISEnterpriseReport.GetValueOrDefault(false) == false)
                {
                    objRptParameters.IsUserCanDelete = true;
                }
                else if (objReportBuilderDTO.ISEnterpriseReport.GetValueOrDefault(false) && SessionHelper.UserType <= 2)
                {
                    objRptParameters.IsUserCanDelete = true;
                }
            }

            if (objReportBuilderDTO.ModuleName.ToLower() == "instockbybin"
                || objReportBuilderDTO.ModuleName.ToLower() == "instockbybinmargin"
                || objReportBuilderDTO.ModuleName.ToLower() == "instockwithqoh"
                || objReportBuilderDTO.ModuleName.ToLower() == "instockbyactivity"
                || objReportBuilderDTO.ModuleName.ToLower() == "precisedemandplanning"
                || objReportBuilderDTO.ModuleName.ToLower() == "precisedemandplanningbyitem")
            {

                DashboardDAL objDashboardDAL = new DashboardDAL(SessionHelper.EnterPriseDBName);
                DashboardParameterDTO objDashboardParameterDTO = new DashboardParameterDTO();
                objDashboardParameterDTO = objDashboardDAL.GetDashboardParameters(SessionHelper.RoomID, SessionHelper.CompanyID);
                if (objDashboardParameterDTO != null)
                {
                    objRptParameters.MonthlyAverageUsage = objDashboardParameterDTO.MonthlyAverageUsage;

                    if (objReportBuilderDTO.ModuleName.ToLower().Equals("PreciseDemandPlanning".ToLower())
                        || objReportBuilderDTO.ModuleName.ToLower().Equals("PreciseDemandPlanningByItem".ToLower()))
                    {
                        objRptParameters.MinMaxDayOfUsageToSample = objDashboardParameterDTO.MinMaxDayOfUsageToSample;
                        objRptParameters.MinMaxMeasureMethod = objDashboardParameterDTO.MinMaxMeasureMethod;
                        objRptParameters.MinMaxDayOfAverage = objDashboardParameterDTO.MinMaxDayOfAverage;
                        objRptParameters.MinMaxMinNumberOfTimesMax = objDashboardParameterDTO.MinMaxMinNumberOfTimesMax;

                        //if (Settinfile.Element("decimalPointFromConfig") != null)
                        //{
                        //    objRptParameters.DecimalPointFromConfig = Convert.ToInt32(Settinfile.Element("decimalPointFromConfig").Value);
                        //}

                        if (SiteSettingHelper.decimalPointFromConfig != string.Empty)
                        {
                            objRptParameters.DecimalPointFromConfig = Convert.ToInt32(SiteSettingHelper.decimalPointFromConfig);
                        }

                        eTurnsRegionInfo objeTurnsRegionInfo = new eTurnsRegionInfo();
                        RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
                        objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(SessionHelper.RoomID, SessionHelper.CompanyID, -1);
                        string DateTimeFormat = "MM/dd/yyyy";
                        if (objeTurnsRegionInfo != null)
                        {
                            DateTimeFormat = objeTurnsRegionInfo.ShortDatePattern + " " + objeTurnsRegionInfo.ShortTimePattern;
                        }

                        DateTime DTNow = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                        DateTime QStartDate, QEndDate;

                        QEndDate = DTNow.AddDays(-1);
                        QStartDate = DTNow.AddDays(-1).AddDays((-objRptParameters.MinMaxDayOfUsageToSample) ?? 0);

                        objRptParameters.QuantumEndDate = QEndDate.Date.AddHours(23).AddMinutes(59).ToString(DateTimeFormat);
                        objRptParameters.QuantumStartDate = QStartDate.Date.ToString(DateTimeFormat);
                    }
                }
                else
                {
                    objRptParameters.MonthlyAverageUsage = 30;
                }
            }
            return objRptParameters;
        }
        public JsonResult ViewReportPerametersData(Int64 ReportID)
        {
            if (ReportID <= 0)
            {
                return null;
            }
            ReportPerameters objRptParameters = ReportParameters(ReportID);
            eTurns.DAL.ReportMasterDAL objReportMasterDAL = new eTurns.DAL.ReportMasterDAL(eTurnsWeb.Helper.SessionHelper.EnterPriseDBName);
            ReportBuilderDTO objReportBuilderDTO = objReportMasterDAL.GetReportDetail(ReportID);

            return Json(new { Data = objRptParameters, ReportBuilder = objReportBuilderDTO, ViewTypes = GetTypesFileterList(objRptParameters.ModuleName), Status = GetFileterList(objRptParameters.ModuleName), WorkOrderStatus = GetFileterList("RequisitionWorkOrderStatus") }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ViewStatusPerameters(string ReportModuleName)
        {
            List<ReportFilterParam> lstReportFilterParam = new List<ReportFilterParam>();
            ReportFilterParam ReportFilterParam = new ReportFilterParam();
            ReportFilterParam.FieldDisplayName = ResReportMaster.Status;
            ReportFilterParam.FieldName = "Status";
            ReportFilterParam.FieldFilterName = GetFileterList(ReportModuleName);
            //ViewBag.FieldName = "";
            lstReportFilterParam.Add(ReportFilterParam);

            if ((ReportModuleName ?? string.Empty).ToLower() == "consume_requisition" || (ReportModuleName ?? string.Empty).ToLower() == "range-consume_requisition")
            {
                ReportFilterParam ReportFilterParamWO = new ReportFilterParam();
                ReportFilterParamWO.FieldName = "Work Order Status";
                //ViewBag.FieldName = "Work Order Status";
                ReportFilterParamWO.FieldDisplayName = ResReportMaster.WorkOrderStatus;
                ReportFilterParamWO.FieldFilterName = GetFileterList("RequisitionWorkOrderStatus");
                lstReportFilterParam.Add(ReportFilterParamWO);
            }

            ViewBag.SattusParam = lstReportFilterParam;
            return PartialView(lstReportFilterParam);
        }

        public ActionResult ViewTypesPerameters(string ReportModuleName)
        {
            List<ReportFilterParam> lstReportFilterParam = new List<ReportFilterParam>();
            ReportFilterParam ReportFilterParam = new ReportFilterParam();
            ReportFilterParam.FieldName = ResReportMaster.Types;
            ReportFilterParam.FieldFilterName = GetTypesFileterList(ReportModuleName);
            lstReportFilterParam.Add(ReportFilterParam);
            ViewBag.SattusParam = lstReportFilterParam;
            return PartialView(lstReportFilterParam);
        }

        public List<KeyValCheckDTO> GetFileterList(string ReportModuleName)
        {
            List<KeyValCheckDTO> lstKeyValCheckDTO = new List<KeyValCheckDTO>();
            KeyValCheckDTO objKeyValCheckDTO = new KeyValCheckDTO();
            int ResRead = !string.IsNullOrWhiteSpace(SiteSettingHelper.ResourceRead) ? Convert.ToInt32(SiteSettingHelper.ResourceRead) : (int)ResourceReadType.File;
            switch (ReportModuleName)
            {

                case "WorkOrder":
                    objKeyValCheckDTO = new KeyValCheckDTO();
                    objKeyValCheckDTO.key = "Open";
                    objKeyValCheckDTO.value = ResWorkOrder.Open;
                    objKeyValCheckDTO.chkvalue = true;
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);

                    objKeyValCheckDTO = new KeyValCheckDTO();
                    objKeyValCheckDTO.key = "Close";
                    objKeyValCheckDTO.value = ResWorkOrder.Close;
                    objKeyValCheckDTO.chkvalue = false;
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);
                    break;
                case "RequisitionWorkOrderStatus":
                    objKeyValCheckDTO = new KeyValCheckDTO();
                    objKeyValCheckDTO.key = "Open";
                    objKeyValCheckDTO.value = ResWorkOrder.Open;
                    objKeyValCheckDTO.chkvalue = false;
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);

                    objKeyValCheckDTO = new KeyValCheckDTO();
                    objKeyValCheckDTO.key = "Close";
                    objKeyValCheckDTO.value = ResWorkOrder.Close;
                    objKeyValCheckDTO.chkvalue = false;
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);
                    break;
                case "Order":
                case "ReturnOrder":
                case "Replenish_Order":
                    foreach (var item in Enum.GetValues(typeof(OrderStatus)))
                    {
                        objKeyValCheckDTO = new KeyValCheckDTO();
                        string itemText = item.ToString();
                        if (ResRead == (int)eTurns.DTO.ResourceReadType.File)
                            objKeyValCheckDTO.value = ResourceHelper.GetResourceValue(itemText, "ResOrder");
                        else
                            objKeyValCheckDTO.value = ResourceModuleHelper.GetResourceValue(itemText, "ResOrder");
                        objKeyValCheckDTO.key = Convert.ToString((int)(Enum.Parse(typeof(OrderStatus), itemText)));
                        if ((int)(Enum.Parse(typeof(OrderStatus), itemText)) != 8)
                        {
                            objKeyValCheckDTO.chkvalue = true;
                        }
                        lstKeyValCheckDTO.Add(objKeyValCheckDTO);
                    }
                    break;
                case "Quote":
                    foreach (var item in Enum.GetValues(typeof(QuoteStatus)))
                    {
                        objKeyValCheckDTO = new KeyValCheckDTO();
                        string itemText = item.ToString();
                        if (ResRead == (int)eTurns.DTO.ResourceReadType.File)
                            objKeyValCheckDTO.value = ResourceHelper.GetResourceValue(itemText, "ResQuoteMaster");
                        else
                            objKeyValCheckDTO.value = ResourceModuleHelper.GetResourceValue(itemText, "ResQuoteMaster");
                        objKeyValCheckDTO.key = Convert.ToString((int)(Enum.Parse(typeof(QuoteStatus), itemText)));
                        if ((int)(Enum.Parse(typeof(OrderStatus), itemText)) != 8)
                        {
                            objKeyValCheckDTO.chkvalue = true;
                        }
                        lstKeyValCheckDTO.Add(objKeyValCheckDTO);
                    }
                    break;
                case "ToolAssetOrder":
                case "Tool_ToolAssetOrder":

                    foreach (var item in Enum.GetValues(typeof(OrderStatus)))
                    {
                        objKeyValCheckDTO = new KeyValCheckDTO();
                        string itemText = item.ToString();
                        if (ResRead == (int)eTurns.DTO.ResourceReadType.File)
                            objKeyValCheckDTO.value = ResourceHelper.GetResourceValue(itemText, "ResToolAssetOrder");
                        else
                            objKeyValCheckDTO.value = ResourceModuleHelper.GetResourceValue(itemText, "ResToolAssetOrder");
                        objKeyValCheckDTO.key = Convert.ToString((int)(Enum.Parse(typeof(ToolAssetOrderStatus), itemText)));
                        if ((int)(Enum.Parse(typeof(ToolAssetOrderStatus), itemText)) != 8)
                        {
                            objKeyValCheckDTO.chkvalue = true;
                        }
                        lstKeyValCheckDTO.Add(objKeyValCheckDTO);
                    }
                    break;
                case "Receive":
                    foreach (var item in Enum.GetValues(typeof(OrderStatus)))
                    {
                        if ((int)item > 3)
                        {
                            objKeyValCheckDTO = new KeyValCheckDTO();
                            string itemText = item.ToString();
                            if (ResRead == (int)eTurns.DTO.ResourceReadType.File)
                                objKeyValCheckDTO.value = ResourceHelper.GetResourceValue(itemText, "ResOrder");
                            else
                                objKeyValCheckDTO.value = ResourceModuleHelper.GetResourceValue(itemText, "ResOrder");
                            objKeyValCheckDTO.key = Convert.ToString((int)(Enum.Parse(typeof(OrderStatus), itemText)));
                            if ((int)(Enum.Parse(typeof(OrderStatus), itemText)) != 8)
                            {
                                objKeyValCheckDTO.chkvalue = true;
                            }
                            lstKeyValCheckDTO.Add(objKeyValCheckDTO);
                        }
                    }
                    break;
                case "Consume_Requisition":

                    objKeyValCheckDTO = new KeyValCheckDTO();
                    objKeyValCheckDTO.value = ResRequisitionMaster.Unsubmitted;
                    objKeyValCheckDTO.key = "Unsubmitted";
                    objKeyValCheckDTO.chkvalue = true;
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);

                    objKeyValCheckDTO = new KeyValCheckDTO();
                    objKeyValCheckDTO.value = ResRequisitionMaster.Submitted;
                    objKeyValCheckDTO.key = "Submitted";
                    objKeyValCheckDTO.chkvalue = true;
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);


                    objKeyValCheckDTO = new KeyValCheckDTO();
                    objKeyValCheckDTO.value = ResRequisitionMaster.Closed;
                    objKeyValCheckDTO.key = "Closed";
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);

                    objKeyValCheckDTO = new KeyValCheckDTO();
                    objKeyValCheckDTO.value = ResRequisitionMaster.Approved;
                    objKeyValCheckDTO.key = "Approved";
                    objKeyValCheckDTO.chkvalue = true;
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);

                    objKeyValCheckDTO = new KeyValCheckDTO();
                    objKeyValCheckDTO.value = ResRequisitionMaster.PartialCheckOut;
                    objKeyValCheckDTO.key = "PartialCheckOut";
                    objKeyValCheckDTO.chkvalue = true;
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);

                    objKeyValCheckDTO = new KeyValCheckDTO();
                    objKeyValCheckDTO.value = ResRequisitionMaster.FullCheckOut;
                    objKeyValCheckDTO.key = "FullCheckOut";
                    objKeyValCheckDTO.chkvalue = true;
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);

                    break;
                case "Range-Consume_Requisition":

                    objKeyValCheckDTO = new KeyValCheckDTO();
                    objKeyValCheckDTO.value = ResRequisitionMaster.Unsubmitted;
                    objKeyValCheckDTO.key = "Unsubmitted";
                    objKeyValCheckDTO.chkvalue = true;
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);

                    objKeyValCheckDTO = new KeyValCheckDTO();
                    objKeyValCheckDTO.value = ResRequisitionMaster.Submitted;
                    objKeyValCheckDTO.key = "Submitted";
                    objKeyValCheckDTO.chkvalue = true;
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);


                    objKeyValCheckDTO = new KeyValCheckDTO();
                    objKeyValCheckDTO.value = ResRequisitionMaster.Closed;
                    objKeyValCheckDTO.key = "Closed";
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);

                    objKeyValCheckDTO = new KeyValCheckDTO();
                    objKeyValCheckDTO.value = ResRequisitionMaster.Approved;
                    objKeyValCheckDTO.key = "Approved";
                    objKeyValCheckDTO.chkvalue = true;
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);

                    objKeyValCheckDTO = new KeyValCheckDTO();
                    objKeyValCheckDTO.value = ResRequisitionMaster.PartialCheckOut;
                    objKeyValCheckDTO.key = "PartialCheckOut";
                    objKeyValCheckDTO.chkvalue = true;
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);

                    objKeyValCheckDTO = new KeyValCheckDTO();
                    objKeyValCheckDTO.value = ResRequisitionMaster.FullCheckOut;
                    objKeyValCheckDTO.key = "FullCheckOut";
                    objKeyValCheckDTO.chkvalue = true;
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);
                    break;
                case "ProjectSpend":
                    objKeyValCheckDTO = new KeyValCheckDTO();
                    objKeyValCheckDTO.value = ResRoleMaster.All;
                    objKeyValCheckDTO.key = Convert.ToString((int)RPT_PSStatus.All);
                    objKeyValCheckDTO.chkvalue = true;
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);

                    objKeyValCheckDTO = new KeyValCheckDTO();
                    objKeyValCheckDTO.value = ResReportMaster.ProjectUsedLessThanLimit;
                    objKeyValCheckDTO.key = Convert.ToString((int)RPT_PSStatus.ProjDollarUsedLessDollarLimit);
                    objKeyValCheckDTO.chkvalue = false;
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);

                    objKeyValCheckDTO = new KeyValCheckDTO();
                    objKeyValCheckDTO.value = ResReportMaster.ProjectUsedMoreThanLimit;
                    objKeyValCheckDTO.key = Convert.ToString((int)RPT_PSStatus.ProjDollarUsedGreaterDollarLimit);
                    objKeyValCheckDTO.chkvalue = false;
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);

                    objKeyValCheckDTO = new KeyValCheckDTO();
                    objKeyValCheckDTO.value = ResWorkOrder.Open;
                    objKeyValCheckDTO.key = Convert.ToString((int)RPT_PSStatus.Open);
                    objKeyValCheckDTO.chkvalue = false;
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);

                    objKeyValCheckDTO = new KeyValCheckDTO();
                    objKeyValCheckDTO.value = ResRequisitionMaster.Closed;
                    objKeyValCheckDTO.key = Convert.ToString((int)RPT_PSStatus.Closed);
                    objKeyValCheckDTO.chkvalue = false;
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);

                    objKeyValCheckDTO = new KeyValCheckDTO();
                    objKeyValCheckDTO.value = ResReportMaster.ItemQtyUsedLessThanLimit;
                    objKeyValCheckDTO.key = Convert.ToString((int)RPT_PSStatus.ItemQtyUsedLessQtyLimit);
                    objKeyValCheckDTO.chkvalue = false;
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);

                    objKeyValCheckDTO = new KeyValCheckDTO();
                    objKeyValCheckDTO.value = ResReportMaster.ItemQtyUsedMoreThanLimit;
                    objKeyValCheckDTO.key = Convert.ToString((int)RPT_PSStatus.ItemQtyUsedGreaterQtyLimit);
                    objKeyValCheckDTO.chkvalue = false;
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);

                    objKeyValCheckDTO = new KeyValCheckDTO();
                    objKeyValCheckDTO.value = ResReportMaster.ItemUsedLessThanLimit;
                    objKeyValCheckDTO.key = Convert.ToString((int)RPT_PSStatus.ItemDollarUsedLessDollarLimit);
                    objKeyValCheckDTO.chkvalue = false;
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);

                    objKeyValCheckDTO = new KeyValCheckDTO();
                    objKeyValCheckDTO.value = ResReportMaster.ItemUsedMoreThanLimit;
                    objKeyValCheckDTO.key = Convert.ToString((int)RPT_PSStatus.ItemDollarUsedGreaterDollarLimit);
                    objKeyValCheckDTO.chkvalue = false;
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);

                    break;
                case "Transfer":
                case "TransferdItems":
                    foreach (var item in Enum.GetValues(typeof(TransferStatus)))
                    {
                        objKeyValCheckDTO = new KeyValCheckDTO();
                        string itemText = item.ToString();
                        if (itemText != "Rejected")
                        {
                            if (ResRead == (int)eTurns.DTO.ResourceReadType.File)
                                objKeyValCheckDTO.value = ResourceHelper.GetResourceValue(itemText, "ResTransfer");
                            else
                                objKeyValCheckDTO.value = ResourceModuleHelper.GetResourceValue(itemText, "ResTransfer");
                            objKeyValCheckDTO.key = Convert.ToString((int)(Enum.Parse(typeof(TransferStatus), itemText)));
                            if ((int)(Enum.Parse(typeof(TransferStatus), itemText)) != 9)
                            {
                                objKeyValCheckDTO.chkvalue = true;
                            }
                            lstKeyValCheckDTO.Add(objKeyValCheckDTO);
                        }
                    }
                    break;

                case "InStockByBin":
                case "InStockByActivity":
                case "InStockByBinMargin":
                case "InStockWithQOH":
                case "PreciseDemandPlanning":
                case "PreciseDemandPlanningByItem":
                    objKeyValCheckDTO = new KeyValCheckDTO();
                    if (ResRead == (int)eTurns.DTO.ResourceReadType.File)
                        objKeyValCheckDTO.value = ResourceHelper.GetResourceValue("QOHBelowCritical", "ResItemMaster");
                    else
                        objKeyValCheckDTO.value = ResourceModuleHelper.GetResourceValue("QOHBelowCritical", "ResItemMaster");

                    //objKeyValCheckDTO.value = "QOH Below Critical";
                    objKeyValCheckDTO.key = "QOH1";
                    objKeyValCheckDTO.chkvalue = false;
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);

                    objKeyValCheckDTO = new KeyValCheckDTO();
                    if (ResRead == (int)eTurns.DTO.ResourceReadType.File)
                        objKeyValCheckDTO.value = ResourceHelper.GetResourceValue("QOHBelowMinimum", "ResItemMaster");
                    else
                        objKeyValCheckDTO.value = ResourceModuleHelper.GetResourceValue("QOHBelowMinimum", "ResItemMaster");

                    //objKeyValCheckDTO.value = "QOH Below Minimum";
                    objKeyValCheckDTO.key = "QOH2";
                    objKeyValCheckDTO.chkvalue = false;
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);


                    objKeyValCheckDTO = new KeyValCheckDTO();
                    if (ResRead == (int)eTurns.DTO.ResourceReadType.File)
                        objKeyValCheckDTO.value = ResourceHelper.GetResourceValue("QOHBelowMaximum", "ResItemMaster");
                    else
                        objKeyValCheckDTO.value = ResourceModuleHelper.GetResourceValue("QOHBelowMaximum", "ResItemMaster");

                    //objKeyValCheckDTO.value = "QOH Below Maxmimum";
                    objKeyValCheckDTO.key = "QOH3";
                    objKeyValCheckDTO.chkvalue = false;
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);

                    objKeyValCheckDTO = new KeyValCheckDTO();
                    if (ResRead == (int)eTurns.DTO.ResourceReadType.File)
                        objKeyValCheckDTO.value = ResourceHelper.GetResourceValue("QOHAboveMaximum", "ResItemMaster");
                    else
                        objKeyValCheckDTO.value = ResourceModuleHelper.GetResourceValue("QOHAboveMaximum", "ResItemMaster");

                    //objKeyValCheckDTO.value = "QOH Above Maximum";
                    objKeyValCheckDTO.key = "QOH4";
                    objKeyValCheckDTO.chkvalue = false;
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);



                    break;

            }
            return lstKeyValCheckDTO;
        }

        public List<KeyValCheckDTO> GetFileterListForOrderStatus(string ReportModuleName)
        {
            int ResRead = !string.IsNullOrWhiteSpace(SiteSettingHelper.ResourceRead) ? Convert.ToInt32(SiteSettingHelper.ResourceRead) : (int)ResourceReadType.File;

            List<KeyValCheckDTO> lstKeyValCheckDTO = new List<KeyValCheckDTO>();
            KeyValCheckDTO objKeyValCheckDTO = new KeyValCheckDTO();
            switch (ReportModuleName)
            {
                //case "Order":
                //    objKeyValCheckDTO = new KeyValCheckDTO();
                //    objKeyValCheckDTO.key = "Open";
                //    objKeyValCheckDTO.value = ResWorkOrder.Open;
                //    objKeyValCheckDTO.chkvalue = true;
                //    lstKeyValCheckDTO.Add(objKeyValCheckDTO);

                //    objKeyValCheckDTO = new KeyValCheckDTO();
                //    objKeyValCheckDTO.key = "Close";
                //    objKeyValCheckDTO.value = ResWorkOrder.Close;
                //    objKeyValCheckDTO.chkvalue = false;
                //    lstKeyValCheckDTO.Add(objKeyValCheckDTO);
                //    break;
                case "Order":
                case "Replenish_Order":
                    foreach (var item in Enum.GetValues(typeof(OrderStatus)))
                    {
                        objKeyValCheckDTO = new KeyValCheckDTO();
                        string itemText = item.ToString();
                        if (ResRead == (int)eTurns.DTO.ResourceReadType.File)
                            objKeyValCheckDTO.value = ResourceHelper.GetResourceValue(itemText, "ResOrder");
                        else
                            objKeyValCheckDTO.value = ResourceModuleHelper.GetResourceValue(itemText, "ResOrder");
                        objKeyValCheckDTO.key = Convert.ToString((int)(Enum.Parse(typeof(OrderStatus), itemText)));
                        if ((int)(Enum.Parse(typeof(OrderStatus), itemText)) != 8)
                        {
                            objKeyValCheckDTO.chkvalue = true;
                        }
                        lstKeyValCheckDTO.Add(objKeyValCheckDTO);
                    }
                    break;
            }
            return lstKeyValCheckDTO;
        }

        public List<KeyValCheckDTO> GetTypesFileterList(string ReportModuleName)
        {
            List<KeyValCheckDTO> lstKeyValCheckDTO = new List<KeyValCheckDTO>();
            KeyValCheckDTO objKeyValCheckDTO = new KeyValCheckDTO();
            switch (ReportModuleName)
            {

                case "WorkOrder":
                    objKeyValCheckDTO = new KeyValCheckDTO();
                    objKeyValCheckDTO.key = "Tool Service";
                    objKeyValCheckDTO.value = ResWorkOrder.ToolService;
                    objKeyValCheckDTO.chkvalue = false;
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);

                    objKeyValCheckDTO = new KeyValCheckDTO();
                    objKeyValCheckDTO.key = "Asset Service";
                    objKeyValCheckDTO.value = ResWorkOrder.AssetService;
                    objKeyValCheckDTO.chkvalue = false;
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);

                    objKeyValCheckDTO = new KeyValCheckDTO();
                    objKeyValCheckDTO.key = "Reqn";
                    objKeyValCheckDTO.value = ResWorkOrder.Requisition;
                    objKeyValCheckDTO.chkvalue = true;
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);

                    objKeyValCheckDTO = new KeyValCheckDTO();
                    objKeyValCheckDTO.key = "WorkOrder";
                    objKeyValCheckDTO.value = ResWorkOrder.WorkOrderType;
                    objKeyValCheckDTO.chkvalue = true;
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);
                    break;
                case "TransferdItems":
                case "Transfer":
                    objKeyValCheckDTO = new KeyValCheckDTO();
                    objKeyValCheckDTO.key = "In";
                    objKeyValCheckDTO.value = ResTransfer.In;
                    objKeyValCheckDTO.chkvalue = true;
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);

                    objKeyValCheckDTO = new KeyValCheckDTO();
                    objKeyValCheckDTO.key = "Out";
                    objKeyValCheckDTO.value = ResTransfer.Out;
                    objKeyValCheckDTO.chkvalue = true;
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);
                    break;
                case "Consume_Requisition":
                case "Range-Consume_Requisition":
                    objKeyValCheckDTO = new KeyValCheckDTO();
                    objKeyValCheckDTO.key = "Tool Service";
                    objKeyValCheckDTO.value = ResRequisitionMaster.ToolService;
                    objKeyValCheckDTO.chkvalue = false;
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);

                    objKeyValCheckDTO = new KeyValCheckDTO();
                    objKeyValCheckDTO.key = "Asset Service";
                    objKeyValCheckDTO.value = ResRequisitionMaster.AssetService;
                    objKeyValCheckDTO.chkvalue = false;
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);

                    objKeyValCheckDTO = new KeyValCheckDTO();
                    objKeyValCheckDTO.key = "Requisition";
                    objKeyValCheckDTO.value = ResRequisitionMaster.Requisition;
                    objKeyValCheckDTO.chkvalue = true;
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);

                    break;

                case "MoveMaterial":
                    objKeyValCheckDTO = new KeyValCheckDTO();
                    objKeyValCheckDTO.key = Convert.ToString(Convert.ToInt32(MoveType.InvToInv));
                    objKeyValCheckDTO.value = ResMoveMaterial.MoveTypeItemInvtoInv;
                    objKeyValCheckDTO.chkvalue = false;
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);

                    objKeyValCheckDTO = new KeyValCheckDTO();
                    objKeyValCheckDTO.key = Convert.ToString(Convert.ToInt32(MoveType.InvToStag));
                    objKeyValCheckDTO.value = ResMoveMaterial.MoveTypeItemInvtoStage;
                    objKeyValCheckDTO.chkvalue = false;
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);

                    objKeyValCheckDTO = new KeyValCheckDTO();
                    objKeyValCheckDTO.key = Convert.ToString(Convert.ToInt32(MoveType.StagToInv));
                    objKeyValCheckDTO.value = ResMoveMaterial.MoveTypeItemStageToInv;
                    objKeyValCheckDTO.chkvalue = false;
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);

                    objKeyValCheckDTO = new KeyValCheckDTO();
                    objKeyValCheckDTO.key = Convert.ToString(Convert.ToInt32(MoveType.StagToStag));
                    objKeyValCheckDTO.value = ResMoveMaterial.MoveTypeItemStageToStage;
                    objKeyValCheckDTO.chkvalue = false;
                    lstKeyValCheckDTO.Add(objKeyValCheckDTO);

                    break;

                case "Room":

                    BillingRoomTypeMasterBAL billingRoomTypeMasterBAL = new BillingRoomTypeMasterBAL();
                    var billingRoomTypes = billingRoomTypeMasterBAL.GetBillingRoomTypeMaster(SessionHelper.EnterPriceID);
                    foreach (var item in billingRoomTypes)
                    {
                            objKeyValCheckDTO = new KeyValCheckDTO();
                            objKeyValCheckDTO.key = Convert.ToString(item.ID);
                            objKeyValCheckDTO.value = Convert.ToString(item.ResourceValue);
                            objKeyValCheckDTO.chkvalue = false;
                            lstKeyValCheckDTO.Add(objKeyValCheckDTO);
                    }
                    break;

            }
            return lstKeyValCheckDTO;
        }

        public void EditReportMaster(ReportBuilderDTO objDTO, string ModuleName)
        {
            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            if (objDTO.ID > 0)
            {
                objReportMasterDAL.EditReportMaster(objDTO, ModuleName);
            }
        }
        public string GetField(string Key, string FileName)
        {
            string KeyVal = string.Empty;
            int ResRead = !string.IsNullOrWhiteSpace(SiteSettingHelper.ResourceRead) ? Convert.ToInt32(SiteSettingHelper.ResourceRead) : (int)ResourceReadType.File;
            //if (!string.IsNullOrEmpty(Key) && Key.ToLower().Contains("itemudf"))
            //{
            //    KeyVal = ResourceHelper.GetResourceValue(Key.Replace("Item", ""), "ResItemMaster");
            //    if (KeyVal == Key)
            //    {
            //        KeyVal = ResourceHelper.GetResourceValue(Key, FileName);
            //    }
            //}
            //else if (!string.IsNullOrEmpty(Key) && Key.ToLower().Contains("pulludf"))
            //{
            //    KeyVal = ResourceHelper.GetResourceValue(Key.Replace("Pull", ""), "ResPullMaster");
            //    if (KeyVal == Key)
            //    {
            //        KeyVal = ResourceHelper.GetResourceValue(Key, FileName);
            //    }
            //}
            //else if (!string.IsNullOrEmpty(Key) && Key.ToLower().Contains("woudf"))
            //{
            //    KeyVal = ResourceHelper.GetResourceValue(Key.Replace("WO", ""), "ResWorkOrder");
            //    if (KeyVal == Key)
            //    {
            //        KeyVal = ResourceHelper.GetResourceValue(Key, FileName);
            //    }
            //}
            //else if (!string.IsNullOrEmpty(Key) && Key.ToLower().Contains("orderudf"))
            //{
            //    KeyVal = ResourceHelper.GetResourceValue(Key.Replace("Order", ""), "ResOrder");
            //    if (KeyVal == Key)
            //    {
            //        KeyVal = ResourceHelper.GetResourceValue(Key, FileName);
            //    }
            //}
            //else
            if (Key.ToLower().Contains("udf"))
            {
                //KeyVal = ResourceHelper.GetResourceValue(Key, FileName, true);
                KeyVal = GetPrefixField(Key, FileName);
            }
            else
            {
                if (ResRead == (int)eTurns.DTO.ResourceReadType.File)
                    KeyVal = ResourceHelper.GetResourceValue(Key, FileName);
                else if (ResRead == (int)eTurns.DTO.ResourceReadType.Database)
                    KeyVal = ResourceModuleHelper.GetResourceValue(Key, FileName);
            }

            return KeyVal;
        }
        public string GetPrefixField(string Key, string FileName)
        {
            string KeyVal = string.Empty;
            int ResRead = !string.IsNullOrWhiteSpace(SiteSettingHelper.ResourceRead) ? Convert.ToInt32(SiteSettingHelper.ResourceRead) : (int)ResourceReadType.File;

            if (Key.ToLower().Contains("udf"))
            {
                string PreFixValue = Key.Replace("UDF10", string.Empty).Replace("UDF1", string.Empty).Replace("UDF2", string.Empty).Replace("UDF3", string.Empty).Replace("UDF4", string.Empty).Replace("UDF5", string.Empty).Replace("UDF6", string.Empty).Replace("UDF7", string.Empty).Replace("UDF8", string.Empty).Replace("UDF9", string.Empty);
                if (ResRead == (int)eTurns.DTO.ResourceReadType.File)
                    KeyVal = ResourceHelper.GetResourceValue(Key, FileName, true);
                else
                    KeyVal = ResourceModuleHelper.GetResourceValue(Key, FileName, true);
                if (!KeyVal.Contains(PreFixValue))
                    KeyVal = PreFixValue + " " + KeyVal;
            }
            else
            {
                if (ResRead == (int)eTurns.DTO.ResourceReadType.File)
                    KeyVal = ResourceHelper.GetResourceValue(Key, FileName);
                else
                    KeyVal = ResourceModuleHelper.GetResourceValue(Key, FileName);

            }

            return KeyVal;
        }
        public string ConvertInchToPx(string inch)
        {
            double retPx = 0;
            double inchdb = Convert.ToDouble(inch.ToLower().Replace("in", "").Replace("px", ""));
            retPx = inchdb * 96;
            return (Convert.ToString(retPx) + "px");
        }
        public JsonResult GetTablixRowHierarchy(string oldRowHierarchy, int Rowcnt)
        {
            string RowHierarchy = string.Empty;

            XmlDocument xd = new XmlDocument();
            try
            {



                xd.LoadXml(oldRowHierarchy);
                if (Rowcnt > 0)
                {
                    string TablixMember = string.Empty;
                    for (int i = 0; i < Rowcnt; i++)
                    {
                        TablixMember += "<TablixMember />";
                    }
                    xd.SelectNodes("/TablixRowHierarchy/TablixMembers/TablixMember/TablixMembers").Item(0).InnerXml = TablixMember;
                }
            }
            catch (Exception)
            {


            }
            return Json(new { Status = true, Message = xd.InnerXml }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateChildEntReport(bool OverwriteExisting, bool OverwirteReportField, string SelectedEnterprise, string SelectedReport)
        {
            if (SessionHelper.UserType != 1)
            {
                return Json(new { Status = false, Message = ResReportMaster.YouAreNotAbleToUpdate }, JsonRequestBehavior.AllowGet);
            }
            string msg = string.Empty;
            EnterpriseMasterDAL objDAL = new EnterpriseMasterDAL();
            List<EnterpriseDTO> lstEnterprise = new List<EnterpriseDTO>();
            if (!string.IsNullOrWhiteSpace(SelectedEnterprise))
            {
                //List<long> entIDs = SelectedEnterprise.Split(',').Select(long.Parse).ToList();
                lstEnterprise = objDAL.GetEnterprisesByIds(SelectedEnterprise);
            }
            else
            {
                lstEnterprise = objDAL.GetAllEnterprisesPlain();
            }
            List<ReportMasterDTO> lstReportMaster = new List<ReportMasterDTO>();
            ReportMasterDAL objReportDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            if (!string.IsNullOrWhiteSpace(SelectedReport))
            {
                List<long> reportIDs = SelectedReport.Split(',').Select(long.Parse).ToList();
                lstReportMaster = objReportDAL.GetAllBaseReport(SessionHelper.RoomID, eTurnsWeb.Helper.SessionHelper.CompanyID, false, false).Where(x => reportIDs.Contains(x.ID)).ToList();
            }

            foreach (EnterpriseDTO item in lstEnterprise)
            {
                try
                {
                    if (objDAL.UpdateRDLCReportMaster(DbConnectionHelper.GetETurnsMasterDBName(), item.EnterpriseDBName, SessionHelper.UserID, OverwriteExisting))
                    {
                        string BaseReportPath = string.Empty;
                        string EntReportPath = string.Empty;
                        BaseReportPath = ResourceHelper.RDLReportDirectoryBasePath + @"\MasterReport";
                        EntReportPath = ResourceHelper.RDLReportDirectoryBasePath + @"\" + item.ID + @"\" + @"\BaseReport";
                        if (System.IO.Directory.Exists(BaseReportPath))
                        {
                            if (!System.IO.Directory.Exists(EntReportPath))
                            {
                                System.IO.Directory.CreateDirectory(EntReportPath);
                            }

                            var fileList = System.IO.Directory.GetFiles(BaseReportPath);
                            if (fileList != null && fileList.Count() > 0 && lstReportMaster != null && lstReportMaster.Count > 0)
                            {
                                List<string> reportFileNames = lstReportMaster.Select(x => (x.ReportFileName ?? string.Empty).ToLower()).ToList();
                                List<string> subReportFileNames = lstReportMaster.Select(x => (x.SubReportFileName ?? string.Empty).ToLower()).ToList();
                                fileList = fileList.Where(x => reportFileNames.Contains(System.IO.Path.GetFileName((x ?? string.Empty).ToLower())) || subReportFileNames.Contains(System.IO.Path.GetFileName((x ?? string.Empty).ToLower()))).ToArray();
                            }

                            foreach (var file in fileList)
                            {
                                try
                                {

                                    string Destfilepath = EntReportPath + @"\" + System.IO.Path.GetFileName(file);
                                    if (!System.IO.File.Exists(Destfilepath))
                                    {
                                        System.IO.File.Copy(file, System.IO.Path.Combine(EntReportPath, System.IO.Path.GetFileName(file)));
                                    }
                                    else
                                    {
                                        if (OverwriteExisting)
                                        {
                                            System.IO.File.Delete(Destfilepath);
                                            System.IO.File.Copy(file, System.IO.Path.Combine(EntReportPath, System.IO.Path.GetFileName(file)));
                                        }
                                    }

                                }
                                catch (Exception exInner)
                                {
                                    msg += (item.EnterpriseDBName + " " + ResCommon.ErrorColon + " " + exInner.ToString());
                                }
                            }
                            if (OverwirteReportField)
                            {
                                msg += UpdateAllReportsFields(item, lstReportMaster);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    msg += (item.EnterpriseDBName + " " + ResCommon.ErrorColon + " " + ex.ToString());
                }
            }

            msg += ResCommon.MsgUpdatedSuccessfully;
            return Json(new { Status = true, Message = msg }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateChildEntReportForSingle(bool OverwriteExisting, EnterpriseDTO item)
        {
            if (SessionHelper.UserType != 1)
            {
                return Json(new { Status = false, Message = ResReportMaster.YouAreNotAbleToUpdate }, JsonRequestBehavior.AllowGet);
            }
            string msg = string.Empty;
            EnterpriseMasterDAL objDAL = new EnterpriseMasterDAL();
            //List<EnterpriseDTO> lstEnterprise = new List<EnterpriseDTO>();
            //lstEnterprise = objDAL.GetAllEnterprise(false);
            //foreach (EnterpriseDTO item in lstEnterprise)
            //{
            if (item != null)
            {
                if (objDAL.UpdateRDLCReportMaster(DbConnectionHelper.GetETurnsMasterDBName(), item.EnterpriseDBName, SessionHelper.UserID, OverwriteExisting))
                {
                    string BaseReportPath = string.Empty;
                    string EntReportPath = string.Empty;
                    BaseReportPath = ResourceHelper.RDLReportDirectoryBasePath + @"\MasterReport";
                    EntReportPath = ResourceHelper.RDLReportDirectoryBasePath + @"\" + item.ID + @"\" + @"\BaseReport";
                    if (System.IO.Directory.Exists(BaseReportPath))
                    {
                        if (!System.IO.Directory.Exists(EntReportPath))
                        {
                            System.IO.Directory.CreateDirectory(EntReportPath);
                        }
                        foreach (var file in System.IO.Directory.GetFiles(BaseReportPath))
                        {
                            string Destfilepath = EntReportPath + @"\" + System.IO.Path.GetFileName(file);
                            if (!System.IO.File.Exists(Destfilepath))
                            {
                                System.IO.File.Copy(file, System.IO.Path.Combine(EntReportPath, System.IO.Path.GetFileName(file)));
                            }
                            else
                            {
                                if (OverwriteExisting)
                                {
                                    System.IO.File.Delete(Destfilepath);
                                    System.IO.File.Copy(file, System.IO.Path.Combine(EntReportPath, System.IO.Path.GetFileName(file)));
                                }
                            }
                        }
                        UpdateAllReportsFields(item);
                    }
                }
            }
            //}

            msg = ResCommon.MsgUpdatedSuccessfully;
            return Json(new { Status = true, Message = msg }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Update All Child Report Fields
        /// </summary>
        /// <param name="objEnt"></param>
        private string UpdateAllReportsFields(EnterpriseDTO objEnt, List<ReportMasterDTO> lstReportMaster = null)
        {
            string msg = string.Empty;
            string ReportBasePath = ResourceHelper.RDLReportDirectoryBasePath;
            string MasterReportBasePath = ResourceHelper.RDLReportDirectoryBasePath + @"\MasterReport";
            IEnumerable<string> strTopSubDirs = Directory.GetDirectories(ReportBasePath);//.Where(x => x != "MasterReport" && x != "Temp");
            //strTopSubDirs = strTopSubDirs.Where(x => x != ReportBasePath + "\\MasterReport").Where(y => y != ReportBasePath + "\\Temp");
            strTopSubDirs = strTopSubDirs.Where(x => x == ReportBasePath + "\\" + objEnt.ID);

            ReportMasterDAL objRptDAL = new ReportMasterDAL(objEnt.EnterpriseDBName);
            List<ReportBuilderDTO> objReportsData = new List<ReportBuilderDTO>();
            if (lstReportMaster != null && lstReportMaster.Count > 0)
            {
                List<string> reportFileNameList = lstReportMaster.Select(x => x.ReportFileName).ToList<string>();
                string reportFileName = string.Join(",", reportFileNameList);
                objReportsData = objRptDAL.GetChildReportListByReportFileName(reportFileName);

            }
            else
            {
                objReportsData = objRptDAL.GetReportList();
            }

            if (strTopSubDirs != null && strTopSubDirs.Count() > 0)
            {
                foreach (var dirName in strTopSubDirs)
                {
                    IEnumerable<string> strSecondSubDirs = Directory.GetDirectories(dirName).Where(x => x != dirName + "\\BaseReport");
                    if (strSecondSubDirs != null && strSecondSubDirs.Count() > 0)
                    {
                        foreach (string Levl2dirName in strSecondSubDirs)
                        {
                            string[] reportFiles = Directory.GetFiles(Levl2dirName);
                            if (reportFiles != null && reportFiles.Length > 0)
                            {
                                foreach (string fileName in reportFiles)
                                {
                                    try
                                    {

                                        string strFileName = fileName.Remove(0, fileName.LastIndexOf("\\") + 1);

                                        ReportBuilderDTO rpt = objReportsData.FirstOrDefault(x => (x.ReportFileName ?? string.Empty).ToLower() == (strFileName ?? string.Empty).ToLower());
                                        if (rpt != null)
                                        {
                                            ReportBuilderDTO parentRpt = null;
                                            if (lstReportMaster != null && lstReportMaster.Count > 0)
                                            {
                                                parentRpt = objReportsData.FirstOrDefault(x => rpt.ParentID > 0 && x.ID == GetBaseParentByReportIDFromList(rpt.ParentID.GetValueOrDefault(0), objReportsData));
                                            }
                                            else
                                            {
                                                parentRpt = objReportsData.FirstOrDefault(x => rpt.ParentID > 0 && x.ID == GetBaseParentByReportID(rpt.ParentID.GetValueOrDefault(0), objEnt.EnterpriseDBName));
                                            }

                                            if (parentRpt != null && !rpt.IsNotEditable.GetValueOrDefault(false) && !rpt.IsDeleted.GetValueOrDefault(false))
                                            {
                                                if (System.IO.File.Exists(MasterReportBasePath + @"\" + parentRpt.ReportFileName))
                                                {
                                                    //Console.WriteLine("Exist");
                                                    XDocument docBase = XDocument.Load(MasterReportBasePath + @"\" + parentRpt.ReportFileName);
                                                    IEnumerable<XElement> lstDSParentFields = docBase.Descendants(ns + "DataSet").Descendants(ns + "Fields");
                                                    XDocument docChild = XDocument.Load(fileName);
                                                    IEnumerable<XElement> lstDSChildFields = docChild.Descendants(ns + "DataSet").Descendants(ns + "Fields");
                                                    docChild.Descendants(ns + "DataSet").Descendants(ns + "Fields").FirstOrDefault().ReplaceWith(lstDSParentFields.ToList());
                                                    docChild.Save(fileName);

                                                    IEnumerable<XElement> lstParentReportPara = docBase.Descendants(ns + "ReportParameters");
                                                    docChild = XDocument.Load(fileName);
                                                    IEnumerable<XElement> lstChildReportPara = docChild.Descendants(ns + "ReportParameters");

                                                    docChild.Descendants(ns + "ReportParameters").FirstOrDefault().ReplaceWith(lstParentReportPara.ToList());
                                                    docChild.Save(fileName);

                                                    IEnumerable<XElement> lstParentQueryPara = docBase.Descendants(ns + "QueryParameters");
                                                    docChild = XDocument.Load(fileName);
                                                    IEnumerable<XElement> lstChildQueryPara = docChild.Descendants(ns + "QueryParameters");

                                                    docChild.Descendants(ns + "QueryParameters").FirstOrDefault().ReplaceWith(lstParentQueryPara.ToList());
                                                    docChild.Save(fileName);
                                                }
                                            }
                                        }

                                        rpt = null;
                                        rpt = objReportsData.FirstOrDefault(x => x.SubReportFileName == strFileName);
                                        if (rpt != null && !rpt.IsNotEditable.GetValueOrDefault(false))
                                        {
                                            ReportBuilderDTO parentRpt = null;
                                            if (lstReportMaster != null && lstReportMaster.Count > 0)
                                            {
                                                parentRpt = objReportsData.FirstOrDefault(x => rpt.ParentID > 0 && x.ID == GetBaseParentByReportIDFromList(rpt.ParentID.GetValueOrDefault(0), objReportsData));
                                            }
                                            else
                                            {
                                                parentRpt = objReportsData.FirstOrDefault(x => rpt.ParentID > 0 && x.ID == GetBaseParentByReportID(rpt.ParentID.GetValueOrDefault(0), objEnt.EnterpriseDBName));
                                            }
                                            if (parentRpt != null)
                                            {
                                                if (System.IO.File.Exists(MasterReportBasePath + @"\" + parentRpt.SubReportFileName))
                                                {
                                                    XDocument docBase = XDocument.Load(MasterReportBasePath + @"\" + parentRpt.SubReportFileName);
                                                    IEnumerable<XElement> lstDSParentFields = docBase.Descendants(ns + "DataSet").Descendants(ns + "Fields");
                                                    XDocument docChild = XDocument.Load(fileName);
                                                    IEnumerable<XElement> lstDSChildFields = docChild.Descendants(ns + "DataSet").Descendants(ns + "Fields");
                                                    if (parentRpt.ReportName.ToLower().Equals("work order with attachment") && lstDSParentFields.ToList().Count > 1)
                                                    {
                                                        docChild.Descendants(ns + "DataSet").Descendants(ns + "Fields").ToList()[0].ReplaceWith(lstDSParentFields.ToList()[0]);
                                                        docChild.Descendants(ns + "DataSet").Descendants(ns + "Fields").ToList()[1].ReplaceWith(lstDSParentFields.ToList()[1]);
                                                    }
                                                    else
                                                    {
                                                        docChild.Descendants(ns + "DataSet").Descendants(ns + "Fields").FirstOrDefault().ReplaceWith(lstDSParentFields.ToList());
                                                    }
                                                    docChild.Save(fileName);
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        msg += " " + string.Format(ResReportMaster.UpdateAllReportsFieldsError, objEnt.EnterpriseDBName, fileName, ex.ToString());
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return msg;
        }

        [HttpPost]
        public JsonResult ReportExecutionData(string ModuleName, string Ids, string sortingOn)
        {
            bool isAppliedSortingByModuleName = false;
            string reportURL = "/Reports/NewReportViewer.aspx?ID=";
            List<KeyValDTO> listkeyval = new List<KeyValDTO>();
            bool? IsNoHeader = false;
            bool? ShowSignature = false;

            //EnterpriseDTO EntDTO = null;
            try
            {
                KeyValDTO objKeyValDTO = new KeyValDTO();
                objKeyValDTO.key = "DataGuids";
                objKeyValDTO.value = Ids;
                if (ModuleName == "Order")
                {
                    reportURL += GetDefaultReportIDByModuleName("Order", out IsNoHeader, out ShowSignature);
                    objKeyValDTO.key = "IDs";
                    objKeyValDTO.value = Ids;
                    isAppliedSortingByModuleName = true;
                }
                else if (ModuleName == "ToolAssetOrder")
                {
                    reportURL += GetReportID("ToolAssetOrder", out IsNoHeader, out ShowSignature);
                    objKeyValDTO.key = "IDs";
                    objKeyValDTO.value = Ids;
                }
                else if (ModuleName == "ReturnOrder")
                {
                    reportURL += GetDefaultReportIDByModuleName("Return Order", out IsNoHeader, out ShowSignature);
                    objKeyValDTO.key = "IDs";
                    objKeyValDTO.value = Ids;
                    isAppliedSortingByModuleName = true;
                }
                else if (ModuleName == "WorkOrder" || ModuleName == "Work Order")
                {
                    reportURL += GetDefaultReportIDByModuleName("Work Order", out IsNoHeader, out ShowSignature);
                    isAppliedSortingByModuleName = true;
                }
                else if (ModuleName == "OrderMasterList")
                {
                    reportURL += GetDefaultReportIDByModuleName("Order", out IsNoHeader, out ShowSignature);
                    isAppliedSortingByModuleName = true;
                    objKeyValDTO.key = "IDs";
                    objKeyValDTO.value = Ids;
                }
                else if (ModuleName == "KitMasterList" || ModuleName == "Kit")
                {
                    reportURL += GetDefaultReportIDByModuleName("Kit", out IsNoHeader, out ShowSignature);
                    isAppliedSortingByModuleName = true;
                    objKeyValDTO.key = "DataGuids";
                    objKeyValDTO.value = Ids;
                }
                else if (ModuleName == "TransferMasterList" || ModuleName == "Transfer")
                {
                    reportURL += GetDefaultReportIDByModuleName("Transfer", out IsNoHeader, out ShowSignature);
                    isAppliedSortingByModuleName = true;
                    objKeyValDTO.key = "DataGuids";
                    objKeyValDTO.value = Ids;
                }
                else if (ModuleName == "InventoryCount" || ModuleName == "Inventory Count")
                {
                    reportURL += GetDefaultReportIDByModuleName("Inventory Count", out IsNoHeader, out ShowSignature);
                    isAppliedSortingByModuleName = true;
                    objKeyValDTO.key = "DataGuids";
                    objKeyValDTO.value = Ids;
                }
                else if (ModuleName == "Items")
                {
                    reportURL += GetDefaultReportIDByModuleName("Items", out IsNoHeader, out ShowSignature);
                    isAppliedSortingByModuleName = true;
                    objKeyValDTO.key = "DataGuids";
                    objKeyValDTO.value = Ids;
                }
                else if (ModuleName == "ItemsBinList")
                {
                    reportURL += GetDefaultReportIDByModuleName("Items", out IsNoHeader, out ShowSignature);
                    isAppliedSortingByModuleName = true;
                    if (Ids.Contains("#"))
                    {
                        string[] strItemBin = Ids.Split(new char[1] { '#' }, StringSplitOptions.RemoveEmptyEntries);
                        if (strItemBin.Length > 0)
                        {
                            objKeyValDTO.key = "DataGuids";
                            objKeyValDTO.value = string.Join(",", strItemBin[0]); // Ids
                            KeyValDTO objKeyValueDTO = new KeyValDTO();
                            objKeyValueDTO.key = "BinIds";
                            objKeyValueDTO.value = string.Join(",", strItemBin[1]); // BinIds
                            listkeyval.Add(objKeyValueDTO);
                        }
                    }

                }
                else if (ModuleName == "Pull")
                {
                    reportURL += GetDefaultReportIDByModuleName("Pull", out IsNoHeader, out ShowSignature);
                    isAppliedSortingByModuleName = true;
                    objKeyValDTO.key = "DataGuids";
                    objKeyValDTO.value = Ids;
                }
                else if (ModuleName == "Requisition")
                {
                    reportURL += GetDefaultReportIDByModuleName("Requisition", out IsNoHeader, out ShowSignature);
                    isAppliedSortingByModuleName = true;
                    objKeyValDTO.key = "DataGuids";
                    objKeyValDTO.value = Ids;
                }
                else if (ModuleName == "Project Spend")
                {
                    reportURL += GetDefaultReportIDByModuleName("Project Spend", out IsNoHeader, out ShowSignature);
                    isAppliedSortingByModuleName = true;
                    objKeyValDTO.key = "DataGuids";
                    objKeyValDTO.value = Ids;
                }
                else if (ModuleName == "Suggested Orders")
                {
                    reportURL += GetDefaultReportIDByModuleName("Suggested Orders", out IsNoHeader, out ShowSignature);
                    isAppliedSortingByModuleName = true;
                    objKeyValDTO.key = "DataGuids";
                    objKeyValDTO.value = Ids;
                }
                else if (ModuleName == "Company")
                {
                    reportURL += GetDefaultReportIDByModuleName("Company", out IsNoHeader, out ShowSignature);
                    isAppliedSortingByModuleName = true;
                    objKeyValDTO.key = "DataGuids";
                    objKeyValDTO.value = Ids;
                }
                else if (ModuleName == "Room")
                {
                    if (SessionHelper.RoleID < 0)
                    {
                        string strEntReport = string.Empty;
                        //System.Xml.Linq.XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));
                        //if (Settinfile != null && Settinfile.Element("RoomReportGrid") != null)
                        //    strEntReport = Convert.ToString(Settinfile.Element("RoomReportGrid").Value);

                        if (SiteSettingHelper.RoomReportGrid != string.Empty)
                            strEntReport = SiteSettingHelper.RoomReportGrid;

                        if (!string.IsNullOrEmpty(strEntReport))
                        {
                            EnterpriseMasterDAL objEntDal = new EnterpriseMasterDAL();
                            //objEntDTO = objEntDal.GetEnterpriseByName(strEntReport); // RoomReportGrid.Name
                            objEntDTO = objEntDal.GetNonDeletedEnterpriseByIdPlain(Convert.ToInt64(strEntReport));
                            if (objEntDTO != null)
                            {
                                reportURL += GetDefaultPrintRoomReportIDBySetting("Room", out IsNoHeader, out ShowSignature);
                                IsRoomGridReportCommon = true;
                                RoomDBName = objEntDTO.EnterpriseDBName;
                            }
                            else
                                reportURL += GetDefaultReportIDByModuleName("Room", out IsNoHeader, out ShowSignature);
                        }
                        else
                        {
                            reportURL += GetDefaultReportIDByModuleName("Room", out IsNoHeader, out ShowSignature);
                        }
                    }
                    else
                    {
                        reportURL += GetDefaultReportIDByModuleName("Room", out IsNoHeader, out ShowSignature);
                    }
                    isAppliedSortingByModuleName = true;
                    objKeyValDTO.key = "DataGuids";
                    objKeyValDTO.value = Ids;
                    if (string.IsNullOrWhiteSpace(sortingOn) || sortingOn == null)
                    {
                        ReportMasterDAL objDAL;
                        if (IsRoomGridReportCommon && objEntDTO != null)
                            objDAL = new ReportMasterDAL(objEntDTO.EnterpriseDBName);
                        else
                            objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                        List<ReportMasterDTO> lstReport = objDAL.GetAllBaseReport(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                        if (lstReport != null && lstReport.Count > 0)
                            lstReport = lstReport.Where(x => x.ReportName == "Room").ToList();
                        if (lstReport != null && lstReport.Count > 0)
                        {
                            List<ReportMasterDTO> lstChildReport = objDAL.GetChildReport(lstReport.FirstOrDefault().ID, false, false);
                            if (lstChildReport != null && lstChildReport.Count > 0)
                            {
                                lstChildReport = lstChildReport.Where(x => x.SetAsDefaultPrintReport == true).ToList();
                                sortingOn = lstChildReport.Select(x => x.SortColumns).FirstOrDefault();
                            }
                        }
                    }
                }
                else if (ModuleName == "Enterprises List")
                {
                    reportURL += GetDefaultReportIDByModuleName("Enterprises List", out IsNoHeader, out ShowSignature);
                    isAppliedSortingByModuleName = true;
                    objKeyValDTO.key = "DataGuids";
                    objKeyValDTO.value = Ids;
                }
                else if (ModuleName == "Users")
                {
                    reportURL += GetDefaultReportIDByModuleName("Users", out IsNoHeader, out ShowSignature);
                    isAppliedSortingByModuleName = true;
                    objKeyValDTO.key = "DataGuids";
                    objKeyValDTO.value = Ids;
                }
                else if (ModuleName == "Supplier")
                {
                    reportURL += GetDefaultReportIDByModuleName("Supplier", out IsNoHeader, out ShowSignature);
                    isAppliedSortingByModuleName = true;
                    objKeyValDTO.key = "DataGuids";
                    objKeyValDTO.value = Ids;
                }
                else if (ModuleName == "Maintenance Due"
                         || ModuleName == "MaintenanceDue")
                {
                    reportURL += GetDefaultReportIDByModuleName("Maintenance Due", out IsNoHeader, out ShowSignature);
                    objKeyValDTO.key = "DataGuids";
                    objKeyValDTO.value = Ids;
                    isAppliedSortingByModuleName = true;
                }
                else if (ModuleName == "Asset Maintenance")
                {
                    reportURL += GetDefaultReportIDByModuleName("Asset Maintenance", out IsNoHeader, out ShowSignature);
                    objKeyValDTO.key = "DataGuids";
                    objKeyValDTO.value = Ids;
                    isAppliedSortingByModuleName = true;
                }
                else if (ModuleName == "QuoteMasterList")
                {
                    reportURL += GetDefaultReportIDByModuleName("Quote", out IsNoHeader, out ShowSignature);
                    objKeyValDTO.key = "DataGuids";
                    objKeyValDTO.value = Ids;
                    isAppliedSortingByModuleName = true;
                }
                else
                {
                    reportURL += GetReportID(ModuleName, out IsNoHeader, out ShowSignature);
                }
                listkeyval.Add(objKeyValDTO);



                SetReportParaDictionary(listkeyval);
                if (!string.IsNullOrWhiteSpace(sortingOn))
                {
                    Dictionary<string, string> rptPara = (Dictionary<string, string>)SessionHelper.Get("ReportPara");
                    if (rptPara != null)
                    {

                        rptPara["SortFields"] = sortingOn;
                    }
                    else
                    {
                        rptPara = new Dictionary<string, string>();
                        rptPara["SortFields"] = sortingOn;
                    }

                    SessionHelper.Add("ReportPara", rptPara);
                    objKeyValDTO = new KeyValDTO();
                    objKeyValDTO.key = "SortFields";
                    objKeyValDTO.value = sortingOn.Replace("UpdatedByName", "RoomUpdateByName").Replace("CreatedByName", "RoomCreatedByName");
                    listkeyval.Add(objKeyValDTO);
                }
                else
                {
                    //string SortingOn = GetReportSortFields(ModuleName);
                    string SortingOn = string.Empty;
                    if (isAppliedSortingByModuleName)
                    {
                        string ModName = ModuleName;
                        if (ModuleName == "Company")
                            ModName = "AdminCompany";
                        else if (ModuleName == "Enterprises List")
                            ModName = "AdminEnterprise";
                        else if (ModuleName == "Inventory Count")
                            ModName = "InventoryCount";
                        else if (ModuleName == "Item" || ModuleName == "Item List" || ModuleName == "ItemListingGroup" || ModuleName == "Items" || ModuleName == "ItemsBinList")
                            ModName = "InventoryItem";
                        else if (ModuleName == "Kit")
                            ModName = "InventoryKit";
                        else if (ModuleName == "Order" || ModuleName == "Order Grouping" || ModuleName == "Order List" || ModuleName == "OrderMasterList")
                            ModName = "ReplenishOrder";
                        else if (ModuleName == "Project Spend")
                            ModName = "ConsumeProjectSpend";
                        else if (ModuleName == "Pull")
                            ModName = "ConsumePull";
                        else if (ModuleName == "Pull Completed" || ModuleName == "Pull Incomplete" || ModuleName == "Pull No Header" || ModuleName == "Pull Summary" || ModuleName == "Pull Summary By ConsignedPO" || ModuleName == "Pull Summary by Quarter")
                            ModName = "Consume_Pull";
                        else if (ModuleName == "Requisition")
                            ModName = "ConsumeRequisition";
                        else if (ModuleName == "Return Order")
                            ModName = "ReplenishReturnOrder";
                        else if (ModuleName == "Room")
                            ModName = "AdminRoom";
                        else if (ModuleName == "Suggested Orders")
                            ModName = "ReplenishCart";
                        else if (ModuleName == "Tools")
                            ModName = "Tool";
                        else if (ModuleName == "Transfer")
                            ModName = "ReplenishTransfer";
                        else if (ModuleName == "Users")
                            ModName = "AdminUser";
                        else if (ModuleName == "Work Order")
                            ModName = "ConsumeWorkOrder";
                        else if (ModuleName == "Maintenance Due")
                            ModName = "MaintenanceToolAsset";
                        SortingOn = GetReportSortFields(ModName);
                    }
                    else
                    {
                        ReportMaster objReportMaster = GetReportMasterFromModuleName(ModuleName);
                        if (objReportMaster != null && !string.IsNullOrEmpty(objReportMaster.SortColumns))
                        {
                            SortingOn = objReportMaster.SortColumns;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(SortingOn))
                    {
                        SortingOn = SortingOn.TrimEnd(',');
                    }
                    Dictionary<string, string> rptPara = (Dictionary<string, string>)SessionHelper.Get("ReportPara");
                    if (rptPara != null)
                    {
                        rptPara["SortFields"] = SortingOn;
                    }
                    else
                    {
                        rptPara = new Dictionary<string, string>();
                        rptPara["SortFields"] = SortingOn;
                    }

                    SessionHelper.Add("ReportPara", rptPara);
                    if (!string.IsNullOrWhiteSpace(SortingOn))
                    {
                        objKeyValDTO = new KeyValDTO();
                        objKeyValDTO.key = "SortFields";
                        objKeyValDTO.value = SortingOn.TrimEnd(',');
                        listkeyval.Add(objKeyValDTO);
                    }


                }
                if (ModuleName != "Room" && ModuleName != "Company")
                {
                    Dictionary<string, string> rptParaSession = (Dictionary<string, string>)SessionHelper.Get("ReportPara");
                    if (rptParaSession != null)
                    {
                        rptParaSession["@Roomids"] = Convert.ToString(SessionHelper.RoomID);
                        rptParaSession["@CompanyIDs"] = Convert.ToString(SessionHelper.CompanyID);
                    }
                    else
                    {
                        rptParaSession = new Dictionary<string, string>();
                        rptParaSession["@Roomids"] = Convert.ToString(SessionHelper.RoomID);
                        rptParaSession["@CompanyIDs"] = Convert.ToString(SessionHelper.CompanyID);
                    }


                    if ((IsNoHeader ?? false))
                    {
                        rptParaSession["IsNoHeader"] = Convert.ToString("1");
                    }

                    if ((ShowSignature ?? false))
                    {
                        rptParaSession["ShowSignature"] = Convert.ToString("1");
                    }

                    listkeyval.Add(objKeyValDTO);
                    SessionHelper.Add("ReportPara", rptParaSession);
                    objKeyValDTO = new KeyValDTO();
                    objKeyValDTO.key = "Roomids";
                    objKeyValDTO.value = Convert.ToString(SessionHelper.RoomID);
                    listkeyval.Add(objKeyValDTO);
                    objKeyValDTO = new KeyValDTO();
                    objKeyValDTO.key = "CompanyIDs";
                    objKeyValDTO.value = Convert.ToString(SessionHelper.CompanyID);
                    listkeyval.Add(objKeyValDTO);
                }
                else if (ModuleName == "Room")
                {
                    Dictionary<string, string> rptParaSession = (Dictionary<string, string>)SessionHelper.Get("ReportPara");
                    if ((IsNoHeader ?? false))
                    {
                        rptParaSession["IsNoHeader"] = Convert.ToString("1");
                    }

                    if ((ShowSignature ?? false))
                    {
                        rptParaSession["ShowSignature"] = Convert.ToString("1");
                    }
                    SessionHelper.Add("ReportPara", rptParaSession);
                }



            }
            catch (Exception)
            { }
            return Json(new { Status = true, ReqURL = reportURL }, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public JsonResult ReportExecutionDataByModuleID(string ModuleID, string Ids)
        {
            string reportURL = "/Reports/NewReportViewer.aspx?ID={0}";
            List<KeyValDTO> listkeyval = new List<KeyValDTO>();
            try
            {
                KeyValDTO objKeyValDTO = new KeyValDTO();
                objKeyValDTO.key = "DataGuids";
                objKeyValDTO.value = Ids;
                reportURL = string.Format(reportURL, ModuleID);

                listkeyval.Add(objKeyValDTO);
                SetReportParaDictionary(listkeyval);


            }
            catch
            { reportURL = string.Format(reportURL, 0); }
            return Json(new { Status = true, ReqURL = reportURL }, JsonRequestBehavior.AllowGet);

        }
        public string GetReportID(string ModuleName, out bool? HideHeader, out bool? ShowSignature)
        {
            string reportID = "0";
            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            ReportMaster objReportMaster = new ReportMaster();
            HideHeader = false;
            ShowSignature = false;
            if (!string.IsNullOrWhiteSpace(ModuleName))
            {

                objReportMaster = objReportMasterDAL.GetReportIDNew(ModuleName);
                HideHeader = objReportMaster.HideHeader ?? false;
                ShowSignature = objReportMaster.ShowSignature ?? false;
                reportID = Convert.ToString(objReportMaster.ID);
            }
            // if (ModuleName == "Requisition" || ModuleName == "Order" || ModuleName == "Return Order" || ModuleName == "Work Order" || ModuleName == "OrderMasterList" || ModuleName == "Room")
            {
                string ModName = string.Empty;
                switch (ModuleName)
                {
                    case "Assets":
                        ModName = "Assetmaster";
                        break;
                    case "Company":
                        ModName = "Company";
                        break;
                    case "Cumulative Pull":
                        ModName = "CumulativePull";
                        break;
                    case "Discrepancy Report":
                        ModName = "Item-Discrepency";
                        break;
                    case "Enterprises List":
                        ModName = "EnterpriseList";
                        break;
                    case "eVMI Poll History":
                        ModName = "eVMIPollH";
                        break;
                    case "eVMI Usage":
                    case "eVMI Usage No Header":
                        ModName = "eVMI";
                        break;
                    case "eVMI Usage Manual Count":
                        ModName = "eVMI_ManualCount";
                        break;
                    case "Expiring Items":
                        ModName = "ExpiringItems";
                        break;
                    case "InStock":
                        ModName = "InStockByBin";
                        break;
                    case "InStock By Activity":
                        ModName = "InStockByActivity";
                        break;
                    case "InStock Margin":
                        ModName = "InStockByBinMargin";
                        break;
                    case "Instock with QOH":
                        ModName = "InStockWithQOH";
                        break;
                    case "Inventory Count":
                        ModName = "CountMaster";
                        break;
                    case "Item":
                    case "Item List":
                    case "ItemListingGroup":
                    case "Items":
                        ModName = "ItemList";
                        break;
                    case "Kit":
                        ModName = "Kit";
                        break;
                    case "Maintenance":
                        ModName = "Maintenance";
                        break;
                    case "Order":
                    case "Order Grouping":
                    case "Order List":
                        ModName = "Order";
                        break;
                    case "Orders With LineItems":
                        ModName = "Replenish_Order";
                        break;
                    case "Project Spend":
                        ModName = "ProjectSpend";
                        break;
                    case "Pull":
                    case "Pull Completed":
                    case "Pull Incomplete":
                    case "Pull No Header":
                    case "Pull Summary":
                    case "Pull Summary By ConsignedPO":
                    case "Pull Summary by Quarter":
                    case "Total Pulled":
                        ModName = "Consume_Pull";
                        break;
                    case "Pull Summary By WO":
                        ModName = "WOPullSummary";
                        break;
                    case "Receivable Items":
                        ModName = "Receive";
                        break;
                    case "Received Items":
                        ModName = "Range-Receive";
                        break;
                    case "Return Item Candidates":
                        ModName = "Range-Receive";
                        break;
                    case "Requisition":
                        ModName = "Consume_Requisition";
                        break;
                    case "Requisition Item Summary":
                        ModName = "ReqItemSummary";
                        break;
                    case "Requisition With LineItems":
                        ModName = "Range-Consume_Requisition";
                        break;
                    case "Return Order":
                        ModName = "ReturnOrder";
                        break;
                    case "Room":
                        ModName = "Room";
                        break;
                    case "Staging":
                        ModName = "Staging";
                        break;
                    case "Sugg. Orders of Exp. Date":
                        ModName = "SuggOrderOfExpDate";
                        break;
                    case "Suggested Orders":
                        ModName = "Cart";
                        break;
                    case "ToolAssetOrder":
                    case "Tool_ToolAssetOrder":

                        ModName = "ToolAssetOrder";
                        break;
                    case "Tools":
                        ModName = "Tool";
                        break;
                    case "Tools checked out":
                    case "Tools Checked Out":
                        ModName = "CheckOutTool";
                        break;
                    case "Tools CheckIn-out History":
                        ModName = "ToolInOutHistory";
                        break;
                    case "Transfer":
                        ModName = "Transfer";
                        break;
                    case "Transfer With LineItems":
                        ModName = "TransferdItems";
                        break;
                    case "UnfulFilled Order LineItems":
                    case "UnfulFilled Orders":
                        ModName = "UnfulFilledOrders";
                        break;
                    case "Users":
                        ModName = "UsersList";
                        break;
                    case "Work Order":
                        ModName = "WorkOrder";
                        break;
                    case "Workorders List":
                        ModName = "WorkorderList";
                        break;
                    case "Item Stock Out History":
                        ModName = "Stock Out Item";
                        break;
                    case "Inventory Stock Out":
                        ModName = "Inventory Stock Out";
                        break;
                    case "Asset Maintenance":
                        ModName = "Assetmaster";
                        break;
                    case "Items With Suppliers":
                        ModName = "ItemsWithSuppliers";
                        break;
                    case "Maintenance Due":
                        ModName = "MaintenanceToolAsset";
                        break;
                    default:
                        ModName = ModuleName;
                        break;
                }

                objReportMaster = objReportMasterDAL.GetDefaultReportIDNew(ModName, SessionHelper.RoomID, SessionHelper.CompanyID);
                if (objReportMaster != null)
                {
                    Int64 RID = Convert.ToInt64(objReportMaster.ID);
                    HideHeader = objReportMaster.HideHeader ?? false;
                    ShowSignature = objReportMaster.ShowSignature ?? false;
                    if (RID > 0)
                    {
                        reportID = Convert.ToString(RID);
                    }
                }
            }
            return reportID;
        }

        public ReportMaster GetReportMasterFromModuleName(string ModuleName)
        {
            //string reportID = "0";
            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            //ReportMaster objReportMaster = new ReportMaster();
            //if (!string.IsNullOrWhiteSpace(ModuleName))
            //{

            //objReportMaster = objReportMasterDAL.GetReportIDNew(ModuleName);
            //reportID = Convert.ToString(objReportMaster.ID);
            //}
            // if (ModuleName == "Requisition" || ModuleName == "Order" || ModuleName == "Return Order" || ModuleName == "Work Order" || ModuleName == "OrderMasterList" || ModuleName == "Room")

            string ModName = string.Empty;
            switch (ModuleName)
            {
                case "Assets":
                    ModName = "Assetmaster";
                    break;
                case "Company":
                    ModName = "Company";
                    break;
                case "Cumulative Pull":
                    ModName = "CumulativePull";
                    break;
                case "Discrepancy Report":
                    ModName = "Item-Discrepency";
                    break;
                case "Enterprises List":
                    ModName = "EnterpriseList";
                    break;
                case "eVMI Poll History":
                    ModName = "eVMIPollH";
                    break;
                case "eVMI Usage":
                case "eVMI Usage No Header":
                    ModName = "eVMI";
                    break;
                case "eVMI Usage Manual Count":
                    ModName = "eVMI_ManualCount";
                    break;
                case "Expiring Items":
                    ModName = "ExpiringItems";
                    break;
                case "InStock":
                    ModName = "InStockByBin";
                    break;
                case "InStock By Activity":
                    ModName = "InStockByActivity";
                    break;
                case "InStock Margin":
                    ModName = "InStockByBinMargin";
                    break;
                case "Instock with QOH":
                    ModName = "InStockWithQOH";
                    break;
                case "Inventory Count":
                    ModName = "CountMaster";
                    break;
                case "Item":
                case "Item List":
                case "ItemListingGroup":
                case "Items":
                    ModName = "ItemList";
                    break;
                case "Kit":
                    ModName = "Kit";
                    break;
                case "Maintenance":
                    ModName = "Maintenance";
                    break;
                case "Order":
                case "Order Grouping":
                case "Order List":
                    ModName = "Order";
                    break;
                case "Orders With LineItems":
                    ModName = "Replenish_Order";
                    break;
                case "Project Spend":
                    ModName = "ProjectSpend";
                    break;
                case "Pull":
                case "Pull Completed":
                case "Pull Incomplete":
                case "Pull No Header":
                case "Pull Summary":
                case "Pull Summary By ConsignedPO":
                case "Pull Summary by Quarter":
                case "Total Pulled":
                    ModName = "Consume_Pull";
                    break;
                case "Pull Summary By WO":
                    ModName = "WOPullSummary";
                    break;
                case "Receivable Items":
                    ModName = "Receive";
                    break;
                case "Received Items":
                    ModName = "Range-Receive";
                    break;
                case "Return Item Candidates":
                    ModName = "Range-Receive";
                    break;
                case "Requisition":
                    ModName = "Consume_Requisition";
                    break;
                case "Requisition Item Summary":
                    ModName = "ReqItemSummary";
                    break;
                case "Requisition With LineItems":
                    ModName = "Range-Consume_Requisition";
                    break;
                case "Return Order":
                    ModName = "ReturnOrder";
                    break;
                case "Room":
                    ModName = "Room";
                    break;
                case "Staging":
                    ModName = "Staging";
                    break;
                case "Sugg. Orders of Exp. Date":
                    ModName = "SuggOrderOfExpDate";
                    break;
                case "Suggested Orders":
                    ModName = "Cart";
                    break;
                case "Tools":
                    ModName = "Tool";
                    break;
                case "Tools checked out":
                case "Tools Checked Out":
                    ModName = "CheckOutTool";
                    break;
                case "Tools CheckIn-out History":
                    ModName = "ToolInOutHistory";
                    break;
                case "Transfer":
                    ModName = "Transfer";
                    break;
                case "Transfer With LineItems":
                    ModName = "TransferdItems";
                    break;
                case "UnfulFilled Order LineItems":
                case "UnfulFilled Orders":
                    ModName = "UnfulFilledOrders";
                    break;
                case "Users":
                    ModName = "UsersList";
                    break;
                case "Work Order":
                    ModName = "WorkOrder";
                    break;
                case "Workorders List":
                    ModName = "WorkorderList";
                    break;
                case "Item Stock Out History":
                    ModName = "Stock Out Item";
                    break;
                case "Inventory Stock Out":
                    ModName = "Inventory Stock Out";
                    break;
                case "Asset Maintenance":
                    ModName = "Assetmaster";
                    break;
                case "Items With Suppliers":
                    ModName = "ItemsWithSuppliers";
                    break;
                case "Maintenance Due":
                    ModName = "MaintenanceToolAsset";
                    break;
                default:
                    ModName = ModuleName;
                    break;
            }

            return objReportMasterDAL.GetDefaultReportIDNew(ModName, SessionHelper.RoomID, SessionHelper.CompanyID);


        }

        public string GetReportSortFields(string ModuleName)
        {
            string reportSortField = string.Empty;
            if (!string.IsNullOrWhiteSpace(ModuleName))
            {
                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                ReportMaster objReportMaster = new ReportMaster();
                objReportMaster = objReportMasterDAL.GetReportDetailsByDefaultReportID(SessionHelper.RoomID, SessionHelper.CompanyID, ModuleName);
                if (objReportMaster != null && !string.IsNullOrEmpty(objReportMaster.SortColumns))
                {
                    reportSortField = objReportMaster.SortColumns;
                }
                else reportSortField = string.Empty;
            }
            return reportSortField;
        }
        public void SetReportParaDictionary(List<KeyValDTO> paras)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            MasterController objMastCtrl = new MasterController();
            string BasePath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath;
            string DBServerName = ConfigurationManager.AppSettings["DBserverName"];
            string DBUserName = ConfigurationManager.AppSettings["DbUserName"];
            string DBPassword = ConfigurationManager.AppSettings["DbPassword"];
            string DBName = SessionHelper.EnterPriseDBName;
            //if (IsRoomGridReportCommon && !string.IsNullOrEmpty(RoomDBName))
            //    DBName = RoomDBName;
            string connectionString = @"Data Source={0};Initial Catalog={1};User ID={2};Password={3}";
            connectionString = string.Format(connectionString, DBServerName, DBName, DBUserName, DBPassword);
            connectionString = DbConnectionHelper.GeteTurnsSQLConnectionString(SessionHelper.EnterPriseDBName, DbConnectionType.GeneralReadOnly.ToString("F"));
            if (IsRoomGridReportCommon && !string.IsNullOrEmpty(RoomDBName))
            {
                connectionString = DbConnectionHelper.GeteTurnsSQLConnectionString(RoomDBName, DbConnectionType.GeneralReadOnly.ToString("F"));
                dictionary.Add("IsRoomGridReportCommon", "true");
                dictionary.Add("DBName", RoomDBName);
                if (objEntDTO != null)
                    dictionary.Add("EnterpriseID", objEntDTO.ID.ToString());
            }
            dictionary.Add("ConnectionString", connectionString);
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);

            //if (System.Web.HttpContext.Current.Request.IsSecureConnection)
            //{
            //    dictionary.Add("eTurnsLogoURL", "http://localhost:4040" + "/Content/OpenAccess/logoInReport.png");
            //    dictionary.Add("EnterpriseLogoURL", "http://localhost:4040" + objMastCtrl.ConvertImageToPNG(BasePath, "Uploads\\EnterpriseLogos\\" + SessionHelper.EnterPriceID + "\\" + SessionHelper.EnterpriseLogoUrl));
            //    dictionary.Add("BarcodeURL", "http://localhost:4040" + "/Barcode/GetBarcodeByKey?barcodekey=");
            //}
            //else
            //{
            //    dictionary.Add("eTurnsLogoURL", "http://localhost:" + Request.Url.Port + "/Content/OpenAccess/logoInReport.png");
            //    dictionary.Add("EnterpriseLogoURL", "http://localhost:" + System.Web.HttpContext.Current.Request.Url.Port + objMastCtrl.ConvertImageToPNG(BasePath, "Uploads\\EnterpriseLogos\\" + SessionHelper.EnterPriceID + "\\" + SessionHelper.EnterpriseLogoUrl));
            //    dictionary.Add("BarcodeURL", "http://localhost:" + Request.Url.Port + "/Barcode/GetBarcodeByKey?barcodekey=");
            //}

            if (ConfigurationManager.AppSettings["IsServer"] == "True")
            {
                string baseURL = System.Web.HttpContext.Current.Request.Url.ToString().Replace(System.Web.HttpContext.Current.Request.Url.AbsolutePath, "");
                baseURL = SessionHelper.CurrentDomainURL;
                if (objCommonDAL.HasSpecialDomain(SessionHelper.CurrentDomainURL, SessionHelper.EnterPriceID))
                {
                    dictionary.Add("eTurnsLogoURL", baseURL + objMastCtrl.ConvertImageToPNG(eTurnsAppConfig.BaseFileSharedPath, "Uploads\\EnterpriseLogos\\" + SessionHelper.EnterPriceID + "\\" + SessionHelper.EnterpriseLogoUrl));
                    dictionary.Add("EnterpriseLogoURL", baseURL + "/Content/OpenAccess/NologoReport.png");
                }
                else
                {
                    dictionary.Add("eTurnsLogoURL", baseURL + "/Content/OpenAccess/logoInReport.png");
                    dictionary.Add("EnterpriseLogoURL", baseURL + objMastCtrl.ConvertImageToPNG(eTurnsAppConfig.BaseFileSharedPath, "Uploads\\EnterpriseLogos\\" + SessionHelper.EnterPriceID + "\\" + SessionHelper.EnterpriseLogoUrl));
                }
                dictionary.Add("CompanyLogoURL", baseURL + objMastCtrl.ConvertImageToPNG(eTurnsAppConfig.BaseFileSharedPath, "Uploads\\CompanyLogos\\" + SessionHelper.CompanyID + "\\" + SessionHelper.CompanyLogoUrl));
                dictionary.Add("BarcodeURL", baseURL + "/Barcode/GetBarcodeByKey?barcodekey=");
                dictionary.Add("WOSignatureURL", baseURL + "/Uploads/WorkOrderSignature/" + SessionHelper.CompanyID + "/");
                dictionary.Add("WOAttachmentPath", baseURL + "/Uploads/WorkOrderFile/" + SessionHelper.EnterPriceID + "/" + SessionHelper.CompanyID + "/" + SessionHelper.RoomID + "/");
            }
            else
            {
                if (objCommonDAL.HasSpecialDomain(SessionHelper.CurrentDomainURL, SessionHelper.EnterPriceID))
                {
                    dictionary.Add("eTurnsLogoURL", "https://localhost:" + System.Web.HttpContext.Current.Request.Url.Port + objMastCtrl.ConvertImageToPNG(BasePath, "Uploads\\EnterpriseLogos\\" + SessionHelper.EnterPriceID + "\\" + SessionHelper.EnterpriseLogoUrl));
                    dictionary.Add("EnterpriseLogoURL", "https://localhost:" + "/Content/OpenAccess/NologoReport.png");
                }
                else
                {
                    dictionary.Add("eTurnsLogoURL", "https://localhost:" + System.Web.HttpContext.Current.Request.Url.Port + "/Content/OpenAccess/logoInReport.png");
                    dictionary.Add("EnterpriseLogoURL", "https://localhost:" + System.Web.HttpContext.Current.Request.Url.Port + objMastCtrl.ConvertImageToPNG(BasePath, "Uploads\\EnterpriseLogos\\" + SessionHelper.EnterPriceID + "\\" + SessionHelper.EnterpriseLogoUrl));
                }
                dictionary.Add("CompanyLogoURL", "http://localhost:" + System.Web.HttpContext.Current.Request.Url.Port + objMastCtrl.ConvertImageToPNG(BasePath, "Uploads\\EnterpriseLogos\\" + SessionHelper.CompanyID + "\\" + SessionHelper.CompanyLogoUrl));
                dictionary.Add("BarcodeURL", "http://localhost:" + System.Web.HttpContext.Current.Request.Url.Port + "/Barcode/GetBarcodeByKey?barcodekey=");
                dictionary.Add("WOSignatureURL", "http://localhost:" + System.Web.HttpContext.Current.Request.Url.Port + "/Uploads/WorkOrderSignature/" + SessionHelper.CompanyID + "/");
                dictionary.Add("WOAttachmentPath", "http://localhost:" + System.Web.HttpContext.Current.Request.Url.Port + "/Uploads/WorkOrderFile/" + SessionHelper.EnterPriceID + "/" + SessionHelper.CompanyID + "/" + SessionHelper.RoomID + "/");
            }

            dictionary.Add("UserID", SessionHelper.UserID.ToString());

            if (paras != null && paras.Count > 0)
            {
                foreach (var item in paras)
                {
                    //if (item.key.ToLower() == "startdate")
                    //    item.value = TimeZoneInfo.ConvertTimeToUtc(DateTime.Parse(item.value), SessionHelper.CurrentTimeZone).ToString("yyyy-MM-dd HH:mm:ss");
                    //if (item.key.ToLower() == "enddate")
                    //    item.value = TimeZoneInfo.ConvertTimeToUtc(DateTime.Parse(item.value).AddSeconds(86399).AddDays(-1), SessionHelper.CurrentTimeZone).ToString("yyyy-MM-dd HH:mm:ss");

                    if (item.key.ToLower() == "startdate")
                    {
                        dictionary.Add("OrigStartDate", DateTime.ParseExact(item.value, (SessionHelper.RoomDateFormat), SessionHelper.RoomCulture).ToString("yyyy-MM-dd HH:mm:ss"));
                        item.value = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(item.value, (SessionHelper.RoomDateFormat), SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    if (item.key.ToLower() == "enddate")
                    {
                        dictionary.Add("OrigEndDate", DateTime.ParseExact(item.value, (SessionHelper.RoomDateFormat), SessionHelper.RoomCulture).AddSeconds(86399).ToString("yyyy-MM-dd HH:mm:ss"));
                        item.value = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(item.value, (SessionHelper.RoomDateFormat), SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString("yyyy-MM-dd HH:mm:ss");
                    }

                    dictionary.Add(item.key, item.value);
                }
            }

            SessionHelper.Add("ReportPara", dictionary);

        }
        public JsonResult GetCompanyList()
        {
            List<KeyValDTO> lstList = new List<KeyValDTO>();
            foreach (CompanyMasterDTO comp in SessionHelper.CompanyList.OrderBy(x => x.Name))
            {
                KeyValDTO obj = new KeyValDTO();
                obj.key = comp.ID.ToString();
                obj.value = comp.Name;
                lstList.Add(obj);
            }
            return Json(new { Status = true, CompanyList = lstList, Selected = SessionHelper.CompanyID }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSuppliersForRooms(string CompanyIds, string RoomIds)
        {
            SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
            bool AppendRoomName = false;

            if (!string.IsNullOrWhiteSpace(RoomIds) && (RoomIds ?? string.Empty).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Count() > 1)
            {
                AppendRoomName = true;
            }
            List<SupplierMasterDTO> lstSuppliers = objSupplierMasterDAL.GetSupplierByRoomsIdsNormal(RoomIds, SessionHelper.UserID);

            //if (SessionHelper.UserSupplierIds != null && SessionHelper.UserSupplierIds.Any())
            //{
            //    lstSuppliers = lstSuppliers.Where(t => SessionHelper.UserSupplierIds.Contains(t.ID)).ToList();
            //}

            lstSuppliers.ForEach(t =>
            {
                t.SupplierName = ((AppendRoomName) ? (t.SupplierName + "-(" + t.RoomName + ")") : (t.SupplierName));
            });

            return Json(lstSuppliers, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRoomList(string CompanyID)
        {
            bool AppendCompanyname = false;
            if (!string.IsNullOrWhiteSpace(CompanyID) && (CompanyID ?? string.Empty).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Count() > 1)
            {
                AppendCompanyname = true;
            }

            string[] arrid = CompanyID.Split(',');
            RoomDAL objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
            List<KeyValDTO> lstRoomDTO = new List<KeyValDTO>();
            Int64 RoleID = SessionHelper.RoleID;
            Int64 Session_EnterPriceID = SessionHelper.EnterPriceID;
            List<RoomDTO> DBRoomDTO = objRoomDAL.GetAllRoomsFromETurnsMaster(SessionHelper.CompanyID, false, false, SessionHelper.RoomList, string.Empty, RoleID, Session_EnterPriceID);

            lstRoomDTO = (from c in DBRoomDTO
                          where arrid.Contains(c.CompanyID.ToString())
                          select new KeyValDTO
                          {
                              key = c.ID.ToString(),
                              value = ((AppendCompanyname) ? (c.RoomName + "-(" + c.CompanyName + ")") : (c.RoomName))
                          }).OrderBy(x => x.value).ToList();
            return Json(new { Status = true, RoomList = lstRoomDTO, Selected = SessionHelper.RoomID }, JsonRequestBehavior.AllowGet);
        }




        public JsonResult GetStatus(string ReportModuleName)
        {
            if (ReportModuleName == "Requisition")
            {
                ReportModuleName = "Consume_Requisition";
            }
            List<ReportFilterParam> lstReportFilterParam = new List<ReportFilterParam>();
            ReportFilterParam ReportFilterParam = new ReportFilterParam();
            List<KeyValCheckDTO> objList = GetFileterList(ReportModuleName);
            lstReportFilterParam.Add(ReportFilterParam);
            List<KeyValDTO> lstStatusDTO = new List<KeyValDTO>();
            lstStatusDTO = (from c in objList

                            select new KeyValDTO
                            {
                                key = c.key.ToString(),
                                value = c.value.ToString()
                            }).ToList();
            return Json(new { Status = true, RoomList = lstStatusDTO, Selected = "Unsubmitted" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetOrderStatus(string ReportModuleName)
        {
            List<ReportFilterParam> lstReportFilterParam = new List<ReportFilterParam>();
            ReportFilterParam ReportFilterParam = new ReportFilterParam();
            List<KeyValCheckDTO> objList = GetFileterListForOrderStatus(ReportModuleName);
            lstReportFilterParam.Add(ReportFilterParam);
            List<KeyValDTO> lstStatusDTO = new List<KeyValDTO>();
            lstStatusDTO = (from c in objList

                            select new KeyValDTO
                            {
                                key = c.key.ToString(),
                                value = c.value.ToString()
                            }).ToList();
            return Json(new { Status = true, RoomList = lstStatusDTO, Selected = "Unsubmitted" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDashboardParmList(string SelectedRoomID)
        {
            ReportPerameters objRptParameters = new ReportPerameters();
            if (!string.IsNullOrWhiteSpace(SelectedRoomID))
            {
                Int64 RoomID = 0;
                long.TryParse(SelectedRoomID, out RoomID);
                if (RoomID > 0)
                {
                    RoomDAL objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
                    //  RoomDTO objRoomDTO = objRoomDAL.GetRoomByIDPlain(RoomID);
                    CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                    string columnList = "ID,RoomName,CompanyID";
                    RoomDTO objRoomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");
                    if (objRoomDTO != null)
                    {

                        long CompanyID = objRoomDTO.CompanyID ?? 0;
                        if (CompanyID > 0)
                        {
                            DashboardDAL objDashboardDAL = new DashboardDAL(SessionHelper.EnterPriseDBName);
                            DashboardParameterDTO objDashboardParameterDTO = new DashboardParameterDTO();
                            objDashboardParameterDTO = objDashboardDAL.GetDashboardParameters(RoomID, CompanyID);
                            if (objDashboardParameterDTO != null)
                            {
                                objRptParameters.MonthlyAverageUsage = objDashboardParameterDTO.MonthlyAverageUsage;

                                objRptParameters.MinMaxDayOfUsageToSample = objDashboardParameterDTO.MinMaxDayOfUsageToSample;
                                objRptParameters.MinMaxMeasureMethod = objDashboardParameterDTO.MinMaxMeasureMethod;
                                objRptParameters.MinMaxDayOfAverage = objDashboardParameterDTO.MinMaxDayOfAverage;
                                objRptParameters.MinMaxMinNumberOfTimesMax = objDashboardParameterDTO.MinMaxMinNumberOfTimesMax;

                                //if (Settinfile.Element("decimalPointFromConfig") != null)
                                //{
                                //    objRptParameters.DecimalPointFromConfig = Convert.ToInt32(Settinfile.Element("decimalPointFromConfig").Value);
                                //}

                                if (SiteSettingHelper.decimalPointFromConfig != string.Empty)
                                {
                                    objRptParameters.DecimalPointFromConfig = Convert.ToInt32(SiteSettingHelper.decimalPointFromConfig);
                                }
                                eTurnsRegionInfo objeTurnsRegionInfo = new eTurnsRegionInfo();
                                RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
                                objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(SessionHelper.RoomID, SessionHelper.CompanyID, -1);
                                string DateTimeFormat = "MM/dd/yyyy";
                                if (objeTurnsRegionInfo != null)
                                {
                                    DateTimeFormat = objeTurnsRegionInfo.ShortDatePattern + " " + objeTurnsRegionInfo.ShortTimePattern;
                                }

                                DateTime DTNow = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                                DateTime QStartDate, QEndDate;

                                QEndDate = DTNow.AddDays(-1);
                                QStartDate = DTNow.AddDays(-1).AddDays((-objRptParameters.MinMaxDayOfUsageToSample) ?? 0);

                                objRptParameters.QuantumEndDate = QEndDate.Date.AddHours(23).AddMinutes(59).ToString(DateTimeFormat);
                                objRptParameters.QuantumStartDate = QStartDate.Date.ToString(DateTimeFormat);
                            }
                        }
                    }
                }
            }
            return Json(new { Status = true, RptParameters = objRptParameters, Selected = SessionHelper.RoomID }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetQuantamStartEndDate(Int32 AUDayOfUsageToSample)
        {
            eTurnsRegionInfo objeTurnsRegionInfo = new eTurnsRegionInfo();
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
            objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(SessionHelper.RoomID, SessionHelper.CompanyID, -1);
            string DateTimeFormat = "MM/dd/yyyy";
            if (objeTurnsRegionInfo != null)
            {
                DateTimeFormat = objeTurnsRegionInfo.ShortDatePattern + " " + objeTurnsRegionInfo.ShortTimePattern;
            }

            DateTime DTNow = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
            DateTime QStartDate, QEndDate;

            QEndDate = DTNow.AddDays(-1);
            QStartDate = DTNow.AddDays(-1).AddDays(-AUDayOfUsageToSample);

            string QuantumStartDate = null;
            string QuantumEndDate = null;

            QuantumEndDate = QEndDate.Date.AddHours(23).AddMinutes(59).ToString(DateTimeFormat);
            QuantumStartDate = QStartDate.Date.ToString(DateTimeFormat);

            //eTurnsRegionInfo objeTurnsRegionInfo = new eTurnsRegionInfo();
            //RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
            //objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(SessionHelper.RoomID, SessionHelper.CompanyID, -1);
            //string DateTimeFormat = objeTurnsRegionInfo.LongDatePattern;          

            //if (AUDayOfUsageToSample != null)
            //{
            //    //QuantumStartDate = TimeZoneInfo.ConvertTime(Convert.ToDateTime((CommonUtility.ConvertDateByTimeZone(DateTime.UtcNow.AddDays(-1).AddDays(-(Convert.ToInt32(AUDayOfUsageToSample))), SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true))), TimeZoneInfo.Utc, TimeZoneInfo.Utc).ToString(DateTimeFormat);
            //    QuantumStartDate = TimeZoneInfo.ConvertTime(DateTime.ParseExact((CommonUtility.ConvertDateByTimeZone(DateTime.UtcNow.AddDays(-1).AddDays(-(Convert.ToInt32(AUDayOfUsageToSample))), SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true)), SessionHelper.DateTimeFormat, SessionHelper.RoomCulture), TimeZoneInfo.Utc, TimeZoneInfo.Utc).ToString(DateTimeFormat);
            //}
            //else
            //    QuantumStartDate = null;

            ////QuantumStartDate = TimeZoneInfo.ConvertTime(DateTime.ParseExact((CommonUtility.ConvertDateByTimeZone(DateTime.UtcNow.AddDays(-1).AddDays(-(Convert.ToInt32(objRptParameters.AUDayOfUsageToSample))), SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true)), SessionHelper.DateTimeFormat, SessionHelper.RoomCulture), TimeZoneInfo.Utc, TimeZoneInfo.Utc).ToString(DateTimeFormat);
            //QuantumEndDate = TimeZoneInfo.ConvertTime(DateTime.ParseExact((CommonUtility.ConvertDateByTimeZone(Convert.ToDateTime(Convert.ToDateTime(DateTime.UtcNow.AddDays(-1)).ToShortDateString()).AddHours(23).AddMinutes(59), SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true)), SessionHelper.DateTimeFormat, SessionHelper.RoomCulture), TimeZoneInfo.Utc, TimeZoneInfo.Utc).ToString(DateTimeFormat);

            return Json(new { QuantumStartDate = QuantumStartDate, QuantumEndDate = QuantumEndDate }, JsonRequestBehavior.AllowGet);
        }


        //public List<KeyValDTO> GetMaintenance(long[] arrCompanyid, long[] arrRoomid, string startDate, string endDate, string[] statusType, Int64 ReportID, string ReportRange, string Starttime, string Endtime)
        //{
        //    List<KeyValDTO> lstKeyValDTO = new List<KeyValDTO>();
        //    List<KeyValDTO> lsttempKeyValDTO = new  List<KeyValDTO>();
        //    CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
        //    IEnumerable<RPT_ToolMaintenanceDTO> DBWOData = null;


        //    string fieldName = "MaintenanceName";
        //    if (!string.IsNullOrEmpty(ReportRange))
        //    {
        //        fieldName = ReportRange;
        //    }

        //    if (!(string.IsNullOrEmpty(startDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(startDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
        //    {
        //        if (!string.IsNullOrWhiteSpace(Starttime))
        //        {
        //            string[] Hours_Minutes = Starttime.Split(':');
        //            int TotalSeconds = 0;
        //            if (Hours_Minutes != null && Hours_Minutes.Length > 0)
        //            {
        //                TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
        //                TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
        //            }
        //            startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
        //        }
        //        else
        //        {
        //            startDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(startDate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString(); //TimeZoneInfo.ConvertTimeToUtc(startDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone);
        //        }
        //    }

        //    if (!(string.IsNullOrEmpty(endDate)) && Convert.ToDateTime(DateTime.ParseExact(Convert.ToString(endDate), Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult)) > DateTime.MinValue)
        //    {
        //        if (!string.IsNullOrWhiteSpace(Endtime))
        //        {
        //            string[] Hours_Minutes = Endtime.Split(':');
        //            int TotalSeconds = 0;
        //            if (Hours_Minutes != null && Hours_Minutes.Length > 0)
        //            {
        //                TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
        //                TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
        //            }
        //            TotalSeconds += 59;
        //            endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
        //        }
        //        else
        //        {
        //            endDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(endDate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString();  //TimeZoneInfo.ConvertTimeToUtc(endDate.GetValueOrDefault(DateTime.MinValue).AddSeconds(86399), SessionHelper.CurrentTimeZone);
        //        }
        //    }

        //     DBWOData = objCommonDAL.GetDataForReportFilterList<RPT_ToolMaintenanceDTO>("RPT_Maintenance", arrCompanyid, arrRoomid, startDate, endDate, SessionHelper.UserID);



        //    List<int> arrOrderStatus = new List<int>();
        //    if (statusType != null && statusType.Length > 0)
        //    {
        //        for (int i = 0; i < statusType.Length; i++)
        //        {
        //            arrOrderStatus.Add(int.Parse(statusType[i]));
        //        }
        //    }


        //    lsttempKeyValDTO = (from p in DBWOData
        //                        select new KeyValDTO
        //                        {
        //                            key = Convert.ToString(p.GUID),
        //                            value = Convert.ToString(p.GetType().GetProperty(fieldName).GetValue(p, null)),
        //                        }).ToList();

        //    foreach (var itemOrd in lsttempKeyValDTO)
        //    {
        //        if (!lstKeyValDTO.Exists(x => x.value.ToLower() == itemOrd.value.ToLower()))
        //        {
        //            lstKeyValDTO.Add(itemOrd);
        //        }
        //        else
        //        {
        //            lstKeyValDTO[lstKeyValDTO.FindIndex(x => x.value.ToLower() == itemOrd.value.ToLower())].key += "," + itemOrd.key;
        //        }
        //    }
        //    if (lstKeyValDTO != null)
        //    {
        //        lstKeyValDTO = lstKeyValDTO.OrderBy(x => x.value.ToLower()).ToList();
        //    }
        //    return lstKeyValDTO;

        //}


        public ActionResult ModuleItemTypes(string ModuleName)
        {
            ViewBag.ModuleName = ModuleName;
            return PartialView();
        }
        public ActionResult ModuleItemStatus(string ModuleName)
        {
            ViewBag.ModuleName = ModuleName;
            return PartialView();
        }
        public ActionResult ModuleItems(string ModuleName)
        {
            ViewBag.ModuleName = ModuleName;
            return PartialView();
        }

        #region *** Call for Create PDF Runtime ****

        string connectionString = "";
        Dictionary<string, string> rptPara = null;
        string FullPath = string.Empty;
        /// <summary>
        /// GetReportParaDictionary
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetReportParaDictionary(List<KeyValDTO> paras)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            if (paras != null && paras.Count > 0)
            {
                foreach (var item in paras)
                {
                    //if (item.key.ToLower() == "startdate")
                    //    item.value = TimeZoneInfo.ConvertTimeToUtc(DateTime.Parse(item.value), SessionHelper.CurrentTimeZone).ToString("yyyy-MM-dd HH:mm:ss");
                    //if (item.key.ToLower() == "enddate")
                    //    item.value = TimeZoneInfo.ConvertTimeToUtc(DateTime.Parse(item.value).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString("yyyy-MM-dd HH:mm:ss");

                    if (item.key.ToLower() == "startdate")
                    {
                        dictionary.Add("OrigStartDate", DateTime.ParseExact(item.value, (SessionHelper.RoomDateFormat), SessionHelper.RoomCulture).ToString("yyyy-MM-dd HH:mm:ss"));
                        item.value = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(item.value, (SessionHelper.RoomDateFormat), SessionHelper.RoomCulture), SessionHelper.CurrentTimeZone).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    if (item.key.ToLower() == "enddate")
                    {
                        dictionary.Add("OrigEndDate", DateTime.ParseExact(item.value, (SessionHelper.RoomDateFormat), SessionHelper.RoomCulture).AddSeconds(86399).ToString("yyyy-MM-dd HH:mm:ss"));
                        item.value = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(item.value, (SessionHelper.RoomDateFormat), SessionHelper.RoomCulture).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString("yyyy-MM-dd HH:mm:ss");
                    }

                    dictionary.Add(item.key, item.value);
                }
            }
            connectionString = rptPara["ConnectionString"];
            return rptPara;
        }

        public string CopyFiletoTemp(string strfile, string reportname)
        {

            string ReportTempPath = string.Empty;
            string ReportRetPath = string.Empty;
            string RDLCBaseFilePath = CommonUtility.RDLCBaseFilePath;
            ReportTempPath = RDLCBaseFilePath + "/Temp";

            if (!System.IO.Directory.Exists(ReportTempPath))
            {
                System.IO.Directory.CreateDirectory(ReportTempPath);
            }
            ReportRetPath = ReportTempPath + @"/" + reportname + ".rdlc";
            //System.IO.File.Copy(strfile, ReportRetPath);

            System.IO.File.Create(ReportRetPath).Dispose();
            System.IO.File.WriteAllText(ReportRetPath, System.IO.File.ReadAllText(strfile));

            return ReportRetPath;

        }
        public IEnumerable<XElement> UpdateResource(IEnumerable<XElement> lstTablix, string ResFile)
        {
            foreach (XElement Table in lstTablix)
            {
                IEnumerable<XElement> lstTableCell = Table.Descendants(ns + "TablixCell");
                foreach (XElement Cell in lstTableCell)
                {
                    if (Cell.Descendants(ns + "Value").Any())
                    {
                        Cell.Descendants(ns + "Value").FirstOrDefault().Value = GetResourceValue(ResFile, Cell.Descendants(ns + "Value").FirstOrDefault().Value);
                    }
                }
            }

            return lstTablix;
        }
        public XDocument UpdateReportTileFromResource(XDocument doc1, string ReportResourceName)
        {
            XDocument doc = new XDocument(doc1);
            XElement xElerptTitle = doc.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Textbox").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "ReportTitle");
            if (xElerptTitle == null)
            {
                xElerptTitle = doc.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Textbox").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "TextboxWO");
            }
            if (!string.IsNullOrWhiteSpace(ReportResourceName)
                && xElerptTitle != null)
            {
                xElerptTitle.Descendants(ns + "Value").FirstOrDefault().Value = ReportResourceName;
            }
            return doc;
        }
        private Dictionary<string, string> GetReportParaDictionary()
        {

            Dictionary<string, string> rptPara = (Dictionary<string, string>)SessionHelper.Get("ReportPara");
            connectionString = rptPara["ConnectionString"];
            return rptPara;
        }
        public string GetResourceValue(string FileName, string Key)
        {
            string KeyVal = string.Empty;
            int ResRead = !string.IsNullOrWhiteSpace(SiteSettingHelper.ResourceRead) ? Convert.ToInt32(SiteSettingHelper.ResourceRead) : (int)ResourceReadType.File;
            if (Key.ToLower().Contains("udf"))
            {
                if (ResRead == (int)eTurns.DTO.ResourceReadType.File)
                    KeyVal = ResourceHelper.GetResourceValue(Key, FileName, true);
                else
                    KeyVal = ResourceModuleHelper.GetResourceValue(Key, FileName, true);
            }
            else
            {
                if (ResRead == (int)eTurns.DTO.ResourceReadType.File)
                    KeyVal = ResourceHelper.GetResourceValue(Key, FileName);
                else
                    KeyVal = ResourceModuleHelper.GetResourceValue(Key, FileName);
            }

            return KeyVal;
        }
        /// <summary>
        /// ReportFullBasePath
        /// ConnetionString
        /// BarcodeURL
        /// LogoURL
        /// </summary>
        /// <param name="paras"></param>
        /// <param name="ReportName"></param>
        /// <returns></returns>
        public JsonResult GeneratePDF(string ReportID, bool IsSendMail = true)
        {
            ParentID = 0;
            ReportBuilderDTO objDTO = new ReportBuilderDTO();
            ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            eTurns.DAL.AlertMail objAlertMail = new eTurns.DAL.AlertMail();
            objDTO = objDAL.GetReportDetail(Convert.ToInt64(ReportID));
            string MasterReportResFile = objDTO.MasterReportResFile;
            SubReportResFile = MasterReportResFile;// objDTO.SubReportResFile;
            string Reportname = objDTO.ReportName;
            string MasterReportname = objDTO.ReportFileName;
            string SubReportname = objDTO.SubReportFileName;
            string mainGuid = "RPT_" + Guid.NewGuid().ToString().Replace("-", "_");
            string subGuid = "SubRPT_" + Guid.NewGuid().ToString().Replace("-", "_");
            string ReportPath = string.Empty;
            bool hasSubReport = false;
            ParentID = objDTO.ParentID ?? 0;

            string RDLCBaseFilePath = CommonUtility.RDLCBaseFilePath;
            if (objDTO.ParentID > 0)
            {
                if (objDTO.ISEnterpriseReport.GetValueOrDefault(false))
                {
                    ReportPath = CopyFiletoTemp(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/EnterpriseReport" + @"\\" + MasterReportname, mainGuid);
                }
                else
                {
                    ReportPath = CopyFiletoTemp(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID + @"\\" + MasterReportname, mainGuid);
                }
            }
            else
            {
                ReportPath = CopyFiletoTemp(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/BaseReport" + @"\\" + MasterReportname, mainGuid);
            }

            XDocument doc = XDocument.Load(ReportPath);
            if (objDTO != null
                && objDTO.IsBaseReport
                && !string.IsNullOrWhiteSpace(objDTO.ReportResourceName))
            {
                doc = UpdateReportTileFromResource(doc, objDTO.ReportResourceName);
            }
            IEnumerable<XElement> lstTablix = doc.Descendants(ns + "Tablix");
            string strTablix = string.Empty;
            if (lstTablix != null && lstTablix.ToList().Count > 0)
            {
                strTablix = lstTablix.ToList()[0].ToString();
            }
            IEnumerable<XElement> lstUpdateTablix = UpdateResource(lstTablix, MasterReportResFile);
            doc.Save(ReportPath);
            //doc.Descendants(ns + "Tablix").FirstOrDefault().Value = lstUpdateTablix.FirstOrDefault().Value;
            IEnumerable<XElement> lstReportPara = doc.Descendants(ns + "ReportParameter");
            List<ReportParameter> rpt = new List<ReportParameter>();
            rptPara = GetReportParaDictionary();

            if ((objDTO.ModuleName.Trim().ToLower() == "user_list") || (objDTO.ReportName.Trim().ToLower() == "users" && objDTO.ModuleName.Trim().ToLower() == "userslist"))
            {
                connectionString = DbConnectionHelper.GeteTurnsMasterSQLConnectionString(DbConnectionHelper.GetETurnsMasterDBName(), DbConnectionType.GeneralReadOnly.ToString("F"));
            }

            doc = amDAL.AddFormatToTaxbox(doc, rsInfo);
            doc.Save(ReportPath);
            doc = XDocument.Load(ReportPath);

            if (doc.Descendants(ns + "Subreport") != null && doc.Descendants(ns + "Subreport").Count() > 0)
            {
                hasSubReport = true;
            }

            #region WI-4947 - Please add a Last Run Date to the schedule reports

            if (rptPara != null && rptPara.Count > 0)
            {
                string JsonReportParameters = string.Empty;
                var convertedDictionary = rptPara.ToDictionary(item => Convert.ToString(item.Key), item => Convert.ToString(item.Value));
                var json = new JavaScriptSerializer().Serialize(convertedDictionary);
                JsonReportParameters = new JavaScriptSerializer().Serialize(convertedDictionary);

                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                ViewReportHistory objViewReportHistory = new ViewReportHistory();
                objViewReportHistory.ReportID = Convert.ToInt64(ReportID);
                objViewReportHistory.RoomId = SessionHelper.RoomID;
                objViewReportHistory.CompanyId = SessionHelper.CompanyID;
                objViewReportHistory.UserId = SessionHelper.UserID;
                objViewReportHistory.RequestType = "SendReport";
                objViewReportHistory.ReportParameters = JsonReportParameters;
                objReportMasterDAL.InsertViewReportHistory(objViewReportHistory);
            }

            #endregion

            //TODO: Start WI-1627: Setting the sort fields does not work            
            if (objDTO.ReportType == 3 && !hasSubReport && rptPara.ContainsKey("SortFields")
                && !string.IsNullOrEmpty(rptPara["SortFields"]))
            {
                string SortFields = rptPara["SortFields"];

                string[] arrSortFields = SortFields.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (arrSortFields != null && arrSortFields.Length > 0)
                {
                    string firstSortFields = arrSortFields[0].Replace(" asc", "").Replace(" desc", "").Replace(" ASC", "").Replace(" DESC", "");
                    XElement xRowHira = doc.Descendants(ns + "TablixRowHierarchy").FirstOrDefault();
                    XElement xGroup = xRowHira.Descendants(ns + "Group").FirstOrDefault();
                    XElement xGroupExpression = xGroup.Descendants(ns + "GroupExpression").FirstOrDefault();
                    if (xGroupExpression != null)
                        xGroupExpression.Value = "=Fields!" + firstSortFields + ".Value";
                }
                doc.Save(ReportPath);
            }

            //TODO: End WI-1627: Setting the sort fields does not work

            Dictionary<string, string> reportParams = new Dictionary<string, string>();

            if (lstReportPara != null && lstReportPara.Count() > 0)
            {
                foreach (var item in lstReportPara)
                {
                    ReportParameter rpara = new ReportParameter();
                    rpara.Name = item.Attribute("Name").Value;
                    if (!string.IsNullOrEmpty(rptPara.FirstOrDefault(x => x.Key.Replace("@", "") == item.Attribute("Name").Value.Replace("@", "")).Value))
                    {
                        rpara.Values.Add(rptPara.FirstOrDefault(x => x.Key.Replace("@", "") == item.Attribute("Name").Value.Replace("@", "")).Value);
                    }
                    rpt.Add(rpara);
                }
            }

            if (rptPara.Keys.Contains("IsFromRunReport") && rptPara["IsFromRunReport"] == "1")
            {
                if (rptPara.Keys.Contains("ShowSignature") && rptPara["ShowSignature"] == "1")
                {
                    doc = objAlertMail.GetFooterForSignature(doc, objDTO);
                    doc.Save(ReportPath);
                    doc = XDocument.Load(ReportPath);
                }
                if (rptPara.Keys.Contains("IsNoHeader") && rptPara["IsNoHeader"] == "1")
                {
                    doc = objAlertMail.GetAdditionalHeaderRow(doc, objDTO, SessionHelper.CompanyName, SessionHelper.RoomName, rptPara, EnterpriseDBName: SessionHelper.EnterPriseDBName);
                    doc.Save(ReportPath);
                    doc = XDocument.Load(ReportPath);
                }
                else
                {
                    if (!objDTO.IsNotEditable.GetValueOrDefault(false)
                         && (objDTO.ReportType == 1 || objDTO.ReportType == 2 || objDTO.ReportType == 3))
                    {
                        XElement xRows = doc.Descendants(ns + "TablixRows").FirstOrDefault();
                        XElement xRow = doc.Descendants(ns + "TablixRow").FirstOrDefault();

                        if (objDTO.ReportType == 2 && xRow.Descendants(ns + "TablixCell").Descendants(ns + "Textbox").
                                FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "TextboxRoom") != null)
                        {
                            doc.Descendants(ns + "TablixRow").FirstOrDefault().Remove();
                            doc.Descendants(ns + "TablixRowHierarchy").Descendants(ns + "TablixMembers").Descendants(ns + "TablixMember").Descendants(ns + "TablixMembers").Descendants(ns + "TablixMember").Remove();
                            foreach (var item in xRows.Descendants(ns + "TablixRow"))
                            {
                                doc.Descendants(ns + "TablixRowHierarchy").Descendants(ns + "TablixMembers").Descendants(ns + "TablixMember").Descendants(ns + "TablixMembers").FirstOrDefault().Add(new XElement(ns + "TablixMember"));
                            }
                            doc.Save(ReportPath);
                            doc = XDocument.Load(ReportPath);
                        }
                    }
                }
            }
            else
            {
                if (!objDTO.IsNotEditable.GetValueOrDefault(false)
                     && (objDTO.ReportType == 1 || objDTO.ReportType == 2 || objDTO.ReportType == 3))
                {
                    XElement xRows = doc.Descendants(ns + "TablixRows").FirstOrDefault();
                    XElement xRow = doc.Descendants(ns + "TablixRow").FirstOrDefault();

                    if (objDTO.ReportType == 2 && xRow.Descendants(ns + "TablixCell").Descendants(ns + "Textbox").
                            FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "TextboxRoom") != null)
                    {
                        doc.Descendants(ns + "TablixRow").FirstOrDefault().Remove();
                        doc.Descendants(ns + "TablixRowHierarchy").Descendants(ns + "TablixMembers").Descendants(ns + "TablixMember").Descendants(ns + "TablixMembers").Descendants(ns + "TablixMember").Remove();
                        foreach (var item in xRows.Descendants(ns + "TablixRow"))
                        {
                            doc.Descendants(ns + "TablixRowHierarchy").Descendants(ns + "TablixMembers").Descendants(ns + "TablixMember").Descendants(ns + "TablixMembers").FirstOrDefault().Add(new XElement(ns + "TablixMember"));
                        }
                        doc.Save(ReportPath);
                        doc = XDocument.Load(ReportPath);
                    }
                }
            }

            string connString = doc.Descendants(ns + "ConnectString").FirstOrDefault().Value;
            SqlConnection myConnection = new SqlConnection();
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter sqla = new SqlDataAdapter();
            DataTable dt = new DataTable();
            myConnection.ConnectionString = connectionString;// connString;

            cmd.Connection = myConnection;
            cmd.CommandText = doc.Descendants(ns + "CommandText").FirstOrDefault().Value; //"SELECT *  FROM   ItemMaster_View";
            cmd.CommandType = CommandType.Text;
            if (doc.Descendants(ns + "CommandType").FirstOrDefault() != null)
            {
                cmd.CommandType = (CommandType)Enum.Parse(typeof(CommandType), doc.Descendants(ns + "CommandType").FirstOrDefault().Value == null ? "Text" : doc.Descendants(ns + "CommandType").FirstOrDefault().Value, true);
            }

            IEnumerable<XElement> lstQueryPara = doc.Descendants(ns + "QueryParameter");

            if (lstQueryPara != null && lstQueryPara.Count() > 0)
            {
                foreach (var item in lstQueryPara)
                {
                    SqlParameter slpar = new SqlParameter();
                    slpar.ParameterName = item.Attribute("Name").Value;//
                    if (!(hasSubReport && slpar.ParameterName.ToLower().Replace("@", "") == "sortfields") && !string.IsNullOrEmpty(rptPara.FirstOrDefault(x => x.Key.Replace("@", "") == item.Attribute("Name").Value.Replace("@", "")).Value))
                        slpar.Value = rptPara.FirstOrDefault(x => x.Key.Replace("@", "") == item.Attribute("Name").Value.Replace("@", "")).Value;
                    else
                        slpar.Value = DBNull.Value;
                    XElement objReportPara = lstReportPara.FirstOrDefault(x => x.Attribute("Name").Value == slpar.ParameterName.Replace("@", ""));

                    if (objReportPara.Descendants(ns + "DataType") != null && objReportPara.Descendants(ns + "DataType").Count() > 0)
                    {
                        slpar.DbType = (DbType)Enum.Parse(typeof(DbType), objReportPara.Descendants(ns + "DataType").FirstOrDefault().Value, true);
                    }

                    cmd.Parameters.Add(slpar);
                }
            }
            cmd.CommandTimeout = 7200;
            sqla = new SqlDataAdapter(cmd);
            sqla.Fill(dt);

            ReportViewer ReportViewer1 = new ReportViewer();
            ReportViewer1.Reset();
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.Visible = true;
            //string strSubTablix = string.Empty;
            //string rdlPath = string.Empty;
            if (doc.Descendants(ns + "Subreport") != null && doc.Descendants(ns + "Subreport").Count() > 0)
            {
                hasSubReport = true;
                //rdlPath = Server.MapPath(@"/RDLC_Reports/" + SessionHelper.EnterPriceID.ToString() + "") + @"\\" + SubReportname  ;
                if (objDTO.ParentID > 0)
                {
                    if (objDTO.ISEnterpriseReport.GetValueOrDefault(false))
                    {
                        rdlPath = CopyFiletoTemp(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/EnterpriseReport" + @"\\" + SubReportname, subGuid);
                    }
                    else
                    {
                        rdlPath = CopyFiletoTemp(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID + @"\\" + SubReportname, subGuid);
                    }
                }
                else
                {

                    rdlPath = CopyFiletoTemp(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/BaseReport" + @"\\" + SubReportname, subGuid);
                }
                doc.Descendants(ns + "Tablix").Descendants(ns + "Subreport").FirstOrDefault().Attribute("Name").Value = Convert.ToString(subGuid);
                doc.Descendants(ns + "Tablix").Descendants(ns + "ReportName").FirstOrDefault().Value = Convert.ToString(subGuid);
                doc.Save(ReportPath);

                XDocument docSub = XDocument.Load(rdlPath);
                IEnumerable<XElement> lstSubTablix = docSub.Descendants(ns + "Tablix");

                if (lstSubTablix != null && lstSubTablix.ToList().Count > 0)
                {
                    strSubTablix = lstSubTablix.ToList()[0].ToString();
                }

                docSub = amDAL.AddFormatToTaxbox(docSub, rsInfo);
                docSub.Save(rdlPath);
                docSub = XDocument.Load(rdlPath);

                IEnumerable<XElement> lstUpdateSubTablix = UpdateResource(lstSubTablix, SubReportResFile);
                //TODO: Start WI-6271:
                ReportBuilderDTO oReportBuilderOrderDTO = objDAL.GetParentReportMasterByReportID(objDTO.ID);
                if (oReportBuilderOrderDTO != null)
                {
                    if (oReportBuilderOrderDTO.ReportName.ToLower().Equals("work order with grouped pulls"))
                    {
                        if (objDTO.ReportType == 2 && hasSubReport
                           && rptPara.ContainsKey("SortFields")
                           && !string.IsNullOrEmpty(rptPara["SortFields"]))
                        {
                            string SortFields = rptPara["SortFields"];

                            string[] arrSortFields = SortFields.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            if (arrSortFields != null && arrSortFields.Length > 0)
                            {
                                string firstSortFields = arrSortFields[0].Replace(" asc", "").Replace(" desc", "").Replace(" ASC", "").Replace(" DESC", "");
                                XElement xRowHira = docSub.Descendants(ns + "TablixRowHierarchy").FirstOrDefault();
                                XElement xGroup = xRowHira.Descendants(ns + "Group").FirstOrDefault();
                                XElement xGroupExpression = xGroup.Descendants(ns + "GroupExpression").FirstOrDefault();
                                if (xGroupExpression != null)
                                    xGroupExpression.Value = "=Fields!" + firstSortFields + ".Value";
                            }
                        }
                    }
                }

                //TODO: End WI-6271:
                docSub.Save(rdlPath);

                ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(LoadSubreport);

                ReportViewer1.LocalReport.EnableExternalImages = true;
                ReportViewer1.LocalReport.EnableHyperlinks = true;
                ReportViewer1.LocalReport.Refresh();
            }
            if (!hasSubReport && rptPara.ContainsKey("SortFields") && !string.IsNullOrEmpty(rptPara["SortFields"]))
            {
                string SortFields = rptPara["SortFields"];

                if (!string.IsNullOrEmpty(SortFields))
                {
                    dt.DefaultView.Sort = SortFields;
                    dt = dt.DefaultView.ToTable();

                }
            }

            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.ReportPath = ReportPath;
            ReportDataSource rds = new ReportDataSource();
            rds.Name = doc.Descendants(ns + "DataSet").FirstOrDefault().FirstAttribute.Value;
            rds.Value = dt;

            ReportViewer1.LocalReport.DataSources.Add(rds);
            ReportViewer1.LocalReport.SetParameters(rpt);
            ReportViewer1.ZoomMode = ZoomMode.Percent;
            ReportViewer1.LocalReport.Refresh();

            string ReportExportPath = string.Empty;
            string ReportExportPathForExcel = string.Empty;

            if (rptPara.Keys.Contains("IsFromRunReport") && rptPara["IsFromRunReport"] == "1")
            {
                if (rptPara.Keys.Contains("AttachmentTypes") && rptPara["AttachmentTypes"].Contains("1"))
                {
                    Warning[] warnings;
                    string[] streamIds;
                    string mimeType = "application/pdf";
                    string encoding = "utf-8";
                    string extension = "pdf";
                    byte[] bytes = ReportViewer1.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                    ReportExportPath = System.Web.HttpContext.Current.Server.MapPath(@"/Downloads/") + objDTO.ReportName + DateTimeUtility.DateTimeNow.ToString("MM/dd/yyyy HH:mm:ss").Replace("/", "").Replace(":", "") + ".pdf";
                    using (FileStream fs = new FileStream(ReportExportPath, FileMode.Create))
                    {
                        fs.Write(bytes, 0, bytes.Length);
                    }
                }
                if (rptPara.Keys.Contains("AttachmentTypes") && rptPara["AttachmentTypes"].Contains("2"))
                {
                    Warning[] warnings = null;
                    string[] streamIds = null;
                    string mimeType = "application/vnd.ms-excel";
                    string encoding = "utf-8";
                    string extension = "xls";

                    byte[] bytes = ReportViewer1.LocalReport.Render("Excel", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
                    ReportExportPathForExcel = System.Web.HttpContext.Current.Server.MapPath(@"/Downloads/") + objDTO.ReportName + DateTimeUtility.DateTimeNow.ToString("MM/dd/yyyy HH:mm:ss").Replace("/", "").Replace(":", "") + ".xls";
                    using (FileStream fs = new FileStream(ReportExportPathForExcel, FileMode.Create))
                    {
                        fs.Write(bytes, 0, bytes.Length);
                    }
                }
            }
            else
            {
                Warning[] warnings;
                string[] streamIds;
                string mimeType = "application/pdf";
                string encoding = "utf-8";
                string extension = "pdf";
                byte[] bytes = ReportViewer1.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                ReportExportPath = System.Web.HttpContext.Current.Server.MapPath(@"/Downloads/") + objDTO.ReportName + DateTimeUtility.DateTimeNow.ToString("MM/dd/yyyy HH:mm:ss").Replace("/", "").Replace(":", "") + ".pdf";
                using (FileStream fs = new FileStream(ReportExportPath, FileMode.Create))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }
            }

            //if (IsSendMail)
            //    SendReportMail(ReportExportPath, objDTO.ReportName);

            return Json(new { Status = true, Message = "ok", ReportPDFFilePath = ReportExportPath, ReportExportPathForExcel = ReportExportPathForExcel }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDataGuidsBasedOnRangeForRunInStockReports(string rangeType, string rangeTypeIds, long ReportID)
        {
            ItemMasterDAL objItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            string DataGuids = string.Empty;


            if (!string.IsNullOrEmpty(rangeType) && !string.IsNullOrEmpty(rangeTypeIds))
            {


                ReportBuilderDTO objRPTDTO = new ReportBuilderDTO();
                ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                objRPTDTO = objDAL.GetReportDetail(ReportID);


                List<ReportGroupMasterDTO> lstReportGroupMasterDTO = null;
                string _FieldColumnID = string.Empty;
                if (objRPTDTO.ParentID.GetValueOrDefault(0) > 0)
                {
                    Int64 ParentID = GetBaseParentByReportID(objRPTDTO.ParentID.GetValueOrDefault(0));
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ParentID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ParentID && (x.FieldName ?? string.Empty).ToLower() == (rangeType ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }
                else
                {
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ReportID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ReportID && (x.FieldName ?? string.Empty).ToLower() == (rangeType ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }

                DataGuids = objItemDAL.GetItemsByReportRange(rangeType, _FieldColumnID, rangeTypeIds, SessionHelper.RoomID.ToString(), SessionHelper.CompanyID.ToString());

                /*
                switch (rangeType)
                {
                    case "ItemNumber":
                        DataGuids = rangeTypeIds;
                        break;
                    case "SupplierPartNo":
                    case "Supplier":
                    case "Bin":
                    case "Category":
                        DataGuids = objItemDAL.GetItemsByReportRange(rangeType, _FieldColumnID, rangeTypeIds, SessionHelper.RoomID.ToString(), SessionHelper.CompanyID.ToString());
                        break;
                }
                */
            }
            return Json(new { Status = true, Message = "ok", data = DataGuids }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDataGuidsBasedOnRangeForRunWokrOrderListReports(string rangeType, string rangeTypeIds, long ReportID)
        {
            WorkOrderDAL objWorkOrderDAL = new WorkOrderDAL(SessionHelper.EnterPriseDBName);
            string DataGuids = string.Empty;
            if (!string.IsNullOrEmpty(rangeType) && !string.IsNullOrEmpty(rangeTypeIds))
            {
                ReportBuilderDTO objRPTDTO = new ReportBuilderDTO();
                ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                objRPTDTO = objDAL.GetReportDetail(ReportID);

                List<ReportGroupMasterDTO> lstReportGroupMasterDTO = null;
                string _FieldColumnID = string.Empty;
                if (objRPTDTO.ParentID.GetValueOrDefault(0) > 0)
                {
                    Int64 ParentID = GetBaseParentByReportID(objRPTDTO.ParentID.GetValueOrDefault(0));
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ParentID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ParentID && (x.FieldName ?? string.Empty).ToLower() == (rangeType ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }
                else
                {
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ReportID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ReportID && (x.FieldName ?? string.Empty).ToLower() == (rangeType ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }

                DataGuids = objWorkOrderDAL.GetWorkOrdersByReportRange(rangeType, _FieldColumnID, rangeTypeIds, SessionHelper.RoomID.ToString(), SessionHelper.CompanyID.ToString());

            }
            return Json(new { Status = true, Message = "ok", data = DataGuids }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDataGuidsBasedOnRangeForRunPullReports(string rangeType, string rangeTypeIds, long ReportID)
        {
            PullMasterDAL objPullDAL = new PullMasterDAL(SessionHelper.EnterPriseDBName);
            string DataGuidsPull = string.Empty;


            if (!string.IsNullOrEmpty(rangeType) && !string.IsNullOrEmpty(rangeTypeIds))
            {
                ReportBuilderDTO objRPTDTO = new ReportBuilderDTO();
                ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                objRPTDTO = objDAL.GetReportDetail(ReportID);


                List<ReportGroupMasterDTO> lstReportGroupMasterDTO = null;
                string _FieldColumnID = string.Empty;
                if (objRPTDTO.ParentID.GetValueOrDefault(0) > 0)
                {
                    Int64 ParentID = GetBaseParentByReportID(objRPTDTO.ParentID.GetValueOrDefault(0));
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ParentID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ParentID && (x.FieldName ?? string.Empty).ToLower() == (rangeType ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }
                else
                {
                    lstReportGroupMasterDTO = objDAL.GetreportGroupFieldList(ReportID);
                    if (lstReportGroupMasterDTO != null && lstReportGroupMasterDTO.Count > 0)
                    {
                        var objField = lstReportGroupMasterDTO.Where(x => x.ReportID == ReportID && (x.FieldName ?? string.Empty).ToLower() == (rangeType ?? string.Empty).ToLower()).FirstOrDefault();
                        if (objField != null)
                        {
                            _FieldColumnID = objField.FieldColumnID;
                        }
                    }
                }


                DataGuidsPull = objPullDAL.GetPullGuidsByReportRange(rangeType, _FieldColumnID, rangeTypeIds, SessionHelper.RoomID.ToString(), SessionHelper.CompanyID.ToString());
            }
            return Json(new { Status = true, Message = "ok", data = DataGuidsPull }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// ReportFullBasePath
        /// ConnetionString
        /// BarcodeURL
        /// LogoURL
        /// </summary>
        /// <param name="paras"></param>
        /// <param name="ReportName"></param>
        /// <returns></returns>
        public JsonResult GenerateExcel(string ReportID, bool IsSendMail = true)
        {
            ParentID = 0;
            ReportBuilderDTO objDTO = new ReportBuilderDTO();
            ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            objDTO = objDAL.GetReportDetail(Convert.ToInt64(ReportID));
            string MasterReportResFile = objDTO.MasterReportResFile;
            SubReportResFile = MasterReportResFile;// objDTO.SubReportResFile;
            string Reportname = objDTO.ReportName;
            string MasterReportname = objDTO.ReportFileName;
            string SubReportname = objDTO.SubReportFileName;
            string mainGuid = "RPT_" + Guid.NewGuid().ToString().Replace("-", "_");
            string subGuid = "SubRPT_" + Guid.NewGuid().ToString().Replace("-", "_");
            string ReportPath = string.Empty;
            bool hasSubReport = false;
            ParentID = objDTO.ParentID ?? 0;
            eTurnsRegionInfo rsInfo = SessionHelper.eTurnsRegionInfoProp;
            string RDLCBaseFilePath = CommonUtility.RDLCBaseFilePath;
            if (objDTO.ParentID > 0)
            {
                if (objDTO.ISEnterpriseReport.GetValueOrDefault(false))
                {
                    ReportPath = CopyFiletoTemp(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/EnterpriseReport" + @"\\" + MasterReportname, mainGuid);
                }
                else
                {
                    ReportPath = CopyFiletoTemp(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID + @"\\" + MasterReportname, mainGuid);
                }
            }
            else
            {
                ReportPath = CopyFiletoTemp(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/BaseReport" + @"\\" + MasterReportname, mainGuid);
            }

            XDocument doc = XDocument.Load(ReportPath);
            if (objDTO != null
                && objDTO.IsBaseReport
                && !string.IsNullOrWhiteSpace(objDTO.ReportResourceName))
            {
                doc = UpdateReportTileFromResource(doc, objDTO.ReportResourceName);
            }
            IEnumerable<XElement> lstTablix = doc.Descendants(ns + "Tablix");
            string strTablix = string.Empty;
            if (lstTablix != null && lstTablix.ToList().Count > 0)
            {
                strTablix = lstTablix.ToList()[0].ToString();
            }



            IEnumerable<XElement> lstUpdateTablix = UpdateResource(lstTablix, MasterReportResFile);
            doc.Save(ReportPath);
            //doc.Descendants(ns + "Tablix").FirstOrDefault().Value = lstUpdateTablix.FirstOrDefault().Value;
            IEnumerable<XElement> lstReportPara = doc.Descendants(ns + "ReportParameter");
            List<ReportParameter> rpt = new List<ReportParameter>();
            rptPara = GetReportParaDictionary();

            doc = amDAL.AddFormatToTaxbox(doc, rsInfo);
            doc.Save(ReportPath);
            doc = XDocument.Load(ReportPath);
            if (lstReportPara != null && lstReportPara.Count() > 0)
            {
                foreach (var item in lstReportPara)
                {
                    ReportParameter rpara = new ReportParameter();
                    rpara.Name = item.Attribute("Name").Value;
                    if (!string.IsNullOrEmpty(rptPara.FirstOrDefault(x => x.Key.Replace("@", "") == item.Attribute("Name").Value.Replace("@", "")).Value))
                        rpara.Values.Add(rptPara.FirstOrDefault(x => x.Key.Replace("@", "") == item.Attribute("Name").Value.Replace("@", "")).Value);

                    rpt.Add(rpara);
                }
            }

            string connString = doc.Descendants(ns + "ConnectString").FirstOrDefault().Value;
            SqlConnection myConnection = new SqlConnection();
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter sqla = new SqlDataAdapter();
            DataTable dt = new DataTable();
            myConnection.ConnectionString = connectionString;// connString;

            cmd.Connection = myConnection;
            cmd.CommandText = doc.Descendants(ns + "CommandText").FirstOrDefault().Value; //"SELECT *  FROM   ItemMaster_View";
            cmd.CommandType = CommandType.Text;
            if (doc.Descendants(ns + "CommandType").FirstOrDefault() != null)
            {
                cmd.CommandType = (CommandType)Enum.Parse(typeof(CommandType), doc.Descendants(ns + "CommandType").FirstOrDefault().Value == null ? "Text" : doc.Descendants(ns + "CommandType").FirstOrDefault().Value, true);
            }

            if (doc.Descendants(ns + "Subreport") != null && doc.Descendants(ns + "Subreport").Count() > 0)
            {
                hasSubReport = true;
            }
            IEnumerable<XElement> lstQueryPara = doc.Descendants(ns + "QueryParameter");

            if (lstQueryPara != null && lstQueryPara.Count() > 0)
            {
                foreach (var item in lstQueryPara)
                {
                    SqlParameter slpar = new SqlParameter();
                    slpar.ParameterName = item.Attribute("Name").Value;//
                    if (!(hasSubReport && slpar.ParameterName.ToLower().Replace("@", "") == "sortfields") && !string.IsNullOrEmpty(rptPara.FirstOrDefault(x => x.Key.Replace("@", "") == item.Attribute("Name").Value.Replace("@", "")).Value))
                        slpar.Value = rptPara.FirstOrDefault(x => x.Key.Replace("@", "") == item.Attribute("Name").Value.Replace("@", "")).Value;
                    else
                        slpar.Value = DBNull.Value;
                    XElement objReportPara = lstReportPara.FirstOrDefault(x => x.Attribute("Name").Value == slpar.ParameterName.Replace("@", ""));

                    if (objReportPara.Descendants(ns + "DataType") != null && objReportPara.Descendants(ns + "DataType").Count() > 0)
                    {
                        slpar.DbType = (DbType)Enum.Parse(typeof(DbType), objReportPara.Descendants(ns + "DataType").FirstOrDefault().Value, true);
                    }

                    cmd.Parameters.Add(slpar);
                }
            }
            sqla = new SqlDataAdapter(cmd);
            sqla.Fill(dt);

            ReportViewer ReportViewer1 = new ReportViewer();
            ReportViewer1.Reset();
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.Visible = true;
            //string strSubTablix = string.Empty;
            //string rdlPath = string.Empty;
            if (doc.Descendants(ns + "Subreport") != null && doc.Descendants(ns + "Subreport").Count() > 0)
            {
                hasSubReport = true;
                //rdlPath = Server.MapPath(@"/RDLC_Reports/" + SessionHelper.EnterPriceID.ToString() + "") + @"\\" + SubReportname  ;
                if (objDTO.ParentID > 0)
                {
                    if (objDTO.ISEnterpriseReport.GetValueOrDefault(false))
                    {
                        rdlPath = CopyFiletoTemp(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/EnterpriseReport" + @"\\" + SubReportname, subGuid);
                    }
                    else
                    {
                        rdlPath = CopyFiletoTemp(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID + @"\\" + SubReportname, subGuid);
                    }
                }
                else
                {

                    rdlPath = CopyFiletoTemp(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/BaseReport" + @"\\" + SubReportname, subGuid);
                }
                doc.Descendants(ns + "Tablix").Descendants(ns + "Subreport").FirstOrDefault().Attribute("Name").Value = Convert.ToString(subGuid);
                doc.Descendants(ns + "Tablix").Descendants(ns + "ReportName").FirstOrDefault().Value = Convert.ToString(subGuid);
                doc.Save(ReportPath);

                XDocument docSub = XDocument.Load(rdlPath);
                IEnumerable<XElement> lstSubTablix = docSub.Descendants(ns + "Tablix");

                if (lstSubTablix != null && lstSubTablix.ToList().Count > 0)
                {
                    strSubTablix = lstSubTablix.ToList()[0].ToString();
                }

                docSub = amDAL.AddFormatToTaxbox(docSub, rsInfo);
                docSub.Save(rdlPath);
                docSub = XDocument.Load(rdlPath);

                IEnumerable<XElement> lstUpdateSubTablix = UpdateResource(lstSubTablix, SubReportResFile);
                docSub.Save(rdlPath);

                ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(LoadSubreport);

                ReportViewer1.LocalReport.EnableExternalImages = true;
                ReportViewer1.LocalReport.EnableHyperlinks = true;
                ReportViewer1.LocalReport.Refresh();
            }
            if (!hasSubReport && rptPara.ContainsKey("SortFields") && !string.IsNullOrEmpty(rptPara["SortFields"]))
            {
                string SortFields = rptPara["SortFields"];

                if (!string.IsNullOrEmpty(SortFields))
                {
                    dt.DefaultView.Sort = SortFields;
                    dt = dt.DefaultView.ToTable();

                }
            }

            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.ReportPath = ReportPath;
            ReportDataSource rds = new ReportDataSource();
            rds.Name = doc.Descendants(ns + "DataSet").FirstOrDefault().FirstAttribute.Value;
            rds.Value = dt;

            ReportViewer1.LocalReport.DataSources.Add(rds);
            ReportViewer1.LocalReport.SetParameters(rpt);
            ReportViewer1.ZoomMode = ZoomMode.Percent;
            ReportViewer1.LocalReport.Refresh();

            Warning[] warnings;
            string[] streamIds;
            string mimeType = "text/csv";
            string encoding = "utf-8";
            string extension = "xls";
            byte[] bytes = ReportViewer1.LocalReport.Render("Excel", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

            string ReportExportPath = System.Web.HttpContext.Current.Server.MapPath(@"/Downloads/") + objDTO.ReportName + DateTimeUtility.DateTimeNow.ToString("MM/dd/yyyy HH:mm:ss").Replace("/", "").Replace(":", "") + ".xls";
            using (FileStream fs = new FileStream(ReportExportPath, FileMode.Create))
            {
                fs.Write(bytes, 0, bytes.Length);
            }
            if (IsSendMail)
                SendReportMail(ReportExportPath, objDTO.ReportName);

            return Json(new { Status = true, Message = "ok", ReportPDFFilePath = ReportExportPath }, JsonRequestBehavior.AllowGet);
        }


        public void SendReportMail(string ReportExportPath, string Reportname)
        {
            ArrayList _Attachments = null;
            Attachment atr = null;
            eTurnsUtility objUtils = null;
            List<eMailAttachmentDTO> objeMailAttchList = null;
            eMailAttachmentDTO objeMailAttch = null;
            eMailMasterDAL objEmailDAL = null;
            byte[] byt = null;
            try
            {
                string StrSubject = Reportname + " Report.";
                string strToAddress = System.Configuration.ConfigurationManager.AppSettings["BCCAddress"];
                string strCCAddress = "";
                string MessageBody = Reportname;
                objeMailAttchList = new List<eMailAttachmentDTO>();
                objeMailAttch = new eMailAttachmentDTO();
                _Attachments = new ArrayList();
                atr = new Attachment(ReportExportPath);
                //_Attachments.Add(atr);
                byt = new byte[atr.ContentStream.Length];
                atr.ContentStream.Read(byt, 0, byt.Length - 1);
                objeMailAttch.FileData = byt;
                objeMailAttch.eMailToSendID = 0;
                objeMailAttch.MimeType = atr.ContentType.MediaType;// "application/pdf";
                objeMailAttch.AttachedFileName = atr.ContentType.Name;
                objeMailAttchList.Add(objeMailAttch);

                //CommonUtility.SendMail(FromAddress, strToAddress, strCCAddress, strBCCAddress, strNotificationAddress, StrSubject, MessageBody, true, _Attachments);
                objUtils = new eTurnsUtility();
                objUtils.SendMail(strToAddress, strCCAddress, StrSubject, MessageBody, _Attachments);
                objEmailDAL = new eMailMasterDAL(SessionHelper.EnterPriseDBName);
                RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
                eTurnsRegionInfo objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(SessionHelper.RoomID, SessionHelper.CompanyID, -1);
                string DateTimeFormat = "MM/dd/yyyy";
                DateTime TZDateTimeNow = DateTime.UtcNow;
                if (objeTurnsRegionInfo != null)
                {
                    DateTimeFormat = objeTurnsRegionInfo.ShortDatePattern;// + " " + objeTurnsRegionInfo.ShortTimePattern;
                    TZDateTimeNow = objeTurnsRegionInfo.TZDateTimeNow ?? DateTime.UtcNow;
                }
                if (StrSubject != null)
                {
                    string CurrentDate = TZDateTimeNow.ToString(DateTimeFormat);
                    // EmailSubject = EmailSubject.Replace("@@DATE@@", CurrentDate).Replace("@@date@@", CurrentDate).Replace("@@Date@@", CurrentDate);
                    StrSubject = Regex.Replace(StrSubject, "@@DATE@@", CurrentDate, RegexOptions.IgnoreCase);
                    if (!string.IsNullOrWhiteSpace(SessionHelper.CompanyName))
                    {
                        StrSubject = Regex.Replace(StrSubject, "@@COMPANYNAME@@", SessionHelper.CompanyName, RegexOptions.IgnoreCase);
                    }
                    if (!string.IsNullOrWhiteSpace(SessionHelper.RoomName))
                    {
                        StrSubject = Regex.Replace(StrSubject, "@@ROOMNAME@@", SessionHelper.RoomName, RegexOptions.IgnoreCase);
                    }
                    StrSubject = Regex.Replace(StrSubject, "@@Year@@", Convert.ToString(DateTime.UtcNow.Year), RegexOptions.IgnoreCase);
                }

                if (MessageBody != null)
                {
                    string CurrentDate = TZDateTimeNow.ToString(DateTimeFormat);
                    //EmailBody = EmailBody.Replace("@@DATE@@", CurrentDate).Replace("@@date@@", CurrentDate).Replace("@@Date@@", CurrentDate);
                    MessageBody = MessageBody.Replace("@@DATE@@", CurrentDate);
                    if (!string.IsNullOrWhiteSpace(SessionHelper.CompanyName))
                    {
                        MessageBody = MessageBody.Replace("@@COMPANYNAME@@", SessionHelper.CompanyName);
                    }
                    if (!string.IsNullOrWhiteSpace(SessionHelper.RoomName))
                    {
                        MessageBody = MessageBody.Replace("@@ROOMNAME@@", SessionHelper.RoomName);
                    }
                    MessageBody = MessageBody.Replace("@@Year@@", Convert.ToString(DateTime.UtcNow.Year));

                }
                objEmailDAL.eMailToSend(strToAddress, strCCAddress, StrSubject, MessageBody.ToString(), SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, objeMailAttchList, "Web => Reprotbuilder => SendReportMail");
            }
            finally
            {
                if (_Attachments != null)
                    _Attachments.Clear();

                _Attachments = null;
                if (atr != null)
                    atr.Dispose();

                atr = null;
                objUtils = null;
                objeMailAttchList = null;
                objeMailAttch = null;
                objEmailDAL = null;
            }

        }
        /// <summary>
        /// DemoSubreportProcessingEventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LoadSubreport(object sender, SubreportProcessingEventArgs e)
        {
            //subRptCounter += 1;
            string rdlPath = string.Empty;
            string RDLCBaseFilePath = CommonUtility.RDLCBaseFilePath;
            rdlPath = RDLCBaseFilePath + "/Temp" + @"\\" + e.ReportPath + ".rdlc";


            XDocument doc = XDocument.Load(rdlPath);

            string connString = doc.Descendants(ns + "ConnectString").FirstOrDefault().Value;
            SqlConnection myConnection = new SqlConnection();
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter sqla = new SqlDataAdapter();
            DataTable dt = new DataTable();
            myConnection.ConnectionString = connectionString; //connString;

            cmd.Connection = myConnection;
            cmd.CommandText = doc.Descendants(ns + "CommandText").FirstOrDefault().Value; //"SELECT *  FROM   ItemMaster_View";
            cmd.CommandType = CommandType.Text;
            if (doc.Descendants(ns + "CommandType").FirstOrDefault() != null)
            {
                cmd.CommandType = (CommandType)Enum.Parse(typeof(CommandType), doc.Descendants(ns + "CommandType").FirstOrDefault().Value == null ? "Text" : doc.Descendants(ns + "CommandType").FirstOrDefault().Value, true);
            }

            IEnumerable<XElement> lstReportPara = doc.Descendants(ns + "ReportParameter");
            List<ReportParameter> rpt = new List<ReportParameter>();

            if (lstReportPara != null && lstReportPara.Count() > 0)
            {
                foreach (var item in lstReportPara)
                {
                    ReportParameter rpara = new ReportParameter();
                    rpara.Name = item.Attribute("Name").Value;
                    rpara.Values.Add(e.Parameters[rpara.Name].Values[0]);
                    rpt.Add(rpara);
                }
            }

            IEnumerable<XElement> lstQueryPara = doc.Descendants(ns + "QueryParameter");

            if (lstQueryPara != null && lstQueryPara.Count() > 0)
            {
                foreach (var item in lstQueryPara)
                {
                    SqlParameter slpar = new SqlParameter();
                    slpar.ParameterName = item.Attribute("Name").Value;//
                    slpar.Value = e.Parameters[slpar.ParameterName.Replace("@", "")].Values[0];
                    XElement objReportPara = lstReportPara.FirstOrDefault(x => x.Attribute("Name").Value == slpar.ParameterName.Replace("@", ""));
                    if (objReportPara.Descendants(ns + "DataType") != null && objReportPara.Descendants(ns + "DataType").Count() > 0)
                    {
                        slpar.DbType = (DbType)Enum.Parse(typeof(DbType), objReportPara.Descendants(ns + "DataType").FirstOrDefault().Value, true);
                    }
                    if (cmd.Parameters.IndexOf(slpar.ParameterName) < 0)
                        cmd.Parameters.Add(slpar);
                }
            }
            sqla = new SqlDataAdapter(cmd);
            sqla.Fill(dt);
            if (rptPara.ContainsKey("SortFields") && !string.IsNullOrEmpty(rptPara["SortFields"]))
            {
                string SortFields = rptPara["SortFields"];

                if (!string.IsNullOrEmpty(SortFields))
                {
                    dt.DefaultView.Sort = SortFields;
                    dt = dt.DefaultView.ToTable();

                }
            }
            ReportDataSource rds = new ReportDataSource();
            rds.Name = doc.Descendants(ns + "DataSet").FirstOrDefault().FirstAttribute.Value;
            rds.Value = dt;
            e.DataSources.Add(rds);

            #region WI-3336 For Work Order Attachement 

            if (e.DataSourceNames.Count > 1)
            {
                if (e.DataSourceNames[1].ToLower().Equals("datasetworkorderattachments"))
                {
                    SqlCommand cmdWOA = new SqlCommand();
                    SqlDataAdapter sqlaWOA = new SqlDataAdapter();
                    DataTable dtWOA = new DataTable();

                    cmdWOA.Connection = myConnection;
                    cmdWOA.CommandText = "RPT_GetWorkOrderAttachments";
                    cmdWOA.CommandType = CommandType.Text;
                    if (doc.Descendants(ns + "CommandType").FirstOrDefault() != null)
                    {
                        cmdWOA.CommandType = (CommandType)Enum.Parse(typeof(CommandType), doc.Descendants(ns + "CommandType").FirstOrDefault().Value == null ? "Text" : doc.Descendants(ns + "CommandType").FirstOrDefault().Value, true);
                    }

                    IEnumerable<XElement> lstQueryParaWOA = doc.Descendants(ns + "QueryParameter");

                    if (lstQueryParaWOA != null && lstQueryParaWOA.Count() > 0)
                    {
                        foreach (var item in lstQueryParaWOA)
                        {
                            if (item.Attribute("Name").Value.ToLower().Equals("@workorderguid"))
                            {
                                SqlParameter slparWOA = new SqlParameter();
                                slparWOA.ParameterName = item.Attribute("Name").Value;
                                slparWOA.Value = e.Parameters[slparWOA.ParameterName.Replace("@", "")].Values[0];
                                XElement objReportParaWO = lstReportPara.FirstOrDefault(x => x.Attribute("Name").Value == slparWOA.ParameterName.Replace("@", ""));
                                if (objReportParaWO.Descendants(ns + "DataType") != null && objReportParaWO.Descendants(ns + "DataType").Count() > 0)
                                {
                                    slparWOA.DbType = (DbType)Enum.Parse(typeof(DbType), objReportParaWO.Descendants(ns + "DataType").FirstOrDefault().Value, true);
                                }
                                if (cmdWOA.Parameters.IndexOf(slparWOA.ParameterName) < 0)
                                    cmdWOA.Parameters.Add(slparWOA);
                            }
                        }
                    }

                    cmdWOA.CommandTimeout = 7200;
                    sqlaWOA = new SqlDataAdapter(cmdWOA);

                    sqlaWOA.Fill(dtWOA);

                    ReportDataSource rds2 = new ReportDataSource();
                    rds2.Name = "DataSetWorkOrderAttachments";
                    rds2.Value = dtWOA;
                    e.DataSources.Add(rds2);
                }
            }

            #endregion

            //if (globalcounter == subRptCounter)
            //{

            //    RefreshSubReport(strSubTablix, rdlPath);

            //}
        }

        /// <summary>
        /// ReportFullBasePath
        /// ConnetionString
        /// BarcodeURL
        /// LogoURL
        /// </summary>
        /// <param name="paras"></param>
        /// <param name="ReportName"></param>
        /// <returns></returns>
        public JsonResult GenerateBytesFromReport(Int64 ReportID, string OutputFormat)
        {
            ReportBuilderDTO objDTO = null;
            ReportMasterDAL objDAL = null;
            XDocument doc = null;
            IEnumerable<XElement> lstTablix = null;
            IEnumerable<XElement> lstUpdateTablix = null;
            IEnumerable<XElement> lstReportPara = null;
            List<ReportParameter> rpt = null;
            ReportParameter rpara = null;
            SqlConnection myConnection = null;
            SqlCommand cmd = null;
            SqlDataAdapter sqla = null;
            DataTable dt = null;
            IEnumerable<XElement> lstQueryPara = null;
            SqlParameter slpar = null;
            XElement objReportPara = null;
            ReportViewer ReportViewer1 = null;
            XDocument docSub = null;
            IEnumerable<XElement> lstSubTablix = null;
            IEnumerable<XElement> lstUpdateSubTablix = null;
            ReportDataSource rds = null;
            byte[] bytes = null;

            try
            {
                objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                ParentID = 0;

                objDTO = objDAL.GetReportDetail(ReportID);
                string MasterReportResFile = objDTO.MasterReportResFile;
                SubReportResFile = MasterReportResFile;
                string Reportname = objDTO.ReportName;
                string MasterReportname = objDTO.ReportFileName;
                string SubReportname = objDTO.SubReportFileName;
                string mainGuid = "RPT_" + Guid.NewGuid().ToString().Replace("-", "_");
                string subGuid = "SubRPT_" + Guid.NewGuid().ToString().Replace("-", "_");
                string ReportPath = string.Empty;
                bool hasSubReport = false;
                ParentID = objDTO.ParentID ?? 0;
                string RDLCBaseFilePath = CommonUtility.RDLCBaseFilePath;
                if (objDTO.ParentID > 0)
                {
                    if (objDTO.ISEnterpriseReport.GetValueOrDefault(false))
                    {
                        ReportPath = CopyFiletoTemp(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/EnterpriseReport" + @"\\" + MasterReportname, mainGuid);
                    }
                    else
                    {
                        ReportPath = CopyFiletoTemp(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID + @"\\" + MasterReportname, mainGuid);
                    }
                }
                else
                {
                    ReportPath = CopyFiletoTemp(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/BaseReport" + @"\\" + MasterReportname, mainGuid);
                }

                doc = XDocument.Load(ReportPath);
                if (objDTO != null
                && objDTO.IsBaseReport
                && !string.IsNullOrWhiteSpace(objDTO.ReportResourceName))
                {
                    doc = UpdateReportTileFromResource(doc, objDTO.ReportResourceName);
                }
                lstTablix = doc.Descendants(ns + "Tablix");
                string strTablix = string.Empty;
                if (lstTablix != null && lstTablix.ToList().Count > 0)
                {
                    strTablix = lstTablix.ToList()[0].ToString();
                }

                lstUpdateTablix = UpdateResource(lstTablix, MasterReportResFile);
                eTurns.DAL.AlertMail rptHelper = new eTurns.DAL.AlertMail();
                if ((objDTO.HideHeader ?? false))
                {
                    //if (!hasSubReport && !objDTO.IsNotEditable.GetValueOrDefault(false)
                    //                 && (objDTO.ReportType == 3 || objDTO.ReportType == 1)
                    //                 && (objDTO.HideHeader ?? false) == true)
                    if (!objDTO.IsNotEditable.GetValueOrDefault(false) && (objDTO.HideHeader ?? false) == true
                            && (objDTO.ReportType == 1 || objDTO.ReportType == 2 || objDTO.ReportType == 3))
                    {
                        doc = rptHelper.GetAdditionalHeaderRow(doc, objDTO, SessionHelper.CompanyName, SessionHelper.RoomName, EnterpriseDBName: SessionHelper.EnterPriseDBName);
                        doc.Save(ReportPath);
                        doc = XDocument.Load(ReportPath);

                    }
                }
                else
                {
                    if (!objDTO.IsNotEditable.GetValueOrDefault(false)
                         && (objDTO.ReportType == 1 || objDTO.ReportType == 2 || objDTO.ReportType == 3))
                    {
                        XElement xRows = doc.Descendants(ns + "TablixRows").FirstOrDefault();
                        XElement xRow = doc.Descendants(ns + "TablixRow").FirstOrDefault();

                        if (objDTO.ReportType == 2 && xRow.Descendants(ns + "TablixCell").Descendants(ns + "Textbox").
                                FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "TextboxRoom") != null)
                        {
                            doc.Descendants(ns + "TablixRow").FirstOrDefault().Remove();
                            doc.Descendants(ns + "TablixRowHierarchy").Descendants(ns + "TablixMembers").Descendants(ns + "TablixMember").Descendants(ns + "TablixMembers").Descendants(ns + "TablixMember").Remove();
                            foreach (var item in xRows.Descendants(ns + "TablixRow"))
                            {
                                doc.Descendants(ns + "TablixRowHierarchy").Descendants(ns + "TablixMembers").Descendants(ns + "TablixMember").Descendants(ns + "TablixMembers").FirstOrDefault().Add(new XElement(ns + "TablixMember"));
                            }
                            doc.Save(ReportPath);
                            doc = XDocument.Load(ReportPath);
                        }
                    }
                }

                if ((objDTO.ShowSignature ?? false))
                {
                    doc = rptHelper.GetFooterForSignature(doc, objDTO);
                    doc.Save(ReportPath);
                    doc = XDocument.Load(ReportPath);
                }
                doc.Save(ReportPath);

                lstReportPara = doc.Descendants(ns + "ReportParameter");
                rpt = new List<ReportParameter>();

                rptPara = GetReportParaDictionary();

                doc = amDAL.AddFormatToTaxbox(doc, rsInfo);
                doc.Save(ReportPath);
                doc = XDocument.Load(ReportPath);

                if (lstReportPara != null && lstReportPara.Count() > 0)
                {
                    foreach (var item in lstReportPara)
                    {
                        rpara = new ReportParameter();
                        rpara.Name = item.Attribute("Name").Value;
                        if (!string.IsNullOrEmpty(rptPara.FirstOrDefault(x => x.Key.Replace("@", "") == item.Attribute("Name").Value.Replace("@", "")).Value))
                            rpara.Values.Add(rptPara.FirstOrDefault(x => x.Key.Replace("@", "") == item.Attribute("Name").Value.Replace("@", "")).Value);

                        rpt.Add(rpara);
                    }
                }

                string connString = doc.Descendants(ns + "ConnectString").FirstOrDefault().Value;
                myConnection = new SqlConnection();
                cmd = new SqlCommand();
                sqla = new SqlDataAdapter();
                dt = new DataTable();
                myConnection.ConnectionString = connectionString;// connString;

                cmd.Connection = myConnection;
                cmd.CommandText = doc.Descendants(ns + "CommandText").FirstOrDefault().Value; //"SELECT *  FROM   ItemMaster_View";
                cmd.CommandType = CommandType.Text;
                if (doc.Descendants(ns + "CommandType").FirstOrDefault() != null)
                {
                    cmd.CommandType = (CommandType)Enum.Parse(typeof(CommandType), doc.Descendants(ns + "CommandType").FirstOrDefault().Value == null ? "Text" : doc.Descendants(ns + "CommandType").FirstOrDefault().Value, true);
                }

                lstQueryPara = doc.Descendants(ns + "QueryParameter");
                if (doc.Descendants(ns + "Subreport") != null && doc.Descendants(ns + "Subreport").Count() > 0)
                {
                    hasSubReport = true;
                }
                if (lstQueryPara != null && lstQueryPara.Count() > 0)
                {
                    foreach (var item in lstQueryPara)
                    {
                        slpar = new SqlParameter();
                        slpar.ParameterName = item.Attribute("Name").Value;//
                        if (!(hasSubReport && slpar.ParameterName.ToLower().Replace("@", "") == "sortfields") && !string.IsNullOrEmpty(rptPara.FirstOrDefault(x => x.Key.Replace("@", "") == item.Attribute("Name").Value.Replace("@", "")).Value))
                            slpar.Value = rptPara.FirstOrDefault(x => x.Key.Replace("@", "") == item.Attribute("Name").Value.Replace("@", "")).Value;
                        else
                            slpar.Value = DBNull.Value;

                        objReportPara = lstReportPara.FirstOrDefault(x => x.Attribute("Name").Value == slpar.ParameterName.Replace("@", ""));

                        if (objReportPara.Descendants(ns + "DataType") != null && objReportPara.Descendants(ns + "DataType").Count() > 0)
                        {
                            slpar.DbType = (DbType)Enum.Parse(typeof(DbType), objReportPara.Descendants(ns + "DataType").FirstOrDefault().Value, true);
                        }

                        cmd.Parameters.Add(slpar);
                    }
                }

                sqla = new SqlDataAdapter(cmd);
                sqla.Fill(dt);

                ReportViewer1 = new ReportViewer();
                ReportViewer1.Reset();
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.Visible = true;

                if (doc.Descendants(ns + "Subreport") != null && doc.Descendants(ns + "Subreport").Count() > 0)
                {
                    hasSubReport = true;
                    if (objDTO.ParentID > 0)
                    {
                        if (objDTO.ISEnterpriseReport.GetValueOrDefault(false))
                        {
                            rdlPath = CopyFiletoTemp(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/EnterpriseReport" + @"\\" + SubReportname, subGuid);
                        }
                        else
                        {
                            rdlPath = CopyFiletoTemp(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID + @"\\" + SubReportname, subGuid);
                        }
                    }
                    else
                    {
                        rdlPath = CopyFiletoTemp(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/BaseReport" + @"\\" + SubReportname, subGuid);
                    }
                    doc.Descendants(ns + "Tablix").Descendants(ns + "Subreport").FirstOrDefault().Attribute("Name").Value = Convert.ToString(subGuid);
                    doc.Descendants(ns + "Tablix").Descendants(ns + "ReportName").FirstOrDefault().Value = Convert.ToString(subGuid);
                    doc.Save(ReportPath);

                    docSub = XDocument.Load(rdlPath);
                    lstSubTablix = docSub.Descendants(ns + "Tablix");
                    lstUpdateSubTablix = UpdateResource(lstSubTablix, SubReportResFile);
                    docSub.Save(rdlPath);

                    if (lstSubTablix != null && lstSubTablix.ToList().Count > 0)
                    {
                        strSubTablix = lstSubTablix.ToList()[0].ToString();
                    }


                    docSub = amDAL.AddFormatToTaxbox(docSub, rsInfo);
                    docSub.Save(rdlPath);
                    docSub = XDocument.Load(rdlPath);

                    //lstUpdateSubTablix = UpdateResource(lstSubTablix, SubReportResFile);
                    // docSub.Save(rdlPath);

                    ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(LoadSubreport);

                    ReportViewer1.LocalReport.EnableExternalImages = true;
                    ReportViewer1.LocalReport.EnableHyperlinks = true;
                    ReportViewer1.LocalReport.Refresh();
                }
                if (!hasSubReport && rptPara.ContainsKey("SortFields") && !string.IsNullOrEmpty(rptPara["SortFields"]))
                {
                    string SortFields = rptPara["SortFields"];

                    if (!string.IsNullOrEmpty(SortFields))
                    {
                        dt.DefaultView.Sort = SortFields;
                        dt = dt.DefaultView.ToTable();
                    }
                }

                ReportViewer1.LocalReport.EnableExternalImages = true;
                ReportViewer1.LocalReport.ReportPath = ReportPath;
                rds = new ReportDataSource();
                rds.Name = doc.Descendants(ns + "DataSet").FirstOrDefault().FirstAttribute.Value;
                rds.Value = dt;

                ReportViewer1.LocalReport.DataSources.Add(rds);
                ReportViewer1.LocalReport.SetParameters(rpt);
                ReportViewer1.ZoomMode = ZoomMode.Percent;
                ReportViewer1.LocalReport.Refresh();

                Warning[] warnings;
                string[] streamIds;
                string mimeType = string.Empty;
                string encoding = string.Empty;
                string extension = string.Empty;
                bytes = ReportViewer1.LocalReport.Render(OutputFormat, null, out mimeType, out encoding, out extension, out streamIds, out warnings);
                string ReportExportPath = System.Web.HttpContext.Current.Server.MapPath(@"/Downloads/") + objDTO.ReportName + DateTimeUtility.DateTimeNow.ToString("MMddyyyy_HHmmss") + "." + extension;
                using (FileStream fs = new FileStream(ReportExportPath, FileMode.Create))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }

                return Json(new
                {
                    Status = true,
                    Message = "ok",
                    FilePath = ReportExportPath,
                    FileMimeType = mimeType,
                    FilEncoding = encoding,
                    FileExtension = extension,
                    Warnings = warnings,
                    StreamIDs = streamIds,
                    SheetName = mainGuid,
                }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                    cmd = null;
                }
                if (sqla != null)
                {
                    sqla.Dispose();
                    sqla = null;
                }
                if (dt != null)
                {
                    dt.Dispose();
                    dt = null;
                }

                if (myConnection != null)
                {
                    myConnection.Close();
                    myConnection.Dispose();
                    myConnection = null;
                }

                objDTO = null;
                objDAL = null;
                doc = null;
                lstTablix = null;
                lstUpdateTablix = null;
                lstReportPara = null;
                rpt = null;
                rpara = null;
                slpar = null;
                lstQueryPara = null;
                objReportPara = null;
                ReportViewer1 = null;
                docSub = null;
                lstSubTablix = null;
                lstUpdateSubTablix = null;
                rds = null;
                bytes = null;
                if (ReportViewer1 != null)
                {
                    ReportViewer1.Dispose();
                }
            }
        }


        #endregion

        public ActionResult ScheduleReportList()
        {
            return View();
        }

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult GetScheduleReportList(JQueryDataTableParamModel param)
        {
            SchedulerDTO entity = new SchedulerDTO();
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
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());

            // set the default column sorting here, if first time then required to set 
            if (sortColumnName == "0" || sortColumnName == "undefined")
                sortColumnName = "ID desc";

            string searchQuery = string.Empty;
            int TotalRecordCount = 0;
            Int64 ScheduleFor = 5;

            ScheduleReportDAL objData = new ScheduleReportDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<SchedulerDTO> DataFromDB = objData.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsDeleted, ScheduleFor);

            return Json(new { sEcho = param.sEcho, iTotalRecords = TotalRecordCount, iTotalDisplayRecords = TotalRecordCount, aaData = DataFromDB, }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetScheduleReportNarrwSearchHTML()
        {
            return PartialView("_ScheduleReportNarrowSearch");
        }
        public PartialViewResult CreateEmail()
        {
            return PartialView("_CreateEmail");
        }
        [HttpPost]
        public JsonResult sendEmail(string sendTo, string subject, string body, string attchfilepath, string attchfilepathforExcel)
        {
            ArrayList _Attachments = null;
            Attachment atr = null;
            eTurnsUtility objUtils = null;
            List<eMailAttachmentDTO> objeMailAttchList = null;
            eMailAttachmentDTO objeMailAttch = null;
            eMailMasterDAL objEmailDAL = null;
            byte[] byt = null;
            try
            {
                string StrSubject = subject;
                string strToAddress = sendTo;
                string strCCAddress = string.Empty;
                string MessageBody = body;
                objeMailAttchList = new List<eMailAttachmentDTO>();
                objeMailAttch = new eMailAttachmentDTO();
                _Attachments = new ArrayList();

                if (!string.IsNullOrEmpty(attchfilepath))
                {
                    atr = new Attachment(attchfilepath);
                    //_Attachments.Add(atr);
                    byt = new byte[atr.ContentStream.Length];
                    atr.ContentStream.Read(byt, 0, byt.Length - 1);
                    objeMailAttch.FileData = byt;
                    objeMailAttch.eMailToSendID = 0;
                    objeMailAttch.MimeType = atr.ContentType.MediaType;// "application/pdf";
                    objeMailAttch.AttachedFileName = atr.ContentType.Name;
                    objeMailAttchList.Add(objeMailAttch);
                }

                if (!string.IsNullOrEmpty(attchfilepathforExcel)) // This code block is for the excel attachment for the RunReport
                {
                    objeMailAttch = new eMailAttachmentDTO();
                    atr = new Attachment(attchfilepathforExcel);
                    byt = new byte[atr.ContentStream.Length];
                    atr.ContentStream.Read(byt, 0, byt.Length - 1);
                    objeMailAttch.FileData = byt;
                    objeMailAttch.eMailToSendID = 0;
                    objeMailAttch.MimeType = atr.ContentType.MediaType;
                    objeMailAttch.AttachedFileName = atr.ContentType.Name;
                    objeMailAttchList.Add(objeMailAttch);
                }
                //CommonUtility.SendMail(FromAddress, strToAddress, strCCAddress, strBCCAddress, strNotificationAddress, StrSubject, MessageBody, true, _Attachments);
                objUtils = new eTurnsUtility();
                objUtils.SendMail(strToAddress, strCCAddress, StrSubject, MessageBody, _Attachments);
                objEmailDAL = new eMailMasterDAL(SessionHelper.EnterPriseDBName);
                RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
                eTurnsRegionInfo objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(SessionHelper.RoomID, SessionHelper.CompanyID, -1);
                string DateTimeFormat = "MM/dd/yyyy";
                DateTime TZDateTimeNow = DateTime.UtcNow;
                if (objeTurnsRegionInfo != null)
                {
                    DateTimeFormat = objeTurnsRegionInfo.ShortDatePattern;// + " " + objeTurnsRegionInfo.ShortTimePattern;
                    TZDateTimeNow = objeTurnsRegionInfo.TZDateTimeNow ?? DateTime.UtcNow;
                }
                if (StrSubject != null)
                {
                    string CurrentDate = TZDateTimeNow.ToString(DateTimeFormat);
                    // EmailSubject = EmailSubject.Replace("@@DATE@@", CurrentDate).Replace("@@date@@", CurrentDate).Replace("@@Date@@", CurrentDate);
                    StrSubject = Regex.Replace(StrSubject, "@@DATE@@", CurrentDate, RegexOptions.IgnoreCase);
                    if (!string.IsNullOrWhiteSpace(SessionHelper.CompanyName))
                    {
                        StrSubject = Regex.Replace(StrSubject, "@@COMPANYNAME@@", SessionHelper.CompanyName, RegexOptions.IgnoreCase);
                    }
                    if (!string.IsNullOrWhiteSpace(SessionHelper.RoomName))
                    {
                        StrSubject = Regex.Replace(StrSubject, "@@ROOMNAME@@", SessionHelper.RoomName, RegexOptions.IgnoreCase);
                    }
                    StrSubject = Regex.Replace(StrSubject, "@@Year@@", Convert.ToString(DateTime.UtcNow.Year), RegexOptions.IgnoreCase);
                }

                if (MessageBody != null)
                {
                    string CurrentDate = TZDateTimeNow.ToString(DateTimeFormat);
                    //EmailBody = EmailBody.Replace("@@DATE@@", CurrentDate).Replace("@@date@@", CurrentDate).Replace("@@Date@@", CurrentDate);
                    MessageBody = MessageBody.Replace("@@DATE@@", CurrentDate);
                    if (!string.IsNullOrWhiteSpace(SessionHelper.CompanyName))
                    {
                        MessageBody = MessageBody.Replace("@@COMPANYNAME@@", SessionHelper.CompanyName);
                    }
                    if (!string.IsNullOrWhiteSpace(SessionHelper.RoomName))
                    {
                        MessageBody = MessageBody.Replace("@@ROOMNAME@@", SessionHelper.RoomName);
                    }
                    MessageBody = MessageBody.Replace("@@Year@@", Convert.ToString(DateTime.UtcNow.Year));

                }
                objEmailDAL.eMailToSend(strToAddress, strCCAddress, StrSubject, MessageBody.ToString(), SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, objeMailAttchList, "Web => Reprotbuilder => SendReportMail");
                return Json(true);

            }
            catch (Exception)
            {
                return Json(false);
            }
            finally
            {
                if (_Attachments != null)
                    _Attachments.Clear();

                _Attachments = null;
                if (atr != null)
                    atr.Dispose();

                atr = null;
                objUtils = null;
                objeMailAttchList = null;
                objeMailAttch = null;
                objEmailDAL = null;
            }
        }

        public ActionResult GetRangeByReportID(long ReportId)
        {
            List<SelectListItem> lstGrouplist, lstDisabledGrouplist;
            string reportName = string.Empty;
            RangebyReportID(ReportId, out lstGrouplist, out lstDisabledGrouplist, out reportName);

            ViewBag.DisabledGroupList = lstDisabledGrouplist;
            ViewBag.IsInstockReport = (!string.IsNullOrEmpty(reportName) && (reportName.ToLower() == "instock" || reportName.ToLower() == "instock margin"
                || reportName.ToLower() == "instock with qoh" || reportName.ToLower() == "instock by activity"))
                ? true : false;
            return PartialView("_RangePerameters", lstGrouplist);
            //return Json(new { Status = true, Message = "ok", Grouplist = lstGrouplist }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRangeByReportIDData(long ReportId)
        {
            List<SelectListItem> lstGrouplist, lstDisabledGrouplist;
            string reportName = string.Empty;
            RangebyReportID(ReportId, out lstGrouplist, out lstDisabledGrouplist, out reportName);

            ViewBag.DisabledGroupList = lstDisabledGrouplist;
            ViewBag.IsInstockReport = (!string.IsNullOrEmpty(reportName) && (reportName.ToLower() == "instock" || reportName.ToLower() == "instock margin"
                || reportName.ToLower() == "instock with qoh" || reportName.ToLower() == "instock by activity"))
                ? true : false;
            return Json(new { Status = true, DisabledGroupList = lstDisabledGrouplist, Grouplist = lstGrouplist }, JsonRequestBehavior.AllowGet);
        }
        private void RangebyReportID(long ReportId, out List<SelectListItem> lstGrouplist, out List<SelectListItem> lstDisabledGrouplist, out string reportName)
        {
            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            ReportBuilderDTO objReportBuilderDTO = new ReportBuilderDTO();
            ReportMasterDTO objParentDTO = new ReportMasterDTO();
            objReportBuilderDTO = objReportMasterDAL.GetReportDetail(ReportId);
            List<ReportGroupMasterDTO> lstReportGroupMasterDTO = null;

            reportName = "";
            if (objReportBuilderDTO.ParentID.GetValueOrDefault(0) > 0)
            {
                Int64 ParentID = GetBaseParentByReportID(objReportBuilderDTO.ParentID.GetValueOrDefault(0));
                lstReportGroupMasterDTO = objReportMasterDAL.GetreportGroupFieldList(ParentID);
                objParentDTO = objReportMasterDAL.GetParentReportSigleRecord(ReportId, 0, "Report");

                if (objParentDTO != null && !string.IsNullOrEmpty(objParentDTO.ReportName) && !string.IsNullOrWhiteSpace(objParentDTO.ReportName))
                {
                    reportName = objParentDTO.ReportName;
                }
            }
            else
            {
                lstReportGroupMasterDTO = objReportMasterDAL.GetreportGroupFieldList(ReportId);

                if (objReportBuilderDTO != null && !string.IsNullOrEmpty(objReportBuilderDTO.ReportName) && !string.IsNullOrWhiteSpace(objReportBuilderDTO.ReportName))
                {
                    reportName = objReportBuilderDTO.ReportName;
                }
            }

            lstReportGroupMasterDTO = lstReportGroupMasterDTO.OrderBy(x => x.GroupOrder).ToList();

            string ReportName = objReportBuilderDTO.ReportFileName;
            string ReportResFile = objReportBuilderDTO.MasterReportResFile;
            string rdlPath = string.Empty;
            string RDLCBaseFilePath = CommonUtility.RDLCBaseFilePath;
            if (objReportBuilderDTO.ParentID > 0)
            {
                if (objReportBuilderDTO.ISEnterpriseReport.GetValueOrDefault(false))
                {
                    rdlPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/EnterpriseReport" + @"\\" + ReportName;
                }
                else
                {
                    rdlPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID + @"\\" + ReportName;
                }
            }
            else
            {
                rdlPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/BaseReport" + @"\\" + ReportName;
            }

            XDocument doc = XDocument.Load(rdlPath);
            XNamespace ns = XNamespace.Get("http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition");
            XNamespace nsrd = XNamespace.Get("http://schemas.microsoft.com/SQLServer/reporting/reportdesigner");

            string strFields = string.Empty;
            string DatasetFields = string.Empty;

            List<XElement> lstTablixRowGrouping = doc.Descendants(ns + "Tablix").Descendants(ns + "TablixRowHierarchy").Descendants(ns + "GroupExpression").ToList();
            List<XElement> lstColumns = doc.Descendants(ns + "TablixColumn").ToList();
            List<XElement> lstReportField = doc.Descendants(ns + "TablixRow").ToList();
            lstGrouplist = new List<SelectListItem>();
            lstDisabledGrouplist = new List<SelectListItem>();
            if ((objReportBuilderDTO.ReportName.ToLower().Equals("work order")
                 ||
                 objReportBuilderDTO.ReportName.ToLower().Equals("work order with grouped pulls"))
                     ||
                     (
                     objParentDTO != null
                     &&
                      !string.IsNullOrEmpty(objParentDTO.ReportName)
                      &&
                      (objParentDTO.ReportName.ToLower().Equals("work order")
                      || objParentDTO.ReportName.ToLower().Equals("work order with grouped pulls"))
                     ))
            {
                if (lstReportField != null && lstReportField.Count > 0)
                {
                    List<string> lstChileReportColmns = new List<string>();
                    foreach (XElement col in lstReportField)
                    {
                        List<XElement> lstFieldChildBottom = col.Descendants(ns + "TablixCell").ToList();
                        IEnumerable<string> lstChildColmns = lstFieldChildBottom.Descendants(ns + "Value").ToList().Select(x => x.Value.Replace("=Fields!", "").Replace("=Parameters!BarcodeURL.Value+Fields!", "").Replace(".Value", ""));
                        foreach (var item in lstChildColmns)
                        {
                            lstChileReportColmns.Add(item);
                        }
                    }
                    IEnumerable<string> lstReportColmns = lstChileReportColmns;

                    string tablixcellBottom = string.Empty;
                    int rowGroupCount = lstTablixRowGrouping.Count;

                    foreach (var item in lstReportGroupMasterDTO)
                    {
                        if (lstReportColmns.Contains(item.FieldName))
                        {
                            SelectListItem objKeyValDTO = new SelectListItem();
                            objKeyValDTO.Value = item.FieldName;
                            objKeyValDTO.Text = GetField(item.FieldName, ReportResFile);
                            objKeyValDTO.Selected = false;
                            lstGrouplist.Add(objKeyValDTO);
                        }
                        else
                        {
                            SelectListItem objKeyValDTO = new SelectListItem();
                            objKeyValDTO.Value = item.FieldName;
                            objKeyValDTO.Text = GetField(item.FieldName, ReportResFile);
                            objKeyValDTO.Selected = false;
                            if (lstGrouplist.FindIndex(x => x.Text == objKeyValDTO.Text && x.Value == objKeyValDTO.Value) < 0)
                                lstDisabledGrouplist.Add(objKeyValDTO);
                        }
                    }
                }
            }
            else
            {
                if (lstReportField != null && lstReportField.Count > 0)
                {
                    List<XElement> lstFieldBottom = lstReportField.Count > 1 ? lstReportField[1].Descendants(ns + "TablixCell").ToList()
                        : lstReportField[0].Descendants(ns + "TablixCell").ToList();

                    IEnumerable<string> lstReportColmns = lstFieldBottom.Descendants(ns + "Value").ToList().Select(x => x.Value.Replace("=Fields!", "").Replace("=Parameters!BarcodeURL.Value+Fields!", "").Replace(".Value", ""));

                    string tablixcellBottom = string.Empty;
                    int rowGroupCount = lstTablixRowGrouping.Count;

                    foreach (var item in lstReportGroupMasterDTO)
                    {
                        if (objReportBuilderDTO.ReportName.ToLower().Equals("kit serial")
                          ||
                          (
                          objParentDTO != null
                          &&
                           !string.IsNullOrEmpty(objParentDTO.ReportName)
                           &&
                           objParentDTO.ReportName.ToLower().Equals("kit serial")
                          ))
                        {
                            SelectListItem objKeyValDTO = new SelectListItem();
                            objKeyValDTO.Value = item.FieldName;
                            objKeyValDTO.Text = GetField(item.FieldName, ReportResFile);
                            objKeyValDTO.Selected = false;
                            lstGrouplist.Add(objKeyValDTO);
                        }
                        else
                        {
                            if (lstReportColmns.Contains(item.FieldName))
                            {
                                SelectListItem objKeyValDTO = new SelectListItem();
                                objKeyValDTO.Value = item.FieldName;
                                objKeyValDTO.Text = GetField(item.FieldName, ReportResFile);
                                objKeyValDTO.Selected = false;
                                lstGrouplist.Add(objKeyValDTO);
                            }
                            else
                            {
                                SelectListItem objKeyValDTO = new SelectListItem();
                                objKeyValDTO.Value = item.FieldName;
                                objKeyValDTO.Text = GetField(item.FieldName, ReportResFile);
                                objKeyValDTO.Selected = false;
                                if (lstGrouplist.FindIndex(x => x.Text == objKeyValDTO.Text && x.Value == objKeyValDTO.Value) < 0)
                                    lstDisabledGrouplist.Add(objKeyValDTO);
                            }
                        }
                    }
                }
            }
        }

        public ActionResult PrintReportPreview(string pdfname)
        {
            ViewBag.URLToPdf = Url.Content("~/Content/OpenAccess/ReportsPDF/" + pdfname);
            return View();
        }


        public ActionResult GetReportRangeByReportIDForScheduler(long TemplateID, long NotificationID)
        {
            long ReportId = 0;
            // Int64 ReportMasterID = 0;
            if (NotificationID > 0)
            {
                NotificationDTO oNotificationDTO = new NotificationDAL(SessionHelper.EnterPriseDBName).GetNotifiactionByID(NotificationID);
                if (oNotificationDTO != null && oNotificationDTO.ReportID.HasValue)
                    ReportId = oNotificationDTO.ReportID.Value;
                else
                    ReportId = TemplateID;
            }
            else
            {
                ReportId = TemplateID;
            }


            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            ReportBuilderDTO objReportBuilderDTO = new ReportBuilderDTO();
            objReportBuilderDTO = objReportMasterDAL.GetReportDetail(ReportId);
            List<ReportGroupMasterDTO> lstReportGroupMasterDTO = null;
            if (objReportBuilderDTO.ParentID.GetValueOrDefault(0) > 0)
            {
                Int64 ParentID = GetBaseParentByReportID(objReportBuilderDTO.ParentID.GetValueOrDefault(0));
                lstReportGroupMasterDTO = objReportMasterDAL.GetreportGroupFieldList(ParentID);
            }
            else
            {
                lstReportGroupMasterDTO = objReportMasterDAL.GetreportGroupFieldList(ReportId);
            }

            lstReportGroupMasterDTO = lstReportGroupMasterDTO.OrderBy(x => x.GroupOrder).ToList();

            string ReportName = objReportBuilderDTO.ReportFileName;
            string ReportResFile = objReportBuilderDTO.MasterReportResFile;
            string rdlPath = string.Empty;
            string RDLCBaseFilePath = CommonUtility.RDLCBaseFilePath;
            if (objReportBuilderDTO.ParentID > 0)
            {
                if (objReportBuilderDTO.ISEnterpriseReport.GetValueOrDefault(false))
                {
                    rdlPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/EnterpriseReport" + @"\\" + ReportName;
                }
                else
                {
                    rdlPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID + @"\\" + ReportName;
                }
            }
            else
            {
                rdlPath = RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/BaseReport" + @"\\" + ReportName;
            }

            XDocument doc = XDocument.Load(rdlPath);
            XNamespace ns = XNamespace.Get("http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition");
            XNamespace nsrd = XNamespace.Get("http://schemas.microsoft.com/SQLServer/reporting/reportdesigner");

            string strFields = string.Empty;
            string DatasetFields = string.Empty;

            List<XElement> lstTablixRowGrouping = doc.Descendants(ns + "Tablix").Descendants(ns + "TablixRowHierarchy").Descendants(ns + "GroupExpression").ToList();
            List<XElement> lstColumns = doc.Descendants(ns + "TablixColumn").ToList();
            List<XElement> lstReportField = doc.Descendants(ns + "TablixRow").ToList();
            List<SelectListItem> lstGrouplist = new List<SelectListItem>();
            List<SelectListItem> lstDisabledGrouplist = new List<SelectListItem>();

            if (lstReportField != null && lstReportField.Count > 0)
            {
                List<XElement> lstFieldBottom = lstReportField[1].Descendants(ns + "TablixCell").ToList();
                IEnumerable<string> lstReportColmns = lstFieldBottom.Descendants(ns + "Value").ToList().Select(x => x.Value.Replace("=Fields!", "").Replace("=Parameters!BarcodeURL.Value+Fields!", "").Replace(".Value", ""));

                string tablixcellBottom = string.Empty;
                int rowGroupCount = lstTablixRowGrouping.Count;

                //for (int i = 0; i <= rowGroupCount - 1; i++)
                //{
                //    SelectListItem objKeyValDTO = new SelectListItem();
                //    tablixcellBottom = Convert.ToString(lstTablixRowGrouping[i].Value).Replace("=Fields!", "").Replace("=Parameters!BarcodeURL.Value+Fields!", "").Replace(".Value", "");
                //    objKeyValDTO.Value = tablixcellBottom;
                //    objKeyValDTO.Text = GetField(tablixcellBottom, ReportResFile);
                //    objKeyValDTO.Selected = true;
                //    lstGrouplist.Add(objKeyValDTO);

                //}

                foreach (var item in lstReportGroupMasterDTO)
                {
                    if (lstReportColmns.Contains(item.FieldName))
                    {
                        SelectListItem objKeyValDTO = new SelectListItem();
                        objKeyValDTO.Value = item.FieldName;
                        objKeyValDTO.Text = GetField(item.FieldName, ReportResFile);
                        objKeyValDTO.Selected = false;
                        lstGrouplist.Add(objKeyValDTO);
                    }
                    else
                    {
                        SelectListItem objKeyValDTO = new SelectListItem();
                        objKeyValDTO.Value = item.FieldName;
                        objKeyValDTO.Text = GetField(item.FieldName, ReportResFile);
                        objKeyValDTO.Selected = false;
                        if (lstGrouplist.FindIndex(x => x.Text == objKeyValDTO.Text && x.Value == objKeyValDTO.Value) < 0)
                            lstDisabledGrouplist.Add(objKeyValDTO);
                    }
                }
            }

            //ViewBag.DisabledGroupList = lstDisabledGrouplist;
            //return PartialView("_RangePerameters", lstGrouplist);
            return Json(new { Status = true, Message = "ok", Grouplist = lstGrouplist, DisableGroupList = lstDisabledGrouplist }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult GenerateReportOfCurrentReport(long? id, string FileType = "")
        {
            Int64 ReportId = id ?? 0;
            ReportHelper objReportHelper = new ReportHelper();
            if (SessionHelper.Get("ReportPara") == null)
            {
                return Json(new { result = "sessionexpired", url = Url.Action("ViewReports", "ReportBuilder") });
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Id"]))
            {
                ReportId = Convert.ToInt64(Request.QueryString["Id"]);
            }
            string Filename = objReportHelper.LoadReport(ReportId, FileType);

            return Json(Filename);
        }


        [HttpPost]
        public JsonResult ReportExecutionDataByType(string ModuleName, string Ids, string ReportType, string TechnicianGuids)
        {
            string reportURL = "/Reports/NewReportViewer.aspx?ID=";
            List<KeyValDTO> listkeyval = new List<KeyValDTO>();
            string reportID = "";
            bool? IsNoHeader = false;
            bool? ShowSignature = false;
            string sortingOn = string.Empty;
            bool isAppliedSortingByModuleName = false;
            try
            {
                KeyValDTO objKeyValDTO = new KeyValDTO();
                objKeyValDTO.key = "DataGuids";
                objKeyValDTO.value = Ids;
                if (ModuleName == "Order")
                {
                    reportID = GetDefaultReportIDByModuleName("Order", out IsNoHeader, out ShowSignature);
                    isAppliedSortingByModuleName = true;
                    objKeyValDTO.key = "IDs";
                    objKeyValDTO.value = Ids;
                }
                else if (ModuleName == "ReturnOrder")
                {
                    reportID = GetDefaultReportIDByModuleName("Return Order", out IsNoHeader, out ShowSignature);
                    isAppliedSortingByModuleName = true;
                    objKeyValDTO.key = "IDs";
                    objKeyValDTO.value = Ids;
                }
                else if (ModuleName == "WorkOrder" || ModuleName == "Work Order")
                {
                    reportID = GetDefaultReportIDByModuleName("Work Order", out IsNoHeader, out ShowSignature);
                    isAppliedSortingByModuleName = true;
                }
                else if (ModuleName == "OrderMasterList")
                {
                    reportID = GetDefaultReportIDByModuleName("Order", out IsNoHeader, out ShowSignature);
                    isAppliedSortingByModuleName = true;
                    objKeyValDTO.key = "IDs";
                    objKeyValDTO.value = Ids;
                }
                else if (ModuleName == "KitMasterList" || ModuleName == "Kit")
                {
                    reportID = GetDefaultReportIDByModuleName("Kit", out IsNoHeader, out ShowSignature);
                    isAppliedSortingByModuleName = true;
                    objKeyValDTO.key = "DataGuids";
                    objKeyValDTO.value = Ids;
                }
                else if (ModuleName == "TransferMasterList" || ModuleName == "Transfer")
                {
                    reportID = GetDefaultReportIDByModuleName("Transfer", out IsNoHeader, out ShowSignature);
                    isAppliedSortingByModuleName = true;
                    objKeyValDTO.key = "DataGuids";
                    objKeyValDTO.value = Ids;
                }
                else if (ModuleName == "InventoryCount" || ModuleName == "Inventory Count")
                {
                    reportID += GetDefaultReportIDByModuleName("Inventory Count", out IsNoHeader, out ShowSignature);
                    isAppliedSortingByModuleName = true;
                    objKeyValDTO.key = "DataGuids";
                    objKeyValDTO.value = Ids;
                }
                else if (ModuleName == "Items")
                {
                    reportID += GetDefaultReportIDByModuleName("Items", out IsNoHeader, out ShowSignature);
                    isAppliedSortingByModuleName = true;
                    objKeyValDTO.key = "DataGuids";
                    objKeyValDTO.value = Ids;
                }
                else if (ModuleName == "Pull")
                {
                    reportID += GetDefaultReportIDByModuleName("Pull", out IsNoHeader, out ShowSignature);
                    isAppliedSortingByModuleName = true;
                    objKeyValDTO.key = "DataGuids";
                    objKeyValDTO.value = Ids;
                }
                else if (ModuleName == "Requisition")
                {
                    reportID += GetDefaultReportIDByModuleName("Requisition", out IsNoHeader, out ShowSignature);
                    isAppliedSortingByModuleName = true;
                    objKeyValDTO.key = "DataGuids";
                    objKeyValDTO.value = Ids;
                }
                else if (ModuleName == "Project Spend")
                {
                    reportID += GetDefaultReportIDByModuleName("Project Spend", out IsNoHeader, out ShowSignature);
                    isAppliedSortingByModuleName = true;
                    objKeyValDTO.key = "DataGuids";
                    objKeyValDTO.value = Ids;
                }
                else if (ModuleName == "Suggested Orders")
                {
                    reportID += GetDefaultReportIDByModuleName("Suggested Orders", out IsNoHeader, out ShowSignature);
                    isAppliedSortingByModuleName = true;
                    objKeyValDTO.key = "DataGuids";
                    objKeyValDTO.value = Ids;
                }
                else if (ModuleName == "Company")
                {
                    reportID += GetDefaultReportIDByModuleName("Company", out IsNoHeader, out ShowSignature);
                    isAppliedSortingByModuleName = true;
                    objKeyValDTO.key = "DataGuids";
                    objKeyValDTO.value = Ids;
                }
                else if (ModuleName == "Room")
                {
                    reportID += GetDefaultReportIDByModuleName("Room", out IsNoHeader, out ShowSignature);
                    isAppliedSortingByModuleName = true;
                    objKeyValDTO.key = "DataGuids";
                    objKeyValDTO.value = Ids;
                }
                else if (ModuleName == "Enterprises List")
                {
                    reportID += GetDefaultReportIDByModuleName("Enterprises List", out IsNoHeader, out ShowSignature);
                    isAppliedSortingByModuleName = true;
                    objKeyValDTO.key = "DataGuids";
                    objKeyValDTO.value = Ids;
                }
                else if (ModuleName == "Users")
                {
                    reportID += GetDefaultReportIDByModuleName("Users", out IsNoHeader, out ShowSignature);
                    isAppliedSortingByModuleName = true;
                    objKeyValDTO.key = "DataGuids";
                    objKeyValDTO.value = Ids;
                }
                else if (ModuleName == "Maintenance Due"
                        || ModuleName == "MaintenanceDue")
                {
                    reportID += GetDefaultReportIDByModuleName("Maintenance Due", out IsNoHeader, out ShowSignature);
                    objKeyValDTO.key = "DataGuids";
                    objKeyValDTO.value = Ids;
                    isAppliedSortingByModuleName = true;
                }
                else if (ModuleName == "Asset Maintenance")
                {
                    reportID += GetDefaultReportIDByModuleName("Asset Maintenance", out IsNoHeader, out ShowSignature);
                    objKeyValDTO.key = "DataGuids";
                    objKeyValDTO.value = Ids;
                    isAppliedSortingByModuleName = true;
                }
                else if (ModuleName == "QuoteMasterList")
                {
                    reportID += GetDefaultReportIDByModuleName("Quote", out IsNoHeader, out ShowSignature);
                    objKeyValDTO.key = "DataGuids";
                    objKeyValDTO.value = Ids;
                    isAppliedSortingByModuleName = true;
                }
                else
                {
                    reportID = GetReportID(ModuleName, out IsNoHeader, out ShowSignature);

                    if (ModuleName.ToLower() == "Tools Checked Out".ToLower() && !string.IsNullOrEmpty(TechnicianGuids) && !string.IsNullOrWhiteSpace(TechnicianGuids))
                    {
                        KeyValDTO TechnicianGuidsParam = new KeyValDTO();
                        TechnicianGuidsParam.key = "TechnicianGuids";
                        TechnicianGuidsParam.value = TechnicianGuids;
                        listkeyval.Add(TechnicianGuidsParam);
                    }
                }

                reportURL += reportID;
                listkeyval.Add(objKeyValDTO);
                SetReportParaDictionary(listkeyval);

                if (!string.IsNullOrWhiteSpace(sortingOn))
                {
                    Dictionary<string, string> rptPara = (Dictionary<string, string>)SessionHelper.Get("ReportPara");
                    if (rptPara != null)
                        rptPara["SortFields"] = sortingOn;
                    else
                    {
                        rptPara = new Dictionary<string, string>();
                        rptPara["SortFields"] = sortingOn;
                    }

                    SessionHelper.Add("ReportPara", rptPara);
                    objKeyValDTO = new KeyValDTO();
                    objKeyValDTO.key = "SortFields";
                    objKeyValDTO.value = sortingOn.Replace("UpdatedByName", "RoomUpdateByName").Replace("CreatedByName", "RoomCreatedByName");
                    listkeyval.Add(objKeyValDTO);
                }
                else
                {
                    string SortingOn = string.Empty;
                    if (isAppliedSortingByModuleName)
                    {
                        string ModName = ModuleName;
                        if (ModuleName == "Company")
                            ModName = "AdminCompany";
                        else if (ModuleName == "Enterprises List")
                            ModName = "AdminEnterprise";
                        else if (ModuleName == "Inventory Count")
                            ModName = "InventoryCount";
                        else if (ModuleName == "Item" || ModuleName == "Item List" || ModuleName == "ItemListingGroup" || ModuleName == "Items")
                            ModName = "InventoryItem";
                        else if (ModuleName == "Kit")
                            ModName = "InventoryKit";
                        else if (ModuleName == "Order" || ModuleName == "Order Grouping" || ModuleName == "Order List" || ModuleName == "OrderMasterList")
                            ModName = "ReplenishOrder";
                        else if (ModuleName == "Project Spend")
                            ModName = "ConsumeProjectSpend";
                        else if (ModuleName == "Pull")
                            ModName = "ConsumePull";
                        else if (ModuleName == "Pull Completed" || ModuleName == "Pull Incomplete" || ModuleName == "Pull No Header" || ModuleName == "Pull Summary" || ModuleName == "Pull Summary By ConsignedPO" || ModuleName == "Pull Summary by Quarter")
                            ModName = "Consume_Pull";
                        else if (ModuleName == "Requisition")
                            ModName = "ConsumeRequisition";
                        else if (ModuleName == "Return Order")
                            ModName = "ReplenishReturnOrder";
                        else if (ModuleName == "Room")
                            ModName = "AdminRoom";
                        else if (ModuleName == "Suggested Orders")
                            ModName = "ReplenishCart";
                        else if (ModuleName == "Tools")
                            ModName = "Tool";
                        else if (ModuleName == "Transfer")
                            ModName = "ReplenishTransfer";
                        else if (ModuleName == "Users")
                            ModName = "AdminUser";
                        else if (ModuleName == "Work Order")
                            ModName = "ConsumeWorkOrder";
                        else if (ModuleName == "Maintenance Due")
                            ModName = "MaintenanceToolAsset";
                        else if (ModuleName == "Asset Maintenance")
                            ModName = "Assetmaster";
                        SortingOn = GetReportSortFields(ModName);
                    }
                    else
                    {
                        ReportMaster objReportMaster = GetReportMasterFromModuleName(ModuleName);
                        if (objReportMaster != null && !string.IsNullOrEmpty(objReportMaster.SortColumns))
                        {
                            SortingOn = objReportMaster.SortColumns;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(SortingOn))
                        SortingOn = SortingOn.TrimEnd(',');
                    Dictionary<string, string> rptPara = (Dictionary<string, string>)SessionHelper.Get("ReportPara");
                    if (rptPara != null)
                        rptPara["SortFields"] = SortingOn;
                    else
                    {
                        rptPara = new Dictionary<string, string>();
                        rptPara["SortFields"] = SortingOn;
                    }

                    SessionHelper.Add("ReportPara", rptPara);
                    if (!string.IsNullOrWhiteSpace(SortingOn))
                    {
                        objKeyValDTO = new KeyValDTO();
                        objKeyValDTO.key = "SortFields";
                        objKeyValDTO.value = SortingOn.TrimEnd(',');
                        listkeyval.Add(objKeyValDTO);
                    }
                }

                if (ModuleName != "Room" && ModuleName != "Company")
                {
                    Dictionary<string, string> rptParaSession = (Dictionary<string, string>)SessionHelper.Get("ReportPara");
                    if (rptParaSession != null)
                    {
                        rptParaSession["@Roomids"] = Convert.ToString(SessionHelper.RoomID);
                        rptParaSession["@CompanyIDs"] = Convert.ToString(SessionHelper.CompanyID);
                    }
                    else
                    {
                        rptParaSession = new Dictionary<string, string>();
                        rptParaSession["@Roomids"] = Convert.ToString(SessionHelper.RoomID);
                        rptParaSession["@CompanyIDs"] = Convert.ToString(SessionHelper.CompanyID);
                    }
                    if ((IsNoHeader ?? false))
                    {
                        rptParaSession["IsNoHeader"] = Convert.ToString("1");
                    }

                    if ((ShowSignature ?? false))
                    {
                        rptParaSession["ShowSignature"] = Convert.ToString("1");
                    }

                    SessionHelper.Add("ReportPara", rptParaSession);
                    objKeyValDTO = new KeyValDTO();
                    objKeyValDTO.key = "Roomids";
                    objKeyValDTO.value = Convert.ToString(SessionHelper.RoomID);
                    listkeyval.Add(objKeyValDTO);
                    objKeyValDTO = new KeyValDTO();
                    objKeyValDTO.key = "CompanyIDs";
                    objKeyValDTO.value = Convert.ToString(SessionHelper.CompanyID);
                    listkeyval.Add(objKeyValDTO);
                }
                string fileNameToReturn = GetReportBytes(reportID, ReportType);
                if (fileNameToReturn.Trim().ToLower() != "no records")
                {
                    string FileURL = "/RDLC_Reports/Temp/" + fileNameToReturn;
                    return Json(new { Status = true, Message = "ok", ReportFileURL = FileURL }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (ReportType.Trim().ToLower() == "pdf")
                        return Json(new { Status = false, Message = ResReportMaster.NoRecordsFoundToGeneratePDF, ReportFileName = "" }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { Status = false, Message = ResReportMaster.NoRecordsFoundToGenerateFile, ReportFileName = "" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message, ReportFileName = "" }, JsonRequestBehavior.AllowGet);

            }

        }
        [HttpPost]
        public JsonResult SendReportExecutionData(string FilePath, string MailBody, string MailCC, string MailSubject, string MailTo, string Excelfilepath)
        {
            ArrayList _Attachments = null;
            Attachment atr = null;
            eTurnsUtility objUtils = null;
            List<eMailAttachmentDTO> objeMailAttchList = null;
            eMailAttachmentDTO objeMailAttch = null;
            eMailMasterDAL objEmailDAL = null;
            byte[] byt = null;
            try
            {
                string StrSubject = MailSubject;
                string strToAddress = MailTo;
                string strCCAddress = MailCC;
                string MessageBody = MailBody;
                objeMailAttchList = new List<eMailAttachmentDTO>();
                objeMailAttch = new eMailAttachmentDTO();
                _Attachments = new ArrayList();

                if (!string.IsNullOrEmpty(FilePath))
                {
                    string file = Server.MapPath(FilePath);
                    atr = new Attachment(file);
                    //_Attachments.Add(atr);
                    byt = new byte[atr.ContentStream.Length];
                    atr.ContentStream.Read(byt, 0, byt.Length - 1);
                    objeMailAttch.FileData = byt;
                    objeMailAttch.eMailToSendID = 0;
                    objeMailAttch.MimeType = atr.ContentType.MediaType;// "application/pdf";
                    objeMailAttch.AttachedFileName = atr.ContentType.Name;
                    objeMailAttchList.Add(objeMailAttch);
                }

                if (!string.IsNullOrEmpty(Excelfilepath)) // This code block is for the excel attachment for the RunReport
                {
                    string attchfilepathforExcel = Server.MapPath(Excelfilepath);
                    objeMailAttch = new eMailAttachmentDTO();
                    atr = new Attachment(attchfilepathforExcel);
                    byt = new byte[atr.ContentStream.Length];
                    atr.ContentStream.Read(byt, 0, byt.Length - 1);
                    objeMailAttch.FileData = byt;
                    objeMailAttch.eMailToSendID = 0;
                    objeMailAttch.MimeType = atr.ContentType.MediaType;
                    objeMailAttch.AttachedFileName = atr.ContentType.Name;
                    objeMailAttchList.Add(objeMailAttch);
                }
                //CommonUtility.SendMail(FromAddress, strToAddress, strCCAddress, strBCCAddress, strNotificationAddress, StrSubject, MessageBody, true, _Attachments);
                objUtils = new eTurnsUtility();
                objUtils.SendMail(strToAddress, strCCAddress, StrSubject, MessageBody, _Attachments);
                objEmailDAL = new eMailMasterDAL(SessionHelper.EnterPriseDBName);
                RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
                eTurnsRegionInfo objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(SessionHelper.RoomID, SessionHelper.CompanyID, -1);
                string DateTimeFormat = "MM/dd/yyyy";
                DateTime TZDateTimeNow = DateTime.UtcNow;
                if (objeTurnsRegionInfo != null)
                {
                    DateTimeFormat = objeTurnsRegionInfo.ShortDatePattern;// + " " + objeTurnsRegionInfo.ShortTimePattern;
                    TZDateTimeNow = objeTurnsRegionInfo.TZDateTimeNow ?? DateTime.UtcNow;
                }
                if (StrSubject != null)
                {
                    string CurrentDate = TZDateTimeNow.ToString(DateTimeFormat);
                    // EmailSubject = EmailSubject.Replace("@@DATE@@", CurrentDate).Replace("@@date@@", CurrentDate).Replace("@@Date@@", CurrentDate);
                    StrSubject = Regex.Replace(StrSubject, "@@DATE@@", CurrentDate, RegexOptions.IgnoreCase);
                    if (!string.IsNullOrWhiteSpace(SessionHelper.CompanyName))
                    {
                        StrSubject = Regex.Replace(StrSubject, "@@COMPANYNAME@@", SessionHelper.CompanyName, RegexOptions.IgnoreCase);
                    }
                    if (!string.IsNullOrWhiteSpace(SessionHelper.RoomName))
                    {
                        StrSubject = Regex.Replace(StrSubject, "@@ROOMNAME@@", SessionHelper.RoomName, RegexOptions.IgnoreCase);
                    }
                    StrSubject = Regex.Replace(StrSubject, "@@Year@@", Convert.ToString(DateTime.UtcNow.Year), RegexOptions.IgnoreCase);
                }

                if (MessageBody != null)
                {
                    string CurrentDate = TZDateTimeNow.ToString(DateTimeFormat);
                    //EmailBody = EmailBody.Replace("@@DATE@@", CurrentDate).Replace("@@date@@", CurrentDate).Replace("@@Date@@", CurrentDate);
                    MessageBody = MessageBody.Replace("@@DATE@@", CurrentDate);
                    if (!string.IsNullOrWhiteSpace(SessionHelper.CompanyName))
                    {
                        MessageBody = MessageBody.Replace("@@COMPANYNAME@@", SessionHelper.CompanyName);
                    }
                    if (!string.IsNullOrWhiteSpace(SessionHelper.RoomName))
                    {
                        MessageBody = MessageBody.Replace("@@ROOMNAME@@", SessionHelper.RoomName);
                    }
                    MessageBody = MessageBody.Replace("@@Year@@", Convert.ToString(DateTime.UtcNow.Year));

                }
                objEmailDAL.eMailToSend(strToAddress, strCCAddress, StrSubject, MessageBody.ToString(), SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, objeMailAttchList, "Web => Reprotbuilder => SendReportMail");
                return Json(true);

            }
            catch (Exception)
            {
                return Json(false);
            }
            finally
            {
                if (_Attachments != null)
                    _Attachments.Clear();

                _Attachments = null;
                if (atr != null)
                    atr.Dispose();

                atr = null;
                objUtils = null;
                objeMailAttchList = null;
                objeMailAttch = null;
                objEmailDAL = null;
            }
        }

        private string GetReportBytes(string reportID, string reportType)
        {

            ParentID = 0;
            ReportBuilderDTO objDTO = new ReportBuilderDTO();
            ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            eTurns.DAL.AlertMail objAlertMail = new eTurns.DAL.AlertMail();
            objDTO = objDAL.GetReportDetail(Convert.ToInt64(reportID));
            string MasterReportResFile = objDTO.MasterReportResFile;
            SubReportResFile = MasterReportResFile;
            string Reportname = objDTO.ReportName;
            string MasterReportname = objDTO.ReportFileName;
            string SubReportname = objDTO.SubReportFileName;
            string mainGuid = "RPT_" + Guid.NewGuid().ToString().Replace("-", "_");
            string subGuid = "SubRPT_" + Guid.NewGuid().ToString().Replace("-", "_");
            string ReportPath = string.Empty;

            bool hasSubReport = false;
            string RDLCBaseFilePath = CommonUtility.RDLCBaseFilePath;
            ParentID = objDTO.ParentID ?? 0;
            if (objDTO.ParentID > 0)
            {
                if (objDTO.ISEnterpriseReport.GetValueOrDefault(false))
                    ReportPath = CopyFiletoTemp(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/EnterpriseReport" + @"\\" + MasterReportname, mainGuid);
                else
                    ReportPath = CopyFiletoTemp(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID + @"\\" + MasterReportname, mainGuid);
            }
            else
                ReportPath = CopyFiletoTemp(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/BaseReport" + @"\\" + MasterReportname, mainGuid);

            XDocument doc = XDocument.Load(ReportPath);
            if (objDTO != null
                && objDTO.IsBaseReport
                && !string.IsNullOrWhiteSpace(objDTO.ReportResourceName))
            {
                doc = UpdateReportTileFromResource(doc, objDTO.ReportResourceName);
            }
            IEnumerable<XElement> lstTablix = doc.Descendants(ns + "Tablix");
            string strTablix = string.Empty;
            if (lstTablix != null && lstTablix.ToList().Count > 0)
                strTablix = lstTablix.ToList()[0].ToString();

            IEnumerable<XElement> lstUpdateTablix = UpdateResource(lstTablix, MasterReportResFile);
            doc.Save(ReportPath);

            IEnumerable<XElement> lstReportPara = doc.Descendants(ns + "ReportParameter");
            List<ReportParameter> rpt = new List<ReportParameter>();
            rptPara = GetReportParaDictionary();

            if (objDTO.ModuleName.ToLower() == "enterpriselist")
            {
                string strEntLogo = rptPara["EnterpriseLogoURL"];
                string baseURL = System.Web.HttpContext.Current.Request.Url.ToString().Replace(System.Web.HttpContext.Current.Request.Url.PathAndQuery, "");

                strEntLogo = baseURL + "/Uploads/EnterpriseLogos/";
                rptPara["EnterpriseLogoURL"] = strEntLogo;
                string DBServerName = System.Configuration.ConfigurationManager.AppSettings["DBserverName"];
                string DBUserName = System.Configuration.ConfigurationManager.AppSettings["DbUserName"];
                string DBPassword = System.Configuration.ConfigurationManager.AppSettings["DbPassword"];
                string DBName = DbConnectionHelper.GetETurnsMasterDBName();
                string connStr = @"Data Source={0};Initial Catalog={1};User ID={2};Password={3}";
                connStr = string.Format(connStr, DBServerName, DBName, DBUserName, DBPassword);
                connStr = DbConnectionHelper.GeteTurnsMasterSQLConnectionString(DbConnectionHelper.GetETurnsMasterDBName(), DbConnectionType.GeneralReadOnly.ToString("F"));
                rptPara["ConnectionString"] = connStr;
                connectionString = connStr;
            }
            if (objDTO.ModuleName.ToLower() == "userslist"
                || objDTO.ModuleName.ToLower() == "user_list")
            {

                string DBServerName = System.Configuration.ConfigurationManager.AppSettings["DBserverName"];
                string DBUserName = System.Configuration.ConfigurationManager.AppSettings["DbUserName"];
                string DBPassword = System.Configuration.ConfigurationManager.AppSettings["DbPassword"];
                string DBName = DbConnectionHelper.GetETurnsMasterDBName();
                string connStr = @"Data Source={0};Initial Catalog={1};User ID={2};Password={3}";
                connStr = string.Format(connStr, DBServerName, DBName, DBUserName, DBPassword);
                connStr = DbConnectionHelper.GeteTurnsMasterSQLConnectionString(DbConnectionHelper.GetETurnsMasterDBName(), DbConnectionType.GeneralReadOnly.ToString("F"));
                rptPara["ConnectionString"] = connStr;
                connectionString = connStr;

            }
            //TODO: Start WI-1627: Setting the sort fields does not work
            if (doc.Descendants(ns + "Subreport") != null && doc.Descendants(ns + "Subreport").Count() > 0)
            {
                hasSubReport = true;
            }
            if (objDTO.ReportType == 3 && !hasSubReport && rptPara.ContainsKey("SortFields") && !string.IsNullOrEmpty(rptPara["SortFields"]))
            {
                string SortFields = rptPara["SortFields"];

                string[] arrSortFields = SortFields.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (arrSortFields != null && arrSortFields.Length > 0)
                {
                    string firstSortFields = arrSortFields[0].Replace(" asc", "").Replace(" desc", "").Replace(" ASC", "").Replace(" DESC", "");
                    XElement xRowHira = doc.Descendants(ns + "TablixRowHierarchy").FirstOrDefault();
                    XElement xGroup = xRowHira.Descendants(ns + "Group").FirstOrDefault();
                    XElement xGroupExpression = xGroup.Descendants(ns + "GroupExpression").FirstOrDefault();
                    if (xGroupExpression != null)
                        xGroupExpression.Value = "=Fields!" + firstSortFields + ".Value";
                }
            }

            //TODO: End WI-1627: Setting the sort fields does not work

            doc.Save(ReportPath);
            if (doc.Descendants(ns + "Subreport") != null && doc.Descendants(ns + "Subreport").Count() > 0)
            {
                hasSubReport = true;
            }
            if (rptPara.Keys.Contains("IsNoHeader"))
            {
                //if (!hasSubReport && !objDTO.IsNotEditable.GetValueOrDefault(false)
                //                 && (objDTO.ReportType == 3 || objDTO.ReportType == 1)
                //                 && LocalDictRptPara["IsNoHeader"] == "1")
                if (!objDTO.IsNotEditable.GetValueOrDefault(false) && rptPara["IsNoHeader"] == "1"
                     && (objDTO.ReportType == 1 || objDTO.ReportType == 2 || objDTO.ReportType == 3))
                {
                    doc = objAlertMail.GetAdditionalHeaderRow(doc, objDTO, SessionHelper.CompanyName, SessionHelper.RoomName, EnterpriseDBName: SessionHelper.EnterPriseDBName);
                    doc.Save(ReportPath);
                    doc = XDocument.Load(ReportPath);

                }
            }
            else
            {
                if (!objDTO.IsNotEditable.GetValueOrDefault(false)
                     && (objDTO.ReportType == 1 || objDTO.ReportType == 2 || objDTO.ReportType == 3))
                {
                    XElement xRows = doc.Descendants(ns + "TablixRows").FirstOrDefault();
                    XElement xRow = doc.Descendants(ns + "TablixRow").FirstOrDefault();

                    if (objDTO.ReportType == 2 && xRow.Descendants(ns + "TablixCell").Descendants(ns + "Textbox").
                            FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "TextboxRoom") != null)
                    {
                        doc.Descendants(ns + "TablixRow").FirstOrDefault().Remove();
                        doc.Descendants(ns + "TablixRowHierarchy").Descendants(ns + "TablixMembers").Descendants(ns + "TablixMember").Descendants(ns + "TablixMembers").Descendants(ns + "TablixMember").Remove();
                        foreach (var item in xRows.Descendants(ns + "TablixRow"))
                        {
                            doc.Descendants(ns + "TablixRowHierarchy").Descendants(ns + "TablixMembers").Descendants(ns + "TablixMember").Descendants(ns + "TablixMembers").FirstOrDefault().Add(new XElement(ns + "TablixMember"));
                        }
                        doc.Save(ReportPath);
                        doc = XDocument.Load(ReportPath);
                    }
                }
            }

            if (rptPara.Keys.Contains("ShowSignature"))
            {
                doc = objAlertMail.GetFooterForSignature(doc, objDTO);
                doc.Save(ReportPath);
                doc = XDocument.Load(ReportPath);
            }

            doc = amDAL.AddFormatToTaxbox(doc, rsInfo);
            doc.Save(ReportPath);
            doc = XDocument.Load(ReportPath);

            if (lstReportPara != null && lstReportPara.Count() > 0)
            {
                foreach (var item in lstReportPara)
                {
                    ReportParameter rpara = new ReportParameter();
                    rpara.Name = item.Attribute("Name").Value;
                    if (!string.IsNullOrEmpty(rptPara.FirstOrDefault(x => x.Key.Replace("@", "").ToLower() == item.Attribute("Name").Value.Replace("@", "").ToLower()).Value))
                        rpara.Values.Add(rptPara.FirstOrDefault(x => x.Key.Replace("@", "").ToLower() == item.Attribute("Name").Value.Replace("@", "").ToLower()).Value);

                    rpt.Add(rpara);
                }
            }

            string connString = doc.Descendants(ns + "ConnectString").FirstOrDefault().Value;
            SqlConnection myConnection = new SqlConnection();
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter sqla = new SqlDataAdapter();
            DataTable dt = new DataTable();
            cmd.CommandTimeout = 7200;
            myConnection.ConnectionString = connectionString;

            cmd.Connection = myConnection;
            cmd.CommandText = doc.Descendants(ns + "CommandText").FirstOrDefault().Value; //"SELECT *  FROM   ItemMaster_View";
            cmd.CommandType = CommandType.Text;
            if (doc.Descendants(ns + "CommandType").FirstOrDefault() != null)
                cmd.CommandType = (CommandType)Enum.Parse(typeof(CommandType), doc.Descendants(ns + "CommandType").FirstOrDefault().Value == null ? "Text" : doc.Descendants(ns + "CommandType").FirstOrDefault().Value, true);

            IEnumerable<XElement> lstQueryPara = doc.Descendants(ns + "QueryParameter");

            if (lstQueryPara != null && lstQueryPara.Count() > 0)
            {
                foreach (var item in lstQueryPara)
                {
                    SqlParameter slpar = new SqlParameter();
                    slpar.ParameterName = item.Attribute("Name").Value;//
                    //if (!string.IsNullOrEmpty(rptPara.FirstOrDefault(x => x.Key.Replace("@", "").ToLower() == item.Attribute("Name").Value.Replace("@", "").ToLower()).Value))
                    if (!(hasSubReport && slpar.ParameterName.ToLower().Replace("@", "") == "sortfields") && !string.IsNullOrEmpty(rptPara.FirstOrDefault(x => x.Key.ToLower().Replace("@", "") == item.Attribute("Name").Value.Replace("@", "").ToLower()).Value))
                        slpar.Value = rptPara.FirstOrDefault(x => x.Key.Replace("@", "").ToLower() == item.Attribute("Name").Value.Replace("@", "").ToLower()).Value;
                    else
                        slpar.Value = DBNull.Value;
                    XElement objReportPara = lstReportPara.FirstOrDefault(x => x.Attribute("Name").Value.ToLower() == slpar.ParameterName.Replace("@", "").ToLower());

                    if (objReportPara.Descendants(ns + "DataType") != null && objReportPara.Descendants(ns + "DataType").Count() > 0)
                        slpar.DbType = (DbType)Enum.Parse(typeof(DbType), objReportPara.Descendants(ns + "DataType").FirstOrDefault().Value, true);

                    if (objDTO.ModuleName.ToLower().Equals("checkouttool") && slpar.ParameterName.ToLower().Replace("@", "").Equals("fromlistpage"))
                    {
                        slpar.Value = true;
                    }

                    cmd.Parameters.Add(slpar);
                }
            }
            sqla = new SqlDataAdapter(cmd);
            sqla.Fill(dt);

            ReportViewer ReportViewer1 = new ReportViewer();
            ReportViewer1.Reset();
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.Visible = true;

            if (doc.Descendants(ns + "Subreport") != null && doc.Descendants(ns + "Subreport").Count() > 0)
            {
                hasSubReport = true;
                if (objDTO.ParentID > 0)
                {
                    if (objDTO.ISEnterpriseReport.GetValueOrDefault(false))
                        rdlPath = CopyFiletoTemp(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/EnterpriseReport" + @"\\" + SubReportname, subGuid);
                    else
                        rdlPath = CopyFiletoTemp(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID + @"\\" + SubReportname, subGuid);
                }
                else
                    rdlPath = CopyFiletoTemp(RDLCBaseFilePath + "/" + SessionHelper.EnterPriceID.ToString() + "/BaseReport" + @"\\" + SubReportname, subGuid);

                doc.Descendants(ns + "Tablix").Descendants(ns + "Subreport").FirstOrDefault().Attribute("Name").Value = Convert.ToString(subGuid);
                doc.Descendants(ns + "Tablix").Descendants(ns + "ReportName").FirstOrDefault().Value = Convert.ToString(subGuid);
                doc.Save(ReportPath);

                XDocument docSub = XDocument.Load(rdlPath);
                IEnumerable<XElement> lstSubTablix = docSub.Descendants(ns + "Tablix");
                IEnumerable<XElement> lstUpdateSubTablix = UpdateResource(lstSubTablix, SubReportResFile);
                docSub.Save(rdlPath);

                if (lstSubTablix != null && lstSubTablix.ToList().Count > 0)
                {
                    strSubTablix = lstSubTablix.ToList()[0].ToString();
                }

                docSub = amDAL.AddFormatToTaxbox(docSub, rsInfo);
                docSub.Save(rdlPath);
                docSub = XDocument.Load(rdlPath);

                docSub.Save(rdlPath);

                ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(LoadSubreport);

                ReportViewer1.LocalReport.EnableExternalImages = true;
                ReportViewer1.LocalReport.EnableHyperlinks = true;
                ReportViewer1.LocalReport.Refresh();
            }

            if (!hasSubReport && rptPara.ContainsKey("SortFields") && !string.IsNullOrEmpty(rptPara["SortFields"]))
            {
                string SortFields = rptPara["SortFields"];

                if (!string.IsNullOrEmpty(SortFields))
                {
                    dt.DefaultView.Sort = SortFields;
                    dt = dt.DefaultView.ToTable();
                }
            }

            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.ReportPath = ReportPath;
            ReportDataSource rds = new ReportDataSource();
            rds.Name = doc.Descendants(ns + "DataSet").FirstOrDefault().FirstAttribute.Value;
            rds.Value = dt;
            if (dt.Rows.Count == 0)
            {
                return "No Records";
            }
            ReportViewer1.LocalReport.DataSources.Add(rds);
            ReportViewer1.LocalReport.SetParameters(rpt);
            ReportViewer1.ZoomMode = ZoomMode.Percent;
            ReportViewer1.LocalReport.Refresh();


            Warning[] warnings;
            string[] streamIds;
            string mimeType = "application/vnd.ms-excel";
            string encoding = "utf-8";
            string extension = "xls";
            if (reportType == "PDF")
            {
                mimeType = "application/pdf";
                encoding = "utf-8";
                extension = "pdf";
            }
            string tmpReportName = Regex.Replace(objDTO.ReportName, @"[^0-9a-zA-Z]+", "_");
            string strOnlyFileName = tmpReportName + "_" + DateTimeUtility.DateTimeNow.ToString("yyyy-MM-dd_HH_mm_ss") + "." + extension;
            byte[] bytes = ReportViewer1.LocalReport.Render(reportType, null, out mimeType, out encoding, out extension, out streamIds, out warnings);

            string reportFileName = RDLCBaseFilePath + "/Temp/" + strOnlyFileName;
            using (FileStream fs = new FileStream(reportFileName, FileMode.Create))
            {
                fs.Write(bytes, 0, bytes.Length);
            }
            try
            {
                if (System.IO.File.Exists(reportFileName))
                {
                    string DestinationTempFilePath = reportFileName.Replace(@"\\amznfsxyfdhnm2f.eturns.local\share\WebSites\eTurns4040\", eTurnsAppConfig.BaseFilePath);
                    System.IO.File.Copy(reportFileName, DestinationTempFilePath, true);
                }
            }
            catch (Exception ex)
            {

            }
            return strOnlyFileName;
        }

        public Int64 GetBaseParentByReportID(long ParentReportID)
        {
            Int64 ParentID = 0;

            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            ReportBuilderDTO objReportBuilderDTO = objReportMasterDAL.GetReportDetail(ParentReportID);
            if (objReportBuilderDTO != null)
            {
                if (!objReportBuilderDTO.IsBaseReport && objReportBuilderDTO.ParentID > 0)
                    ParentID = GetBaseParentByReportID(objReportBuilderDTO.ParentID.GetValueOrDefault(0));
                else
                    ParentID = objReportBuilderDTO.ID;
            }

            return ParentID;
        }

        public Int64 GetBaseParentByReportID(long ParentReportID, string EnterpriseDBName)
        {
            Int64 ParentID = 0;

            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(EnterpriseDBName);
            ReportBuilderDTO objReportBuilderDTO = objReportMasterDAL.GetReportDetail(ParentReportID);
            if (objReportBuilderDTO != null)
            {
                if (!objReportBuilderDTO.IsBaseReport && objReportBuilderDTO.ParentID > 0)
                    ParentID = GetBaseParentByReportID(objReportBuilderDTO.ParentID.GetValueOrDefault(0));
                else
                    ParentID = objReportBuilderDTO.ID;
            }

            return ParentID;
        }


        public Int64 GetBaseParentByReportIDFromList(long ParentReportID, List<ReportBuilderDTO> reportList)
        {
            Int64 ParentID = 0;

            ReportBuilderDTO objReportBuilderDTO = reportList.Where(x => x.ID == ParentReportID).FirstOrDefault();
            if (objReportBuilderDTO != null)
            {
                if (!objReportBuilderDTO.IsBaseReport && objReportBuilderDTO.ParentID > 0)
                    ParentID = GetBaseParentByReportIDFromList(objReportBuilderDTO.ParentID.GetValueOrDefault(0), reportList);
                else
                    ParentID = objReportBuilderDTO.ID;
            }

            return ParentID;
        }

        private void GetChildReportByParentReportID(ReportBuilderDTO objParentReport, IEnumerable<ReportBuilderDTO> lstReports, List<ReportBuilderDTO> ChildReports)
        {
            List<ReportBuilderDTO> lstChildReports = lstReports.Where(x => x.ParentID == objParentReport.ID).ToList();
            ChildReports.AddRange(lstChildReports);
            foreach (var item in lstChildReports)
            {
                GetChildReportByParentReportID(item, lstReports, ChildReports);
            }
        }

        [HttpPost]
        public string GetModuleNameByReportId(Int64 TemplateId, Int64 NotificationID)
        {
            try
            {
                Int64 ReportMasterID = 0;
                if (NotificationID > 0)
                {
                    NotificationDTO oNotificationDTO = new NotificationDAL(SessionHelper.EnterPriseDBName).GetNotifiactionByID(NotificationID);
                    if (oNotificationDTO != null && oNotificationDTO.ReportID.HasValue)
                        ReportMasterID = oNotificationDTO.ReportID.Value;
                    else
                        ReportMasterID = TemplateId;
                }
                else
                {
                    ReportMasterID = TemplateId;
                }
                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                ReportBuilderDTO objReportBuilderDTO = objReportMasterDAL.GetReportList().Where(x => x.ID == ReportMasterID).FirstOrDefault();
                if (objReportBuilderDTO != null)
                {
                    return objReportBuilderDTO.ModuleName;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        [HttpPost]
        public ActionResult ImportNewReportFile(FormCollection collection, HttpPostedFileBase fuResource)
        {
            string PostedFileName = string.Empty;
            try
            {
                if (SessionHelper.RoleID == -1)
                {
                    if (fuResource != null)
                    {
                        string BaseReportPath = CommonUtility.RDLCBaseFilePath;

                        string BaseMasterReportPath = ResourceHelper.RDLReportDirectoryBasePath + "\\MasterReport";
                        PostedFileName = Path.GetFileName(fuResource.FileName);
                        string reportPath = Path.Combine(BaseMasterReportPath, fuResource.FileName);
                        fuResource.SaveAs(reportPath);
                        string selectedEnterprise = Convert.ToString(collection["hdnEntList"]);
                        UpdateChildEntReportByReportFileName(fuResource.FileName, true, true, selectedEnterprise);

                    }
                }
                return RedirectToAction("ReportSetting", "ReportBuilder");
            }
            catch
            {
                throw;
            }
            finally
            {

            }
        }



        public JsonResult UpdateChildEntReportByReportFileName(string FileName, bool OverwriteExisting, bool OverwirteReportField, string SelectedEnterprise = "")
        {
            if (SessionHelper.UserType != 1)
            {
                return Json(new { Status = false, Message = ResReportMaster.YouAreNotAbleToUpdate }, JsonRequestBehavior.AllowGet);
            }

            string msg = string.Empty;
            EnterpriseMasterDAL objDAL = new EnterpriseMasterDAL();
            List<EnterpriseDTO> lstEnterprise = new List<EnterpriseDTO>();

            if (!string.IsNullOrWhiteSpace(SelectedEnterprise))
            {
                //List<long> entIDs = SelectedEnterprise.Split(',').Select(long.Parse).ToList();
                lstEnterprise = objDAL.GetEnterprisesByIds(SelectedEnterprise);
            }
            else
            {
                lstEnterprise = objDAL.GetAllEnterprisesPlain();
            }



            foreach (EnterpriseDTO item in lstEnterprise)
            {
                try
                {
                    objDAL.UpdateRDLCReportMasterByReportFile(DbConnectionHelper.GetETurnsMasterDBName(), item.EnterpriseDBName, SessionHelper.UserID, OverwriteExisting, FileName);
                    string BaseReportPath = string.Empty;
                    string EntReportPath = string.Empty;
                    BaseReportPath = ResourceHelper.RDLReportDirectoryBasePath + @"\MasterReport";
                    EntReportPath = ResourceHelper.RDLReportDirectoryBasePath + @"\" + item.ID + @"\" + @"\BaseReport";
                    if (System.IO.Directory.Exists(BaseReportPath))
                    {
                        if (!System.IO.Directory.Exists(EntReportPath))
                            System.IO.Directory.CreateDirectory(EntReportPath);

                        try
                        {
                            string Destfilepath = EntReportPath + @"\" + FileName;
                            if (!System.IO.File.Exists(Destfilepath))
                                System.IO.File.Copy(Path.Combine(BaseReportPath, FileName), Path.Combine(EntReportPath, FileName));
                            else if (OverwriteExisting)
                            {
                                System.IO.File.Delete(Path.Combine(EntReportPath, FileName));
                                System.IO.File.Copy(Path.Combine(BaseReportPath, FileName), Path.Combine(EntReportPath, FileName));
                            }
                        }
                        catch (Exception exInner)
                        {
                            msg += (item.EnterpriseDBName + " " + ResCommon.ErrorColon + " " + exInner.ToString());
                        }

                        if (OverwirteReportField)
                        {
                            msg += UpdateAllReportsFieldsByReportFileName(item, FileName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    msg += (item.EnterpriseDBName + " " + ResCommon.ErrorColon + " " + ex.ToString());
                }
            }

            msg += ResCommon.MsgUpdatedSuccessfully;
            return Json(new { Status = true, Message = msg }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Update All Child Report Fields
        /// </summary>
        /// <param name="objEnt"></param>
        private string UpdateAllReportsFieldsByReportFileName(EnterpriseDTO objEnt, string FileName)
        {
            string msg = string.Empty;
            string ReportBasePath = ResourceHelper.RDLReportDirectoryBasePath;
            string MasterReportBasePath = ResourceHelper.RDLReportDirectoryBasePath + @"\MasterReport";

            ReportMasterDAL objRptDAL = new ReportMasterDAL(objEnt.EnterpriseDBName);
            List<ReportBuilderDTO> objReportsData = objRptDAL.GetReportList();
            List<ReportBuilderDTO> lstChildReport = new List<ReportBuilderDTO>();
            ReportBuilderDTO parentReport = objReportsData.FirstOrDefault(x => (x.ReportFileName == FileName || x.SubReportFileName == x.ReportFileName) && x.ParentID == 0);
            ReportBuilderDTO rpt = null;

            GetChildReportByParentReportID(parentReport, objReportsData, lstChildReport);
            //IEnumerable<string> ChildReportFileName = lstChildReport.Select(x => x.ReportFileName);
            //IEnumerable<string> ChildSubReportFileName = lstChildReport.Select(x => x.SubReportFileName);
            //string[] ChildRptFiles = ChildReportFileName.Concat(ChildSubReportFileName).ToArray();


            IEnumerable<string> strTopSubDirs = Directory.GetDirectories(ReportBasePath);
            strTopSubDirs = strTopSubDirs.Where(x => x == ReportBasePath + "\\" + objEnt.ID);

            if (!(strTopSubDirs != null && strTopSubDirs.Count() > 0))
                return msg;

            foreach (var dirName in strTopSubDirs)
            {
                IEnumerable<string> strSecondSubDirs = Directory.GetDirectories(dirName).Where(x => x != dirName + "\\BaseReport");

                if (!(strSecondSubDirs != null && strSecondSubDirs.Count() > 0))
                    continue;

                foreach (string Levl2dirName in strSecondSubDirs)
                {
                    string[] reportFiles = Directory.GetFiles(Levl2dirName);
                    if (!(reportFiles != null && reportFiles.Length > 0))
                        continue;

                    foreach (string fileName in reportFiles)
                    {
                        try
                        {
                            string strFileName = fileName.Remove(0, fileName.LastIndexOf("\\") + 1);
                            rpt = lstChildReport.FirstOrDefault(x => x.ReportFileName == strFileName || x.SubReportFileName == strFileName);

                            if (rpt == null)
                                continue;

                            if (parentReport != null && !rpt.IsNotEditable.GetValueOrDefault(false))
                            {
                                if (System.IO.File.Exists(MasterReportBasePath + @"\" + parentReport.ReportFileName))
                                {
                                    //Console.WriteLine("Exist");
                                    XDocument docBase = XDocument.Load(MasterReportBasePath + @"\" + parentReport.ReportFileName);
                                    IEnumerable<XElement> lstDSParentFields = docBase.Descendants(ns + "DataSet").Descendants(ns + "Fields");
                                    XDocument docChild = XDocument.Load(fileName);
                                    IEnumerable<XElement> lstDSChildFields = docChild.Descendants(ns + "DataSet").Descendants(ns + "Fields");
                                    docChild.Descendants(ns + "DataSet").Descendants(ns + "Fields").FirstOrDefault().ReplaceWith(lstDSParentFields.ToList());
                                    docChild.Save(fileName);

                                    IEnumerable<XElement> lstParentReportPara = docBase.Descendants(ns + "ReportParameters");
                                    docChild = XDocument.Load(fileName);
                                    IEnumerable<XElement> lstChildReportPara = docChild.Descendants(ns + "ReportParameters");

                                    docChild.Descendants(ns + "ReportParameters").FirstOrDefault().ReplaceWith(lstParentReportPara.ToList());
                                    docChild.Save(fileName);

                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            msg += " " + string.Format(ResReportMaster.UpdateAllReportsFieldsError, objEnt.EnterpriseDBName, fileName, ex.ToString());
                        }
                    }

                }
            }
            return msg;
        }

        [HttpGet]
        public ActionResult ReportList()
        {
            var isViewReport = SessionHelper.GetAdminPermission(SessionHelper.ModuleList.ViewReport);

            if (isViewReport)
            {
                long TotalRecordCount = 0;
                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
                TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
                List<ReportMasterDTO> DataFromDB = objReportMasterDAL.GetPagedReports(0, int.MaxValue, out TotalRecordCount, string.Empty, string.Empty, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false, RoomDateFormat, CurrentTimeZone);
                if (SessionHelper.RoleID >= 0 && DataFromDB != null)
                {
                    DataFromDB = DataFromDB.Where(x => x.ReportName != "EnterpriseRoom" && x.ReportName != "EnterpriseUser").ToList();
                }
                Session["ReportMasterList"] = DataFromDB;
                return View();
            }
            else
            {
                return RedirectToAction("MyProfile", "Master");
            }
        }

        public ActionResult ReportListAJAX(JQueryDataTableParamModel param)
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
                    sortColumnName = "ID";

                if (sortDirection == "asc")
                    sortColumnName = sortColumnName + " asc";
                else
                    sortColumnName = sortColumnName + " desc";
            }
            else
                sortColumnName = "ID desc";


            string searchQuery = string.Empty;

            long TotalRecordCount = 0;
            bool UserConsignmentAllowed = SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowOrderToConsignedItem);
            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            List<ReportMasterDTO> DataFromDB = objReportMasterDAL.GetPagedReports(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, RoomDateFormat, CurrentTimeZone);

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
        public JsonResult ReportSave(ReportMasterDTO objDTO)
        {
            string message = "";
            ReportMasterDAL obj = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            if (objDTO != null)
            {
                if (!string.IsNullOrEmpty(objDTO.AllowedAttahmentReports))
                {
                    objDTO.AllowedAttahmentReports = objDTO.AllowedAttahmentReports.Trim(',');
                }
                if (!string.IsNullOrEmpty(objDTO.AllowedIMMActions))
                {
                    objDTO.AllowedIMMActions = objDTO.AllowedIMMActions.Trim(',');
                }
            }
            if (objDTO.AlertConfigID == 0)
            {
                objDTO.CreatedBy = SessionHelper.UserID;
                objDTO.UpdatedBy = SessionHelper.UserID;
                objDTO.CreatedOn = DateTimeUtility.DateTimeNow;
                objDTO.UpdatedON = DateTimeUtility.DateTimeNow;

                long ReturnVal = obj.InsertReportAlertConfig(objDTO, SessionHelper.CompanyID, SessionHelper.UserID);
                if (ReturnVal > 0)
                {
                    message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";

                }
                else
                {
                    message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);
                }
            }
            else
            {
                bool ReturnVal = obj.EditReportAlertConfig(objDTO, SessionHelper.CompanyID, SessionHelper.UserID);
                if (ReturnVal)
                {
                    message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                }
                else
                {
                    message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);
                }
            }
            return Json(new { Message = message }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ReportCreate()
        {
            ReportMasterDTO objDTO = new ReportMasterDTO()
            {
                CreatedOn = DateTimeUtility.DateTimeNow,
                UpdatedON = DateTimeUtility.DateTimeNow,
                CreatedBy = SessionHelper.UserID,
                CreatedByName = SessionHelper.UserName,
                UpdatedBy = SessionHelper.UserID,
                RoomID = SessionHelper.RoomID,
                CompanyID = SessionHelper.CompanyID,
                RoomName = SessionHelper.RoomName,
                UpdatedByName = SessionHelper.UserName,

            };

            return PartialView("_CreateReportDetails", objDTO);
        }


        public ActionResult ReportEdit(long ID, long EmailTemplateID, string ItemType)
        {
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            if (IsDeleted || IsArchived)
            {
                ViewBag.ViewOnly = true;
            }


            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);

            ReportMasterDTO objDTO = objReportMasterDAL.GetReportSigleRecord(ID, EmailTemplateID, ItemType);

            /*
            if(objDTO != null && objDTO.ParentID.GetValueOrDefault(0) > 0)
            {
                ReportMasterDTO objParentDTO = objReportMasterDAL.GetParentReportSigleRecord(ID, EmailTemplateID, ItemType);
                if(objParentDTO != null && objParentDTO.ID > 0)
                {
                    objDTO.ParentReportName = objParentDTO.ReportName;
                    objDTO.AllowScheduleIMM = objParentDTO.AllowScheduleIMM;
                    objDTO.AllowScheduleHourly = objParentDTO.AllowScheduleHourly;
                    objDTO.AllowScheduleWeekly = objParentDTO.AllowScheduleWeekly;
                    objDTO.AllowScheduleMonthly = objParentDTO.AllowScheduleMonthly;
                    objDTO.AllowedIMMActions = objParentDTO.AllowedIMMActions;
                    objDTO.AllowDataSelectSinceLastReportFilter = objParentDTO.AllowDataSelectSinceLastReportFilter;
                    objDTO.AllowDataSelectFirstOfMonth = objParentDTO.AllowDataSelectFirstOfMonth;
                    objDTO.AllowDataSinceFilter = objParentDTO.AllowDataSinceFilter;
                    objDTO.AllowSupplierFilter = objParentDTO.AllowSupplierFilter;
                    objDTO.AllowPDFAttachment = objParentDTO.AllowPDFAttachment;
                    objDTO.AllowExcelAttachment = objParentDTO.AllowExcelAttachment;
                    objDTO.AllowAttachmentSelection = objParentDTO.AllowAttachmentSelection;
                    objDTO.AllowedAttahmentReports = objParentDTO.AllowedAttahmentReports;
                    objDTO.AllowScheduleDaily = objParentDTO.AllowScheduleDaily;
                    objDTO.AllowRangeDataSelect = objParentDTO.AllowRangeDataSelect;
                }
            }
            */

            return PartialView("_CreateReportDetails", objDTO);
        }

        public JsonResult GetTransactionEventFromCode(string vEventCode)
        {
            List<TransactionEventMasterDTO> lstEventDTO = new List<TransactionEventMasterDTO>();
            lstEventDTO = new eTurns.DAL.ReportMasterDAL(SessionHelper.EnterPriseDBName).GetTransactionEventByCode(vEventCode).ToList();
            return Json(new { Status = true, EventList = lstEventDTO }, JsonRequestBehavior.AllowGet);
        }

        #region For Default Print option as par module WI-4440

        [HttpGet]
        public ActionResult ModuleWiseReportListForDefaultPrint()
        {
            List<ModuleWiseReportListForDefaultPrintDTO> lstModulereports = new List<ModuleWiseReportListForDefaultPrintDTO>();

            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            List<ModuleWiseReportListForDefaultPrintDTO> ModuleWiseReportList = objReportMasterDAL.GetModuleWiseReportListForDefaultPrint(SessionHelper.CompanyID);

            List<ModuleWiseReportListForDefaultPrintDTO> distinctModule = new List<ModuleWiseReportListForDefaultPrintDTO>();
            distinctModule = ModuleWiseReportList.GroupBy(x => x.ModuleID).Select(x => x.FirstOrDefault()).ToList();

            foreach (ModuleWiseReportListForDefaultPrintDTO ModuleList in distinctModule)
            {
                ModuleWiseReportListForDefaultPrintDTO objModulereports = new ModuleWiseReportListForDefaultPrintDTO();
                objModulereports.ModuleID = ModuleList.ModuleID;
                if (!string.IsNullOrWhiteSpace(ModuleList.ResModuleName))
                {
                    string ResourceAppModuleName = ResourceHelper.GetReportModuleResource(ModuleList.ResModuleName);
                    if (!string.IsNullOrWhiteSpace(ResourceAppModuleName))
                    {
                        objModulereports.ModuleName = ResourceAppModuleName;
                    }
                    else
                    {
                        objModulereports.ModuleName = ModuleList.ModuleName;
                    }
                }
                else
                {
                    objModulereports.ModuleName = ModuleList.ModuleName;
                }

                objModulereports.lstModuleWiseMasterReport = new List<ModuleWiseMasterReportList>();

                foreach (ModuleWiseReportListForDefaultPrintDTO Masterreportlist in ModuleWiseReportList.Where(x => x.ModuleID == ModuleList.ModuleID && x.isBaseReport == true).GroupBy(x => new { ModuleID = x.ModuleID, MasterReportID = x.MasterReportID, x.ChildParentID }).Select(x => x.FirstOrDefault()).ToList())
                {
                    ModuleWiseMasterReportList objMasterreport = new ModuleWiseMasterReportList();
                    objMasterreport.MasterReportID = Masterreportlist.MasterReportID;
                    if (!string.IsNullOrWhiteSpace(Masterreportlist.ResourceKey))
                    {
                        string ResMasterReportName = "";
                        ResMasterReportName = ResourceHelper.GetReportNameByResource(Masterreportlist.ResourceKey);
                        if (!string.IsNullOrWhiteSpace(ResMasterReportName))
                        {
                            objMasterreport.MasterReportName = ResMasterReportName;
                        }
                        else
                        {
                            objMasterreport.MasterReportName = Masterreportlist.MasterReportName;
                        }
                    }
                    else
                    {
                        objMasterreport.MasterReportName = Masterreportlist.MasterReportName;
                    }
                    objModulereports.lstModuleWiseMasterReport.Add(objMasterreport);

                    objModulereports.lstModuleWiseMasterReport[objModulereports.lstModuleWiseMasterReport.Count - 1].lstModuleWiseChildReport = new List<ModuleWiseChildReportList>();

                    List<ModuleWiseReportListForDefaultPrintDTO> lstChildRrports = (from rpt in ModuleWiseReportList
                                                                                    orderby rpt.MasterReportName
                                                                                    where rpt.Parents.Split('.').ToList().Contains(Masterreportlist.MasterReportID.ToString())
                                                                                    select rpt
                                                                                  ).ToList();

                    //foreach (ModuleWiseReportListForDefaultPrintDTO Childreportlist in ModuleWiseReportList.Where(x => x.MasterReportID && x.ModuleID == ModuleList.ModuleID))
                    foreach (ModuleWiseReportListForDefaultPrintDTO Childreportlist in lstChildRrports.Where(x => x.ModuleID == ModuleList.ModuleID))
                    {
                        ModuleWiseChildReportList objChildreport = new ModuleWiseChildReportList();
                        objChildreport.ChildReportID = Childreportlist.MasterReportID;
                        objChildreport.ChildReportName = Childreportlist.MasterReportName;
                        objChildreport.ISEnterpriseReport = Childreportlist.ISEnterpriseReport.GetValueOrDefault(false);
                        objModulereports.lstModuleWiseMasterReport[objModulereports.lstModuleWiseMasterReport.Count - 1].lstModuleWiseChildReport.Add(objChildreport);
                    }
                }

                lstModulereports.Add(objModulereports);
            }

            return View(lstModulereports);
        }

        [HttpGet]
        public JsonResult GetReportForDefaultPrintByModuleID(Int64? ModuleID)
        {
            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            List<ModuleWiseReportForDefaultPrintDTO> ModuleWiseselectedDefaultReport = objReportMasterDAL.GetReportForDefaultPrintByModuleID(ModuleID ?? null, SessionHelper.RoomID, SessionHelper.CompanyID, null);

            return Json(new { Status = true, ModuleWiseDefaultPrintList = ModuleWiseselectedDefaultReport }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckReportISEnterpriseReport(string ReportID)
        {
            bool ISEnterpriseReport = false;
            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            ReportBuilderDTO objReportDetails = objReportMasterDAL.GetReportDetail(Convert.ToInt64(ReportID));
            if (objReportDetails != null && objReportDetails.ID > 0)
            {
                ISEnterpriseReport = objReportDetails.ISEnterpriseReport.GetValueOrDefault(false);
            }
            return Json(new { Status = true, ISEnterpriseReport = ISEnterpriseReport }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ModuleWiseReportSaveForDefaultPrint(List<ModuleWiseReportForDefaultPrintDTO> lstDTO)
        {
            string message = "";
            ReportMasterDAL obj = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            if (lstDTO != null && lstDTO.Count > 0)
            {
                long ReturnVal = obj.InsertOrUpdateModuleWiseDefaultPrint(lstDTO, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                if (ReturnVal > 0)
                {
                    message = ResMessage.SaveMessage;
                }
                else
                {
                    message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);
                }
            }
            else
            {
                message = ResReportMaster.SelectDefaultReportForPrint;
            }
            return Json(new { Message = message }, JsonRequestBehavior.AllowGet);
        }

        public string GetDefaultReportIDByModuleName(string ModuleName, out bool? HideHeader, out bool? ShowSignature)
        {
            string reportID = "0";
            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            ReportMaster objReportMaster = new ReportMaster();
            HideHeader = false;
            ShowSignature = false;
            if (!string.IsNullOrWhiteSpace(ModuleName))
            {

                objReportMaster = objReportMasterDAL.GetReportIDNew(ModuleName);
                HideHeader = objReportMaster.HideHeader ?? false;
                ShowSignature = objReportMaster.ShowSignature ?? false;
                reportID = Convert.ToString(objReportMaster.ID);
            }
            // if (ModuleName == "Requisition" || ModuleName == "Order" || ModuleName == "Return Order" || ModuleName == "Work Order" || ModuleName == "OrderMasterList" || ModuleName == "Room")
            {
                string ModName = string.Empty;
                bool isAppModuleApplay = false;
                switch (ModuleName)
                {
                    case "Assets":
                        ModName = "Assetmaster";
                        break;
                    case "Company":
                        ModName = "AdminCompany";
                        isAppModuleApplay = true;
                        break;
                    case "Cumulative Pull":
                        ModName = "Consume_Pull";
                        isAppModuleApplay = true;
                        break;
                    case "Discrepancy Report":
                        ModName = "Item-Discrepency";
                        break;
                    case "Enterprises List":
                        ModName = "AdminEnterprise";
                        isAppModuleApplay = true;
                        break;
                    case "eVMI Poll History":
                        ModName = "eVMIPollH";
                        break;
                    case "eVMI Usage":
                    case "eVMI Usage No Header":
                        ModName = "eVMI";
                        break;
                    case "eVMI Usage Manual Count":
                        ModName = "eVMI_ManualCount";
                        break;
                    case "Expiring Items":
                        ModName = "ExpiringItems";
                        break;
                    case "InStock":
                        ModName = "InStockByBin";
                        break;
                    case "InStock By Activity":
                        ModName = "InStockByActivity";
                        break;
                    case "InStock Margin":
                        ModName = "InStockByBinMargin";
                        break;
                    case "Instock with QOH":
                        ModName = "InStockWithQOH";
                        break;
                    case "Inventory Count":
                        ModName = "InventoryCount";
                        isAppModuleApplay = true;
                        break;
                    case "Item":
                    case "Item List":
                    case "ItemListingGroup":
                    case "Items":
                        ModName = "InventoryItem";
                        isAppModuleApplay = true;
                        break;
                    case "Kit":
                        ModName = "InventoryKit";
                        isAppModuleApplay = true;
                        break;
                    case "Maintenance":
                        ModName = "Maintenance";
                        break;
                    case "Order":
                    case "Order Grouping":
                    case "Order List":
                    case "OrderMasterList":
                        ModName = "ReplenishOrder";
                        isAppModuleApplay = true;
                        break;
                    case "Orders With LineItems":
                        ModName = "Replenish_Order";
                        break;
                    case "Project Spend":
                        ModName = "ConsumeProjectSpend";
                        isAppModuleApplay = true;
                        break;
                    case "Pull":
                        ModName = "ConsumePull";
                        isAppModuleApplay = true;
                        break;
                    case "Pull Completed":
                    case "Pull Incomplete":
                    case "Pull No Header":
                    case "Pull Summary":
                    case "Pull Summary By ConsignedPO":
                    case "Pull Summary by Quarter":
                    case "Total Pulled":
                        ModName = "Consume_Pull";
                        isAppModuleApplay = true;
                        break;
                    case "Pull Summary By WO":
                        ModName = "WOPullSummary";
                        break;
                    case "Receivable Items":
                        ModName = "Receive";
                        break;
                    case "Received Items":
                        ModName = "Range-Receive";
                        break;
                    case "Return Item Candidates":
                        ModName = "Range-Receive";
                        break;
                    case "Requisition":
                        ModName = "ConsumeRequisition";
                        isAppModuleApplay = true;
                        break;
                    case "Requisition Item Summary":
                        ModName = "ReqItemSummary";
                        break;
                    case "Requisition With LineItems":
                        ModName = "Range-Consume_Requisition";
                        break;
                    case "Return Order":
                        ModName = "ReplenishReturnOrder";
                        isAppModuleApplay = true;
                        break;
                    case "Room":
                        ModName = "AdminRoom";
                        isAppModuleApplay = true;
                        break;
                    case "Staging":
                        ModName = "Staging";
                        break;
                    case "Sugg. Orders of Exp. Date":
                        ModName = "SuggOrderOfExpDate";
                        break;
                    case "Suggested Orders":
                        ModName = "ReplenishCart";
                        isAppModuleApplay = true;
                        break;
                    case "Tools":
                        ModName = "Tool";
                        isAppModuleApplay = true;
                        break;
                    case "Tools checked out":
                    case "Tools Checked Out":
                        ModName = "CheckOutTool";
                        break;
                    case "Tools CheckIn-out History":
                        ModName = "ToolInOutHistory";
                        break;
                    case "Transfer":
                        ModName = "ReplenishTransfer";
                        isAppModuleApplay = true;
                        break;
                    case "Transfer With LineItems":
                        ModName = "TransferdItems";
                        break;
                    case "UnfulFilled Order LineItems":
                    case "UnfulFilled Orders":
                        ModName = "UnfulFilledOrders";
                        break;
                    case "Users":
                        ModName = "AdminUser";
                        isAppModuleApplay = true;
                        break;
                    case "Work Order":
                        ModName = "ConsumeWorkOrder";
                        isAppModuleApplay = true;
                        break;
                    case "Workorders List":
                        ModName = "WorkorderList";
                        break;
                    case "Supplier":
                        ModName = "Supplier";
                        isAppModuleApplay = true;
                        break;
                    case "Asset Maintenance":
                        ModName = "Assetmaster";
                        isAppModuleApplay = true;
                        break;
                    case "Maintenance Due":
                        isAppModuleApplay = true;
                        ModName = "MaintenanceToolAsset";
                        break;
                    case "Quote":
                    case "Quote List":

                        ModName = "ReplenishQuote";
                        isAppModuleApplay = true;
                        break;
                    default:
                        ModName = ModuleName;
                        break;
                }
                if (isAppModuleApplay)
                {
                    Int64 RID = objReportMasterDAL.GetDefaultReportIDBasedonModuleName(ModName, SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (RID > 0)
                    {
                        ReportBuilderDTO objReportDetails = objReportMasterDAL.GetReportDetail(RID);
                        HideHeader = (objReportDetails != null ? objReportDetails.HideHeader ?? false : false);
                        ShowSignature = (objReportDetails != null ? objReportDetails.ShowSignature ?? false : false);
                        if (RID > 0)
                        {
                            reportID = Convert.ToString(RID);
                        }
                    }
                }
                else
                {
                    objReportMaster = objReportMasterDAL.GetDefaultReportIDNew(ModName, SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (objReportMaster != null)
                    {
                        Int64 RID = Convert.ToInt64(objReportMaster.ID);
                        HideHeader = objReportMaster.HideHeader ?? false;
                        ShowSignature = objReportMaster.ShowSignature ?? false;
                        if (RID > 0)
                        {
                            reportID = Convert.ToString(RID);
                        }
                    }
                }
            }
            return reportID;
        }

        #region For display room level default print repotrt based on setting file

        public string GetDefaultPrintRoomReportIDBySetting(string ModuleName, out bool? HideHeader, out bool? ShowSignature)
        {
            string reportID = "0";
            HideHeader = false;
            ShowSignature = false;
            string strEntID = string.Empty;
            string strCompanyID = string.Empty;
            string strRoomID = string.Empty;
            //System.Xml.Linq.XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));
            //if (Settinfile != null && Settinfile.Element("RoomReportGrid") != null)
            //    strEntID = Convert.ToString(Settinfile.Element("RoomReportGrid").Value);

            if (SiteSettingHelper.RoomReportGrid != string.Empty)
                strEntID = Convert.ToString(SiteSettingHelper.RoomReportGrid);

            //if (Settinfile != null && Settinfile.Element("RoomReportGridCompanyID") != null)
            //    strCompanyID = Convert.ToString(Settinfile.Element("RoomReportGridCompanyID").Value);

            if (SiteSettingHelper.RoomReportGridCompanyID != string.Empty)
                strCompanyID = Convert.ToString(SiteSettingHelper.RoomReportGridCompanyID);

            //if (Settinfile != null && Settinfile.Element("RoomReportGridRoomID") != null)
            //    strRoomID = Convert.ToString(Settinfile.Element("RoomReportGridRoomID").Value);

            if (SiteSettingHelper.RoomReportGridRoomID != string.Empty)
                strRoomID = Convert.ToString(SiteSettingHelper.RoomReportGridRoomID);

            if (!string.IsNullOrWhiteSpace(strEntID) && !string.IsNullOrWhiteSpace(strCompanyID) && !string.IsNullOrWhiteSpace(strRoomID))
            {
                EnterpriseMasterDAL objEntDal = new EnterpriseMasterDAL();
                objEntDTO = objEntDal.GetNonDeletedEnterpriseByIdPlain(Convert.ToInt64(strEntID));

                if (objEntDTO != null && !string.IsNullOrWhiteSpace(objEntDTO.EnterpriseDBName))
                {
                    ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(objEntDTO.EnterpriseDBName);
                    HideHeader = false;
                    ShowSignature = false;
                    string ModName = string.Empty;
                    bool isAppModuleApplay = false;
                    switch (ModuleName)
                    {
                        case "Room":
                            ModName = "AdminRoom";
                            isAppModuleApplay = true;
                            break;
                        default:
                            ModName = ModuleName;
                            break;
                    }
                    if (isAppModuleApplay)
                    {
                        Int64 RID = objReportMasterDAL.GetDefaultReportIDBasedonModuleName(ModName, Convert.ToInt64(strRoomID), Convert.ToInt64(strCompanyID));
                        if (RID > 0)
                        {
                            ReportBuilderDTO objReportDetails = objReportMasterDAL.GetReportDetail(RID);
                            HideHeader = (objReportDetails != null ? objReportDetails.HideHeader ?? false : false);
                            ShowSignature = (objReportDetails != null ? objReportDetails.ShowSignature ?? false : false);
                            if (RID > 0)
                            {
                                reportID = Convert.ToString(RID);
                            }
                        }
                    }
                }
            }
            return reportID;
        }

        #endregion

        public string GetDefaultReportForRoom(string ModuleName, out bool? HideHeader, out bool? ShowSignature)
        {
            //string strEntReport = string.Empty;
            //System.Xml.Linq.XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml")); //System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
            //if (Settinfile != null && Settinfile.Element("RoomReportGrid") != null)
            //    strEntReport = Convert.ToString(Settinfile.Element("RoomReportGrid").Value);
            //EnterpriseMasterDAL objEntDal = new EnterpriseMasterDAL();
            ////EnterpriseDTO objEntDTO = objEntDal.GetEnterpriseByName(RoomReportGrid.Name);
            //EnterpriseDTO objEntDTO = objEntDal.GetEnterpriseByName(strEntReport);
            //EntDTO = objEntDTO;

            string reportID = "0";
            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(objEntDTO.EnterpriseDBName);
            ReportMaster objReportMaster = new ReportMaster();
            HideHeader = false;
            ShowSignature = false;
            if (!string.IsNullOrWhiteSpace(ModuleName))
            {
                objReportMaster = objReportMasterDAL.GetReportIDNew(ModuleName);
                HideHeader = objReportMaster.HideHeader ?? false;
                ShowSignature = objReportMaster.ShowSignature ?? false;
                reportID = Convert.ToString(objReportMaster.ID);
            }

            return reportID;
        }

        #endregion

        #region for find pullsummaryreport child WI-5107 Pull Summary report - consolidate each item into only one line item summary

        public JsonResult FindPullSummaryChildReport()
        {
            List<ReportMasterDTO> lstContainesReports = new List<ReportMasterDTO>();
            List<ReportMasterDTO> lstReportMasterDTO = new List<ReportMasterDTO>();
            lstReportMasterDTO = new eTurns.DAL.ReportMasterDAL(SessionHelper.EnterPriseDBName).FindPullSummaryChildReport().ToList();

            if (lstReportMasterDTO != null && lstReportMasterDTO.Count > 0)
            {
                string ReportBasePath = CommonUtility.RDLCBaseFilePath;
                if (!string.IsNullOrEmpty(ReportBasePath))
                {
                    foreach (string file in Directory.EnumerateFiles(ReportBasePath, "*.rdlc", SearchOption.AllDirectories))
                    {
                        string ReportName = Path.GetFileName(file);

                        ReportMasterDTO objavailableReports = new ReportMasterDTO();
                        objavailableReports = lstReportMasterDTO.Where(x => x.ReportFileName.ToLower().Equals(ReportName.ToLower())).FirstOrDefault();

                        if (objavailableReports != null)
                        {
                            string text1 = System.IO.File.ReadAllText(file);
                            if (text1.Contains("=Fields!PullBin.Value")
                                || text1.Contains("=Fields!ConsignedPO.Value")
                                || text1.Contains("=Fields!customer.Value")
                                || text1.Contains("=Fields!WorkOrderDescription.Value"))
                            {
                                lstContainesReports.Add(objavailableReports);
                            }
                        }
                    }
                }
            }

            return Json(new { Status = true, ReportList = lstContainesReports }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        public JsonResult GetChildReportsFromParentID(Int64 ParentReportID)
        {
            string strReportName = "";
            bool hasChildReport = false;
            List<ReportMasterDTO> lstReports = new List<ReportMasterDTO>();
            lstReports = new eTurns.DAL.ReportMasterDAL(SessionHelper.EnterPriseDBName).GetChildReportsFromParentID(ParentReportID).ToList();
            if (lstReports != null && lstReports.Count > 0)
            {
                strReportName = string.Join(",", lstReports.Select(x => x.ReportName));
                hasChildReport = true;
            }
            return Json(new { ReportsName = strReportName, hasChildReport = hasChildReport }, JsonRequestBehavior.AllowGet);
        }

    }
}
