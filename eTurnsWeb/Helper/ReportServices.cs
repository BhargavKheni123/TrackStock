using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using Microsoft.Reporting.WebForms;
//using Microsoft.SqlServer.ReportingServices2005.Execution;

//using eTurnsWeb.localhost2012;
//using eTurnsWeb.rsExecution2012;
using System.Xml;
using System.Configuration;


/// <summary>
/// This Reporting service contains Methods to work with ReportingService2005
/// For This some of sequence to Fill Report
///  1) Assign Report Server URL to Report Viewer
///  2) Netowork Authentication
///  3) Get Report Definition
///  4) Fill Definition to Sql Report viewer
///  5) Database Authentication
/// </summary>

namespace eTurnsWeb.Helper
{
    public class ReportServices
    {
        #region Variable declaration

        ReportingService2010 objReportingService2005 = new ReportingService2010();
        ReportExecutionService objReportExecutionService = new ReportExecutionService();

        #endregion

        #region Properties

        string _Domain;
        public string Domain
        {
            get
            {
                return _Domain;
            }
            set
            {
                _Domain = value;
            }
        }

        string _UserName;
        public string Username
        {
            get
            {
                return _UserName;
            }
            set
            {
                _UserName = value;
            }
        }

        string _Password;
        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                _Password = value;
            }
        }

        string _DatabaseUsername;
        public string DatabaseUsername
        {
            get
            {
                return _DatabaseUsername;
            }
            set
            {
                _DatabaseUsername = value;
            }
        }

        string _DatabasePassword;
        public string DatabasePassword
        {
            get
            {
                return _DatabasePassword;
            }
            set
            {
                _DatabasePassword = value;
            }
        }

        #endregion

        #region Constructor

        public ReportServices(ReportingService2010 reportingService2005, string Domain, string Username, string Password, string DatabaseUsername, string DatabasePassword)
        {
            try
            {
                objReportingService2005 = reportingService2005;
                _Domain = Domain;
                _UserName = Username;
                _Password = Password;

                _DatabaseUsername = DatabaseUsername;
                _DatabasePassword = DatabasePassword;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        /// <summary>
        /// Get All Reports From Report sever
        /// </summary>
        /// <param name="rs"> Object of ReportingService2005</param>
        /// <returns>Data table with two columns Report Name , Report Path</returns>
        public DataTable GetReportList()
        {
            DataTable dtReport = new DataTable();
            try
            {
                string FolderName = "/" + System.Configuration.ConfigurationManager.AppSettings["ReportFolder_Reports"];
                DoNetworkAuthentication();
                List<string> reportPaths = new List<string>();
                CatalogItem[] catalogItems;
                catalogItems = objReportingService2005.ListChildren(FolderName, true);
                dtReport.Columns.Add("ReportName");
                dtReport.Columns.Add("ReportPath");
                for (int i = 0; i < catalogItems.Length; i++)
                {
                    if (catalogItems[i].TypeName == "Report")
                    {
                        DataRow dr = dtReport.NewRow();
                        reportPaths.Add(catalogItems[i].Path);
                        dr["ReportName"] = catalogItems[i].Name;
                        dr["ReportPath"] = catalogItems[i].Path;
                        dtReport.Rows.Add(dr);
                    }
                }
                return dtReport;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                dtReport.Dispose();
            }

        }

        public Dictionary<string, string> GetTransactionReportList()
        {
            Dictionary<string, string> dtReport = new Dictionary<string, string>();
            try
            {
                string TempReportType = "";
                if (System.Configuration.ConfigurationManager.AppSettings["ReportFolder_Transaction"] == null)
                    TempReportType = "Transaction";
                else
                    TempReportType = System.Configuration.ConfigurationManager.AppSettings["ReportFolder_Transaction"];

                string FolderName = "/" + TempReportType;
                string FolderNameMaster = "/" + System.Configuration.ConfigurationManager.AppSettings["ReportFolder_Reports"];

                DoNetworkAuthentication();
                Dictionary<string, string> reportNames = GetStaticReportName();
                CatalogItem[] catalogItems;
                catalogItems = objReportingService2005.ListChildren(FolderNameMaster, true);
                for (int i = 0; i < catalogItems.Length; i++)
                {
                    if (catalogItems[i].TypeName == "Report")
                    {
                        if (reportNames.ContainsKey(catalogItems[i].Path))
                            dtReport.Add(reportNames[catalogItems[i].Path], catalogItems[i].Path);
                    }
                }
                catalogItems = objReportingService2005.ListChildren(FolderName, true);
                for (int i = 0; i < catalogItems.Length; i++)
                {
                    if (catalogItems[i].TypeName == "Report")
                    {
                        if (reportNames.ContainsKey(catalogItems[i].Path))
                            dtReport.Add(reportNames[catalogItems[i].Path], catalogItems[i].Path);
                    }
                }

                return dtReport;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public Dictionary<string, string> GetStaticReportName()
        {
            Dictionary<string, string> lstReportName = new Dictionary<string, string>();
            lstReportName.Add("/Supporting Information/BinLocation", "Bin Location");
            lstReportName.Add("/Supporting Information/Category", "Category");
            lstReportName.Add("/Supporting Information/Customers", "Customers");
            lstReportName.Add("/Supporting Information/FreightTypes", "FreightTypes");
            lstReportName.Add("/Supporting Information/GLAccounts", "GLAccounts");
            lstReportName.Add("/Supporting Information/Locations", "Locations");
            lstReportName.Add("/Supporting Information/Manufacturers", "Manufacturers");
            lstReportName.Add("/Supporting Information/MeasurementTerms", "Measurement Terms");
            lstReportName.Add("/Supporting Information/ShipVia", "ShipVia");
            lstReportName.Add("/Supporting Information/Supplier", "Supplier");
            lstReportName.Add("/Supporting Information/Technicians", "Technicians");
            lstReportName.Add("/Supporting Information/ToolCategory", "Tool Category");
            lstReportName.Add("/Supporting Information/Units", "Units");
            lstReportName.Add("/Supporting Information/Items", "Items");
            lstReportName.Add("/Supporting Information/CostUOM1", "Cost UOM");
            lstReportName.Add("/Supporting Information/InventoryClassificationMaster", "Inventory Classification");
            lstReportName.Add("/Transaction/PurchaseOrder", "Purchase Order");
            lstReportName.Add("/Transaction/WorkOrder", "Work Order");
            lstReportName.Add("/Transaction/Requisition", "Requisition");
            lstReportName.Add("/Transaction/Pulls", "Pulls");
            lstReportName.Add("/Transaction/Receives_HeaderReport", "Receives");
            lstReportName.Add("/Transaction/OrderedItems_HeaderReport", "Order Items");
            lstReportName.Add("/Transaction/ItemBarcodeLabelOneColumn", "Item Barcode 1 Column");
            lstReportName.Add("/Transaction/ItemBarcodeLabelTwoColumn", "Item Barcode 2 Column");
            return lstReportName;
        }

        /// <summary>
        /// Authentication of Network User
        /// </summary>
        /// <param name="rs"> Object of ReportingService2005</param>
        /// <param name="Username">Name of user</param>
        /// <param name="Password">Passsword</param>
        /// <param name="Domain"> Name of domain</param>
        private void DoNetworkAuthentication()
        {
            //try
            //{
            NetworkCredential objNetworkCredential;
            if (_Domain.Trim() != "")
            {
                objNetworkCredential = new NetworkCredential(_UserName, _Password, _Domain);
            }
            else
            {
                objNetworkCredential = new NetworkCredential(_UserName, _Password);
            }
            objReportingService2005.Credentials = objNetworkCredential;

            objReportExecutionService.Credentials = objNetworkCredential;
            objReportExecutionService.PreAuthenticate = true;
            objReportingService2005.PreAuthenticate = true;

            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }

        /// <summary>
        /// Authentication of Network User
        /// </summary>
        /// <param name="rs"> Object of ReportingService2005</param>
        /// <param name="Username">Name of user</param>
        /// <param name="Password">Passsword</param>
        /// <param name="Domain"> Name of domain</param>
        private void NetworkAuthentication()
        {
            try
            {
                NetworkCredential objNetworkCredential;

                objNetworkCredential = new NetworkCredential(_UserName, _Password);
                //objReportingService2005.Credentials = objNetworkCredential;
                //objReportingService2005.PreAuthenticate = true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Generates the definition of the report.
        /// </summary>
        /// <param name="ReportPath">Name and path of the report for which definition to be generate.</param>
        /// <returns>Stream</returns>
        private Stream GetReportDefinition(string ReportPath)
        {
            //try
            //{
            return (new MemoryStream(objReportingService2005.GetItemDefinition(ReportPath)));
            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }


        //public void FillReport(Microsoft.Reporting.WebForms.ReportViewer sqlReportViewer, string ReportPath)
        //{
        //    Stream stream = GetReportDefinition(ReportPath);
        //    sqlReportViewer.ServerReport.LoadReportDefinition(stream);
        //}

        public void LoadReport(ReportViewer sqlReportViewer, string ReportServerURL, string ReportPath)
        {
            try
            {
                // Assign Report Server URL
                sqlReportViewer.ServerReport.ReportServerUrl = new Uri(ReportServerURL);

                // Network Authentication
                DoNetworkAuthentication();

                // Report Filling
                Stream stream = GetReportDefinition(ReportPath);
                sqlReportViewer.ServerReport.LoadReportDefinition(stream);

                // Get Database Credential
                Microsoft.Reporting.WebForms.DataSourceCredentials myCredential = new Microsoft.Reporting.WebForms.DataSourceCredentials();
                myCredential.UserId = DatabaseUsername;
                myCredential.Password = DatabasePassword;

                Microsoft.Reporting.WebForms.ReportDataSourceInfoCollection reportdatasourcecollection = sqlReportViewer.ServerReport.GetDataSources();
                Microsoft.Reporting.WebForms.DataSourceCredentials[] myCredentials = null;
                if (reportdatasourcecollection.Count > 0)
                {
                    myCredentials = new
                    Microsoft.Reporting.WebForms.DataSourceCredentials[reportdatasourcecollection.Count];
                }

                for (int iIndex = 0; iIndex < reportdatasourcecollection.Count; iIndex++)
                {
                    Microsoft.Reporting.WebForms.ReportDataSourceInfo datasourceinfo =
                    reportdatasourcecollection[iIndex];
                    myCredential.Name = datasourceinfo.Name;
                    myCredentials[iIndex] = myCredential;
                }

                // Assign Database Credential
                sqlReportViewer.ServerReport.SetDataSourceCredentials(myCredentials);

                sqlReportViewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Remote;
                sqlReportViewer.ShowCredentialPrompts = false;
            }
            catch (Exception)
            {

                throw;
            }


        }

        public void LoadReport(ReportViewer sqlReportViewer, string ReportServerURL, string ReportPath, Microsoft.Reporting.WebForms.ReportParameter[] ReportParameter)
        {
            //try
            //{
            // Assign Report Server URL
            string strDomainUserName = ConfigurationManager.AppSettings["NetworkUser"];
            string strDomainName = ConfigurationManager.AppSettings["NetworkDomain"];
            string strDomainPassword = ConfigurationManager.AppSettings["NetworkPassword"];



            CustomReportCredentials myCred = new CustomReportCredentials(strDomainUserName, strDomainPassword, strDomainName);
            sqlReportViewer.ServerReport.ReportServerCredentials = myCred;
            sqlReportViewer.ServerReport.ReportServerUrl = new Uri(ReportServerURL);

            // Network Authentication
            DoNetworkAuthentication();

            // Report Filling
            Stream stream = GetReportDefinition(ReportPath);

            sqlReportViewer.ServerReport.LoadReportDefinition(stream);
            sqlReportViewer.ServerReport.SetParameters(ReportParameter);

            //  ParameterValue[] objParameterValue = new ParameterValue[1];
            //  objParameterValue[0].Label = "";


            // Microsoft.Reporting.WebForms.ReportParameter[] objReportParameter = new Microsoft.Reporting.WebForms.ReportParameter[1];
            //objReportParameter[0] = new Microsoft.Reporting.WebForms.ReportParameter("AcademicYear", "2007-08");


            // Get Database Credential
            Microsoft.Reporting.WebForms.DataSourceCredentials myCredential = new Microsoft.Reporting.WebForms.DataSourceCredentials();
            myCredential.UserId = DatabaseUsername;
            myCredential.Password = DatabasePassword;

            Microsoft.Reporting.WebForms.ReportDataSourceInfoCollection reportdatasourcecollection = sqlReportViewer.ServerReport.GetDataSources();
            Microsoft.Reporting.WebForms.DataSourceCredentials[] myCredentials = null;
            if (reportdatasourcecollection.Count > 0)
            {
                myCredentials = new
                Microsoft.Reporting.WebForms.DataSourceCredentials[reportdatasourcecollection.Count];
            }

            for (int iIndex = 0; iIndex < reportdatasourcecollection.Count; iIndex++)
            {
                Microsoft.Reporting.WebForms.ReportDataSourceInfo datasourceinfo =
                reportdatasourcecollection[iIndex];
                myCredential.Name = datasourceinfo.Name;

                myCredentials[iIndex] = myCredential;
            }

            // Assign Database Credential
            sqlReportViewer.ServerReport.SetDataSourceCredentials(myCredentials);

            sqlReportViewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Remote;
            sqlReportViewer.ShowCredentialPrompts = false;
            sqlReportViewer.ShowParameterPrompts = true;
            sqlReportViewer.ShowPrintButton = true;
            sqlReportViewer.ShowToolBar = true;
            sqlReportViewer.ShowZoomControl = true;

            //}
            //catch (Exception)
            //{

            //    throw;
            //}
        }

        public void CopyReport(string FolderName, string SourceReportName, string DestinationReportName, string ReportPath)
        {
            try
            {

                DoNetworkAuthentication();
                #region Copy Report
                string _reportPath = null;
                _reportPath = ReportPath;

                //Get source report definition as Byte array
                Byte[] reportDefinition = null;
                reportDefinition = objReportingService2005.GetItemDefinition(_reportPath);


                XmlDocument doc = new XmlDocument();
                MemoryStream stream = new MemoryStream(reportDefinition);
                doc.Load(stream);
                XmlNodeList nodes = (doc.GetElementsByTagName("ReportName"));
                foreach (XmlElement element in nodes)
                {
                    element.InnerText = element.InnerText.Replace("Master/Transaction", FolderName).Replace("Master/Supporting Information", FolderName);
                    //element.InnerText = element.InnerText.Replace("Master/Supporting Information", FolderName);
                }

                Byte[] NewreportDefinition = null;
                using (MemoryStream Newstream1 = new MemoryStream())
                {
                    doc.Save(Newstream1);
                    NewreportDefinition = Newstream1.ToArray();
                }

                //for (int i = 0; i < lstVideos.Count; i++)
                //    Console.WriteLine("{0}", lstVideos[i].InnerText);

                //Get report Data source of source report
                eTurnsWeb.localhost2012.DataSource[] ds = null;
                ds = objReportingService2005.GetItemDataSources(_reportPath);

                eTurnsWeb.localhost2012.Property[] prop = null;
                eTurnsWeb.localhost2012.Warning[] warnings = null;

                for (int i = 0; i < ds.Length; i++)
                {
                    if (ds[i].Name != "SSRS2012")
                    {
                        ((eTurnsWeb.localhost2012.DataSourceDefinition)(ds[i].Item)).CredentialRetrieval = CredentialRetrievalEnum.Store;
                        ((eTurnsWeb.localhost2012.DataSourceDefinition)(ds[i].Item)).UserName = ConfigurationManager.AppSettings["DbUserName"].ToString();
                        ((eTurnsWeb.localhost2012.DataSourceDefinition)(ds[i].Item)).Password = ConfigurationManager.AppSettings["DbPassword"].ToString();
                    }
                }
                try
                {
                    objReportingService2005.CreateFolder(FolderName, "/", prop);

                }
                catch (Exception) { }

                eTurnsWeb.localhost2012.CatalogItem copiedReport = objReportingService2005.CreateCatalogItem("Report", DestinationReportName, "/" + FolderName, true, NewreportDefinition, null, out warnings);
                objReportingService2005.SetItemDataSources("/" + FolderName + "/" + DestinationReportName, ds);

                #endregion
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void CopyReportInSameFolder(string FolderName, string SourceReportName, string DestinationReportName, string ReportPath)
        {
            try
            {

                DoNetworkAuthentication();
                #region Copy Report
                string _reportPath = null;
                _reportPath = ReportPath;

                //Get source report definition as Byte array
                Byte[] reportDefinition = null;
                reportDefinition = objReportingService2005.GetItemDefinition(_reportPath);


                XmlDocument doc = new XmlDocument();
                MemoryStream stream = new MemoryStream(reportDefinition);
                doc.Load(stream);
                XmlNodeList nodes = (doc.GetElementsByTagName("ReportName"));
                foreach (XmlElement element in nodes)
                {
                    element.InnerText = element.InnerText.Replace("Master/Transaction", FolderName).Replace("Master/Supporting Information", FolderName);
                    //element.InnerText = element.InnerText.Replace("Master/Supporting Information", FolderName);
                }

                Byte[] NewreportDefinition = null;
                using (MemoryStream Newstream1 = new MemoryStream())
                {
                    doc.Save(Newstream1);
                    NewreportDefinition = Newstream1.ToArray();
                }

                //for (int i = 0; i < lstVideos.Count; i++)
                //    Console.WriteLine("{0}", lstVideos[i].InnerText);

                //Get report Data source of source report
                eTurnsWeb.localhost2012.DataSource[] ds = null;
                ds = objReportingService2005.GetItemDataSources(_reportPath);

                //eTurnsWeb.localhost2012.Property[] prop = null;
                eTurnsWeb.localhost2012.Warning[] warnings = null;

                for (int i = 0; i < ds.Length; i++)
                {
                    if (ds[i].Name != "SSRS2012")
                    {
                        ((eTurnsWeb.localhost2012.DataSourceDefinition)(ds[i].Item)).CredentialRetrieval = CredentialRetrievalEnum.Store;
                        ((eTurnsWeb.localhost2012.DataSourceDefinition)(ds[i].Item)).UserName = ConfigurationManager.AppSettings["DbUserName"].ToString();
                        ((eTurnsWeb.localhost2012.DataSourceDefinition)(ds[i].Item)).Password = ConfigurationManager.AppSettings["DbPassword"].ToString();
                    }
                }
                //try
                //{
                //    objReportingService2005.CreateFolder(FolderName, "/", prop);
                //}
                //catch (Exception) { }

                eTurnsWeb.localhost2012.CatalogItem copiedReport = objReportingService2005.CreateCatalogItem("Report", DestinationReportName, "/" + FolderName, true, NewreportDefinition, null, out warnings);
                objReportingService2005.SetItemDataSources("/" + FolderName + "/" + DestinationReportName, ds);

                #endregion
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void DownloadReport(string ServerReportPath, string ReportName, string LocalReportPath)
        {
            try
            {

                Byte[] reportDefinition = null;
                reportDefinition = objReportingService2005.GetItemDefinition(ServerReportPath);
                XmlDocument doc = new XmlDocument();
                MemoryStream stream = new MemoryStream(reportDefinition);
                string sOutFile = "";
                sOutFile = string.Format(@"{0}{1}.rdl", LocalReportPath, ReportName);

                if (File.Exists(sOutFile))
                    File.Delete(sOutFile);

                doc.Load(stream);
                doc.Save(sOutFile);

            }
            catch (Exception)
            {

                throw;
            }
        }
        public void UploadReport(string ReportSourcePath, string DestinationReportName, string DestReportFolder)
        {
            try
            {
                byte[] reportContents = null;
                FileStream fstream = File.OpenRead(ReportSourcePath);
                reportContents = new Byte[fstream.Length];
                fstream.Read(reportContents, 0, Convert.ToInt32(fstream.Length));
                fstream.Close();
                DoNetworkAuthentication();
                eTurnsWeb.localhost2012.Warning[] warnings = null;
                eTurnsWeb.localhost2012.CatalogItem copiedReport = objReportingService2005.CreateCatalogItem("Report", DestinationReportName, "/" + DestReportFolder, true, reportContents, null, out warnings);

                eTurnsWeb.localhost2012.DataSource[] ds = null;
                ds = objReportingService2005.GetItemDataSources("/" + DestReportFolder + "/" + DestinationReportName);
                for (int i = 0; i < ds.Length; i++)
                {
                    if (ds[i].Name != "SSRS2012")
                    {
                        ((eTurnsWeb.localhost2012.DataSourceDefinition)(ds[i].Item)).CredentialRetrieval = CredentialRetrievalEnum.Store;
                        ((eTurnsWeb.localhost2012.DataSourceDefinition)(ds[i].Item)).UserName = ConfigurationManager.AppSettings["DbUserName"].ToString();
                        ((eTurnsWeb.localhost2012.DataSourceDefinition)(ds[i].Item)).Password = ConfigurationManager.AppSettings["DbPassword"].ToString();
                    }
                }
                objReportingService2005.SetItemDataSources("/" + DestReportFolder + "/" + DestinationReportName, ds);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void CreateFolder(string Foldername)
        {
            try
            {
                eTurnsWeb.localhost2012.Property[] prop = null;
                objReportingService2005.CreateFolder(Foldername, "/", prop);
            }
            catch (Exception)
            {

            }
        }

        public void CopyAllFilesAndFolder(string FolderName, string CompanyFolderName)
        {
            try
            {
                DoNetworkAuthentication();

                CreateFolder(CompanyFolderName);
                //string FolderName = "/Master";

                List<string> reportPaths = new List<string>();
                CatalogItem[] catalogItems;
                catalogItems = objReportingService2005.ListChildren(FolderName, true);

                foreach (CatalogItem item in catalogItems)
                {
                    if (item.TypeName == "Folder")
                    {
                        try
                        {
                            eTurnsWeb.localhost2012.Property[] prop = null;
                            objReportingService2005.CreateFolder(item.Name, "/" + CompanyFolderName, prop);

                        }
                        catch (Exception) { }
                    }
                    if (item.TypeName == "Report")
                    {
                        if (item.Path.Contains("Supporting Information"))
                        {
                            CopyReport(CompanyFolderName + "/Supporting Information", item.Name, item.Name, item.Path);
                        }
                        else if (item.Path.Contains("Transaction"))
                        {
                            CopyReport(CompanyFolderName + "/Transaction", item.Name, item.Name, item.Path);
                        }

                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
        }


        public void LoadReport(string ReportServerURL, string ReportPath, Microsoft.Reporting.WebForms.ReportParameter[] ReportParameter, eTurnsWeb.rsExecution2012.ParameterValue[] parameters, string TargetFileName)
        {
            try
            {
                // Prepare Render arguments

                string historyID = null;
                string deviceInfo = null;
                string format = "PDF";
                Byte[] results;
                string encoding = String.Empty;
                string mimeType = String.Empty;
                string extension = String.Empty;
                eTurnsWeb.rsExecution2012.Warning[] warnings = null;
                string[] streamIDs = null;


                // Get the report name
                string _reportName = "@" + ReportPath;
                string _historyID = null;
                bool _forRendering = false;
                eTurnsWeb.localhost2012.ParameterValue[] _values = null;
                eTurnsWeb.localhost2012.DataSourceCredentials[] _credentials = null;
                eTurnsWeb.localhost2012.ItemParameter[] _parameters = null;

                _credentials = new eTurnsWeb.localhost2012.DataSourceCredentials[0];

                object UserId = "UserId";

                // _credentials[0] = new SignalRApp.localhost2012.

                //_credentials = SignalRApp.localhost2012.DataSourceCredentials

                //myCredential.UserId = DatabaseUsername;
                //myCredential.Password = DatabasePassword;

                _parameters = objReportingService2005.GetItemParameters(_reportName, _historyID, _forRendering, _values, _credentials);

                //Load the selected report

                eTurnsWeb.rsExecution2012.ExecutionInfo ei = objReportExecutionService.LoadReport(_reportName, historyID);

                // Assign Report Server URL
                objReportingService2005.Url = ReportServerURL;
                objReportExecutionService.Url = ReportServerURL;

                // Network Authentication
                DoNetworkAuthentication();

                //objReportExecutionService.SetExecutionParameters(ReportParameter, "en-us");       
                results = objReportExecutionService.Render(format, deviceInfo, out extension, out mimeType, out encoding, out warnings, out streamIDs);

                // Get Database Credential
                Microsoft.Reporting.WebForms.DataSourceCredentials myCredential = new Microsoft.Reporting.WebForms.DataSourceCredentials();
                myCredential.UserId = DatabaseUsername;
                myCredential.Password = DatabasePassword;


                using (FileStream stream1 = File.OpenWrite(TargetFileName))
                {
                    stream1.Write(results, 0, results.Length);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
    public class CustomReportCredentials : Microsoft.Reporting.WebForms.IReportServerCredentials
    {
        // local variable for network credential.
        private string _UserName;
        private string _PassWord;
        private string _DomainName;
        public CustomReportCredentials(string UserName, string PassWord, string DomainName)
        {
            _UserName = UserName;
            _PassWord = PassWord;
            _DomainName = DomainName;
        }
        public System.Security.Principal.WindowsIdentity ImpersonationUser
        {
            get
            {
                return null;  // not use ImpersonationUser
            }
        }
        public System.Net.ICredentials NetworkCredentials
        {
            get
            {
                // use NetworkCredentials
                return new NetworkCredential(_UserName, _PassWord, _DomainName);
            }
        }
        public bool GetFormsCredentials(out Cookie authCookie, out string user, out string password, out string authority)
        {
            // not use FormsCredentials unless you have implements a custom autentication.
            authCookie = null;
            user = password = authority = null;
            return false;
        }
    }

}