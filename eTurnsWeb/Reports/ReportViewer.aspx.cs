using System;
using System.Web;
//using MLAFMS_BAL;
using System.Threading;
using System.Globalization;
using System.Configuration;
using System.Data;
using System.Security.Policy;
using System.IO;
using eTurnsWeb.Helper;
using eTurns.DTO;
//using Globle;

namespace EturnsWeb
{
    public partial class ReportViewer : System.Web.UI.Page
    {
        #region "Globle Varibles"
        string param1, param2, param3, param4, eTurnsLogo, EnterpriseLogo, CompanyLogo, BarcodeURL;//ReportName, 
        string[] param5;
        string[] param6;
        string param7BarcodeColumn;
        string param5Company = "";
        string param6Room = "";
        string DisplayColumnParam = "";
        //bool bParam1, bParam2;
        eTurnsWeb.localhost2012.ReportingService2010 rs = new eTurnsWeb.localhost2012.ReportingService2010();
        ReportServices objReportServices;
        string Username = ConfigurationManager.AppSettings["NetworkUser"];
        string Password = ConfigurationManager.AppSettings["NetworkPassword"];
        string Domain = ConfigurationManager.AppSettings["NetworkDomain"];
        string DatabaseUsername = ConfigurationManager.AppSettings["DbUserName"];
        string DatabasePassword = ConfigurationManager.AppSettings["DbPassword"];
        string ReportServerUrl = ConfigurationManager.AppSettings["ReportServerURL"];
        public string ReportsUrl = ConfigurationManager.AppSettings["ReportsUrl"];

        #region Report Const
        public const string EBinLocation = "/Supporting Information/BinLocation";
        public const string ECategory = "/Supporting Information/Category";
        public const string ECustomers = "/Supporting Information/Customers";
        public const string EFreightTypes = "/Supporting Information/FreightTypes";
        public const string EGLAccounts = "/Supporting Information/GLAccounts";
        public const string ELocations = "/Supporting Information/Locations";
        public const string EManufacturers = "/Supporting Information/Manufacturers";
        public const string EMeasurementTerms = "/Supporting Information/MeasurementTerms";
        public const string EShipVia = "/Supporting Information/ShipVia";
        public const string ESupplier = "/Supporting Information/Supplier";
        public const string ETechnicians = "/Supporting Information/Technicians";
        public const string EToolCategory = "/Supporting Information/ToolCategory";
        public const string EAssetCategory = "/Supporting Information/AssetCategory";
        public const string EUnits = "/Supporting Information/Units";
        public const string EItem = "/Supporting Information/Items";
        public const string EPO = "/Transaction/PurchaseOrder";
        public const string EWO = "/Transaction/WorkOrder";
        public const string ERQ = "/Transaction/Requisition";
        public const string EPulls = "/Transaction/Pulls";
        public const string EReceives = "/Transaction/Receives_HeaderReport";
        public const string EOrderedItems = "/Transaction/OrderedItems_HeaderReport";
        public const string ECostUOM = "/Supporting Information/CostUOM1";
        public const string EInventoryClassification = "/Supporting Information/InventoryClassificationMaster";
        public const string EItemBarcodeLabelTwoColumn = "/Transaction/ItemBarcodeLabelTwoColumn";
        public const string EItemBarcodeLabelOneColumn = "/Transaction/ItemBarcodeLabelOneColumn";
        public const string Erpt_StagingHeader = "/Transaction/rpt_StagingHeader";

        #endregion

        #endregion


        string IDs = "";
        new string Title = "";
        DateTime? dt1 = null;
        DateTime? dt2 = null;


