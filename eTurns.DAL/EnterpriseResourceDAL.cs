using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eTurns.DTO;
using System.Data;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Data.SqlClient;

namespace eTurns.DAL
{
    public partial class EnterpriseResourceDAL : eTurnsBaseDAL
    {
        public EnterpriseResourceDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        public List<EnterpriseResourcesDTO> GetPagedEnterpriseResource(long EnterpriseID, long ResoucePageID, long LanguageID, string SortColumnName, string SearchText)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@EnterpriseID", EnterpriseID), new SqlParameter("@ResoucePageID", ResoucePageID), new SqlParameter("@LanguageID", LanguageID), new SqlParameter("@SortColumnName", SortColumnName), new SqlParameter("@SearchText", SearchText) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<EnterpriseResourcesDTO>("exec [GetPagedEnterpriseResource] @EnterpriseID,@ResoucePageID,@LanguageID,@SortColumnName,@SearchText", params1).ToList();
            }
        }

        public List<EnterpriseResourcesDTO> UpdateEnterpriseResourceByID(long ID, string ResourceKey, string ResourceValue, long LastUpdatedBy, string EditedFrom, bool IsAcrossAll)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID), new SqlParameter("@ResourceKey", ResourceKey), new SqlParameter("@ResourceValue", ResourceValue), new SqlParameter("@LastUpdatedBy", LastUpdatedBy), new SqlParameter("@EditedFrom", EditedFrom), new SqlParameter("@IsAcrossAll", IsAcrossAll) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<EnterpriseResourcesDTO>("exec [UpdateEnterpriseResourceByID] @ID,@ResourceKey,@ResourceValue,@LastUpdatedBy,@EditedFrom,@IsAcrossAll", params1).ToList();
            }
        }

        public void ResetEnterpriseResource(long EnterpriseID, long LanguageID, long ModuleID, string ModuleName, long ResourcePageID, string ResourcePageName, long LastUpdatedBy, string EditedFrom)
        {
            var params1 = new SqlParameter[] {
                new SqlParameter("@EnterpriseID", EnterpriseID),
                new SqlParameter("@LanguageID", LanguageID),
                new SqlParameter("@ModuleID", ModuleID),
                new SqlParameter("@ModuleName", ModuleName),
                new SqlParameter("@ResourcePageID", ResourcePageID),
                new SqlParameter("@ResourcePageName", ResourcePageName),
                new SqlParameter("@LastUpdatedBy", LastUpdatedBy),
                new SqlParameter("@EditedFrom", EditedFrom) };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strCommand = "EXEC [ResetEnterpriseResource] @EnterpriseID,@LanguageID,@ModuleID,@ModuleName,@ResourcePageID,@ResourcePageName,@LastUpdatedBy,@EditedFrom";
                int intReturn = context.Database.SqlQuery<int>(strCommand, params1).FirstOrDefault();
            }
        }
        public IEnumerable<EnterpriseResourcesDTO> GetAllEnterprises()
        {
            eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString);
            IEnumerable<EnterpriseResourcesDTO> obj = (from u in context.Database.SqlQuery<EnterpriseResourcesDTO>("exec [GetEnterprises]")
                                                         select new EnterpriseResourcesDTO
                                                         {
                                                             ID = u.ID

                                                         }).AsParallel().ToList();
            return obj;
        }
        public void SaveEnterpriseResource(string ResourceKey, string ResourceValue, string resourcefile, string resourcelang, long Enterprise, long UserID, string EditedFrom)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                    new SqlParameter("@ResourceFile", resourcefile),
                    new SqlParameter("@ResourceKey", ResourceKey),
                    new SqlParameter("@ResourceValue", ResourceValue),
                    new SqlParameter("@LanguageCulture", resourcelang),
                    new SqlParameter("@RoomID", "0"),
                    new SqlParameter("@CompanyID", "0"),
                    new SqlParameter("@LastUpdatedBy", UserID),
                    new SqlParameter("@EditedFrom", EditedFrom),
                    new SqlParameter("@EnterpriseID", Enterprise)
                };

                context.Database.ExecuteSqlCommand("EXEC [InsertEnterpriseResource] @ResourceFile,@ResourceKey,@ResourceValue,@LanguageCulture,@RoomID,@CompanyID,@LastUpdatedBy,@EditedFrom,@EnterpriseID", params1);
            }
        }

        public void CreateEnterpriseResource(long EnterpriseID, long UserID, string EditedFrom)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                    new SqlParameter("@EnterpriseID", EnterpriseID),
                    new SqlParameter("@LastUpdatedBy", UserID),
                    new SqlParameter("@EditedFrom", EditedFrom)
                };

                context.Database.ExecuteSqlCommand("EXEC [CreateEnterpriseResource] @EnterpriseID,@LastUpdatedBy,@EditedFrom", params1);
            }
        }

        public void InsertRoomWisResource()
        {
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var params1 = new SqlParameter[] { };
                    context.Database.CommandTimeout = 7200;
                    context.Database.SqlQuery<string>("exec [InsertRoomWisResourceUsingJob]", params1).FirstOrDefault();
                }
            }catch(Exception ex)
            {
                throw ex;
            }
            
        }

    }
}
