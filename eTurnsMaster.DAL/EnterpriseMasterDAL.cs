using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Data.SqlClient;
using eTurns.DTO.Resources;


namespace eTurnsMaster.DAL
{
    public partial class EnterpriseMasterDAL : eTurnsMasterBaseDAL
    {
        public List<EnterPriseSQLScriptsDTO> GetDbAllScripts(bool IsDeleted)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@IsDeleted", IsDeleted)
                                                };
                return context.Database.SqlQuery<EnterPriseSQLScriptsDTO>("exec [GetAllEnterpriseScriptsMaster] @IsDeleted", params1).ToList();
            }
        }

        public EnterPriseSQLScriptsDTO GetEnterpriseScriptsMasterBySQLScriptIDPlain(long SQLScriptID)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@SQLScriptID", SQLScriptID)
                                                };
                return context.Database.SqlQuery<EnterPriseSQLScriptsDTO>("exec [GetEnterpriseScriptsMasterBySQLScriptIDPlain] @SQLScriptID", params1).FirstOrDefault();
            }

        }

        public List<EnterPriseSQLScriptsDtlDTO> GetEnterpriceStatusForScript(long SqlScriptId)
        {
            List<EnterPriseSQLScriptsDtlDTO> lstEnterpriseDTO = new List<EnterPriseSQLScriptsDtlDTO>();
            using (eTurns_MasterEntities objData = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                lstEnterpriseDTO = (from epm in objData.EnterpriseMasters
                                    join epmscrptdtl in objData.EnterpriseSqlScriptsDetails on new { epid = epm.ID, sqlscrid = SqlScriptId, isl = true } equals new { epid = epmscrptdtl.EnterpriseID, sqlscrid = epmscrptdtl.SQLScriptID, isl = epmscrptdtl.IsLatestExecution } into epm_epmscrptdtl_join
                                    from epm_epmscrptdtl in epm_epmscrptdtl_join.DefaultIfEmpty()
                                    select new EnterPriseSQLScriptsDtlDTO
                                    {
                                        EnterpriseID = epm.ID,
                                        EnterPriseName = epm.Name,
                                        EnterpriseSqlScriptsDtlID = epm_epmscrptdtl != null ? epm_epmscrptdtl.EnterpriseSqlScriptsDtlID : 0,
                                        IsExecuitedSuccessfully = epm_epmscrptdtl != null ? epm_epmscrptdtl.IsExecuitedSuccessfully : false,
                                        IsLatestExecution = epm_epmscrptdtl != null ? epm_epmscrptdtl.IsLatestExecution : false,
                                        ReturnResult = epm_epmscrptdtl != null ? epm_epmscrptdtl.ReturnResult : "Pending script Execution",
                                        SQLScriptID = SqlScriptId,
                                        EnterPriseDBName = epm.EnterpriseDBName
                                    }).OrderBy(t => t.SQLScriptID).ToList();


            }
            return lstEnterpriseDTO;
        }

        public bool ExecuitScriptOnEnterprises(long SqlScriptId, string EnterpriseIds)
        {
            string Connectionstring = MasterDbConnectionHelper.GeteTurnsMasterSQLConnectionString(MasterDbConnectionHelper.GetETurnsMasterDBName(), MasterDbConnectionType.GeneralReadWrite.ToString("F"));
            SqlHelper.ExecuteScalar(Connectionstring, "ExecuitDbScriptForEnterPrice", EnterpriseIds, SqlScriptId, null);
            return true;
        }

        public EnterpriseDTO GetEnterprise(Int64 id)
        {
            EnterpriseDTO objEnterpriseDTO = new EnterpriseDTO();
            string MasterDbConnectionString = MasterDbConnectionHelper.GeteTurnsMasterSQLConnectionString(MasterDbConnectionHelper.GetETurnsMasterDBName(), MasterDbConnectionType.GeneralReadWrite.ToString("F"));
            SqlConnection EturnsMasterConnection = new SqlConnection(MasterDbConnectionString);
            DataSet dsEnterprise = new DataSet();
            dsEnterprise = SqlHelper.ExecuteDataset(EturnsMasterConnection, "GetEnterpriseWithSuperAdminById", id);

            if (dsEnterprise != null && dsEnterprise.Tables.Count > 0)
            {
                if (dsEnterprise.Tables.Count > 1)
                {
                    objEnterpriseDTO = DataTableHelper.ToList<EnterpriseDTO>(dsEnterprise.Tables[1]).FirstOrDefault();
                    objEnterpriseDTO.lstSuperAdmins = DataTableHelper.ToList<EnterpriseSuperAdmin>(dsEnterprise.Tables[0]);
                    objEnterpriseDTO.EnterpriseSuperAdmins = string.Join(",", objEnterpriseDTO.lstSuperAdmins.Select(t => t.UserName).ToArray());
                }
            }
            return objEnterpriseDTO;
        }

        public EnterpriseDTO GetEnterpriseByIdPlain(long Id)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Id", Id) };
                return context.Database.SqlQuery<EnterpriseDTO>("exec [GetEnterpriseByIdPlain] @Id", params1).FirstOrDefault();
            }
        }

        public EnterpriseDTO GetEnterpriseByNamePlain(string Name)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Name", Name) };
                return context.Database.SqlQuery<EnterpriseDTO>("exec [GetEnterpriseByNamePlain] @Name", params1).FirstOrDefault();
            }
        }

        public EnterpriseDTO GetNonDeletedEnterpriseByIdPlain(long Id)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Id", Id) };
                return context.Database.SqlQuery<EnterpriseDTO>("exec [GetNonDeletedEnterpriseByIdPlain] @Id", params1).FirstOrDefault();
            }
        }

        public Dictionary<long, string> GetEnterpriseListWithDBName(List<long> EnterpriseIDs)
        {
            string Ids = string.Join(",", EnterpriseIDs);
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Ids", Ids) };
                var enterprises = context.Database.SqlQuery<EnterpriseDTO>("exec [GetEnterprisesByIds] @Ids", params1).ToList();
                Dictionary<long, string> enterpriseList = enterprises.ToDictionary(o => o.ID, o => o.EnterpriseDBName);
                return enterpriseList;
            }
        }

        public List<EnterpriseDTO> GetAllEnterpriseForExecution()
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<EnterpriseDTO>("exec [GetAllEnterpriseForExecution] ").ToList();
            }
        }

        public List<EnterpriseDTO> GetAllEnterpriseForExecutionWithDemo()
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                List<EnterpriseDTO> obj = context.Database.SqlQuery<EnterpriseDTO>("exec [GetAllEnterpriseForExecutionWithDemo] ").ToList();

                if (System.Configuration.ConfigurationManager.AppSettings["TemplateDbName"] != null)
                {
                    string EturnsName = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["TemplateDbName"]);
                    obj.Insert(0, new EnterpriseDTO { Name = EturnsName, EnterpriseDBName = EturnsName });
                }
                return obj;
            }
        }

        public List<EnterpriseDTO> GetAllEnterprisesPlain()
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<EnterpriseDTO>("exec [GetAllEnterprisesPlain] ").ToList();
            }
        }
        public EnterpriseDTO GetBorderStateEnterprisesAndCompany(long EnterpriseId)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@EnterpriseId", EnterpriseId) };
                return context.Database.SqlQuery<EnterpriseDTO>("EXEC [GetBorderStateEnterprisesAndCompany] @EnterpriseId", params1).FirstOrDefault();
            }
        }
        public EnterPriseConfigDTO GetEnterpriseConfigByEnterpriseIdPlain(long EnterpriseId)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@EnterpriseId", EnterpriseId) };
                return context.Database.SqlQuery<EnterPriseConfigDTO>("exec [GetEnterpriseConfigByEnterpriseIdPlain] @EnterpriseId", params1).FirstOrDefault();
            }
        }

        public DashboardParameterDTO GetDashboardParamByRoomAndCompanyPlain(long RoomID, long CompanyID)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                return context.Database.SqlQuery<DashboardParameterDTO>("exec [GetDashboardParamByRoomAndCompanyPlain] @RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }

        public bool InsertDashboardParameter(DashboardParameterDTO objDTO)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                //Assign & Mapping of the DTO Class property to Entity Class 
                var obj = new DashboardParameter
                {
                    ID = 0,
                    RoomId = objDTO.RoomId,
                    CompanyId = objDTO.CompanyId,
                    CreatedOn = objDTO.CreatedOn,
                    CreatedBy = objDTO.CreatedBy,
                    UpdatedOn = objDTO.UpdatedOn,
                    UpdatedBy = objDTO.UpdatedBy,
                    TurnsMeasureMethod = objDTO.TurnsMeasureMethod,
                    TurnsMonthsOfUsageToSample = objDTO.TurnsMonthsOfUsageToSample,
                    TurnsDaysOfUsageToSample = objDTO.TurnsDaysOfUsageToSample,
                    AUDayOfUsageToSample = objDTO.AUDayOfUsageToSample,
                    AUMeasureMethod = objDTO.AUMeasureMethod,
                    AUDaysOfDailyUsage = objDTO.AUDaysOfDailyUsage,
                    MinMaxMeasureMethod = objDTO.MinMaxMeasureMethod,
                    MinMaxDayOfUsageToSample = objDTO.MinMaxDayOfUsageToSample,
                    MinMaxDayOfAverage = objDTO.MinMaxDayOfAverage,
                    MinMaxMinNumberOfTimesMax = objDTO.MinMaxMinNumberOfTimesMax,
                    MinMaxOptValue1 = objDTO.MinMaxOptValue1,
                    MinMaxOptValue2 = objDTO.MinMaxOptValue2,
                    GraphFromMonth = objDTO.GraphFromMonth,
                    GraphToMonth = objDTO.GraphToMonth,
                    GraphFromYear = objDTO.GraphFromYear,
                    GraphToYear = objDTO.GraphToYear,
                    IsTrendingEnabled = objDTO.IsTrendingEnabled,
                    PieChartmetricOn = objDTO.PieChartmetricOn,
                    TurnsCalculatedStockRoomTurn = objDTO.TurnsCalculatedStockRoomTurn,
                    AUCalculatedDailyUsageOverSample = objDTO.AUCalculatedDailyUsageOverSample,
                    MinMaxCalculatedDailyUsageOverSample = objDTO.MinMaxCalculatedDailyUsageOverSample,
                    MinMaxCalcAvgPullByDay = objDTO.MinMaxCalcAvgPullByDay,
                    MinMaxCalcualtedMax = objDTO.MinMaxCalcualtedMax,
                    AutoClassification = objDTO.AutoClassification,
                    MonthlyAverageUsage = objDTO.MonthlyAverageUsage,
                    AnnualCarryingCostPercent = objDTO.AnnualCarryingCostPercent,
                    LargestAnnualCashSavings = objDTO.LargestAnnualCashSavings
                };

                context.DashboardParameters.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;
            }

            return true;
        }


        /// <summary>
        /// Insert Record in the DataBase Technician Master
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public EnterpriseDTO Insert(EnterpriseDTO objDTO)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                //Assign & Mapping of the DTO Class property to Entity Class 
                var obj = new EnterpriseMaster
                {
                    ID = objDTO.ID,
                    Name = objDTO.Name,
                    Address = objDTO.Address,
                    City = objDTO.City,
                    State = objDTO.State,
                    PostalCode = objDTO.PostalCode,
                    Country = objDTO.Country,
                    ContactPhone = objDTO.ContactPhone,
                    ContactEmail = objDTO.ContactEmail,
                    UDF1 = objDTO.UDF1,
                    UDF2 = objDTO.UDF2,
                    UDF3 = objDTO.UDF3,
                    UDF4 = objDTO.UDF4,
                    UDF5 = objDTO.UDF5,
                    GUID = Guid.NewGuid(),
                    Updated = DateTime.UtcNow,
                    Created = DateTime.UtcNow,
                    CreatedBy = objDTO.CreatedBy,
                    LastUpdatedBy = objDTO.LastUpdatedBy,
                    IsDeleted = false,
                    IsArchived = false,
                    IsActive = objDTO.IsActive,
                    EnterpriseDomainURL = objDTO.EnterPriseDomainURL,
                    AllowABIntegration = objDTO.AllowABIntegration
                };
                context.EnterpriseMasters.Add(obj);
                context.SaveChanges();
                List<EnterPriseSQLScriptsDTO> lstScripts = new List<EnterPriseSQLScriptsDTO>();
                lstScripts = GetDbAllScripts(false).Where(x => x.SQLScriptID <= 4).OrderBy(t => t.SQLScriptID).ToList();
                foreach (var item in lstScripts)
                {
                    ExecuitScriptOnEnterprises(item.SQLScriptID, obj.ID.ToString());
                }
                //ExecuitScriptOnEnterprises(1, obj.ID.ToString());
                //ExecuitScriptOnEnterprises(2, obj.ID.ToString());
                //ExecuitScriptOnEnterprises(3, obj.ID.ToString());
                //ExecuitScriptOnEnterprises(4, obj.ID.ToString());


                EnterpriseMasterDAL objEnterprise = new EnterpriseMasterDAL();
                //EnterpriseDTO objEnterPrise = new EnterpriseDTO();
                //objEnterPrise = objEnterprise.GetEnterprise(obj.ID);
                string EnterpriseDbConnection = MasterDbConnectionHelper.GeteTurnsSQLConnectionString(obj.EnterpriseDBName, MasterDbConnectionType.GeneralReadWrite.ToString("F"));
                Int64 EnterpriseID = obj.ID;
                SqlHelper.ExecuteNonQuery(EnterpriseDbConnection, "InsertEnterpriseConfig", EnterpriseID);
                for (int i = 1; i < 4; i++)
                {
                    EnterPriseUserMasterDAL objEnterPriseUserMasterDAL = new EnterPriseUserMasterDAL();
                    UserMasterDTO objUserMasterDTO = new UserMasterDTO();
                    objUserMasterDTO.CompanyID = 0;
                    objUserMasterDTO.ConfirmPassword = "password";
                    objUserMasterDTO.Created = DateTime.UtcNow;
                    objUserMasterDTO.CreatedBy = objDTO.CreatedBy;
                    objUserMasterDTO.CreatedByName = objDTO.CreatedByName;
                    objUserMasterDTO.Email = objDTO.EnterpriseUserEmail;
                    objUserMasterDTO.EnterpriseDbName = string.Empty;
                    objUserMasterDTO.EnterpriseId = obj.ID;
                    objUserMasterDTO.GUID = Guid.NewGuid();
                    objUserMasterDTO.IsArchived = false;
                    objUserMasterDTO.IsDeleted = false;
                    objUserMasterDTO.LastUpdatedBy = obj.LastUpdatedBy;
                    objUserMasterDTO.Password = objDTO.EnterpriseUserPassword;
                    objUserMasterDTO.Phone = objDTO.ContactPhone;
                    objUserMasterDTO.RoleName = "Enterprise Role";
                    objUserMasterDTO.Updated = DateTime.UtcNow;

                    if (i == 1)
                    {
                        objUserMasterDTO.UserType = 2;
                        objUserMasterDTO.UserName = objDTO.UserName;
                        objUserMasterDTO.RoleID = -2;
                    }
                    else if (i == 2)
                    {
                        objUserMasterDTO.UserType = 4;
                        objUserMasterDTO.UserName = "Enterprise System User";
                        objUserMasterDTO.RoleID = -4;
                    }
                    else if (i == 3)
                    {
                        objUserMasterDTO.UserType = 5;
                        objUserMasterDTO.UserName = "eTurns User";
                        objUserMasterDTO.RoleID = -5;
                    }
                    objUserMasterDTO = objEnterPriseUserMasterDAL.SaveUseronMasterDB(objUserMasterDTO);

                    Int64 NewUserID = objUserMasterDTO.ID;

                    // Insert into Enterprise Config File

                    if (i == 1)
                    {

                        {

                            EnterpriseMaster objEnterpriseMaster = context.EnterpriseMasters.Where(t => t.ID == obj.ID).FirstOrDefault();
                            if (objEnterpriseMaster != null)
                            {
                                objEnterpriseMaster.EnterpriseSuperAdmin = NewUserID;
                            }
                            context.SaveChanges();

                        }
                    }
                    InsertUserInChildDB(NewUserID, objUserMasterDTO, obj.ID);
                }

                return GetEnterprise(obj.ID);
            }
        }

        public void InsertUserInChildDB(Int64 NewUserID, UserMasterDTO objUserMasterDTO, long EnterpriseID)
        {
            EnterpriseMasterDAL objEnterprise = new EnterpriseMasterDAL();
            EnterpriseDTO obj = new EnterpriseDTO();
            obj = objEnterprise.GetEnterpriseByIdPlain(EnterpriseID);
            string EnterpriseDbConnection = MasterDbConnectionHelper.GeteTurnsSQLConnectionString(obj.EnterpriseDBName, MasterDbConnectionType.GeneralReadWrite.ToString("F"));
            SqlHelper.ExecuteNonQuery(EnterpriseDbConnection, "CreateUserData", NewUserID, objUserMasterDTO.UserName, objUserMasterDTO.Password, objUserMasterDTO.Phone, objUserMasterDTO.Email, objUserMasterDTO.RoleID, DateTime.UtcNow.ToString("yyyy-MM-dd"), objUserMasterDTO.LastUpdatedBy, false, objUserMasterDTO.GUID, false, false, objUserMasterDTO.CompanyID, objUserMasterDTO.UserType);
        }

        public void UpdateEnterpriseSuperUser(Int64 NewUserID, UserMasterDTO objUserMasterDTO, EnterpriseDTO objDTO)
        {
            EnterpriseMasterDAL objEnterprise = new EnterpriseMasterDAL();
            EnterpriseDTO obj = new EnterpriseDTO();
            obj = objEnterprise.GetEnterpriseByIdPlain(objDTO.ID);
            string EnterpriseDbConnection = MasterDbConnectionHelper.GeteTurnsSQLConnectionString(obj.EnterpriseDBName, MasterDbConnectionType.GeneralReadWrite.ToString("F"));

            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                UserMaster objUserMaster = context.UserMasters.FirstOrDefault(t => t.ID == objUserMasterDTO.ID);
                objUserMaster.UserName = objDTO.UserName;
                if (objDTO.IsPasswordChanged == "1")
                {
                    objUserMaster.Password = objDTO.EnterpriseUserPassword;
                }
                if (objDTO.IsEmailChanged == "1")
                {
                    objUserMaster.Email = objDTO.EnterpriseUserEmail;
                }
                context.SaveChanges();
            }
            bool IsPasswordChanged = false;
            bool IsEmailChanged = false;

            if (objDTO.IsPasswordChanged == "1" && objDTO.IsEmailChanged == "1")
            {
                IsPasswordChanged = true;
                IsEmailChanged = true;
            }
            else if (objDTO.IsPasswordChanged == "1")
            {
                IsPasswordChanged = true;
            }
            else if (objDTO.IsEmailChanged == "1")
            {
                IsEmailChanged = true;
            }

            SqlConnection EturnsConnection = new SqlConnection(EnterpriseDbConnection);
            int retval = SqlHelper.ExecuteNonQuery(EturnsConnection, "UpdateEnterpriseSuperUser", IsPasswordChanged, IsEmailChanged, objUserMasterDTO.ID, objDTO.EnterpriseUserEmail, objDTO.EnterpriseUserPassword);
        }

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public EnterpriseDTO Edit(EnterpriseDTO objDTO)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                EnterpriseMaster obj = context.EnterpriseMasters.FirstOrDefault(t => t.ID == objDTO.ID);
                if (obj != null)
                {
                    obj.ID = objDTO.ID;
                    obj.Name = objDTO.Name;
                    obj.Address = objDTO.Address;
                    obj.City = objDTO.City;
                    obj.State = objDTO.State;
                    obj.PostalCode = objDTO.PostalCode;
                    obj.Country = objDTO.Country;
                    obj.ContactPhone = objDTO.ContactPhone;
                    obj.ContactEmail = objDTO.ContactEmail;
                    obj.UDF1 = objDTO.UDF1;
                    obj.UDF2 = objDTO.UDF2;
                    obj.UDF3 = objDTO.UDF3;
                    obj.UDF4 = objDTO.UDF4;
                    obj.UDF5 = objDTO.UDF5;
                    obj.Updated = DateTime.UtcNow;
                    obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                    obj.IsActive = objDTO.IsActive;
                    obj.EnterpriseSuperAdmin = objDTO.EnterpriseSuperAdmin;
                    obj.AllowABIntegration = objDTO.AllowABIntegration;
                    //obj.EnterPriseDomainURL = objDTO.EnterPriseDomainURL;
                    context.SaveChanges();

                    //UserMasterDTO objUserMaster = new UserMasterDAL().GetUserByID(objDTO.EnterpriseUserID);
                    //UpdateEnterpriseSuperUser(objUserMaster.ID, objUserMaster, objDTO);

                    return GetEnterprise(objDTO.ID);
                }
                else
                {
                    return null;
                }

            }

        }

        /// <summary>
        /// Delete Multiple Records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public bool DeleteEnterprise(string IDs, long UserId)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@EnterpriseIds", IDs), new SqlParameter("@UserId", UserId) };
                context.Database.ExecuteSqlCommand("exec [DeleteEnterprise] @EnterpriseIds, @UserId", params1);
                return true;
            }
        }
        public bool UnDeleteEnterprise(string IDs, long UserId)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@EnterpriseIds", IDs), new SqlParameter("@UserId", UserId) };
                context.Database.ExecuteSqlCommand("exec [UnDeleteEnterprise] @EnterpriseIds, @UserId", params1);
                return true;
            }
        }

        /// <summary>
        /// Update Data - Grid Update
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="ColumnValue"></param>
        /// <param name="rowId"></param>
        /// <param name="columnPosition"></param>
        /// <param name="columnId"></param>
        /// <param name="ColumnName"></param>
        /// <returns></returns>
        public string UpdateEnterpriseDynamic(long Id, string ColumnValue, string ColumnName)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@ColumnName", ColumnName),
                                                    new SqlParameter("@ColumnValue", ColumnValue) ,
                                                    new SqlParameter("@ID", Id)
                                                };
                context.Database.ExecuteSqlCommand("exec [UpdateEnterpriseDynamic] @ColumnName,@ColumnValue,@ID", params1);
            }
            return ColumnValue;
        }

        public List<EnterpriseDTO> GetEnterprisesByUserPlain(long UserId, long RoleId, int UserType)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@UserId", UserId),
                                                    new SqlParameter("@RoleId", RoleId) ,
                                                    new SqlParameter("@UserType", UserType)
                                                 };
                return context.Database.SqlQuery<EnterpriseDTO>("exec [GetEnterprisesByUserPlain] @UserId,@RoleId,@UserType", params1).ToList();
            }
        }

        public List<RoleCompanyMasterDTO> GetCompaniesByEnterpriseIdsNormal(long[] EnterpriseIds)
        {
            string Ids = string.Join(",", EnterpriseIds);
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@EnterpriseIds", Ids)
                                                 };
                return context.Database.SqlQuery<RoleCompanyMasterDTO>("exec [GetCompaniesByEnterpriseIdsNormal] @EnterpriseIds", params1).ToList();
            }
        }

        public List<RoomDTO> GetRoomsByCompany(List<CompanyMasterDTO> lstCompanies)
        {
            List<RoomDTO> lstRooms = new List<RoomDTO>();
            long[] EnterpriseIds = lstCompanies.Select(t => t.EnterPriseId).Distinct().ToArray();

            foreach (long eid in EnterpriseIds)
            {
                string strCompanies = string.Join(",", lstCompanies.Where(t => t.EnterPriseId == eid).Select(t => t.ID).ToArray());
                EnterpriseDTO objEnterprise = new EnterpriseDTO();
                objEnterprise = GetEnterpriseByIdPlain(eid);

                if (objEnterprise != null)
                {
                    if (!string.IsNullOrWhiteSpace(objEnterprise.EnterpriseDBName))
                    {
                        DataSet dsRooms = new DataSet();
                        string EnterpriseDbConnection = MasterDbConnectionHelper.GeteTurnsSQLConnectionString(objEnterprise.EnterpriseDBName, MasterDbConnectionType.GeneralReadWrite.ToString("F"));
                        dsRooms = SqlHelper.ExecuteDataset(EnterpriseDbConnection, "GetRoomByCompanyIdsNormal", strCompanies);

                        if (dsRooms != null && dsRooms.Tables.Count > 0)
                        {
                            var rooms = DataTableHelper.ToList<RoomDTO>(dsRooms.Tables[0]);

                            if (rooms != null && rooms.Any())
                            {
                                lstRooms.AddRange(rooms);
                            }
                        }
                    }
                }

            }
            return lstRooms;
        }

        public List<RoleDetailsRoom> GetRoomsByEntIdsAndCmpIdsNormal(List<CompanyMasterDTO> lstCompanies)
        {
            long[] eps = lstCompanies.Select(t => t.EnterPriseId).Distinct().ToArray();
            long[] cps = lstCompanies.Select(t => t.ID).Distinct().ToArray();
            string EnterpriseIds = string.Join(",", eps);
            string CompanyIds = string.Join(",", cps);

            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@EnterpriseIds", EnterpriseIds),
                                                    new SqlParameter("@CompanyIds", CompanyIds)
                                                 };
                return context.Database.SqlQuery<RoleDetailsRoom>("exec [GetRoomsByEntIdsAndCmpIdsNormal] @EnterpriseIds,@CompanyIds", params1).ToList();

            }
        }

        public RoomDTO GetRoomById(RolePermissionInfo objRolePermissionInfo)
        {
            EnterpriseDTO objEnterprise = new EnterpriseDTO();
            RoomDTO objRoomDTO = new RoomDTO();
            objEnterprise = GetEnterpriseByIdPlain(objRolePermissionInfo.EnterPriseId);

            if (objEnterprise != null)
            {
                objRoomDTO.EnterpriseId = objEnterprise.ID;
                objRoomDTO.EnterpriseName = objEnterprise.Name;

                if (!string.IsNullOrWhiteSpace(objEnterprise.EnterpriseDBName))
                {
                    DataSet dsRooms = new DataSet();
                    string EnterpriseDbConnection = MasterDbConnectionHelper.GeteTurnsSQLConnectionString(objEnterprise.EnterpriseDBName, MasterDbConnectionType.GeneralReadWrite.ToString("F"));
                    dsRooms = SqlHelper.ExecuteDataset(EnterpriseDbConnection, "GetRoomByIDNormal", objRolePermissionInfo.RoomId);

                    if (dsRooms != null && dsRooms.Tables.Count > 0)
                    {
                        DataTable dtRooms = dsRooms.Tables[0];
                        var room = DataTableHelper.ToList<RoomDTO>(dsRooms.Tables[0]).FirstOrDefault();

                        if (room != null && room.ID > 0)
                        {
                            objRoomDTO.ID = room.ID;
                            objRoomDTO.CompanyID = room.CompanyID;
                            objRoomDTO.RoomName = room.RoomName;
                            objRoomDTO.CompanyName = room.CompanyName;
                            objRoomDTO.IsRoomActive = room.IsRoomActive;
                            objRoomDTO.IsCompanyActive = room.IsCompanyActive;
                        }
                        return objRoomDTO;
                    }
                }
            }
            return null;
        }

        public List<EnterpriseDTO> GetPagedEnterprises(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string sortColumnName, bool IsArchived, bool IsDeleted, List<long> EnterpriseFilter, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            List<EnterpriseDTO> enterprises = new List<EnterpriseDTO>();
            TotalCount = 0;

            string UserCreaters = string.Empty;
            string UserUpdators = string.Empty;
            string CreatedDateFrom = string.Empty;
            string CreatedDateTo = string.Empty;
            string UpdatedDateFrom = string.Empty;
            string UpdatedDateTo = string.Empty;
            string EnterpriseIds = string.Empty;

            if (EnterpriseFilter != null && EnterpriseFilter.Any() && EnterpriseFilter.Count > 0)
            {
                EnterpriseIds = string.Join(",", EnterpriseFilter);
            }

            if (!string.IsNullOrEmpty(SearchTerm) && SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');

                if (Fields.Length > 2)
                {
                    if (!string.IsNullOrEmpty(Fields[2]))
                        SearchTerm = Fields[2];
                    else
                        SearchTerm = string.Empty;
                }
                else
                {
                    SearchTerm = string.Empty;
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    UserCreaters = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    UserUpdators = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
            }

            var params1 = new SqlParameter[]
                        {
                                new SqlParameter("@StartRowIndex", StartRowIndex)
                              , new SqlParameter("@MaxRows", MaxRows)
                              , new SqlParameter("@SearchTerm", SearchTerm??string.Empty)
                              , new SqlParameter("@sortColumnName", sortColumnName)
                              , new SqlParameter("@UserCreaters", UserCreaters)
                              , new SqlParameter("@UserUpdators", UserUpdators)
                              , new SqlParameter("@CreatedDateFrom", CreatedDateFrom)
                              , new SqlParameter("@CreatedDateTo", CreatedDateTo)
                              , new SqlParameter("@UpdatedDateFrom", UpdatedDateFrom)
                              , new SqlParameter("@UpdatedDateTo", UpdatedDateTo)
                              , new SqlParameter("@IsDeleted", IsDeleted)
                              , new SqlParameter("@IsArchived", IsArchived)
                              , new SqlParameter("@EnterpriseIds", EnterpriseIds)
                        };

            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                enterprises = context.Database.SqlQuery<EnterpriseDTO>("exec GetPagedEnterprises @StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@UserCreaters,@UserUpdators,@CreatedDateFrom,@CreatedDateTo,@UpdatedDateFrom,@UpdatedDateTo,@IsDeleted,@IsArchived,@EnterpriseIds", params1).ToList();

                if (enterprises != null && enterprises.Count > 0)
                {
                    TotalCount = enterprises.First().TotalRecords.GetValueOrDefault(0);
                }
            }

            return enterprises;
        }

        public bool UpdateLogoName(long Id, string fileName)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                EnterpriseMaster objEnterpriseMaster = context.EnterpriseMasters.FirstOrDefault(t => t.ID == Id);
                if (objEnterpriseMaster != null)
                {
                    objEnterpriseMaster.EnterPriseLogo = fileName;
                    context.SaveChanges();
                }
            }
            return true;
        }

        public long InsertScript(EnterPriseSQLScriptsDTO objDTO)
        {
            EnterpriseScriptsMaster objEnterpriseScriptsMaster = new EnterpriseScriptsMaster();
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                objEnterpriseScriptsMaster.CreatedBy = objDTO.CreatedBy;
                objEnterpriseScriptsMaster.CreatedDate = DateTime.UtcNow;
                objEnterpriseScriptsMaster.IsDeleted = objDTO.IsDeleted;
                objEnterpriseScriptsMaster.IsMasterScript = false;
                objEnterpriseScriptsMaster.IsSQLCEScriptTemplate = false;
                objEnterpriseScriptsMaster.ScriptName = objDTO.ScriptName;
                objEnterpriseScriptsMaster.ScriptText = objDTO.ScriptText;
                objEnterpriseScriptsMaster.Version = string.Empty;
                objEnterpriseScriptsMaster.UpdatedBy = objDTO.UpdatedBy;
                objEnterpriseScriptsMaster.UpdatedDate = DateTime.UtcNow;
                context.EnterpriseScriptsMasters.Add(objEnterpriseScriptsMaster);
                context.SaveChanges();
            }
            return objEnterpriseScriptsMaster.SQLScriptID;
        }

        public long UpdateScripts(EnterPriseSQLScriptsDTO objDTO)
        {
            EnterpriseScriptsMaster objEnterpriseScriptsMaster = new EnterpriseScriptsMaster();
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                objEnterpriseScriptsMaster = context.EnterpriseScriptsMasters.FirstOrDefault(t => t.SQLScriptID == objDTO.SQLScriptID);
                if (objEnterpriseScriptsMaster != null)
                {
                    objEnterpriseScriptsMaster.CreatedBy = objDTO.CreatedBy;
                    objEnterpriseScriptsMaster.CreatedDate = DateTime.UtcNow;
                    objEnterpriseScriptsMaster.IsDeleted = objDTO.IsDeleted;
                    objEnterpriseScriptsMaster.IsMasterScript = false;
                    objEnterpriseScriptsMaster.IsSQLCEScriptTemplate = false;
                    objEnterpriseScriptsMaster.ScriptName = objDTO.ScriptName;
                    objEnterpriseScriptsMaster.ScriptText = objDTO.ScriptText;
                    objEnterpriseScriptsMaster.Version = string.Empty;
                    context.SaveChanges();
                }
            }
            return objEnterpriseScriptsMaster.SQLScriptID;
        }

        public List<UserAccessDTO> GetUserAccessWithNames(long UserId)
        {
            List<UserAccessDTO> UserRoomAccess = new UserMasterDAL().GetUserRoomAccessByUserIdPlain(UserId);
            List<UserAccessDTO> lstUserAccess = new List<UserAccessDTO>();
            EnterpriseDTO objEnterprise = new EnterpriseDTO();
            UserAccessDTO objUserAccessDTO = new UserAccessDTO();

            if (UserRoomAccess != null && UserRoomAccess.Count > 0)
            {
                string enterpriseIds = string.Join(",", UserRoomAccess.Where(t => t.EnterpriseId > 0 && t.CompanyId > 0 && t.RoomId > 0).Select(t => t.EnterpriseId).Distinct().ToArray());
                var enterprises = GetEnterprisesByIds(enterpriseIds);

                foreach (var item in UserRoomAccess.Where(t => t.EnterpriseId > 0 && t.CompanyId > 0 && t.RoomId > 0).Select(t => t.EnterpriseId).Distinct())
                {
                    objEnterprise = new EnterpriseDTO();
                    objEnterprise = enterprises.Where(e => e.ID == item).FirstOrDefault();

                    if (objEnterprise != null && objEnterprise.ID > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(objEnterprise.EnterpriseDBName))
                        {
                            DataSet dsRooms = new DataSet();
                            string EnterpriseDbConnection = MasterDbConnectionHelper.GeteTurnsSQLConnectionString(objEnterprise.EnterpriseDBName, MasterDbConnectionType.GeneralReadWrite.ToString("F"));
                            string Rmids = string.Join(",", UserRoomAccess.Where(t => t.EnterpriseId > 0 && t.CompanyId > 0 && t.RoomId > 0 && t.EnterpriseId == item).Select(t => t.RoomId).ToArray());
                            dsRooms = SqlHelper.ExecuteDataset(EnterpriseDbConnection, "GetCompaniesAndRoomsByRoomIdsNormal", Rmids);

                            if (dsRooms != null && dsRooms.Tables.Count > 0)
                            {
                                var rooms = DataTableHelper.ToList<UserAccessDTO>(dsRooms.Tables[0]);

                                if (rooms != null && rooms.Any())
                                {
                                    rooms.ForEach(u =>
                                    {
                                        u.EnterpriseId = objEnterprise.ID;
                                        u.EnterpriseName = objEnterprise.Name;
                                        u.IsEnterpriseActive = objEnterprise.IsActive;
                                    });
                                    lstUserAccess.AddRange(rooms);
                                }
                            }
                        }
                    }
                }
                foreach (var item in UserRoomAccess.Where(t => t.EnterpriseId > 0 && t.CompanyId > 0 && t.RoomId == 0).Select(t => t.EnterpriseId).Distinct())
                {
                    objEnterprise = new EnterpriseDTO();
                    objEnterprise = enterprises.Where(e => e.ID == item).FirstOrDefault();

                    if (objEnterprise != null && objEnterprise.ID > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(objEnterprise.EnterpriseDBName))
                        {
                            DataSet dsCompanies = new DataSet();
                            string EnterpriseDbConnection = MasterDbConnectionHelper.GeteTurnsSQLConnectionString(objEnterprise.EnterpriseDBName, MasterDbConnectionType.GeneralReadWrite.ToString("F"));
                            string cmids = string.Join(",", UserRoomAccess.Where(t => t.EnterpriseId > 0 && t.CompanyId > 0 && t.RoomId == 0 && t.EnterpriseId == item).Select(t => t.CompanyId).ToArray());
                            dsCompanies = SqlHelper.ExecuteDataset(EnterpriseDbConnection, "GetCompaniesByIdsNormal", cmids);

                            if (dsCompanies != null && dsCompanies.Tables.Count > 0)
                            {
                                var companies = DataTableHelper.ToList<UserAccessDTO>(dsCompanies.Tables[0]);

                                if (companies != null && companies.Any())
                                {
                                    companies.ForEach(u =>
                                    {
                                        u.EnterpriseId = objEnterprise.ID;
                                        u.EnterpriseName = objEnterprise.Name;
                                        u.IsEnterpriseActive = objEnterprise.IsActive;
                                    });
                                    lstUserAccess.AddRange(companies);
                                }
                            }
                        }
                    }
                }
                foreach (var item in UserRoomAccess.Where(t => t.EnterpriseId > 0 && t.CompanyId == 0 && t.RoomId == 0).Select(t => t.EnterpriseId).Distinct())
                {
                    objEnterprise = new EnterpriseDTO();
                    objEnterprise = enterprises.Where(e => e.ID == item).FirstOrDefault();  // //objEnterprise = GetEnterpriseByIdPlain(item);
                    objUserAccessDTO = new UserAccessDTO();

                    if (objEnterprise != null && objEnterprise.ID > 0)
                    {
                        objUserAccessDTO.EnterpriseId = objEnterprise.ID;
                        objUserAccessDTO.EnterpriseName = objEnterprise.Name;
                        objUserAccessDTO.IsEnterpriseActive = objEnterprise.IsActive;
                        lstUserAccess.Add(objUserAccessDTO);
                    }
                }
            }
            return lstUserAccess;
        }

        public List<UserAccessDTO> GetRoleAccessWithNames(long RoleId)
        {
            List<UserAccessDTO> RoleRoomAccess = new UserMasterDAL().GetRoleRoomAccessByRoleIdPlain(RoleId);
            List<UserAccessDTO> lstUserAccess = new List<UserAccessDTO>();
            EnterpriseDTO objEnterprise = new EnterpriseDTO();
            UserAccessDTO objUserAccessDTO = new UserAccessDTO();

            if (RoleRoomAccess != null && RoleRoomAccess.Count > 0)
            {
                string enterpriseIds = string.Join(",", RoleRoomAccess.Where(t => t.EnterpriseId > 0 && t.CompanyId > 0 && t.RoomId > 0).Select(t => t.EnterpriseId).Distinct().ToArray());
                var enterprises = GetEnterprisesByIds(enterpriseIds);

                foreach (var item in RoleRoomAccess.Where(t => t.EnterpriseId > 0 && t.CompanyId > 0 && t.RoomId > 0).Select(t => t.EnterpriseId).Distinct())
                {
                    objEnterprise = new EnterpriseDTO();
                    objEnterprise = enterprises.Where(e => e.ID == item).FirstOrDefault();

                    if (objEnterprise != null)
                    {
                        if (!string.IsNullOrWhiteSpace(objEnterprise.EnterpriseDBName))
                        {
                            DataSet dsRooms = new DataSet();
                            string EnterpriseDbConnection = MasterDbConnectionHelper.GeteTurnsSQLConnectionString(objEnterprise.EnterpriseDBName, MasterDbConnectionType.GeneralReadWrite.ToString("F"));
                            string Rmids = string.Join(",", RoleRoomAccess.Where(t => t.EnterpriseId > 0 && t.CompanyId > 0 && t.RoomId > 0 && t.EnterpriseId == item).Select(t => t.RoomId).ToArray());
                            //dsRooms = SqlHelper.ExecuteDataset(EnterpriseDbConnection, CommandType.Text, "select rm.ID as RoomId,rm.IsRoomActive,rm.RoomName,cm.ID as CompanyId,cm.Name as CompanyName,cm.IsActive as IsCompanyActive,isnull(rm.IsDeleted,0) as IsRoomDeleted,isnull(cm.IsDeleted,0) as IsCompanyDeleted from Room as rm inner join CompanyMaster as cm on rm.CompanyID = cm.ID Where rm.ID in (" + Rmids + ")");
                            dsRooms = SqlHelper.ExecuteDataset(EnterpriseDbConnection, "GetCompaniesAndRoomsByRoomIdsNormal", Rmids);

                            if (dsRooms != null && dsRooms.Tables.Count > 0)
                            {
                                var rooms = DataTableHelper.ToList<UserAccessDTO>(dsRooms.Tables[0]);

                                if (rooms != null && rooms.Any())
                                {
                                    rooms.ForEach(u =>
                                    {
                                        u.EnterpriseId = objEnterprise.ID;
                                        u.EnterpriseName = objEnterprise.Name;
                                        u.IsEnterpriseActive = objEnterprise.IsActive;
                                    });
                                    lstUserAccess.AddRange(rooms);
                                }
                            }
                        }
                    }
                }
                foreach (var item in RoleRoomAccess.Where(t => t.EnterpriseId > 0 && t.CompanyId > 0 && t.RoomId == 0).Select(t => t.EnterpriseId).Distinct())
                {
                    objEnterprise = new EnterpriseDTO();
                    objEnterprise = enterprises.Where(e => e.ID == item).FirstOrDefault();

                    if (objEnterprise != null)
                    {
                        if (!string.IsNullOrWhiteSpace(objEnterprise.EnterpriseDBName))
                        {
                            DataSet dsCompanies = new DataSet();
                            string EnterpriseDbConnection = MasterDbConnectionHelper.GeteTurnsSQLConnectionString(objEnterprise.EnterpriseDBName, MasterDbConnectionType.GeneralReadWrite.ToString("F"));
                            string cmids = string.Join(",", RoleRoomAccess.Where(t => t.EnterpriseId > 0 && t.CompanyId > 0 && t.RoomId == 0 && t.EnterpriseId == item).Select(t => t.CompanyId).ToArray());
                            //dsCompanies = SqlHelper.ExecuteDataset(EnterpriseDbConnection, CommandType.Text, "Select ID as CompanyId,Name as CompanyName,IsActive as IsCompanyActive,ISNULL(isdeleted,0) as IsCompanyDeleted from CompanyMaster Where ID in (" + cmids + ")");
                            dsCompanies = SqlHelper.ExecuteDataset(EnterpriseDbConnection, "GetCompaniesByIdsNormal", cmids);

                            if (dsCompanies != null && dsCompanies.Tables.Count > 0)
                            {
                                var companies = DataTableHelper.ToList<UserAccessDTO>(dsCompanies.Tables[0]);

                                if (companies != null && companies.Any())
                                {
                                    companies.ForEach(u =>
                                    {
                                        u.EnterpriseId = objEnterprise.ID;
                                        u.EnterpriseName = objEnterprise.Name;
                                        u.IsEnterpriseActive = objEnterprise.IsActive;
                                    });
                                    lstUserAccess.AddRange(companies);
                                }
                            }
                        }
                    }
                }
                foreach (var item in RoleRoomAccess.Where(t => t.EnterpriseId > 0 && t.CompanyId == 0 && t.RoomId == 0).Select(t => t.EnterpriseId).Distinct())
                {
                    objEnterprise = new EnterpriseDTO();
                    objEnterprise = enterprises.Where(e => e.ID == item).FirstOrDefault();
                    objUserAccessDTO = new UserAccessDTO();

                    if (objEnterprise != null)
                    {
                        objUserAccessDTO.EnterpriseId = objEnterprise.ID;
                        objUserAccessDTO.EnterpriseName = objEnterprise.Name;
                        objUserAccessDTO.IsEnterpriseActive = objEnterprise.IsActive;
                        lstUserAccess.Add(objUserAccessDTO);
                    }
                }
            }
            return lstUserAccess;
        }

        public List<UserAccessDTO> GetRoleAccessWithNamesNew(long RoleId)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                List<UserAccessDTO> lstRoleAccess = context.Database.SqlQuery<UserAccessDTO>("EXEC [dbo].[GetRoleAccessWithNames] " + RoleId + "").ToList();
                return lstRoleAccess;
            }
        }

        public List<UserAccessDTO> GetUserAccessWithNames(List<UserAccessDTO> UserRoomAccess)
        {
            List<UserAccessDTO> lstUserAccess = new List<UserAccessDTO>();
            EnterpriseDTO objEnterprise = new EnterpriseDTO();
            UserAccessDTO objUserAccessDTO = new UserAccessDTO();

            if (UserRoomAccess != null && UserRoomAccess.Count > 0)
            {
                string enterpriseIds = string.Join(",", UserRoomAccess.Where(t => t.EnterpriseId > 0 && t.CompanyId > 0 && t.RoomId > 0).Select(t => t.EnterpriseId).Distinct().ToArray());
                var enterprises = GetEnterprisesByIds(enterpriseIds);

                foreach (var item in UserRoomAccess.Where(t => t.EnterpriseId > 0 && t.CompanyId > 0 && t.RoomId > 0).Select(t => t.EnterpriseId).Distinct())
                {
                    objEnterprise = new EnterpriseDTO();
                    objEnterprise = enterprises.Where(e => e.ID == item).FirstOrDefault();

                    if (objEnterprise != null)
                    {
                        if (!string.IsNullOrWhiteSpace(objEnterprise.EnterpriseDBName))
                        {
                            DataSet dsRooms = new DataSet();
                            string EnterpriseDbConnection = MasterDbConnectionHelper.GeteTurnsSQLConnectionString(objEnterprise.EnterpriseDBName, MasterDbConnectionType.GeneralReadWrite.ToString("F"));
                            string Rmids = string.Join(",", UserRoomAccess.Where(t => t.EnterpriseId > 0 && t.CompanyId > 0 && t.RoomId > 0 && t.EnterpriseId == item).Select(t => t.RoomId).ToArray());
                            //dsRooms = SqlHelper.ExecuteDataset(EnterpriseDbConnection, CommandType.Text, "select rm.ID as RoomId,rm.IsRoomActive,rm.RoomName,cm.ID as CompanyId,cm.Name as CompanyName,cm.IsActive as IsCompanyActive,isnull(rm.IsDeleted,0) as IsRoomDeleted,isnull(cm.IsDeleted,0) as IsCompanyDeleted from Room as rm inner join CompanyMaster as cm on rm.CompanyID = cm.ID Where rm.ID in (" + Rmids + ")");
                            dsRooms = SqlHelper.ExecuteDataset(EnterpriseDbConnection, "GetCompaniesAndRoomsByRoomIdsNormal", Rmids);

                            if (dsRooms != null && dsRooms.Tables.Count > 0)
                            {
                                var rooms = DataTableHelper.ToList<UserAccessDTO>(dsRooms.Tables[0]);

                                if (rooms != null && rooms.Any())
                                {
                                    rooms.ForEach(u =>
                                    {
                                        u.EnterpriseId = objEnterprise.ID;
                                        u.EnterpriseName = objEnterprise.Name;
                                        u.IsEnterpriseActive = objEnterprise.IsActive;
                                    });
                                    lstUserAccess.AddRange(rooms);
                                }
                            }
                        }
                    }
                }
                foreach (var item in UserRoomAccess.Where(t => t.EnterpriseId > 0 && t.CompanyId > 0 && t.RoomId == 0).Select(t => t.EnterpriseId).Distinct())
                {
                    objEnterprise = new EnterpriseDTO();
                    objEnterprise = enterprises.Where(e => e.ID == item).FirstOrDefault();

                    if (objEnterprise != null)
                    {
                        if (!string.IsNullOrWhiteSpace(objEnterprise.EnterpriseDBName))
                        {
                            DataSet dsCompanies = new DataSet();
                            string EnterpriseDbConnection = MasterDbConnectionHelper.GeteTurnsSQLConnectionString(objEnterprise.EnterpriseDBName, MasterDbConnectionType.GeneralReadWrite.ToString("F"));
                            string cmids = string.Join(",", UserRoomAccess.Where(t => t.EnterpriseId > 0 && t.CompanyId > 0 && t.RoomId == 0 && t.EnterpriseId == item).Select(t => t.CompanyId).ToArray());
                            //dsCompanies = SqlHelper.ExecuteDataset(EnterpriseDbConnection, CommandType.Text, "Select ID as CompanyId,Name as CompanyName,IsActive as IsCompanyActive,ISNULL(isdeleted,0) as IsCompanyDeleted from CompanyMaster Where ID in (" + cmids + ")");
                            dsCompanies = SqlHelper.ExecuteDataset(EnterpriseDbConnection, "GetCompaniesByIdsNormal", cmids);

                            if (dsCompanies != null && dsCompanies.Tables.Count > 0)
                            {
                                var companies = DataTableHelper.ToList<UserAccessDTO>(dsCompanies.Tables[0]);

                                if (companies != null && companies.Any())
                                {
                                    companies.ForEach(u =>
                                    {
                                        u.EnterpriseId = objEnterprise.ID;
                                        u.EnterpriseName = objEnterprise.Name;
                                        u.IsEnterpriseActive = objEnterprise.IsActive;
                                    });
                                    lstUserAccess.AddRange(companies);
                                }
                            }
                        }
                    }
                }
                foreach (var item in UserRoomAccess.Where(t => t.EnterpriseId > 0 && t.CompanyId == 0 && t.RoomId == 0).Select(t => t.EnterpriseId).Distinct())
                {
                    objEnterprise = new EnterpriseDTO();
                    objEnterprise = enterprises.Where(e => e.ID == item).FirstOrDefault();
                    objUserAccessDTO = new UserAccessDTO();

                    if (objEnterprise != null)
                    {
                        objUserAccessDTO.EnterpriseId = objEnterprise.ID;
                        objUserAccessDTO.EnterpriseName = objEnterprise.Name;
                        objUserAccessDTO.IsEnterpriseActive = objEnterprise.IsActive;
                        lstUserAccess.Add(objUserAccessDTO);
                    }
                }
            }

            return lstUserAccess;
        }

        public bool UpdateRDLCReportMaster(string MasterDBName, string ChildDBName, long UserId, bool OverwriteExisting)
        {
            string Connectionstring = MasterDbConnectionHelper.GeteTurnsSQLConnectionString(MasterDBName, MasterDbConnectionType.GeneralReadWrite.ToString("F"));

            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            SqlHelper.ExecuteNonQuery(EturnsConnection, "RPT_UpdateReportMaster", "[" + MasterDBName + "]", "[" + ChildDBName + "]", UserId, OverwriteExisting);
            return true;
        }

        public bool UpdateRDLCReportMasterByReportFile(string MasterDBName, string ChildDBName, long UserId, bool OverwriteExisting, string ReportFiles)
        {
            string Connectionstring = MasterDbConnectionHelper.GeteTurnsSQLConnectionString(MasterDBName, MasterDbConnectionType.GeneralReadWrite.ToString("F"));

            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            SqlHelper.ExecuteNonQuery(EturnsConnection, "RPT_UpdateReportMasterByReportFileName", "[" + MasterDBName + "]", "[" + ChildDBName + "]", UserId, OverwriteExisting, ReportFiles);
            return true;
        }

        public bool UpdateResource(string MasterDBName, string ChildDBName)
        {
            string Connectionstring = MasterDbConnectionHelper.GeteTurnsSQLConnectionString(MasterDBName, MasterDbConnectionType.GeneralReadWrite.ToString("F"));
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            SqlHelper.ExecuteNonQuery(EturnsConnection, "USP_MobileResourceManagement", "[" + MasterDBName + "]", "[" + ChildDBName + "]");
            return true;
        }

        public ForgotPasswordRequest SaveForgotPasswordRequest(ForgotPasswordRequest objForgotPasswordRequest)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                ForgotPasswordRequest objForgotPasswordRequestDB = context.ForgotPasswordRequests.FirstOrDefault(t => t.ID == objForgotPasswordRequest.ID);
                if (objForgotPasswordRequestDB != null)
                {
                    objForgotPasswordRequestDB.IsExpired = objForgotPasswordRequest.IsExpired;
                    objForgotPasswordRequestDB.IsProcessed = objForgotPasswordRequest.IsProcessed;
                }
                else
                {
                    objForgotPasswordRequestDB = new ForgotPasswordRequest
                    {
                        IsExpired = objForgotPasswordRequest.IsExpired,
                        IsProcessed = objForgotPasswordRequest.IsProcessed,
                        RequestToken = objForgotPasswordRequest.RequestToken,
                        TokenGeneratedDate = objForgotPasswordRequest.TokenGeneratedDate,
                        UserId = objForgotPasswordRequest.UserId
                    };
                    context.ForgotPasswordRequests.Add(objForgotPasswordRequestDB);
                }
                context.SaveChanges();
                return objForgotPasswordRequest;
            }
        }
        public ForgotPasswordRequest GetForgotPasswordRequestByTokenPlain(Guid Token)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@RequestToken", Token)
                                                 };
                return context.Database.SqlQuery<ForgotPasswordRequest>("exec [GetForgotPasswordRequestByTokenPlain] @RequestToken", params1).FirstOrDefault();
            }
        }

        public List<EnterpriseDomainDTO> GetAllEnterpriseDomains()
        {
            List<EnterpriseDomainDTO> ObjCache = CacheHelper<List<EnterpriseDomainDTO>>.GetCacheItem("AllEPDomains");
            if (ObjCache == null)
            {
                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    ObjCache = (from ed in context.EnterpriseDomains
                                join em in context.EnterpriseMasters on ed.EnterpriseID equals em.ID
                                select new EnterpriseDomainDTO
                                {
                                    Created = ed.Created,
                                    CreatedBy = ed.CreatedBy,
                                    DomainName = ed.DomainName,
                                    DomainURL = ed.DomainURL,
                                    EnterpriseID = ed.EnterpriseID,
                                    EnterpriseName = em.Name,
                                    EpUniqueKey = ed.EpUniqueKey,
                                    GUID = ed.GUID,
                                    ID = ed.ID,
                                    RedirectDomainURL = ed.RedirectDomainURL,
                                    Updated = ed.Updated,
                                    UpdatedBy = ed.UpdatedBy,
                                    IsSecureOnly = ed.IsSecureOnly
                                }).ToList();

                    CacheHelper<IEnumerable<EnterpriseDomainDTO>>.AddCacheItem("AllEPDomains", ObjCache);
                }
            }

            return ObjCache;
        }

        public List<RoomDTO> GetAllRoomsNormal()
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RoomDTO>("exec [GetAllRoomsNormal] ").ToList();
            }
        }

        public List<RoomDTO> GetAllRoomsByEnterpriseIdNormal(long EnterpriseId)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@EnterpriseId", EnterpriseId)
                                                 };
                return context.Database.SqlQuery<RoomDTO>("exec [GetAllRoomsByEnterpriseIdNormal] @EnterpriseId ", params1).ToList();
            }
        }

        public List<UserAccessDTO> GetSuperUserPermissionsNormal(UserMasterDTO objDTO)
        {
            List<UserAccessDTO> lstPermissions = new List<UserAccessDTO>();

            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@UserId", objDTO.ID)
                                                 };
                lstPermissions = context.Database.SqlQuery<UserAccessDTO>("exec [GetSuperUserPermissionsNormal] @UserId ", params1).ToList();

                if (lstPermissions != null && lstPermissions.Any())
                {
                    lstPermissions.ForEach(u =>
                    {
                        u.RoleId = objDTO.RoleID;
                        u.RoleName = objDTO.RoleName;
                        u.UserName = objDTO.UserName;
                        u.UserType = objDTO.UserType;
                    });
                }
            }

            return lstPermissions;
        }

        public bool DeleteEnterpriseScriptsMaster(string IDs, long UserId)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@SQLScriptIDs", IDs), new SqlParameter("@UserId", UserId) };
                context.Database.ExecuteSqlCommand("exec [DeleteEnterpriseScriptsMaster] @SQLScriptIDs, @UserId", params1);
                return true;
            }
        }

        public string ExecuteNonQueryScript(string script, string dbName, long userId)
        {
            List<object> obj = new List<object>();
            string EnterpriseDbConnection = MasterDbConnectionHelper.GeteTurnsSQLConnectionString(dbName, MasterDbConnectionType.GeneralReadWrite.ToString("F"));
            try
            {
                EnterpriseDTO enterpriseObj = new EnterpriseDTO();
                int recordsAffected = 0;
                {
                    recordsAffected = SqlHelper.ExecuteNonQuery(EnterpriseDbConnection, "ExecuteScript", script, dbName, userId);
                }
                object result = new object();
                return recordsAffected + " Rows Effected";
            }
            catch (Exception ex)
            {
                return (ex.Message.ToString());
            }
            finally
            {
                // conn.Close();
            }

        }
        public List<object> ExecuteReaderScript(string script, string dbName, long userId, ref List<object> columns, ref string message)
        {
            List<string> obj = new List<string>();
            string EnterpriseDbConnection = MasterDbConnectionHelper.GeteTurnsSQLConnectionString(dbName, MasterDbConnectionType.GeneralReadWrite.ToString("F"));
            try
            {
                DataTable dt = SqlHelper.ExecuteDataset(EnterpriseDbConnection, "ExecuteScript", script, dbName, userId).Tables[0];
                DataTableReader rdr = dt.CreateDataReader();
                for (int i = 0; i < rdr.FieldCount; i++)
                {
                    columns.Add(rdr.GetName(i));
                }
                return dt.AsEnumerable().ToList<object>();
            }
            catch (Exception ex)
            {
                message = ex.Message.ToString();
                return null;
            }
            finally
            {
            }
        }

        public Dictionary<string, string> GetTablesList(string DataBaseName)
        {
            Dictionary<string, string> TableList = new Dictionary<string, string>();
            string EnterpriseDbConnection = MasterDbConnectionHelper.GeteTurnsSQLConnectionString(DataBaseName, MasterDbConnectionType.GeneralReadWrite.ToString("F"));
            DataTable dt = SqlHelper.ExecuteDataset(EnterpriseDbConnection, "GetTables", DataBaseName).Tables[0];
            TableList = dt.AsEnumerable().ToDictionary<DataRow, string, string>(row => row.Field<string>(0),
                                  row => row.Field<string>(0));
            return TableList;
        }
        public Dictionary<string, string> GetColumnList(string DataBaseName, string TableName)
        {
            Dictionary<string, string> TableList = new Dictionary<string, string>();
            string EnterpriseDbConnection = MasterDbConnectionHelper.GeteTurnsSQLConnectionString(DataBaseName, MasterDbConnectionType.GeneralReadWrite.ToString("F"));
            DataTable dt = SqlHelper.ExecuteDataset(EnterpriseDbConnection, "GetColumns", DataBaseName, TableName).Tables[0];
            TableList = dt.AsEnumerable().ToDictionary<DataRow, string, string>(row => row.Field<string>(0),
                                  row => row.Field<string>(0));
            return TableList;
        }

        public List<EnterpriseSuperAdmin> SetEnterpriseSuperAdmin(List<EnterpriseSuperAdmin> lstAdmins)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                bool IsSave = false;
                foreach (var item in lstAdmins)
                {
                    UserMaster objUserMasterDTO = context.UserMasters.FirstOrDefault(t => t.UserName == item.UserName && t.IsDeleted == false);
                    if (objUserMasterDTO != null)
                    {
                        if (!item.MarkDeleted && objUserMasterDTO.RoleId != item.RoleID)
                        {
                            if (item.IsEPSuperAdmin)
                            {
                                objUserMasterDTO.RoleId = -2;
                            }
                            else
                            {
                                objUserMasterDTO.RoleId = item.RoleID;
                            }
                            IsSave = true;
                        }
                    }
                }
                if (IsSave)
                {
                    context.SaveChanges();
                }
            }
            return lstAdmins;
        }

        #region [IDisposable interface]

        bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here. 
                //
            }

            // Free any unmanaged objects here. 
            //
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
        public long GetEnterpriseIdByUserID(Int64 UserId)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Id", UserId) };
                return context.Database.SqlQuery<UserMasterDTO>("exec [GetUserByIdPlain] @Id", params1).FirstOrDefault().EnterpriseId;
            }
        }

        public EnterpriseDTO GetEnterpriseByUserIdNormal(long UserId)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@UserId", UserId),
                                                 };
                return context.Database.SqlQuery<EnterpriseDTO>("exec [GetEnterpriseByUserIdNormal] @UserId ", params1).FirstOrDefault();
            }
        }

        public List<MstCompanyMaster> GetCompaniesByName(string Companies, long EnterpriseID, string EnterpriseName)
        {
            var params1 = new SqlParameter[] {
                                               new SqlParameter("@EnterpriseID", EnterpriseID),
                                               new SqlParameter("@EnterpriseName", EnterpriseName ?? (object)DBNull.Value),
                                               new SqlParameter("@CompanyNames", Companies ?? (object)DBNull.Value)
                                            };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<MstCompanyMaster>("exec [GetCompaniesByName] @EnterpriseID,@EnterpriseName,@CompanyNames", params1).ToList();
            }
        }

        public List<MstCompanyMaster> GetCompaniesByEnterPriseID(long EnterpriseID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@EnterpriseID", EnterpriseID) };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<MstCompanyMaster>("exec [GetCompaniesByEnterpriseID] @EnterpriseID", params1).ToList();
            }
        }

        public List<MstRoom> GetRoomsByName(string RoomNames, long EnterpriseID, string EnterpriseName)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@EnterpriseID", EnterpriseID), new SqlParameter("@EnterpriseName", EnterpriseName ?? (object)DBNull.Value), new SqlParameter("@RoomNames", RoomNames ?? (object)DBNull.Value) };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<MstRoom>("exec [GetRoomsByName] @EnterpriseID,@EnterpriseName,@RoomNames", params1).ToList();
            }
        }
        public List<EnterpriseDTO> GetEnterprisesByIds(string EnterpriseIds)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@EnterpriseIds", EnterpriseIds ?? (object)DBNull.Value) };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<EnterpriseDTO>("exec [GetEnterprisesByIds] @EnterpriseIds", params1).ToList();
            }
        }

        public List<NotificationMasterDTO> GetAllStockOutScheduleEnterprises()
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<NotificationMasterDTO>("exec [GetAllStockOutScheduleEnterprises]").ToList();
            }
        }

        public string GetEnterpriseDBNameByID(long EnterpriseID)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Id", EnterpriseID) };
                return context.Database.SqlQuery<EnterpriseDTO>("exec [GetNonDeletedEnterpriseByIdPlain] @Id", params1).FirstOrDefault().EnterpriseDBName;
            }
        }

        public bool RemoveEnterpriseImage(Guid EnterpriseGUID, string EditedFrom, Int64 UserID)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                bool bResult = false;
                try
                {
                    string strQuery = "EXEC [dbo].[RemoveEnterpriseImage] '" + Convert.ToString(EnterpriseGUID) + "','" + EditedFrom + "'," + UserID + "";
                    context.Database.ExecuteSqlCommand(strQuery);
                    bResult = true;
                }
                catch
                {
                    bResult = false;
                }
                return bResult;
            }

        }
        public List<EnterpriseDTO> GetAllExportLiveDBs()
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<EnterpriseDTO>("exec [GetAllExportLiveDBs]").ToList();
            }
        }
        public List<EVMIRoomDTO> GetEVMIRooms(string _eVMIServer = "1", string eVMIEnterpriseList = "")
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@eVMIServer", _eVMIServer),
                                                   new SqlParameter("@eVMIEnterpriseList", eVMIEnterpriseList)
                                                 };
                var list = context.Database.SqlQuery<EVMIRoomDTO>("exec [GetEVMIRooms] @eVMIServer, @eVMIEnterpriseList", params1).ToList();
                return list;
            }
        }

        public List<EnterpriseDTO> GetEnterpriseHistoryFromHistoryID(Int64 HistoryID)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@HistoryID", HistoryID) };
                return context.Database.SqlQuery<EnterpriseDTO>("exec [GetEnterpriseHistoryFromHistoryID] @HistoryID", params1).ToList();
            }
        }

        public List<CompanyMasterDTO> GetCompanyHistoryFromHistoryID(Int64 HistoryID)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@HistoryID", HistoryID) };
                return context.Database.SqlQuery<CompanyMasterDTO>("exec [GetCompanyHistoryFromHistoryID] @HistoryID", params1).ToList();
            }
        }
        public List<RoomDTO> GetRoomyHistoryFromHistoryID(Int64 HistoryID)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@HistoryID", HistoryID) };
                return context.Database.SqlQuery<RoomDTO>("exec [GetRoomyHistoryFromHistoryID] @HistoryID", params1).ToList();
            }
        }

        public int GetCountOfABIntegrationRoomByEnterpriseId(long EnterpriseId)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@EnterpriseId", EnterpriseId) };
                return context.Database.SqlQuery<int>("exec [GetCountOfABIntegrationRoomByEnterpriseId] @EnterpriseId", params1).FirstOrDefault();
            }
        }

        public string ActivateUndeletedEnterpriseDB()
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    var params1 = new SqlParameter[] { };
                    return context.Database.SqlQuery<string>("exec [CheckUndeleteEnterpriseAndActiveDB] ", params1).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
        }
        public List<DateTime> GetRoomCountMonthYear_InventoryMonthlyAnalysis(long EnterpriseId, long CompanyId, long RoomId)
        {

            var params1 = new SqlParameter[] {
                new SqlParameter("@EnterpriseId", EnterpriseId),
                new SqlParameter("@CompanyId", CompanyId),
                new SqlParameter("@RoomId", RoomId)
            };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<DateTime>("exec [GetRoomCountMonthYear_InventoryMonthlyAnalysis] @EnterpriseId,@CompanyId,@RoomId", params1).ToList();
            }
        }
        public long InsertUpdateInventoryMonthlyAnalysis_RunHistory(long ID, long EnterpriseId, long CompanyId, long RoomId, int Month, int Year, bool IsStarted, bool IsCompleted, bool IsFailed, string ErrorMessage)
        {
            var paramInnerCase = new SqlParameter[] {
                                                new SqlParameter("@ID", ID),
                                                new SqlParameter("@EnterpriseId", EnterpriseId),
                                                new SqlParameter("@CompanyId", CompanyId),
                                                new SqlParameter("@RoomId", RoomId),
                                                new SqlParameter("@Month", Month),
                                                new SqlParameter("@Year", Year),
                                                new SqlParameter("@IsStarted", IsStarted),
                                                new SqlParameter("@IsCompleted", IsCompleted),
                                                new SqlParameter("@IsFailed", IsFailed),
                                                new SqlParameter("@ErrorMessage", ErrorMessage)
                                            };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<long>("exec InsertUpdateInventoryMonthlyAnalysis_RunHistory @ID,@EnterpriseId,@CompanyId,@RoomId,@Month,@Year,@IsStarted,@IsCompleted,@IsFailed,@ErrorMessage", paramInnerCase).FirstOrDefault();
            }
        }
    }
}