using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace eTurns.DTO.Resources
{
    public class ResourceDBHelper
    {
        string providerName = ConfigurationManager.AppSettings.AllKeys.Contains("DBproviderName") ? Convert.ToString(ConfigurationManager.AppSettings["DBproviderName"]) : "System.Data.SqlClient";
        string DBserverName = ConfigurationManager.AppSettings.AllKeys.Contains("DBserverName") ? Convert.ToString(ConfigurationManager.AppSettings["DBserverName"]) : "DESKTOP-LV6DASV";
        string DBName = GeteTurnsResourceDBName();
        string DbUserName = ConfigurationManager.AppSettings.AllKeys.Contains("DbUserName") ? Convert.ToString(ConfigurationManager.AppSettings["DbUserName"]) : "sa";
        string DbPassword = ConfigurationManager.AppSettings.AllKeys.Contains("DbPassword") ? Convert.ToString(ConfigurationManager.AppSettings["DbPassword"]) : "ibm@@123";
        string DBFailoverPartner = ConfigurationManager.AppSettings.AllKeys.Contains("DBFailoverPartner") ? Convert.ToString(ConfigurationManager.AppSettings["DBFailoverPartner"]) : "172.31.12.215";
        private string GetConnectionString()
        {
            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();
            sqlBuilder.DataSource = DBserverName;
            sqlBuilder.InitialCatalog = DBName;
            sqlBuilder.UserID = DbUserName;
            sqlBuilder.Password = DbPassword;
            sqlBuilder.MultipleActiveResultSets = true;
            sqlBuilder.FailoverPartner = DBFailoverPartner;
            string providerString = sqlBuilder.ToString();
            sqlBuilder.PersistSecurityInfo = true;
            //sqlBuilder.MultipleActiveResultSets = true;
            sqlBuilder.ApplicationName = "EntityFramework";
            EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder();
            entityBuilder.Provider = "System.Data.SqlClient";
            providerString = sqlBuilder.ToString();
            entityBuilder.ProviderConnectionString = providerString;
            entityBuilder.Metadata = @"res://*/eTurnsEntities.csdl|res://*/eTurnsEntities.ssdl|res://*/eTurnsEntities.msl";
            return entityBuilder.ToString();
        }

        private string GetSQLConnectionString()
        {
            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();
            sqlBuilder.DataSource = DBserverName;
            sqlBuilder.InitialCatalog = DBName;
            sqlBuilder.UserID = DbUserName;
            sqlBuilder.Password = DbPassword;
            sqlBuilder.MultipleActiveResultSets = true;
            sqlBuilder.FailoverPartner = DBFailoverPartner;
            string providerString = sqlBuilder.ToString();
            sqlBuilder.PersistSecurityInfo = true;
            //sqlBuilder.MultipleActiveResultSets = true;
            sqlBuilder.ApplicationName = "EntityFramework";            
            providerString = sqlBuilder.ToString();
            return providerString;
        }

        private static string GeteTurnsResourceDBName()
        {
            try
            {
                return ConfigurationManager.AppSettings["eTurnsResourceDBName"].ToString();
            }
            catch
            {
                return "eTurnsResource";
            }
        }
        public BaseResourcesDTO GetResources(long? ID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID ?? (object)DBNull.Value) };
            using (var context = new DbContext(GetSQLConnectionString()))
            {
                return context.Database.SqlQuery<BaseResourcesDTO>("exec [GetBaseResourceByIDPlain] @ID", params1).FirstOrDefault();
            }
        }

        public List<CompanyResourcesKeyValDTO> GetCompanyResourceByResFileCulture(Int64 EnterpriseID, Int64 CompanyID, Int64 RoomID, string ResourceFile, string LanguageCulture)
        {
            var params1 = new SqlParameter[] { 
                new SqlParameter("@EnterpriseID", EnterpriseID), 
                new SqlParameter("@CompanyID", CompanyID),
                new SqlParameter("@RoomID", RoomID),
                new SqlParameter("@ResourceFile", ResourceFile),
                new SqlParameter("@LanguageCulture", LanguageCulture)
            };
            using (var context = new DbContext(GetSQLConnectionString()))
            {
                return context.Database.SqlQuery<CompanyResourcesKeyValDTO>("exec [GetCompanyResourceByResFileCulture] @EnterpriseID,@CompanyID,@RoomID,@ResourceFile,@LanguageCulture", params1).ToList();
            }
        }

        public CompanyResourcesKeyValDTO GetCompanyResourceByResFileCultureByKey(Int64 EnterpriseID, Int64 CompanyID, Int64 RoomID, string ResourceFile, string LanguageCulture, string ResourceKey)
        {
            var params1 = new SqlParameter[] { 
                new SqlParameter("@EnterpriseID", EnterpriseID), 
                new SqlParameter("@CompanyID", CompanyID),
                new SqlParameter("@RoomID", RoomID),
                new SqlParameter("@ResourceFile", ResourceFile),
                new SqlParameter("@LanguageCulture", LanguageCulture),
                new SqlParameter("@ResourceKey", ResourceKey)
            };
            using (var context = new DbContext(GetSQLConnectionString()))
            {
                return context.Database.SqlQuery<CompanyResourcesKeyValDTO>("exec [GetCompanyResourceByResFileCultureByKey] @EnterpriseID,@CompanyID,@RoomID,@ResourceFile,@LanguageCulture,@ResourceKey", params1).FirstOrDefault();
            }
        }

        public List<EnterpriseResourcesKeyValDTO> GetEnterpriseResourceByResFileCulture(Int64 EnterpriseID, string ResourceFile, string LanguageCulture)
        {
            
            var params1 = new SqlParameter[] { 
                new SqlParameter("@EnterpriseID", EnterpriseID), 
                new SqlParameter("@ResourceFile", ResourceFile),
                new SqlParameter("@LanguageCulture", LanguageCulture)
            };
            using (var context = new DbContext(GetSQLConnectionString()))
            {
                return context.Database.SqlQuery<EnterpriseResourcesKeyValDTO>("exec [GetEnterpriseResourceByResFileCulture] @EnterpriseID,@ResourceFile,@LanguageCulture", params1).ToList();
            }
        }

        public List<BaseResourcesKeyValDTO> GetBaseResourceByResFileCulture(string ResourceFile, string LanguageCulture)
        {
            var params1 = new SqlParameter[] { 
                new SqlParameter("@ResourceFile", ResourceFile),
                new SqlParameter("@LanguageCulture", LanguageCulture)
            };
            using (var context = new DbContext(GetSQLConnectionString()))
            {
                return context.Database.SqlQuery<BaseResourcesKeyValDTO>("exec [GetBaseResourceByResFileCulture] @ResourceFile,@LanguageCulture", params1).ToList();
            }
        }

        public void SaveCompanyResource(string ResourceKey, string ResourceValue, string Resourcefile, string Resourcelang, long EnterpriseID, long CompanyID, long RoomID, bool IsAcrossCompany, long UserID, string EditedFrom)
        {
            using (var context = new DbContext(GetSQLConnectionString()))
            {
                var params1 = new SqlParameter[] {
                    new SqlParameter("@ResourceKey", ResourceKey),
                    new SqlParameter("@ResourceValue", ResourceValue),
                    new SqlParameter("@ResourceFile", Resourcefile),
                    new SqlParameter("@LanguageCulture", Resourcelang),
                    new SqlParameter("@EnterpriseID", EnterpriseID),
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@LastUpdatedBy", UserID),
                    new SqlParameter("@EditedFrom", EditedFrom),
                    new SqlParameter("@IsInsertAcrossCompany", IsAcrossCompany)
                };

                context.Database.ExecuteSqlCommand("EXEC [InsertCompanyResource] @ResourceKey,@ResourceValue,@ResourceFile,@LanguageCulture, @EnterpriseID, @CompanyID, @RoomID, @LastUpdatedBy, @EditedFrom, @IsInsertAcrossCompany", params1);
            }
        }

        public void SaveBaseResourceDetail(string ResourceKey, string ResourceValue, string resourcefile, string resourcelang, long UserID, string EditedFrom)
        {
            using (var context = new DbContext(GetSQLConnectionString()))
            {
                var params1 = new SqlParameter[] {
                    new SqlParameter("@ResourceFile", resourcefile),
                    new SqlParameter("@ResourceKey", ResourceKey),
                    new SqlParameter("@ResourceValue", ResourceValue),
                    new SqlParameter("@LanguageCulture", resourcelang),
                    new SqlParameter("@RoomID", "0"),
                    new SqlParameter("@CompanyID", "0"),
                    new SqlParameter("@LastUpdatedBy",UserID),
                    new SqlParameter("@EditedFrom", EditedFrom)
                };
                context.Database.ExecuteSqlCommand("EXEC [InsertBaseResourceDetail] @ResourceFile,@ResourceKey,@ResourceValue,@LanguageCulture,@RoomID,@CompanyID,@LastUpdatedBy,@EditedFrom", params1);
            }
        }

        public void SaveEnterpriseResource(string ResourceKey, string ResourceValue, string resourcefile, string resourcelang, long EnterpriseID, long UserID, string EditedFrom)
        {
            using (var context = new DbContext(GetSQLConnectionString()))
            {
                var params1 = new SqlParameter[] {
                    new SqlParameter("@ResourceFile", resourcefile),
                    new SqlParameter("@ResourceKey", ResourceKey),
                    new SqlParameter("@ResourceValue", ResourceValue),
                    new SqlParameter("@LanguageCulture", resourcelang),
                    new SqlParameter("@RoomID", "0"),
                    new SqlParameter("@CompanyID", "0"),
                    new SqlParameter("@LastUpdatedBy",UserID),
                    new SqlParameter("@EditedFrom", EditedFrom),
                    new SqlParameter("@EnterpriseID", EnterpriseID)
                };
                context.Database.ExecuteSqlCommand("EXEC [InsertEnterpriseResource] @ResourceFile,@ResourceKey,@ResourceValue,@LanguageCulture,@RoomID,@CompanyID,@LastUpdatedBy,@EditedFrom,@EnterpriseID", params1);
            }
        }

    }
}
