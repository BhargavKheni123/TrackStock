using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using eTurns.DTO.Helper;
namespace eTurnsWeb.Helper
{
    public class ReportMail
    {
        #region [Global Declaration]

        string SubReportResFile = string.Empty;
        long ParentID = 0;
        string ReportBasePath = System.Configuration.ConfigurationManager.AppSettings["RDLCBaseFilePath"];
        string ResourceBaseFilePath = System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"];
        XNamespace ns = XNamespace.Get("http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition");
        XNamespace nsrd = XNamespace.Get("http://schemas.microsoft.com/SQLServer/reporting/reportdesigner");
        Dictionary<string, string> rptPara = null;
        string rdlPath = string.Empty;
        string strSubTablix = string.Empty;
        string DBconectstring = string.Empty;

        #endregion

        #region [Class Constructor]
        public ReportMail()
        {

        }
        #endregion

        #region [Send Email Method]

        public void SendReportNotification(NotificationDTO objNotificationDTO, EnterpriseDTO objEnterpriseDTO, Dictionary<string, string> Params)
        {
            List<eMailAttachmentDTO> lstAttachments = new List<eMailAttachmentDTO>();
            string EmailSubject = string.Empty;
            string EmailBody = string.Empty;
            string ToEmailAddersses = string.Empty;
            if (objNotificationDTO != null)
            {
                ToEmailAddersses = objNotificationDTO.EmailAddress;
                if (objNotificationDTO.EmailTemplateDetail != null && objNotificationDTO.EmailTemplateDetail.lstEmailTemplateDtls != null && objNotificationDTO.EmailTemplateDetail.lstEmailTemplateDtls.Count() > 0)
                {
                    EmailSubject = objNotificationDTO.EmailTemplateDetail.lstEmailTemplateDtls.First().MailSubject;
                    RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(objEnterpriseDTO.EnterpriseDBName);
                    eTurnsRegionInfo objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(objNotificationDTO.RoomID, objNotificationDTO.CompanyID, -1);
                    string DateTimeFormat = "MM/dd/yyyy";
                    DateTime TZDateTimeNow = DateTime.UtcNow;
                    if (objeTurnsRegionInfo != null)
                    {
                        DateTimeFormat = objeTurnsRegionInfo.ShortDatePattern;// + " " + objeTurnsRegionInfo.ShortTimePattern;
                        TZDateTimeNow = objeTurnsRegionInfo.TZDateTimeNow ?? DateTime.UtcNow;
                    }
                    if (EmailSubject != null)
                    {
                        string CurrentDate = TZDateTimeNow.ToString(DateTimeFormat);
                        // EmailSubject = EmailSubject.Replace("@@DATE@@", CurrentDate).Replace("@@date@@", CurrentDate).Replace("@@Date@@", CurrentDate);
                        EmailSubject = Regex.Replace(EmailSubject, "@@DATE@@", CurrentDate, RegexOptions.IgnoreCase);
                        if (!string.IsNullOrWhiteSpace(objNotificationDTO.CompanyName))
                        {
                            EmailSubject = Regex.Replace(EmailSubject, "@@COMPANYNAME@@", objNotificationDTO.CompanyName, RegexOptions.IgnoreCase);
                        }
                        if (!string.IsNullOrWhiteSpace(objNotificationDTO.RoomName))
                        {
                            EmailSubject = Regex.Replace(EmailSubject, "@@ROOMNAME@@", objNotificationDTO.RoomName, RegexOptions.IgnoreCase);
                        }

                        EmailSubject = Regex.Replace(EmailSubject, "@@Year@@", Convert.ToString(DateTime.UtcNow.Year), RegexOptions.IgnoreCase);


                    }
                    EmailBody = objNotificationDTO.EmailTemplateDetail.lstEmailTemplateDtls.First().MailBodyText;
                    if (EmailBody != null && objNotificationDTO != null)
                    {
                        string CurrentDate = TZDateTimeNow.ToString(DateTimeFormat);
                        //EmailBody = EmailBody.Replace("@@DATE@@", CurrentDate).Replace("@@date@@", CurrentDate).Replace("@@Date@@", CurrentDate);
                        EmailBody = Regex.Replace(EmailBody, "@@DATE@@", CurrentDate, RegexOptions.IgnoreCase);
                        if (!string.IsNullOrWhiteSpace(objNotificationDTO.CompanyName))
                        {
                            EmailBody = Regex.Replace(EmailBody, "@@COMPANYNAME@@", objNotificationDTO.CompanyName, RegexOptions.IgnoreCase);
                        }
                        if (!string.IsNullOrWhiteSpace(objNotificationDTO.RoomName))
                        {
                            EmailBody = Regex.Replace(EmailBody, "@@ROOMNAME@@", objNotificationDTO.RoomName, RegexOptions.IgnoreCase);
                        }
                        EmailBody = Regex.Replace(EmailBody, "@@Year@@", Convert.ToString(DateTime.UtcNow.Year), RegexOptions.IgnoreCase);

                    }
                    //EmailBody=SendMailForPendingOrders(objNotificationDTO, objEnterpriseDTO, objNotificationDTO.ReportMasterDTO.ReportName, EmailBody);
                }
            }
            lstAttachments = GenerateBytesBasedOnAttachment(objNotificationDTO, objEnterpriseDTO, Params, ref EmailBody);
            // if (lstAttachments.Count() > 0 || objNotificationDTO.SendEmptyEmail)
            {
                if (objNotificationDTO.NotificationMode == 1)
                {
                    CreateReportMail(lstAttachments, EmailSubject, EmailBody, ToEmailAddersses, objNotificationDTO, objEnterpriseDTO);
                }
                else if (objNotificationDTO.NotificationMode == 2)
                {
                    if (lstAttachments.Count > 0)
                    {
                        //SaveAttachMentsTosFTP(objNotificationDTO, objEnterpriseDTO, lstAttachments);
                        //CreateReportMail(lstAttachments, EmailSubject, EmailBody, Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["FailedEmailTo"]), objNotificationDTO, objEnterpriseDTO);
                    }
                }
            }
        }

        #endregion

        #region [Helper Method]

