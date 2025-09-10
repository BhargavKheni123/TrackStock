using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace eTurnsMaster.DAL
{
    public partial class CommonMasterDAL : eTurnsMasterBaseDAL
    {
        /// <summary>
        /// Check Duplicate 
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="ActionMode"></param>
        /// <param name="ID"></param>
        /// <param name="TableName"></param>
        /// <param name="FieldName"></param>
        /// <param name="RoomID"></param>
        /// <param name="EnterpriseID"></param>
        /// <returns></returns>
        public string DuplicateCheck(string Name, string ActionMode, Int64 ID, string TableName, string FieldName)
        {
            Name = Name.Replace("'", "''");
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                
                    string Msg = "";
                    string WhereCond = "";

                    if (ActionMode == "add")
                        WhereCond = " " + FieldName + " = '" + Name + "' and IsDeleted = 0 and IsArchived = 0 ";
                    else
                        WhereCond = " " + FieldName + " = '" + Name + "' and ID = " + ID + " ";

                    // var data = context.ChkDuplicate(TableName, "ID", WhereCond);
                    var params1 = new SqlParameter[] { new SqlParameter("@TableName", TableName), new SqlParameter("@PkID", "ID"), new SqlParameter("@WhereCond", WhereCond) };
                    var data = (from u in context.Database.SqlQuery<int?>("exec [ChkDuplicate] @TableName,@PkID,@WhereCond", params1) select u).ToList(); //A.IsDeleted!=1 AND A.IsArchived != 1 AND 

                    foreach (var item in data)
                    {
                        if (item.Value == 0 && ActionMode == "add")
                            Msg = "ok";
                        else if (item.Value == 0 && ActionMode == "edit")
                        {
                            WhereCond = " " + FieldName + " = '" + Name + "' and IsDeleted = 0 and IsArchived = 0 ";
                            //data = context.ChkDuplicate(TableName, "ID", WhereCond);
                            params1 = new SqlParameter[] { new SqlParameter("@TableName", TableName), new SqlParameter("@PkID", "ID"), new SqlParameter("@WhereCond", WhereCond) };
                            data = (from u in context.Database.SqlQuery<int?>("exec [ChkDuplicate] @TableName,@PkID,@WhereCond", params1) select u).ToList(); //A.IsDeleted!=1 AND A.IsArchived != 1 AND 

                            foreach (var item1 in data)
                            {
                                if (item1.Value == 0)
                                    Msg = "ok";
                                else
                                    Msg = "duplicate";
                            }
                        }
                        else
                        {
                            if (ActionMode == "edit" && item.Value == 1)
                                Msg = "ok";
                            else
                                Msg = "duplicate";
                        }
                    }
                    return Msg;
                
            }
        }

        public string EnterPriseDuplicateCheck(long ID, string EnterPriseName)
        {
            string msg = ""; 
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                List<SqlParameter> lstPara = new List<SqlParameter>() {
                    new SqlParameter ("@ID",ID),
                    new SqlParameter ("@EnterpriseName",EnterPriseName),
                };

                msg = context.Database.SqlQuery<string>("EXEC CheckEnterpriseNameDuplicate @ID,@EnterpriseName", lstPara.ToArray()).FirstOrDefault<string>();
            }
            return msg;
        }

        public string UserDuplicateCheckUserName(long ID, string UserName)
        {
            string msg = "";
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                List<SqlParameter> lstPara = new List<SqlParameter>() {
                    new SqlParameter ("@ID",ID),
                    new SqlParameter ("@UserName",UserName),
                };

                msg = context.Database.SqlQuery<string>("EXEC upsCheckUserNameDuplicate @ID,@UserName", lstPara.ToArray())
                    .FirstOrDefault<string>();
            }
            return msg;
        }

        public string RoleDuplicateCheck(long ID, string Rolename)
        {
            string msg = "";
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                List<SqlParameter> lstPara = new List<SqlParameter>() {
                    new SqlParameter ("@ID",ID),
                    new SqlParameter ("@RoleName",Rolename),
                };

                msg = context.Database.SqlQuery<string>("EXEC CheckRoleNameDuplicate @ID,@RoleName", lstPara.ToArray())
                    .FirstOrDefault<string>();
            }
            return msg;
        }

        private List<NarrowSearchDTO> GetEnterpriseListNarrowSearch(bool IsArchived, bool IsDeleted, string NarrowSearchKey)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@IsArchived", IsArchived),
                                                    new SqlParameter("@IsDeleted", IsDeleted),
                                                    new SqlParameter("@NarrowSearchKey", NarrowSearchKey ?? string.Empty)
                                                };
                return context.Database.SqlQuery<NarrowSearchDTO>("exec [GetEnterpriseListNarrowSearch] @IsArchived,@IsDeleted,@NarrowSearchKey", params1).ToList();
            }
        }

        private List<NarrowSearchDTO> GetRoleListNarrowSearch(int UserType, long? EnterPriseId, long? CompanyId,bool IsArchived, bool IsDeleted, string NarrowSearchKey)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                List<SqlParameter> lstPara = new List<SqlParameter>();
                lstPara.Add(new SqlParameter("@UserType", UserType));

                if (EnterPriseId.GetValueOrDefault(0) > 0)
                {
                    lstPara.Add(new SqlParameter("@EnterpriseId", EnterPriseId));
                }
                else
                {
                    lstPara.Add(new SqlParameter("@EnterpriseId", DBNull.Value));
                }

                if (CompanyId.GetValueOrDefault(0) > 0)
                {
                    lstPara.Add(new SqlParameter("@CompanyId", CompanyId));
                }
                else
                {
                    lstPara.Add(new SqlParameter("@CompanyId", DBNull.Value));
                }

                lstPara.Add(new SqlParameter("@IsArchived", IsArchived));
                lstPara.Add(new SqlParameter("@IsDeleted", IsDeleted));
                lstPara.Add(new SqlParameter("@NarrowSearchKey", NarrowSearchKey ?? string.Empty));

                return context.Database.SqlQuery<NarrowSearchDTO>("exec [GetRoleListNarrowSearch] @UserType,@EnterpriseId,@CompanyId,@IsArchived,@IsDeleted,@NarrowSearchKey", lstPara.ToArray()).ToList();
            }
        }
        public Dictionary<string, int> GetNarrowDDData(string TableName, string TextFieldName, bool IsArchived, bool IsDeleted, int LoggedInUserType, long? EnterpriseId, long? CompanyId, long? RoomId)
        {
            Dictionary<string, int> ColUDFData = new Dictionary<string, int>();
            switch (TableName)
            {
                case "EnterpriseMaster":
                    return GetNarrowDDData_EnterpriseMaster(TextFieldName, IsArchived, IsDeleted);
                case "RoleMaster":
                    return GetNarrowDDData_RoleMaster(LoggedInUserType, EnterpriseId, CompanyId,TextFieldName, IsArchived, IsDeleted);
            }
            return ColUDFData;
        }

        private Dictionary<string, int> GetNarrowDDData_EnterpriseMaster(string TextFieldName, bool IsArchived, bool IsDeleted)
        {
            return GetEnterpriseListNarrowSearch(IsArchived, IsDeleted, TextFieldName).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
        }

        private Dictionary<string, int> GetNarrowDDData_RoleMaster(int UserType, long? EnterpriseId, long? CompanyId,string TextFieldName, bool IsArchived, bool IsDeleted)
        {
            return GetRoleListNarrowSearch(UserType, EnterpriseId, CompanyId,IsArchived, IsDeleted, TextFieldName).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
        }

        public IEnumerable<object> GetPagedEnterpriseMasterHistory(int StartRowIndex, int MaxRows, out int TotalCount , long ID)
        {
            TotalCount = 0;
            var history = new List<EnterpriseDTO>();

            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                            new SqlParameter("@StartRowIndex", StartRowIndex),
                                            new SqlParameter("@MaxRows", MaxRows),
                                            new SqlParameter("@ID", ID)
                                        };
                history = context.Database.SqlQuery<EnterpriseDTO>("EXEC dbo.GetPagedEnterpriseMasterHistory @StartRowIndex,@MaxRows,@ID ", params1).ToList();

                if (history != null && history.Count() > 0)
                {
                    TotalCount = history.First().TotalRecords ?? 0;
                }
                return history;               
            }
        }
        public List<EnterpriseDTO> GetEnterpriseHistoryByIDsNormal(string IDs)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@IDs", IDs ?? (object)DBNull.Value) };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<EnterpriseDTO>("exec [GetEnterpriseHistoryByIDsNormal] @IDs", params1).ToList();
            }
        }

        public string DuplicateUFDOptionCheck(string Name, string ActionMode, long ID, string TableName, string FieldName, long _UDFID)
        {
            Name = Name.Replace("'", "''");
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                string Msg = "";
                string WhereCond = "";

                    if (ActionMode == "add")
                        WhereCond = " " + FieldName + " = '" + Name + "' and IsDeleted = 0 and UDFID =" + _UDFID + "";
                    else
                        WhereCond = " " + FieldName + " = '" + Name + "' and ID = " + ID + " and UDFID =" + _UDFID + "";

                    //var data = context.ChkDuplicate(TableName, "ID", WhereCond).ToList();
                    var params1 = new SqlParameter[] { new SqlParameter("@TableName", TableName), new SqlParameter("@PkID", "ID"), new SqlParameter("@WhereCond", WhereCond) };
                    var data = (from u in context.Database.SqlQuery<int?>("exec [ChkDuplicate] @TableName,@PkID,@WhereCond", params1) select u).ToList(); //A.IsDeleted!=1 AND A.IsArchived != 1 AND 
                    
                    foreach (var item in data)
                    {
                        if (item.Value == 0 && ActionMode == "add")
                            Msg = "ok";
                        else if (item.Value == 0 && ActionMode == "edit")
                        {
                            WhereCond = " " + FieldName + " = '" + Name + "' and IsDeleted = 0 and UDFID =" + _UDFID + "";
                            // data = context.ChkDuplicate(TableName, "ID", WhereCond).ToList();
                            params1 = new SqlParameter[] { new SqlParameter("@TableName", TableName), new SqlParameter("@PkID", "ID"), new SqlParameter("@WhereCond", WhereCond) };
                            data = (from u in context.Database.SqlQuery<int?>("exec [ChkDuplicate] @TableName,@PkID,@WhereCond", params1) select u).ToList(); //A.IsDeleted!=1 AND A.IsArchived != 1 AND 

                            foreach (var item1 in data)
                            {
                                if (item1.Value == 0)
                                    Msg = "ok";
                                else
                                    Msg = "duplicate";
                            }
                        }
                        else
                        {
                            if (ActionMode == "edit" && item.Value == 1)
                                Msg = "ok";
                            else
                                Msg = "duplicate";
                        }
                    }
                    return Msg;
            }
        }

        public IEnumerable<eTurns.DTO.Resources.ResourceLanguageDTO> GetCachedResourceLanguageData(long CompanyID)
        {
            IEnumerable<eTurns.DTO.Resources.ResourceLanguageDTO> ObjCache = CacheHelper<IEnumerable<eTurns.DTO.Resources.ResourceLanguageDTO>>.GetCacheItem("Cached_MasterLanguages_" + CompanyID);

            if (ObjCache == null || ObjCache.Count() <= 0)
            {
                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<eTurns.DTO.Resources.ResourceLanguageDTO> obj = context.Database.SqlQuery<eTurns.DTO.Resources.ResourceLanguageDTO>("exec [GetLanguages]").ToList();
                    ObjCache = CacheHelper<IEnumerable<eTurns.DTO.Resources.ResourceLanguageDTO>>.AddCacheItem("Cached_MasterLanguages_" + CompanyID, obj);
                }
            }

            return ObjCache;
        }

        public void SaveNotificationError(ReportSchedulerError objReportSchedulerError)
        {
            string MasterDbConnectionString = MasterDbConnectionHelper.GeteTurnsMasterSQLConnectionString(MasterDbConnectionHelper.GetETurnsMasterDBName(), MasterDbConnectionType.GeneralReadWrite.ToString("F"));
            SqlConnection EturnsMasterConnection = new SqlConnection(MasterDbConnectionString);
            DataSet dsCart = new DataSet();
            int retval = SqlHelper.ExecuteNonQuery(EturnsMasterConnection, "INSERTReportSchedulerError", objReportSchedulerError.NotificationID, objReportSchedulerError.Exception, objReportSchedulerError.ScheduleFor, objReportSchedulerError.RoomID, objReportSchedulerError.CompanyID, objReportSchedulerError.EnterpriseID, objReportSchedulerError.UserID);
        }

        public SchedulerErrorLog SaveSchedulerErrorLog(SchedulerErrorLog objSchedulerErrorLog)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                SchedulerErrorLog objToInsert = new SchedulerErrorLog();
                objToInsert.CompanyID = objSchedulerErrorLog.CompanyID;
                objToInsert.Created = DateTime.UtcNow;
                objToInsert.EnterpriseID = objSchedulerErrorLog.EnterpriseID;
                objToInsert.ExceptionDetails = objSchedulerErrorLog.ExceptionDetails;
                objToInsert.MailSentTo = objSchedulerErrorLog.MailSentTo;
                objToInsert.RoomID = objSchedulerErrorLog.RoomID;
                objToInsert.ScheduleFor = objSchedulerErrorLog.ScheduleFor;
                objToInsert.SchedulerID = objSchedulerErrorLog.SchedulerID;
                context.SchedulerErrorLogs.Add(objToInsert);
                context.SaveChanges();
                objSchedulerErrorLog.ID = objToInsert.ID;
                objSchedulerErrorLog.Created = objToInsert.Created;
                return objSchedulerErrorLog;
            }
        }

        public void DeleteSchedulerErrorLog(long ScheduleID, int ScheduleFor)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ScheduleID", ScheduleID) , new SqlParameter("@ScheduleFor", ScheduleFor) };
                context.Database.ExecuteSqlCommand("exec [DeleteSchedulerErrorLog] @ScheduleID, @ScheduleFor", params1);
            }
        }

        public int GetSchedulerErrorLogCountByIDAndScheduleFor(long ScheduleID, int ScheduleFor)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ScheduleID", ScheduleID), new SqlParameter("@ScheduleFor", ScheduleFor) };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<int>("exec [GetSchedulerErrorLogCountByIDAndScheduleFor] @ScheduleID, @ScheduleFor", params1).FirstOrDefault();
            }
        }
        public List<SchedulerErrorLog> getLogByScheduleID(long ScheduleID, int ScheduleFor)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.SchedulerErrorLogs.Where(t => t.ScheduleFor == ScheduleFor && t.SchedulerID == ScheduleID).ToList();
            }
        }
        public IEnumerable<AlleTurnsActionMethodsDTO> GetAlleTurnsActionMethodsData()
        {
            //Get Cached-Media
            IEnumerable<AlleTurnsActionMethodsDTO> ObjCache;

            //Get Cached-Media
            ObjCache = CacheHelper<IEnumerable<AlleTurnsActionMethodsDTO>>.GetCacheItem("Cached_AlleTurnsActionMethods");

            if (ObjCache == null)
            {
                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<AlleTurnsActionMethodsDTO> obj = (from u in context.Database.SqlQuery<AlleTurnsActionMethodsDTO>(@"exec usp_GetAlleTurnsActionMethods")
                                                                  select new AlleTurnsActionMethodsDTO
                                                                  {
                                                                      ID = u.ID,
                                                                      PermissionModuleID = u.PermissionModuleID,
                                                                      ActionMethod = u.ActionMethod,
                                                                      Controller = u.Controller,
                                                                      Module = u.Module,
                                                                      Attributes = u.Attributes,
                                                                      IsView = u.IsView.GetValueOrDefault(false),
                                                                      IsChecked = u.IsChecked.GetValueOrDefault(false),
                                                                      IsInsert = u.IsInsert.GetValueOrDefault(false),
                                                                      IsUpdate = u.IsUpdate.GetValueOrDefault(false),
                                                                      IsDelete = u.IsDelete.GetValueOrDefault(false),
                                                                      ShowDeleted = u.ShowDeleted.GetValueOrDefault(false),
                                                                      ShowArchived = u.ShowArchived.GetValueOrDefault(false),
                                                                      ShowUDF = u.ShowUDF.GetValueOrDefault(false),
                                                                      ShowChangeLog = u.ShowChangeLog.GetValueOrDefault(false)

                                                                  }).AsParallel().ToList();
                    ObjCache = CacheHelper<IEnumerable<AlleTurnsActionMethodsDTO>>.AddCacheItem("Cached_AlleTurnsActionMethods", obj);
                }
            }


            return ObjCache;
        }


        public IEnumerable<AllIntegrateAPIActionMethodsDTO> GetAllIntegrateAPIActionMethodsData()
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<AllIntegrateAPIActionMethodsDTO> obj = (from u in context.Database.SqlQuery<AllIntegrateAPIActionMethodsDTO>(@"exec usp_GetAllIntegrateAPIActionMethods")
                                                                    select new AllIntegrateAPIActionMethodsDTO
                                                                    {
                                                                        ID = u.ID,
                                                                        PermissionModuleID = u.PermissionModuleID,
                                                                        ActionMethod = u.ActionMethod,
                                                                        Controller = u.Controller,
                                                                        Module = u.Module,
                                                                        Attributes = u.Attributes,
                                                                        IsView = u.IsView.GetValueOrDefault(false),
                                                                        IsChecked = u.IsChecked.GetValueOrDefault(false),
                                                                        IsInsert = u.IsInsert.GetValueOrDefault(false),
                                                                        IsUpdate = u.IsUpdate.GetValueOrDefault(false),
                                                                        IsDelete = u.IsDelete.GetValueOrDefault(false),
                                                                        ShowDeleted = u.ShowDeleted.GetValueOrDefault(false),
                                                                        ShowArchived = u.ShowArchived.GetValueOrDefault(false),
                                                                        ShowUDF = u.ShowUDF.GetValueOrDefault(false),
                                                                        ShowChangeLog = u.ShowChangeLog.GetValueOrDefault(false)

                                                                    }).AsParallel().ToList();
                return obj;
            }




        }

        public void SaveeTurnsAuthorizationError(long EnterpriceID, long CompanyID, long RoomID, long UserID, string UserName, string ActionName, string ControllerName, string ErrorPurposeFor, string ErrorException)
        {
            try
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ErrorPurposeFor", ErrorPurposeFor ?? (object)DBNull.Value), new SqlParameter("@ActionMethod", ActionName ?? (object)DBNull.Value), new SqlParameter("@ControllerName", ControllerName ?? (object)DBNull.Value), new SqlParameter("@UserName", UserName ?? (object)DBNull.Value), new SqlParameter("@UserID", UserID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@EnterpriseID", EnterpriceID), new SqlParameter("@ErrorException", ErrorException ?? (object)DBNull.Value) };
                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    string SQLCommand = "EXEC [" + MasterDbConnectionHelper.GeteTurnsLoggingDBName() + "].[dbo].[INSERTeTurnsAuthorizationError]  @ErrorPurposeFor,@ActionMethod,@ControllerName,@UserName,@UserID,@RoomID,@CompanyID,@EnterpriseID,@ErrorException";
                    context.Database.ExecuteSqlCommand(SQLCommand, params1);
                }
            }
            catch
            {
            }

        }


        public static void LogError(Exception ex, Int64 RoomId, Int64 CompanyID, Int64 EnterpriseID)
        {
            eTurnsMaster.DAL.ReportSchedulerError objReportSchedulerError = new eTurnsMaster.DAL.ReportSchedulerError();
            eTurnsMaster.DAL.CommonMasterDAL commonDAL = new eTurnsMaster.DAL.CommonMasterDAL();
            objReportSchedulerError.CompanyID = CompanyID;
            objReportSchedulerError.EnterpriseID = EnterpriseID;
            objReportSchedulerError.RoomID = RoomId;

            objReportSchedulerError.Exception = Convert.ToString(ex) ?? "Null exception";
            if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Request != null && System.Web.HttpContext.Current.Request.Url != null)
            {
                objReportSchedulerError.Exception = objReportSchedulerError.Exception + " " + System.Web.HttpContext.Current.Request.Url.ToString();
            }
            objReportSchedulerError.ID = 0;
            objReportSchedulerError.NotificationID = 0;
            objReportSchedulerError.ScheduleFor = 0;
            commonDAL.SaveNotificationError(objReportSchedulerError);
            if (ex.InnerException != null)
            {
                LogError(ex.InnerException, RoomId, CompanyID, EnterpriseID);
            }

        }
        public void InsertScriptsExecution(long UserId, string DatabaseName, string Script)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@UserId", UserId), new SqlParameter("@DatabaseName", (DatabaseName ?? (object)DBNull.Value)), new SqlParameter("@Script", (Script ?? (object)DBNull.Value)) };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("EXEC InsertScriptsExecution @UserId,@DatabaseName,@Script", params1);
            }

        }

        public Dictionary<int, string> getGridResourceListName(string CurrentUDFListName)
        {
                using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    var params1 = new SqlParameter[] { new SqlParameter("@UDfTableName", CurrentUDFListName) };
                    var data = (from u in context.Database.SqlQuery<UDFGridLinked>("exec [GetUDFGridDataByTableName] @UDfTableName", params1)
                                select new
                                {
                                    Id = u.Id,
                                    Result = u.Result

                                }).ToList();

                    return data.AsParallel().ToDictionary(ci => ci.Id, ci => ci.Result);
                }            
        }

        public Dictionary<int, string> GetUDfTableNameByListName(string ListName)
        {
                using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    var params1 = new SqlParameter[] { new SqlParameter("@ListName", ListName) };
                    var data = (from u in context.Database.SqlQuery<UDFGridLinked>("exec [GetUDFGridDataByListName] @ListName", params1)
                                select new
                                {
                                    Id = u.Id,
                                    Result = u.Result

                                }).ToList();

                    return data.AsParallel().ToDictionary(ci => ci.Id, ci => ci.Result);
                }
        }

        public List<OLEDBConnectionInfo> GetAllConnectionparams()
        {
            List<OLEDBConnectionInfo> lstOLEDBConnectionsdecrypted = CacheHelper<List<OLEDBConnectionInfo>>.GetCacheItem("Connectionparams");
            if (lstOLEDBConnectionsdecrypted == null || lstOLEDBConnectionsdecrypted.Count == 0)
            {
                string EnKey = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["Enkey"]);
                SecHelper objSecHelper = new SecHelper();
                List<OLEDBConnectionInfo> lstOLEDBConnections = getCons();
                if (lstOLEDBConnections != null && lstOLEDBConnections.Count > 0)
                {
                    try
                    {
                        lstOLEDBConnectionsdecrypted = (from t in lstOLEDBConnections
                                                        select new OLEDBConnectionInfo
                                                        {
                                                            ID = t.ID,
                                                            ConectionType = t.ConectionType,
                                                            APP = objSecHelper.DecryptData(t.APP, EnKey),
                                                            ApplicationIntent = objSecHelper.DecryptData(t.ApplicationIntent, EnKey),
                                                            AppDatabase = objSecHelper.DecryptData(t.AppDatabase, EnKey),
                                                            MarsConn = objSecHelper.DecryptData(t.MarsConn, EnKey),
                                                            PacketSize = objSecHelper.DecryptData(t.PacketSize, EnKey),
                                                            PWD = objSecHelper.DecryptData(t.PWD, EnKey),
                                                            Server = objSecHelper.DecryptData(t.Server, EnKey),
                                                            Timeout = objSecHelper.DecryptData(t.Timeout, EnKey),
                                                            Trusted_Connection = objSecHelper.DecryptData(t.Trusted_Connection, EnKey),
                                                            UID = objSecHelper.DecryptData(t.UID, EnKey),
                                                            FailoverPartner = objSecHelper.DecryptData(t.FailoverPartner, EnKey),
                                                            PersistSensitive = objSecHelper.DecryptData(t.PersistSensitive, EnKey),
                                                            MultiSubnetFailover = objSecHelper.DecryptData(t.MultiSubnetFailover, EnKey),
                                                            Created = t.Created,
                                                            Updated = t.Updated,
                                                            CreatedBy = t.CreatedBy,
                                                            UpdatedBy = t.UpdatedBy,
                                                            GUID = t.GUID

                                                        }).ToList();

                        CacheHelper<List<OLEDBConnectionInfo>>.InvalidateCache();
                        CacheHelper<List<OLEDBConnectionInfo>>.AppendToCacheItem("Connectionparams", lstOLEDBConnectionsdecrypted);
                        return lstOLEDBConnectionsdecrypted;
                    }
                    catch
                    {
                        return lstOLEDBConnections;
                    }
                }
                return lstOLEDBConnections;
            }
            else
            {
                return lstOLEDBConnectionsdecrypted;
            }
        }
        public void SaveConnectionParams(List<OLEDBConnectionInfo> lstOLEDBConnections)
        {
            SecHelper objSecHelper = new SecHelper();
            List<OLEDBConnectionInfo> lstOLEDBConnectionsEncrypted = new List<OLEDBConnectionInfo>();
            if (lstOLEDBConnections != null && lstOLEDBConnections.Count > 0)
            {
                string EnKey = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["Enkey"]);
                OLEDBConnectionInfo objOLEDBConnectionInfo;
                foreach (var item in lstOLEDBConnections)
                {
                    objOLEDBConnectionInfo = new OLEDBConnectionInfo();
                    objOLEDBConnectionInfo.ID = item.ID;
                    objOLEDBConnectionInfo.ConectionType = objSecHelper.EncryptData((item.ConectionType ?? string.Empty), EnKey);
                    objOLEDBConnectionInfo.APP = objSecHelper.EncryptData(item.APP ?? string.Empty, EnKey);
                    objOLEDBConnectionInfo.ApplicationIntent = objSecHelper.EncryptData(item.ApplicationIntent ?? string.Empty, EnKey);
                    objOLEDBConnectionInfo.AppDatabase = objSecHelper.EncryptData(item.AppDatabase ?? string.Empty, EnKey);
                    objOLEDBConnectionInfo.MarsConn = objSecHelper.EncryptData(item.MarsConn ?? string.Empty, EnKey);
                    objOLEDBConnectionInfo.PacketSize = objSecHelper.EncryptData(item.PacketSize ?? string.Empty, EnKey);
                    objOLEDBConnectionInfo.PWD = objSecHelper.EncryptData(item.PWD ?? string.Empty, EnKey);
                    objOLEDBConnectionInfo.Server = objSecHelper.EncryptData(item.Server ?? string.Empty, EnKey);
                    objOLEDBConnectionInfo.Timeout = objSecHelper.EncryptData(item.Timeout ?? string.Empty, EnKey);
                    objOLEDBConnectionInfo.Trusted_Connection = objSecHelper.EncryptData(item.Trusted_Connection ?? string.Empty, EnKey);
                    objOLEDBConnectionInfo.UID = objSecHelper.EncryptData(item.UID ?? string.Empty, EnKey);
                    objOLEDBConnectionInfo.FailoverPartner = objSecHelper.EncryptData(item.FailoverPartner ?? string.Empty, EnKey);
                    objOLEDBConnectionInfo.PersistSensitive = objSecHelper.EncryptData(item.PersistSensitive ?? string.Empty, EnKey);
                    objOLEDBConnectionInfo.MultiSubnetFailover = objSecHelper.EncryptData(item.MultiSubnetFailover ?? string.Empty, EnKey);
                    objOLEDBConnectionInfo.UpdatedBy = item.UpdatedBy;
                    objOLEDBConnectionInfo.Updated = DateTime.UtcNow;
                    lstOLEDBConnectionsEncrypted.Add(objOLEDBConnectionInfo);
                }
                lstOLEDBConnections = SaveCons(lstOLEDBConnectionsEncrypted);
                List<OLEDBConnectionInfo> lstOLEDBConnectionsdecrypted = (from t in lstOLEDBConnections
                                                                          select new OLEDBConnectionInfo
                                                                          {
                                                                              ID = t.ID,
                                                                              ConectionType = t.ConectionType,
                                                                              APP = objSecHelper.DecryptData(t.APP, EnKey),
                                                                              ApplicationIntent = objSecHelper.DecryptData(t.ApplicationIntent, EnKey),
                                                                              AppDatabase = objSecHelper.DecryptData(t.AppDatabase, EnKey),
                                                                              MarsConn = objSecHelper.DecryptData(t.MarsConn, EnKey),
                                                                              PacketSize = objSecHelper.DecryptData(t.PacketSize, EnKey),
                                                                              PWD = objSecHelper.DecryptData(t.PWD, EnKey),
                                                                              Server = objSecHelper.DecryptData(t.Server, EnKey),
                                                                              Timeout = objSecHelper.DecryptData(t.Timeout, EnKey),
                                                                              Trusted_Connection = objSecHelper.DecryptData(t.Trusted_Connection, EnKey),
                                                                              UID = objSecHelper.DecryptData(t.UID, EnKey),
                                                                              FailoverPartner = objSecHelper.DecryptData(t.FailoverPartner, EnKey),
                                                                              PersistSensitive = objSecHelper.DecryptData(t.PersistSensitive, EnKey),
                                                                              MultiSubnetFailover = objSecHelper.DecryptData(t.MultiSubnetFailover, EnKey),
                                                                              Created = t.Created,
                                                                              Updated = t.Updated,
                                                                              CreatedBy = t.CreatedBy,
                                                                              UpdatedBy = t.UpdatedBy,
                                                                              GUID = t.GUID

                                                                          }).ToList();

                CacheHelper<List<OLEDBConnectionInfo>>.InvalidateCache();

                CacheHelper<List<OLEDBConnectionInfo>>.AppendToCacheItem("Connectionparams", lstOLEDBConnectionsdecrypted);

            }

        }


        public List<OLEDBConnectionInfo> getCons()
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionStringeTurns))
            {
                return context.Database.SqlQuery<OLEDBConnectionInfo>("EXEC " + MasterDbConnectionHelper.GetETurnsMasterDBName() + ".dbo.[getCons]").ToList();
            }
        }
        public List<OLEDBConnectionInfo> SaveCons(List<OLEDBConnectionInfo> lstUpdates)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                if (lstUpdates != null && lstUpdates.Count > 0)
                {
                    foreach (var item in lstUpdates)
                    {
                        var params1 = new SqlParameter[] { new SqlParameter("@ID", item.ID), new SqlParameter("@APP", item.APP ?? (object)DBNull.Value), new SqlParameter("@ApplicationIntent", item.ApplicationIntent ?? (object)DBNull.Value), new SqlParameter("@AppDatabase", item.AppDatabase ?? (object)DBNull.Value), new SqlParameter("@MarsConn", item.MarsConn ?? (object)DBNull.Value), new SqlParameter("@PacketSize", item.PacketSize ?? (object)DBNull.Value), new SqlParameter("@PWD", item.PWD ?? (object)DBNull.Value), new SqlParameter("@Server", item.Server ?? (object)DBNull.Value), new SqlParameter("@Timeout", item.Timeout ?? (object)DBNull.Value), new SqlParameter("@Trusted_Connection", item.Trusted_Connection ?? (object)DBNull.Value), new SqlParameter("@UID", item.UID ?? (object)DBNull.Value), new SqlParameter("@FailoverPartner", item.FailoverPartner ?? (object)DBNull.Value), new SqlParameter("@PersistSensitive", item.PersistSensitive ?? (object)DBNull.Value), new SqlParameter("@MultiSubnetFailover", item.MultiSubnetFailover ?? (object)DBNull.Value), new SqlParameter("@UserID", item.UpdatedBy) };
                        context.Database.ExecuteSqlCommand("exec " + MasterDbConnectionHelper.GetETurnsMasterDBName() + ".dbo.[SaveCon] @ID,@APP,@ApplicationIntent,@AppDatabase,@MarsConn,@PacketSize,@PWD,@Server,@Timeout,@Trusted_Connection,@UID,@FailoverPartner,@PersistSensitive,@MultiSubnetFailover,@UserID", params1);
                    }
                }
            }
            return getCons();
        }

        public SiteUrlSettingDTO GetSiteUrlSetting(string MVCUrl)
        {
            IEnumerable<SiteUrlSettingDTO> CachedData = CacheHelper<IEnumerable<SiteUrlSettingDTO>>.GetCacheItem("Cached_SiteUrlSettingDTO");
            
            if (CachedData == null)
            {
                using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                   var urlSettings =  context.Database.SqlQuery<SiteUrlSettingDTO>("exec [GetSiteUrlSetting]").ToList();
                   CachedData = CacheHelper<IEnumerable<SiteUrlSettingDTO>>.AddCacheItem("Cached_SiteUrlSettingDTO", urlSettings);
                }
            }

            SiteUrlSettingDTO setting;
            setting = CachedData.FirstOrDefault(t => t.MVCUrl.ToLower().Trim() == MVCUrl.ToLower().Trim());
            return setting;
        }

        public void RefreshSiteURLSetting()
        {
                using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    var urlSettings = context.Database.SqlQuery<SiteUrlSettingDTO>("exec [GetSiteUrlSetting]").ToList();
                    CacheHelper<IEnumerable<SiteUrlSettingDTO>>.AddCacheItem("Cached_SiteUrlSettingDTO", urlSettings);
                }
        }

        public List<SQLTimeZone> GetAllTimeZonesFromDB()
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<SQLTimeZone>("exec [GetAllTimeZonesFromDB]").ToList();
            }
        }

        public class UDFGridLinked
        {

            public int Id { get; set; }
            public string Result { get; set; }
        }
    }
}