        #region "Page Methods"
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                objReportServices = new ReportServices(rs, Domain, Username, Password, DatabaseUsername, DatabasePassword);
                DataTable dt = objReportServices.GetReportList();
                if (!IsPostBack)
                {
                    IDs = Convert.ToString(Session["Ids"]);
                    Title = Convert.ToString(Session["Title"]);
                    DisplayColumnParam = Convert.ToString(Session["DisplayFields"]);

                    if (!string.IsNullOrEmpty(Session["StartDate"].ToString()))
                    {
                        dt1 = Convert.ToDateTime(Session["StartDate"]);
                    }
                    if (!string.IsNullOrEmpty(Session["EndDate"].ToString()))
                    {
                        dt2 = Convert.ToDateTime(Session["EndDate"]);
                    }
                    if (!string.IsNullOrEmpty(Session["BarcodeColumn"].ToString()))
                    {
                        param7BarcodeColumn = Session["BarcodeColumn"].ToString();
                    }

                    drpreport.DataSource = dt;
                    drpreport.DataValueField = "ReportPath";
                    drpreport.DataTextField = "ReportName";
                    drpreport.DataBind();
                    if (Page.RouteData.Values.ContainsKey("reportname") && Page.RouteData.Values.ContainsKey("foldername"))
                    {
                        LoadReport(Convert.ToString(Page.RouteData.Values["foldername"]), Convert.ToString(Page.RouteData.Values["reportname"]));
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message.ToString();
                //ExceptionHandeling.ExceptionFire(this.Page, ex);

            }
        }
        #endregion

        #region "Button Mothod"
        //protected void btnGenerateReport_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        objReportServices = new ReportServices(rs, Domain, Username, Password, DatabaseUsername, DatabasePassword);
        //        SqlReportViewer.Reset();

        //        ReportName = @"/Supporting Information/BinLocation";
        //        ReportName = drpreport.SelectedValue.ToString();

        //        string DatabaseName = "10055_477BAFE9-F320-4702-93E9-D1844F9E5882"; // This will come from session
        //        string ServerName = "192.168.0.26\\SQL2012"; // This will come from session

        //        SqlReportViewer.ServerReport.ReportServerUrl = new Uri(ReportServerUrl);
        //        SqlReportViewer.ServerReport.ReportPath = ReportName;
        //        objReportServices.LoadReport(SqlReportViewer, ReportServerUrl, ReportName, SetReportParameters(ReportName, DatabaseName, ServerName));

        //    }
        //    catch (Exception ex)
        //    {
        //        lblMessage.Text = ex.Message.ToString();
        //    }
        //} 

        public void LoadReport(string FolderName, string ReportName)
        {
            try
            {
                //Response.Write("Started");
                objReportServices = new ReportServices(rs, Domain, Username, Password, DatabaseUsername, DatabasePassword);


                //Response.End();
                SqlReportViewer.Reset();
                string ReportPath = "";
                if (FolderName != "")
                    ReportPath = @"/" + FolderName + "/" + ReportName;
                else
                    ReportPath = @"/" + ReportName;
                //ReportName = drpreport.SelectedValue.ToString();  

                //string DatabaseName =  "10055_477BAFE9-F320-4702-93E9-D1844F9E5882"; // This will come from session
                string DatabaseName = SessionHelper.EnterPriseDBName; // This will come from session
                string ServerName = ConfigurationManager.AppSettings["DBserverName"];//"192.168.0.26\\SQL2012"; // This will come from session

                SqlReportViewer.ServerReport.ReportServerUrl = new Uri(ReportServerUrl);
                SqlReportViewer.ServerReport.ReportPath = ReportPath;
                objReportServices.LoadReport(SqlReportViewer, ReportServerUrl, ReportPath, SetReportParameters(ReportPath, DatabaseName, ServerName));
                //Response.Write(Domain + "______" + Username + "______" + Password + "______" + DatabaseUsername + "______" + DatabasePassword);
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    if (ex.InnerException != null)
                    {
                        Response.Write(ex.InnerException.ToString());
                    }
                    else
                    {
                        Response.Write(ex.Message);
                        Response.Write(ex.Source);
                    }

                }
            }

            //string SourceReportName = ReportName;
            //string DestinationReportName = ReportName;
            //string NewCompany = "Company2"; 
            //objReportServices.CopyFiles("/Master", NewCompany);
            //objReportServices.CopyReport("Master/Transaction", SourceReportName, DestinationReportName, ReportPath);
            //objReportServices.DownloadReport(ReportPath, DestinationReportName, "C:\\");
            //objReportServices.UploadReport("C:\\\\BinLocation.rdl", "BinLocationupload", "TestReport");

