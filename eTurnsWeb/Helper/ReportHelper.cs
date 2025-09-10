using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;


namespace eTurnsWeb.Helper
{
    public class ReportHelper : IDisposable
    {
        private int m_currentPageIndex;
        private IList<Stream> m_streams;
        XNamespace ns = XNamespace.Get("http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition");
        XNamespace nsrd = XNamespace.Get("http://schemas.microsoft.com/SQLServer/reporting/reportdesigner");
        string connectionString = "";
        string SubReportResFile = "";
        int globalcounter = 0;
        //int subRptCounter = 0;
        eTurnsRegionInfo rsInfo = SessionHelper.eTurnsRegionInfoProp;
        eTurns.DAL.AlertMail amDAL = new eTurns.DAL.AlertMail();
        string strSubTablix = string.Empty;
        string rdlPath = string.Empty;
        long ParentID = 0;
        Dictionary<string, string> LocalDictRptPara = null;
        private DataTable LoadSalesData()
        {

            // Create a new DataSet and read sales data file 
            //    data.xml into the first DataTable.
            DataSet dataSet = new DataSet();
            dataSet.ReadXml(@"..\..\data.xml");
            return dataSet.Tables[0];
        }
        // Routine to provide to the report renderer, in order to
        //    save an image for each page of the report.
        private Stream CreateStream(string name, string fileNameExtension, Encoding encoding, string mimeType, bool willSeek)
        {
            Stream stream = new MemoryStream();
            m_streams.Add(stream);
            return stream;
        }
        // Export the given report as an EMF (Enhanced Metafile) file.
        private string Export(LocalReport report, string FileType = "")
        {
            string mimeType;
            string encoding;
            string fileNameExtension;

            //string deviceInfo =
            //  @"<DeviceInfo>
            //    <OutputFormat>EMF</OutputFormat>
            //    <PageWidth>8.5in</PageWidth>
            //    <PageHeight>11in</PageHeight>
            //    <MarginTop>0.25in</MarginTop>
            //    <MarginLeft>0.25in</MarginLeft>
            //    <MarginRight>0.25in</MarginRight>
            //    <MarginBottom>0.25in</MarginBottom>
            //</DeviceInfo>";
            Warning[] warnings;
            m_streams = new List<Stream>();
            string[] am_sterams = new string[] { };

            //byte[] pdfContent = report.Render("PDF", deviceInfo, CreateStream, out warnings);

            byte[] Content;
            string fileName = "";

            //byte[] pdfContent = report.Render("PDF", null, out mimeType, out encoding, out fileNameExtension, out am_sterams, out warnings);
            //string fileName = Guid.NewGuid().ToString("N") + ".pdf";

            if (!string.IsNullOrWhiteSpace(FileType))
            {
                if (FileType.ToLower().Equals("pdf"))
                {
                    Content = report.Render("PDF", null, out mimeType, out encoding, out fileNameExtension, out am_sterams, out warnings);
                    fileName = Guid.NewGuid().ToString("N") + ".pdf";
                }
                else if (FileType.ToLower().Equals("xls"))
                {
                    Content = report.Render("EXCEL", null, out mimeType, out encoding, out fileNameExtension, out am_sterams, out warnings);
                    fileName = Guid.NewGuid().ToString("N") + ".xls";
                }
                else if (FileType.ToLower().Equals("xlsx"))
                {
                    Content = report.Render("EXCELOPENXML", null, out mimeType, out encoding, out fileNameExtension, out am_sterams, out warnings);
                    fileName = Guid.NewGuid().ToString("N") + ".xlsx";
                }
                else if (FileType.ToLower().Equals("doc"))
                {
                    Content = report.Render("WORD", null, out mimeType, out encoding, out fileNameExtension, out am_sterams, out warnings);
                    fileName = Guid.NewGuid().ToString("N") + ".doc";
                }
                else if (FileType.ToLower().Equals("docx"))
                {
                    Content = report.Render("WORDOPENXML", null, out mimeType, out encoding, out fileNameExtension, out am_sterams, out warnings);
                    fileName = Guid.NewGuid().ToString("N") + ".docx";
                }
                else if (FileType.ToLower().Equals("tif"))
                {
                    Content = report.Render("IMAGE", null, out mimeType, out encoding, out fileNameExtension, out am_sterams, out warnings);
                    fileName = Guid.NewGuid().ToString("N") + ".tif";
                }
                else
                {
                    Content = report.Render("PDF", null, out mimeType, out encoding, out fileNameExtension, out am_sterams, out warnings);
                    fileName = Guid.NewGuid().ToString("N") + ".pdf";
                }
            }
            else
            {
                Content = report.Render("PDF", null, out mimeType, out encoding, out fileNameExtension, out am_sterams, out warnings);
                fileName = Guid.NewGuid().ToString("N") + ".pdf";
            }

            string filePath = HttpContext.Current.Server.MapPath("~/Content/OpenAccess/ReportsPDF/") + fileName;
            System.IO.File.WriteAllBytes(filePath, Content);

            return fileName;
            ////foreach (Stream stream in m_streams)
            ////    stream.Position = 0;
            //string fileName = Guid.NewGuid().ToString("N") + ".pdf";


        }
        // Handler for PrintPageEvents
        private void PrintPage(object sender, PrintPageEventArgs ev)
        {
            Metafile pageImage = new
               Metafile(m_streams[m_currentPageIndex]);

            // Adjust rectangular area with printer margins.
            Rectangle adjustedRect = new Rectangle(
                ev.PageBounds.Left - (int)ev.PageSettings.HardMarginX,
                ev.PageBounds.Top - (int)ev.PageSettings.HardMarginY,
                ev.PageBounds.Width,
                ev.PageBounds.Height);

            // Draw a white background for the report
            ev.Graphics.FillRectangle(Brushes.White, adjustedRect);

            // Draw the report content
            ev.Graphics.DrawImage(pageImage, adjustedRect);

            // Prepare for the next page. Make sure we haven't hit the end.
            m_currentPageIndex++;
            ev.HasMorePages = (m_currentPageIndex < m_streams.Count);
        }

