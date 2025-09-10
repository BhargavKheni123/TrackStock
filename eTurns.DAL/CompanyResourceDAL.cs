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
    public partial class CompanyResourceDAL : eTurnsBaseDAL
    {
        public CompanyResourceDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        public List<CompanyResourcesDTO> GetPagedCompanyResource(long EnterpriseID, long CompanyID, long RoomID, long ResoucePageID, long LanguageID, string SortColumnName, string SearchText)
        {
            var params1 = new SqlParameter[] { 
                new SqlParameter("@EnterpriseID", EnterpriseID), 
                new SqlParameter("@CompanyID", CompanyID), 
                new SqlParameter("@RoomID", RoomID), 
                new SqlParameter("@ResoucePageID", ResoucePageID),
                new SqlParameter("@LanguageID", LanguageID), 
                new SqlParameter("@SortColumnName", SortColumnName), 
                new SqlParameter("@SearchText", SearchText) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CompanyResourcesDTO>("exec [GetPagedCompanyResource] @EnterpriseID,@CompanyID,@RoomID,@ResoucePageID,@LanguageID,@SortColumnName,@SearchText", params1).ToList();
            }
        }

        public List<CompanyResourcesDTO> UpdateCompanyResourceByID(long ID, string ResourceKey, string ResourceValue, long LastUpdatedBy, string EditedFrom)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID), new SqlParameter("@ResourceKey", ResourceKey), new SqlParameter("@ResourceValue", ResourceValue), new SqlParameter("@LastUpdatedBy", LastUpdatedBy), new SqlParameter("@EditedFrom", EditedFrom) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CompanyResourcesDTO>("exec [UpdateCompanyResourceByID] @ID,@ResourceKey,@ResourceValue,@LastUpdatedBy,@EditedFrom", params1).ToList();
            }
        }

        public List<CompanyResourcesDTO> UpdateCompanyResource(long ID, string ResourceKey, string ResourceValue, long LastUpdatedBy, string EditedFrom, long EnterpriseID, long CompanyID, long RoomID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID), new SqlParameter("@ResourceKey", ResourceKey), new SqlParameter("@ResourceValue", ResourceValue), 
                new SqlParameter("@LastUpdatedBy", LastUpdatedBy), new SqlParameter("@EditedFrom", EditedFrom), 
                new SqlParameter("@EnterpriseID", EnterpriseID),  new SqlParameter("@CompanyID", CompanyID),
                new SqlParameter("@RoomID", RoomID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CompanyResourcesDTO>("exec [UpdateCompanyResource] @ID,@ResourceKey,@ResourceValue,@LastUpdatedBy,@EditedFrom,@EnterpriseID,@CompanyID,@RoomID", params1).ToList();
            }
        }

        public List<CompanyResourcesDTO> GetPagedEnterpriseResourceForCompany(long EnterpriseID, long ResoucePageID, long LanguageID, string SortColumnName, string SearchText)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@EnterpriseID", EnterpriseID), new SqlParameter("@ResoucePageID", ResoucePageID), new SqlParameter("@LanguageID", LanguageID), new SqlParameter("@SortColumnName", SortColumnName), new SqlParameter("@SearchText", SearchText) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CompanyResourcesDTO>("exec [GetPagedEnterpriseResource] @EnterpriseID,@ResoucePageID,@LanguageID,@SortColumnName,@SearchText", params1).ToList();
            }
        }

        public void SaveCompanyResource(string ResourceKey, string ResourceValue, string Resourcefile, string Resourcelang, long EnterpriseID, long CompanyID, long RoomID, bool IsAcrossCompany, long UserID, string EditedFrom)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
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

        public void InsertUDFResource(string ResourceKey, string ResourceValue, string Resourcefile, string Resourcelang, long EnterpriseID, long CompanyID, long RoomID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                    new SqlParameter("@ResourceKey", ResourceKey),
                    new SqlParameter("@ResourceValue", ResourceValue),
                    new SqlParameter("@ResourceFile", Resourcefile),
                    new SqlParameter("@LanguageCulture", Resourcelang),
                    new SqlParameter("@EnterpriseID", EnterpriseID),
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@LastUpdatedBy","0"),
                    new SqlParameter("@EditedFrom", "Web")
                };

                context.Database.ExecuteSqlCommand("EXEC [InsertUDFResource] @ResourceKey,@ResourceValue,@ResourceFile,@LanguageCulture, @EnterpriseID, @CompanyID, @RoomID, @LastUpdatedBy, @EditedFrom", params1);
            }
        }

        public void CreateRoomResource(long EnterpriseID, long CompanyID, long RoomID, long UserID, string EditedFrom)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                    new SqlParameter("@EnterpriseID", EnterpriseID),
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@LastUpdatedBy", UserID),
                    new SqlParameter("@EditedFrom", EditedFrom)
                };

                context.Database.ExecuteSqlCommand("EXEC [CreateRoomResource] @EnterpriseID,@CompanyID,@RoomID,@LastUpdatedBy,@EditedFrom", params1);
            }
        }

        public CompanyResourcesDTO GetCompanyResourceByID(long ID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CompanyResourcesDTO>("EXEC [GetCompanyResourceByID] @ID", params1).FirstOrDefault();
            }
        }

        public CompanyResourcesDTO GetCompanyResource(long EnterpriseID, long CompanyID, long RoomID, long ID)
        {
            var params1 = new SqlParameter[] { 
                new SqlParameter("@EnterpriseID", EnterpriseID),
                new SqlParameter("@CompanyID", CompanyID),
                new SqlParameter("@RoomID", RoomID),
                new SqlParameter("@ID", ID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CompanyResourcesDTO>("EXEC [GetCompanyResource] @EnterpriseID,@CompanyID,@RoomID,@ID", params1).FirstOrDefault();
            }
        }

        public void InsertRoomWiseResourceByRoomID(long EnterpriseID, long CompanyID, long RoomID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                    new SqlParameter("@EnterpriseID", EnterpriseID),
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@RoomID", RoomID)
                };

                context.Database.ExecuteSqlCommand("EXEC [InsertRoomWiseResourceByRoomID] @EnterPriseID,@CompanyID,@RoomID", params1);
            }
        }

    }
}