            //objReportServices.CopyReport("Company1/Transaction", "PurchaseOrder", "PurchaseOrder", "/Master/Transaction/PurchaseOrder");

            //byte[] bytes = SqlReportViewer.ServerReport.Render("PDF");
            //Response.Clear();
            //Response.ContentType = "application/pdf";
            //Response.AddHeader("Content-disposition", "filename=output.pdf");
            //Response.OutputStream.Write(bytes, 0, bytes.Length);
            //Response.OutputStream.Flush();
            //Response.OutputStream.Close();
            //Response.Flush();
            //Response.Close();

        }
        #endregion

        #region "Private Events"

        private Microsoft.Reporting.WebForms.ReportParameter[] SetReportParameters(string ReportName, string DatabaseName, string ServerName)
        {
            try
            {
                Microsoft.Reporting.WebForms.ReportParameter[] objReportParameter = new Microsoft.Reporting.WebForms.ReportParameter[0];

                param1 = "false"; // IsDeleted
                param2 = "false"; // IsArchived
                param3 = DatabaseName; // Database Name
                param4 = ServerName; // Server Name

                string[] CompanyArray = { SessionHelper.CompanyID.ToString() };
                string[] RoomArray = { SessionHelper.RoomID.ToString() };
                param5 = CompanyArray;
                param6 = RoomArray;




                if (ReportName == EReceives || ReportName == EOrderedItems)
                {
                    param6 = Convert.ToString(Session["RoomIds"]).Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                }

                param5Company = Convert.ToString(Session["CompanyIds"]);
                param6Room = Convert.ToString(Session["RoomIds"]);


                eTurnsLogo = "http://202.131.117.224:4040/Content/images/logo.jpg";
                //EnterpriseLogo = "http://202.131.117.224:4040/Content/images/oasis.jpg";
                //EnterpriseLogo = "http://" + Request.ServerVariables["HTTP_HOST"] + "/Uploads/EnterpriseLogos/" + SessionHelper.EnterPriceID + "/" + SessionHelper.EnterpriseLogoUrl;
                //CompanyLogo = "http://" + Request.ServerVariables["HTTP_HOST"] + "/Uploads/CompanyLogos/" + SessionHelper.CompanyID + "/" + SessionHelper.CompanyLogoUrl;

                CompanyLogo = "http://" + Request.ServerVariables["HTTP_HOST"] + "/Content/Images/CompanyLogo.jpg";
                EnterpriseLogo = "http://" + Request.ServerVariables["HTTP_HOST"] + "/Content/Images/EnterpariseLogo.jpg";
                BarcodeURL = "http://" + Request.ServerVariables["HTTP_HOST"] + "/Barcode/GetBarcodeImage";

                if (eTurnsWeb.Helper.SessionHelper.EnterPriceID > 0)
                {
                    EnterpriseDTO objEnterpriseDTO = new eTurnsMaster.DAL.EnterpriseMasterDAL().GetEnterprise(eTurnsWeb.Helper.SessionHelper.EnterPriceID);
                    if (objEnterpriseDTO != null && !string.IsNullOrWhiteSpace(objEnterpriseDTO.EnterpriseLogo))
                    {
                        EnterpriseLogo = "http://" + Request.ServerVariables["HTTP_HOST"] + "/Uploads/EnterpriseLogos/" + eTurnsWeb.Helper.SessionHelper.EnterPriceID.ToString() + "/" + objEnterpriseDTO.EnterpriseLogo;
                    }
                }
                if (eTurnsWeb.Helper.SessionHelper.CompanyID > 0)
                {
                    CompanyMasterDTO objCompanyMasterDTO = new eTurns.DAL.CompanyMasterDAL(SessionHelper.EnterPriseDBName).GetRecord(eTurnsWeb.Helper.SessionHelper.CompanyID);
                    if (objCompanyMasterDTO != null && !string.IsNullOrWhiteSpace(objCompanyMasterDTO.CompanyLogo))
                    {
                        CompanyLogo = "http://" + Request.ServerVariables["HTTP_HOST"] + "/Uploads/CompanyLogos/" + eTurnsWeb.Helper.SessionHelper.CompanyID.ToString() + "/" + objCompanyMasterDTO.CompanyLogo;
                        //CompanyLogo = Url.Content("~/Uploads/CompanyLogos/" + eTurnsWeb.Helper.SessionHelper.EnterPriceID.ToString() + "/" + objCompanyMasterDTO.CompanyLogo);
                    }
                }
                switch (ReportName)
                {

                    case EBinLocation:
                    case ECategory:
                    case EFreightTypes:
                    case EShipVia:
                    case ETechnicians:
                    case ELocations:
                    case EToolCategory:
                    case EAssetCategory:
                    case EManufacturers:
                    case EMeasurementTerms:
                    case ECustomers:
                    case EGLAccounts:
                    case EUnits:
                    case EInventoryClassification:
                    case ECostUOM:


                    //objReportParameter = new Microsoft.Reporting.WebForms.ReportParameter[12];
                    //objReportParameter[0] = new Microsoft.Reporting.WebForms.ReportParameter("IsDeleted", param1, false);
                    //objReportParameter[1] = new Microsoft.Reporting.WebForms.ReportParameter("IsArchived", param2, false);
                    //objReportParameter[2] = new Microsoft.Reporting.WebForms.ReportParameter("DatabaseName", param3, false);
                    //objReportParameter[3] = new Microsoft.Reporting.WebForms.ReportParameter("ServerName", param4, false);
                    //objReportParameter[4] = new Microsoft.Reporting.WebForms.ReportParameter("Company", param5, false);
                    //objReportParameter[5] = new Microsoft.Reporting.WebForms.ReportParameter("Room", param6, false);
                    //objReportParameter[6] = new Microsoft.Reporting.WebForms.ReportParameter("eturnslogo", eTurnsLogo, false);
                    //objReportParameter[7] = new Microsoft.Reporting.WebForms.ReportParameter("EnterpriseLogo", EnterpriseLogo, false);
                    //objReportParameter[8] = new Microsoft.Reporting.WebForms.ReportParameter("CompanyLogo", CompanyLogo, false);
                    //objReportParameter[9] = new Microsoft.Reporting.WebForms.ReportParameter("pDisplayFields", DisplayColumnParam, false);
                    //objReportParameter[10] = new Microsoft.Reporting.WebForms.ReportParameter("ReportTitle", Title, false);
                    //string[] IMString = null;
                    //if (!string.IsNullOrEmpty(IDs))
                    //{
                    //    if (IDs.Contains(","))
                    //    {
                    //        IDs = IDs.Substring(0, (IDs.Length - 1));
                    //    }
                    //    IMString = IDs.Split(',');
                    //}
                    //if (IMString == null)
                    //{
                    //    IMString = new string[1];
                    //    IMString[0] = "0";
                    //}
                    //objReportParameter[11] = new Microsoft.Reporting.WebForms.ReportParameter("ID", IMString, false);
                    //break;
                    case ESupplier:
                    case EItem:
                        objReportParameter = new Microsoft.Reporting.WebForms.ReportParameter[12];
                        objReportParameter[0] = new Microsoft.Reporting.WebForms.ReportParameter("IsDeleted", param1, false);
                        objReportParameter[1] = new Microsoft.Reporting.WebForms.ReportParameter("IsArchived", param2, false);
                        objReportParameter[2] = new Microsoft.Reporting.WebForms.ReportParameter("DatabaseName", param3, false);
                        objReportParameter[3] = new Microsoft.Reporting.WebForms.ReportParameter("ServerName", param4, false);
                        objReportParameter[4] = new Microsoft.Reporting.WebForms.ReportParameter("Company", param5Company, false);
                        objReportParameter[5] = new Microsoft.Reporting.WebForms.ReportParameter("Room", param6Room, false);
                        objReportParameter[6] = new Microsoft.Reporting.WebForms.ReportParameter("eturnslogo", eTurnsLogo, false);
                        objReportParameter[7] = new Microsoft.Reporting.WebForms.ReportParameter("EnterpriseLogo", EnterpriseLogo, false);
                        objReportParameter[8] = new Microsoft.Reporting.WebForms.ReportParameter("CompanyLogo", CompanyLogo, false);
                        objReportParameter[9] = new Microsoft.Reporting.WebForms.ReportParameter("pDisplayFields", DisplayColumnParam, false);
                        objReportParameter[10] = new Microsoft.Reporting.WebForms.ReportParameter("ReportTitle", Title, false);
                        objReportParameter[11] = new Microsoft.Reporting.WebForms.ReportParameter("ID", IDs, false);
                        break;
                    case EPO:
                        objReportParameter = new Microsoft.Reporting.WebForms.ReportParameter[13];
                        objReportParameter[0] = new Microsoft.Reporting.WebForms.ReportParameter("IsDeleted", param1, false);
                        objReportParameter[1] = new Microsoft.Reporting.WebForms.ReportParameter("IsArchived", param2, false);
                        objReportParameter[2] = new Microsoft.Reporting.WebForms.ReportParameter("DatabaseName", param3, false);
                        objReportParameter[3] = new Microsoft.Reporting.WebForms.ReportParameter("ServerName", param4, false);
                        objReportParameter[4] = new Microsoft.Reporting.WebForms.ReportParameter("Company", param5Company, false);
                        objReportParameter[5] = new Microsoft.Reporting.WebForms.ReportParameter("Room", param6Room, false);
                        objReportParameter[6] = new Microsoft.Reporting.WebForms.ReportParameter("eturnslogo", eTurnsLogo, false);
                        objReportParameter[7] = new Microsoft.Reporting.WebForms.ReportParameter("EnterpriseLogo", EnterpriseLogo, false);
                        objReportParameter[8] = new Microsoft.Reporting.WebForms.ReportParameter("CompanyLogo", CompanyLogo, false);
                        objReportParameter[9] = new Microsoft.Reporting.WebForms.ReportParameter("pDisplayFields", DisplayColumnParam, false);
                        objReportParameter[10] = new Microsoft.Reporting.WebForms.ReportParameter("ReportTitle", Title, false);
                        objReportParameter[11] = new Microsoft.Reporting.WebForms.ReportParameter("OrderIDs", IDs, false);
                        objReportParameter[12] = new Microsoft.Reporting.WebForms.ReportParameter("BarcodeImgBaseURL", BarcodeURL, false);

                        break;
                    case Erpt_StagingHeader:
                        objReportParameter = new Microsoft.Reporting.WebForms.ReportParameter[13];
                        objReportParameter[0] = new Microsoft.Reporting.WebForms.ReportParameter("IsDeleted", param1, false);
                        objReportParameter[1] = new Microsoft.Reporting.WebForms.ReportParameter("IsArchived", param2, false);
                        objReportParameter[2] = new Microsoft.Reporting.WebForms.ReportParameter("DatabaseName", param3, false);
                        objReportParameter[3] = new Microsoft.Reporting.WebForms.ReportParameter("ServerName", param4, false);
                        objReportParameter[4] = new Microsoft.Reporting.WebForms.ReportParameter("Company", param5Company, false);
                        objReportParameter[5] = new Microsoft.Reporting.WebForms.ReportParameter("Room", param6Room, false);
                        objReportParameter[6] = new Microsoft.Reporting.WebForms.ReportParameter("eturnslogo", eTurnsLogo, false);
                        objReportParameter[7] = new Microsoft.Reporting.WebForms.ReportParameter("EnterpriseLogo", EnterpriseLogo, false);
                        objReportParameter[8] = new Microsoft.Reporting.WebForms.ReportParameter("CompanyLogo", CompanyLogo, false);
                        objReportParameter[9] = new Microsoft.Reporting.WebForms.ReportParameter("pDisplayFields", DisplayColumnParam, false);
                        if (Title == "")
                            Title = "Material Staging";
                        objReportParameter[10] = new Microsoft.Reporting.WebForms.ReportParameter("ReportTitle", Title, false);
                        objReportParameter[11] = new Microsoft.Reporting.WebForms.ReportParameter("StagingHeaderIds", IDs, false);
                        objReportParameter[12] = new Microsoft.Reporting.WebForms.ReportParameter("ID", "0", false);
                        break;
                    case ERQ:
                        objReportParameter = new Microsoft.Reporting.WebForms.ReportParameter[12];
                        objReportParameter[0] = new Microsoft.Reporting.WebForms.ReportParameter("IsDeleted", param1, false);
                        objReportParameter[1] = new Microsoft.Reporting.WebForms.ReportParameter("IsArchived", param2, false);
                        objReportParameter[2] = new Microsoft.Reporting.WebForms.ReportParameter("DatabaseName", param3, false);
                        objReportParameter[3] = new Microsoft.Reporting.WebForms.ReportParameter("ServerName", param4, false);
                        objReportParameter[4] = new Microsoft.Reporting.WebForms.ReportParameter("Company", param5Company, false);
                        objReportParameter[5] = new Microsoft.Reporting.WebForms.ReportParameter("Room", param6Room, false);
                        objReportParameter[6] = new Microsoft.Reporting.WebForms.ReportParameter("eturnslogo", eTurnsLogo, false);
                        objReportParameter[7] = new Microsoft.Reporting.WebForms.ReportParameter("EnterpriseLogo", EnterpriseLogo, false);
                        objReportParameter[8] = new Microsoft.Reporting.WebForms.ReportParameter("CompanyLogo", CompanyLogo, false);
                        objReportParameter[9] = new Microsoft.Reporting.WebForms.ReportParameter("pDisplayFields", DisplayColumnParam, false);
                        objReportParameter[10] = new Microsoft.Reporting.WebForms.ReportParameter("ReportTitle", Title, false);
                        //string[] RQIDs = null;
                        //if (!string.IsNullOrEmpty(IDs))
                        //{
                        //    if (IDs.Contains(","))
                        //    {
                        //        IDs = IDs.Substring(0, (IDs.Length - 1));
                        //    }
                        //    RQIDs = IDs.Split(',');
                        //}
                        //if (RQIDs == null)
                        //{
                        //    RQIDs = new string[1];
                        //    RQIDs[0] = "0";
                        //}
                        objReportParameter[11] = new Microsoft.Reporting.WebForms.ReportParameter("ID", IDs, false);
                        break;
                    case EWO:
                        objReportParameter = new Microsoft.Reporting.WebForms.ReportParameter[13];
                        objReportParameter[0] = new Microsoft.Reporting.WebForms.ReportParameter("IsDeleted", param1, false);
                        objReportParameter[1] = new Microsoft.Reporting.WebForms.ReportParameter("IsArchived", param2, false);
                        objReportParameter[2] = new Microsoft.Reporting.WebForms.ReportParameter("DatabaseName", param3, false);
                        objReportParameter[3] = new Microsoft.Reporting.WebForms.ReportParameter("ServerName", param4, false);
                        objReportParameter[4] = new Microsoft.Reporting.WebForms.ReportParameter("Company", param5Company, false);
                        objReportParameter[5] = new Microsoft.Reporting.WebForms.ReportParameter("Room", param6Room, false);
                        objReportParameter[6] = new Microsoft.Reporting.WebForms.ReportParameter("eturnslogo", eTurnsLogo, false);
                        objReportParameter[7] = new Microsoft.Reporting.WebForms.ReportParameter("EnterpriseLogo", EnterpriseLogo, false);
                        objReportParameter[8] = new Microsoft.Reporting.WebForms.ReportParameter("CompanyLogo", CompanyLogo, false);
                        objReportParameter[9] = new Microsoft.Reporting.WebForms.ReportParameter("pDisplayFields", DisplayColumnParam, false);
                        objReportParameter[10] = new Microsoft.Reporting.WebForms.ReportParameter("ReportTitle", Title, false);
                        //string[] WOIDs = null;
                        //if (!string.IsNullOrEmpty(IDs))
                        //{
                        //    if (IDs.Contains(","))
                        //    {
                        //        IDs = IDs.Substring(0, (IDs.Length - 1));
                        //    }
                        //    WOIDs = IDs.Split(',');
                        //}
                        //if (WOIDs == null)
                        //{
                        //    WOIDs = new string[1];
                        //    WOIDs[0] = "0";
                        //}
                        objReportParameter[11] = new Microsoft.Reporting.WebForms.ReportParameter("ID", IDs, false);
                        objReportParameter[12] = new Microsoft.Reporting.WebForms.ReportParameter("BarcodeImgBaseURL", BarcodeURL, false);
                        break;
                    case EPulls:
                        objReportParameter = new Microsoft.Reporting.WebForms.ReportParameter[13];
                        objReportParameter[0] = new Microsoft.Reporting.WebForms.ReportParameter("IsDeleted", param1, false);
                        objReportParameter[1] = new Microsoft.Reporting.WebForms.ReportParameter("IsArchived", param2, false);
                        objReportParameter[2] = new Microsoft.Reporting.WebForms.ReportParameter("DatabaseName", param3, false);
                        objReportParameter[3] = new Microsoft.Reporting.WebForms.ReportParameter("ServerName", param4, false);
                        objReportParameter[4] = new Microsoft.Reporting.WebForms.ReportParameter("Company", param5Company, false);
                        objReportParameter[5] = new Microsoft.Reporting.WebForms.ReportParameter("Room", param6Room, false);
                        objReportParameter[6] = new Microsoft.Reporting.WebForms.ReportParameter("eturnslogo", eTurnsLogo, false);
                        objReportParameter[7] = new Microsoft.Reporting.WebForms.ReportParameter("EnterpriseLogo", EnterpriseLogo, false);
                        objReportParameter[8] = new Microsoft.Reporting.WebForms.ReportParameter("CompanyLogo", CompanyLogo, false);
                        objReportParameter[9] = new Microsoft.Reporting.WebForms.ReportParameter("pDisplayFields", DisplayColumnParam, false);
                        objReportParameter[10] = new Microsoft.Reporting.WebForms.ReportParameter("ReportTitle", Title, false);
                        objReportParameter[11] = new Microsoft.Reporting.WebForms.ReportParameter("StartDate", Convert.ToString(dt1), false);
                        objReportParameter[12] = new Microsoft.Reporting.WebForms.ReportParameter("EndDate", Convert.ToString(dt2), false);
                        break;
                    case EReceives:
                        objReportParameter = new Microsoft.Reporting.WebForms.ReportParameter[13];
                        objReportParameter[0] = new Microsoft.Reporting.WebForms.ReportParameter("IsDeleted", param1, false);
                        objReportParameter[1] = new Microsoft.Reporting.WebForms.ReportParameter("IsArchived", param2, false);
                        objReportParameter[2] = new Microsoft.Reporting.WebForms.ReportParameter("DatabaseName", param3, false);
                        objReportParameter[3] = new Microsoft.Reporting.WebForms.ReportParameter("ServerName", param4, false);
                        objReportParameter[4] = new Microsoft.Reporting.WebForms.ReportParameter("Company", param5, false);
                        objReportParameter[5] = new Microsoft.Reporting.WebForms.ReportParameter("Room", param6, false);
                        objReportParameter[6] = new Microsoft.Reporting.WebForms.ReportParameter("eturnslogo", eTurnsLogo, false);
                        objReportParameter[7] = new Microsoft.Reporting.WebForms.ReportParameter("EnterpriseLogo", EnterpriseLogo, false);
                        objReportParameter[8] = new Microsoft.Reporting.WebForms.ReportParameter("CompanyLogo", CompanyLogo, false);
                        objReportParameter[9] = new Microsoft.Reporting.WebForms.ReportParameter("pDisplayFields", DisplayColumnParam, false);
                        objReportParameter[10] = new Microsoft.Reporting.WebForms.ReportParameter("ReportTitle", Title, false);
                        objReportParameter[11] = new Microsoft.Reporting.WebForms.ReportParameter("StartDate", Convert.ToString(dt1), false);
                        objReportParameter[12] = new Microsoft.Reporting.WebForms.ReportParameter("EndDate", Convert.ToString(dt2), false);
                        break;
                    case EOrderedItems:
                        objReportParameter = new Microsoft.Reporting.WebForms.ReportParameter[13];
                        objReportParameter[0] = new Microsoft.Reporting.WebForms.ReportParameter("IsDeleted", param1, false);
                        objReportParameter[1] = new Microsoft.Reporting.WebForms.ReportParameter("IsArchived", param2, false);
                        objReportParameter[2] = new Microsoft.Reporting.WebForms.ReportParameter("DatabaseName", param3, false);
                        objReportParameter[3] = new Microsoft.Reporting.WebForms.ReportParameter("ServerName", param4, false);
                        objReportParameter[4] = new Microsoft.Reporting.WebForms.ReportParameter("Company", param5, false);
                        objReportParameter[5] = new Microsoft.Reporting.WebForms.ReportParameter("Room", param6, false);
                        objReportParameter[6] = new Microsoft.Reporting.WebForms.ReportParameter("eturnslogo", eTurnsLogo, false);
                        objReportParameter[7] = new Microsoft.Reporting.WebForms.ReportParameter("EnterpriseLogo", EnterpriseLogo, false);
                        objReportParameter[8] = new Microsoft.Reporting.WebForms.ReportParameter("CompanyLogo", CompanyLogo, false);
                        objReportParameter[9] = new Microsoft.Reporting.WebForms.ReportParameter("pDisplayFields", DisplayColumnParam, false);
                        objReportParameter[10] = new Microsoft.Reporting.WebForms.ReportParameter("ReportTitle", Title, false);
                        objReportParameter[11] = new Microsoft.Reporting.WebForms.ReportParameter("StartDate", Convert.ToString(dt1), false);
                        objReportParameter[12] = new Microsoft.Reporting.WebForms.ReportParameter("EndDate", Convert.ToString(dt2), false);
                        break;
                    case EItemBarcodeLabelTwoColumn:
                    case EItemBarcodeLabelOneColumn:
                        objReportParameter = new Microsoft.Reporting.WebForms.ReportParameter[7];
                        objReportParameter[0] = new Microsoft.Reporting.WebForms.ReportParameter("DatabaseName", param3, false);
                        objReportParameter[1] = new Microsoft.Reporting.WebForms.ReportParameter("ServerName", param4, false);
                        objReportParameter[2] = new Microsoft.Reporting.WebForms.ReportParameter("Company", param5, false);
                        objReportParameter[3] = new Microsoft.Reporting.WebForms.ReportParameter("Room", param6, false);
                        objReportParameter[4] = new Microsoft.Reporting.WebForms.ReportParameter("NoOflblPerItem", param7BarcodeColumn, false);
                        objReportParameter[5] = new Microsoft.Reporting.WebForms.ReportParameter("BarcodeImgBaseURL", "http://" + Request.ServerVariables["HTTP_HOST"] + "/", false);
                        objReportParameter[6] = new Microsoft.Reporting.WebForms.ReportParameter("ItemIds", IDs, false);
                        break;
                    default:
                        break;
                }

                return objReportParameter;
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message.ToString();
                throw;
            }
        }

        #endregion

        protected void drpreport_SelectedIndexChanged(object sender, EventArgs e)
        {
            string FName = Path.GetDirectoryName(drpreport.SelectedValue.ToString()).Remove(0, 1);
            string RName = Path.GetFileName(drpreport.SelectedValue.ToString());
            Response.RedirectToRoute("ReportRoute", new { FolderName = FName, reportname = RName });
        }

    }
}