        public List<eMailAttachmentDTO> GenerateBytesBasedOnAttachment(NotificationDTO objNotificationDTO, EnterpriseDTO objEnterprise, Dictionary<string, string> Params, ref string Emailbody)
        {
            List<eMailAttachmentDTO> bytes = new List<eMailAttachmentDTO>();

            ReportMailLogDTO objReportMailLogDTO = null;
            ReportMasterDAL objDAL = null;
            string strEmail = string.Empty;
            string MasterReportResFile = string.Empty;
            IEnumerable<XElement> lstTablix = null;
            IEnumerable<XElement> lstUpdateTablix = null;
            IEnumerable<XElement> lstReportPara = null;
            List<ReportParameter> rpt = null;
            SqlConnection myConnection = null;
            SqlCommand cmd = null;
            SqlDataAdapter sqla = null;
            DataTable dt = null;
            IEnumerable<XElement> lstQueryPara = null;
            SqlParameter slpar = null;
            XElement objReportPara = null;
            ReportViewer ReportViewer1 = null;
            XDocument docSub = null;
            ReportDataSource rds = null;
            XDocument doc = null;
            RegionSettingDAL rsDAL = null;
            eTurnsRegionInfo rsInfo = null;
            eTurns.DAL.AlertMail amDAL = null;
            //Dictionary<string, string> LocalDictRptPara = null;
            try
            {
                if (objNotificationDTO != null)
                {
                    objDAL = new ReportMasterDAL(objEnterprise.EnterpriseDBName);
                    rsDAL = new RegionSettingDAL(objEnterprise.EnterpriseDBName);
                    rsInfo = rsDAL.GetRegionSettingsById(objNotificationDTO.RoomID, objNotificationDTO.CompanyID, 0);
                    amDAL = new eTurns.DAL.AlertMail();
                    //Get mail logo 

                    /* TO DO : remove*/
                    objReportMailLogDTO = objDAL.GetReportMaillog(Convert.ToInt64(objNotificationDTO.ID), false, null);


                    strEmail = Convert.ToString(objNotificationDTO.EmailAddress);

                    /* TO DO : attchemtn maaster Report DTO*/
                    if (objNotificationDTO.ReportMasterDTO != null)
                    {
                        //Set values from ReportBuilder Objects
                        MasterReportResFile = objNotificationDTO.ReportMasterDTO.MasterReportResFile;
                        SubReportResFile = MasterReportResFile;
                        string Reportname = objNotificationDTO.ReportMasterDTO.ReportName;
                        string MasterReportname = objNotificationDTO.ReportMasterDTO.ReportFileName;
                        string SubReportname = objNotificationDTO.ReportMasterDTO.SubReportFileName;
                        string mainGuid = "RPT_" + Guid.NewGuid().ToString().Replace("-", "_");
                        string subGuid = "SubRPT_" + Guid.NewGuid().ToString().Replace("-", "_");
                        string ReportPath = string.Empty;
                        bool hasSubReport = false;
                        string strCulture = "en-US";
                        if (rsInfo != null && !string.IsNullOrEmpty(rsInfo.CultureCode))
                            strCulture = rsInfo.CultureCode;

                        if (!string.IsNullOrEmpty(objNotificationDTO.CultureCode))
                            strCulture = objNotificationDTO.CultureCode;

                        ParentID = objNotificationDTO.ReportMasterDTO.ParentID ?? 0;
                        if (objNotificationDTO.ReportMasterDTO.ParentID > 0)
                        {

                            if (objNotificationDTO.ReportMasterDTO.ISEnterpriseReport.GetValueOrDefault(false))
                            {
                                ReportPath = CopyFiletoTempForAlert(ReportBasePath + @"\" + objEnterprise.ID + @"\EnterpriseReport" + @"\" + MasterReportname, mainGuid, ReportBasePath + @"\Temp");
                            }
                            else
                            {
                                ReportPath = CopyFiletoTempForAlert(ReportBasePath + @"\" + objEnterprise.ID + @"\" + objNotificationDTO.CompanyID + @"\" + MasterReportname, mainGuid, ReportBasePath + @"\Temp");
                            }
                        }
                        else
                        {
                            ReportPath = CopyFiletoTempForAlert(ReportBasePath + @"\" + objEnterprise.ID + @"\BaseReport" + @"\\" + MasterReportname, mainGuid, ReportBasePath + @"\Temp");
                        }



                        doc = XDocument.Load(ReportPath);
                        doc = amDAL.AddFormatToTaxbox(doc, rsInfo);
                        doc.Save(ReportPath);
                        doc = XDocument.Load(ReportPath);

                        lstTablix = doc.Descendants(ns + "Tablix");

                        if (doc.Descendants(ns + "Subreport") != null && doc.Descendants(ns + "Subreport").Count() > 0)
                        {
                            hasSubReport = true;
                        }

                        //Hardik Code Started

                        eTurns.DAL.AlertMail objAlertMail = new eTurns.DAL.AlertMail();
                        if (objNotificationDTO != null && (objNotificationDTO.HideHeader))
                        {
                            if (!hasSubReport && !objNotificationDTO.ReportMasterDTO.IsNotEditable.GetValueOrDefault(false)
                                             && (objNotificationDTO.ReportMasterDTO.ReportType == 3 || objNotificationDTO.ReportMasterDTO.ReportType == 1)
                                             && (objNotificationDTO.HideHeader))
                            {
                                doc = objAlertMail.GetAdditionalHeaderRow(doc, objNotificationDTO.ReportMasterDTO, objNotificationDTO.CompanyName, objNotificationDTO.RoomName, EnterpriseDBName: objEnterprise.EnterpriseDBName);
                                doc.Save(ReportPath);
                                doc = XDocument.Load(ReportPath);

                            }
                        }

                        if (objNotificationDTO != null && (objNotificationDTO.ShowSignature))
                        {
                            doc = objAlertMail.GetFooterForSignature(doc, objNotificationDTO.ReportMasterDTO);
                            doc.Save(ReportPath);
                            doc = XDocument.Load(ReportPath);
                        }
                        if (objNotificationDTO != null && objNotificationDTO.SortSequence != null && (!string.IsNullOrWhiteSpace(objNotificationDTO.SortSequence)))
                        {
                            objNotificationDTO.ReportMasterDTO.SortColumns = objNotificationDTO.SortSequence;
                        }
                        //Hardik Code ended


                        string strTablix = string.Empty;
                        if (lstTablix != null && lstTablix.ToList().Count > 0)
                        {
                            strTablix = lstTablix.ToList()[0].ToString();
                        }
                        lstUpdateTablix = UpdateResourceNew(lstTablix, MasterReportResFile, strCulture, objEnterprise.ID, objNotificationDTO.CompanyID, objNotificationDTO.RoomID);
                        //lstUpdateTablix = UpdateResource(lstTablix, MasterReportResFile, strCulture, objEnterprise.ID, objNotificationDTO.CompanyID);

                        rptPara = SetGetPDFReportParaDictionary(objEnterprise, objNotificationDTO.CompanyID, objNotificationDTO.RoomID, objReportMailLogDTO, objNotificationDTO, Params);


                        if (objNotificationDTO != null && objNotificationDTO.ReportMasterDTO != null && objNotificationDTO.ReportMasterDTO.ReportType == 3 && !hasSubReport && rptPara != null && rptPara.ContainsKey("SortFields") && !string.IsNullOrEmpty(rptPara["SortFields"]))
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


                        doc.Save(ReportPath);
                        lstReportPara = doc.Descendants(ns + "ReportParameter");
                        rpt = new List<ReportParameter>();

                        /* TO DO : remove mail log ref*/



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
                        DBconectstring = DbConnectionHelper.GeteTurnsSQLConnectionString(objEnterprise.EnterpriseDBName, DbConnectionType.GeneralReadOnly.ToString("F"));
                        string connString = doc.Descendants(ns + "ConnectString").FirstOrDefault().Value;
                        myConnection = new SqlConnection();
                        cmd = new SqlCommand();
                        sqla = new SqlDataAdapter();
                        dt = new DataTable();
                        myConnection.ConnectionString = DbConnectionHelper.GeteTurnsSQLConnectionString(objEnterprise.EnterpriseDBName, DbConnectionType.GeneralReadOnly.ToString("F"));
                        cmd.Connection = myConnection;
                        cmd.CommandText = doc.Descendants(ns + "CommandText").FirstOrDefault().Value;
                        cmd.CommandType = CommandType.Text;
                        if (doc.Descendants(ns + "CommandType").FirstOrDefault() != null)
                        {
                            cmd.CommandType = (CommandType)Enum.Parse(typeof(CommandType), doc.Descendants(ns + "CommandType").FirstOrDefault().Value == null ? "Text" : doc.Descendants(ns + "CommandType").FirstOrDefault().Value, true);
                        }

                        lstQueryPara = doc.Descendants(ns + "QueryParameter");

                        if (lstQueryPara != null && lstQueryPara.Count() > 0)
                        {
                            foreach (var item in lstQueryPara)
                            {
                                slpar = new SqlParameter();
                                slpar.ParameterName = item.Attribute("Name").Value;//
                                if (!(hasSubReport && slpar.ParameterName.ToLower().Replace("@", "") == "sortfields") && !(string.IsNullOrEmpty(rptPara.FirstOrDefault(x => x.Key.Replace("@", "") == item.Attribute("Name").Value.Replace("@", "")).Value)))
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
                        cmd.CommandTimeout = 600;
                        sqla = new SqlDataAdapter(cmd);
                        sqla.Fill(dt);
                        if (Emailbody.ToLower().Contains("@@table@@"))
                        {
                            Emailbody = NewEmailBody(dt, Emailbody, doc, MasterReportResFile);
                        }
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            if (objNotificationDTO != null && (objNotificationDTO.XMLValue) != null && (!string.IsNullOrWhiteSpace(objNotificationDTO.XMLValue)))
                            {
                                // List<ReportBuilderDTO> lstReportList = new ReportMasterDAL(objEnterprise.EnterpriseDBName).GetReportList();

                                if ((objNotificationDTO.ReportMasterDTO != null && objNotificationDTO.ReportMasterDTO.ModuleName != null && (objNotificationDTO.ReportMasterDTO.ModuleName.Trim().ToLower() == "consume_requisition" || objNotificationDTO.ReportMasterDTO.ModuleName.Trim().ToLower() == "range-consume_requisition"))
                                || (objNotificationDTO.AttachedReportMasterDTO != null && objNotificationDTO.AttachedReportMasterDTO.ModuleName != null && (objNotificationDTO.AttachedReportMasterDTO.ModuleName.Trim().ToLower() == "consume_requisition" || objNotificationDTO.AttachedReportMasterDTO.ModuleName.Trim().ToLower() == "range-consume_requisition")))
                                {
                                    if (objNotificationDTO.XMLValue.ToLower().IndexOf("status") >= 0)
                                    {
                                        DataView dv = new DataView(dt);
                                        string wherecondition = string.Empty;

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
                                                        if (!string.IsNullOrWhiteSpace(wherecondition))
                                                        {
                                                            wherecondition += "or RequisitionStatus in('" + node["Status" + i].InnerText + "') ";
                                                        }
                                                        else
                                                        {
                                                            wherecondition = " RequisitionStatus in('" + node["Status" + i].InnerText + "') ";
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        nodeList = xmldoc.SelectNodes("/Data/WOStatus");
                                        string WoWhereCondition = string.Empty;
                                        if (nodeList != null && nodeList.Count > 0)
                                        {
                                            foreach (XmlNode node in nodeList)
                                            {
                                                for (int i = 1; i <= node.ChildNodes.Count; i++)
                                                {
                                                    if (!string.IsNullOrWhiteSpace(node["WOStatus" + i].InnerText))
                                                    {
                                                        if (!string.IsNullOrWhiteSpace(WoWhereCondition))
                                                        {
                                                            WoWhereCondition += "or WOStatus in('" + node["WOStatus" + i].InnerText + "') ";
                                                        }
                                                        else
                                                        {
                                                            WoWhereCondition = " WOStatus in('" + node["WOStatus" + i].InnerText + "') ";
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        nodeList = xmldoc.SelectNodes("/Data/OrderStatus");
                                        string OrderWhereCondition = string.Empty;
                                        if (nodeList != null && nodeList.Count > 0)
                                        {
                                            foreach (XmlNode node in nodeList)
                                            {
                                                for (int i = 1; i <= node.ChildNodes.Count; i++)
                                                {
                                                    if (!string.IsNullOrWhiteSpace(node["OrderStatus" + i].InnerText))
                                                    {
                                                        if (!string.IsNullOrWhiteSpace(OrderWhereCondition))
                                                        {
                                                            OrderWhereCondition += "or OrderStatus in('" + node["OrderStatus" + i].InnerText + "') ";
                                                        }
                                                        else
                                                        {
                                                            OrderWhereCondition = " OrderStatus in('" + node["OrderStatus" + i].InnerText + "') ";
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        // query example = "id = 10"

                                        dv.RowFilter = wherecondition; // query example = "id = 10"
                                        dv.RowFilter = WoWhereCondition;
                                        dv.RowFilter = OrderWhereCondition;
                                        dt = dv.ToTable();
                                    }

                                }
                                if (objNotificationDTO != null &&
                                       (objNotificationDTO.ReportMasterDTO != null && objNotificationDTO.ReportMasterDTO.ModuleName != null && (objNotificationDTO.ReportMasterDTO.ModuleName.Trim().ToLower() == "order"))
                                       || (objNotificationDTO.AttachedReportMasterDTO != null && objNotificationDTO.AttachedReportMasterDTO.ModuleName != null && (objNotificationDTO.AttachedReportMasterDTO.ModuleName.Trim().ToLower() == "order"))
                                       )
                                {
                                    if (objNotificationDTO.XMLValue.ToLower().IndexOf("orderstatus") >= 0)
                                    {
                                        DataView dv = new DataView(dt);
                                        XmlDocument xmldoc = new XmlDocument();
                                        xmldoc.LoadXml("<Data>" + objNotificationDTO.XMLValue + "</Data>");
                                        XmlNodeList nodeList = xmldoc.SelectNodes("/Data/OrderStatus");
                                        string WoWhereCondition = string.Empty;
                                        if (nodeList != null && nodeList.Count > 0)
                                        {
                                            foreach (XmlNode node in nodeList)
                                            {
                                                for (int i = 1; i <= node.ChildNodes.Count; i++)
                                                {
                                                    if (!string.IsNullOrWhiteSpace(node["OrderStatus" + i].InnerText))
                                                    {
                                                        if (!string.IsNullOrWhiteSpace(WoWhereCondition))
                                                        {
                                                            if (node["OrderStatus" + i].InnerText.ToLower().Equals("open"))
                                                            {
                                                                WoWhereCondition += "or OrderStatus in('UnSubmitted') ";
                                                                WoWhereCondition += "or OrderStatus in('Submitted') ";
                                                                WoWhereCondition += "or OrderStatus in('Approved') ";
                                                                WoWhereCondition += "or OrderStatus in('Transmitted') ";
                                                                WoWhereCondition += "or OrderStatus in('Incomplete') ";
                                                                WoWhereCondition += "or OrderStatus in('Past Due') ";
                                                                WoWhereCondition += "or OrderStatus in('Incomplete Past Due') ";
                                                            }
                                                            else if (node["OrderStatus" + i].InnerText.ToLower().Equals("close"))
                                                            {
                                                                WoWhereCondition += "or OrderStatus in('Closed') ";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (node["OrderStatus" + i].InnerText.ToLower().Equals("open"))
                                                            {
                                                                WoWhereCondition = " OrderStatus in('UnSubmitted') ";
                                                                WoWhereCondition += "or OrderStatus in('Submitted') ";
                                                                WoWhereCondition += "or OrderStatus in('Approved') ";
                                                                WoWhereCondition += "or OrderStatus in('Transmitted') ";
                                                                WoWhereCondition += "or OrderStatus in('Incomplete') ";
                                                                WoWhereCondition += "or OrderStatus in('Past Due') ";
                                                                WoWhereCondition += "or OrderStatus in('Incomplete Past Due') ";
                                                            }
                                                            else if (node["OrderStatus" + i].InnerText.ToLower().Equals("close"))
                                                            {
                                                                WoWhereCondition = " OrderStatus in('Closed') ";
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            dv.RowFilter = WoWhereCondition;
                                            dt = dv.ToTable();
                                        }
                                    }

                                }
                                if (objNotificationDTO != null &&
                                       (objNotificationDTO.ReportMasterDTO != null && objNotificationDTO.ReportMasterDTO.ModuleName != null && (objNotificationDTO.ReportMasterDTO.ModuleName.Trim().ToLower() == "workorder"))
                                       || (objNotificationDTO.AttachedReportMasterDTO != null && objNotificationDTO.AttachedReportMasterDTO.ModuleName != null && (objNotificationDTO.AttachedReportMasterDTO.ModuleName.Trim().ToLower() == "workorder"))
                                       )
                                {

                                    if (objNotificationDTO.XMLValue.ToLower().IndexOf("wostatus") >= 0)
                                    {

                                        DataView dv = new DataView(dt);
                                        XmlDocument xmldoc = new XmlDocument();
                                        xmldoc.LoadXml("<Data>" + objNotificationDTO.XMLValue + "</Data>");
                                        XmlNodeList nodeList = xmldoc.SelectNodes("/Data/WOStatus");
                                        string WoWhereCondition = string.Empty;
                                        if (nodeList != null && nodeList.Count > 0)
                                        {
                                            foreach (XmlNode node in nodeList)
                                            {
                                                for (int i = 1; i <= node.ChildNodes.Count; i++)
                                                {
                                                    if (!string.IsNullOrWhiteSpace(node["WOStatus" + i].InnerText))
                                                    {
                                                        if (!string.IsNullOrWhiteSpace(WoWhereCondition))
                                                        {
                                                            WoWhereCondition += "or WOStatus in('" + node["WOStatus" + i].InnerText + "') ";
                                                        }
                                                        else
                                                        {
                                                            WoWhereCondition = " WOStatus in('" + node["WOStatus" + i].InnerText + "') ";
                                                        }
                                                    }
                                                }
                                            }
                                            dv.RowFilter = WoWhereCondition;
                                            dt = dv.ToTable();

                                        }
                                        // query example = "id = 10"

                                    }

                                }
                                if ((objNotificationDTO.ReportMasterDTO != null && objNotificationDTO.ReportMasterDTO.ModuleName != null && (objNotificationDTO.ReportMasterDTO.ModuleName.Trim().ToLower() == "consume_pull"))
                                || (objNotificationDTO.AttachedReportMasterDTO != null && objNotificationDTO.AttachedReportMasterDTO.ModuleName != null && ((objNotificationDTO.AttachedReportMasterDTO.ModuleName.Trim().ToLower() == "consume_pull") || (objNotificationDTO.AttachedReportMasterDTO.ModuleName.Trim().ToLower() == "not consume_pull"))))
                                {
                                    if (objNotificationDTO.XMLValue.ToLower().IndexOf("quantitytype") >= 0)
                                    {
                                        DataView dv = new DataView(dt);
                                        string wherecondition = string.Empty;

                                        XmlDocument xmldoc = new XmlDocument();
                                        xmldoc.LoadXml("<Data>" + objNotificationDTO.XMLValue + "</Data>");
                                        XmlNodeList nodeList = xmldoc.SelectNodes("/Data/QuantityType");

                                        string WoWhereCondition = "";
                                        if (nodeList != null && nodeList.Count > 0)
                                        {
                                            foreach (XmlNode node in nodeList)
                                            {
                                                for (int i = 1; i <= node.ChildNodes.Count; i++)
                                                {
                                                    if (!string.IsNullOrWhiteSpace(node["Type" + i].InnerText))
                                                    {
                                                        if (node["Type" + i].InnerText == "1")
                                                        {
                                                            if (!string.IsNullOrWhiteSpace(WoWhereCondition))
                                                            {
                                                                WoWhereCondition += "or CustomerOwnedQuantity > 0 ";
                                                            }
                                                            else
                                                            {
                                                                WoWhereCondition += "CustomerOwnedQuantity > 0 ";
                                                            }
                                                        }
                                                        if (node["Type" + i].InnerText == "2")
                                                        {
                                                            if (!string.IsNullOrWhiteSpace(WoWhereCondition))
                                                            {
                                                                WoWhereCondition += " or ConsignedQuantity > 0 ";
                                                            }
                                                            else
                                                            {
                                                                WoWhereCondition += " ConsignedQuantity > 0 ";
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        if (!string.IsNullOrWhiteSpace(WoWhereCondition))
                                        {
                                            WoWhereCondition = " (" + WoWhereCondition + ")";
                                        }

                                        dv.RowFilter = wherecondition; // query example = "id = 10"
                                        dv.RowFilter = WoWhereCondition;
                                        dt = dv.ToTable();
                                    }

                                }
                            }
                        }
                        if (dt != null && dt.Rows.Count > 0)
                        {



                            ReportViewer1 = new ReportViewer();
                            ReportViewer1.Reset();
                            ReportViewer1.LocalReport.DataSources.Clear();

                            if (doc.Descendants(ns + "Subreport") != null && doc.Descendants(ns + "Subreport").Count() > 0)
                            {
                                hasSubReport = true;
                                if (objNotificationDTO.ReportMasterDTO.ParentID > 0)
                                {
                                    if (objNotificationDTO.ReportMasterDTO.ISEnterpriseReport.GetValueOrDefault(false))
                                    {
                                        rdlPath = CopyFiletoTempForAlert(ReportBasePath + @"\" + objEnterprise.ID + @"\EnterpriseReport" + @"\" + SubReportname, subGuid, ReportBasePath + @"\Temp");
                                    }
                                    else
                                    {
                                        rdlPath = CopyFiletoTempForAlert(ReportBasePath + @"\" + objEnterprise.ID + @"\" + objNotificationDTO.CompanyID + @"\" + SubReportname, subGuid, ReportBasePath + @"\Temp");
                                    }
                                }
                                else
                                {

                                    rdlPath = CopyFiletoTempForAlert(ReportBasePath + @"\" + objEnterprise.ID + @"\BaseReport" + @"\\" + SubReportname, subGuid, ReportBasePath + @"\Temp");
                                }
                                doc.Descendants(ns + "Tablix").Descendants(ns + "Subreport").FirstOrDefault().Attribute("Name").Value = Convert.ToString(subGuid);
                                doc.Descendants(ns + "Tablix").Descendants(ns + "ReportName").FirstOrDefault().Value = Convert.ToString(subGuid);
                                doc.Save(ReportPath);

                                docSub = XDocument.Load(rdlPath);
                                //lstSubTablix = docSub.Descendants(ns + "Tablix");

                                IEnumerable<XElement> lstSubTablixNew = docSub.Descendants(ns + "Tablix");
                                IEnumerable<XElement> lstUpdateSubTablixNew = UpdateResourceNew(lstSubTablixNew, SubReportResFile, strCulture, objEnterprise.ID, objNotificationDTO.CompanyID, objNotificationDTO.RoomID);

                                docSub.Save(rdlPath);


                                if (lstSubTablixNew != null && lstSubTablixNew.ToList().Count > 0)
                                {
                                    strSubTablix = lstSubTablixNew.ToList()[0].ToString();
                                }

                                docSub = amDAL.AddFormatToTaxbox(docSub, rsInfo);
                                docSub.Save(rdlPath);
                                docSub = XDocument.Load(rdlPath);

                                //lstUpdateSubTablix = UpdateResource(lstSubTablix, SubReportResFile, strCulture, objEnterprise.ID, objNotificationDTO.CompanyID);
                                //lstUpdateSubTablix = UpdateResourceNew(lstSubTablix, SubReportResFile, strCulture, objEnterprise.ID, objNotificationDTO.CompanyID, objNotificationDTO.RoomID);
                                //docSub.Save(rdlPath);

                                ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(LoadSubreport);

                                ReportViewer1.LocalReport.EnableExternalImages = true;
                                ReportViewer1.LocalReport.EnableHyperlinks = true;
                                ReportViewer1.LocalReport.Refresh();
                            }
                            if (!hasSubReport && rptPara.ContainsKey("SortFields") && !string.IsNullOrEmpty(rptPara["SortFields"]))
                            {
                                string SortFields = rptPara["SortFields"].TrimEnd(',');

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

                            if (objNotificationDTO.AttachmentTypes == "1")
                            {
                                //Generate byte for PDF File Only
                                Warning[] warnings;
                                string[] streamIds;
                                string mimeType = "application/pdf";
                                string encoding = "utf-8";
                                string extension = "pdf";
                                byte[] tempByte = null;
                                tempByte = ReportViewer1.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
                                eMailAttachmentDTO objAttachment = new eMailAttachmentDTO();
                                objAttachment.ID = 0;
                                objAttachment.AttachedFileName = Reportname + ".pdf";
                                objAttachment.eMailToSendID = 0;
                                objAttachment.MimeType = mimeType;
                                objAttachment.FileData = tempByte;
                                bytes.Add(objAttachment);
                                //bytes.Add(tempByte);
                            }
                            else if (objNotificationDTO.AttachmentTypes == "2")
                            {
                                //Generate byte for EXCEL file
                                Warning[] warnings2;
                                string[] streamIds2;
                                string mimeType2 = "application/vnd.ms-excel";
                                string encoding2 = "utf-8";
                                string extension2 = "xls";

                                byte[] tempByte = null;
                                tempByte = ReportViewer1.LocalReport.Render("Excel", null, out mimeType2, out encoding2, out extension2, out streamIds2, out warnings2);

                                eMailAttachmentDTO objAttachment = new eMailAttachmentDTO();
                                objAttachment.ID = 0;
                                objAttachment.AttachedFileName = Reportname + ".xls";
                                objAttachment.eMailToSendID = 0;
                                objAttachment.MimeType = mimeType2;
                                objAttachment.FileData = tempByte;
                                bytes.Add(objAttachment);
                            }
                            else if (objNotificationDTO.AttachmentTypes == "1,2")
                            {
                                //Generate Excel and PDF files bytes
                                Warning[] warnings;
                                string[] streamIds;
                                string mimeType = "application/pdf";
                                string encoding = "utf-8";
                                string extension = "pdf";
                                byte[] tempByte = null;
                                tempByte = ReportViewer1.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                                eMailAttachmentDTO objAttachment = new eMailAttachmentDTO();
                                objAttachment.ID = 1;
                                objAttachment.AttachedFileName = Reportname + ".pdf";
                                objAttachment.eMailToSendID = 0;
                                objAttachment.MimeType = mimeType;
                                objAttachment.FileData = tempByte;
                                bytes.Add(objAttachment);

                                warnings = null;
                                mimeType = "application/vnd.ms-excel";
                                streamIds = null;
                                extension = "xls";
                                tempByte = null;

                                tempByte = ReportViewer1.LocalReport.Render("Excel", null, out mimeType, out encoding, out extension, out streamIds, out warnings);


                                objAttachment = new eMailAttachmentDTO();
                                objAttachment.ID = 1;
                                objAttachment.AttachedFileName = Reportname + ".xls";
                                objAttachment.eMailToSendID = 0;
                                objAttachment.MimeType = mimeType;
                                objAttachment.FileData = tempByte;
                                bytes.Add(objAttachment);
                            }
                        }

                    }


                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {

                objReportMailLogDTO = null;
                objDAL = null;
                doc = null;
                lstTablix = null;
                lstUpdateTablix = null;
                lstReportPara = null;
                rpt = null;
                if (myConnection != null)
                    myConnection.Dispose();

                if (cmd != null)
                    cmd.Dispose();
                if (sqla != null)
                    sqla.Dispose();

                if (dt != null)
                    dt.Dispose();

                if (sqla != null)
                    sqla.Dispose();

                sqla = null;
                myConnection = null;
                cmd = null;
                dt = null;
                lstQueryPara = null;
                slpar = null;
                objReportPara = null;
                docSub = null;

                rds = null;
                if (ReportViewer1 != null)
                {
                    ReportViewer1.Dispose();
                }

            }
            return bytes;
        }

        private string CopyFiletoTempForAlert(string strfile, string reportname, string ReportTempPath)
        {

            string ReportRetPath = string.Empty;

            if (!System.IO.Directory.Exists(ReportTempPath))
            {
                System.IO.Directory.CreateDirectory(ReportTempPath);
            }
            ReportRetPath = ReportTempPath + @"\" + reportname + ".rdlc";
            System.IO.File.Copy(strfile, ReportRetPath);
            return ReportRetPath;

        }

        //created new function to get information for resoures file(resolved WI-1123 )
        private string GetResourceFileFullPath(string fileName, string Culture, Int64 EntID, Int64 CompanyID)
        {
            string path = ResourceBaseFilePath;
            path += "\\" + EntID + "\\" + CompanyID + "\\" + fileName;
            if (Culture != "en-US")
            {
                path += "." + Culture;
            }
            path += ".resx";

            return path;
        }

        private string GetResourceFileFullPathForUDF(string fileName, string Culture, Int64 EntID, Int64 CompanyID, Int64 RoomID)
        {
            string path = ResourceBaseFilePath;
            path += "\\" + EntID + "\\" + CompanyID + "\\" + RoomID + "\\" + fileName;
            if (Culture != "en-US")
            {
                path += "." + Culture;
            }
            path += ".resx";

            return path;
        }

        private IEnumerable<XElement> UpdateResource(IEnumerable<XElement> lstTablix, string ResFile, string strCulture, Int64 EntID, Int64 CompanyID)
        {
            //make changes to resolved WI-1123
            string ResourceFilePath = GetResourceFileFullPath(ResFile, strCulture, EntID, CompanyID);
            IEnumerable<XElement> lstTableCell = null;
            try
            {
                foreach (XElement Table in lstTablix)
                {
                    lstTableCell = Table.Descendants(ns + "TablixCell");
                    foreach (XElement Cell in lstTableCell)
                    {
                        if (Cell.Descendants(ns + "Value").Any())
                        {
                            //Cell.Descendants(ns + "Value").FirstOrDefault().Value = GetResourceValue(ResFile, Cell.Descendants(ns + "Value").FirstOrDefault().Value);
                            Cell.Descendants(ns + "Value").FirstOrDefault().Value = GetResourceValue(ResourceFilePath, Cell.Descendants(ns + "Value").FirstOrDefault().Value);
                        }
                    }
                }

                return lstTablix;
            }
            finally
            {
                lstTableCell = null;
            }
        }

        private IEnumerable<XElement> UpdateResourceNew(IEnumerable<XElement> lstTablix, string ResFile, string strCulture, Int64 EntID, Int64 CompanyID, Int64 RoomID)
        {
            //make changes to resolved WI-1123
            string ResourceFilePath = GetResourceFileFullPath(ResFile, strCulture, EntID, CompanyID);
            string ResourceFilePathForUDF = GetResourceFileFullPathForUDF(ResFile, strCulture, EntID, CompanyID, RoomID);
            IEnumerable<XElement> lstTableCell = null;
            try
            {
                foreach (XElement Table in lstTablix)
                {
                    lstTableCell = Table.Descendants(ns + "TablixCell");
                    foreach (XElement Cell in lstTableCell)
                    {
                        if (Cell.Descendants(ns + "Value").Any())
                        {
                            //Cell.Descendants(ns + "Value").FirstOrDefault().Value = GetResourceValue(ResFile, Cell.Descendants(ns + "Value").FirstOrDefault().Value);
                            //Cell.Descendants(ns + "Value").FirstOrDefault().Value = GetResourceValue(ResourceFilePath, Cell.Descendants(ns + "Value").FirstOrDefault().Value);
                            Cell.Descendants(ns + "Value").FirstOrDefault().Value = GetResourceValueNew(ResourceFilePath, Cell.Descendants(ns + "Value").FirstOrDefault().Value, ResourceFilePathForUDF, EntID, CompanyID, RoomID, ResFile, strCulture);
                        }
                    }
                }

                return lstTablix;
            }
            finally
            {
                lstTableCell = null;
            }
        }

        private Dictionary<string, string> SetGetPDFReportParaDictionary(EnterpriseDTO EntDTO, long CompanyID, long RoomID, ReportMailLogDTO objReportMailLogDTO, NotificationDTO objNotificationDTO, Dictionary<string, string> Params)
        {
            Dictionary<string, string> dictionary = null;
            try
            {

                dictionary = new Dictionary<string, string>();
                DateTime startDate = DateTime.MinValue;
                DateTime EndDate = DateTime.MinValue;
                CompanyMasterDTO objCompDTO = new CompanyMasterDAL(EntDTO.EnterpriseDBName).GetCompanyByID(CompanyID);

                //For Room Report and company report, RoomId will not added
                List<ReportBuilderDTO> lstReportList = new ReportMasterDAL(EntDTO.EnterpriseDBName).GetReportList();
                bool AddRoomIDFilter = true;

                if (objNotificationDTO != null && objNotificationDTO.ReportName.Trim().ToLower() == "room")
                {
                    AddRoomIDFilter = false;
                }

                if (objNotificationDTO != null && objNotificationDTO.ReportName.Trim().ToLower() == "company")
                {
                    AddRoomIDFilter = false;
                }
                if (lstReportList.ToList().Where(r => r.ReportName.Trim().ToLower() == "room").Any() && AddRoomIDFilter)
                {
                    ReportBuilderDTO objRoomReportDTO = lstReportList.ToList().Where(r => r.ReportName.Trim().ToLower() == "room").FirstOrDefault();
                    ReportBuilderDTO CurrentReport = lstReportList.ToList().Where(r => r.ReportName.Trim().ToLower() == objNotificationDTO.ReportName.Trim().ToLower()).FirstOrDefault();
                    if (CurrentReport != null && objRoomReportDTO != null && CurrentReport.ParentID == objRoomReportDTO.ID)
                    {
                        AddRoomIDFilter = false;
                    }
                }
                if (lstReportList.ToList().Where(r => r.ReportName.Trim().ToLower() == "company").Any() && AddRoomIDFilter)
                {
                    ReportBuilderDTO objRoomReportDTO = lstReportList.ToList().Where(r => r.ReportName.Trim().ToLower() == "company").FirstOrDefault();
                    ReportBuilderDTO CurrentReport = lstReportList.ToList().Where(r => r.ReportName.Trim().ToLower() == objNotificationDTO.ReportName.Trim().ToLower()).FirstOrDefault();
                    if (CurrentReport != null && objRoomReportDTO != null && CurrentReport.ParentID == objRoomReportDTO.ID)
                    {
                        AddRoomIDFilter = false;
                    }
                }


                dictionary.Add("ConnectionString", DbConnectionHelper.GeteTurnsSQLConnectionString(EntDTO.EnterpriseDBName, DbConnectionType.GeneralReadOnly.ToString("F")));

                if (objNotificationDTO != null && AddRoomIDFilter)
                {
                    //dictionary.Add("CompanyIDs", Convert.ToString(CompanyID));
                    //dictionary.Add("RoomIDs", Convert.ToString(RoomID));
                    dictionary.Add("CompanyIDs", Convert.ToString(objNotificationDTO.CompanyIds));
                    dictionary.Add("RoomIDs", Convert.ToString(objNotificationDTO.RoomIds));
                }


                CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                if (objCommonDAL.HasSpecialDomain(SessionHelper.CurrentDomainURL,EntDTO.ID))
                {
                    dictionary.Add("eTurnsLogoURL", System.Configuration.ConfigurationManager.AppSettings["LocalHostName"] + ConvertImageToPNG(eTurnsAppConfig.BaseFileSharedPath, "Uploads\\EnterpriseLogos\\" + EntDTO.ID + "\\" + EntDTO.EnterpriseLogo));
                    dictionary.Add("EnterpriseLogoURL", System.Configuration.ConfigurationManager.AppSettings["LocalHostName"] + "/Content/OpenAccess/NologoReport.png");
                }
                else
                {
                    dictionary.Add("eTurnsLogoURL", System.Configuration.ConfigurationManager.AppSettings["LocalHostName"] + "/Content/OpenAccess/logoInReport.png");
                    dictionary.Add("EnterpriseLogoURL", System.Configuration.ConfigurationManager.AppSettings["LocalHostName"] + ConvertImageToPNG(eTurnsAppConfig.BaseFileSharedPath, "Uploads\\EnterpriseLogos\\" + EntDTO.ID + "\\" + EntDTO.EnterpriseLogo));
                }
                dictionary.Add("CompanyLogoURL", System.Configuration.ConfigurationManager.AppSettings["LocalHostName"] + ConvertImageToPNG(eTurnsAppConfig.BaseFileSharedPath, "Uploads\\CompanyLogos\\" + CompanyID + "\\" + objCompDTO.CompanyLogo));

                dictionary.Add("UserID", null);
                if (Params != null && Params.Keys.Contains("DataGuids"))
                {
                    dictionary.Add("DataGuids", Params["DataGuids"]);
                }
                dictionary.Add("BarcodeURL", System.Configuration.ConfigurationManager.AppSettings["LocalHostName"] + "/Barcode/GetBarcodeByKey?barcodekey=");
                if (objNotificationDTO != null && objNotificationDTO.ReportDataSelectionType == 2)
                {
                    startDate = DateTime.UtcNow.AddDays(-(Convert.ToDouble(objNotificationDTO.ReportDataSince)));
                    EndDate = objNotificationDTO.NextRunDate ?? DateTime.UtcNow;
                    dictionary.Add("StartDate", startDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    dictionary.Add("EndDate", EndDate.ToString("yyyy-MM-dd  HH:mm:ss"));
                    dictionary.Add("OrigStartDate", DateTimeUtility.ConvertDateFromUTC(objNotificationDTO.objeTurnsRegionInfo.TimeZoneName, startDate).ToString("yyyy-MM-dd  HH:mm:ss"));
                    dictionary.Add("OrigEndDate", DateTimeUtility.ConvertDateFromUTC(objNotificationDTO.objeTurnsRegionInfo.TimeZoneName, EndDate).ToString("yyyy-MM-dd  HH:mm:ss"));

                }
                else if (objNotificationDTO != null && objNotificationDTO.ReportDataSelectionType == 1)
                {
                    if (objReportMailLogDTO != null)
                    {
                        startDate = objReportMailLogDTO.SendDate;
                        EndDate = (objNotificationDTO.NextRunDate ?? DateTime.UtcNow).AddSeconds(-1);

                        dictionary.Add("StartDate", startDate.ToString("yyyy-MM-dd HH:mm:ss"));
                        dictionary.Add("EndDate", EndDate.ToString("yyyy-MM-dd  HH:mm:ss"));
                        dictionary.Add("OrigStartDate", DateTimeUtility.ConvertDateFromUTC(objNotificationDTO.objeTurnsRegionInfo.TimeZoneName, startDate).ToString("yyyy-MM-dd  HH:mm:ss"));
                        dictionary.Add("OrigEndDate", DateTimeUtility.ConvertDateFromUTC(objNotificationDTO.objeTurnsRegionInfo.TimeZoneName, EndDate).ToString("yyyy-MM-dd  HH:mm:ss"));

                    }
                    else
                    {
                        dictionary.Add("StartDate", null);
                        dictionary.Add("EndDate", null);
                    }

                }
                else if (objNotificationDTO != null && objNotificationDTO.ReportDataSelectionType == 3)
                {

                    DateTime current = (DateTime.UtcNow);
                    startDate = new DateTime(current.Year, current.Month, 1);
                    EndDate = (objNotificationDTO.NextRunDate ?? DateTime.UtcNow).AddSeconds(-1);

                    dictionary.Add("StartDate", startDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    dictionary.Add("EndDate", EndDate.ToString("yyyy-MM-dd  HH:mm:ss"));
                    dictionary.Add("OrigStartDate", DateTimeUtility.ConvertDateFromUTC(objNotificationDTO.objeTurnsRegionInfo.TimeZoneName, startDate).ToString("yyyy-MM-dd  HH:mm:ss"));
                    dictionary.Add("OrigEndDate", DateTimeUtility.ConvertDateFromUTC(objNotificationDTO.objeTurnsRegionInfo.TimeZoneName, EndDate).ToString("yyyy-MM-dd  HH:mm:ss"));

                }

                if (objNotificationDTO != null)
                {
                    if (objNotificationDTO.ReportMasterDTO != null)
                    {
                        if (!string.IsNullOrEmpty(objNotificationDTO.ReportMasterDTO.SortColumns))
                        {
                            dictionary.Add("SortFields", objNotificationDTO.ReportMasterDTO.SortColumns.TrimEnd(','));
                        }
                    }
                }
                if (!string.IsNullOrEmpty(objNotificationDTO.SupplierIds) && AddRoomIDFilter)
                    dictionary.Add("SupplierIDs", objNotificationDTO.SupplierIds);
                else
                    dictionary.Add("SupplierIDs", string.Empty);
                if ((objNotificationDTO != null && objNotificationDTO.ReportMasterDTO != null && objNotificationDTO.ReportMasterDTO.ModuleName != null && (objNotificationDTO.ReportMasterDTO.ModuleName.Trim().ToLower() == "expiringitems"))
               || (objNotificationDTO != null && objNotificationDTO.AttachedReportMasterDTO != null && objNotificationDTO.AttachedReportMasterDTO.ModuleName != null && (objNotificationDTO.AttachedReportMasterDTO.ModuleName.Trim().ToLower() == "expiringitems")))
                {
                    string RequisitionStatus = string.Empty;
                    string DaysUntilItemExpires = string.Empty;
                    string DaysToApproveOrder = string.Empty;
                    string ProjectExpirationDate = string.Empty;

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
                    dictionary.Add("OnlyExirationItems", RequisitionStatus);
                    if (string.IsNullOrWhiteSpace(ProjectExpirationDate))
                    {
                        dictionary.Add("DaysToApproveOrder", DaysUntilItemExpires);
                        dictionary.Add("DaysUntilItemExpires", DaysToApproveOrder);
                    }
                    else
                    {
                        dictionary.Add("DaysToApproveOrder", string.Empty);
                        dictionary.Add("DaysUntilItemExpires", string.Empty);

                    }
                    dictionary.Add("ProjectedExpirationDate", ProjectExpirationDate);
                    if (!string.IsNullOrWhiteSpace(ProjectExpirationDate))
                    {
                        objNotificationDTO.ProjectExpirationDate = FnCommon.ConvertDateByTimeZone(Convert.ToDateTime(ProjectExpirationDate), true).Split(' ')[0];// ProjectExpirationDate.ToString(SessionHelper.RoomDateFormat);
                    }
                    // objNotificationDTO.ProjectExpirationDate = ProjectExpirationDate;

                }
                else if ((objNotificationDTO != null && objNotificationDTO.ReportMasterDTO != null && objNotificationDTO.ReportMasterDTO.ModuleName != null && (objNotificationDTO.ReportMasterDTO.ModuleName.Trim().ToLower() == "tool"))
               || (objNotificationDTO != null && objNotificationDTO.AttachedReportMasterDTO != null && objNotificationDTO.AttachedReportMasterDTO.ModuleName != null && (objNotificationDTO.AttachedReportMasterDTO.ModuleName.Trim().ToLower() == "tool")))
                {
                    XDocument xDoc = XDocument.Parse("<Data>" + objNotificationDTO.XMLValue + "</Data>");
                    if (xDoc.Descendants("OnlyAvailableTools").FirstOrDefault() != null && xDoc.Descendants("OnlyAvailableTools").FirstOrDefault().Value == "Yes")
                        dictionary.Add("@OnlyAvailable", "HasQty");
                }
                return dictionary;
            }
            finally
            {
                dictionary = null;
            }

        }

        private string GetResourceValue(string ResourceFilePath, string Key)
        {
            string KeyVal = string.Empty;
            KeyVal = ResourceHelper.GetResourceValueByKeyAndFullFilePath(Key, ResourceFilePath);
            //KeyVal = ResourceHelper.GetResourceValue(Key, FileName);
            return KeyVal;
        }

        private string GetResourceValueNew(string ResourceFilePath, string Key, string ResourceFilePathForUDF = "", Int64 EntID = 0, Int64 CompanyID = 0, Int64 RoomID = 0, string ResFile = "", string strCulture = "")
        {
            string KeyVal = string.Empty;
            if (Key.Trim().ToLower().Contains("udf"))
            {
                //KeyVal = ResourceHelper.GetResourceValueByKeyAndFullFilePath(Key, ResourceFilePathForUDF);
                KeyVal = ResourceRead.GetResourceValueByKeyAndFullFilePath(Key, ResourceFilePathForUDF, EntID, CompanyID, RoomID, ResFile, strCulture);
            }
            else
            {
                //KeyVal = ResourceHelper.GetResourceValueByKeyAndFullFilePath(Key, ResourceFilePath);
                KeyVal = ResourceRead.GetResourceValueByKeyAndFullFilePath(Key, ResourceFilePath, EntID, CompanyID, RoomID, ResFile, strCulture);
            }
            //KeyVal = ResourceHelper.GetResourceValue(Key, FileName);
            return KeyVal;
        }

        protected void LoadSubreport(object sender, SubreportProcessingEventArgs e)
        {
            XDocument doc = null;
            SqlConnection myConnection = null;
            SqlCommand cmd = null;
            SqlDataAdapter sqla = null;
            DataTable dt = null;
            IEnumerable<XElement> lstReportPara = null;
            List<ReportParameter> rpt = null;
            ReportParameter rpara = null;
            IEnumerable<XElement> lstQueryPara = null;
            SqlParameter slpar = null;
            XElement objReportPara = null;
            ReportDataSource rds = null;

            try
            {
                string rdlPath = string.Empty;
                rdlPath = ReportBasePath + @"/Temp" + @"\\" + e.ReportPath + ".rdlc";
                doc = XDocument.Load(rdlPath);

                string connString = doc.Descendants(ns + "ConnectString").FirstOrDefault().Value;
                myConnection = new SqlConnection();
                cmd = new SqlCommand();
                sqla = new SqlDataAdapter();
                dt = new DataTable();
                myConnection.ConnectionString = DBconectstring;

                cmd.Connection = myConnection;
                cmd.CommandText = doc.Descendants(ns + "CommandText").FirstOrDefault().Value;
                cmd.CommandType = CommandType.Text;
                if (doc.Descendants(ns + "CommandType").FirstOrDefault() != null)
                {
                    cmd.CommandType = (CommandType)Enum.Parse(typeof(CommandType), doc.Descendants(ns + "CommandType").FirstOrDefault().Value == null ? "Text" : doc.Descendants(ns + "CommandType").FirstOrDefault().Value, true);
                }

                lstReportPara = doc.Descendants(ns + "ReportParameter");
                rpt = new List<ReportParameter>();

                if (lstReportPara != null && lstReportPara.Count() > 0)
                {
                    foreach (var item in lstReportPara)
                    {
                        rpara = new ReportParameter();
                        rpara.Name = item.Attribute("Name").Value;
                        rpara.Values.Add(e.Parameters[rpara.Name].Values[0]);
                        rpt.Add(rpara);
                    }
                }

                lstQueryPara = doc.Descendants(ns + "QueryParameter");

                if (lstQueryPara != null && lstQueryPara.Count() > 0)
                {
                    foreach (var item in lstQueryPara)
                    {
                        slpar = new SqlParameter();
                        slpar.ParameterName = item.Attribute("Name").Value;//
                        slpar.Value = e.Parameters[slpar.ParameterName.Replace("@", "")].Values[0];
                        objReportPara = lstReportPara.FirstOrDefault(x => x.Attribute("Name").Value == slpar.ParameterName.Replace("@", ""));
                        if (objReportPara.Descendants(ns + "DataType") != null && objReportPara.Descendants(ns + "DataType").Count() > 0)
                        {
                            slpar.DbType = (DbType)Enum.Parse(typeof(DbType), objReportPara.Descendants(ns + "DataType").FirstOrDefault().Value, true);
                        }
                        if (cmd.Parameters.IndexOf(slpar.ParameterName) < 0)
                            cmd.Parameters.Add(slpar);
                    }
                }
                sqla = new SqlDataAdapter(cmd);
                cmd.CommandTimeout = 600;
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
                rds = new ReportDataSource();
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
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                doc = null;
                lstReportPara = null;
                rpt = null;
                rpara = null;
                lstQueryPara = null;
                objReportPara = null;
                rds = null;
                slpar = null;

                if (myConnection != null)
                    myConnection.Dispose();

                if (cmd != null)
                    cmd.Dispose();
                if (sqla != null)
                    sqla.Dispose();

                if (dt != null)
                    dt.Dispose();

                myConnection = null;
                cmd = null;
                sqla = null;
                dt = null;



            }


        }

        private void CreateReportMail(List<eMailAttachmentDTO> objeMailAttchList, string MailSubject, string MessageBody, string ToEmailAddress, NotificationDTO objRoomScheduleDetailDTO, EnterpriseDTO objEnterpriseDTO)
        {
            eMailMasterDAL objUtils = null;
            try
            {
                //string StrSubject = Reportname + " Report.";
                //if (string.IsNullOrEmpty(ToEmailAddress))
                //    return;
                //ToEmailAddress = "niraj_dave@semaphore-software.com";

                string strCCAddress = "";

                if ((objeMailAttchList != null && objeMailAttchList.Count() > 0) || objRoomScheduleDetailDTO.SendEmptyEmail)
                {
                    objUtils = new eMailMasterDAL(SessionHelper.EnterPriseDBName);
                    eTurnsRegionInfo objeTurnsRegionInfo = null;
                    if (objRoomScheduleDetailDTO != null)
                    {
                        RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(objEnterpriseDTO.EnterpriseDBName);
                        objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(objRoomScheduleDetailDTO.RoomID, objRoomScheduleDetailDTO.CompanyID, -1);

                        string DateTimeFormat = "MM/dd/yyyy";
                        DateTime TZDateTimeNow = DateTime.UtcNow;
                        if (objeTurnsRegionInfo != null)
                        {
                            DateTimeFormat = objeTurnsRegionInfo.ShortDatePattern;// + " " + objeTurnsRegionInfo.ShortTimePattern;
                            TZDateTimeNow = objeTurnsRegionInfo.TZDateTimeNow ?? DateTime.UtcNow;
                        }
                        if (MailSubject != null)
                        {
                            string CurrentDate = TZDateTimeNow.ToString(DateTimeFormat);
                            // EmailSubject = EmailSubject.Replace("@@DATE@@", CurrentDate).Replace("@@date@@", CurrentDate).Replace("@@Date@@", CurrentDate);
                            MailSubject = Regex.Replace(MailSubject, "@@DATE@@", CurrentDate, RegexOptions.IgnoreCase);
                            if (!string.IsNullOrWhiteSpace(objRoomScheduleDetailDTO.CompanyName))
                            {
                                MailSubject = Regex.Replace(MailSubject, "@@COMPANYNAME@@", objRoomScheduleDetailDTO.CompanyName, RegexOptions.IgnoreCase);
                            }
                            if (!string.IsNullOrWhiteSpace(objRoomScheduleDetailDTO.RoomName))
                            {
                                MailSubject = Regex.Replace(MailSubject, "@@ROOMNAME@@", objRoomScheduleDetailDTO.RoomName, RegexOptions.IgnoreCase);
                            }

                            MailSubject = Regex.Replace(MailSubject, "@@Year@@", Convert.ToString(DateTime.UtcNow.Year), RegexOptions.IgnoreCase);
                        }

                        if (MessageBody != null)
                        {
                            string CurrentDate = TZDateTimeNow.ToString(DateTimeFormat);
                            //EmailBody = EmailBody.Replace("@@DATE@@", CurrentDate).Replace("@@date@@", CurrentDate).Replace("@@Date@@", CurrentDate);
                            MessageBody = Regex.Replace(MessageBody, "@@DATE@@", CurrentDate, RegexOptions.IgnoreCase);
                            if (!string.IsNullOrWhiteSpace(objRoomScheduleDetailDTO.CompanyName))
                            {
                                MessageBody = Regex.Replace(MessageBody, "@@COMPANYNAME@@", objRoomScheduleDetailDTO.CompanyName, RegexOptions.IgnoreCase);
                            }
                            if (!string.IsNullOrWhiteSpace(objRoomScheduleDetailDTO.RoomName))
                            {
                                MessageBody = Regex.Replace(MessageBody, "@@ROOMNAME@@", objRoomScheduleDetailDTO.RoomName, RegexOptions.IgnoreCase);
                            }
                        }
                        MessageBody = Regex.Replace(MessageBody, "@@Year@@", Convert.ToString(DateTime.UtcNow.Year), RegexOptions.IgnoreCase);

                    }

                    objUtils.eMailToSend(ToEmailAddress, strCCAddress, MailSubject, MessageBody, objEnterpriseDTO.ID, objRoomScheduleDetailDTO.CompanyID, objRoomScheduleDetailDTO.RoomID, 0, objeMailAttchList);
                }
                AddToReportMailLog(objEnterpriseDTO, objRoomScheduleDetailDTO, ToEmailAddress, objeMailAttchList);
            }
            finally
            {
                objUtils = null;

            }



        }

        public void AddToReportMailLog(EnterpriseDTO objEnterpriseDTO, NotificationDTO objRoomScheduleDetailDTO, string ToEmailAddress, List<eMailAttachmentDTO> objeMailAttchList)
        {
            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(objEnterpriseDTO.EnterpriseDBName);
            ReportMailLogDTO objReportMailLogDTO = new ReportMailLogDTO();
            try
            {

                objReportMailLogDTO.Id = 0;
                objReportMailLogDTO.ReportID = objRoomScheduleDetailDTO.ReportID ?? 0;
                objReportMailLogDTO.ScheduleID = objRoomScheduleDetailDTO.RoomScheduleID;
                objReportMailLogDTO.CompanyID = objRoomScheduleDetailDTO.CompanyID;
                objReportMailLogDTO.RoomID = objRoomScheduleDetailDTO.RoomID;
                objReportMailLogDTO.SendDate = objRoomScheduleDetailDTO.NextRunDate ?? DateTime.UtcNow;
                objReportMailLogDTO.SendEmailAddress = ToEmailAddress;
                objReportMailLogDTO.NotificationID = objRoomScheduleDetailDTO.ID;
                if (objeMailAttchList != null)
                {
                    objReportMailLogDTO.AttachmentCount = objeMailAttchList.Count;
                }
                objReportMasterDAL.InsertMailLog(objReportMailLogDTO);
            }
            finally
            {
                objReportMasterDAL = null;
                objReportMailLogDTO = null;
            }

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
                            return InnerPath;
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
                    string NoImagePath = BasePath + "\\Uploads\\EnterpriseLogos\\";
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

        //public void SaveAttachMentsTosFTP(NotificationDTO objNotificationDTO, EnterpriseDTO objEnterpriseDTO, List<eMailAttachmentDTO> lstAttachments)
        //{
        //    string FTPReportSavePath = Convert.ToString(ConfigurationManager.AppSettings["FTPReportSavePath"]);
        //    List<string> ZipFilePaths = SFTPHelper.AddFileToTemplLocation(objNotificationDTO, objEnterpriseDTO, lstAttachments);
        //    FTPMasterDTO objFTP = objNotificationDTO.FtpDetails;
        //    if (ZipFilePaths != null && ZipFilePaths.Count > 0)
        //    {
        //        foreach (string strpath in ZipFilePaths)
        //        {
        //            SFTPHelper.UploadFileToSFTPServer(objFTP.ServerAddress, objFTP.Port, objFTP.UserName, objFTP.Password, FTPReportSavePath, strpath);
        //        }
        //    }
        //}

        private string NewEmailBody(DataTable dt, string EmailBody, XDocument doc, string ReportResFile)
        {
            string UpdatedEmailbody = string.Empty;
            string headerSequence = string.Empty;
            try
            {
                bool hasTotalField = true;
                List<XElement> lstColumns = doc.Descendants(ns + "TablixColumn").ToList();
                string DatasetFields = string.Empty;
                List<XElement> lstReportField = doc.Descendants(ns + "TablixRow").ToList();
                List<XElement> lstFieldBottom = lstReportField[1].Descendants(ns + "TablixCell").ToList();
                List<XElement> lstDSFields = doc.Descendants(ns + "Field").ToList();
                List<XElement> lstTablixRowGrouping = doc.Descendants(ns + "Tablix").Descendants(ns + "TablixRowHierarchy").Descendants(ns + "GroupExpression").ToList();
                List<XElement> lstGroupCSS = doc.Descendants(ns + "Tablix").Descendants(ns + "TablixRowHierarchy").Descendants(ns + "TablixMember").ToList()[0].Descendants(ns + "TablixHeader").ToList();
                List<KeyValSelectDTO> lstGrouplist = new List<KeyValSelectDTO>();
                List<KeyValSelectDTO> lstRemainingGrlist = new List<KeyValSelectDTO>();
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
                        IEnuDSFields = IEnuDSFields.Where(x => x.Descendants(nsrd + "TypeName").FirstOrDefault() != null && !(x.Descendants(nsrd + "TypeName").FirstOrDefault().Value.Contains("System.Int64"))).ToList();

                    if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                        IEnuDSFields = IEnuDSFields.Where(x => x.Descendants(nsrd + "TypeName").FirstOrDefault() != null && !(x.Descendants(nsrd + "TypeName").FirstOrDefault().Value.Contains("System.Guid"))).ToList();

                    if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                    {
                        lstDSFields.Remove();
                        lstDSFields = IEnuDSFields.OrderBy(e => e.Attribute("Name").Value).ToList();
                    }
                }
                #endregion

                if (lstReportField != null && lstReportField.Count > 0)
                {

                    List<XElement> lstFieldTop = lstReportField[0].Descendants(ns + "TablixCell").ToList();
                    lstFieldBottom = lstReportField[1].Descendants(ns + "TablixCell").ToList();
                    int cellcount = lstFieldBottom.Count;
                    if (!hasTotalField)
                    {
                        cellcount = cellcount - 1;
                    }
                    string tablixcelltop = string.Empty;
                    string tablixcellBottom = string.Empty;
                    int rowGroupCount = lstTablixRowGrouping.Count;
                    int tdcount = 0;
                    UpdatedEmailbody += "<table style='margin-left: 0px; width: 99%;'  border='1' cellpadding='0' cellspacing ='0' > ";
                    UpdatedEmailbody += "<thead><tr role='row'>";
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
                        string Header = GetField(tablixcellBottom, ReportResFile);
                        if (!string.IsNullOrWhiteSpace(headerSequence))
                        {
                            //  headerSequence += "," + tablixcellBottom.Replace("=Parameters!BarcodeURL.Value+Fields!", "").Replace(".Value", "");
                        }
                        else
                        {
                            // headerSequence = tablixcellBottom.Replace("=Parameters!BarcodeURL.Value+Fields!", "").Replace(".Value", "");
                        }
                        // UpdatedEmailbody += "<td id='tddropline_" + tdcount + "'><span class='ChangeText'  " + GetSpanStyle(fontstyle, fontweight) + ">" + Header + "</span></td>";
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
                        string Header = GetField(tablixcelltop, ReportResFile);
                        if (!string.IsNullOrWhiteSpace(headerSequence))
                        {
                            headerSequence += "," + tablixcellBottom.Replace("=Parameters!BarcodeURL.Value+Fields!", "").Replace(".Value", "");
                        }
                        else
                        {
                            headerSequence = tablixcellBottom.Replace("=Parameters!BarcodeURL.Value+Fields!", "").Replace(".Value", "");
                        }
                        UpdatedEmailbody += "<td id='tddropline_" + tdcount + "' style='text-align: left;' ><span class='ChangeText' " + GetSpanStyle(fontstyle, fontweight) + " >" + Header + "</span></td>";
                        tdcount += 1;
                        //if (lstReportGroupMasterDTO.Any(x => x.FieldName == tablixcellBottom))

                        DataColumnCollection columns = dt.Columns;
                        //if (lstReportGroupMasterDTO.Any(x => x.FieldName == DatasetFields))
                        if (columns.Contains(DatasetFields))
                        {
                            KeyValSelectDTO objKeyValDTO = new KeyValSelectDTO();

                        }
                        else
                        {
                            KeyValSelectDTO objKeyValDTO = new KeyValSelectDTO();
                            objKeyValDTO.key = tablixcellBottom;
                            objKeyValDTO.value = GetField(tablixcellBottom, ReportResFile);
                            objKeyValDTO.IsSelect = false;
                            if (lstGrouplist.FindIndex(x => x.key == objKeyValDTO.key && x.value == objKeyValDTO.value) < 0)
                                lstRemainingGrlist.Add(objKeyValDTO);
                        }
                    }
                    UpdatedEmailbody += "</tr></thead>";
                    UpdatedEmailbody += "<tbody>";
                    string AlterNativeRowStyle = "Style='background:#DBD9D9;'";
                    for (int cnt = 0; cnt < dt.Rows.Count; cnt++)
                    {
                        string RowStyle = string.Empty;
                        if (cnt % 2 == 0)
                        {
                            RowStyle = AlterNativeRowStyle;
                        }
                        else
                        {

                        }
                        UpdatedEmailbody += @"<tr " + RowStyle + @" >";
                        string[] Columns = headerSequence.Split(',');
                        foreach (string column in Columns)
                        {
                            string Value = Convert.ToString(dt.Rows[cnt][column]);
                            UpdatedEmailbody += " <td>" + Value + "</td>";
                        }
                        UpdatedEmailbody += "</tr>";
                    }
                    if (dt.Rows.Count == 0)
                    {
                        string[] Columns = headerSequence.Split(',');
                        string Str = "No Data found";
                        string RowStyle = string.Empty;
                        UpdatedEmailbody += @"<tr " + RowStyle + @" >
                        <td colspan=" + Columns.Length + @">
                            " + Str + @"
                        </td>
                    </tr>";
                    }
                    UpdatedEmailbody += "</tbody>";
                    UpdatedEmailbody += "</table>";

                }
                EmailBody = EmailBody.Replace("@@table@@", UpdatedEmailbody).Replace("@@TABLE@@", UpdatedEmailbody).Replace("@@Table@@", UpdatedEmailbody);
            }
            catch (Exception)
            {
                return EmailBody;
            }
            return EmailBody;
        }
        public string GetField(string Key, string FileName)
        {
            try
            {
                string KeyVal = string.Empty;

                if (Key.ToLower().Contains("udf"))
                {
                    //KeyVal = ResourceHelper.GetResourceValue(Key, FileName, true);
                    KeyVal = ResourceRead.GetResourceValue(Key, FileName, true);
                }
                else
                {
                    //KeyVal = ResourceHelper.GetResourceValue(Key, FileName);
                    KeyVal = ResourceRead.GetResourceValue(Key, FileName);
                }
                return KeyVal;
            }
            catch (Exception)
            {
                return Key;
            }

        }
        public string GetSpanStyle(string fontstyle, string fontweight)
        {
            try
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
            catch
            {
                return "style=''";
            }
        }
        public string GetTDStyleWithWidth(string TextAlign, string width, bool IsVisible = true)
        {
            try
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
            catch
            {
                return "style=''";
            }
        }

        public string GetTDStyle(string TextAlign)
        {
            try
            {
                string retStyle = string.Empty;
                retStyle = "style='";
                if (!string.IsNullOrEmpty(TextAlign))
                {
                    retStyle += "text-align:" + TextAlign + "; ";
                }

                retStyle += "'";
                return retStyle;
            }
            catch
            {
                return "style=''";
            }
        }
        public string ConvertInchToPx(string inch)
        {
            try
            {
                Int64 retPx = 0;
                double inchdb = Convert.ToDouble(inch.ToLower().Replace("in", "").Replace("px", ""));
                retPx = Convert.ToInt64(Math.Round((inchdb * 75), 2));
                return (Convert.ToString(retPx) + "px");
            }
            catch
            {
                return "0px";
            }
        }

        #endregion

    }
}
