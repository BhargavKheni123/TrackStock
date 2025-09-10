using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsWeb.Helper;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace eTurnsWeb.Reports
{
    public partial class NewReportViewer : System.Web.UI.Page
    {
        XNamespace ns = XNamespace.Get("http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition");
        XNamespace nsrd = XNamespace.Get("http://schemas.microsoft.com/SQLServer/reporting/reportdesigner");
        string connectionString = "";
        string RPT_connectionString = "";
        eTurns.DAL.AlertMail objAlertMail = new eTurns.DAL.AlertMail();
        string SubReportResFile = "";
        int globalcounter = 0;
        //int subRptCounter = 0;
        string strSubTablix = string.Empty;
        string rdlPath = string.Empty;
        long ParentID = 0;
        bool isRunWithReportConnection = false;
        bool IsRoomGridReportCommon = false;
        string RoomDBName = string.Empty;
        long EntID = 0;

        Dictionary<string, string> LocalDictRptPara = null;
        /// <summary>
        /// GetReportParaDictionary
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetReportParaDictionary()
        {
            Dictionary<string, string> rptPara = (Dictionary<string, string>)SessionHelper.Get("ReportPara");
            connectionString = rptPara["ConnectionString"];
            return rptPara;
        }

        private void SetReportConnectionString(ReportBuilderDTO objDTO)
        {
            //Convert.ToString(ConfigurationManager.AppSettings["DBFailoverPartner"]) ?? "172.31.12.215";
            string DBServerName = Convert.ToString(ConfigurationManager.AppSettings["RPT_DBserverName"]) ?? Convert.ToString(ConfigurationManager.AppSettings["DBserverName"]);
            string DBUserName = Convert.ToString(ConfigurationManager.AppSettings["RPT_DbUserName"]) ?? Convert.ToString(ConfigurationManager.AppSettings["DbUserName"]);
            string DBPassword = Convert.ToString(ConfigurationManager.AppSettings["RPT_DbPassword"]) ?? Convert.ToString(ConfigurationManager.AppSettings["DbPassword"]);
            string DBName = SessionHelper.EnterPriseDBName;
            //  RPT_connectionString = DbConnectionHelper.GetOledbConnection("GeneralReadOnly", DBName);
            //string.Format(_connectionString, DBServerName, DBName, DBUserName, DBPassword);

            string _strRunWithReportConnection = "No";
            if (SiteSettingHelper.RunWithReportConnection != string.Empty)
            {
                _strRunWithReportConnection = Convert.ToString(SiteSettingHelper.RunWithReportConnection);
            }
            if (objDTO != null && !string.IsNullOrEmpty(_strRunWithReportConnection) && _strRunWithReportConnection.ToLower() == "yes" && objDTO.ReportAppIntent == "ReadOnly")
            {
                RPT_connectionString = DbConnectionHelper.GetOledbConnection("GeneralReadOnly", DBName);
            }
            else
            {
                RPT_connectionString = DbConnectionHelper.GetOledbConnection("GeneralReadWrite", DBName);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!string.IsNullOrEmpty(Request.QueryString["ReportFor"]))
            //{
            //    if (!Page.IsPostBack)
            //    {
            //        ReportBinding("test");
            //    }              
            //}
            //else
            //{
            globalcounter = 0;
            Int64 ReportId = 0;
            if (!string.IsNullOrEmpty(Request.QueryString["Id"]))
                Int64.TryParse(Request.QueryString["Id"], out ReportId);

            rptID.Value = Convert.ToString(ReportId);
            hdnURL.Value = GetRouteUrl("Default", new { controller = "ReportBuilder", action = "GenerateReportOfCurrentReport", id = ReportId });


            //isRunWithReportConnection
            //XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));
            string _strRunWithReportConnection = "No";
            //if (Settinfile.Element("RunWithReportConnection") != null)
            //{
            //    _strRunWithReportConnection = Convert.ToString(Settinfile.Element("RunWithReportConnection").Value);
            //}

            if (SiteSettingHelper.RunWithReportConnection != string.Empty)
            {
                _strRunWithReportConnection = Convert.ToString(SiteSettingHelper.RunWithReportConnection);
            }

            if (!string.IsNullOrEmpty(_strRunWithReportConnection) && _strRunWithReportConnection.ToLower() == "yes")
            {
                isRunWithReportConnection = true;
            }
            else
            {
                isRunWithReportConnection = false;
            }

            if (ReportId > 0)
            {
                if (SessionHelper.Get("ReportPara") == null)
                {
                    Response.Redirect("/ReportBuilder/ViewReports", true);
                }

                if (!Page.IsPostBack)
                {
                    ShowReport(ReportId);
                }
                else if (Request.Params["ReportViewer1$ctl03$ctl00"] == "Refresh" && !(Request.Form[0].Contains("Next") || Request.Form[0].Contains("Previous") || Request.Form[0].Contains("CurrentPage") || Request.Form[0].Contains("Reserved_AsyncLoadTarget")))
                {
                    ShowReport(ReportId);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(Request.QueryString["Id"]) || SessionHelper.Get("ReportPara") == null)
                {
                    Response.Redirect("/ReportBuilder/ViewReports");
                }

                if (!Page.IsPostBack)
                {
                    ReportBinding(Request.QueryString["Id"]);
                }
                else if (Request.Params["ReportViewer1$ctl03$ctl00"] == "Refresh" && !(Request.Form[0].Contains("Next") || Request.Form[0].Contains("Previous") || Request.Form[0].Contains("CurrentPage") || Request.Form[0].Contains("Reserved_AsyncLoadTarget")))
                {
                    ReportBinding(Request.QueryString["Id"]);
                }
            }
            // }
        }
        public string CopyFiletoTemp(string strfile, string reportname)
        {

            string ReportTempPath = string.Empty;
            string ReportRetPath = string.Empty;
            ReportTempPath = Server.MapPath(@"/RDLC_Reports/Temp");
            string RDLCBaseFilePath = CommonUtility.RDLCBaseFilePath;

            ReportTempPath = RDLCBaseFilePath + @"/Temp";
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

        private void ShowReport(Int64 Id)
        {
            if (Id <= 0 || SessionHelper.Get("ReportPara") == null)
            {
                Response.Redirect("/ReportBuilder/ViewReports");
            }
            ParentID = 0;
            ReportBuilderDTO objDTO = new ReportBuilderDTO();
            ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            if (SessionHelper.RoleID < 0 && IsRoomGridReportCommon && !string.IsNullOrWhiteSpace(RoomDBName))
                objDAL = new ReportMasterDAL(RoomDBName);

            objDTO = objDAL.GetReportDetail(Id);
            SetReportConnectionString(objDTO);

            LocalDictRptPara = GetReportParaDictionary();
            if (LocalDictRptPara.Keys.Contains("IsRoomGridReportCommon"))
                IsRoomGridReportCommon = Convert.ToBoolean(LocalDictRptPara["IsRoomGridReportCommon"]);
            if (LocalDictRptPara.Keys.Contains("DBName"))
                RoomDBName = LocalDictRptPara["DBName"];
            if (LocalDictRptPara.Keys.Contains("EnterpriseID"))
                EntID = Convert.ToInt64(LocalDictRptPara["EnterpriseID"]);


            if (objDTO != null && objDTO.ReportAppIntent == "ReadOnly" && isRunWithReportConnection == true)
            {
                isRunWithReportConnection = true;
            }
            else
            {
                isRunWithReportConnection = false;
            }

            string DBNameNew = SessionHelper.EnterPriseDBName;
            string _strRunWithReportConnection = "No";
            if (SiteSettingHelper.RunWithReportConnection != string.Empty)
            {
                _strRunWithReportConnection = Convert.ToString(SiteSettingHelper.RunWithReportConnection);
            }
            if (objDTO != null && !string.IsNullOrEmpty(_strRunWithReportConnection) && _strRunWithReportConnection.ToLower() == "yes" && objDTO.ReportAppIntent == "ReadOnly")
            {
                connectionString = DbConnectionHelper.GeteTurnsSQLConnectionString(DBNameNew, DbConnectionType.GeneralReadOnly.ToString("F"));
            }
            else
            {
                connectionString = DbConnectionHelper.GeteTurnsSQLConnectionString(DBNameNew, DbConnectionType.GeneralReadWrite.ToString("F"));
            }

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
            eTurns.DAL.AlertMail amDAL = new eTurns.DAL.AlertMail();

            long EnterPriceID = SessionHelper.EnterPriceID;
            long CompanyID = SessionHelper.CompanyID;
            long RoomID = SessionHelper.RoomID;
            if (EntID > 0 && IsRoomGridReportCommon && !string.IsNullOrWhiteSpace(RoomDBName))
            {
                EnterPriceID = EntID;

                string strCompanyID = string.Empty;
                string strRoomID = string.Empty;
                //System.Xml.Linq.XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));

                //if (Settinfile != null && Settinfile.Element("RoomReportGridCompanyID") != null)
                //    strCompanyID = Convert.ToString(Settinfile.Element("RoomReportGridCompanyID").Value);

                if (SiteSettingHelper.RoomReportGridCompanyID != string.Empty)
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
            string RDLCBaseFilePath = CommonUtility.RDLCBaseFilePath;
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

            bool isReceivedReceivableItemReport = false;
            ReportBuilderDTO oReportBuilderDTO = objDAL.GetParentReportDetailByReportID(objDTO.ID);
            if (oReportBuilderDTO != null)
            {
                if (oReportBuilderDTO.ReportName.ToLower().Equals("item received receivable"))
                {
                    isReceivedReceivableItemReport = true;
                }
                else
                {
                    isReceivedReceivableItemReport = false;
                }
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

            //if (oReportBuilderDTO.ReportName.ToLower().Equals("pull summary by quarter"))
            //{
            //    IEnumerable<XElement> lstTableCell = doc.Descendants(ns + "TablixCell");
            //    var tablixMembers = doc.Descendants(ns + "TablixColumnHierarchy").Descendants(ns + "TablixMembers").Descendants(ns + "TablixMember");//.Descendants(ns + "TablixMembers").Descendants(ns + "TablixMember");//.Remove();
            //    string startDateStr = string.Empty;
            //    string endDateStr =string.Empty;

            //    if (LocalDictRptPara.Keys.Contains("OrigStartDate"))
            //        startDateStr = Convert.ToString(LocalDictRptPara["OrigStartDate"]);

            //    if (LocalDictRptPara.Keys.Contains("OrigEndDate"))
            //        endDateStr = Convert.ToString(LocalDictRptPara["OrigEndDate"]);

            //    //dictionary.Add("OrigStartDate", DateTime.ParseExact(item.value, (SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).AddSeconds(TotalSeconds).ToString("yyyy-MM-dd HH:mm:ss"));
            //    //item.value = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(item.value, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();

            //    DateTime startDate, endDate;
            //    string tmpStartDtStr = startDateStr.Split(' ')[0];
            //    //DateTime.TryParseExact(tmpStartDtStr, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult, System.Globalization.DateTimeStyles.None, out startDate);
            //    DateTime.TryParseExact(tmpStartDtStr, "yyyy-MM-dd", ResourceHelper.CurrentCult, System.Globalization.DateTimeStyles.None, out startDate);
            //    //"yyyy-MM-dd HH:mm:ss"
            //    string tmpEndDtStr = endDateStr.Split(' ')[0];
            //    DateTime.TryParseExact(tmpEndDtStr, "yyyy-MM-dd", ResourceHelper.CurrentCult, System.Globalization.DateTimeStyles.None, out endDate);
            //    int startDateMonth;
            //    int endDateMonth;
            //    int startDateYear;
            //    int endDateYear;
            //    List<string> monthsColumnToKeep = new List<string>();
            //    List<string> monthsColumnToKeepValues = new List<string>();
            //    List<string> monthsColumns = new List<string>();
            //    List<string> monthsColumnsValuesStr = new List<string>();

            //    for (int month = 1; month <=12; month++)
            //    {
            //        monthsColumns.Add("Month" + month);
            //        monthsColumnsValuesStr.Add("=Fields!Month" + month+ ".Value"); //=Fields!Month12.Value
            //    }

            //    if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
            //    {
            //        startDateMonth = startDate.Month;
            //        startDateYear = startDate.Year;
            //        endDateMonth = endDate.Month;
            //        endDateYear = endDate.Year;

            //        if (startDateMonth == endDateMonth && startDateYear == endDateYear)
            //        {
            //            monthsColumnToKeep.Add("Month"+ startDateMonth);
            //            monthsColumnToKeepValues.Add("=Fields!Month" + startDateMonth + ".Value"); //=Fields!Month12.Value
            //        }
            //        else
            //        { 
            //            if (startDateYear == endDateYear)
            //            { 
            //                for (int monthCounter = startDateMonth; monthCounter <= endDateMonth; monthCounter++)
            //                {
            //                    monthsColumnToKeep.Add("Month" + monthCounter);
            //                    monthsColumnToKeepValues.Add("=Fields!Month" + monthCounter + ".Value"); //=Fields!Month12.Value
            //                }
            //            }
            //            else 
            //            {
            //                for (int monthCounter = startDateMonth; monthCounter <= 24; monthCounter++)
            //                {
            //                    int tmpMonth = monthCounter;

            //                    if (tmpMonth > 12)
            //                    {
            //                        tmpMonth-= 12;
            //                    }

            //                    monthsColumnToKeep.Add("Month" + tmpMonth);
            //                    monthsColumnToKeepValues.Add("=Fields!Month" + tmpMonth + ".Value"); //=Fields!Month12.Value

            //                    if (tmpMonth == endDateMonth)
            //                    { 
            //                        break;    
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    List<int> tblixMemeberToDelete = new List<int>();

            //    if(monthsColumnToKeep != null && monthsColumnToKeep.Any() && monthsColumnToKeep.Count > 0)
            //    {
            //        int elementTotalCount = lstTableCell.Count();

            //        for (int cnt = 0; cnt < elementTotalCount; cnt++ ) //foreach (XElement Cell in lstTableCell)
            //        {
            //            var Cell = lstTableCell.ToArray()[cnt];

            //            if (Cell != null)
            //            {
            //                if (Cell.Descendants(ns + "Value").Any())
            //                {
            //                    //=Fields!Month12.Value
            //                    var cellValue = Cell.Descendants(ns + "Value").FirstOrDefault().Value;
            //                    if (!string.IsNullOrEmpty(cellValue) && !string.IsNullOrWhiteSpace(cellValue))
            //                    {
            //                        if (monthsColumns.Contains(cellValue) || monthsColumnsValuesStr.Contains(cellValue))
            //                        {
            //                            if (!monthsColumnToKeep.Contains(cellValue) && !monthsColumnToKeepValues.Contains(cellValue))
            //                            {
            //                                Cell.Remove();
            //                                elementTotalCount -= 1;
            //                                cnt-=1;
            //                                tblixMemeberToDelete.Add(cnt+1);
            //                            }
            //                        }
            //                    }
            //                    //Cell.Remove();
            //                }
            //            }

            //        }

            //        if (tblixMemeberToDelete != null && tblixMemeberToDelete.Any() && tblixMemeberToDelete.Count > 0)
            //        {
            //            foreach (var index in tblixMemeberToDelete)
            //            {
            //                if (tablixMembers.Count() > index)
            //                {
            //                    tablixMembers.ToList()[index].Remove();
            //                }                            
            //            }                        
            //        }                    
            //    }

            //}

            if (isReceivedReceivableItemReport)
            {
                IEnumerable<XElement> lstUpdateTablix = UpdateItemReceivedReceivableResource(lstTablix, MasterReportResFile, oReportBuilderDTO.CombineReportID);
            }
            else
            {
                IEnumerable<XElement> lstUpdateTablix = UpdateResource(lstTablix, MasterReportResFile);
            }
            if (objDTO.ModuleName.ToLower() == "enterpriselist")
            {
                string strEntLogo = LocalDictRptPara["EnterpriseLogoURL"];
                string baseURL = System.Web.HttpContext.Current.Request.Url.ToString().Replace(System.Web.HttpContext.Current.Request.Url.PathAndQuery, "");
                baseURL = SessionHelper.CurrentDomainURL;
                strEntLogo = baseURL + "/Uploads/EnterpriseLogos/";
                LocalDictRptPara["EnterpriseLogoURL"] = strEntLogo;
                string DBServerName = System.Configuration.ConfigurationManager.AppSettings["DBserverName"];
                string DBUserName = System.Configuration.ConfigurationManager.AppSettings["DbUserName"];
                string DBPassword = System.Configuration.ConfigurationManager.AppSettings["DbPassword"];
                string DBName = DbConnectionHelper.GetETurnsMasterDBName();
                string connStr = @"Data Source={0};Initial Catalog={1};User ID={2};Password={3}";
                connStr = string.Format(connStr, DBServerName, DBName, DBUserName, DBPassword);
                connStr = DbConnectionHelper.GeteTurnsMasterSQLConnectionString(DbConnectionHelper.GetETurnsMasterDBName(), DbConnectionType.GeneralReadOnly.ToString("F"));
                LocalDictRptPara["ConnectionString"] = connStr;

                if (isRunWithReportConnection)
                {
                    RPT_connectionString = connStr;
                }
                else
                {
                    connectionString = connStr;
                }


            }
            if (objDTO.ModuleName.ToLower() == "userslist")
            {

                string DBServerName = System.Configuration.ConfigurationManager.AppSettings["DBserverName"];
                string DBUserName = System.Configuration.ConfigurationManager.AppSettings["DbUserName"];
                string DBPassword = System.Configuration.ConfigurationManager.AppSettings["DbPassword"];
                string DBName = DbConnectionHelper.GetETurnsMasterDBName();
                string connStr = @"Data Source={0};Initial Catalog={1};User ID={2};Password={3}";
                connStr = string.Format(connStr, DBServerName, DBName, DBUserName, DBPassword);
                //connStr = DbConnectionHelper.GeteTurnsMasterSQLConnectionString(DbConnectionHelper.GetETurnsMasterDBName(), DbConnectionType.GeneralReadOnly.ToString("F"));
                connStr = DbConnectionHelper.GeteTurnsMasterSQLConnectionString(DbConnectionHelper.GetETurnsMasterDBName(), DbConnectionType.GeneralReadOnly.ToString("F"));
                LocalDictRptPara["ConnectionString"] = connStr;

                if (isRunWithReportConnection)
                {
                    RPT_connectionString = connStr;
                }
                else
                {
                    connectionString = connStr;
                }

            }
            if (objDTO.ModuleName.ToLower() == "user_list")
            {

                string DBServerName = System.Configuration.ConfigurationManager.AppSettings["DBserverName"];
                string DBUserName = System.Configuration.ConfigurationManager.AppSettings["DbUserName"];
                string DBPassword = System.Configuration.ConfigurationManager.AppSettings["DbPassword"];
                string DBName = DbConnectionHelper.GetETurnsMasterDBName();
                string connStr = @"Data Source={0};Initial Catalog={1};User ID={2};Password={3}";
                connStr = string.Format(connStr, DBServerName, DBName, DBUserName, DBPassword);
                //connStr = DbConnectionHelper.GeteTurnsMasterSQLConnectionString(DbConnectionHelper.GetETurnsMasterDBName(), DbConnectionType.GeneralReadOnly.ToString("F"));
                connStr = DbConnectionHelper.GeteTurnsMasterSQLConnectionString(DbConnectionHelper.GetETurnsMasterDBName(), DbConnectionType.GeneralReadOnly.ToString("F"));
                LocalDictRptPara["ConnectionString"] = connStr;

                if (isRunWithReportConnection)
                {
                    RPT_connectionString = connStr;
                }
                else
                {
                    connectionString = connStr;
                }

            }
            #region WI-4947 - Please add a Last Run Date to the schedule reports

            if (LocalDictRptPara != null && LocalDictRptPara.Count > 0)
            {
                string JsonReportParameters = string.Empty;
                var convertedDictionary = LocalDictRptPara.ToDictionary(item => Convert.ToString(item.Key), item => Convert.ToString(item.Value));
                var json = new JavaScriptSerializer().Serialize(convertedDictionary);
                JsonReportParameters = new JavaScriptSerializer().Serialize(convertedDictionary);

                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                ViewReportHistory objViewReportHistory = new ViewReportHistory();
                objViewReportHistory.ReportID = Id;
                objViewReportHistory.RoomId = SessionHelper.RoomID;
                objViewReportHistory.CompanyId = SessionHelper.CompanyID;
                objViewReportHistory.UserId = SessionHelper.UserID;
                objViewReportHistory.RequestType = "ViewReport";
                objViewReportHistory.ReportParameters = JsonReportParameters;
                objReportMasterDAL.InsertViewReportHistory(objViewReportHistory);
            }

            #endregion

            //TODO: Start WI-1627: Setting the sort fields does not work

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
            if (!LocalDictRptPara.Keys.Contains("ReportFields"))
            {
                try
                {
                    List<XElement> lstSortingRows = new List<XElement>();
                    List<XElement> lstReportFieldsnew = new List<XElement>();

                    lstSortingRows = doc.Descendants(ns + "Tablix").Descendants(ns + "TablixRow").ToList();
                    if (lstSortingRows.Count > 1)
                    {
                        //lstReportFields = lstSortingRows.Count > 1 ? lstSortingRows[1].Descendants(ns + "TablixCell").ToList()
                        //                                                : lstSortingRows[0].Descendants(ns + "TablixCell").ToList();
                        List<string> lstReportFields = lstSortingRows.SelectMany(x => x.Descendants(ns + "Value")).Where(x => x.Value.Contains("Fields!")).Select(x => x.Value).ToList();

                        List<string> lstReportFieldsList = new List<string>();
                        foreach (string cl in lstReportFields)
                        {
                            string tdval = string.Empty;
                            if (!string.IsNullOrEmpty(cl))
                            {
                                string str = cl;
                                string output = string.Empty;
                                while (str != string.Empty)
                                {
                                    int pos1 = str.IndexOf("Fields!") + "Fields!".Length;

                                    if (str.IndexOf("Fields!") >= 0)
                                    {
                                        int pos2 = str.Substring(pos1).IndexOf(".Value");
                                        string outputStr = str.Substring(pos1).Substring(0, pos2 + (".Value").Length);
                                        tdval = outputStr;

                                        str = str.Replace("Fields!" + outputStr, "");
                                        if (!string.IsNullOrEmpty(tdval))
                                        {
                                            tdval = tdval.Replace("=Fields!", "").Replace(".Value", "");
                                            lstReportFieldsList.Add(tdval);
                                        }
                                    }
                                    else
                                    {
                                        str = "";
                                    }
                                }

                            }
                        }
                        ReportParameter rparaReportFieldsList = new ReportParameter();
                        rparaReportFieldsList.Name = "ReportFields";
                        string strlstReportFieldsList = string.Join(", ", lstReportFieldsList.Distinct());
                        rparaReportFieldsList.Values.Add(strlstReportFieldsList);
                        LocalDictRptPara.Add("ReportFields", strlstReportFieldsList);
                    }
                }
                catch (Exception ex)
                {

                }
            }
            //TODO: End WI-1627: Setting the sort fields does not work

            doc.Save(ReportPath);
            //doc.Descendants(ns + "Tablix").FirstOrDefault().Value = lstUpdateTablix.FirstOrDefault().Value;
            IEnumerable<XElement> lstReportPara = doc.Descendants(ns + "ReportParameter");
            List<ReportParameter> rpt = new List<ReportParameter>();


            if (doc.Descendants(ns + "Subreport") != null && doc.Descendants(ns + "Subreport").Count() > 0)
            {
                hasSubReport = true;
            }
            if (LocalDictRptPara.Keys.Contains("IsNoHeader"))
            {
                //if (!hasSubReport && !objDTO.IsNotEditable.GetValueOrDefault(false)
                //                 && (objDTO.ReportType == 3 || objDTO.ReportType == 1)
                //                 && LocalDictRptPara["IsNoHeader"] == "1")
                if (!objDTO.IsNotEditable.GetValueOrDefault(false) && LocalDictRptPara["IsNoHeader"] == "1"
                     && (objDTO.ReportType == 1 || objDTO.ReportType == 2 || objDTO.ReportType == 3))
                {
                    doc = objAlertMail.GetAdditionalHeaderRow(doc, objDTO, SessionHelper.CompanyName, SessionHelper.RoomName, LocalDictRptPara, EnterpriseDBName: SessionHelper.EnterPriseDBName);
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


            string[] arrRooms = null;

            if (LocalDictRptPara.Keys.Contains("RoomIDs"))
            {
                string strRoomIDs = LocalDictRptPara["RoomIDs"];
                arrRooms = strRoomIDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }

            //TODO: Start WI-3754
            if (objDTO.ReportType == 2 && doc.Descendants(ns + "TablixBody").Descendants(ns + "TablixColumns").Descendants(ns + "TablixColumn").Count() == 4)
            {
                IEnumerable<XElement> lstColumns = doc.Descendants(ns + "TablixBody").Descendants(ns + "TablixColumns").Descendants(ns + "TablixColumn");
                int index = 0;
                foreach (var item in lstColumns)
                {
                    if (index % 2 == 0)
                        item.Descendants(ns + "Width").FirstOrDefault().Value = "1.00in";
                    else
                    {
                        if (double.Parse(doc.Root.Element(ns + "Width").Value.Replace("in", "")) > 9)
                            item.Descendants(ns + "Width").FirstOrDefault().Value = "4.15in";
                        else
                            item.Descendants(ns + "Width").FirstOrDefault().Value = "2.95in";
                    }
                    index += 1;
                }
                doc.Save(ReportPath);
            }
            //TODO: End WI-3754

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

                    //foreach (var item in RoomDTO)
                    //{
                    //    if (strRoomCompany.Length > 0)
                    //        strRoomCompany += ",";

                    //    strRoomCompany += " " + item.CompanyName + "\t" + item.RoomName;
                    //}
                    //if (doc.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Textbox").Descendants(ns + "Value").FirstOrDefault(x => x.Value.Contains("Fields!RoomInfo.Value")).Parent.Parent.Parent.Parent.Parent.Descendants(ns + "Height").Count() > 0)
                    //{
                    //    string height = doc.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Textbox").Descendants(ns + "Value").FirstOrDefault(x => x.Value.Contains("Fields!RoomInfo.Value")).Parent.Parent.Parent.Parent.Parent.Descendants(ns + "Height").FirstOrDefault().Value;
                    //    string intHieght = height.Replace("in", "");
                    //    double dblHeight = double.Parse(intHieght);
                    //    double newHeight = 0.15 * arrRooms.Length;
                    //    if (dblHeight < newHeight)
                    //    {
                    //        doc.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Textbox").Descendants(ns + "Value").FirstOrDefault(x => x.Value.Contains("Fields!RoomInfo.Value")).Parent.Parent.Parent.Parent.Parent.Descendants(ns + "Height").FirstOrDefault().Value = Convert.ToString(newHeight) + "in";
                    //        doc.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "Height").FirstOrDefault().Value = Convert.ToString(0.855 + newHeight) + "in";
                    //    }
                    //}


                }
                //if (1 == 0)
                //{
                //    IEnumerable<XElement> lstGroups = doc.Descendants(ns + "TablixRowHierarchy").Descendants(ns + "TablixMembers").Descendants(ns + "GroupExpressions");
                //    string strInnerTextOfTablixRowHierarchy = string.Empty;
                //    if (objDTO.ReportType == 1 && lstGroups.Count() == 0)
                //    {
                //        string footer = "";
                //        if (objDTO.IsIncludeGrandTotal)
                //            footer = objAlertMail.GetBaseFooter();

                //        strInnerTextOfTablixRowHierarchy = objAlertMail.GetTablixRowHierarchy("", "", footer, false);
                //        doc.Descendants(ns + "TablixRowHierarchy").FirstOrDefault().Value = strInnerTextOfTablixRowHierarchy;
                //    }
                //    else if (objDTO.ReportType == 3 && lstGroups.Count() >= 1 && lstGroups.Descendants(ns + "GroupExpression").FirstOrDefault() != null)
                //    {
                //        XElement[] lstTablixMember = doc.Descendants(ns + "TablixRowHierarchy").Elements(ns + "TablixMembers").Elements(ns + "TablixMember").ToArray();
                //        string header = string.Empty;
                //        string group = string.Empty;
                //        string footer = string.Empty;
                //        if (lstTablixMember.Length > 0)
                //            header = lstTablixMember[0].ToString();
                //        if (lstTablixMember.Length > 1)
                //            group = lstTablixMember[1].ToString();
                //        if (lstTablixMember.Length > 2)
                //            footer = lstTablixMember[2].ToString();
                //        strInnerTextOfTablixRowHierarchy = objAlertMail.GetTablixRowHierarchy(header, group, footer, true);
                //    }

                //    if (!string.IsNullOrEmpty(strInnerTextOfTablixRowHierarchy))
                //    {
                //        bool hasRoomInfo = false;

                //        if (doc.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Textbox").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "TextboxRoom") != null)
                //        {
                //            doc.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Textbox").FirstOrDefault(x => x.Attribute("Name").Value.ToString().Trim() == "TextboxRoom").Descendants(ns + "Value").FirstOrDefault().Value = "=ReportItems!txtRoomInfo.Value";
                //            hasRoomInfo = true;
                //        }
                //        else if (doc.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Textbox").Descendants(ns + "Value").FirstOrDefault(x => x.Value.Contains("Fields!RoomInfo.Value")) != null)
                //        {
                //            doc.Descendants(ns + "Page").Descendants(ns + "PageHeader").Descendants(ns + "ReportItems").Descendants(ns + "Textbox").Descendants(ns + "Value").FirstOrDefault(x => x.Value.Contains("Fields!RoomInfo.Value")).Value = "=ReportItems!txtRoomInfo.Value";
                //            hasRoomInfo = true;
                //        }
                //        if (hasRoomInfo)
                //        {
                //            doc.Descendants(ns + "TablixRowHierarchy").FirstOrDefault().Value = strInnerTextOfTablixRowHierarchy;
                //            string strdoc = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
                //            strdoc += doc.ToString().Replace("&gt;", ">");
                //            strdoc = strdoc.ToString().Replace("&lt;", "<");
                //            System.IO.File.WriteAllText(ReportPath, strdoc);
                //            doc = XDocument.Load(ReportPath);
                //        }
                //    }
                //}
            }

            if (LocalDictRptPara.Keys.Contains("ShowSignature"))
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
                    //DateTime.TryParseExact(tmpStartDtStr, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult, System.Globalization.DateTimeStyles.None, out startDate);
                    DateTime.TryParse(tmpStartDtStr, out startDate);
                    //item.value = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(item.value, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();

                    string tmpEndDtStr = endDateStr.Split(' ')[0];
                    //DateTime.TryParseExact(tmpEndDtStr, "yyyy-MM-dd", ResourceHelper.CurrentCult, System.Globalization.DateTimeStyles.None, out endDate);
                    //DateTime.TryParseExact(tmpEndDtStr, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult, System.Globalization.DateTimeStyles.None, out endDate);
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
                            //monthStrToCompareWithParam = rpara.Name.Replace("Is", "").Replace("Visible", "");
                            //monthVSValue = monthsColumnToKeep.Contains(monthStrToCompareWithParam) ? "true" : "false";
                            //rpara.Values.Add(monthVSValue);

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
                        //IsMonth1Visible
                    }
                }
                else
                {
                    foreach (var item in lstReportPara)
                    {
                        ReportParameter rpara = new ReportParameter();
                        rpara.Name = item.Attribute("Name").Value;
                        if (!string.IsNullOrEmpty(LocalDictRptPara.FirstOrDefault(x => x.Key.Replace("@", "") == item.Attribute("Name").Value.Replace("@", "")).Value))
                        {
                            rpara.Values.Add(LocalDictRptPara.FirstOrDefault(x => x.Key.Replace("@", "") == item.Attribute("Name").Value.Replace("@", "")).Value);
                        }

                        rpt.Add(rpara);
                    }
                }

            }
            DataTable dt = new DataTable();
            DataTable dtReceivablesItems = new DataTable();
            string connString = doc.Descendants(ns + "ConnectString").FirstOrDefault().Value;

            using (SqlConnection myConnection = new SqlConnection())
            {
                if (isReceivedReceivableItemReport)
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
                        if (isRunWithReportConnection)
                        {
                            myConnection.ConnectionString = RPT_connectionString;// connString;
                        }
                        else
                        {
                            myConnection.ConnectionString = connectionString;// connString;
                        }
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
                    SqlCommand cmd = new SqlCommand();
                    SqlDataAdapter sqla = new SqlDataAdapter();

                    if (isRunWithReportConnection)
                    {
                        myConnection.ConnectionString = RPT_connectionString;// connString;
                    }
                    else
                    {
                        myConnection.ConnectionString = connectionString;// connString;
                    }

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
                            if (objReportPara != null)
                            {
                                if (objReportPara.Descendants(ns + "DataType") != null && objReportPara.Descendants(ns + "DataType").Count() > 0)
                                {
                                    slpar.DbType = (DbType)Enum.Parse(typeof(DbType), objReportPara.Descendants(ns + "DataType").FirstOrDefault().Value, true);
                                }

                                cmd.Parameters.Add(slpar);
                            }
                        }
                    }

                    //
                    if (cmd.CommandText == "RPT_GetAuditTrail_Data" || cmd.CommandText == "RPT_GetItemAuditTrail_Trans")
                    {
                        Call_Sub_SP_ForDML(cmd);
                    }

                    cmd.CommandTimeout = 7200;
                    sqla = new SqlDataAdapter(cmd);


                    sqla.Fill(dt);
                    sqla.Dispose();
                }
            }

            ReportViewer1.Reset();
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.Visible = true;

            if (hasSubReport)
            {
                hasSubReport = true;
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
                if (lstSubTablix != null && lstSubTablix.ToList().Count > 0)
                {
                    strSubTablix = lstSubTablix.ToList()[0].ToString();
                }

                docSub = amDAL.AddFormatToTaxbox(docSub, rsInfo);
                docSub.Save(rdlPath);
                docSub = XDocument.Load(rdlPath);


                ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(LoadSubreport);

                ReportViewer1.LocalReport.EnableExternalImages = true;
                ReportViewer1.LocalReport.EnableHyperlinks = true;
            }

            //if (!hasSubReport && LocalDictRptPara.ContainsKey("SortFields") && !string.IsNullOrEmpty(LocalDictRptPara["SortFields"]))
            //{
            //    string SortFields = LocalDictRptPara["SortFields"];

            //    if (!string.IsNullOrEmpty(SortFields))
            //    {
            //        dt.DefaultView.Sort = SortFields;
            //        dt = dt.DefaultView.ToTable();
            //    }
            //}

            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.ReportPath = ReportPath;
            if (isReceivedReceivableItemReport)
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
                    ReportViewer1.LocalReport.DataSources.Add(rds);
                    globalcounter = dt.Rows.Count;

                }
            }
            else
            {
                ReportDataSource rds = new ReportDataSource();
                rds.Name = doc.Descendants(ns + "DataSet").FirstOrDefault().FirstAttribute.Value;
                rds.Value = dt;
                globalcounter = dt.Rows.Count;
                ReportViewer1.LocalReport.DataSources.Add(rds);
            }

            ReportViewer1.LocalReport.SetParameters(rpt);
            ReportViewer1.ZoomMode = ZoomMode.Percent;
            ReportViewer1.LocalReport.Refresh();
            //if (doc.Descendants(ns + "Subreport") != null && doc.Descendants(ns + "Subreport").Count() > 0)
            //{
            //    RefreshSubReport(strSubTablix, rdlPath);
            //}
            ////strTablix = strTablix.Replace("xmlns:rd=\"http://schemas.microsoft.com/SQLServer/reporting/reportdesigner\"", "").Replace("xmlns=\"http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition\"", "");
            ////strTablix = strTablix.Replace("<Tablix Name=\"Tablix1\" >", "");
            ////strTablix = strTablix.Replace("</Tablix>", "");
            ////doc.Descendants(ns + "Tablix").First().Value = strTablix;
            ////string strSubdoc = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
            ////strSubdoc += doc.ToString().Replace("&gt;", ">");
            ////strSubdoc = strSubdoc.ToString().Replace("&lt;", "<");
            ////System.IO.File.WriteAllText(ReportPath, strSubdoc);

            dt.Dispose();
        }

        private void Call_Sub_SP_ForDML(SqlCommand cmd)
        {
            ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            if (cmd.CommandText == "RPT_GetAuditTrail_Data")
            {
                string _dataGuids = string.Empty;
                if (cmd.Parameters.Contains("@DataGuids"))
                {
                    _dataGuids = (cmd.Parameters["@DataGuids"].Value ?? string.Empty).ToString();
                    if (!string.IsNullOrEmpty(_dataGuids))
                    {
                        objDAL.Call_AT_AT_Calc_Qty(_dataGuids);
                    }
                }
            }
            else if (cmd.CommandText == "RPT_GetItemAuditTrail_Trans")
            {
                string _dataGuids = string.Empty;
                if (cmd.Parameters.Contains("@DataGuids"))
                {
                    _dataGuids = (cmd.Parameters["@DataGuids"].Value ?? string.Empty).ToString();
                    if (!string.IsNullOrEmpty(_dataGuids))
                    {
                        objDAL.Call_AT_AuditTrail_CalculateQty(_dataGuids);
                    }
                }
            }
        }

        private void ShowReportModuleWise(Int64 Id)
        {
            ParentID = 0;
            ReportBuilderDTO objDTO = new ReportBuilderDTO();
            ReportMasterDAL objDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            objDTO = objDAL.GetReportDetail(Id);
            string MasterReportResFile = objDTO.MasterReportResFile;
            SubReportResFile = MasterReportResFile;// objDTO.SubReportResFile;
            string Reportname = objDTO.ReportName;
            string MasterReportname = objDTO.ReportFileName;
            string SubReportname = objDTO.SubReportFileName;
            string mainGuid = "RPT_" + Guid.NewGuid().ToString().Replace("-", "_");
            string subGuid = "SubRPT_" + Guid.NewGuid().ToString().Replace("-", "_");
            string ReportPath = string.Empty;
            //bool hasSubReport = false;
            ParentID = objDTO.ParentID ?? 0;
            string RDLCBaseFilePath = CommonUtility.RDLCBaseFilePath;
            if (objDTO.ParentID > 0)
            {
                if (objDTO.ISEnterpriseReport.GetValueOrDefault(false))
                {
                    ReportPath = CopyFiletoTemp(RDLCBaseFilePath + @"/" + SessionHelper.EnterPriceID.ToString() + "/EnterpriseReport" + @"\\" + MasterReportname, mainGuid);
                }
                else
                {
                    ReportPath = CopyFiletoTemp(RDLCBaseFilePath + @"/" + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID + @"\\" + MasterReportname, mainGuid);
                }
            }
            else
            {
                ReportPath = CopyFiletoTemp(RDLCBaseFilePath + @"/" + SessionHelper.EnterPriceID.ToString() + "/BaseReport" + @"\\" + MasterReportname, mainGuid);
            }



            XDocument doc = XDocument.Load(ReportPath);
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
            LocalDictRptPara = GetReportParaDictionary();

            if (lstReportPara != null && lstReportPara.Count() > 0)
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

            string connString = doc.Descendants(ns + "ConnectString").FirstOrDefault().Value;
            SqlConnection myConnection = new SqlConnection();
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter sqla = new SqlDataAdapter();
            DataTable dt = new DataTable();

            if (isRunWithReportConnection)
            {
                myConnection.ConnectionString = RPT_connectionString;// connString;
            }
            else
            {
                myConnection.ConnectionString = connectionString;// connString;
            }

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
                    if (!string.IsNullOrEmpty(LocalDictRptPara.FirstOrDefault(x => x.Key.Replace("@", "") == item.Attribute("Name").Value.Replace("@", "")).Value))
                        slpar.Value = LocalDictRptPara.FirstOrDefault(x => x.Key.Replace("@", "") == item.Attribute("Name").Value.Replace("@", "")).Value;
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


            ReportViewer1.Reset();
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.Visible = true;
            //string strSubTablix = string.Empty;
            //string rdlPath = string.Empty;
            if (doc.Descendants(ns + "Subreport") != null && doc.Descendants(ns + "Subreport").Count() > 0)
            {
                // hasSubReport = true;
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
                docSub.Save(rdlPath);

                ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(LoadSubreport);

                ReportViewer1.LocalReport.EnableExternalImages = true;
                ReportViewer1.LocalReport.EnableHyperlinks = true;
                //ReportViewer1.LocalReport.Refresh();
            }
            //if (!hasSubReport && LocalDictRptPara.ContainsKey("SortFields") && !string.IsNullOrEmpty(LocalDictRptPara["SortFields"]))
            //{
            //    string SortFields = LocalDictRptPara["SortFields"];

            //    if (!string.IsNullOrEmpty(SortFields))
            //    {
            //        dt.DefaultView.Sort = SortFields;
            //        dt = dt.DefaultView.ToTable();

            //    }
            //}
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.ReportPath = ReportPath;
            ReportDataSource rds = new ReportDataSource();
            rds.Name = doc.Descendants(ns + "DataSet").FirstOrDefault().FirstAttribute.Value;
            rds.Value = dt;
            globalcounter = dt.Rows.Count;

            ReportViewer1.LocalReport.DataSources.Add(rds);
            ReportViewer1.LocalReport.SetParameters(rpt);
            ReportViewer1.ZoomMode = ZoomMode.Percent;
            //ReportViewer1.LocalReport.Refresh();
            //if (doc.Descendants(ns + "Subreport") != null && doc.Descendants(ns + "Subreport").Count() > 0)
            //{
            //    RefreshSubReport(strSubTablix, rdlPath);
            //}
            ////strTablix = strTablix.Replace("xmlns:rd=\"http://schemas.microsoft.com/SQLServer/reporting/reportdesigner\"", "").Replace("xmlns=\"http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition\"", "");
            ////strTablix = strTablix.Replace("<Tablix Name=\"Tablix1\" >", "");
            ////strTablix = strTablix.Replace("</Tablix>", "");
            ////doc.Descendants(ns + "Tablix").First().Value = strTablix;
            ////string strSubdoc = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
            ////strSubdoc += doc.ToString().Replace("&gt;", ">");
            ////strSubdoc = strSubdoc.ToString().Replace("&lt;", "<");
            ////System.IO.File.WriteAllText(ReportPath, strSubdoc);


        }
        public void RefreshSubReport(string strTablix, string rdlPath)
        {
            XDocument doc = XDocument.Load(rdlPath);
            strTablix = strTablix.Replace("xmlns:rd=\"http://schemas.microsoft.com/SQLServer/reporting/reportdesigner\"", "").Replace("xmlns=\"http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition\"", "");
            strTablix = strTablix.Replace("<Tablix Name=\"Tablix1\" >", "");
            strTablix = strTablix.Replace("</Tablix>", "");
            doc.Descendants(ns + "Tablix").First().Value = strTablix;
            string strSubdoc = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
            strSubdoc += doc.ToString().Replace("&gt;", ">");
            strSubdoc = strSubdoc.ToString().Replace("&lt;", "<");
            System.IO.File.WriteAllText(rdlPath, strSubdoc);
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

        public string GetResourceValue(string FileName, string Key)
        {
            string KeyVal = string.Empty;
            int ResRead = !string.IsNullOrWhiteSpace(SiteSettingHelper.ResourceRead) ? Convert.ToInt32(SiteSettingHelper.ResourceRead) : (int)ResourceReadType.File;
            if (Key.ToLower().Contains("udf"))
            {
                string PreFixValue = Key.Replace("UDF10", string.Empty).Replace("UDF1", string.Empty).Replace("UDF2", string.Empty).Replace("UDF3", string.Empty).Replace("UDF4", string.Empty).Replace("UDF5", string.Empty).Replace(".Value", string.Empty).Replace("UDF6", string.Empty).Replace("UDF7", string.Empty).Replace("UDF8", string.Empty).Replace("UDF9", string.Empty);

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

            if (isRunWithReportConnection)
            {
                myConnection.ConnectionString = RPT_connectionString;// connString;
            }
            else
            {
                myConnection.ConnectionString = connectionString;// connString;
            }


            cmd.Connection = myConnection;
            cmd.CommandText = doc.Descendants(ns + "CommandText").FirstOrDefault().Value; //"SELECT *  FROM   ItemMaster_View";
            cmd.CommandType = CommandType.Text;
            if (doc.Descendants(ns + "CommandType").FirstOrDefault() != null)
            {
                cmd.CommandType = (CommandType)Enum.Parse(typeof(CommandType), doc.Descendants(ns + "CommandType").FirstOrDefault().Value == null ? "Text" : doc.Descendants(ns + "CommandType").FirstOrDefault().Value, true);
            }

            IEnumerable<XElement> lstReportPara = doc.Descendants(ns + "ReportParameter");
            List<ReportParameter> rpt = new List<ReportParameter>();
            string strlstReportFieldsList = "";
            try
            {
                List<XElement> lstSortingRows = new List<XElement>();
                List<XElement> lstReportFieldsnew = new List<XElement>();

                lstSortingRows = doc.Descendants(ns + "Tablix").Descendants(ns + "TablixRow").ToList();
                if (lstSortingRows.Count > 1)
                {
                    //lstReportFields = lstSortingRows.Count > 1 ? lstSortingRows[1].Descendants(ns + "TablixCell").ToList()
                    //                                                : lstSortingRows[0].Descendants(ns + "TablixCell").ToList();
                    List<string> lstReportFields = lstSortingRows.SelectMany(x => x.Descendants(ns + "Value")).Where(x => x.Value.Contains("Fields!")).Select(x => x.Value).ToList();

                    List<string> lstReportFieldsList = new List<string>();
                    foreach (string cl in lstReportFields)
                    {
                        string tdval = string.Empty;
                        if (!string.IsNullOrEmpty(cl))
                        {
                            string str = cl;
                            string output = string.Empty;
                            while (str != string.Empty)
                            {
                                int pos1 = str.IndexOf("Fields!") + "Fields!".Length;

                                if (str.IndexOf("Fields!") >= 0)
                                {
                                    int pos2 = str.Substring(pos1).IndexOf(".Value");
                                    string outputStr = str.Substring(pos1).Substring(0, pos2 + (".Value").Length);
                                    tdval = outputStr;

                                    str = str.Replace("Fields!" + outputStr, "");
                                    if (!string.IsNullOrEmpty(tdval))
                                    {
                                        tdval = tdval.Replace("=Fields!", "").Replace(".Value", "");
                                        lstReportFieldsList.Add(tdval);
                                    }
                                }
                                else
                                {
                                    str = "";
                                }
                            }

                        }
                    }
                    ReportParameter rparaReportFieldsList = new ReportParameter();
                    rparaReportFieldsList.Name = "ReportFields";
                    strlstReportFieldsList = string.Join(", ", lstReportFieldsList.Distinct());
                    rparaReportFieldsList.Values.Add(strlstReportFieldsList);
                    rpt.Add(rparaReportFieldsList);
                }
            }
            catch (Exception ex)
            {

            }

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
                    if (slpar.ParameterName.Replace("@", "").ToLower() == "reportfields")
                    {
                        slpar.Value = strlstReportFieldsList;
                    }
                    else
                    {
                        slpar.Value = e.Parameters[slpar.ParameterName.Replace("@", "")].Values[0];
                    }
                    XElement objReportPara = lstReportPara.FirstOrDefault(x => x.Attribute("Name").Value == slpar.ParameterName.Replace("@", ""));
                    if (objReportPara.Descendants(ns + "DataType") != null && objReportPara.Descendants(ns + "DataType").Count() > 0)
                    {
                        slpar.DbType = (DbType)Enum.Parse(typeof(DbType), objReportPara.Descendants(ns + "DataType").FirstOrDefault().Value, true);
                    }
                    if (cmd.Parameters.IndexOf(slpar.ParameterName) < 0)
                        cmd.Parameters.Add(slpar);
                }
            }

            cmd.CommandTimeout = 7200;
            sqla = new SqlDataAdapter(cmd);

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

        }

        protected void btnPrintReport_Click(object sender, EventArgs e)
        {

            Int64 ReportId = 0;
            ReportHelper objReportHelper = new ReportHelper();
            if (!string.IsNullOrEmpty(Request.QueryString["Id"]))
            {
                ReportId = Convert.ToInt64(Request.QueryString["Id"]);
            }
            objReportHelper.LoadReport(ReportId);
        }


        public String ReportName { get; set; }
        public String ReportTitle { get; set; }


        /// <summary>
        /// Bind Report With DataSet
        /// </summary>
        /// <param name="ds">DataSet</param>
        public void ReportBinding(string ReportFor)
        {
            DataSet ds = new DataSet();
            LocalDictRptPara = GetReportParaDictionary();
            string connString = string.Empty;
            SqlConnection myConnection = new SqlConnection();
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter sqla = new SqlDataAdapter();
            string strPrependTitle = string.Empty;



            if (isRunWithReportConnection)
            {
                myConnection.ConnectionString = RPT_connectionString;// connString;
            }
            else
            {
                myConnection.ConnectionString = connectionString;// connString;
            }



            string recordId = LocalDictRptPara["RecordID"];
            cmd.Connection = myConnection;
            cmd.CommandText = "GetChangeLogByPageName";
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter slpar = new SqlParameter();
            slpar.ParameterName = "@ChangeLogFor";//
            slpar.Value = ReportFor;
            cmd.Parameters.Add(slpar);
            slpar = new SqlParameter();
            slpar.ParameterName = "@RecordID";//
            slpar.Value = recordId;
            cmd.Parameters.Add(slpar);
            slpar = new SqlParameter();
            slpar.ParameterName = "@RoomId";//
            slpar.Value = SessionHelper.RoomID;
            cmd.Parameters.Add(slpar);
            slpar = new SqlParameter();
            slpar.ParameterName = "@UDFTableName";//
            slpar.Value = GetResourceFileNameOrTitleOrUDFName(ReportFor, ReturnStringType.UDFTableName, strPrependTitle);
            cmd.Parameters.Add(slpar);

            cmd.CommandTimeout = 7200;
            sqla = new SqlDataAdapter(cmd);

            sqla.Fill(ds);

            int count = 0;

            foreach (DataTable dt in ds.Tables)
            {
                if (LocalDictRptPara.ContainsKey("OnlyChangedColumns") && LocalDictRptPara["OnlyChangedColumns"] == "True")
                {
                    List<DataColumn> columnsToDelete = new List<DataColumn>();
                    foreach (DataColumn col in dt.Columns)
                    {
                        if (col.ColumnName.Contains("CurrentDateTime") || col.ColumnName.Contains("RoomInfo"))
                            continue;

                        if (string.IsNullOrEmpty(strPrependTitle) && IsMainColumn(ReportFor, col))
                            strPrependTitle = Convert.ToString(dt.Rows[0][col]);

                        object first = dt.Rows[0][col];
                        if (dt.AsEnumerable().Skip(1).All(r => r[col].Equals(first)))
                        {
                            columnsToDelete.Add(col);
                        }
                    }

                    foreach (DataColumn colToRemove in columnsToDelete)
                        dt.Columns.Remove(colToRemove);
                }

                count++;
                var report_name = "Report" + count;
                DataTable dt1 = new DataTable(report_name.ToString());
                dt1 = ds.Tables[count - 1];
                dt1.TableName = report_name.ToString();
            }

            if (!string.IsNullOrEmpty(strPrependTitle))
                strPrependTitle = "\"" + strPrependTitle + "\"" + " - ";


            //Report Viewer, Builder and Engine 
            ReportViewer1.Reset();
            for (int i = 0; i < ds.Tables.Count; i++)
                ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource(ds.Tables[i].TableName, ds.Tables[i]));

            MainReportBuilder reportBuilder = new MainReportBuilder();
            reportBuilder.DataSource = ds;

            reportBuilder.Page = new ReportPage();

            //ReportSections reportFooter = new ReportSections();
            //ReportItems reportFooterItems = new ReportItems();
            //ReportTextBoxControl[] footerTxt = new ReportTextBoxControl[3];
            //string footer = string.Format("Copyright  {0}         Report Generated On  {1}          Page  {2}  of {3} ", DateTime.Now.Year, DateTime.Now, ReportGlobalParameters.CurrentPageNumber, ReportGlobalParameters.TotalPages);
            //footerTxt[0] = new ReportTextBoxControl() { Name = "txtCopyright", ValueOrExpression = new string[] { footer } };
            //reportFooterItems.TextBoxControls = footerTxt;
            //reportFooter.ReportControlItems = reportFooterItems;
            //reportBuilder.Page.ReportFooter = reportFooter;

            ReportSections reportHeader = new ReportSections();
            reportHeader.Size = new ReportScale();
            reportHeader.Size.Height = 1.50;

            ReportItems reportHeaderItems = new ReportItems();

            ReportTextBoxControl[] headerTxt = new ReportTextBoxControl[1];
            //headerTxt[0] = new ReportTextBoxControl() { Name = "txtReportTitle", ValueOrExpression = new string[] { "Report Name: " + ReportTitle } };
            headerTxt[0] = new ReportTextBoxControl() { Name = "txtReportTitle", ValueOrExpression = new string[] { "" } };

            List<ReportParameter> rpt = new List<ReportParameter>();
            ReportParameter rpara = new ReportParameter();
            rpara.Name = "eTurnsLogoURL";
            rpara.Values.Add(LocalDictRptPara["eTurnsLogoURL"]);
            rpt.Add(rpara);

            rpara = new ReportParameter();
            rpara.Name = "EnterpriseLogoURL";
            rpara.Values.Add(LocalDictRptPara["EnterpriseLogoURL"]);
            rpt.Add(rpara);

            rpara = new ReportParameter();
            rpara.Name = "CompanyLogoURL";
            rpara.Values.Add(LocalDictRptPara["CompanyLogoURL"]);
            rpt.Add(rpara);
            rpara = new ReportParameter();
            rpara.Name = "ReportTitle";
            rpara.Values.Add(GetResourceFileNameOrTitleOrUDFName(ReportFor, ReturnStringType.ReportTitle, strPrependTitle));
            rpt.Add(rpara);

            ReportEngine.ResourceFileName = GetResourceFileNameOrTitleOrUDFName(ReportFor, ReturnStringType.ResourceFile, strPrependTitle);
            reportHeaderItems.TextBoxControls = headerTxt;
            reportHeader.ReportControlItems = reportHeaderItems;
            reportBuilder.Page.ReportHeader = reportHeader;
            ReportViewer1.LocalReport.LoadReportDefinition(ReportEngine.GenerateReport(reportBuilder));
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.SetParameters(rpt);
            ReportViewer1.LocalReport.DisplayName = ReportName;

        }


        private bool IsMainColumn(string ReportOf, DataColumn col)
        {
            if (ReportOf.ToLower().Contains("shipviachangelog") && col.ColumnName.ToLower().Contains("shipvia"))
                return true;
            if (ReportOf.ToLower().Contains("itemmasterchangelog") && col.ColumnName.ToLower().Contains("itemnumber"))
                return true;

            return false;
        }

        private string GetMainColumnValue(string ReportOf, DataColumn col, DataTable dt)
        {
            if (ReportOf.ToLower().Contains("shipviachangelog") && col.ColumnName.ToLower().Contains("shipvia"))
                return Convert.ToString(dt.Rows[0]["ShipVia"]);
            if (ReportOf.ToLower().Contains("itemmasterchangelog") && col.ColumnName.ToLower().Contains("itemnumber"))
                return Convert.ToString(dt.Rows[0]["ItemNumber"]);

            return string.Empty;
        }


        private string GetResourceFileNameOrTitleOrUDFName(string ReportOf, ReturnStringType returnType, string PrependTitle)
        {
            if (ReportOf.ToLower().Contains("shipviachangelog"))
            {
                if (returnType == ReturnStringType.ResourceFile)
                    return "ResShipVia";
                else if (returnType == ReturnStringType.ReportTitle)
                    return "ShipVai Change Log";
                else if (returnType == ReturnStringType.UDFTableName)
                    return "ShipVaiMaster";
                else
                    return "";
            }
            if (ReportOf.ToLower().Contains("itemmasterchangelog"))
            {
                if (returnType == ReturnStringType.ResourceFile)
                    return "ResItemMaster";
                else if (returnType == ReturnStringType.ReportTitle)
                    return PrependTitle + "Item Change Log";
                else if (returnType == ReturnStringType.UDFTableName)
                    return "ItemMaster";
                else
                    return "";
            }
            return "";
        }

        private enum ReturnStringType
        {
            ResourceFile,
            ReportTitle,
            UDFTableName,
        }

        public string GetField(string Key, string FileName)
        {
            string KeyVal = string.Empty;
            int ResRead = !string.IsNullOrWhiteSpace(SiteSettingHelper.ResourceRead) ? Convert.ToInt32(SiteSettingHelper.ResourceRead) : (int)ResourceReadType.File;
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
        //private XDocument GetFooterForSignature(XDocument doc1, ReportBuilderDTO objRPTDTO)
        //{
        //    XDocument doc = new XDocument(doc1);

        //    string strHidden = "=IIf(Globals.PageNumber<Globals.TotalPages,true,false)";
        //    string strLeft = "5.12in";

        //    if (objRPTDTO.ReportType == 2)
        //    {
        //        strHidden = "false";
        //    }

        //    string rptWidth = doc.Descendants(ns + "Page").Descendants(ns + "PageWidth").FirstOrDefault().Value;
        //    rptWidth = rptWidth.Replace("in", "");
        //    double dblPageWith = 0;

        //    double.TryParse(rptWidth, out dblPageWith);

        //    if (dblPageWith > 8.5)
        //    {
        //        strLeft = "7.12in";
        //    }


        //    XElement element = new XElement(ns + "PageFooter", new XElement(ns + "Height", "0.80in")
        //                                                        , new XElement(ns + "PrintOnFirstPage", "true")
        //                                                        , new XElement(ns + "PrintOnLastPage", "true"),
        //                       new XElement(ns + "ReportItems",
        //                       new XElement(ns + "Rectangle", new XAttribute("Name", "RectSig"),
        //                       new XElement(ns + "ReportItems",
        //                       new XElement(ns + "Textbox", new XAttribute("Name", "txtSign"), new XElement(ns + "CanGrow", "true"),
        //                       new XElement(ns + "KeepTogether", "true"),
        //                       new XElement(ns + "Paragraphs", new XElement(ns + "Paragraph",
        //                       new XElement(ns + "TextRuns", new XElement(ns + "TextRun", new XElement(ns + "Value", "(Signature)"),
        //                       new XElement(ns + "Style", new XElement(ns + "FontStyle", "Normal")
        //                                                , new XElement(ns + "FontFamily", "Times New Roman")
        //                                                , new XElement(ns + "FontSize", "12pt")
        //                                                , new XElement(ns + "FontWeight", "Normal")
        //                                                , new XElement(ns + "TextDecoration", "None")
        //                                                , new XElement(ns + "Color", "#000000")))),
        //                    new XElement(ns + "Style", new XElement(ns + "TextAlign", "Center")))),
        //                    new XElement(nsrd + "DefaultName", "txtSign"),
        //                    new XElement(ns + "Top", "0.35in"),
        //                    new XElement(ns + "Height", "0.25in"),
        //                    new XElement(ns + "Width", "2.50in"),
        //                    new XElement(ns + "Style", new XElement(ns + "Border", new XElement(ns + "Style", "None")),
        //                     new XElement(ns + "PaddingLeft", "2pt"),
        //                     new XElement(ns + "PaddingRight", "2pt"),
        //                      new XElement(ns + "PaddingTop", "2pt"),
        //                       new XElement(ns + "PaddingBottom", "2pt"))),
        //                       new XElement(ns + "Line", new XAttribute("Name", "lineSign"),
        //                        new XElement(ns + "Top", "0.33in"),
        //                    new XElement(ns + "Left", "0.0040in"),
        //                    new XElement(ns + "Height", "0in"),
        //                    new XElement(ns + "Width", "2.50in"),
        //                    new XElement(ns + "ZIndex", "1"),
        //                     new XElement(ns + "Style", new XElement(ns + "Border", new XElement(ns + "Style", "Solid"))))),
        //                         new XElement(ns + "KeepTogether", "true"),
        //                           new XElement(ns + "Top", "0.070in"),
        //                    new XElement(ns + "Left", "0.2in"),
        //                    new XElement(ns + "Height", "0.60in"),
        //                    new XElement(ns + "Width", "2.50in"),
        //                     new XElement(ns + "Visibility", new XElement(ns + "Hidden", strHidden)),
        //                      new XElement(ns + "Style", new XElement(ns + "Border", new XElement(ns + "Style", "None"))))),
        //                        new XElement(ns + "Style", new XElement(ns + "Border", new XElement(ns + "Style", "None"))));

        //    doc.Descendants(ns + "PageFooter").Remove();
        //    doc.Descendants(ns + "Page").FirstOrDefault().Add(element);

        //    return doc;
        //}
    }
}