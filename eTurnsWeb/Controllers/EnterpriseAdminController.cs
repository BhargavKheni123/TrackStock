using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsMaster.DAL;
using eTurnsWeb.Helper;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace eTurnsWeb.Controllers
{
    public class EnterpriseAdminController : eTurnsControllerBase
    {
        string CtrlName = Convert.ToString(ConfigurationManager.AppSettings["CtrlName"]);
        string ActName = Convert.ToString(ConfigurationManager.AppSettings["ActName"]);
        public ActionResult DbScriptTemplates()
        {
            if (SessionHelper.RoleID == -1)
            {
                return View();
            }
            else
            {
                return RedirectToAction(ActName, CtrlName);
            }
        }

        public ActionResult SQLScriptsAJAX(JQueryDataTableParamModel param)
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
            if (sortColumnName == "0" || sortColumnName == "undefined")
                sortColumnName = "ID";

            if (sortDirection == "asc")
                sortColumnName = sortColumnName + " asc";
            else
                sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;
            EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
            List<EnterPriseSQLScriptsDTO> lstScripts = objEnterpriseMasterDAL.GetDbAllScripts(IsDeleted);
            //lstScripts.RemoveAt(3);
            //lstScripts.RemoveAt(2);
            //lstScripts.RemoveAt(1);
            //lstScripts.RemoveAt(0);
            lstScripts.ForEach(t =>
            {
                t.ScriptText = t.ScriptText.Length > 100 ? t.ScriptText.Substring(0, 100) + "..." : t.ScriptText;
            });
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount
            ,
                aaData = lstScripts
            }, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 0, VaryByParam = "None")]
        public ActionResult SQlScriptEdit(long? ID)
        {

            if (VerifyToAccessPage())
            {
                EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
                EnterPriseSQLScriptsDTO objDTO = new EnterPriseSQLScriptsDTO();
                //ClearMsSession();
                if (ID.HasValue && ID.Value > 0)
                {
                    objDTO = objEnterpriseMasterDAL.GetEnterpriseScriptsMasterBySQLScriptIDPlain(ID.Value);
                }
                if (objDTO == null)
                {
                    objDTO = new EnterPriseSQLScriptsDTO()
                    {
                        CreatedBy = SessionHelper.UserID,
                        CreatedDate = DateTimeUtility.DateTimeNow,
                        IsDeleted = false,
                        IsMasterScript = false,
                        ScriptName = string.Empty,
                        ScriptText = string.Empty,
                        SQLScriptID = 0,
                        UpdatedBy = SessionHelper.UserID,
                        UpdatedDate = DateTimeUtility.DateTimeNow

                    };

                    return PartialView("_SQlScriptDetails", objDTO);
                }
                else
                {
                    return PartialView("_SQlScriptDetails", objDTO);
                }

            }
            else
            {
                return RedirectToAction(ActName, CtrlName);
            }
        }

        public ActionResult SQlScriptCreate(long? ID)
        {
            EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
            EnterPriseSQLScriptsDTO objDTO = new EnterPriseSQLScriptsDTO();
            //ClearMsSession();
            if (ID.HasValue && ID.Value > 0)
            {
                objDTO = objEnterpriseMasterDAL.GetEnterpriseScriptsMasterBySQLScriptIDPlain(ID.Value);
            }
            if (objDTO == null)
            {
                objDTO = new EnterPriseSQLScriptsDTO()
                {
                    CreatedBy = SessionHelper.UserID,
                    CreatedDate = DateTimeUtility.DateTimeNow,
                    IsDeleted = false,
                    IsMasterScript = false,
                    ScriptName = string.Empty,
                    ScriptText = string.Empty,
                    SQLScriptID = 0,
                    UpdatedBy = SessionHelper.UserID,
                    UpdatedDate = DateTimeUtility.DateTimeNow

                };

                return PartialView("_SQlScriptDetails", objDTO);
            }
            else
            {
                return PartialView("_SQlScriptDetails", objDTO);
            }
        }

        public ActionResult EnterpriseList(long? id)
        {
            EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
            EnterPriseSQLScriptsDTO objDTO = new EnterPriseSQLScriptsDTO();
            if (id.HasValue && id.Value > 0)
            {
                objDTO = objEnterpriseMasterDAL.GetEnterpriseScriptsMasterBySQLScriptIDPlain(id.Value);
                return PartialView("_EnterpriceSqlScriptDetails", objDTO);
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public JsonResult RunScriptOnEnterprice(long SqlScriptId, string epids)
        {
            try
            {

                if (VerifyToAccessPage())
                {
                    if (!string.IsNullOrWhiteSpace(epids))
                    {
                        epids = epids.TrimEnd(','); 
                        EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
                        if (objEnterpriseMasterDAL.ExecuitScriptOnEnterprises(SqlScriptId, epids)) 
                        {
                            return Json(new { Message = ResCommon.ScriptSuccessfullyExecuted, Status = "ok" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { Message = ResCommon.SelectOneEnterprise, Status = "fail" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { Message = ResCommon.SelectOneEnterprise, Status = "fail" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { Message = ResCommon.NoPermissionToExecuteScript, Status = "norights" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message, Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SaveSQLScript(EnterPriseSQLScriptsDTO objDTO)
        {
            if (VerifyToAccessPage())
            {
                string message = "";
                string status = "";
                EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
                if (string.IsNullOrWhiteSpace(objDTO.ScriptName))
                {
                    message = string.Format(ResMessage.Required, ResMaterialStaging.StagingName);
                    status = "fail";
                    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string strOK = string.Empty;
                    if (objDTO.SQLScriptID == 0)
                    {

                        long ReturnVal = objEnterpriseMasterDAL.InsertScript(objDTO);
                        if (ReturnVal > 0)
                        {
                            message = ResMessage.SaveMessage;
                            status = "ok";
                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);
                            status = "fail";
                        }

                    }
                    else
                    {
                        long ReturnVal = objEnterpriseMasterDAL.UpdateScripts(objDTO);
                        if (ReturnVal > 0)
                        {
                            message = ResMessage.SaveMessage;
                            status = "ok";
                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);
                            status = "fail";
                        }
                    }
                }
                return Json(new { Message = message, Status = status, UpdatedDTO = objDTO }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Message = ResCommon.NoPermissionToExecuteScript, Status = "norights" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetListEnterpriceWithStatusAJAX(QuickListJQueryDataTableParamModel param)
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
            if (sortColumnName == "0" || sortColumnName == "undefined")
                sortColumnName = "ID";

            if (sortDirection == "asc")
                sortColumnName = sortColumnName + " asc";
            else
                sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;

            long SqlScriptId = 0;
            int TotalRecordCount = 0;
            long.TryParse(Convert.ToString(Request["SqlScriptID"]), out SqlScriptId);
            EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
            List<EnterPriseSQLScriptsDtlDTO> objQLItems = objEnterpriseMasterDAL.GetEnterpriceStatusForScript(SqlScriptId);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = objQLItems
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ExecuteStoredProcedure()
        {
            if (VerifyToAccessPage())
            {
                EnterpriseMasterDAL obj = new EnterpriseMasterDAL();
                List<EnterpriseDTO> lstEnterpriseList = obj.GetAllEnterpriseForExecution();
                //EnterpriseDTO enterpriseDTO = new EnterpriseDTO();
                //enterpriseDTO.Name = DbConnectionHelper.GetETurnsMasterDBName();
                //enterpriseDTO.EnterpriseDBName = DbConnectionHelper.GetETurnsMasterDBName();
                //lstEnterpriseList.Add(enterpriseDTO);
                //enterpriseDTO = new EnterpriseDTO();
                //enterpriseDTO.Name = eTurnsMaster.DAL.MasterDbConnectionHelper.GeteTurnsDBName();
                //enterpriseDTO.EnterpriseDBName = eTurnsMaster.DAL.MasterDbConnectionHelper.GeteTurnsDBName();
                //lstEnterpriseList.Add(enterpriseDTO);
                ViewBag.EnterPriseList = lstEnterpriseList;
                ScriptTemplate scriptTemplate = new ScriptTemplate();
                scriptTemplate.Message = string.Empty;
                scriptTemplate.EnterPriceDB = "0";
                ViewBag.SelectedDBs = null;
                return View("ExecuteStoredProcedure", scriptTemplate);
            }
            else
            {
                return RedirectToAction(ActName, CtrlName);
            }
        }

        public ActionResult ScriptTemplates()
        {
            if (VerifyToAccessPage())
            {
                EnterpriseMasterDAL obj = new EnterpriseMasterDAL();
                List<EnterpriseDTO> lstAssetCategory = obj.GetAllEnterprisesPlain();
                EnterpriseDTO enterpriseDTO = new EnterpriseDTO();
                enterpriseDTO.Name = DbConnectionHelper.GetETurnsMasterDBName();
                enterpriseDTO.EnterpriseDBName = DbConnectionHelper.GetETurnsMasterDBName();
                lstAssetCategory.Add(enterpriseDTO);
                ViewBag.EnterPriseList = lstAssetCategory;
                ScriptTemplate scriptTemplate = new ScriptTemplate();
                scriptTemplate.Message = string.Empty;
                scriptTemplate.EnterPriceDB = "0";
                return View("ScriptTemplates", scriptTemplate);
            }
            else
            {
                return RedirectToAction(ActName, CtrlName);
            }
        }
        [HttpPost]
        public ActionResult ScriptTemplates(ScriptTemplate scrpt)
        {
            if (VerifyToAccessPage())
            {
                EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
                if (scrpt.IsUpdateDeleteQuery)
                {
                    scrpt.Message = objEnterpriseMasterDAL.ExecuteNonQueryScript(scrpt.Script, scrpt.EnterPriceDB, SessionHelper.UserID);
                }
                else
                {
                    scrpt.ColumnList = null;
                    List<object> columList = new List<object>();
                    scrpt.Message = string.Empty;
                    string msg = string.Empty;
                    ViewBag.ResultList = objEnterpriseMasterDAL.ExecuteReaderScript(scrpt.Script, scrpt.EnterPriceDB, SessionHelper.UserID, ref columList, ref msg);
                    scrpt.Message = msg;
                    scrpt.ColumnList = columList;

                    //ViewBag.ResultList = objEnterpriseMasterDAL.ExecuteReaderScript(scrpt.Script, scrpt.EnterPriceDB, SessionHelper.UserID, ref columList);
                    //ViewBag.ColumnList = columList;
                    if (string.IsNullOrEmpty(scrpt.Message))
                    {
                        scrpt.Message = string.Format(ResCommon.RecordsDisplayed, ((List<object>)ViewBag.ResultList).Count());
                    }
                }

                List<EnterpriseDTO> lstAssetCategory = objEnterpriseMasterDAL.GetAllEnterprisesPlain();
                EnterpriseDTO enterpriseDTO = new EnterpriseDTO();
                enterpriseDTO.Name = DbConnectionHelper.GetETurnsMasterDBName();
                enterpriseDTO.EnterpriseDBName = DbConnectionHelper.GetETurnsMasterDBName();
                lstAssetCategory.Add(enterpriseDTO);
                ViewBag.EnterPriseList = lstAssetCategory;
                return View("ScriptTemplates", scrpt);
            }
            else
            {
                return RedirectToAction(ActName, CtrlName);
            }
        }
        public JsonResult getTable(string currentDB)
        {
            Dictionary<string, string> retData = new Dictionary<string, string>();

            EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
            retData = objEnterpriseMasterDAL.GetTablesList(currentDB);
            return Json(new { DDData = retData }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetColumn(string TableName, string currentDB)
        {
            Dictionary<string, string> retData = new Dictionary<string, string>();

            EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
            retData = objEnterpriseMasterDAL.GetColumnList(currentDB, TableName);
            return Json(new { DDData = retData }, JsonRequestBehavior.AllowGet);

        }

        public string DeleteScript(string ids)
        {
            try
            {
                if (VerifyToAccessPage())
                {
                    EnterpriseMasterDAL obj = new EnterpriseMasterDAL();
                    obj.DeleteEnterpriseScriptsMaster(ids, SessionHelper.UserID);
                    return ResCommon.Ok;
                }
                return ResCommon.NotOk;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ExecuteStoredProcedure(ScriptTemplate ScriptTemplete, HttpPostedFileBase SQLFile)
        {
            if (VerifyToAccessPage())
            {
                string Message = string.Empty;
                string script = string.Empty;
                var readablefilename = string.Empty;
                string CurrentFilename = DateTime.Now.Ticks.ToString();
                try
                {
                    string[] DBNameList = ScriptTemplete.SelectedDB.Split(',');
                    if (!Directory.Exists(Server.MapPath("/TemporaryExecuteScript/" + DateTime.Now.ToShortDateString().Replace("/", "_"))))
                    {
                        Directory.CreateDirectory(Server.MapPath("/TemporaryExecuteScript/" + DateTime.Now.ToShortDateString().Replace("/", "_")));
                    }
                    DirectoryInfo dirinfo = new DirectoryInfo(Server.MapPath("/TemporaryExecuteScript/" + DateTime.Now.ToShortDateString().Replace("/", "_")));
                    FileInfo[] files = dirinfo.GetFiles().OrderBy(p => p.CreationTime).ToArray();
                    //foreach (System.IO.FileInfo f in files)
                    //{
                    //    f.Delete();
                    //}
                    //Copy file dabaseName wise in temp folder if user upload file
                    if (SQLFile != null && SQLFile.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(SQLFile.FileName);
                        CurrentFilename = CurrentFilename + fileName;
                        //foreach (string dbName in DBNameList)
                        //{
                        //    if (!string.IsNullOrWhiteSpace(dbName))
                        //    {
                        string path = Server.MapPath("/TemporaryExecuteScript/" + DateTime.Now.ToShortDateString().Replace("/", "_")) + "\\" + CurrentFilename;
                        //if (System.IO.File.Exists(path))
                        //{
                        //    System.IO.File.Delete(path);
                        //}
                        SQLFile.SaveAs(path);
                        readablefilename = SQLFile.FileName.ToString();
                        //    }
                        //}

                    }
                    //Make file dabaseName wise in temp folder datewise if user enter script
                    if (!string.IsNullOrWhiteSpace(ScriptTemplete.Script))
                    {

                        {
                            //if (!string.IsNullOrWhiteSpace(dbName))
                            {
                                string path = Server.MapPath("/TemporaryExecuteScript/" + DateTime.Now.ToShortDateString().Replace("/", "_")) + "\\" + CurrentFilename + ".sql";
                                CurrentFilename = CurrentFilename + ".sql";
                                //if (System.IO.File.Exists(path))
                                //{
                                //    System.IO.File.Delete(path);

                                //}
                                using (FileStream fs = System.IO.File.Create(path))
                                {
                                    Byte[] info = new UTF8Encoding(true).GetBytes(ScriptTemplete.Script);

                                    fs.Write(info, 0, info.Length);
                                }
                            }
                        }
                        readablefilename = "Script.sql";
                    }

                    //var startInfo = new ProcessStartInfo();
                    //startInfo.FileName = "SQLCMD.EXE";
                    //string user = ConfigurationManager.AppSettings["DbUserName"];
                    //string password = ConfigurationManager.AppSettings["DbPassword"];
                    //string server = ConfigurationManager.AppSettings["DBserverName"];

                    dirinfo = new DirectoryInfo(Server.MapPath("/TemporaryExecuteScript/" + DateTime.Now.ToShortDateString().Replace("/", "_")));
                    files = dirinfo.GetFiles().Where(f => f.Name == CurrentFilename).OrderBy(p => p.CreationTime).ToArray();
                    Message = string.Empty;
                    Server server = null;
                    ServerConnection con = null;
                    Microsoft.Data.SqlClient.SqlConnection conn = null;
                    CommonMasterDAL objCommonMasterDAL = new CommonMasterDAL();
                    foreach (string Getfile in ScriptTemplete.SelectedDB.Split(','))
                    {
                        try
                        {
                            if (!string.IsNullOrWhiteSpace(Getfile))
                            {
                                //    startInfo.Arguments = String.Format("-S {0} -d {1}, -U {2} -P {3} -i {4}",
                                //                                        server,
                                //                                        file.Name.Replace(".sql",""),
                                //                                        user,
                                //                                        password,
                                //                                        file);
                                //    Process.Start(startInfo);
                                //}
                                //string sqlConnectionString = ConfigurationManager.ConnectionStrings["eTurnsMasterDbConnection"].ConnectionString.ToString();
                                string sqlConnectionString = DbConnectionHelper.GeteTurnsMasterSQLConnectionString(DbConnectionHelper.GetETurnsMasterDBName(), DbConnectionType.GeneralReadWrite.ToString("F"));
                                script = System.IO.File.ReadAllText(Server.MapPath("/TemporaryExecuteScript/" + DateTime.Now.ToShortDateString().Replace("/", "_") + "/" + CurrentFilename.ToString()));
                                //  script = "use[" + Getfile.Name.ToString().Replace(".sql", "") + "]\r\nGo\r\n" + script;
                                conn = new Microsoft.Data.SqlClient.SqlConnection(sqlConnectionString);
                                con = new ServerConnection(conn);
                                server = new Server(con);
                                server.ConnectionContext.StatementTimeout = int.MaxValue;

                                string[] singleCommand = Regex.Split(script, "^GO", RegexOptions.Multiline);
                                StringCollection scl = new StringCollection();
                                foreach (string t in singleCommand)
                                {
                                    if (t.Trim().Length > 0) scl.Add("use[" + Getfile.ToString().Replace(".sql", "") + "]\r\nGo\r\n" + t.Trim());
                                }
                                try
                                {
                                    int[] result = server.ConnectionContext.ExecuteNonQuery(scl);
                                    int i = 0;
                                    foreach (int res in result)
                                    {
                                        if (res != -1)
                                        {
                                            Message += "\r\n" + string.Format(ResCommon.ScriptSuccess, Getfile.ToString().Replace(".sql", "")) + " ";
                                        }
                                        else
                                        {
                                            Message += "\r\n" + string.Format(ResCommon.ScriptSuccess, Getfile.ToString().Replace(".sql", ""));
                                        }
                                        i++;
                                    }

                                    objCommonMasterDAL.InsertScriptsExecution(SessionHelper.UserID, Getfile.ToString().Replace(".sql", ""), script);

                                }
                                catch (ExecutionFailureException ex)
                                {
                                    Message += string.Format(ResCommon.FailedScript, Getfile.ToString().Replace(".sql", ""), ex.GetBaseException().Message);
                                }
                                catch (Exception ex)
                                {
                                    Message += string.Format(ResCommon.FailedScript, Getfile.ToString().Replace(".sql", ""), ex.Message.ToString());
                                }
                                finally
                                {
                                    con.Disconnect();
                                    conn.Close();
                                    server = null;
                                    conn = null;
                                    scl = null;
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            Message += string.Format(ResCommon.FailedScript, Getfile.ToString().Replace(".sql", ""), ex.Message.ToString());
                        }

                    }
                    dirinfo = null;
                    files = null;
                }
                catch (Exception ex)
                {
                    Message = ex.Message.ToString();
                }

                ScriptTemplete.Message = Message;


                List<EnterpriseDTO> lstEnterpriseList = new EnterpriseMasterDAL().GetAllEnterpriseForExecution();
                ViewBag.EnterPriseList = lstEnterpriseList;
                ViewBag.SelectedDBs = ScriptTemplete.SelectedDB;
                return View("ExecuteStoredProcedure", ScriptTemplete);
            }
            else
            {
                return RedirectToAction(ActName, CtrlName);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ExecuteStoredProcedureNew(ScriptTemplate ScriptTemplete, HttpPostedFileBase SQLFile)
        {
            if (VerifyToAccessPage())
            {
                string Message = string.Empty;
                string script = string.Empty;
                var readablefilename = string.Empty;
                string CurrentFilename = DateTime.Now.Ticks.ToString();
                try
                {
                    string[] DBNameList = ScriptTemplete.SelectedDB.Split(',');
                    if (!Directory.Exists(Server.MapPath("/TemporaryExecuteScript/" + DateTime.Now.ToShortDateString().Replace("/", "_"))))
                    {
                        Directory.CreateDirectory(Server.MapPath("/TemporaryExecuteScript/" + DateTime.Now.ToShortDateString().Replace("/", "_")));
                    }
                    DirectoryInfo dirinfo = new DirectoryInfo(Server.MapPath("/TemporaryExecuteScript/" + DateTime.Now.ToShortDateString().Replace("/", "_")));
                    FileInfo[] files = dirinfo.GetFiles().OrderBy(p => p.CreationTime).ToArray();
                    //foreach (System.IO.FileInfo f in files)
                    //{
                    //    f.Delete();
                    //}
                    //Copy file dabaseName wise in temp folder if user upload file
                    if (SQLFile != null && SQLFile.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(SQLFile.FileName);
                        CurrentFilename = CurrentFilename + fileName;
                        //foreach (string dbName in DBNameList)
                        //{
                        //    if (!string.IsNullOrWhiteSpace(dbName))
                        //    {
                        string path = Server.MapPath("/TemporaryExecuteScript/" + DateTime.Now.ToShortDateString().Replace("/", "_")) + "\\" + CurrentFilename;
                        //if (System.IO.File.Exists(path))
                        //{
                        //    System.IO.File.Delete(path);
                        //}
                        SQLFile.SaveAs(path);
                        readablefilename = SQLFile.FileName.ToString();
                        //    }
                        //}

                    }
                    //Make file dabaseName wise in temp folder datewise if user enter script
                    if (!string.IsNullOrWhiteSpace(ScriptTemplete.Script))
                    {

                        {
                            //if (!string.IsNullOrWhiteSpace(dbName))
                            {
                                string path = Server.MapPath("/TemporaryExecuteScript/" + DateTime.Now.ToShortDateString().Replace("/", "_")) + "\\" + CurrentFilename + ".sql";
                                CurrentFilename = CurrentFilename + ".sql";
                                //if (System.IO.File.Exists(path))
                                //{
                                //    System.IO.File.Delete(path);

                                //}
                                using (FileStream fs = System.IO.File.Create(path))
                                {
                                    Byte[] info = new UTF8Encoding(true).GetBytes(ScriptTemplete.Script);

                                    fs.Write(info, 0, info.Length);
                                }
                            }
                        }
                        readablefilename = "Script.sql";
                    }

                    using (FileStream fs = System.IO.File.Create(Server.MapPath("/TemporaryExecuteScript/" + DateTime.Now.ToShortDateString().Replace("/", "_")) + "\\" + "allDblist.txt"))
                    {
                        Byte[] info = new UTF8Encoding(true).GetBytes(ScriptTemplete.SelectedDB);

                        fs.Write(info, 0, info.Length);
                    }



                }
                catch
                {

                }
                List<EnterpriseDTO> lstEnterpriseList = new EnterpriseMasterDAL().GetAllEnterpriseForExecution();
                ViewBag.EnterPriseList = lstEnterpriseList;
                ViewBag.SelectedDBs = ScriptTemplete.SelectedDB;
                return View("ExecuteStoredProcedure", ScriptTemplete);
            }
            else
            {
                return RedirectToAction(ActName, CtrlName);
            }
        }

        public ActionResult BuildSetup()
        {
            if (VerifyToAccessPage())
            {

                // string XMLFilePath = ConfigurationManager.AppSettings["XMLFilePath"] != null ? ConfigurationManager.AppSettings["XMLFilePath"] : "";
                //XElement Settinfile = XElement.Load(Server.MapPath("/SiteSettings.xml"));
                string XMLFilePath = SiteSettingHelper.XMLFilePath; // Settinfile.Element("XMLFilePath").Value;
                eTrunsPub objeTrunsPub = JsonConvert.DeserializeObject<eTrunsPub>(System.IO.File.ReadAllText(Convert.ToString(XMLFilePath)));
                return View(objeTrunsPub);
            }
            else
            {
                return RedirectToAction(ActName, CtrlName);
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult BuildSetup(eTrunsPub objeTrunsPubNew)
        {
            //JavaScriptSerializer s = new JavaScriptSerializer();

            //s.MaxJsonLength = int.MaxValue;
            //List<Project> objeTrunsPub = s.Deserialize<List<Project>>(para);
            try
            {
                //string XMLFilePath = ConfigurationManager.AppSettings["XMLFilePath"] != null ? ConfigurationManager.AppSettings["XMLFilePath"] : "";
                //XElement Settinfile = XElement.Load(Server.MapPath("/SiteSettings.xml"));
                string XMLFilePath = SiteSettingHelper.XMLFilePath; //Settinfile.Element("XMLFilePath").Value;
                if (System.IO.File.Exists(XMLFilePath))
                {
                    System.IO.File.Delete(XMLFilePath);
                }
                //eTrunsPub objeTrunsPubNew = new eTrunsPub();
                //objeTrunsPubNew.Projects = objeTrunsPub;
                objeTrunsPubNew.Projects.ForEach(t =>
                {
                    if (t.Files == null)
                    {
                        t.Files = new List<FileOrFolderInfo>();
                    }
                    if (t.Folders == null)
                    {
                        t.Folders = new List<FileOrFolderInfo>();
                    }

                });
                System.IO.File.WriteAllText(XMLFilePath, JsonConvert.SerializeObject(objeTrunsPubNew));
                return View("BuildSetup", objeTrunsPubNew);
            }
            catch
            {
                return View("BuildSetup", objeTrunsPubNew);
            }
        }
        public bool VerifyToAccessPage()
        {
            try
            {
                //System.Xml.Linq.XElement Settinfile = System.Xml.Linq.XElement.Load(Server.MapPath("/SiteSettings.xml"));
                string ReleaseNumber = SiteSettingHelper.ReleaseNumber; // Settinfile.Element("ReleaseNumber").Value;

                //string AccessQryUserNames = Settinfile.Element("AccessQryUserNames") != null ? Settinfile.Element("AccessQryUserNames").Value : string.Empty;
                string AccessQryUserNames = SiteSettingHelper.AccessQryUserNames != string.Empty ? SiteSettingHelper.AccessQryUserNames : string.Empty;
                //string AccessQryRoleIds = Settinfile.Element("AccessQryRoleIds") != null ? Settinfile.Element("AccessQryRoleIds").Value : string.Empty;
                string AccessQryRoleIds = SiteSettingHelper.AccessQryRoleIds != string.Empty ? SiteSettingHelper.AccessQryRoleIds : string.Empty;

                bool ShowMenutoThisUser = false;
                if (!string.IsNullOrWhiteSpace(AccessQryUserNames))
                {
                    string[] UserNameList = AccessQryUserNames.Split('$');
                    for (int i = 0; i < UserNameList.Length; i++)
                    {
                        if (UserNameList[i] == eTurnsWeb.Helper.SessionHelper.UserName)
                        {
                            ShowMenutoThisUser = true;
                            break;
                        }
                    }
                }
                if (!ShowMenutoThisUser)
                {
                    if (!string.IsNullOrWhiteSpace(AccessQryRoleIds))
                    {
                        string[] RoleIDS = AccessQryRoleIds.Split('$');
                        for (int i = 0; i < RoleIDS.Length; i++)
                        {
                            if (Convert.ToInt64(RoleIDS[i]) == eTurnsWeb.Helper.SessionHelper.RoleID)
                            {
                                ShowMenutoThisUser = true;
                                break;
                            }
                        }
                    }
                }
                return ShowMenutoThisUser;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