        public void Print(string printer)
        {
            PrinterSettings objPrinterSettings = new PrinterSettings();

            if (m_streams == null || m_streams.Count == 0)
                throw new Exception("Error: no stream to print.");
            PrintDocument printDoc = new PrintDocument();
            printDoc.PrinterSettings.PrinterName = printer;
            if (!printDoc.PrinterSettings.IsValid)
            {
                throw new Exception("Error: cannot find the default printer.");
            }
            else
            {

                printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
                m_currentPageIndex = 0;
                printDoc.Print();
            }
        }
        // Create a local report for Report.rdlc, load the data,
        //    export the report to an .emf file, and print it.
        private void Run()
        {
            LocalReport report = new LocalReport();
            report.ReportPath = @"..\..\Report.rdlc";
            report.DataSources.Add(
               new ReportDataSource("Sales", LoadSalesData()));
            Export(report);
            PrinterSettings settings = new PrinterSettings();
            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                settings.PrinterName = printer;
                if (settings.IsDefaultPrinter)
                    Print(printer);
            }
        }

        public void Dispose()
        {
            if (m_streams != null)
            {
                foreach (Stream stream in m_streams)
                    stream.Close();
                m_streams = null;
            }
        }
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
            cmd.CommandTimeout = 120;
            sqla.Fill(dt);

