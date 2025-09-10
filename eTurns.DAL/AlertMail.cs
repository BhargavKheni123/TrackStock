using eTurns.DTO;
using eTurns.DTO.Resources;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using eTurns.DTO.Helper;
namespace eTurns.DAL
{
    public class AlertMail
    {
        #region Variable Declaration
        string SubReportResFile = string.Empty;
        long ParentID = 0;
        string ReportBasePath = System.Configuration.ConfigurationManager.AppSettings["RDLCBaseFilePath"];
        string ResourceBaseFilePath = System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"];
        //string OverrideToEmail = System.Configuration.ConfigurationManager.AppSettings["OverrideToEmail"];
        XNamespace ns = XNamespace.Get("http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition");
        XNamespace nsrd = XNamespace.Get("http://schemas.microsoft.com/SQLServer/reporting/reportdesigner");
        Dictionary<string, string> rptPara = null;
        string rdlPath = string.Empty;
        string strSubTablix = string.Empty;
        string DBconectstring = string.Empty;
        string AlterNativeRowStyle = "Style='background:#DBD9D9;'";

        #endregion

        #region Generate byte for Files
        /// <summary>
        /// Generate list of byte based on attachement types
        /// </summary>
        /// <param name="objNotificationDTO"></param>
        /// <param name="objEnterprise"></param>
        /// <returns></returns>
        public List<eMailAttachmentDTO> GenerateBytesBasedOnAttachmentForAlert(NotificationDTO objNotificationDTO, EnterpriseDTO objEnterprise, Dictionary<string, string> Params)
        {
            List<eMailAttachmentDTO> bytes = new List<eMailAttachmentDTO>();

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
            IEnumerable<XElement> lstSubTablix = null;
            IEnumerable<XElement> lstUpdateSubTablix = null;
            ReportDataSource rds = null;
            XDocument doc = null;
            RegionSettingDAL rsDAL = null;
            eTurnsRegionInfo rsInfo = null;
            try
            {
                if (objNotificationDTO != null)
                {
                    objDAL = new ReportMasterDAL(objEnterprise.EnterpriseDBName);
                    rsDAL = new RegionSettingDAL(objEnterprise.EnterpriseDBName);
                    rsInfo = rsDAL.GetRegionSettingsById(objNotificationDTO.RoomID, objNotificationDTO.CompanyID, 0);

                    strEmail = Convert.ToString(objNotificationDTO.EmailAddress);


                    if (objNotificationDTO.AttachedReportMasterDTO != null && !string.IsNullOrEmpty(strEmail))
                    {
                        //Set values required for report 
                        MasterReportResFile = objNotificationDTO.AttachedReportMasterDTO.MasterReportResFile;
                        SubReportResFile = MasterReportResFile;
                        string Reportname = objNotificationDTO.AttachedReportMasterDTO.ReportName;
                        string MasterReportname = objNotificationDTO.AttachedReportMasterDTO.ReportFileName;
                        string SubReportname = objNotificationDTO.AttachedReportMasterDTO.SubReportFileName;
                        string mainGuid = "RPT_" + Guid.NewGuid().ToString().Replace("-", "_");
                        string subGuid = "SubRPT_" + Guid.NewGuid().ToString().Replace("-", "_");
                        string ReportPath = string.Empty;
                        bool hasSubReport = false;
                        string strCulture = "en-US";
                        if (rsInfo != null && !string.IsNullOrEmpty(rsInfo.CultureCode))
                            strCulture = rsInfo.CultureCode;

                        if (!string.IsNullOrEmpty(objNotificationDTO.CultureCode))
                            strCulture = objNotificationDTO.CultureCode;

                        ParentID = objNotificationDTO.AttachedReportMasterDTO.ParentID ?? 0;





                        if (objNotificationDTO.AttachedReportMasterDTO.ParentID > 0)
                        {
                            if (objNotificationDTO.AttachedReportMasterDTO.ISEnterpriseReport.GetValueOrDefault(false))
                            {
                                ReportPath = CopyFiletoTempForAlert(ReportBasePath + @"\" + objEnterprise.ID + @"\EnterpriseReport" + @"\" + MasterReportname, mainGuid, ReportBasePath + @"\Temp");
                            }
                            else
                            {
                                ReportPath = CopyFiletoTempForAlert(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["RDLCBaseFilePath"]) + @"\" + objEnterprise.ID + @"\" + objNotificationDTO.CompanyID + @"\\" + MasterReportname, mainGuid, ReportBasePath + @"\Temp");
                            }
                        }
                        else
                        {
                            ReportPath = CopyFiletoTempForAlert(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["RDLCBaseFilePath"]) + @"\" + objEnterprise.ID + @"\BaseReport" + @"\\" + MasterReportname, mainGuid, ReportBasePath + @"\Temp");
                        }

                        doc = XDocument.Load(ReportPath);
                        lstTablix = doc.Descendants(ns + "Tablix");

                        lstUpdateTablix = UpdateResourceForAlertNew(lstTablix, MasterReportResFile, strCulture, objEnterprise.ID, objNotificationDTO.CompanyID, objNotificationDTO.RoomID);
                        doc.Save(ReportPath);


                        doc = AddFormatToTaxbox(doc, rsInfo);
                        doc.Save(ReportPath);
                        doc = XDocument.Load(ReportPath);

                        if (doc.Descendants(ns + "Subreport") != null && doc.Descendants(ns + "Subreport").Count() > 0)
                        {
                            hasSubReport = true;
                        }


                        //Hardik Code Started

                        eTurns.DAL.AlertMail objAlertMail = new eTurns.DAL.AlertMail();
                        if (objNotificationDTO != null && (objNotificationDTO.HideHeader))
                        {
                            if (!hasSubReport && !objNotificationDTO.AttachedReportMasterDTO.IsNotEditable.GetValueOrDefault(false)
                                             && (objNotificationDTO.AttachedReportMasterDTO.ReportType == 3 || objNotificationDTO.AttachedReportMasterDTO.ReportType == 1)
                                             && (objNotificationDTO.HideHeader))
                            {
                                doc = objAlertMail.GetAdditionalHeaderRow(doc, objNotificationDTO.AttachedReportMasterDTO, objNotificationDTO.CompanyName, objNotificationDTO.RoomName, EnterpriseDBName: objEnterprise.EnterpriseDBName);
                                doc.Save(ReportPath);
                                doc = XDocument.Load(ReportPath);

                            }
                        }

                        if (objNotificationDTO != null && (objNotificationDTO.ShowSignature))
                        {
                            doc = objAlertMail.GetFooterForSignature(doc, objNotificationDTO.AttachedReportMasterDTO);
                            doc.Save(ReportPath);
                            doc = XDocument.Load(ReportPath);
                        }
                        if (objNotificationDTO != null && objNotificationDTO.SortSequence != null && (!string.IsNullOrWhiteSpace(objNotificationDTO.SortSequence)))
                        {
                            objNotificationDTO.AttachedReportMasterDTO.SortColumns = objNotificationDTO.SortSequence;
                        }
                        //Hardik Code ended

                        string strTablix = string.Empty;
                        if (lstTablix != null && lstTablix.ToList().Count > 0)
                        {
                            strTablix = lstTablix.ToList()[0].ToString();
                        }
                        //lstUpdateTablix = UpdateResourceForAlert(lstTablix, MasterReportResFile);
                        
                        rptPara = SetGetPDFReportParaDictionary(objEnterprise, objNotificationDTO.CompanyID, objNotificationDTO.RoomID, objNotificationDTO, Params);

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
                                //if (!string.IsNullOrEmpty(rptPara.FirstOrDefault(x => x.Key.Replace("@", "") == item.Attribute("Name").Value.Replace("@", "")).Value))
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
                        sqla = new SqlDataAdapter(cmd);
                        sqla.Fill(dt);

                        if (dt != null && dt.Rows.Count > 0)
                        {

                            ReportViewer1 = new ReportViewer();
                            ReportViewer1.Reset();
                            ReportViewer1.LocalReport.DataSources.Clear();

                            if (doc.Descendants(ns + "Subreport") != null && doc.Descendants(ns + "Subreport").Count() > 0)
                            {
                                hasSubReport = true;
                                if (objNotificationDTO.AttachedReportMasterDTO.ParentID > 0)
                                {

                                    if (objNotificationDTO.AttachedReportMasterDTO.ISEnterpriseReport.GetValueOrDefault(false))
                                    {
                                        rdlPath = CopyFiletoTempForAlert(ReportBasePath + @"\" + objEnterprise.ID + @"\EnterpriseReport" + @"\" + SubReportname, subGuid, ReportBasePath + @"\Temp");
                                    }
                                    else
                                    {
                                        rdlPath = CopyFiletoTempForAlert(ReportBasePath + @"\" + objEnterprise.ID + @"\" + objNotificationDTO.CompanyID + @"\\" + SubReportname, subGuid, ReportBasePath + @"\Temp");
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
                                lstSubTablix = docSub.Descendants(ns + "Tablix");

                                if (lstSubTablix != null && lstSubTablix.ToList().Count > 0)
                                {
                                    strSubTablix = lstSubTablix.ToList()[0].ToString();
                                }
                               
                                //lstUpdateSubTablix = UpdateResourceForAlert(lstSubTablix, SubReportResFile);
                                lstUpdateSubTablix = UpdateResourceForAlertNew(lstSubTablix, SubReportResFile, strCulture, objEnterprise.ID, objNotificationDTO.CompanyID, objNotificationDTO.RoomID);
                                docSub.Save(rdlPath);
                                docSub = XDocument.Load(rdlPath);

                                docSub = AddFormatToTaxbox(docSub, rsInfo);
                                docSub.Save(rdlPath);
                                docSub = XDocument.Load(rdlPath);

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
                                //bytes.Add(tempByte);
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
                                //Generate byte for both Excel and PDF files
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

                                // bytes.Add(tempByte);

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
            catch
            {
                return null;
                throw;
            }
            finally
            {
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
                lstSubTablix = null;
                lstUpdateSubTablix = null;
                rds = null;
                //if (ReportViewer1 != null)
                //{
                //    ReportViewer1.Dispose();
                //}
            }
            return bytes;
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
            catch
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

        private Dictionary<string, string> SetGetPDFReportParaDictionary(EnterpriseDTO EntDTO, long CompanyID, long RoomID, NotificationDTO objNotificationDTO, Dictionary<string, string> Params)
        {
            Dictionary<string, string> dictionary = null;
            try
            {

                dictionary = new Dictionary<string, string>();
                DateTime startDate = DateTime.MinValue;
                DateTime EndDate = DateTime.MinValue;
                CompanyMasterDTO objCompDTO = new CompanyMasterDAL(EntDTO.EnterpriseDBName).GetCompanyByID(CompanyID);
                CommonDAL objCommonDAL = new CommonDAL(EntDTO.EnterpriseDBName);


                dictionary.Add("ConnectionString", DbConnectionHelper.GeteTurnsSQLConnectionString(EntDTO.EnterpriseDBName, DbConnectionType.GeneralReadOnly.ToString("F")));
                dictionary.Add("CompanyIDs", Convert.ToString(CompanyID));
                dictionary.Add("RoomIDs", Convert.ToString(RoomID));
                if (objCommonDAL.HasSpecialDomain(EntDTO.EnterpriseDBName, EntDTO.ID))
                {
                    dictionary.Add("eTurnsLogoURL", System.Configuration.ConfigurationManager.AppSettings["DomainName"] + ConvertImageToPNG(eTurnsAppConfig.BaseFileSharedPath, "Uploads\\EnterpriseLogos\\" + EntDTO.ID + "\\" + EntDTO.EnterpriseLogo));
                    dictionary.Add("EnterpriseLogoURL", System.Configuration.ConfigurationManager.AppSettings["DomainName"] + "/Content/OpenAccess/NologoReport.png");
                }
                else
                {
                    dictionary.Add("eTurnsLogoURL", System.Configuration.ConfigurationManager.AppSettings["DomainName"] + "/Content/OpenAccess/logoInReport.png");
                    dictionary.Add("EnterpriseLogoURL", System.Configuration.ConfigurationManager.AppSettings["DomainName"] + ConvertImageToPNG(eTurnsAppConfig.BaseFileSharedPath, "Uploads\\EnterpriseLogos\\" + EntDTO.ID + "\\" + EntDTO.EnterpriseLogo));
                }
                dictionary.Add("CompanyLogoURL", System.Configuration.ConfigurationManager.AppSettings["LocalHostName"] + ConvertImageToPNG(eTurnsAppConfig.BaseFileSharedPath, "Uploads\\CompanyLogos\\" + CompanyID + "\\" + objCompDTO.CompanyLogo));
                dictionary.Add("UserID", null);
                dictionary.Add("BarcodeURL", System.Configuration.ConfigurationManager.AppSettings["DomainName"] + "/Barcode/GetBarcodeByKey?barcodekey=");
                dictionary.Add("WOSignatureURL", System.Configuration.ConfigurationManager.AppSettings["DomainName"] + "/Uploads/WorkOrderSignature/" + CompanyID + "/");
                dictionary.Add("WOAttachmentPath", System.Configuration.ConfigurationManager.AppSettings["DomainName"] + "/Uploads/WorkOrderFile/" + EntDTO.ID + "/" + CompanyID + "/" + RoomID + "/");

                //if (objNotificationDTO.SchedulerParams != null && objNotificationDTO.SchedulerParams.ReportDataSelectionType == 2)
                //{
                //    startDate = DateTime.Now.AddDays(-(Convert.ToDouble(objNotificationDTO.SchedulerParams.ReportDataSince)));
                //    EndDate = DateTime.Now;
                //    dictionary.Add("StartDate", startDate.ToString("yyyy-MM-dd"));
                //    dictionary.Add("EndDate", EndDate.ToString("yyyy-MM-dd"));
                //}
                //else if (objNotificationDTO.SchedulerParams != null && objNotificationDTO.SchedulerParams.ReportDataSelectionType == 1)
                //{
                //    if (objReportMailLogDTO != null)
                //    {
                //        startDate = objReportMailLogDTO.SendDate;
                //        dictionary.Add("StartDate", startDate.ToString("yyyy-MM-dd"));
                //    }
                //    EndDate = DateTime.Now;
                //    dictionary.Add("EndDate", EndDate.ToString("yyyy-MM-dd"));
                //}
                if (Params.Keys.Contains("DataGuids"))
                {
                    dictionary.Add("DataGuids", Params["DataGuids"]);
                }
                dictionary.Add("StartDate", null);
                dictionary.Add("EndDate", null);

                if (!string.IsNullOrEmpty(objNotificationDTO.SupplierIds))
                    dictionary.Add("SupplierIDs", objNotificationDTO.SupplierIds);
                else
                    dictionary.Add("SupplierIDs", string.Empty);
                return dictionary;


            }
            catch
            {
                throw;
            }
            finally
            {
                dictionary = null;
            }

        }

        private string ConvertImageToPNG(string BasePath, string InnerPath)
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

        private string GetResourceValueForAlert(string FileName, string Key)
        {
            string KeyVal = string.Empty;
            KeyVal = ResourceHelper.GetResourceValue(Key, FileName);
            return KeyVal;
        }
        private IEnumerable<XElement> UpdateResourceForAlertNew(IEnumerable<XElement> lstTablix, string ResFile, string strCulture, Int64 EntID, Int64 CompanyID, Int64 RoomID)
        {
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
                            Cell.Descendants(ns + "Value").FirstOrDefault().Value = GetResourceValueForAlertNew(ResourceFilePath, Cell.Descendants(ns + "Value").FirstOrDefault().Value, ResourceFilePathForUDF, EntID, CompanyID, RoomID, ResFile, strCulture);
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
        private string GetResourceValueForAlertNew(string FileName, string Key, string FileNameForUDF, Int64 EntID = 0, Int64 CompanyID = 0, Int64 RoomID = 0, string ResFile = "", string strCulture = "")
        {
            string KeyVal = string.Empty;
            if (Key.Trim().ToLower().Contains("udf"))
            {
                KeyVal = ResourceRead.GetResourceValueByKeyAndFullFilePath(Key, FileNameForUDF, EntID, CompanyID, RoomID, ResFile, strCulture);
            }
            else
            {
                KeyVal = ResourceRead.GetResourceValueByKeyAndFullFilePath(Key, FileName, EntID, CompanyID, RoomID, ResFile, strCulture);
            }

            return KeyVal;
        }
        private IEnumerable<XElement> UpdateResourceForAlert(IEnumerable<XElement> lstTablix, string ResFile)
        {
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
                            Cell.Descendants(ns + "Value").FirstOrDefault().Value = GetResourceValueForAlert(ResFile, Cell.Descendants(ns + "Value").FirstOrDefault().Value);
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

        private string CopyFiletoTempForAlert(string strfile, string reportname, string ReportTempPath)
        {
            try
            {
                string ReportRetPath = string.Empty;

                if (!System.IO.Directory.Exists(ReportTempPath))
                {
                    System.IO.Directory.CreateDirectory(ReportTempPath);
                }
                ReportRetPath = ReportTempPath + @"\" + reportname + ".rdlc";
                //System.IO.File.Copy(strfile, ReportRetPath);
                System.IO.File.Create(ReportRetPath).Dispose();
                System.IO.File.WriteAllText(ReportRetPath, System.IO.File.ReadAllText(strfile));

                return ReportRetPath;
            }
            catch (Exception)
            {
                return "";
            }
            finally
            {

            }
        }

        private void GetFormatedTextbox(string name, XElement txtBox, eTurnsRegionInfo rSInfo)
        {
            string costDecimal = "c";//@"=CStr(""c"")+ CStr(Fields!CostDecimalPoint.Value)";
            string qtyDecimal = "f";//@"=CStr(""f"")+ CStr(Fields!QuantityDecimalPoint.Value)";
            string lngCulture = "en-US";

            if (rSInfo != null)
            {
                if(string.IsNullOrWhiteSpace(rSInfo.CurrencySymbol))
                {
                    costDecimal = "n";
                }
                costDecimal = costDecimal + rSInfo.CurrencyDecimalDigits;
                qtyDecimal = qtyDecimal + rSInfo.NumberDecimalDigits;

                if (!string.IsNullOrEmpty(rSInfo.CultureCode))
                {
                    lngCulture = rSInfo.CultureCode;
                }
            }


            XElement textRun = txtBox.Descendants(ns + "TextRun").FirstOrDefault();
            XElement style = textRun.Descendants(ns + "Style").FirstOrDefault();
            if (style == null)
            {
                style = new XElement(ns + "Style");
                textRun.Add(style);
            }
            if (style != null)
            {
                XElement frmt = textRun.Descendants(ns + "Format").FirstOrDefault();
                if (frmt == null)
                {
                    frmt = new XElement(ns + "Format");
                    if (name.ToLower().Contains("qty") || name.ToLower().Contains("quantity"))
                    {
                        frmt.Value = qtyDecimal;
                    }
                    else if (name.ToLower().Contains("cost") || name.ToLower().Contains("price") || name.ToLower().Contains("total"))
                    {
                        frmt.Value = costDecimal;
                    }

                    style.Add(frmt);
                }

                XElement langEle = textRun.Descendants(ns + "Language").FirstOrDefault();
                if (langEle == null)
                {
                    langEle = new XElement(ns + "Language");
                    langEle.Value = lngCulture;
                    style.Add(langEle);
                }

            }

        }

        public XDocument AddFormatToTaxbox(XDocument doc1, eTurnsRegionInfo rSInfo)
        {
            XDocument doc = new XDocument(doc1);
            XElement ds = doc.Descendants(ns + "DataSet").FirstOrDefault();
            IEnumerable<XElement> dsFields = ds.Descendants(ns + "Field");
            dsFields = dsFields.Where(y => y.Descendants(nsrd + "TypeName").Where(x => x.Value == "System.Double").Count() > 0);
            XElement xRows = doc.Descendants(ns + "TablixRows").FirstOrDefault();
            foreach (var dsField in dsFields)
            {
                string name = dsField.Descendants(ns + "DataField").FirstOrDefault().Value;
                XElement txtBox = xRows.Descendants(ns + "Textbox").Where(x => x.Descendants(ns + "Value").Where(y => y.Value == "=Fields!" + name + ".Value").Count() > 0).FirstOrDefault();

                if (txtBox != null)
                {
                    GetFormatedTextbox(name, txtBox, rSInfo);
                    if (name.ToLower() == "total")
                    {
                        IEnumerable<XElement> txtBoxSumTotal = xRows.Descendants(ns + "Textbox").Where(x => x.Descendants(ns + "Value").Where(y => y.Value == "=Sum(Fields!Total.Value)").Count() > 0);
                        if (txtBoxSumTotal != null && txtBoxSumTotal.Count() > 0)
                        {
                            foreach (var txt in txtBoxSumTotal)
                            {
                                GetFormatedTextbox(name, txt, rSInfo);
                            }

                        }
                    }
                }
            }

            return doc;
        }


        #endregion

        #region Send Mail

        public void SendReportNotificationForAlert(NotificationDTO objNotificationDTO, EnterpriseDTO objEnterpriseDTO)
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
                    string TemplateName = string.Empty;
                    if (!string.IsNullOrEmpty(objNotificationDTO.EmailTemplateDetail.TemplateName))
                        TemplateName = objNotificationDTO.EmailTemplateDetail.TemplateName.ToLower();
                    switch (TemplateName)
                    {
                        case "pendingorders":
                            EmailBody = SendMailForPendingOrders(objNotificationDTO, objEnterpriseDTO);
                            break;
                        case "pendingrequisition":
                            EmailBody = SendMailForPendingRequisition(objNotificationDTO, objEnterpriseDTO);
                            break;
                        case "pendingtransfers":
                            EmailBody = SendMailForPendingTransfer(objNotificationDTO, objEnterpriseDTO);
                            break;
                        case "suggestedorderscritical":
                            EmailBody = SendMailForSuggestedOrdersCritical(objNotificationDTO, objEnterpriseDTO);
                            break;
                        case "suggestedordersminimum":
                            EmailBody = SendMailForSuggestedOrdersMinimum(objNotificationDTO, objEnterpriseDTO);
                            break;
                        case "suggestedorders":
                            EmailBody = SendMailForSuggestedOrder(objNotificationDTO, objEnterpriseDTO);
                            break;
                        case "pendingreturnorders":
                            EmailBody = SendMailForPendingReturnOrders(objNotificationDTO, objEnterpriseDTO);
                            break;
                        default:
                            break;
                    }
                    //EmailBody = objNotificationDTO.EmailTemplateDetail.lstEmailTemplateDtls.First().MailBodyText;
                }
            }
            lstAttachments = GenerateBytesBasedOnAttachmentForAlert(objNotificationDTO, objEnterpriseDTO, null);
            CreateAlertMail(lstAttachments, EmailSubject, EmailBody, ToEmailAddersses, objNotificationDTO, objEnterpriseDTO);
        }

        public void CreateAlertMail(List<eMailAttachmentDTO> objeMailAttchList, string MailSubject, string MessageBody, string ToEmailAddress, NotificationDTO objNotificationDTO, EnterpriseDTO objEnterpriseDTO, Int64 UserId = 0)
        {
            eMailMasterDAL objUtils = null;
            ReportMailLogDTO objReportMailLogDTO = null;
            ReportMasterDAL objReportMasterDAL = null;

            try
            {
                string strCCAddress = "";
                objUtils = new eMailMasterDAL(DbConnectionHelper.GetETurnsMasterDBName());
                if (objNotificationDTO != null)
                {
                    eTurnsRegionInfo objeTurnsRegionInfo = null;

                    RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(DbConnectionHelper.GetETurnsMasterDBName());
                    objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(objNotificationDTO.RoomID, objNotificationDTO.CompanyID, -1);

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
                        if (!string.IsNullOrWhiteSpace(objNotificationDTO.CompanyName))
                        {
                            MailSubject = Regex.Replace(MailSubject, "@@COMPANYNAME@@", objNotificationDTO.CompanyName, RegexOptions.IgnoreCase);
                        }
                        if (!string.IsNullOrWhiteSpace(objNotificationDTO.RoomName))
                        {
                            MailSubject = Regex.Replace(MailSubject, "@@ROOMNAME@@", objNotificationDTO.RoomName, RegexOptions.IgnoreCase);
                        }

                        MailSubject = Regex.Replace(MailSubject, "@@Year@@", Convert.ToString(DateTime.UtcNow.Year), RegexOptions.IgnoreCase);
                    }

                    if (MessageBody != null)
                    {
                        string CurrentDate = TZDateTimeNow.ToString(DateTimeFormat);
                        //EmailBody = EmailBody.Replace("@@DATE@@", CurrentDate).Replace("@@date@@", CurrentDate).Replace("@@Date@@", CurrentDate);
                        MessageBody = Regex.Replace(MessageBody, "@@DATE@@", CurrentDate, RegexOptions.IgnoreCase);
                        if (!string.IsNullOrWhiteSpace(objNotificationDTO.CompanyName))
                        {
                            MessageBody = Regex.Replace(MessageBody, "@@COMPANYNAME@@", objNotificationDTO.CompanyName, RegexOptions.IgnoreCase);
                        }
                        if (!string.IsNullOrWhiteSpace(objNotificationDTO.CompanyName))
                        {
                            MessageBody = Regex.Replace(MessageBody, "@@ROOMNAME@@", objNotificationDTO.RoomName, RegexOptions.IgnoreCase);
                        }

                        MessageBody = Regex.Replace(MessageBody, "@@Year@@", Convert.ToString(DateTime.UtcNow.Year), RegexOptions.IgnoreCase);

                    }
                }
                objUtils.eMailToSend(ToEmailAddress, strCCAddress, MailSubject, MessageBody, objEnterpriseDTO.ID, objNotificationDTO.CompanyID, objNotificationDTO.RoomID, UserId, objeMailAttchList);

                objReportMailLogDTO = new ReportMailLogDTO();
                objReportMasterDAL = new ReportMasterDAL(objEnterpriseDTO.EnterpriseDBName);
                objReportMailLogDTO.Id = 0;
                if (objNotificationDTO.AttachedReportMasterDTO != null && objNotificationDTO.AttachedReportMasterDTO.ID > 0)
                    objReportMailLogDTO.ReportID = objNotificationDTO.AttachedReportMasterDTO.ID;
                objReportMailLogDTO.ScheduleID = objNotificationDTO.RoomScheduleID;
                objReportMailLogDTO.CompanyID = objNotificationDTO.CompanyID;
                objReportMailLogDTO.RoomID = objNotificationDTO.RoomID;
                objReportMailLogDTO.SendDate = DateTime.UtcNow;
                objReportMailLogDTO.SendEmailAddress = ToEmailAddress;
                objReportMailLogDTO.NotificationID = objNotificationDTO.ID;
                if (objeMailAttchList != null)
                {
                    objReportMailLogDTO.AttachmentCount = objeMailAttchList.Count;
                }
                objReportMasterDAL.InsertMailLog(objReportMailLogDTO);

            }
            finally
            {
                objUtils = null;
                objReportMailLogDTO = null;
                objReportMasterDAL = null;
            }
        }
        #endregion

        #region Templates

        private string SendMailForPendingOrders(NotificationDTO objNotificationDTO, EnterpriseDTO objEnterprise)
        {
            OrderMasterDAL objOrderDAL = null;
            IEnumerable<OrderMasterDTO> objPendingOrders = null;
            StringBuilder MessageBody = null;
            EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;

            try
            {
                int PriseSelectionOption = 1;
                RoomDAL objRoomDAL = new RoomDAL(objEnterprise.EnterpriseDBName);
                RoomModuleSettingsDTO objRoomModuleSettingsDTO = new RoomModuleSettingsDTO();
                objRoomModuleSettingsDTO = objRoomDAL.GetRoomModuleSettings(objNotificationDTO.CompanyID, objNotificationDTO.RoomID, (long)ModuleInfo.Orders);

                if (objRoomModuleSettingsDTO != null && objRoomModuleSettingsDTO.ID > 0)
                {
                    PriseSelectionOption = objRoomModuleSettingsDTO.PriseSelectionOption.GetValueOrDefault(0);
                }

                string OrderCostTitle = "Order Cost";
                OrderCostTitle = (PriseSelectionOption == 1 ? "Order Price" : "Order Cost");

                string QtyFormat = "N";
                string CostFormat = "N";
                string dateFormate = "MM/dd/yyyy";


                if (objNotificationDTO.objeTurnsRegionInfo.NumberDecimalDigits > 0)
                    QtyFormat += objNotificationDTO.objeTurnsRegionInfo.NumberDecimalDigits;

                if (objNotificationDTO.objeTurnsRegionInfo.CurrencyDecimalDigits > 0)
                    CostFormat += objNotificationDTO.objeTurnsRegionInfo.CurrencyDecimalDigits;

                if (!string.IsNullOrEmpty(objNotificationDTO.objeTurnsRegionInfo.ShortDatePattern) && !string.IsNullOrEmpty(objNotificationDTO.objeTurnsRegionInfo.ShortTimePattern))
                    dateFormate = objNotificationDTO.objeTurnsRegionInfo.ShortDatePattern + " " + objNotificationDTO.objeTurnsRegionInfo.ShortTimePattern;

                //List<EmailTemplateDetailDTO> lstEmailTemplateDtl = new List<EmailTemplateDetailDTO>();
                if (objNotificationDTO != null && objNotificationDTO.EmailTemplateDetail != null && objNotificationDTO.EmailTemplateDetail.lstEmailTemplateDtls != null && objNotificationDTO.EmailTemplateDetail.lstEmailTemplateDtls.Count() > 0)
                {
                    objEmailTemplateDetailDTO = objNotificationDTO.EmailTemplateDetail.lstEmailTemplateDtls.First();
                }
                if (objEmailTemplateDetailDTO == null)
                {
                    objEmailTemplateDetailDTO = new EmailTemplateDetailDTO();
                }
                string strCulture = "en-US";

                if (!string.IsNullOrEmpty(objNotificationDTO.objeTurnsRegionInfo.CultureCode))
                    strCulture = objNotificationDTO.objeTurnsRegionInfo.CultureCode.ToString();

                string ResourceFilePath = GetResourceFileFullPath("ResOrder", strCulture, objEnterprise.ID, objNotificationDTO.CompanyID);
                string OrderCostTitleKey = (PriseSelectionOption == 1 ? "OrderPriceMail" : "OrderCostMail");
                OrderCostTitle = ResourceRead.GetResourceValueByKeyAndFullFilePath(OrderCostTitleKey, ResourceFilePath,objEnterprise.ID,objNotificationDTO.CompanyID,objNotificationDTO.RoomID, "ResOrder",strCulture);
                string htmlTabl = string.Empty;
                htmlTabl = @"<table style=""margin-left: 0px; width: 99%;""  border=""1"" cellpadding=""0""
                            cellspacing=""0"">
                            <thead>
                                <tr role=""row"">
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResourceRead.GetResourceValueByKeyAndFullFilePath("OrderNumber", ResourceFilePath, objEnterprise.ID, objNotificationDTO.CompanyID, objNotificationDTO.RoomID, "ResOrder", strCulture) + @"
                                    </th>
                                    <th  style=""width: 5%; text-align: left;"">
                                        " + ResourceRead.GetResourceValueByKeyAndFullFilePath("ReleaseNumber", ResourceFilePath, objEnterprise.ID, objNotificationDTO.CompanyID, objNotificationDTO.RoomID, "ResOrder", strCulture) + @"
                                    </th>
                                    <th  style=""width: 15%; text-align: left;"">
                                        " + ResourceRead.GetResourceValueByKeyAndFullFilePath("Supplier", ResourceFilePath, objEnterprise.ID, objNotificationDTO.CompanyID, objNotificationDTO.RoomID, "ResOrder", strCulture) + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResourceRead.GetResourceValueByKeyAndFullFilePath("equiredDate", ResourceFilePath, objEnterprise.ID, objNotificationDTO.CompanyID, objNotificationDTO.RoomID, "ResOrder", strCulture) + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResourceRead.GetResourceValueByKeyAndFullFilePath("OrderStatus", ResourceFilePath, objEnterprise.ID, objNotificationDTO.CompanyID, objNotificationDTO.RoomID, "ResOrder", strCulture) + @"
                                    </th>
                                    <th  style=""width: 5%; text-align: left;"">
                                        No Of Items
                                    </th>
                                    <th  style=""width: 5%; text-align: left;"">
                                       " + OrderCostTitle + @"
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                            ##TRS##
                            </tbody>
                        </table>";


                objOrderDAL = new OrderMasterDAL(objEnterprise.EnterpriseDBName);

                if (string.IsNullOrEmpty(objNotificationDTO.SupplierIds) || string.IsNullOrWhiteSpace(objNotificationDTO.SupplierIds))
                    objPendingOrders = objOrderDAL.GetPendingOrderListNormal(objNotificationDTO.RoomID, objNotificationDTO.CompanyID, OrderType.Order);
                else // supplier filter
                    objPendingOrders = objOrderDAL.GetPendingOrderListBySupplierIdsNormal(objNotificationDTO.RoomID, objNotificationDTO.CompanyID, OrderType.Order, objNotificationDTO.SupplierIds);

                string trs = "";

                if (objPendingOrders != null && objPendingOrders.Count() > 0)
                {
                    if (objPendingOrders != null)
                    {
                        int cntrow = 1;
                        foreach (var item in objPendingOrders)
                        {
                            string RowStyle = string.Empty;
                            if (cntrow % 2 == 0)
                            {
                                RowStyle = AlterNativeRowStyle;
                            }
                            else
                            {

                            }
                            trs += @"<tr " + RowStyle + @" >
                        <td>
                            " + item.OrderNumber + @"
                        </td>
                        <td>
                            " + (string.IsNullOrEmpty(item.ReleaseNumber) ? "&nbsp;" : item.ReleaseNumber) + @"
                        </td>
                        <td>
                            " + (string.IsNullOrEmpty(item.SupplierName) ? "&nbsp;" : item.SupplierName) + @"
                        </td>
                        <td>
                            " + item.RequiredDate.ToString(dateFormate) + @"
                        </td>
                        <td>
                            " + ResourceRead.GetResourceValueByKeyAndFullFilePath(((eTurns.DTO.OrderStatus)item.OrderStatus).ToString(), ResourceFilePath,objEnterprise.ID,objNotificationDTO.CompanyID,objNotificationDTO.RoomID, "ResOrder", strCulture) + @"
                        </td>
                        <td>
                            " + item.NoOfLineItems.GetValueOrDefault(0).ToString() + @"
                        </td>
                        <td>                            
                            " + (Convert.ToString(PriseSelectionOption) == "1" ? item.OrderPrice.ToString(CostFormat) : item.OrderCost.GetValueOrDefault(0).ToString(CostFormat)) + @"
                        </td>
                    </tr>";
                            cntrow += 1;
                        }
                    }


                }
                //if Not Data Found 
                if (objPendingOrders != null && objPendingOrders.Count() <= 0)
                {
                    string ResourceFileCommon = GetResourceFileFullPath("ResCommon", strCulture, objEnterprise.ID, objNotificationDTO.CompanyID);
                    string Str = ResourceRead.GetResourceValueByKeyAndFullFilePath("msgNoDataFound", ResourceFileCommon,objEnterprise.ID,objNotificationDTO.CompanyID,objNotificationDTO.RoomID, "ResCommon",strCulture);
                    string RowStyle = string.Empty;
                    trs += @"<tr " + RowStyle + @" >
                        <td colspan=7>
                            " + Str + @"
                        </td>
                    </tr>";
                }
                htmlTabl = htmlTabl.Replace("##TRS##", trs);

                MessageBody = new StringBuilder();
                MessageBody.Append(objEmailTemplateDetailDTO.MailBodyText);
                MessageBody.Replace("@@TABLE@@", htmlTabl);
                MessageBody.Replace("@@COMPANYNAME@@", objNotificationDTO.CompanyName);
                MessageBody.Replace("@@ROOMNAME@@", objNotificationDTO.RoomName);

            }
            catch (Exception)
            {
                return "";
            }
            finally
            {
                objOrderDAL = null;
                objPendingOrders = null;
                objEmailTemplateDetailDTO = null;
            }
            return MessageBody.ToString();
        }

        private string SendMailForPendingReturnOrders(NotificationDTO objNotificationDTO, EnterpriseDTO objEnterprise)
        {
            OrderMasterDAL objOrderDAL = null;
            IEnumerable<OrderMasterDTO> objPendingOrders = null;
            StringBuilder MessageBody = null;
            EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;

            try
            {
                int PriseSelectionOption = 1;
                RoomDAL objRoomDAL = new RoomDAL(objEnterprise.EnterpriseDBName);
                RoomModuleSettingsDTO objRoomModuleSettingsDTO = new RoomModuleSettingsDTO();
                objRoomModuleSettingsDTO = objRoomDAL.GetRoomModuleSettings(objNotificationDTO.CompanyID, objNotificationDTO.RoomID, (long)ModuleInfo.Orders);

                if (objRoomModuleSettingsDTO != null && objRoomModuleSettingsDTO.ID > 0)
                {
                    PriseSelectionOption = objRoomModuleSettingsDTO.PriseSelectionOption.GetValueOrDefault(0);
                }

                string OrderCostTitle = "Order Cost";
                OrderCostTitle = (PriseSelectionOption == 1 ? "Order Price" : "Order Cost");

                string QtyFormat = "N";
                string CostFormat = "N";
                string dateFormate = "MM/dd/yyyy";


                if (objNotificationDTO.objeTurnsRegionInfo.NumberDecimalDigits > 0)
                    QtyFormat += objNotificationDTO.objeTurnsRegionInfo.NumberDecimalDigits;

                if (objNotificationDTO.objeTurnsRegionInfo.CurrencyDecimalDigits > 0)
                    CostFormat += objNotificationDTO.objeTurnsRegionInfo.CurrencyDecimalDigits;

                if (!string.IsNullOrEmpty(objNotificationDTO.objeTurnsRegionInfo.ShortDatePattern) && !string.IsNullOrEmpty(objNotificationDTO.objeTurnsRegionInfo.ShortTimePattern))
                    dateFormate = objNotificationDTO.objeTurnsRegionInfo.ShortDatePattern + " " + objNotificationDTO.objeTurnsRegionInfo.ShortTimePattern;

                if (objNotificationDTO != null && objNotificationDTO.EmailTemplateDetail != null && objNotificationDTO.EmailTemplateDetail.lstEmailTemplateDtls != null && objNotificationDTO.EmailTemplateDetail.lstEmailTemplateDtls.Count() > 0)
                {
                    objEmailTemplateDetailDTO = objNotificationDTO.EmailTemplateDetail.lstEmailTemplateDtls.First();
                }
                if (objEmailTemplateDetailDTO == null)
                {
                    objEmailTemplateDetailDTO = new EmailTemplateDetailDTO();
                }


                string strCulture = "en-US";

                if (!string.IsNullOrEmpty(objNotificationDTO.objeTurnsRegionInfo.CultureCode))
                    strCulture = objNotificationDTO.objeTurnsRegionInfo.CultureCode.ToString();

                string ResourceFilePath = GetResourceFileFullPath("ResOrder", strCulture, objEnterprise.ID, objNotificationDTO.CompanyID);
                string htmlTabl = string.Empty;
                string OrderCostTitleKey = (PriseSelectionOption == 1 ? "OrderPriceMail" : "OrderCostMail");
                OrderCostTitle = ResourceRead.GetResourceValueByKeyAndFullFilePath(OrderCostTitleKey, ResourceFilePath,objEnterprise.ID,objNotificationDTO.CompanyID,objNotificationDTO.RoomID, "ResOrder", strCulture);
                htmlTabl = @"<table style=""margin-left: 0px; width: 99%;""  border=""1"" cellpadding=""0""
                            cellspacing=""0"">
                            <thead>
                                <tr role=""row"">
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResourceRead.GetResourceValueByKeyAndFullFilePath("OrderNumber", ResourceFilePath,objEnterprise.ID, objNotificationDTO.CompanyID, objNotificationDTO.RoomID, "ResOrder", strCulture) + @"
                                    </th>
                                    <th  style=""width: 5%; text-align: left;"">
                                        " + ResourceRead.GetResourceValueByKeyAndFullFilePath("ReleaseNumber", ResourceFilePath,objEnterprise.ID, objNotificationDTO.CompanyID, objNotificationDTO.RoomID, "ResOrder", strCulture) + @"
                                    </th>
                                    <th  style=""width: 15%; text-align: left;"">
                                        " + ResourceRead.GetResourceValueByKeyAndFullFilePath("Supplier", ResourceFilePath, objEnterprise.ID, objNotificationDTO.CompanyID, objNotificationDTO.RoomID, "ResOrder", strCulture) + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResourceRead.GetResourceValueByKeyAndFullFilePath("equiredDate", ResourceFilePath, objEnterprise.ID, objNotificationDTO.CompanyID, objNotificationDTO.RoomID, "ResOrder", strCulture) + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResourceRead.GetResourceValueByKeyAndFullFilePath("OrderStatus", ResourceFilePath, objEnterprise.ID, objNotificationDTO.CompanyID, objNotificationDTO.RoomID, "ResOrder", strCulture) + @"
                                    </th>
                                    <th  style=""width: 5%; text-align: left;"">
                                        No Of Items
                                    </th>
                                    <th  style=""width: 5%; text-align: left;"">
                                        " + OrderCostTitle + @"
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                            ##TRS##
                            </tbody>
                        </table>";


                objOrderDAL = new OrderMasterDAL(objEnterprise.EnterpriseDBName);

                if (string.IsNullOrEmpty(objNotificationDTO.SupplierIds) || string.IsNullOrWhiteSpace(objNotificationDTO.SupplierIds))
                    objPendingOrders = objOrderDAL.GetPendingOrderListNormal(objNotificationDTO.RoomID, objNotificationDTO.CompanyID, OrderType.RuturnOrder);
                else // supplier filter
                    objPendingOrders = objOrderDAL.GetPendingOrderListBySupplierIdsNormal(objNotificationDTO.RoomID, objNotificationDTO.CompanyID, OrderType.RuturnOrder, objNotificationDTO.SupplierIds);

                string trs = "";

                if (objPendingOrders != null && objPendingOrders.Count() > 0)
                {
                    int cntrow = 1;

                    foreach (var item in objPendingOrders)
                    {
                        string RowStyle = string.Empty;
                        if (cntrow % 2 == 0)
                        {
                            RowStyle = AlterNativeRowStyle;
                        }
                        trs += @"<tr " + RowStyle + @" >
                        <td>
                            " + item.OrderNumber + @"
                        </td>
                        <td>
                            " + (string.IsNullOrEmpty(item.ReleaseNumber) ? "&nbsp;" : item.ReleaseNumber) + @"
                        </td>
                        <td>
                            " + (string.IsNullOrEmpty(item.SupplierName) ? "&nbsp;" : item.SupplierName) + @"
                        </td>
                        <td>
                            " + item.RequiredDate.ToString(dateFormate) + @"
                        </td>
                        <td>
                            " + ResourceRead.GetResourceValueByKeyAndFullFilePath(((eTurns.DTO.OrderStatus)item.OrderStatus).ToString(), ResourceFilePath,objEnterprise.ID,objNotificationDTO.CompanyID,objNotificationDTO.RoomID, "ResOrder", strCulture) + @"
                        </td>
                        <td>
                            " + item.NoOfLineItems.GetValueOrDefault(0).ToString() + @"
                        </td>
                        <td>
                            " + (Convert.ToString(PriseSelectionOption) == "1" ? item.OrderPrice.ToString(CostFormat) : item.OrderCost.GetValueOrDefault(0).ToString(CostFormat)) + @"
                        </td>
                    </tr>";
                        cntrow += 1;
                    }
                }
                //if Not Data Found 
                if (objPendingOrders != null && objPendingOrders.Count() <= 0)
                {
                    string ResourceFileCommon = GetResourceFileFullPath("ResCommon", strCulture, objEnterprise.ID, objNotificationDTO.CompanyID);
                    string Str = ResourceRead.GetResourceValueByKeyAndFullFilePath("msgNoDataFound", ResourceFileCommon, objEnterprise.ID, objNotificationDTO.CompanyID,objNotificationDTO.RoomID,"ResCommon",strCulture);
                    string RowStyle = string.Empty;
                    trs += @"<tr " + RowStyle + @" >
                        <td colspan=7>
                            " + Str + @"
                        </td>
                    </tr>";
                }
                htmlTabl = htmlTabl.Replace("##TRS##", trs);
                MessageBody = new StringBuilder();
                MessageBody.Append(objEmailTemplateDetailDTO.MailBodyText);
                MessageBody.Replace("@@TABLE@@", htmlTabl);
                MessageBody.Replace("@@COMPANYNAME@@", objNotificationDTO.CompanyName);
                MessageBody.Replace("@@ROOMNAME@@", objNotificationDTO.RoomName);

            }
            finally
            {
                objOrderDAL = null;
                objPendingOrders = null;
                objEmailTemplateDetailDTO = null;
            }
            return MessageBody.ToString();
        }

        private string SendMailForPendingTransfer(NotificationDTO objNotificationDTO, EnterpriseDTO objEnterprise)
        {
            StringBuilder MessageBody = null;
            EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;
            TransferMasterDAL objOrderDAL = null;
            IEnumerable<TransferMasterDTO> objPendingOrders = null;

            try
            {
                string QtyFormat = "N";
                string CostFormat = "N";
                string dateFormate = "MM/dd/yyyy";

                if (objNotificationDTO.objeTurnsRegionInfo.NumberDecimalDigits > 0)
                    QtyFormat += objNotificationDTO.objeTurnsRegionInfo.NumberDecimalDigits;

                if (objNotificationDTO.objeTurnsRegionInfo.CurrencyDecimalDigits > 0)
                    CostFormat += objNotificationDTO.objeTurnsRegionInfo.CurrencyDecimalDigits;

                if (!string.IsNullOrEmpty(objNotificationDTO.objeTurnsRegionInfo.ShortDatePattern) && !string.IsNullOrEmpty(objNotificationDTO.objeTurnsRegionInfo.ShortTimePattern))
                    dateFormate = objNotificationDTO.objeTurnsRegionInfo.ShortDatePattern + " " + objNotificationDTO.objeTurnsRegionInfo.ShortTimePattern;

                string strCulture = "en-US";

                if (!string.IsNullOrEmpty(objNotificationDTO.objeTurnsRegionInfo.CultureCode))
                    strCulture = objNotificationDTO.objeTurnsRegionInfo.CultureCode.ToString();

                if (objNotificationDTO != null && objNotificationDTO.EmailTemplateDetail != null && objNotificationDTO.EmailTemplateDetail.lstEmailTemplateDtls != null && objNotificationDTO.EmailTemplateDetail.lstEmailTemplateDtls.Count() > 0)
                {
                    objEmailTemplateDetailDTO = objNotificationDTO.EmailTemplateDetail.lstEmailTemplateDtls.First();
                }
                if (objEmailTemplateDetailDTO == null)
                {
                    objEmailTemplateDetailDTO = new EmailTemplateDetailDTO();
                }

                string ResourceFilePath = GetResourceFileFullPath("ResTransfer", strCulture, objEnterprise.ID, objNotificationDTO.CompanyID);
                string htmlTabl = string.Empty;
                htmlTabl = @"<table style=""margin-left: 0px; width: 99%;""  border=""1"" cellpadding=""0""
                            cellspacing=""0"">
                            <thead>
                                <tr role=""row"">
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResourceRead.GetResourceValueByKeyAndFullFilePath("TransferNumber", ResourceFilePath,objEnterprise.ID,objNotificationDTO.CompanyID,objNotificationDTO.RoomID, "ResTransfer", strCulture) + @"
                                    </th>
                                    <th  style=""width: 15%; text-align: left;"">
                                        " + ResourceRead.GetResourceValueByKeyAndFullFilePath("ReplinishRoom", ResourceFilePath, objEnterprise.ID, objNotificationDTO.CompanyID, objNotificationDTO.RoomID, "ResTransfer", strCulture) + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResourceRead.GetResourceValueByKeyAndFullFilePath("RequestType", ResourceFilePath, objEnterprise.ID, objNotificationDTO.CompanyID, objNotificationDTO.RoomID, "ResTransfer", strCulture) + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResourceRead.GetResourceValueByKeyAndFullFilePath("RequireDate", ResourceFilePath, objEnterprise.ID, objNotificationDTO.CompanyID, objNotificationDTO.RoomID, "ResTransfer", strCulture) + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResourceRead.GetResourceValueByKeyAndFullFilePath("TransferStatus", ResourceFilePath, objEnterprise.ID, objNotificationDTO.CompanyID, objNotificationDTO.RoomID, "ResTransfer", strCulture) + @"
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                            ##TRS##
                            </tbody>
                        </table>";

                objOrderDAL = new TransferMasterDAL(objEnterprise.EnterpriseDBName);
                objPendingOrders = objOrderDAL.GetPendingTransfersByRoomNormal(objNotificationDTO.RoomID, objNotificationDTO.CompanyID);

                string trs = "";
                if (objPendingOrders != null && objPendingOrders.Count() > 0)
                {
                    //objPendingOrders = objPendingOrders.Where(x => x.TransferStatus > (int)OrderStatus.Approved && x.TransferStatus < (int)OrderStatus.Closed);

                    int cntrow = 1;
                    foreach (var item in objPendingOrders)
                    {

                        string RowStyle = string.Empty;
                        if (cntrow % 2 == 0)
                        {
                            RowStyle = AlterNativeRowStyle;
                        }

                        trs += @"<tr " + RowStyle + @" >
                        <td>
                            " + item.TransferNumber + @"
                        </td>
                        <td>
                            " + item.ReplenishingRoomName + @"
                        </td>
                          <td>
                            " + (item.RequestType == 0 ? "In" : "Out") + @"
                        </td>
                        <td>
                            " + item.RequireDate.ToString(dateFormate) + @"
                        </td>
                        <td>
                            " + ((eTurns.DTO.TransferStatus)item.TransferStatus).ToString() + @"
                        </td>
                    </tr>";
                        cntrow += 1;
                    }
                }

                //if Not Data Found 
                if (objPendingOrders != null && objPendingOrders.Count() <= 0)
                {
                    string ResourceFileCommon = GetResourceFileFullPath("ResCommon", strCulture, objEnterprise.ID, objNotificationDTO.CompanyID);
                    string Str = ResourceRead.GetResourceValueByKeyAndFullFilePath("msgNoDataFound", ResourceFileCommon, objEnterprise.ID, objNotificationDTO.CompanyID,objNotificationDTO.RoomID, "ResCommon", strCulture);
                    string RowStyle = string.Empty;
                    trs += @"<tr " + RowStyle + @" >
                        <td colspan=7>
                            " + Str + @"
                        </td>
                    </tr>";
                }

                htmlTabl = htmlTabl.Replace("##TRS##", trs);
                MessageBody = new StringBuilder();
                MessageBody.Append(objEmailTemplateDetailDTO.MailBodyText);
                MessageBody.Replace("@@TABLE@@", htmlTabl);
                MessageBody.Replace("@@COMPANYNAME@@", objNotificationDTO.CompanyName);
                MessageBody.Replace("@@ROOMNAME@@", objNotificationDTO.RoomName);

            }
            finally
            {
                objEmailTemplateDetailDTO = null;
                objOrderDAL = null;
                objPendingOrders = null;
            }
            return MessageBody.ToString();
        }

        private string SendMailForPendingRequisition(NotificationDTO objNotificationDTO, EnterpriseDTO objEnterprise)
        {

            StringBuilder MessageBody = null;

            EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;
            RequisitionMasterDAL objDAL = null;
            IEnumerable<RequisitionMasterDTO> objPendingOrders = null;

            try
            {
                int PriseSelectionOption = 1;
                RoomDAL objRoomDAL = new RoomDAL(objEnterprise.EnterpriseDBName);
                RoomModuleSettingsDTO objRoomModuleSettingsDTO = new RoomModuleSettingsDTO();
                objRoomModuleSettingsDTO = objRoomDAL.GetRoomModuleSettings(objNotificationDTO.CompanyID, objNotificationDTO.RoomID, (long)ModuleInfo.Requisitions);

                if (objRoomModuleSettingsDTO != null && objRoomModuleSettingsDTO.ID > 0)
                {
                    PriseSelectionOption = objRoomModuleSettingsDTO.PriseSelectionOption.GetValueOrDefault(0);
                }

                string CostTitle = "Total Cost";
                CostTitle = (PriseSelectionOption == 1 ? "Total Price" : "Total Cost");

                string QtyFormat = "N";
                string CostFormat = "N";
                string dateFormate = "MM/dd/yyyy";

                if (objNotificationDTO.objeTurnsRegionInfo.NumberDecimalDigits > 0)
                    QtyFormat += objNotificationDTO.objeTurnsRegionInfo.NumberDecimalDigits;

                if (objNotificationDTO.objeTurnsRegionInfo.CurrencyDecimalDigits > 0)
                    CostFormat += objNotificationDTO.objeTurnsRegionInfo.CurrencyDecimalDigits;

                if (!string.IsNullOrEmpty(objNotificationDTO.objeTurnsRegionInfo.ShortDatePattern) && !string.IsNullOrEmpty(objNotificationDTO.objeTurnsRegionInfo.ShortTimePattern))
                    dateFormate = objNotificationDTO.objeTurnsRegionInfo.ShortDatePattern + " " + objNotificationDTO.objeTurnsRegionInfo.ShortTimePattern;

                string strCulture = "en-US";

                if (!string.IsNullOrEmpty(objNotificationDTO.objeTurnsRegionInfo.CultureCode))
                    strCulture = objNotificationDTO.objeTurnsRegionInfo.CultureCode.ToString();

                if (objNotificationDTO != null && objNotificationDTO.EmailTemplateDetail != null && objNotificationDTO.EmailTemplateDetail.lstEmailTemplateDtls != null && objNotificationDTO.EmailTemplateDetail.lstEmailTemplateDtls.Count() > 0)
                {
                    objEmailTemplateDetailDTO = objNotificationDTO.EmailTemplateDetail.lstEmailTemplateDtls.First();
                }
                if (objEmailTemplateDetailDTO == null)
                {
                    objEmailTemplateDetailDTO = new EmailTemplateDetailDTO();
                }

                string ResourceFilePath = GetResourceFileFullPath("ResRequisitionMaster", strCulture, objEnterprise.ID, objNotificationDTO.CompanyID);
                string htmlTabl = string.Empty;
                string CostTitleKey = (PriseSelectionOption == 1 ? "TotalPrice" : "TotalCost");
                CostTitle = ResourceRead.GetResourceValueByKeyAndFullFilePath(CostTitleKey, ResourceFilePath,objEnterprise.ID,objNotificationDTO.CompanyID,objNotificationDTO.RoomID, "ResRequisitionMaster",strCulture);
                htmlTabl = @"<table style=""margin-left: 0px; width: 99%;""  border=""1"" cellpadding=""0""
                            cellspacing=""0"">
                            <thead>
                                <tr role=""row"">
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResourceRead.GetResourceValueByKeyAndFullFilePath("RequisitionNumber", ResourceFilePath, objEnterprise.ID, objNotificationDTO.CompanyID, objNotificationDTO.RoomID, "ResRequisitionMaster", strCulture) + @"
                                    </th>
                                    <th  style=""width: 5%; text-align: left;"">
                                        " + ResourceRead.GetResourceValueByKeyAndFullFilePath("RequisitionType", ResourceFilePath, objEnterprise.ID, objNotificationDTO.CompanyID, objNotificationDTO.RoomID, "ResRequisitionMaster", strCulture) + @"
                                    </th>
                                    <th  style=""width: 15%; text-align: left;"">
                                        " + ResourceRead.GetResourceValueByKeyAndFullFilePath("WorkorderName", ResourceFilePath, objEnterprise.ID, objNotificationDTO.CompanyID, objNotificationDTO.RoomID, "ResRequisitionMaster", strCulture) + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResourceRead.GetResourceValueByKeyAndFullFilePath("RequiredDate", ResourceFilePath, objEnterprise.ID, objNotificationDTO.CompanyID, objNotificationDTO.RoomID, "ResRequisitionMaster", strCulture) + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResourceRead.GetResourceValueByKeyAndFullFilePath("RequisitionStatus", ResourceFilePath, objEnterprise.ID, objNotificationDTO.CompanyID, objNotificationDTO.RoomID, "ResRequisitionMaster", strCulture) + @"
                                    </th>
                                    <th  style=""width: 5%; text-align: left;"">
                                        No Of Items
                                    </th>
                                    <th  style=""width: 5%; text-align: left;"">
                                        " + CostTitle + @"
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                            ##TRS##
                            </tbody>
                        </table>";
                objDAL = new RequisitionMasterDAL(objEnterprise.EnterpriseDBName);
                objPendingOrders = objDAL.GetPendingRequisitions(objNotificationDTO.RoomID, objNotificationDTO.CompanyID);

                string trs = "";
                if (objPendingOrders != null && objPendingOrders.Count() > 0)
                {
                    int cntrow = 1;
                    foreach (var item in objPendingOrders)
                    {

                        string RowStyle = string.Empty;
                        if (cntrow % 2 == 0)
                        {
                            RowStyle = AlterNativeRowStyle;
                        }

                        trs += @"<tr " + RowStyle + @" >
                        <td>
                            " + item.RequisitionNumber + @"
                        </td>
                        <td>
                            " + (string.IsNullOrEmpty(item.RequisitionType) ? "&nbsp;" : item.RequisitionType) + @"
                        </td>
                        <td>
                            " + (string.IsNullOrEmpty(item.WorkorderName) ? "&nbsp;" : item.WorkorderName) + @"
                        </td>
                        <td>
                            " + (item.RequiredDate == null ? "&nbsp;" : item.RequiredDate.GetValueOrDefault(DateTime.MinValue).ToString(dateFormate)) + @"
                        </td>
                        <td>
                            " + (string.IsNullOrEmpty(item.RequisitionStatus) ? "&nbsp;" : item.RequisitionStatus) + @"
                        </td>
                        <td>
                            " + (item.RequisitionListItem != null && item.RequisitionListItem.Count > 0 ? item.RequisitionListItem.Count.ToString() : "&nbsp;") + @"
                        </td>
                        <td>                           
                            " + (Convert.ToString(PriseSelectionOption) == "1" ? item.TotalSellPrice.ToString(CostFormat) : item.TotalCost.GetValueOrDefault(0).ToString(CostFormat)) + @"
                        </td>
                    </tr>";
                        cntrow += 1;
                    }
                }
                else                //if Not Data Found 
                {
                    string ResourceFileCommon = GetResourceFileFullPath("ResCommon", strCulture, objEnterprise.ID, objNotificationDTO.CompanyID);
                    string Str = ResourceRead.GetResourceValueByKeyAndFullFilePath("msgNoDataFound", ResourceFileCommon, objEnterprise.ID, objNotificationDTO.CompanyID,objNotificationDTO.RoomID, "ResCommon", strCulture);
                    string RowStyle = string.Empty;
                    trs += @"<tr " + RowStyle + @" >
                        <td colspan=7>
                            " + Str + @"
                        </td>
                    </tr>";
                }
                htmlTabl = htmlTabl.Replace("##TRS##", trs);

                MessageBody = new StringBuilder();
                MessageBody.Append(objEmailTemplateDetailDTO.MailBodyText);
                MessageBody.Replace("@@COMPANYNAME@@", objNotificationDTO.CompanyName);
                MessageBody.Replace("@@ROOMNAME@@", objNotificationDTO.RoomName);
                MessageBody.Replace("@@TABLE@@", htmlTabl);

            }
            finally
            {
                objEmailTemplateDetailDTO = null;
                objDAL = null;
                objPendingOrders = null;
            }
            return MessageBody.ToString();

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

        private string SendMailForSuggestedOrder(NotificationDTO objNotificationDTO, EnterpriseDTO objEnterprise)
        {
            CartItemDAL objCartItemDAL = null;
            List<CartItemDTO> lstSuggestedOrder = null;
            StringBuilder MessageBody = null;
            EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;

            try
            {
                string QtyFormat = "N";
                string CostFormat = "N";
                string dateFormate = "MM/dd/yyyy";
                string RoomDateFormat = "M/d/yyyy";

                if (objNotificationDTO.objeTurnsRegionInfo.NumberDecimalDigits > 0)
                    QtyFormat += objNotificationDTO.objeTurnsRegionInfo.NumberDecimalDigits;

                if (objNotificationDTO.objeTurnsRegionInfo.CurrencyDecimalDigits > 0)
                    CostFormat += objNotificationDTO.objeTurnsRegionInfo.CurrencyDecimalDigits;

                if (!string.IsNullOrEmpty(objNotificationDTO.objeTurnsRegionInfo.ShortDatePattern) && !string.IsNullOrEmpty(objNotificationDTO.objeTurnsRegionInfo.ShortTimePattern))
                {
                    dateFormate = objNotificationDTO.objeTurnsRegionInfo.ShortDatePattern + " " + objNotificationDTO.objeTurnsRegionInfo.ShortTimePattern;
                    RoomDateFormat = objNotificationDTO.objeTurnsRegionInfo.ShortDatePattern;
                }

                if (objNotificationDTO != null && objNotificationDTO.EmailTemplateDetail != null && objNotificationDTO.EmailTemplateDetail.lstEmailTemplateDtls != null && objNotificationDTO.EmailTemplateDetail.lstEmailTemplateDtls.Count() > 0)
                {
                    objEmailTemplateDetailDTO = objNotificationDTO.EmailTemplateDetail.lstEmailTemplateDtls.First();
                }
                if (objEmailTemplateDetailDTO == null)
                {
                    objEmailTemplateDetailDTO = new EmailTemplateDetailDTO();
                }

                string strCulture = "en-US";

                if (!string.IsNullOrEmpty(objNotificationDTO.objeTurnsRegionInfo.CultureCode))
                    strCulture = objNotificationDTO.objeTurnsRegionInfo.CultureCode.ToString();

                string ResourceFilePath = GetResourceFileFullPath("ResItemMaster", strCulture, objEnterprise.ID, objNotificationDTO.CompanyID);

                string htmlTabl = string.Empty;
                htmlTabl = @"<table style=""margin-left: 0px; width: 99%;""  border=""1"" cellpadding=""0""
                            cellspacing=""0"">
                            <thead>
                                <tr role=""row"">
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResourceRead.GetResourceValueByKeyAndFullFilePath("ItemNumber", ResourceFilePath, objEnterprise.ID, objNotificationDTO.CompanyID,objNotificationDTO.RoomID, "ResItemMaster",strCulture) + @"
                                    </th>
                                    <th  style=""width: 5%; text-align: left;"">
                                        " + ResourceRead.GetResourceValueByKeyAndFullFilePath("SupplierName", ResourceFilePath, objEnterprise.ID, objNotificationDTO.CompanyID, objNotificationDTO.RoomID, "ResItemMaster", strCulture) + @"
                                    </th>
                                    <th  style=""width: 15%; text-align: left;"">
                                        " + ResourceRead.GetResourceValueByKeyAndFullFilePath("BinName", ResourceFilePath, objEnterprise.ID, objNotificationDTO.CompanyID, objNotificationDTO.RoomID, "ResItemMaster", strCulture) + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResourceRead.GetResourceValueByKeyAndFullFilePath("Quantity", ResourceFilePath, objEnterprise.ID, objNotificationDTO.CompanyID, objNotificationDTO.RoomID, "ResItemMaster", strCulture) + @"
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                            ##TRS##
                            </tbody>
                        </table>";
                int TotalRecordCount = 0;

                objCartItemDAL = new CartItemDAL(objEnterprise.EnterpriseDBName);
                var tmpsupplierIds = new List<long>();
                TimeZoneInfo CurrentTimeZone = TimeZoneInfo.Utc;
                if (objNotificationDTO != null)
                {
                    if (objNotificationDTO.objeTurnsRegionInfo != null && !string.IsNullOrWhiteSpace(objNotificationDTO.objeTurnsRegionInfo.TimeZoneName))
                    {
                        CurrentTimeZone = TimeZoneInfo.FindSystemTimeZoneById(objNotificationDTO.objeTurnsRegionInfo.TimeZoneName);
                    }
                    else
                    {
                        RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(objEnterprise.EnterpriseDBName);
                        eTurnsRegionInfo objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(objNotificationDTO.RoomID, objNotificationDTO.CompanyID, objNotificationDTO.UpdatedBy);
                        if (objeTurnsRegionInfo != null)
                        {
                            CurrentTimeZone = TimeZoneInfo.FindSystemTimeZoneById(objeTurnsRegionInfo.TimeZoneName);
                        }

                    }
                }
                lstSuggestedOrder = objCartItemDAL.GetPagedRecordsCartListFromDB(0, 100000, out TotalRecordCount, "", "ItemNumber asc", objNotificationDTO.RoomID, objNotificationDTO.CompanyID, false, false, "", tmpsupplierIds, true, "Purchase", RoomDateFormat, CurrentTimeZone);

                //Filter supplier wise list
                if (lstSuggestedOrder != null && lstSuggestedOrder.Count() > 0 && !string.IsNullOrEmpty(objNotificationDTO.SupplierIds))
                    lstSuggestedOrder = lstSuggestedOrder.Where(x => objNotificationDTO.SupplierIds.Contains(x.SupplierId.ToString())).ToList();//x.SupplierId != null && 

                string trs = "";
                if (lstSuggestedOrder != null && lstSuggestedOrder.Count() > 0)
                {
                    int cntrow = 1;
                    foreach (var item in lstSuggestedOrder)
                    {
                        string RowStyle = string.Empty;
                        if (cntrow % 2 == 0)
                        {
                            RowStyle = AlterNativeRowStyle;
                        }
                        trs += @"<tr " + RowStyle + @" >
                               <td>
                                   " + item.ItemNumber + @"
                               </td>
                               <td>
                                   " + (string.IsNullOrEmpty(item.SupplierName) ? "&nbsp;" : item.SupplierName) + @"
                               </td>
                               <td>
                                   " + (string.IsNullOrEmpty(item.BinName) ? "&nbsp;" : item.BinName) + @"
                               </td>
                               <td>
                                   " + item.Quantity.GetValueOrDefault(0) + @"
                               </td>
                             </tr>";
                        cntrow += 1;
                    }
                }
                else
                {
                    string ResourceFileCommon = GetResourceFileFullPath("ResCommon", strCulture, objEnterprise.ID, objNotificationDTO.CompanyID);
                    string Str = ResourceRead.GetResourceValueByKeyAndFullFilePath("msgNoDataFound", ResourceFileCommon,objEnterprise.ID, objNotificationDTO.CompanyID,objNotificationDTO.RoomID, "ResCommon", strCulture);
                    string RowStyle = string.Empty;
                    trs += @"<tr " + RowStyle + @" >
                        <td colspan=7>
                            " + Str + @"
                        </td>
                   </tr>";
                }

                htmlTabl = htmlTabl.Replace("##TRS##", trs);
                MessageBody = new StringBuilder();
                MessageBody.Append(objEmailTemplateDetailDTO.MailBodyText);
                MessageBody.Replace("@@TABLE@@", htmlTabl);
                MessageBody.Replace("@@COMPANYNAME@@", objNotificationDTO.CompanyName);
                MessageBody.Replace("@@ROOMNAME@@", objNotificationDTO.RoomName);

            }
            finally
            {
                objCartItemDAL = null;
                lstSuggestedOrder = null;
                objEmailTemplateDetailDTO = null;
            }
            return MessageBody.ToString();
        }

        private string SendMailForSuggestedOrdersCritical(NotificationDTO objNotificationDTO, EnterpriseDTO objEnterprise)
        {
            CartItemDAL objCartItemDAL = null;
            List<CartItemDTO> lstSuggestedOrder = null;
            StringBuilder MessageBody = null;
            EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;

            try
            {
                string QtyFormat = "N";
                string CostFormat = "N";
                string dateFormate = "MM/dd/yyyy";
                string RoomDateFormat = "M/d/yyyy";

                if (objNotificationDTO.objeTurnsRegionInfo.NumberDecimalDigits > 0)
                    QtyFormat += objNotificationDTO.objeTurnsRegionInfo.NumberDecimalDigits;

                if (objNotificationDTO.objeTurnsRegionInfo.CurrencyDecimalDigits > 0)
                    CostFormat += objNotificationDTO.objeTurnsRegionInfo.CurrencyDecimalDigits;

                if (!string.IsNullOrEmpty(objNotificationDTO.objeTurnsRegionInfo.ShortDatePattern) && !string.IsNullOrEmpty(objNotificationDTO.objeTurnsRegionInfo.ShortTimePattern))
                {
                    dateFormate = objNotificationDTO.objeTurnsRegionInfo.ShortDatePattern + " " + objNotificationDTO.objeTurnsRegionInfo.ShortTimePattern;
                    RoomDateFormat = objNotificationDTO.objeTurnsRegionInfo.ShortDatePattern;
                }

                if (objNotificationDTO != null && objNotificationDTO.EmailTemplateDetail != null && objNotificationDTO.EmailTemplateDetail.lstEmailTemplateDtls != null && objNotificationDTO.EmailTemplateDetail.lstEmailTemplateDtls.Count() > 0)
                {
                    objEmailTemplateDetailDTO = objNotificationDTO.EmailTemplateDetail.lstEmailTemplateDtls.First();
                }
                if (objEmailTemplateDetailDTO == null)
                {
                    objEmailTemplateDetailDTO = new EmailTemplateDetailDTO();
                }

                string strCulture = "en-US";

                if (!string.IsNullOrEmpty(objNotificationDTO.objeTurnsRegionInfo.CultureCode))
                    strCulture = objNotificationDTO.objeTurnsRegionInfo.CultureCode.ToString();

                string ResourceFilePath = GetResourceFileFullPath("ResItemMaster", strCulture, objEnterprise.ID, objNotificationDTO.CompanyID);

                string htmlTabl = string.Empty;
                htmlTabl = @"<table style=""margin-left: 0px; width: 99%;""  border=""1"" cellpadding=""0""
                            cellspacing=""0"">
                            <thead>
                                <tr role=""row"">
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResourceRead.GetResourceValueByKeyAndFullFilePath("ItemNumber", ResourceFilePath,objEnterprise.ID,objNotificationDTO.CompanyID,objNotificationDTO.RoomID, "ResItemMaster",strCulture) + @"
                                    </th>
                                    <th  style=""width: 5%; text-align: left;"">
                                        " + ResourceRead.GetResourceValueByKeyAndFullFilePath("SupplierName", ResourceFilePath) + @"
                                    </th>
                                    <th  style=""width: 15%; text-align: left;"">
                                        " + ResourceRead.GetResourceValueByKeyAndFullFilePath("BinName", ResourceFilePath, objEnterprise.ID, objNotificationDTO.CompanyID, objNotificationDTO.RoomID, "ResItemMaster", strCulture) + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResourceRead.GetResourceValueByKeyAndFullFilePath("Quantity", ResourceFilePath, objEnterprise.ID, objNotificationDTO.CompanyID, objNotificationDTO.RoomID, "ResItemMaster", strCulture) + @"
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                            ##TRS##
                            </tbody>
                        </table>";
                int TotalRecordCount = 0;

                objCartItemDAL = new CartItemDAL(objEnterprise.EnterpriseDBName);
                var tmpsupplierIds = new List<long>();
                TimeZoneInfo CurrentTimeZone = TimeZoneInfo.Utc;
                if (objNotificationDTO != null)
                {
                    if (objNotificationDTO.objeTurnsRegionInfo != null && !string.IsNullOrWhiteSpace(objNotificationDTO.objeTurnsRegionInfo.TimeZoneName))
                    {
                        CurrentTimeZone = TimeZoneInfo.FindSystemTimeZoneById(objNotificationDTO.objeTurnsRegionInfo.TimeZoneName);
                    }
                    else
                    {
                        RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(objEnterprise.EnterpriseDBName);
                        eTurnsRegionInfo objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(objNotificationDTO.RoomID, objNotificationDTO.CompanyID, objNotificationDTO.UpdatedBy);
                        if (objeTurnsRegionInfo != null)
                        {
                            CurrentTimeZone = TimeZoneInfo.FindSystemTimeZoneById(objeTurnsRegionInfo.TimeZoneName);
                        }

                    }
                }
                lstSuggestedOrder = objCartItemDAL.GetPagedRecordsCartListFromDB(0, 100000, out TotalRecordCount, "", "ItemNumber asc", objNotificationDTO.RoomID, objNotificationDTO.CompanyID, false, false, "", tmpsupplierIds, true, "Purchase", RoomDateFormat, CurrentTimeZone);

                //Filter supplier wise list
                if (lstSuggestedOrder != null && lstSuggestedOrder.Count() > 0 && !string.IsNullOrEmpty(objNotificationDTO.SupplierIds))
                    lstSuggestedOrder = lstSuggestedOrder.Where(x => objNotificationDTO.SupplierIds.Contains(x.SupplierId.ToString())).ToList();//x.SupplierId != null &&

                string trs = "";
                if (lstSuggestedOrder != null && lstSuggestedOrder.Count() > 0)
                {
                    lstSuggestedOrder = lstSuggestedOrder.Where(x => x.OnHandQuantity < x.CriticalQuantity).ToList();

                    int cntrow = 1;
                    foreach (var item in lstSuggestedOrder)
                    {
                        string RowStyle = string.Empty;
                        if (cntrow % 2 == 0)
                        {
                            RowStyle = AlterNativeRowStyle;
                        }
                        trs += @"<tr " + RowStyle + @" >
                                   <td>
                                       " + item.ItemNumber + @"
                                   </td>
                                   <td>
                                       " + (string.IsNullOrEmpty(item.SupplierName) ? "&nbsp;" : item.SupplierName) + @"
                                   </td>
                                   <td>
                                       " + (string.IsNullOrEmpty(item.BinName) ? "&nbsp;" : item.BinName) + @"
                                   </td>
                                   <td>
                                       " + item.Quantity.GetValueOrDefault(0) + @"
                                   </td>
                                 </tr>";
                        cntrow += 1;
                    }


                }
                //if Not Data Found 
                if (lstSuggestedOrder != null && lstSuggestedOrder.Count() <= 0)
                {
                    string ResourceFileCommon = GetResourceFileFullPath("ResCommon", strCulture, objEnterprise.ID, objNotificationDTO.CompanyID);
                    string Str = ResourceRead.GetResourceValueByKeyAndFullFilePath("msgNoDataFound", ResourceFileCommon,objEnterprise.ID,objNotificationDTO.CompanyID,objNotificationDTO.RoomID, "ResCommon",strCulture);
                    string RowStyle = string.Empty;
                    trs += @"<tr " + RowStyle + @" >
                        <td colspan=7>
                            " + Str + @"
                        </td>
                    </tr>";
                }

                htmlTabl = htmlTabl.Replace("##TRS##", trs);
                MessageBody = new StringBuilder();
                MessageBody.Append(objEmailTemplateDetailDTO.MailBodyText);
                MessageBody.Replace("@@TABLE@@", htmlTabl);
                MessageBody.Replace("@@COMPANYNAME@@", objNotificationDTO.CompanyName);
                MessageBody.Replace("@@ROOMNAME@@", objNotificationDTO.RoomName);
            }
            finally
            {
                objCartItemDAL = null;
                lstSuggestedOrder = null;
                objEmailTemplateDetailDTO = null;
            }
            return MessageBody.ToString();
        }

        private string SendMailForSuggestedOrdersMinimum(NotificationDTO objNotificationDTO, EnterpriseDTO objEnterprise)
        {
            CartItemDAL objCartItemDAL = null;
            List<CartItemDTO> lstSuggestedOrder = null;
            StringBuilder MessageBody = null;
            EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;

            try
            {
                string QtyFormat = "N";
                string CostFormat = "N";
                string dateFormate = "MM/dd/yyyy";
                string RoomDateFormat = "M/d/yyyy";

                if (objNotificationDTO.objeTurnsRegionInfo.NumberDecimalDigits > 0)
                    QtyFormat += objNotificationDTO.objeTurnsRegionInfo.NumberDecimalDigits;

                if (objNotificationDTO.objeTurnsRegionInfo.CurrencyDecimalDigits > 0)
                    CostFormat += objNotificationDTO.objeTurnsRegionInfo.CurrencyDecimalDigits;

                if (!string.IsNullOrEmpty(objNotificationDTO.objeTurnsRegionInfo.ShortDatePattern) && !string.IsNullOrEmpty(objNotificationDTO.objeTurnsRegionInfo.ShortTimePattern))
                {
                    dateFormate = objNotificationDTO.objeTurnsRegionInfo.ShortDatePattern + " " + objNotificationDTO.objeTurnsRegionInfo.ShortTimePattern;
                    RoomDateFormat = objNotificationDTO.objeTurnsRegionInfo.ShortDatePattern;
                }

                if (objNotificationDTO != null && objNotificationDTO.EmailTemplateDetail != null && objNotificationDTO.EmailTemplateDetail.lstEmailTemplateDtls != null && objNotificationDTO.EmailTemplateDetail.lstEmailTemplateDtls.Count() > 0)
                {
                    objEmailTemplateDetailDTO = objNotificationDTO.EmailTemplateDetail.lstEmailTemplateDtls.First();
                }
                if (objEmailTemplateDetailDTO == null)
                {
                    objEmailTemplateDetailDTO = new EmailTemplateDetailDTO();
                }

                string strCulture = "en-US";

                if (!string.IsNullOrEmpty(objNotificationDTO.objeTurnsRegionInfo.CultureCode))
                    strCulture = objNotificationDTO.objeTurnsRegionInfo.CultureCode.ToString();

                string ResourceFilePath = GetResourceFileFullPath("ResItemMaster", strCulture, objEnterprise.ID, objNotificationDTO.CompanyID);

                string htmlTabl = string.Empty;
                htmlTabl = @"<table style=""margin-left: 0px; width: 99%;""  border=""1"" cellpadding=""0""
                            cellspacing=""0"">
                            <thead>
                                <tr role=""row"">
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResourceRead.GetResourceValueByKeyAndFullFilePath("ItemNumber", ResourceFilePath,objEnterprise.ID,objNotificationDTO.CompanyID,objNotificationDTO.RoomID, "ResItemMaster",strCulture) + @"
                                    </th>
                                    <th  style=""width: 5%; text-align: left;"">
                                        " + ResourceRead.GetResourceValueByKeyAndFullFilePath("SupplierName", ResourceFilePath, objEnterprise.ID, objNotificationDTO.CompanyID, objNotificationDTO.RoomID, "ResItemMaster", strCulture) + @"
                                    </th>
                                    <th  style=""width: 15%; text-align: left;"">
                                        " + ResourceRead.GetResourceValueByKeyAndFullFilePath("BinName", ResourceFilePath, objEnterprise.ID, objNotificationDTO.CompanyID, objNotificationDTO.RoomID, "ResItemMaster", strCulture) + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResourceRead.GetResourceValueByKeyAndFullFilePath("Quantity", ResourceFilePath, objEnterprise.ID, objNotificationDTO.CompanyID, objNotificationDTO.RoomID, "ResItemMaster", strCulture) + @"
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                            ##TRS##
                            </tbody>
                        </table>";
                int TotalRecordCount = 0;

                objCartItemDAL = new CartItemDAL(objEnterprise.EnterpriseDBName);
                var tmpsupplierIds = new List<long>();
                TimeZoneInfo CurrentTimeZone = TimeZoneInfo.Utc;
                if (objNotificationDTO != null)
                {
                    if (objNotificationDTO.objeTurnsRegionInfo != null && !string.IsNullOrWhiteSpace(objNotificationDTO.objeTurnsRegionInfo.TimeZoneName))
                    {
                        CurrentTimeZone = TimeZoneInfo.FindSystemTimeZoneById(objNotificationDTO.objeTurnsRegionInfo.TimeZoneName);
                    }
                    else
                    {
                        RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(objEnterprise.EnterpriseDBName);
                        eTurnsRegionInfo objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(objNotificationDTO.RoomID, objNotificationDTO.CompanyID, objNotificationDTO.UpdatedBy);
                        if (objeTurnsRegionInfo != null)
                        {
                            CurrentTimeZone = TimeZoneInfo.FindSystemTimeZoneById(objeTurnsRegionInfo.TimeZoneName);
                        }

                    }
                }
                lstSuggestedOrder = objCartItemDAL.GetPagedRecordsCartListFromDB(0, 100000, out TotalRecordCount, "", "ItemNumber asc", objNotificationDTO.RoomID, objNotificationDTO.CompanyID, false, false, "", tmpsupplierIds, true, "Purchase", RoomDateFormat, CurrentTimeZone);

                //Filter supplier wise list
                if (lstSuggestedOrder != null && lstSuggestedOrder.Count() > 0 && !string.IsNullOrEmpty(objNotificationDTO.SupplierIds))
                    lstSuggestedOrder = lstSuggestedOrder.Where(x => objNotificationDTO.SupplierIds.Contains(x.SupplierId.ToString())).ToList();//x.SupplierId != null &&

                string trs = "";
                if (lstSuggestedOrder != null && lstSuggestedOrder.Count() > 0)
                {
                    lstSuggestedOrder = lstSuggestedOrder.Where(x => x.OnHandQuantity <= x.MinimumQuantity).ToList();

                    int cntrow = 1;
                    foreach (var item in lstSuggestedOrder)
                    {
                        string RowStyle = string.Empty;
                        if (cntrow % 2 == 0)
                        {
                            RowStyle = AlterNativeRowStyle;
                        }
                        trs += @"<tr " + RowStyle + @" >
                                   <td>
                                       " + item.ItemNumber + @"
                                   </td>
                                   <td>
                                       " + (string.IsNullOrEmpty(item.SupplierName) ? "&nbsp;" : item.SupplierName) + @"
                                   </td>
                                   <td>
                                       " + (string.IsNullOrEmpty(item.BinName) ? "&nbsp;" : item.BinName) + @"
                                   </td>
                                   <td>
                                       " + item.Quantity.GetValueOrDefault(0) + @"
                                   </td>
                                 </tr>";
                        cntrow += 1;
                    }
                }
                //if Not Data Found 
                if (lstSuggestedOrder != null && lstSuggestedOrder.Count() <= 0)
                {
                    string ResourceFileCommon = GetResourceFileFullPath("ResCommon", strCulture, objEnterprise.ID, objNotificationDTO.CompanyID);
                    string Str = ResourceRead.GetResourceValueByKeyAndFullFilePath("msgNoDataFound", ResourceFileCommon,objEnterprise.ID, objNotificationDTO.CompanyID,objNotificationDTO.RoomID, "ResCommon", strCulture
                        );
                    string RowStyle = string.Empty;
                    trs += @"<tr " + RowStyle + @" >
                        <td colspan=7>
                            " + Str + @"
                        </td>
                    </tr>";
                }
                htmlTabl = htmlTabl.Replace("##TRS##", trs);
                MessageBody = new StringBuilder();
                MessageBody.Append(objEmailTemplateDetailDTO.MailBodyText);
                MessageBody.Replace("@@TABLE@@", htmlTabl);
                MessageBody.Replace("@@COMPANYNAME@@", objNotificationDTO.CompanyName);
                MessageBody.Replace("@@ROOMNAME@@", objNotificationDTO.RoomName);
            }
            finally
            {
                objCartItemDAL = null;
                lstSuggestedOrder = null;
                objEmailTemplateDetailDTO = null;
            }
            return MessageBody.ToString();
        }


        #endregion

        /// <summary>
        /// GetFooterForSignature
        /// </summary>
        /// <param name="doc1"></param>
        /// <param name="objRPTDTO"></param>
        /// <returns></returns>
        public XDocument GetFooterForSignature(XDocument doc1, ReportBuilderDTO objRPTDTO)
        {
            XDocument doc = new XDocument(doc1);

            string strHidden = "=IIf(Globals.PageNumber<Globals.TotalPages,true,false)";
            //string strLeft = "5.12in";

            if (objRPTDTO.ReportType == 2)
            {
                strHidden = "false";
            }

            string rptWidth = string.Empty;
            if (doc.Descendants(ns + "Page").Descendants(ns + "PageWidth").FirstOrDefault() != null)
            {
                rptWidth = doc.Descendants(ns + "Page").Descendants(ns + "PageWidth").FirstOrDefault().Value;
            }

            rptWidth = rptWidth.Replace("in", "");
            double dblPageWith = 0;

            double.TryParse(rptWidth, out dblPageWith);

            //if (dblPageWith > 8.5)
            //{
            //    strLeft = "7.12in";
            //}


            XElement element = new XElement(ns + "PageFooter", new XElement(ns + "Height", "0.80in")
                                                                , new XElement(ns + "PrintOnFirstPage", "true")
                                                                , new XElement(ns + "PrintOnLastPage", "true"),
                               new XElement(ns + "ReportItems",
                               new XElement(ns + "Rectangle", new XAttribute("Name", "RectSig"),
                               new XElement(ns + "ReportItems",
                               new XElement(ns + "Textbox", new XAttribute("Name", "txtSign"), new XElement(ns + "CanGrow", "true"),
                               new XElement(ns + "KeepTogether", "true"),
                               new XElement(ns + "Paragraphs", new XElement(ns + "Paragraph",
                               //new XElement(ns + "TextRuns", new XElement(ns + "TextRun", new XElement(ns + "Value", "(Signature)"),
                               new XElement(ns + "TextRuns", new XElement(ns + "TextRun", new XElement(ns + "Value", GetResourceValueForAlert(objRPTDTO.MasterReportResFile, "Signature")),
                               new XElement(ns + "Style", new XElement(ns + "FontStyle", "Normal")
                                                        , new XElement(ns + "FontFamily", "Times New Roman")
                                                        , new XElement(ns + "FontSize", "12pt")
                                                        , new XElement(ns + "FontWeight", "Normal")
                                                        , new XElement(ns + "TextDecoration", "None")
                                                        , new XElement(ns + "Color", "#000000")))),
                            new XElement(ns + "Style", new XElement(ns + "TextAlign", "Center")))),
                            new XElement(nsrd + "DefaultName", "txtSign"),
                            new XElement(ns + "Top", "0.35in"),
                            new XElement(ns + "Height", "0.25in"),
                            new XElement(ns + "Width", "2.50in"),
                            new XElement(ns + "Style", new XElement(ns + "Border", new XElement(ns + "Style", "None")),
                             new XElement(ns + "PaddingLeft", "2pt"),
                             new XElement(ns + "PaddingRight", "2pt"),
                              new XElement(ns + "PaddingTop", "2pt"),
                               new XElement(ns + "PaddingBottom", "2pt"))),
                               new XElement(ns + "Line", new XAttribute("Name", "lineSign"),
                                new XElement(ns + "Top", "0.33in"),
                            new XElement(ns + "Left", "0.0040in"),
                            new XElement(ns + "Height", "0in"),
                            new XElement(ns + "Width", "2.50in"),
                            new XElement(ns + "ZIndex", "1"),
                             new XElement(ns + "Style", new XElement(ns + "Border", new XElement(ns + "Style", "Solid"))))),
                                 new XElement(ns + "KeepTogether", "true"),
                                   new XElement(ns + "Top", "0.070in"),
                            new XElement(ns + "Left", "0.2in"),
                            new XElement(ns + "Height", "0.60in"),
                            new XElement(ns + "Width", "2.50in"),
                             new XElement(ns + "Visibility", new XElement(ns + "Hidden", strHidden)),
                              new XElement(ns + "Style", new XElement(ns + "Border", new XElement(ns + "Style", "None"))))),
                                new XElement(ns + "Style", new XElement(ns + "Border", new XElement(ns + "Style", "None"))));

            doc.Descendants(ns + "PageFooter").Remove();
            doc.Descendants(ns + "Page").FirstOrDefault().Add(element);

            return doc;
        }

        public XDocument GetAdditionalHeaderRow(XDocument doc1, ReportBuilderDTO objRPTDTO, string CompanyName, string RoomName, Dictionary<string, string> ReportPara = null, string EnterpriseDBName = null, Int64 NotificationID = 0, Int64 EnterPriseID = 0, Int64 CompanyID = 0, Int64 RoomID = 0, eTurnsRegionInfo RegionInfo = null)
        {
            XDocument doc = new XDocument(doc1);
            XElement xNonDeletedRow = doc.Descendants(ns + "TablixRow").FirstOrDefault();
            XElement xRows = doc.Descendants(ns + "TablixRows").FirstOrDefault();
            XElement xRow = doc.Descendants(ns + "TablixRow").FirstOrDefault();
            if (objRPTDTO.ReportType == 2 && xRow.Descendants(ns + "TablixCell").Descendants(ns + "Textbox").
                        FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "TextboxRoom") != null)
            {
                doc.Descendants(ns + "TablixRow").FirstOrDefault().Remove();
                doc.Descendants(ns + "TablixRowHierarchy").Descendants(ns + "TablixMembers").Descendants(ns + "TablixMember").Descendants(ns + "TablixMembers").Descendants(ns + "TablixMember").Remove();
                foreach (var item in xRows.Descendants(ns + "TablixRow"))
                {
                    doc.Descendants(ns + "TablixRowHierarchy").Descendants(ns + "TablixMembers").Descendants(ns + "TablixMember").Descendants(ns + "TablixMembers").FirstOrDefault().Add(new XElement(ns + "TablixMember"));
                }
            }
            xRow = doc.Descendants(ns + "TablixRow").FirstOrDefault();
            int TotalCells = xRow.Descendants(ns + "TablixCell").Count();

            string rptTitle = "";
            XElement xElerptTitle = doc.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Textbox").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "ReportTitle");
            if (xElerptTitle == null)
            {
                xElerptTitle = doc.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Textbox").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "TextboxWO");
            }

            if (xElerptTitle != null)
            {
                rptTitle = xElerptTitle.Descendants(ns + "Value").FirstOrDefault().Value;
            }

            XElement xNewRow = new XElement(xRow);
            xRows.AddFirst(xNewRow);
            xNewRow = null;
            xNewRow = doc.Descendants(ns + "TablixRow").FirstOrDefault();
            xNewRow.Descendants(ns + "Height").FirstOrDefault().Value = xNonDeletedRow.Descendants(ns + "Height").FirstOrDefault().Value;
            IEnumerable<XElement> lstXnew = xNewRow.Descendants(ns + "TablixCell");

            string txtAttrName = xNewRow.Descendants(ns + "Textbox").FirstOrDefault().FirstAttribute.Value;

            txtAttrName = txtAttrName + "_1";
            xNewRow.Descendants(ns + "Textbox").FirstOrDefault().FirstAttribute.SetValue(txtAttrName);
            if (xNewRow.Descendants(ns + "Textbox").FirstOrDefault().Descendants(ns + "TextAlign").FirstOrDefault() != null)
                xNewRow.Descendants(ns + "Textbox").FirstOrDefault().Descendants(ns + "TextAlign").FirstOrDefault().Value = "Left";

            xNewRow.Descendants(nsrd + "DefaultName").FirstOrDefault().Value = txtAttrName;
            XElement FirstTextBoxValue = lstXnew.FirstOrDefault().Descendants(ns + "TextRun").FirstOrDefault().Descendants(ns + "Value").FirstOrDefault();

            string[] arrRooms = null;

            if (ReportPara != null && ReportPara.Keys.Contains("RoomIDs"))
            {
                string strRoomIDs = ReportPara["RoomIDs"];
                arrRooms = strRoomIDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
            string strVal = @"=First(Fields!CurrentDateTime.Value, ""DataSet1"") + " + @"""     " + CompanyName + @""" + " + @"""     " + RoomName + @"""";

            string ReportParentName = string.Empty;
            ReportMasterDAL objReportDAL = new ReportMasterDAL(EnterpriseDBName);
            ReportBuilderDTO oReportDTO = objReportDAL.GetParentReportMasterByReportID(objRPTDTO.ID);
            if (oReportDTO != null && !string.IsNullOrEmpty(oReportDTO.ReportName))
            {
                ReportParentName = oReportDTO.ReportName.ToLower();
            }
            if(ReportParentName.Equals("Order Summary LineItem".ToLower())
                && rptTitle.ToLower().Contains("=IIF".ToLower()))
            {
                strVal = strVal + @"     " + rptTitle.Replace("=IIF", "+ IIF");
            }
            else
            {
                if(!strVal.EndsWith("+"))
                {
                    strVal = strVal + "+";
                }
                strVal = strVal + @"""     " + rptTitle + @"""";
            }
            if (objRPTDTO.ReportType != 2 && arrRooms != null && arrRooms.Length > 1)
            {
                if (ReportParentName.Equals("Order Summary LineItem".ToLower())
                    && rptTitle.ToLower().Contains("=IIF".ToLower()))
                {
                    strVal = @"=First(Fields!CurrentDateTime.Value, ""DataSet1"") + " + @"";
                    strVal = strVal + @"     " +  rptTitle.Replace("=IIF", "IIF");
                }
                else
                {
                    strVal = @"=First(Fields!CurrentDateTime.Value, ""DataSet1"") + " + @"""     " + rptTitle + @"""";
                }
            }

            if (ReportPara != null)
            {
                if (ReportPara.ContainsKey("OrigStartDate") && !string.IsNullOrEmpty(ReportPara["OrigStartDate"]))
                {
                    strVal = strVal + @"+""        Start: " + ReportPara["OrigStartDate"] + @"""";
                }

                if (ReportPara.ContainsKey("OrigEndDate") && !string.IsNullOrEmpty(ReportPara["OrigEndDate"]))
                {
                    strVal = strVal + @"  +""   To: " + ReportPara["OrigEndDate"] + @"""";
                }
            }

            if (Convert.ToInt64(NotificationID) > 0 && RegionInfo != null)
            {
                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(EnterpriseDBName);
                ReportMailLogDTO objReportMailLogDTO = new ReportMailLogDTO();
                objReportMailLogDTO = objReportMasterDAL.GetLastRunDateforSchedule(Convert.ToInt64(NotificationID), Convert.ToInt64(CompanyID), Convert.ToInt64(RoomID));
                if (objReportMailLogDTO != null)
                {
                    strVal = strVal + @"+""        Last Run Date : " + DateTimeUtility.ConvertDateByTimeZone(objReportMailLogDTO.SendDate, RegionInfo.TimeZoneName, RegionInfo.LongDatePattern, RegionInfo.CultureCode, true) + @"""";
                }
                else
                {
                    strVal = strVal + @"+""        Last Run Date : " + DateTimeUtility.ConvertDateByTimeZone(DateTime.UtcNow, RegionInfo.TimeZoneName, RegionInfo.LongDatePattern, RegionInfo.CultureCode, true) + @"""";
                }
            }

            if (objRPTDTO.ReportType != 2 && arrRooms != null && arrRooms.Length > 1)
            {
                // string strRoomCompany = @"";
                RoomDAL rDAL = null;
                if (EnterpriseDBName != null && (!string.IsNullOrWhiteSpace(EnterpriseDBName)))
                {
                    rDAL = new RoomDAL(EnterpriseDBName);
                }
                else
                {
                    rDAL = new RoomDAL("eTurns");
                }

                IEnumerable<RoomDTO> RoomDTO = rDAL.GetRoomByIDsNormal(ReportPara["RoomIDs"]);
                IEnumerable<string> lstCompanies = RoomDTO.Select(x => x.CompanyName).Distinct();
                IEnumerable<string> lstRooms = RoomDTO.Select(x => x.RoomName).Distinct();

                string strCompanies = string.Join(", ", lstCompanies);
                string strRooms = string.Join(", ", lstRooms);
                strVal = strVal + @"+ VbCrLf +""" + strCompanies + @"""";
                strVal = strVal + @"+ VbCrLf +""" + strRooms + @"""";

                //foreach (var item in RoomDTO)
                //{
                //    if (strRoomCompany.Length > 0)
                //        strRoomCompany += ", ";

                //    strRoomCompany += item.CompanyName + "\t" + item.RoomName;
                //}
                //strVal = strVal + @"+ VbCrLf +""" + System.Environment.NewLine + strRoomCompany + @"""";
            }


            FirstTextBoxValue.Value = strVal;// + ""      "" + First(Fields!CompanyName.Value, ""DataSet1"") + ""      "" + First(Fields!RoomName.Value, ""DataSet1"") + " + @"""     " + rptTitle + @"""";

            lstXnew.FirstOrDefault().Descendants(ns + "BackgroundColor").Remove();
            if (lstXnew.FirstOrDefault().Descendants(ns + "CellContents").Descendants(ns + "ColSpan").FirstOrDefault() == null)
            {
                lstXnew.FirstOrDefault().Descendants(ns + "CellContents").FirstOrDefault().Add(new XElement(ns + "ColSpan", TotalCells.ToString()));
            }

            lstXnew.ToList().ForEach(x =>
            {
                if (x.Descendants(ns + "ColSpan").Count() <= 0)
                {
                    x.Remove();
                }
            });

            for (int i = 1; i < TotalCells; i++)
            {
                doc.Descendants(ns + "TablixCells").FirstOrDefault().Add(new XElement(ns + "TablixCell"));
            }

            XElement xRowHira = doc.Descendants(ns + "TablixRowHierarchy").FirstOrDefault();
            XElement xTablixMember = xRowHira.Descendants(ns + "TablixMember").FirstOrDefault();
            XElement xTablixMembers = xRowHira.Descendants(ns + "TablixMembers").FirstOrDefault();
            if (objRPTDTO.ReportType == 3)
            {
                //txtAttrName = xTablixMember.Descendants(ns + "Textbox").FirstOrDefault().FirstAttribute.Value;
                txtAttrName = txtAttrName + "_2";
                xTablixMember.Descendants(ns + "Textbox").FirstOrDefault().FirstAttribute.SetValue(txtAttrName);
                xTablixMember.Descendants(nsrd + "DefaultName").FirstOrDefault().Value = txtAttrName;
                xTablixMember.Descendants(ns + "Textbox").FirstOrDefault().Descendants(ns + "Value").FirstOrDefault().Value = string.Empty;
                XElement newTablixMember = new XElement(xTablixMember);
                xTablixMembers.AddFirst(newTablixMember);

                newTablixMember = null;
                xRowHira = doc.Descendants(ns + "TablixRowHierarchy").FirstOrDefault();
                newTablixMember = xRowHira.Descendants(ns + "TablixMember").FirstOrDefault();
                txtAttrName = txtAttrName + "_2";
                newTablixMember.Descendants(ns + "Textbox").FirstOrDefault().FirstAttribute.SetValue(txtAttrName);
                newTablixMember.Descendants(nsrd + "DefaultName").FirstOrDefault().Value = txtAttrName;
                newTablixMember.Descendants(ns + "Textbox").FirstOrDefault().Descendants(ns + "Value").FirstOrDefault().Value = string.Empty;

                IEnumerable<XElement> textboxs = xRowHira.Descendants(ns + "Textbox");

                int indx = 101;

                foreach (var item in textboxs)
                {
                    string txtname = item.FirstAttribute.Value;
                    string txtnewname = txtname + "_" + indx.ToString();
                    item.FirstAttribute.SetValue(txtnewname);

                    XElement txtDefaultName = xRowHira.Descendants(nsrd + "DefaultName").Where(x => x.Value == txtname).FirstOrDefault();
                    txtDefaultName.Value = txtnewname;
                    indx += 1;
                }
            }
            else if (objRPTDTO.ReportType == 1)
            {
                XElement newTablixMember = new XElement(xTablixMember);
                xTablixMembers.AddFirst(newTablixMember);
            }
            else if (objRPTDTO.ReportType == 2)
            {
                doc.Descendants(ns + "TablixRowHierarchy").Descendants(ns + "TablixMembers").Descendants(ns + "TablixMember").Descendants(ns + "TablixMembers").FirstOrDefault().Add(new XElement(ns + "TablixMember"));
            }
            doc.Descendants(ns + "PageHeader").Remove();
            return doc;
        }

        public string GetTablixRowHierarchyForWithouGroupReport()
        {
            return @"<TablixMembers>
                            <TablixMember>
                              <TablixHeader>
                                <Size>0.01in</Size>
                                <CellContents>
                                  <Textbox Name=""txtRoomInfoGroupHeader"">
                                    <CanGrow>false</CanGrow>
                                    <KeepTogether>true</KeepTogether>
                                    <Paragraphs>
                                      <Paragraph>
                                        <TextRuns>
                                          <TextRun>
                                            <Value />
                                          </TextRun>
                                        </TextRuns>
                                      </Paragraph>
                                    </Paragraphs>
                                     <Visibility>
                                        <Hidden>true</Hidden>
                                    </Visibility>
                                    <Style />
                                  </Textbox>
                                </CellContents>
                              </TablixHeader>
                              <TablixMembers>
                                <TablixMember />
                              </TablixMembers>
                              <KeepWithGroup>After</KeepWithGroup>
                              <RepeatOnNewPage>true</RepeatOnNewPage>
                            </TablixMember>
                            <TablixMember>
                              <Group Name=""RoomGroup"">
                                <GroupExpressions>
                                  <GroupExpression>=Fields!RoomID.Value</GroupExpression>
                                </GroupExpressions>
                                <PageBreak>
                                  <BreakLocation>Between</BreakLocation>
                                </PageBreak>
                              </Group>
                              <TablixHeader>
                                <Size>0.01in</Size>
                                <CellContents>
                                  <Textbox Name=""txtRoomInfo"">
                                    <CanGrow>false</CanGrow>
                                    <KeepTogether>true</KeepTogether>
                                    <Paragraphs>
                                      <Paragraph>
                                        <TextRuns>
                                          <TextRun>
                                            <Value>=Fields!RoomInfo.Value</Value>
                                          </TextRun>
                                        </TextRuns>
                                      </Paragraph>
                                    </Paragraphs>
                                    <Visibility>
                                        <Hidden>true</Hidden>
                                    </Visibility>
                                    <Style />
                                  </Textbox>
                                  <rd:Selected>true</rd:Selected>
                                </CellContents>
                              </TablixHeader>
                              <TablixMembers>
                                <TablixMember>
                                  <Group Name=""Details"" />
                                </TablixMember>
                              </TablixMembers>
                            </TablixMember>
                          </TablixMembers>";
        }

        public string GetTablixRowHierarchyForSingleGroup(string BaseGroupName, bool hasGrandTotal)
        {
            string strReturn = @"<TablixMembers>
                                    <TablixMember>
                                      <TablixHeader>
                                        <Size>0.01in</Size>
                                        <CellContents>
                                          <Textbox Name=""txtRoomInfoGroupHeader"">
                                            <CanGrow>true</CanGrow>
                                            <KeepTogether>true</KeepTogether>
                                            <Paragraphs>
                                              <Paragraph>
                                                <TextRuns>
                                                  <TextRun>
                                                    <Value />
                                                  </TextRun>
                                                </TextRuns>
                                              </Paragraph>
                                            </Paragraphs>
                                            <Visibility>
                                                <Hidden>true</Hidden>
                                            </Visibility>
                                           <Style />
                                          </Textbox>
                                        </CellContents>
                                      </TablixHeader>
                                      <TablixMembers>
                                        <TablixMember />
                                      </TablixMembers>
                                    </TablixMember>
                                    <TablixMember>
                                      <Group Name=""RoomGroup"">
                                        <GroupExpressions>
                                          <GroupExpression>=Fields!RoomID.Value</GroupExpression>
                                        </GroupExpressions>
                                        <PageBreak>
                                          <BreakLocation>Between</BreakLocation>
                                        </PageBreak>
                                      </Group>
                                      <TablixHeader>
                                        <Size>0.01in</Size>
                                        <CellContents>
                                          <Textbox Name=""txtRoomInfo"">
                                            <CanGrow>true</CanGrow>
                                            <KeepTogether>true</KeepTogether>
                                            <Paragraphs>
                                              <Paragraph>
                                                <TextRuns>
                                                  <TextRun>
                                                    <Value>=Fields!RoomInfo.Value</Value>
                                                  </TextRun>
                                                </TextRuns>
                                              </Paragraph>
                                            </Paragraphs>
                                            <rd:DefaultName>txtRoomInfo</rd:DefaultName>
                                            <Visibility>
                                                <Hidden>true</Hidden>
                                            </Visibility>
                                           <Style />
                                          </Textbox>
                                        </CellContents>
                                      </TablixHeader>
                                      <TablixMembers>
                                        <TablixMember>
                                          <Group Name=""Group1"">
                                            <GroupExpressions>
                                              <GroupExpression>=Fields!" + BaseGroupName + @".Value</GroupExpression>
                                            </GroupExpressions>
                                          </Group>
                                          <TablixMembers>
                                            <TablixMember>
                                              <Group Name=""Details"" />
                                                <TablixMembers>
                                                  <TablixMember />
                                                </TablixMembers>
                                            </TablixMember>
                                          </TablixMembers>
                                        </TablixMember>
                                      </TablixMembers>
                                    </TablixMember>";

            if (hasGrandTotal)
            {
                strReturn = strReturn + @"<TablixMember>
                                              <TablixHeader>
                                                <Size>0.01in</Size>
                                                <CellContents>
                                                  <Textbox Name=""txtRoomInfoGT"">
                                                    <CanGrow>true</CanGrow>
                                                    <KeepTogether>true</KeepTogether>
                                                    <Paragraphs>
                                                      <Paragraph>
                                                        <TextRuns>
                                                          <TextRun>
                                                            <Value />                            
                                                          </TextRun>
                                                        </TextRuns>                        
                                                      </Paragraph>
                                                    </Paragraphs>
                                                    <Visibility>
                                                        <Hidden>true</Hidden>
                                                    </Visibility>
                                                   <Style />
                                                    <rd:DefaultName>txtRoomInfoGT</rd:DefaultName>                   
                                                  </Textbox>
                                                </CellContents>
                                              </TablixHeader>
                                              <TablixMembers>
                                                <TablixMember />
                                              </TablixMembers>
                                              <KeepWithGroup>Before</KeepWithGroup>
                                            </TablixMember>";
            }

            strReturn = strReturn + @"</TablixMembers>";
            //</TablixRowHierarchy>";
            return strReturn;
        }

        public string GetTablixRowHierarchy(string HearderElement, string GroupElement, string Footer, bool IsGroupExist)
        {
            string strBaseHeararchyy = string.Empty;
            if (IsGroupExist)
            {
                strBaseHeararchyy = GetBaseTablixRowHierarchy();
                if (!string.IsNullOrEmpty(Footer))
                    strBaseHeararchyy = strBaseHeararchyy.Replace("{{REPLACE FOOTER}}", "<TablixMembers>" + Footer + @"</TablixMembers>");
                else
                {
                    strBaseHeararchyy = GetBaseTablixRowHierarchyWithoutFooter();
                    strBaseHeararchyy = strBaseHeararchyy.Replace("{{REPLACE FOOTER}}", "");
                }

                if (!string.IsNullOrEmpty(HearderElement))
                    strBaseHeararchyy = strBaseHeararchyy.Replace("{{REPLACE HEADER}}", "<TablixMembers>" + HearderElement + "</TablixMembers>");
                else
                    strBaseHeararchyy = strBaseHeararchyy.Replace("{{REPLACE HEADER}}", "");
                if (!string.IsNullOrEmpty(GroupElement))
                    strBaseHeararchyy = strBaseHeararchyy.Replace("{{REPLACE GROUP}}", "<TablixMembers>" + GroupElement + @"</TablixMembers>");
                else
                    strBaseHeararchyy = strBaseHeararchyy.Replace("{{REPLACE GROUP}}", "");

            }
            else
            {
                strBaseHeararchyy = GetBaseTablixRowHierarchyWithoutFooter();
                strBaseHeararchyy = strBaseHeararchyy.Replace("{{REPLACE HEADER}}", "<TablixMembers><TablixMember/></TablixMembers>");
                strBaseHeararchyy = strBaseHeararchyy.Replace("{{REPLACE GROUP}}", @"<TablixMembers><TablixMember><Group Name=""Details""/></TablixMember></TablixMembers>");
                strBaseHeararchyy = strBaseHeararchyy.Replace("{{REPLACE Footer}}", Footer);
                //strBaseHeararchyy = strBaseHeararchyy.Replace("{{REPLACE GROUP}}", @"<TablixMember><Group Name=""Details""/></TablixMember>");
            }

            return strBaseHeararchyy;
        }
        private string GetBaseTablixRowHierarchy()
        {
            string baseGroupString = @"<TablixMembers>
                                                <TablixMember>
                                                    <TablixHeader>
                                                    <Size>0.032in</Size>
                                                    <CellContents>
                                                    <Textbox Name=""txtRoomInfoGroupHead"">
                                                    <CanGrow>false</CanGrow>
                                                    <KeepTogether>true</KeepTogether>
                                                    <Paragraphs>
                                                       <Paragraph>
                                                            <TextRuns>
                                                                <TextRun>
                                                                    <Value></Value>
                                                                    <Style>
                                                                    <FontStyle>Normal</FontStyle>
                                                                    <FontFamily>Calibri</FontFamily>
                                                                    <FontWeight>Bold</FontWeight>
                                                                    </Style>
                                                                </TextRun>
                                                               </TextRuns>
                                                            <Style>
                                                                <TextAlign>Center</TextAlign>
                                                            </Style>
                                                        </Paragraph>
                                                    </Paragraphs>
                                                <rd:DefaultName>txtRoomInfoGroupHead</rd:DefaultName>
                                                <Visibility>
                                                    <Hidden>true</Hidden>
                                                </Visibility>
                                                <Style>
                                                <Border>
                                                <Color>LightGrey</Color>
                                                <Style>Solid</Style>
                                                </Border>
                                                  <BackgroundColor>LightGrey</BackgroundColor>
                                                  <PaddingLeft>2pt</PaddingLeft>
                                                  <PaddingRight>2pt</PaddingRight>
                                                  <PaddingTop>2pt</PaddingTop>
                                                  <PaddingBottom>2pt</PaddingBottom>
                                                </Style>
                                                  </Textbox>
                                                </CellContents>
                                              </TablixHeader>
                                              {{REPLACE HEADER}}
                                              <KeepWithGroup>After</KeepWithGroup>
                                              <RepeatOnNewPage>true</RepeatOnNewPage>
                                            </TablixMember>
                                            <TablixMember>
                                              <Group Name=""RoomGroup"">
                                                <GroupExpressions>
                                                  <GroupExpression>=Fields!RoomID.Value</GroupExpression>
                                                </GroupExpressions>
                                                <PageBreak>
                                                    <BreakLocation>Between</BreakLocation>
                                                </PageBreak>
                                              </Group>
                                              <TablixHeader>
                                                <Size>0.032in</Size>
                                                <CellContents>
                                                  <Textbox Name=""txtRoomInfo"">
                                                    <CanGrow>false</CanGrow>
                                                    <KeepTogether>true</KeepTogether>
                                                    <Paragraphs>
                                                      <Paragraph>
                                                        <TextRuns>
                                                          <TextRun>
                                                            <Value>=Fields!RoomInfo.Value</Value>
                                                            <Style>
                                                              <FontFamily>Calibri</FontFamily>
                                                            </Style>
                                                          </TextRun>
                                                        </TextRuns>
                                                        <Style>
                                                          <TextAlign>Center</TextAlign>
                                                        </Style>
                                                      </Paragraph>
                                                    </Paragraphs>
                                                    <rd:DefaultName>txtRoomInfo</rd:DefaultName>
                                                    <Visibility>
                                                      <Hidden>true</Hidden>
                                                    </Visibility>
                                                    <Style>
                                                      <Border>
                                                        <Color>LightGrey</Color>
                                                        <Style>Solid</Style>
                                                      </Border>
                                                      <BackgroundColor>= IIf(RowNumber(Nothing) Mod 2 = 0,""White"",""White"")</BackgroundColor>
                                                      <PaddingLeft>2pt</PaddingLeft>
                                                      <PaddingRight>2pt</PaddingRight>
                                                      <PaddingTop>2pt</PaddingTop>
                                                      <PaddingBottom>2pt</PaddingBottom>
                                                    </Style>
                                                  </Textbox>
                                                </CellContents>
                                              </TablixHeader>
                                              {{REPLACE GROUP}}
                                            </TablixMember>
                                            <TablixMember>
                                              <TablixHeader>
                                                <Size>0.032in</Size>
                                                <CellContents>
                                                  <Textbox Name=""txtFooter001"">
                                                    <CanGrow>false</CanGrow>
                                                    <KeepTogether>true</KeepTogether>
                                                    <Paragraphs>
                                                      <Paragraph>
                                                        <TextRuns>
                                                          <TextRun>
                                                            <Value />
                                                            <Style>
                                                              <FontFamily>Calibri</FontFamily>
                                                            </Style>
                                                          </TextRun>
                                                        </TextRuns>
                                                        <Style>
                                                          <TextAlign>Center</TextAlign>
                                                        </Style>
                                                      </Paragraph>
                                                    </Paragraphs>
                                                    <rd:DefaultName>txtFooter001</rd:DefaultName>
                                                    <Visibility>
                                                      <Hidden>true</Hidden>
                                                    </Visibility>
                                                    <Style>
                                                      <Border>
                                                        <Color>LightGrey</Color>
                                                        <Style>Solid</Style>
                                                      </Border>
                                                      <BackgroundColor>#ffffde</BackgroundColor>
                                                      <PaddingLeft>2pt</PaddingLeft>
                                                      <PaddingRight>2pt</PaddingRight>
                                                      <PaddingTop>2pt</PaddingTop>
                                                      <PaddingBottom>2pt</PaddingBottom>
                                                    </Style>
                                                  </Textbox>
                                                </CellContents>
                                              </TablixHeader>
                                              {{REPLACE FOOTER}}
                                              <KeepWithGroup>Before</KeepWithGroup>
                                            </TablixMember>
                                          </TablixMembers>
                                        ";


            return baseGroupString;
        }

        private string GetBaseTablixRowHierarchyWithoutFooter()
        {
            string baseGroupString = @"<TablixMembers>
                                                <TablixMember>
                                                    <TablixHeader>
                                                    <Size>0.022in</Size>
                                                    <CellContents>
                                                    <Textbox Name=""txtRoomInfoGroupHead"">
                                                    <CanGrow>false</CanGrow>
                                                    <KeepTogether>true</KeepTogether>
                                                    <Paragraphs>
                                                       <Paragraph>
                                                            <TextRuns>
                                                                <TextRun>
                                                                    <Value></Value>
                                                                    <Style>
                                                                    <FontStyle>Normal</FontStyle>
                                                                    <FontFamily>Calibri</FontFamily>
                                                                    <FontWeight>Bold</FontWeight>
                                                                    </Style>
                                                                </TextRun>
                                                               </TextRuns>
                                                            <Style>
                                                                <TextAlign>Center</TextAlign>
                                                            </Style>
                                                        </Paragraph>
                                                    </Paragraphs>
                                                <rd:DefaultName>txtRoomInfoGroupHead</rd:DefaultName>
                                                <Visibility>
                                                    <Hidden>true</Hidden>
                                                </Visibility>
                                                <Style>
                                                <Border>
                                                <Color>LightGrey</Color>
                                                <Style>Solid</Style>
                                                </Border>
                                                  <BackgroundColor>LightGrey</BackgroundColor>
                                                  <PaddingLeft>2pt</PaddingLeft>
                                                  <PaddingRight>2pt</PaddingRight>
                                                  <PaddingTop>2pt</PaddingTop>
                                                  <PaddingBottom>2pt</PaddingBottom>
                                                </Style>
                                                  </Textbox>
                                                </CellContents>
                                              </TablixHeader>
                                              {{REPLACE HEADER}}
                                              <KeepWithGroup>After</KeepWithGroup>
                                              <RepeatOnNewPage>true</RepeatOnNewPage>
                                            </TablixMember>
                                            <TablixMember>
                                            <TablixMembers>
                                            <TablixMember>
                                              <Group Name=""RoomGroup"">
                                                <GroupExpressions>
                                                  <GroupExpression>=Fields!RoomID.Value</GroupExpression>
                                                </GroupExpressions>
                                                <PageBreak>
                                                    <BreakLocation>Between</BreakLocation>
                                                </PageBreak>
                                              </Group>
                                              <TablixHeader>
                                                <Size>0.022in</Size>
                                                <CellContents>
                                                  <Textbox Name=""txtRoomInfo"">
                                                    <CanGrow>false</CanGrow>
                                                    <KeepTogether>true</KeepTogether>
                                                    <Paragraphs>
                                                      <Paragraph>
                                                        <TextRuns>
                                                          <TextRun>
                                                            <Value>=Fields!RoomInfo.Value</Value>
                                                            <Style>
                                                              <FontFamily>Calibri</FontFamily>
                                                            </Style>
                                                          </TextRun>
                                                        </TextRuns>
                                                        <Style>
                                                          <TextAlign>Center</TextAlign>
                                                        </Style>
                                                      </Paragraph>
                                                    </Paragraphs>
                                                    <rd:DefaultName>txtRoomInfo</rd:DefaultName>
                                                    <Visibility>
                                                      <Hidden>true</Hidden>
                                                    </Visibility>
                                                    <Style>
                                                      <Border>
                                                        <Color>LightGrey</Color>
                                                        <Style>Solid</Style>
                                                      </Border>
                                                      <BackgroundColor>= IIf(RowNumber(Nothing) Mod 2 = 0,""White"",""White"")</BackgroundColor>
                                                      <PaddingLeft>2pt</PaddingLeft>
                                                      <PaddingRight>2pt</PaddingRight>
                                                      <PaddingTop>2pt</PaddingTop>
                                                      <PaddingBottom>2pt</PaddingBottom>
                                                    </Style>
                                                  </Textbox>
                                                </CellContents>
                                              </TablixHeader>
                                              {{REPLACE GROUP}}
                                                </TablixMember>
                                            </TablixMembers>
                                            </TablixMember> 
                                             {{REPLACE Footer}}
                                          </TablixMembers>
                                        ";


            return baseGroupString;
        }

        public string GetBaseFooter()
        {
            string baseGroupString = @"<TablixMember>
                                                  <TablixHeader>
                                                    <Size>0.022in</Size>
                                                    <CellContents>
                                                    <Textbox Name=""txtTRHFooter"">
                                                    <CanGrow>false</CanGrow>
                                                    <KeepTogether>true</KeepTogether>
                                                    <Paragraphs>
                                                       <Paragraph>
                                                            <TextRuns>
                                                                <TextRun>
                                                                    <Value></Value>
                                                                    <Style>
                                                                    <FontStyle>Normal</FontStyle>
                                                                    <FontFamily>Calibri</FontFamily>
                                                                    <FontWeight>Bold</FontWeight>
                                                                    </Style>
                                                                </TextRun>
                                                               </TextRuns>
                                                            <Style>
                                                                <TextAlign>Center</TextAlign>
                                                            </Style>
                                                        </Paragraph>
                                                    </Paragraphs>
                                                <rd:DefaultName>txtTRHFooter</rd:DefaultName>
                                                <Visibility>
                                                    <Hidden>true</Hidden>
                                                </Visibility>
                                                <Style>
                                                <Border>
                                                <Color>LightGrey</Color>
                                                <Style>Solid</Style>
                                                </Border>
                                                  <BackgroundColor>LightGrey</BackgroundColor>
                                                  <PaddingLeft>2pt</PaddingLeft>
                                                  <PaddingRight>2pt</PaddingRight>
                                                  <PaddingTop>2pt</PaddingTop>
                                                  <PaddingBottom>2pt</PaddingBottom>
                                                </Style>
                                                  </Textbox>
                                                </CellContents>
                                              </TablixHeader>
                                               <TablixMembers>
                                                    <TablixMember />
                                               </TablixMembers>
                                        </TablixMember>
                                        ";


            return baseGroupString;
        }

        #region WI-4947 - Please add a Last Run Date to the schedule reports

        public XDocument AddLastRunDateToHeader(XDocument doc1, ReportBuilderDTO objRPTDTO, Int64 NotificationID, string CompanyName, string RoomName, Int64 EnterPriseID, Int64 CompanyID, Int64 RoomID, eTurnsRegionInfo RegionInfo, Dictionary<string, string> ReportPara = null, string EnterpriseDBName = null)
        {
            XDocument doc = new XDocument(doc1);
            XElement xNonDeletedRow = doc.Descendants(ns + "TablixRow").FirstOrDefault();
            XElement xRows = doc.Descendants(ns + "TablixRows").FirstOrDefault();
            XElement xRow = doc.Descendants(ns + "TablixRow").FirstOrDefault();
            if (objRPTDTO.ReportType == 2 && xRow.Descendants(ns + "TablixCell").Descendants(ns + "Textbox").
                        FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "TextboxRoom") != null)
            {
                doc.Descendants(ns + "TablixRow").FirstOrDefault().Remove();
                doc.Descendants(ns + "TablixRowHierarchy").Descendants(ns + "TablixMembers").Descendants(ns + "TablixMember").Descendants(ns + "TablixMembers").Descendants(ns + "TablixMember").Remove();
                foreach (var item in xRows.Descendants(ns + "TablixRow"))
                {
                    doc.Descendants(ns + "TablixRowHierarchy").Descendants(ns + "TablixMembers").Descendants(ns + "TablixMember").Descendants(ns + "TablixMembers").FirstOrDefault().Add(new XElement(ns + "TablixMember"));
                }
            }
            xRow = doc.Descendants(ns + "TablixRow").FirstOrDefault();
            int TotalCells = xRow.Descendants(ns + "TablixCell").Count();

            string rptTitle = "";
            XElement xElerptTitle = doc.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Textbox").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "ReportTitle");
            if (xElerptTitle == null)
            {
                xElerptTitle = doc.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Textbox").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "TextboxWO");
            }

            if (xElerptTitle != null)
            {
                rptTitle = xElerptTitle.Descendants(ns + "Value").FirstOrDefault().Value;
            }

            XElement xNewRow = new XElement(xRow);
            xRows.AddFirst(xNewRow);
            xNewRow = null;
            xNewRow = doc.Descendants(ns + "TablixRow").FirstOrDefault();
            xNewRow.Descendants(ns + "Height").FirstOrDefault().Value = xNonDeletedRow.Descendants(ns + "Height").FirstOrDefault().Value;
            IEnumerable<XElement> lstXnew = xNewRow.Descendants(ns + "TablixCell");

            string txtAttrName = xNewRow.Descendants(ns + "Textbox").FirstOrDefault().FirstAttribute.Value;

            txtAttrName = txtAttrName + "_1";
            xNewRow.Descendants(ns + "Textbox").FirstOrDefault().FirstAttribute.SetValue(txtAttrName);
            if (xNewRow.Descendants(ns + "Textbox").FirstOrDefault().Descendants(ns + "TextAlign").FirstOrDefault() != null)
                xNewRow.Descendants(ns + "Textbox").FirstOrDefault().Descendants(ns + "TextAlign").FirstOrDefault().Value = "Left";

            xNewRow.Descendants(nsrd + "DefaultName").FirstOrDefault().Value = txtAttrName;
            XElement FirstTextBoxValue = lstXnew.FirstOrDefault().Descendants(ns + "TextRun").FirstOrDefault().Descendants(ns + "Value").FirstOrDefault();

            string[] arrRooms = null;

            if (ReportPara != null && ReportPara.Keys.Contains("RoomIDs"))
            {
                string strRoomIDs = ReportPara["RoomIDs"];
                arrRooms = strRoomIDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
            string strVal = @"=First(Fields!CurrentDateTime.Value, ""DataSet1"") + " + @"""     " + CompanyName + @""" + " + @"""     " + RoomName + @""" + " + @"""     " + rptTitle + @"""";
            if (objRPTDTO.ReportType != 2 && arrRooms != null && arrRooms.Length > 1)
            {
                strVal = @"=First(Fields!CurrentDateTime.Value, ""DataSet1"") + " + @"""     " + rptTitle + @"""";
            }

            if (ReportPara != null)
            {
                if (ReportPara.ContainsKey("OrigStartDate") && !string.IsNullOrEmpty(ReportPara["OrigStartDate"]))
                {
                    strVal = strVal + @"+""        Start: " + ReportPara["OrigStartDate"] + @"""";
                }

                if (ReportPara.ContainsKey("OrigEndDate") && !string.IsNullOrEmpty(ReportPara["OrigEndDate"]))
                {
                    strVal = strVal + @"  +""   To: " + ReportPara["OrigEndDate"] + @"""";
                }
            }

            if (RegionInfo != null)
            {
                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(EnterpriseDBName);
                ReportMailLogDTO objReportMailLogDTO = new ReportMailLogDTO();
                objReportMailLogDTO = objReportMasterDAL.GetLastRunDateforSchedule(NotificationID, CompanyID, RoomID);
                if (objReportMailLogDTO != null)
                {
                    strVal = strVal + @"+""        Last Run Date : " + DateTimeUtility.ConvertDateByTimeZone(objReportMailLogDTO.SendDate, RegionInfo.TimeZoneName, RegionInfo.LongDatePattern, RegionInfo.CultureCode, true) + @"""";
                }
                else
                {
                    strVal = strVal + @"+""        Last Run Date : " + DateTimeUtility.ConvertDateByTimeZone(DateTime.UtcNow, RegionInfo.TimeZoneName, RegionInfo.LongDatePattern, RegionInfo.CultureCode, true) + @"""";
                }
            }
            if (objRPTDTO.ReportType != 2 && arrRooms != null && arrRooms.Length > 1)
            {
                // string strRoomCompany = @"";
                RoomDAL rDAL = null;
                if (EnterpriseDBName != null && (!string.IsNullOrWhiteSpace(EnterpriseDBName)))
                {
                    rDAL = new RoomDAL(EnterpriseDBName);
                }
                else
                {
                    rDAL = new RoomDAL("eTurns");
                }

                IEnumerable<RoomDTO> RoomDTO = rDAL.GetRoomByIDsNormal(ReportPara["RoomIDs"]);
                IEnumerable<string> lstCompanies = RoomDTO.Select(x => x.CompanyName).Distinct();
                IEnumerable<string> lstRooms = RoomDTO.Select(x => x.RoomName).Distinct();

                string strCompanies = string.Join(", ", lstCompanies);
                string strRooms = string.Join(", ", lstRooms);
                strVal = strVal + @"+ VbCrLf +""" + strCompanies + @"""";
                strVal = strVal + @"+ VbCrLf +""" + strRooms + @"""";

                //foreach (var item in RoomDTO)
                //{
                //    if (strRoomCompany.Length > 0)
                //        strRoomCompany += ", ";

                //    strRoomCompany += item.CompanyName + "\t" + item.RoomName;
                //}
                //strVal = strVal + @"+ VbCrLf +""" + System.Environment.NewLine + strRoomCompany + @"""";
            }


            FirstTextBoxValue.Value = strVal;// + ""      "" + First(Fields!CompanyName.Value, ""DataSet1"") + ""      "" + First(Fields!RoomName.Value, ""DataSet1"") + " + @"""     " + rptTitle + @"""";

            lstXnew.FirstOrDefault().Descendants(ns + "BackgroundColor").Remove();
            if (lstXnew.FirstOrDefault().Descendants(ns + "CellContents").Descendants(ns + "ColSpan").FirstOrDefault() == null)
            {
                lstXnew.FirstOrDefault().Descendants(ns + "CellContents").FirstOrDefault().Add(new XElement(ns + "ColSpan", TotalCells.ToString()));
            }

            lstXnew.ToList().ForEach(x =>
            {
                if (x.Descendants(ns + "ColSpan").Count() <= 0)
                {
                    x.Remove();
                }
            });

            for (int i = 1; i < TotalCells; i++)
            {
                doc.Descendants(ns + "TablixCells").FirstOrDefault().Add(new XElement(ns + "TablixCell"));
            }

            XElement xRowHira = doc.Descendants(ns + "TablixRowHierarchy").FirstOrDefault();
            XElement xTablixMember = xRowHira.Descendants(ns + "TablixMember").FirstOrDefault();
            XElement xTablixMembers = xRowHira.Descendants(ns + "TablixMembers").FirstOrDefault();
            if (objRPTDTO.ReportType == 3)
            {
                //txtAttrName = xTablixMember.Descendants(ns + "Textbox").FirstOrDefault().FirstAttribute.Value;
                txtAttrName = txtAttrName + "_2";
                xTablixMember.Descendants(ns + "Textbox").FirstOrDefault().FirstAttribute.SetValue(txtAttrName);
                xTablixMember.Descendants(nsrd + "DefaultName").FirstOrDefault().Value = txtAttrName;
                xTablixMember.Descendants(ns + "Textbox").FirstOrDefault().Descendants(ns + "Value").FirstOrDefault().Value = string.Empty;
                XElement newTablixMember = new XElement(xTablixMember);
                xTablixMembers.AddFirst(newTablixMember);

                newTablixMember = null;
                xRowHira = doc.Descendants(ns + "TablixRowHierarchy").FirstOrDefault();
                newTablixMember = xRowHira.Descendants(ns + "TablixMember").FirstOrDefault();
                txtAttrName = txtAttrName + "_2";
                newTablixMember.Descendants(ns + "Textbox").FirstOrDefault().FirstAttribute.SetValue(txtAttrName);
                newTablixMember.Descendants(nsrd + "DefaultName").FirstOrDefault().Value = txtAttrName;
                newTablixMember.Descendants(ns + "Textbox").FirstOrDefault().Descendants(ns + "Value").FirstOrDefault().Value = string.Empty;

                IEnumerable<XElement> textboxs = xRowHira.Descendants(ns + "Textbox");

                int indx = 101;

                foreach (var item in textboxs)
                {
                    string txtname = item.FirstAttribute.Value;
                    string txtnewname = txtname + "_" + indx.ToString();
                    item.FirstAttribute.SetValue(txtnewname);

                    XElement txtDefaultName = xRowHira.Descendants(nsrd + "DefaultName").Where(x => x.Value == txtname).FirstOrDefault();
                    txtDefaultName.Value = txtnewname;
                    indx += 1;
                }
            }
            else if (objRPTDTO.ReportType == 1)
            {
                XElement newTablixMember = new XElement(xTablixMember);
                xTablixMembers.AddFirst(newTablixMember);
            }
            else if (objRPTDTO.ReportType == 2)
            {
                doc.Descendants(ns + "TablixRowHierarchy").Descendants(ns + "TablixMembers").Descendants(ns + "TablixMember").Descendants(ns + "TablixMembers").FirstOrDefault().Add(new XElement(ns + "TablixMember"));
            }
            // doc.Descendants(ns + "PageHeader").Remove();
            return doc;
        }

        #endregion

        #region WI-5107 Add Date Range on the Header of the Report.

        public XDocument GetAdditionalHeaderRowWithDateRange(XDocument doc1, ReportBuilderDTO objRPTDTO, string CompanyName, string RoomName, Dictionary<string, string> ReportPara = null, string EnterpriseDBName = null, Int64 NotificationID = 0, Int64 EnterPriseID = 0, Int64 CompanyID = 0, Int64 RoomID = 0, eTurnsRegionInfo RegionInfo = null)
        {
            XDocument doc = new XDocument(doc1);
            XElement xNonDeletedRow = doc.Descendants(ns + "TablixRow").FirstOrDefault();
            XElement xRows = doc.Descendants(ns + "TablixRows").FirstOrDefault();
            XElement xRow = doc.Descendants(ns + "TablixRow").FirstOrDefault();
            if (objRPTDTO.ReportType == 2 && xRow.Descendants(ns + "TablixCell").Descendants(ns + "Textbox").
                        FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "TextboxRoom") != null)
            {
                doc.Descendants(ns + "TablixRow").FirstOrDefault().Remove();
                doc.Descendants(ns + "TablixRowHierarchy").Descendants(ns + "TablixMembers").Descendants(ns + "TablixMember").Descendants(ns + "TablixMembers").Descendants(ns + "TablixMember").Remove();
                foreach (var item in xRows.Descendants(ns + "TablixRow"))
                {
                    doc.Descendants(ns + "TablixRowHierarchy").Descendants(ns + "TablixMembers").Descendants(ns + "TablixMember").Descendants(ns + "TablixMembers").FirstOrDefault().Add(new XElement(ns + "TablixMember"));
                }
            }
            xRow = doc.Descendants(ns + "TablixRow").FirstOrDefault();
            int TotalCells = xRow.Descendants(ns + "TablixCell").Count();

            string rptTitle = "";
            XElement xElerptTitle = doc.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Textbox").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "ReportTitle");
            if (xElerptTitle == null)
            {
                xElerptTitle = doc.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Textbox").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "TextboxWO");
            }

            if (xElerptTitle != null)
            {
                rptTitle = xElerptTitle.Descendants(ns + "Value").FirstOrDefault().Value;
            }

            XElement xNewRow = new XElement(xRow);
            xRows.AddFirst(xNewRow);
            xNewRow = null;
            xNewRow = doc.Descendants(ns + "TablixRow").FirstOrDefault();
            xNewRow.Descendants(ns + "Height").FirstOrDefault().Value = xNonDeletedRow.Descendants(ns + "Height").FirstOrDefault().Value;
            IEnumerable<XElement> lstXnew = xNewRow.Descendants(ns + "TablixCell");

            string txtAttrName = xNewRow.Descendants(ns + "Textbox").FirstOrDefault().FirstAttribute.Value;

            txtAttrName = txtAttrName + "_1";
            xNewRow.Descendants(ns + "Textbox").FirstOrDefault().FirstAttribute.SetValue(txtAttrName);
            if (xNewRow.Descendants(ns + "Textbox").FirstOrDefault().Descendants(ns + "TextAlign").FirstOrDefault() != null)
                xNewRow.Descendants(ns + "Textbox").FirstOrDefault().Descendants(ns + "TextAlign").FirstOrDefault().Value = "Left";

            xNewRow.Descendants(nsrd + "DefaultName").FirstOrDefault().Value = txtAttrName;
            XElement FirstTextBoxValue = lstXnew.FirstOrDefault().Descendants(ns + "TextRun").FirstOrDefault().Descendants(ns + "Value").FirstOrDefault();

            string[] arrRooms = null;

            //if (ReportPara != null && ReportPara.Keys.Contains("RoomIDs"))
            //{
            //    string strRoomIDs = ReportPara["RoomIDs"];
            //    arrRooms = strRoomIDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            //}
            //string strVal = @"=First(Fields!CurrentDateTime.Value, ""DataSet1"") + " + @"""     " + CompanyName + @""" + " + @"""     " + RoomName + @""" + " + @"""     " + rptTitle + @"""";
            //if (objRPTDTO.ReportType != 2 && arrRooms != null && arrRooms.Length > 1)
            //{
            //    strVal = @"=First(Fields!CurrentDateTime.Value, ""DataSet1"") + " + @"""     " + rptTitle + @"""";
            //}
            string strVal = @"";
            if (ReportPara != null)
            {
                if (ReportPara.ContainsKey("OrigStartDate") && !string.IsNullOrEmpty(ReportPara["OrigStartDate"]))
                {
                    strVal = strVal + @"Start: " + ReportPara["OrigStartDate"] + @"";
                }

                if (ReportPara.ContainsKey("OrigEndDate") && !string.IsNullOrEmpty(ReportPara["OrigEndDate"]))
                {
                    strVal = strVal + @"   To: " + ReportPara["OrigEndDate"] + @"";
                }
            }

            //if (Convert.ToInt64(NotificationID) > 0 && RegionInfo != null)
            //{
            //    ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(EnterpriseDBName);
            //    ReportMailLogDTO objReportMailLogDTO = new ReportMailLogDTO();
            //    objReportMailLogDTO = objReportMasterDAL.GetLastRunDateforSchedule(Convert.ToInt64(NotificationID), Convert.ToInt64(CompanyID), Convert.ToInt64(RoomID));
            //    if (objReportMailLogDTO != null)
            //    {
            //        strVal = strVal + @"+""        Last Run Date : " + DateTimeUtility.ConvertDateByTimeZone(objReportMailLogDTO.SendDate, RegionInfo.TimeZoneName, RegionInfo.LongDatePattern, RegionInfo.CultureCode, true) + @"""";
            //    }
            //    else
            //    {
            //        strVal = strVal + @"+""        Last Run Date : " + DateTimeUtility.ConvertDateByTimeZone(DateTime.UtcNow, RegionInfo.TimeZoneName, RegionInfo.LongDatePattern, RegionInfo.CultureCode, true) + @"""";
            //    }
            //}

            if (objRPTDTO.ReportType != 2 && arrRooms != null && arrRooms.Length > 1)
            {
                // string strRoomCompany = @"";
                RoomDAL rDAL = null;
                if (EnterpriseDBName != null && (!string.IsNullOrWhiteSpace(EnterpriseDBName)))
                {
                    rDAL = new RoomDAL(EnterpriseDBName);
                }
                else
                {
                    rDAL = new RoomDAL("eTurns");
                }

                IEnumerable<RoomDTO> RoomDTO = rDAL.GetRoomByIDsNormal(ReportPara["RoomIDs"]);
                IEnumerable<string> lstCompanies = RoomDTO.Select(x => x.CompanyName).Distinct();
                IEnumerable<string> lstRooms = RoomDTO.Select(x => x.RoomName).Distinct();

                string strCompanies = string.Join(", ", lstCompanies);
                string strRooms = string.Join(", ", lstRooms);
                strVal = strVal + @"+ VbCrLf +""" + strCompanies + @"""";
                strVal = strVal + @"+ VbCrLf +""" + strRooms + @"""";

                //foreach (var item in RoomDTO)
                //{
                //    if (strRoomCompany.Length > 0)
                //        strRoomCompany += ", ";

                //    strRoomCompany += item.CompanyName + "\t" + item.RoomName;
                //}
                //strVal = strVal + @"+ VbCrLf +""" + System.Environment.NewLine + strRoomCompany + @"""";
            }


            FirstTextBoxValue.Value = strVal;// + ""      "" + First(Fields!CompanyName.Value, ""DataSet1"") + ""      "" + First(Fields!RoomName.Value, ""DataSet1"") + " + @"""     " + rptTitle + @"""";

            lstXnew.FirstOrDefault().Descendants(ns + "BackgroundColor").Remove();
            if (lstXnew.FirstOrDefault().Descendants(ns + "CellContents").Descendants(ns + "ColSpan").FirstOrDefault() == null)
            {
                lstXnew.FirstOrDefault().Descendants(ns + "CellContents").FirstOrDefault().Add(new XElement(ns + "ColSpan", TotalCells.ToString()));
            }

            lstXnew.ToList().ForEach(x =>
            {
                if (x.Descendants(ns + "ColSpan").Count() <= 0)
                {
                    x.Remove();
                }
            });

            for (int i = 1; i < TotalCells; i++)
            {
                doc.Descendants(ns + "TablixCells").FirstOrDefault().Add(new XElement(ns + "TablixCell"));
            }

            XElement xRowHira = doc.Descendants(ns + "TablixRowHierarchy").FirstOrDefault();
            XElement xTablixMember = xRowHira.Descendants(ns + "TablixMember").FirstOrDefault();
            XElement xTablixMembers = xRowHira.Descendants(ns + "TablixMembers").FirstOrDefault();
            if (objRPTDTO.ReportType == 3)
            {
                //txtAttrName = xTablixMember.Descendants(ns + "Textbox").FirstOrDefault().FirstAttribute.Value;
                txtAttrName = txtAttrName + "_2";
                xTablixMember.Descendants(ns + "Textbox").FirstOrDefault().FirstAttribute.SetValue(txtAttrName);
                xTablixMember.Descendants(nsrd + "DefaultName").FirstOrDefault().Value = txtAttrName;
                xTablixMember.Descendants(ns + "Textbox").FirstOrDefault().Descendants(ns + "Value").FirstOrDefault().Value = string.Empty;
                XElement newTablixMember = new XElement(xTablixMember);
                xTablixMembers.AddFirst(newTablixMember);

                newTablixMember = null;
                xRowHira = doc.Descendants(ns + "TablixRowHierarchy").FirstOrDefault();
                newTablixMember = xRowHira.Descendants(ns + "TablixMember").FirstOrDefault();
                txtAttrName = txtAttrName + "_2";
                newTablixMember.Descendants(ns + "Textbox").FirstOrDefault().FirstAttribute.SetValue(txtAttrName);
                newTablixMember.Descendants(nsrd + "DefaultName").FirstOrDefault().Value = txtAttrName;
                newTablixMember.Descendants(ns + "Textbox").FirstOrDefault().Descendants(ns + "Value").FirstOrDefault().Value = string.Empty;

                IEnumerable<XElement> textboxs = xRowHira.Descendants(ns + "Textbox");

                int indx = 101;

                foreach (var item in textboxs)
                {
                    string txtname = item.FirstAttribute.Value;
                    string txtnewname = txtname + "_" + indx.ToString();
                    item.FirstAttribute.SetValue(txtnewname);

                    XElement txtDefaultName = xRowHira.Descendants(nsrd + "DefaultName").Where(x => x.Value == txtname).FirstOrDefault();
                    txtDefaultName.Value = txtnewname;
                    indx += 1;
                }
            }
            else if (objRPTDTO.ReportType == 1)
            {
                XElement newTablixMember = new XElement(xTablixMember);
                xTablixMembers.AddFirst(newTablixMember);
            }
            else if (objRPTDTO.ReportType == 2)
            {
                doc.Descendants(ns + "TablixRowHierarchy").Descendants(ns + "TablixMembers").Descendants(ns + "TablixMember").Descendants(ns + "TablixMembers").FirstOrDefault().Add(new XElement(ns + "TablixMember"));
            }
            return doc;
        }

        #endregion

    }
}