            //if (LocalDictRptPara.ContainsKey("SortFields") && !string.IsNullOrEmpty(LocalDictRptPara["SortFields"]))
            //{
            //    string SortFields = LocalDictRptPara["SortFields"];
            //    if (!string.IsNullOrEmpty(SortFields))
            //    {
            //        dt.DefaultView.Sort = SortFields;
            //        dt = dt.DefaultView.ToTable();
            //    }
            //}

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
                                XElement objReportPara = lstReportPara.FirstOrDefault(x => x.Attribute("Name").Value == slparWOA.ParameterName.Replace("@", ""));
                                if (objReportPara.Descendants(ns + "DataType") != null && objReportPara.Descendants(ns + "DataType").Count() > 0)
                                {
                                    slparWOA.DbType = (DbType)Enum.Parse(typeof(DbType), objReportPara.Descendants(ns + "DataType").FirstOrDefault().Value, true);
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
        public string LoadReport(Int64 Id, string FileType = "")
        {
            LocalReport rptLocalReport = new LocalReport();
            ParentID = 0;
            ReportBuilderDTO objDTO = new ReportBuilderDTO();
            bool IsRoomGridReportCommon = false;
            string RoomDBName = string.Empty;
            Int64 EntID = 0;

            LocalDictRptPara = GetReportParaDictionary();
            if (LocalDictRptPara.Keys.Contains("IsRoomGridReportCommon"))
                IsRoomGridReportCommon = Convert.ToBoolean(LocalDictRptPara["IsRoomGridReportCommon"]);
            if (LocalDictRptPara.Keys.Contains("DBName"))
                RoomDBName = LocalDictRptPara["DBName"];
            if (LocalDictRptPara.Keys.Contains("EnterpriseID"))
                EntID = Convert.ToInt64(LocalDictRptPara["EnterpriseID"]);

            string EnterPriseDBName = SessionHelper.EnterPriseDBName;
            long EnterPriceID = SessionHelper.EnterPriceID;
            long CompanyID = SessionHelper.CompanyID;
            long RoomID = SessionHelper.RoomID;
            if (EntID > 0 && IsRoomGridReportCommon && !string.IsNullOrWhiteSpace(RoomDBName))
            {
                EnterPriceID = EntID;
                EnterPriseDBName = RoomDBName;

                string strCompanyID = string.Empty;
                string strRoomID = string.Empty;
                //System.Xml.Linq.XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));

                //if (Settinfile != null && Settinfile.Element("RoomReportGridCompanyID") != null)
                //    strCompanyID = Convert.ToString(Settinfile.Element("RoomReportGridCompanyID").Value);

                if (SiteSettingHelper.RoomReportGridCompanyID  != string.Empty)
                    strCompanyID = Convert.ToString(SiteSettingHelper.RoomReportGridCompanyID);

                //if (Settinfile != null && Settinfile.Element("RoomReportGridRoomID") != null)
                //    strRoomID = Convert.ToString(Settinfile.Element("RoomReportGridRoomID").Value);

                if (SiteSettingHelper.RoomReportGridRoomID != string.Empty)
                    strRoomID = Convert.ToString(SiteSettingHelper.RoomReportGridRoomID);

                if (!string.IsNullOrWhiteSpace(strCompanyID))
                    CompanyID = Convert.ToInt64(strCompanyID);

                if (!string.IsNullOrWhiteSpace(strRoomID))
                    RoomID = Convert.ToInt64(strRoomID);
            }
            ReportMasterDAL objDAL = new ReportMasterDAL(EnterPriseDBName);
            objDTO = objDAL.GetReportDetail(Id);
            string MasterReportResFile = objDTO.MasterReportResFile;
            SubReportResFile = MasterReportResFile;// objDTO.SubReportResFile;
            string Reportname = objDTO.ReportName;
            string MasterReportname = objDTO.ReportFileName;
            string SubReportname = objDTO.SubReportFileName;
            string mainGuid = "RPT_" + Guid.NewGuid().ToString().Replace("-", "_");
            string subGuid = "SubRPT_" + Guid.NewGuid().ToString().Replace("-", "_");
            string ReportPath = string.Empty;
            bool hasSubReport = false;
            eTurns.DAL.AlertMail objAlertMail = new eTurns.DAL.AlertMail();


            string RDLCBaseFilePath = CommonUtility.RDLCBaseFilePath;
            ParentID = objDTO.ParentID ?? 0;
            if (objDTO.ParentID > 0)
            {
                if (objDTO.ISEnterpriseReport.GetValueOrDefault(false))
                {
                    ReportPath = CopyFiletoTemp(RDLCBaseFilePath + @"/" + EnterPriceID.ToString() + "/EnterpriseReport" + @"\\" + MasterReportname, mainGuid);
                }
                else
                {
                    ReportPath = CopyFiletoTemp(RDLCBaseFilePath + @"/" + EnterPriceID.ToString() + "/" + CompanyID.ToString() + @"\\" + MasterReportname, mainGuid);
                }
            }
            else
            {
                ReportPath = CopyFiletoTemp(RDLCBaseFilePath + @"/" + EnterPriceID.ToString() + "/BaseReport" + @"\\" + MasterReportname, mainGuid);
            }

            bool isReceivedItemReport = false;
            ReportBuilderDTO oReportBuilderDTO = objDAL.GetParentReportDetailByReportID(objDTO.ID);
            if (oReportBuilderDTO != null)
            {
                if (oReportBuilderDTO.ReportName.ToLower().Equals("item received receivable"))
                {
                    isReceivedItemReport = true;
                }
                else
                {
                    isReceivedItemReport = false;
                }
            }

            XDocument doc = XDocument.Load(ReportPath);
            if (isReceivedItemReport)
            {
                bool isTopNodeFound = false;
                double FirstTablixHeight = 0;
                if ((doc.Descendants(ns + "Tablix").ToList())[0] != null)
                {
                    foreach (var nd in (doc.Descendants(ns + "Tablix").ToList())[0].Nodes().ToList())
                    {
                        string nodeName = ((System.Xml.Linq.XElement)nd).Name.LocalName;
                        if (nodeName == "Height")
                        {
                            if (!string.IsNullOrEmpty(((System.Xml.Linq.XElement)nd).Value))
                            {
                                double.TryParse(((System.Xml.Linq.XElement)nd).Value.Replace("in", ""), out FirstTablixHeight);
                            }
                        }
                    }
                }
                if ((doc.Descendants(ns + "Tablix").ToList())[1] != null)
                {
                    foreach (var nd in (doc.Descendants(ns + "Tablix").ToList())[1].Nodes().ToList())
                    {
                        string nodeName = ((System.Xml.Linq.XElement)nd).Name.LocalName;
                        if (nodeName == "Top")
                        {
                            FirstTablixHeight = FirstTablixHeight + 0.5;
                            isTopNodeFound = true;
                            ((System.Xml.Linq.XElement)nd).Value = FirstTablixHeight + "in";
                        }
                    }
                    if (!isTopNodeFound)
                    {
                        FirstTablixHeight = FirstTablixHeight + 0.5;
                        (doc.Descendants(ns + "Tablix").ToList())[1].Add(new XElement(ns + "Top", FirstTablixHeight + "in"));
                    }
                }
            }
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
            if (isReceivedItemReport)
            {
                IEnumerable<XElement> lstUpdateTablix = UpdateItemReceivedReceivableResource(lstTablix, MasterReportResFile, oReportBuilderDTO.CombineReportID);
            }
            else
            {
                IEnumerable<XElement> lstUpdateTablix = UpdateResource(lstTablix, MasterReportResFile);
            }
            if (objDTO.ReportType == 3 && !hasSubReport && LocalDictRptPara.ContainsKey("SortFields")
                && !string.IsNullOrEmpty(LocalDictRptPara["SortFields"]))
            {
                string SortFields = LocalDictRptPara["SortFields"];

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
            //doc.Descendants(ns + "Tablix").FirstOrDefault().Value = lstUpdateTablix.FirstOrDefault().Value;
            IEnumerable<XElement> lstReportPara = doc.Descendants(ns + "ReportParameter");
            List<ReportParameter> rpt = new List<ReportParameter>();


            doc = amDAL.AddFormatToTaxbox(doc, rsInfo);
            eTurns.DAL.AlertMail rptHelper = new eTurns.DAL.AlertMail();
            if (LocalDictRptPara.FirstOrDefault(x => x.Key.Replace("@", "").ToLower() == ("IsNoHeader").ToLower()).Value != null
                && (LocalDictRptPara.FirstOrDefault(x => x.Key.Replace("@", "").ToLower() == ("IsNoHeader").ToLower()).Value) == "1")
            {
                //if (!hasSubReport && !objDTO.IsNotEditable.GetValueOrDefault(false)
                //                 && (objDTO.ReportType == 3 || objDTO.ReportType == 1)
                //                 && (LocalDictRptPara.FirstOrDefault(x => x.Key.Replace("@", "").ToLower() == ("IsNoHeader").ToLower()).Value) == "1")
                if (!objDTO.IsNotEditable.GetValueOrDefault(false) && (objDTO.ReportType == 3 || objDTO.ReportType == 1 || objDTO.ReportType == 2))
                {
                    doc = rptHelper.GetAdditionalHeaderRow(doc, objDTO, SessionHelper.CompanyName, SessionHelper.RoomName, EnterpriseDBName: SessionHelper.EnterPriseDBName);
                    doc.Save(ReportPath);
                    doc = XDocument.Load(ReportPath);
                }
                else
                {
                    if (!objDTO.IsNotEditable.GetValueOrDefault(false)
                           && (objDTO.ReportType == 1 || objDTO.ReportType == 2 || objDTO.ReportType == 3))
                    {
                        doc = objAlertMail.GetAdditionalHeaderRowWithDateRange(doc, objDTO, SessionHelper.CompanyName, SessionHelper.RoomName, LocalDictRptPara, EnterpriseDBName: SessionHelper.EnterPriseDBName);
                        doc.Save(ReportPath);
                        doc = XDocument.Load(ReportPath);
                    }
                }
            }
            else if (!objDTO.IsNotEditable.GetValueOrDefault(false) && (objDTO.ReportType == 3 || objDTO.ReportType == 1 || objDTO.ReportType == 2))
            {
                doc = rptHelper.GetAdditionalHeaderRowWithDateRange(doc, objDTO, SessionHelper.CompanyName, SessionHelper.RoomName, EnterpriseDBName: SessionHelper.EnterPriseDBName);
                doc.Save(ReportPath);
                doc = XDocument.Load(ReportPath);
            }

            string[] arrRooms = null;

            if (LocalDictRptPara.Keys.Contains("RoomIDs"))
            {
                string strRoomIDs = LocalDictRptPara["RoomIDs"];
                arrRooms = strRoomIDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }

            if (arrRooms != null && arrRooms.Length > 1 && !(LocalDictRptPara.Keys.Contains("IsNoHeader") && LocalDictRptPara["IsNoHeader"] == "1")
                   && objDTO.ModuleName.ToLower() != "room")
            {
                if (objDTO.ReportType != 2 && doc.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Textbox").Descendants(ns + "Value").Count() > 0)
                {
                    string strRoomCompany = "";
                    RoomDAL rDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
                    List<RoomDTO> RoomDTO = rDAL.GetRoomByIDsNormal(LocalDictRptPara["RoomIDs"]).ToList();
                    List<string> lstCompanies = RoomDTO.Select(x => x.CompanyName).Distinct().ToList();
                    List<string> lstRooms = RoomDTO.Select(x => x.RoomName).Distinct().ToList();
                    string strCompanies = string.Join(", ", lstCompanies);
                    string strRooms = string.Join(", ", lstRooms);
                    strRoomCompany = strCompanies + "\r\n" + strRooms;
                    if (doc.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Textbox").Descendants(ns + "Value").FirstOrDefault(x => x.Value.Contains("Fields!RoomInfo.Value")) != null)
                        doc.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Textbox").Descendants(ns + "Value").FirstOrDefault(x => x.Value.Contains("Fields!RoomInfo.Value")).Value = strRoomCompany;
                    doc.Save(ReportPath);
                }
            }

            if (LocalDictRptPara.FirstOrDefault(x => x.Key.Replace("@", "").ToLower() == ("ShowSignature").ToLower()).Value != null && (LocalDictRptPara.FirstOrDefault(x => x.Key.Replace("@", "").ToLower() == ("ShowSignature").ToLower()).Value) == "1")
            {
                doc = rptHelper.GetFooterForSignature(doc, objDTO);
                doc.Save(ReportPath);
                doc = XDocument.Load(ReportPath);
            }
            doc.Save(ReportPath);
            doc = XDocument.Load(ReportPath);

            if (doc.Descendants(ns + "Subreport") != null && doc.Descendants(ns + "Subreport").Count() > 0)
            {
                hasSubReport = true;
            }

            if (lstReportPara != null && lstReportPara.Count() > 0)
            {
                if (oReportBuilderDTO.ReportName.ToLower().Equals("pull summary by quarter"))
                {
                    string startDateStr = string.Empty;
                    string endDateStr = string.Empty;

                    //if (LocalDictRptPara.Keys.Contains("OrigStartDate"))
                    //    startDateStr = Convert.ToString(LocalDictRptPara["OrigStartDate"]);

                    //if (LocalDictRptPara.Keys.Contains("OrigEndDate"))
                    //    endDateStr = Convert.ToString(LocalDictRptPara["OrigEndDate"]);

                    if (LocalDictRptPara.Keys.Contains("StartDate"))
                        startDateStr = Convert.ToString(LocalDictRptPara["StartDate"]);

                    if (LocalDictRptPara.Keys.Contains("EndDate"))
                        endDateStr = Convert.ToString(LocalDictRptPara["EndDate"]);

                    DateTime startDate, endDate;
                    string tmpStartDtStr = startDateStr.Split(' ')[0];
                    //DateTime.TryParseExact(tmpStartDtStr, "yyyy-MM-dd", ResourceHelper.CurrentCult, System.Globalization.DateTimeStyles.None, out startDate);
                    DateTime.TryParse(tmpStartDtStr, out startDate);
                    string tmpEndDtStr = endDateStr.Split(' ')[0];
                    //DateTime.TryParseExact(tmpEndDtStr, "yyyy-MM-dd", ResourceHelper.CurrentCult, System.Globalization.DateTimeStyles.None, out endDate);
                    DateTime.TryParse(tmpEndDtStr, out endDate);
                    int startDateMonth, endDateMonth, startDateYear, endDateYear;
                    List<string> monthsColumnToKeep = new List<string>();
                    List<string> monthsColumnToKeepValues = new List<string>();
                    List<string> monthsColumns = new List<string>();
                    List<string> monthsColumnsValuesStr = new List<string>();
                    List<string> monthVisibilityReportParams = new List<string>();
                    List<string> monthHeaderReportParams = new List<string>();
                    List<string> monthColumnReportParams = new List<string>();
                    List<int> monthSequence = new List<int>();
                    Dictionary<int, int> dictMonthYear = new Dictionary<int, int>();

                    for (int month = 1; month <= 12; month++)
                    {
                        monthsColumns.Add("Month" + month);
                        monthsColumnsValuesStr.Add("=Fields!Month" + month + ".Value"); //=Fields!Month12.Value
                        monthVisibilityReportParams.Add("IsMonth" + month + "Visible");
                        monthHeaderReportParams.Add("Month" + month + "Header");
                        monthColumnReportParams.Add("Month" + month + "Column");
                    }

                    if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
                    {
                        startDateMonth = startDate.Month;
                        startDateYear = startDate.Year;
                        endDateMonth = endDate.Month;
                        endDateYear = endDate.Year;

                        if (startDateMonth == endDateMonth && startDateYear == endDateYear)
                        {
                            monthsColumnToKeep.Add("Month" + startDateMonth);
                            monthsColumnToKeepValues.Add("=Fields!Month" + startDateMonth + ".Value"); //=Fields!Month12.Value
                            monthSequence.Add(startDateMonth);

                            if (!dictMonthYear.ContainsKey(startDateMonth))
                            {
                                dictMonthYear.Add(startDateMonth, startDateYear);
                            }
                        }
                        else
                        {
                            if (startDateYear == endDateYear)
                            {
                                for (int monthCounter = startDateMonth; monthCounter <= endDateMonth; monthCounter++)
                                {
                                    monthsColumnToKeep.Add("Month" + monthCounter);
                                    monthsColumnToKeepValues.Add("=Fields!Month" + monthCounter + ".Value"); //=Fields!Month12.Value
                                    monthSequence.Add(monthCounter);

                                    if (!dictMonthYear.ContainsKey(monthCounter))
                                    {
                                        dictMonthYear.Add(monthCounter, startDateYear);
                                    }
                                }
                            }
                            else
                            {
                                int monthCnt = 0;

                                for (int monthCounter = startDateMonth; monthCounter <= 24; monthCounter++)
                                {
                                    int tmpMonth = monthCounter;
                                    int tmpYear = startDateYear;

                                    if (tmpMonth > 12)
                                    {
                                        tmpMonth -= 12;
                                        tmpYear = endDateYear;
                                    }

                                    monthsColumnToKeep.Add("Month" + tmpMonth);
                                    monthsColumnToKeepValues.Add("=Fields!Month" + tmpMonth + ".Value"); //=Fields!Month12.Value
                                    monthSequence.Add(tmpMonth);

                                    if (!dictMonthYear.ContainsKey(tmpMonth))
                                    {
                                        dictMonthYear.Add(tmpMonth, tmpYear);
                                    }

                                    monthCnt++;

                                    if (tmpMonth == endDateMonth && monthCnt > 1)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    string monthStrToCompareWithParam = string.Empty;
                    string monthVSValue = string.Empty;

                    foreach (var item in lstReportPara)
                    {
                        ReportParameter rpara = new ReportParameter();
                        rpara.Name = item.Attribute("Name").Value;

                        if (monthVisibilityReportParams.Contains(rpara.Name))
                        {
                            monthStrToCompareWithParam = rpara.Name.Replace("IsMonth", "").Replace("Visible", "");

                            if (!string.IsNullOrEmpty(monthStrToCompareWithParam) && !string.IsNullOrWhiteSpace(monthStrToCompareWithParam))
                            {
                                int tmpMonth;
                                int.TryParse(monthStrToCompareWithParam, out tmpMonth);

                                monthVSValue = monthSequence.Count >= tmpMonth ? "true" : "false";
                                rpara.Values.Add(monthVSValue);
                            }
                        }
                        else if (monthHeaderReportParams.Contains(rpara.Name))
                        {
                            monthStrToCompareWithParam = rpara.Name.Replace("Month", "").Replace("Header", "");

                            if (!string.IsNullOrEmpty(monthStrToCompareWithParam) && !string.IsNullOrWhiteSpace(monthStrToCompareWithParam))
                            {
                                int tmpMonth;

                                if (int.TryParse(monthStrToCompareWithParam, out tmpMonth))
                                {
                                    var monthNo = monthSequence.Count >= tmpMonth ? monthSequence[tmpMonth - 1] : 1;
                                    var monthStr = "Month" + monthNo;
                                    var monthNameFromResource = GetResourceValue(MasterReportResFile, monthStr);

                                    if (dictMonthYear.ContainsKey(monthNo) && !string.IsNullOrEmpty(monthNameFromResource) &&
                                        !string.IsNullOrWhiteSpace(monthNameFromResource))
                                    {
                                        monthNameFromResource += (" " + Convert.ToString(dictMonthYear[monthNo]));
                                    }

                                    rpara.Values.Add(monthNameFromResource);
                                }
                            }
                        }
                        else if (monthColumnReportParams.Contains(rpara.Name))
                        {
                            monthStrToCompareWithParam = rpara.Name.Replace("Month", "").Replace("Column", "");

                            if (!string.IsNullOrEmpty(monthStrToCompareWithParam) && !string.IsNullOrWhiteSpace(monthStrToCompareWithParam))
                            {
                                int tmpMonth;

                                if (int.TryParse(monthStrToCompareWithParam, out tmpMonth))
                                {
                                    var monthNo = monthSequence.Count >= tmpMonth ? monthSequence[tmpMonth - 1] : 1;
                                    var monthColumnName = "Month" + monthNo;
                                    rpara.Values.Add(monthColumnName);
                                }
                            }

                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(LocalDictRptPara.FirstOrDefault(x => x.Key.Replace("@", "") == item.Attribute("Name").Value.Replace("@", "")).Value))
                            {
                                rpara.Values.Add(LocalDictRptPara.FirstOrDefault(x => x.Key.Replace("@", "") == item.Attribute("Name").Value.Replace("@", "")).Value);
                            }
                        }

                        rpt.Add(rpara);
                    }
                }
                else
                {
                    foreach (var item in lstReportPara)
                    {
                        ReportParameter rpara = new ReportParameter();
                        rpara.Name = item.Attribute("Name").Value;
                        if (!string.IsNullOrEmpty(LocalDictRptPara.FirstOrDefault(x => x.Key.Replace("@", "") == item.Attribute("Name").Value.Replace("@", "")).Value))
                            rpara.Values.Add(LocalDictRptPara.FirstOrDefault(x => x.Key.Replace("@", "") == item.Attribute("Name").Value.Replace("@", "")).Value);

                        rpt.Add(rpara);
                    }
                }                
            }

            string connString = doc.Descendants(ns + "ConnectString").FirstOrDefault().Value;
            SqlConnection myConnection = new SqlConnection();
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter sqla = new SqlDataAdapter();
            DataTable dt = new DataTable();
            DataTable dtReceivablesItems = new DataTable();
            myConnection.ConnectionString = connectionString;// connString;



            if (isReceivedItemReport)
            {
                List<string> lstDataSet1Fields = new List<string>();
                List<string> lstDS_ReceivablesItemsFields = new List<string>();
                foreach (var ds in doc.Descendants(ns + "DataSet").ToList())
                {
                    if (ds.FirstAttribute.Value == "DataSet1")
                    {
                        foreach (var field in ds.Descendants(ns + "Fields").Descendants(ns + "Field").ToList())
                        {
                            lstDataSet1Fields.Add(field.FirstAttribute.Value);
                        }
                    }
                    if (ds.FirstAttribute.Value == "DS_ReceivablesItems")
                    {
                        foreach (var field in ds.Descendants(ns + "Fields").Descendants(ns + "Field").ToList())
                        {
                            lstDS_ReceivablesItemsFields.Add(field.FirstAttribute.Value);
                        }
                    }
                }

                for (int i = 0; i < doc.Descendants(ns + "CommandText").ToList().Count(); i++)
                {
                    var cmdt = doc.Descendants(ns + "CommandText").ToList()[i];
                    SqlCommand cmdReceived = new SqlCommand();
                    SqlDataAdapter sqlaReceived = new SqlDataAdapter();

                    cmdReceived.Connection = myConnection;
                    cmdReceived.CommandText = cmdt.Value;
                    cmdReceived.CommandType = CommandType.Text;
                    if (doc.Descendants(ns + "CommandType").FirstOrDefault() != null)
                    {
                        cmdReceived.CommandType = (CommandType)Enum.Parse(typeof(CommandType), doc.Descendants(ns + "CommandType").FirstOrDefault().Value == null ? "Text" : doc.Descendants(ns + "CommandType").FirstOrDefault().Value, true);
                    }
                    IEnumerable<XElement> lstQueryPara = doc.Descendants(ns + "QueryParameter");

                    if (lstQueryPara != null && lstQueryPara.Count() > 0)
                    {
                        foreach (var item in lstQueryPara)
                        {
                            SqlParameter slpar = new SqlParameter();
                            slpar.ParameterName = item.Attribute("Name").Value;//
                            if (!(hasSubReport && slpar.ParameterName.ToLower().Replace("@", "") == "sortfields") && !string.IsNullOrEmpty(LocalDictRptPara.FirstOrDefault(x => x.Key.ToLower().Replace("@", "") == item.Attribute("Name").Value.Replace("@", "").ToLower()).Value))
                            {
                                if (slpar.ParameterName.ToLower().Replace("@", "") == "sortfields")
                                {
                                    string SortFieldName = LocalDictRptPara.FirstOrDefault(x => x.Key.Replace("@", "").ToLower() == item.Attribute("Name").Value.Replace("@", "").ToLower()).Value;
                                    string[] arrSortFieldsDS = SortFieldName.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                                    if (cmdReceived.CommandText.ToLower().Equals("rpt_get_receive_orderitems"))
                                    {
                                        for (int isr = 0; isr < arrSortFieldsDS.Length; isr++)
                                        {
                                            if (lstDataSet1Fields.Where(x => arrSortFieldsDS[isr].Trim().Replace(" asc", "").Replace(" desc", "").Replace(" ASC", "").Replace(" DESC", "").ToLower().Equals(x.ToLower())).ToList().Count <= 0)
                                            {
                                                SortFieldName = SortFieldName.Replace(arrSortFieldsDS[isr] + ",", "");
                                                SortFieldName = SortFieldName.Replace(arrSortFieldsDS[isr], "");
                                            }
                                        }

                                        if (!string.IsNullOrWhiteSpace(SortFieldName))
                                        {
                                            slpar.Value = SortFieldName;
                                        }
                                        else
                                        {
                                            slpar.Value = DBNull.Value;
                                        }
                                    }
                                    else if (cmdReceived.CommandText.ToLower().Equals("rpt_getreceivableitems"))
                                    {
                                        for (int isr = 0; isr < arrSortFieldsDS.Length; isr++)
                                        {
                                            if (lstDS_ReceivablesItemsFields.Where(x => arrSortFieldsDS[isr].Trim().Replace(" asc", "").Replace(" desc", "").Replace(" ASC", "").Replace(" DESC", "").ToLower().Equals(x.ToLower())).ToList().Count <= 0)
                                            {
                                                SortFieldName = SortFieldName.Replace(arrSortFieldsDS[isr] + ",", "");
                                                SortFieldName = SortFieldName.Replace(arrSortFieldsDS[isr], "");
                                            }
                                        }

                                        if (!string.IsNullOrWhiteSpace(SortFieldName))
                                        {
                                            slpar.Value = SortFieldName;
                                        }
                                        else
                                        {
                                            slpar.Value = DBNull.Value;
                                        }
                                    }
                                    else
                                        slpar.Value = DBNull.Value;
                                }
                                else
                                {
                                    slpar.Value = LocalDictRptPara.FirstOrDefault(x => x.Key.Replace("@", "").ToLower() == item.Attribute("Name").Value.Replace("@", "").ToLower()).Value;
                                }
                            }
                            else
                                slpar.Value = DBNull.Value;
                            XElement objReportPara = lstReportPara.FirstOrDefault(x => x.Attribute("Name").Value.ToLower() == slpar.ParameterName.ToLower().Replace("@", ""));

                            if (objReportPara.Descendants(ns + "DataType") != null && objReportPara.Descendants(ns + "DataType").Count() > 0)
                            {
                                slpar.DbType = (DbType)Enum.Parse(typeof(DbType), objReportPara.Descendants(ns + "DataType").FirstOrDefault().Value, true);
                            }
                            if (cmdReceived.Parameters.IndexOf(slpar.ParameterName) < 0)
                            {
                                if (cmdReceived.CommandText.ToLower().Equals("rpt_get_receive_orderitems"))
                                {
                                    if (!slpar.ParameterName.ToLower().Equals("@ids"))
                                        cmdReceived.Parameters.Add(slpar);
                                }
                                else
                                {
                                    if (!slpar.ParameterName.ToLower().Equals("@startdate")
                                        && !slpar.ParameterName.ToLower().Equals("@enddate"))
                                        cmdReceived.Parameters.Add(slpar);
                                }
                            }
                        }
                    }
                    cmdReceived.CommandTimeout = 7200;
                    sqlaReceived = new SqlDataAdapter(cmdReceived);
                    if (cmdReceived.CommandText.ToLower().Equals("rpt_getreceivableitems"))
                    {
                        sqlaReceived.Fill(dtReceivablesItems);
                    }
                    else if (cmdReceived.CommandText.ToLower().Equals("rpt_get_receive_orderitems"))
                    {
                        sqlaReceived.Fill(dt);
                    }

                    sqlaReceived.Dispose();
                }
            }
            else
            {
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
                        if (!(hasSubReport && slpar.ParameterName.ToLower().Replace("@", "") == "sortfields") && !string.IsNullOrEmpty(LocalDictRptPara.FirstOrDefault(x => x.Key.ToLower().Replace("@", "") == item.Attribute("Name").Value.Replace("@", "").ToLower()).Value))
                            slpar.Value = LocalDictRptPara.FirstOrDefault(x => x.Key.Replace("@", "").ToLower() == item.Attribute("Name").Value.Replace("@", "").ToLower()).Value;
                        else
                            slpar.Value = DBNull.Value;
                        XElement objReportPara = lstReportPara.FirstOrDefault(x => x.Attribute("Name").Value.ToLower() == slpar.ParameterName.ToLower().Replace("@", ""));

                        if (objReportPara.Descendants(ns + "DataType") != null && objReportPara.Descendants(ns + "DataType").Count() > 0)
                        {
                            slpar.DbType = (DbType)Enum.Parse(typeof(DbType), objReportPara.Descendants(ns + "DataType").FirstOrDefault().Value, true);
                        }

                        cmd.Parameters.Add(slpar);
                    }
                }
                sqla = new SqlDataAdapter(cmd);
                cmd.CommandTimeout = 120;
                sqla.Fill(dt);
            }
            rptLocalReport.DataSources.Clear();
            //ReportViewer1.Reset();
            //ReportViewer1.LocalReport.DataSources.Clear();
            //ReportViewer1.Visible = true;
            //string strSubTablix = string.Empty;
            //string rdlPath = string.Empty;
            if (hasSubReport)
            {
                hasSubReport = true;
                //rdlPath = Server.MapPath(@"/RDLC_Reports/" + SessionHelper.EnterPriceID.ToString() + "") + @"\\" + SubReportname  ;
                if (objDTO.ParentID > 0)
                {
                    if (objDTO.ISEnterpriseReport.GetValueOrDefault(false))
                    {
                        rdlPath = CopyFiletoTemp(RDLCBaseFilePath + @"/" + SessionHelper.EnterPriceID.ToString() + "/EnterpriseReport" + @"\\" + SubReportname, subGuid);
                    }
                    else
                    {
                        rdlPath = CopyFiletoTemp(RDLCBaseFilePath + @"/" + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID + @"\\" + SubReportname, subGuid);
                    }
                }
                else
                {

                    rdlPath = CopyFiletoTemp(RDLCBaseFilePath + @"/" + SessionHelper.EnterPriceID.ToString() + "/BaseReport" + @"\\" + SubReportname, subGuid);
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
                IEnumerable<XElement> lstUpdateSubTablix = UpdateResource(lstSubTablix, SubReportResFile);

                //TODO: Start WI-6271:
                ReportBuilderDTO oReportBuilderOrderDTO = objDAL.GetParentReportMasterByReportID(objDTO.ID);
                if (oReportBuilderOrderDTO != null)
                {
                    if (oReportBuilderOrderDTO.ReportName.ToLower().Equals("work order with grouped pulls"))
                    {
                        if (objDTO.ReportType == 2 && hasSubReport
                        && LocalDictRptPara.ContainsKey("SortFields")
                        && !string.IsNullOrEmpty(LocalDictRptPara["SortFields"]))
                        {
                            string SortFields = LocalDictRptPara["SortFields"];

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

                docSub = amDAL.AddFormatToTaxbox(docSub, rsInfo);
                docSub.Save(rdlPath);
                docSub = XDocument.Load(rdlPath);

                rptLocalReport.SubreportProcessing += new SubreportProcessingEventHandler(LoadSubreport);
                rptLocalReport.EnableExternalImages = true;
                rptLocalReport.EnableHyperlinks = true;
                //ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(LoadSubreport);

                //ReportViewer1.LocalReport.EnableExternalImages = true;
                //  ReportViewer1.LocalReport.Refresh();
            }

            rptLocalReport.EnableExternalImages = true;
            rptLocalReport.ReportPath = ReportPath;
            //ReportViewer1.LocalReport.EnableExternalImages = true;
            //ReportViewer1.LocalReport.ReportPath = ReportPath;
            if (isReceivedItemReport)
            {
                foreach (var dsname in doc.Descendants(ns + "DataSet").ToList())
                {
                    ReportDataSource rds = new ReportDataSource();
                    rds.Name = dsname.FirstAttribute.Value;
                    if (rds.Name.Equals("DS_ReceivablesItems"))
                    {
                        rds.Value = dtReceivablesItems;
                    }
                    else
                    {
                        rds.Value = dt;
                    }
                    rptLocalReport.DataSources.Add(rds);
                    globalcounter = dt.Rows.Count;
                }
            }
            else
            {
                ReportDataSource rds = new ReportDataSource();
                rds.Name = doc.Descendants(ns + "DataSet").FirstOrDefault().FirstAttribute.Value;
                rds.Value = dt;
                globalcounter = dt.Rows.Count;
                rptLocalReport.DataSources.Add(rds);
            }
            rptLocalReport.SetParameters(rpt);
            rptLocalReport.Refresh();            
            return Export(rptLocalReport, FileType);


            //PrinterSettings settings = new PrinterSettings();
            //foreach (string printer in PrinterSettings.InstalledPrinters)
            //{
            //    settings.PrinterName = printer;
            //    if (settings.IsDefaultPrinter)
            //        Print(printer);
            //}

        }
        public string CopyFiletoTemp(string strfile, string reportname)
        {

            string ReportTempPath = string.Empty;
            string ReportRetPath = string.Empty;
            string RDLCBaseFilePath = CommonUtility.RDLCBaseFilePath;
            ReportTempPath = RDLCBaseFilePath + @"/Temp";

            if (!System.IO.Directory.Exists(ReportTempPath))
            {
                System.IO.Directory.CreateDirectory(ReportTempPath);
            }
            ReportRetPath = ReportTempPath + @"/" + reportname + ".rdlc";
            System.IO.File.Copy(strfile, ReportRetPath);
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

                IEnumerable<XElement> lstTableHeader = Table.Descendants(ns + "TablixHeader");
                if (lstTableHeader != null && lstTableHeader.Count() > 0)
                {
                    foreach (XElement Cell in lstTableHeader)
                    {
                        if (Cell.Descendants(ns + "Value").Any())
                        {
                            Cell.Descendants(ns + "Value").FirstOrDefault().Value = GetResourceValue(ResFile, Cell.Descendants(ns + "Value").FirstOrDefault().Value);
                        }
                    }
                }
            }

            return lstTablix;
        }
        public IEnumerable<XElement> UpdateItemReceivedReceivableResource(IEnumerable<XElement> lstTablix, string ResFile, string CombineReportID)
        {
            string[] ReportIDs = null;
            if (!string.IsNullOrWhiteSpace(CombineReportID))
            {
                ReportIDs = CombineReportID.Split(',');
            }
            bool isReceivedReport = false;
            bool isReceivableReport = false;
            string ReceivedResourcFileName = "";
            string ReceivableResourcFileName = "";
            if (ReportIDs != null && ReportIDs.Length > 0)
            {
                ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                for (int i = 0; i < ReportIDs.Length; i++)
                {
                    ReportBuilderDTO reportBuilderDTO = objDAL.GetParentReportDetailByReportID(Convert.ToInt32(ReportIDs[i]));
                    if (reportBuilderDTO != null && reportBuilderDTO.ID > 0)
                    {
                        if (reportBuilderDTO.ReportName.ToLower().Equals("received items"))
                        {
                            ReceivedResourcFileName = reportBuilderDTO.MasterReportResFile;
                            isReceivedReport = true;
                        }
                        if (reportBuilderDTO.ReportName.ToLower().Equals("receivable items"))
                        {
                            ReceivableResourcFileName = reportBuilderDTO.MasterReportResFile;
                            isReceivableReport = true;
                        }
                    }
                }
            }

            foreach (XElement Table in lstTablix)
            {
                if (Table.LastAttribute.Value == "Tablix1")
                {
                    ResFile = ReceivedResourcFileName;
                }
                else if (Table.LastAttribute.Value == "Tablix2")
                { ResFile = ReceivableResourcFileName; }
                IEnumerable<XElement> lstTableCell = Table.Descendants(ns + "TablixCell");
                foreach (XElement Cell in lstTableCell)
                {
                    if (Cell.Descendants(ns + "Value").Any())
                    {
                        Cell.Descendants(ns + "Value").FirstOrDefault().Value = GetResourceValue(ResFile, Cell.Descendants(ns + "Value").FirstOrDefault().Value);
                    }
                }

                IEnumerable<XElement> lstTableHeader = Table.Descendants(ns + "TablixHeader");
                if (lstTableHeader != null && lstTableHeader.Count() > 0)
                {
                    foreach (XElement Cell in lstTableHeader)
                    {
                        if (Cell.Descendants(ns + "Value").Any())
                        {
                            Cell.Descendants(ns + "Value").FirstOrDefault().Value = GetResourceValue(ResFile, Cell.Descendants(ns + "Value").FirstOrDefault().Value);
                        }
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
            //if (!string.IsNullOrEmpty(Key) && Key.ToLower().Contains("itemudf") && Key.ToLower().Contains("=Fields!"))
            //{
            //    KeyVal = ResourceHelper.GetResourceValue(Key.Replace("Item", ""), "ResItemMaster");
            //    if (KeyVal == Key)
            //    {
            //        KeyVal = ResourceHelper.GetResourceValue(Key, FileName);
            //    }
            //}
            //else if (!string.IsNullOrEmpty(Key) && Key.ToLower() == "pulludf" && Key.ToLower().Contains("=Fields!"))
            //{
            //    KeyVal = ResourceHelper.GetResourceValue(Key.Replace("Pull", ""), "ResPullMaster");
            //    if (KeyVal == Key)
            //    {
            //        KeyVal = ResourceHelper.GetResourceValue(Key, FileName);
            //    }
            //}
            //else
            //  KeyVal = ResourceHelper.GetResourceValue(Key, FileName);
            if (Key.ToLower().Contains("udf"))
            {
                string PreFixValue = Key.Replace("UDF10", string.Empty).Replace("UDF1", string.Empty).Replace("UDF2", string.Empty).Replace("UDF3", string.Empty).Replace("UDF4", string.Empty).Replace("UDF5", string.Empty).Replace(".Value", string.Empty).Replace("UDF6", string.Empty).Replace("UDF7", string.Empty).Replace("UDF8", string.Empty).Replace("UDF9", string.Empty);
                KeyVal = ResourceHelper.GetResourceValue(Key, FileName, true);
                if (!KeyVal.Contains(PreFixValue))
                    KeyVal = PreFixValue + " " + KeyVal;
            }
            else
            {
                KeyVal = ResourceHelper.GetResourceValue(Key, FileName);
            }
            //    switch (Key.ToLower())
            //{
            //    case "udf1":
            //    case "udf2":
            //    case "udf3":
            //    case "udf4":
            //    case "udf5":
            //    case "udf6":
            //    case "udf7":
            //    case "udf8":
            //    case "udf9":
            //    case "udf10":
            //        KeyVal = ResourceHelper.GetResourceValue(Key, FileName, true);
            //        break;
            //    default:
            //        KeyVal = ResourceHelper.GetResourceValue(Key, FileName);
            //        break;
            //}

            return KeyVal;
        }

    }
}