using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public class SiteListColumnDetailDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        //public SiteListColumnDetailDAL(base.DataBaseName)
        //{

        //}

        public SiteListColumnDetailDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public SiteListColumnDetailDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        public IEnumerable<SiteListColumnDetailDTO> GetAllItems()
        {

            using (var dbcntx = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<SiteListColumnDetailDTO> obj = (from scm in dbcntx.SiteListColumnDetails
                                                            select new SiteListColumnDetailDTO
                                                            {
                                                                ID = scm.ID,
                                                                ListId = scm.ListId,
                                                                ColumnName = scm.ColumnName
                                                            }).AsParallel().ToList();
                return obj;
            }

        }
        public IEnumerable<SiteListColumnDetailDTO> GetAllItemsByListId(Int64 ListId,string UsersUISettingType)
        {
            using (var dbcntx = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ListID", ListId), new SqlParameter("@UserUISettingType", UsersUISettingType) };
                IEnumerable<SiteListColumnDetailDTO> obj = (from scm in dbcntx.Database.SqlQuery<SiteListColumnDetailDTO>("exec [GetAllItemsByListId] @ListID,@UserUISettingType", params1)                                                            
                                                            select new SiteListColumnDetailDTO
                                                            {
                                                                ID = scm.ID,
                                                                ListId = scm.ListId,
                                                                ColumnName = scm.ColumnName,
                                                                OrderNumber = scm.OrderNumber,
                                                                LastOrder = scm.OrderNumber,
                                                                ResourceFileName = scm.ResourceFileName,
                                                                ActualColumnName = scm.SortColumnName,
                                                                IsVisibilityEditable = scm.IsVisibilityEditable
                                                            }).AsParallel().ToList();
                return obj;
            }
        }

        public bool ImportSiteListColumnDetailTable(DataTable SiteListColumnDetailTable)
        {
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString("eTurns", DbConnectionType.GeneralReadWrite.ToString("F"));
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            SqlHelper.ExecuteNonQuery(EturnsConnection, "ImportSiteListColumnDetailTable", SiteListColumnDetailTable);
            return true;
        }

        public int GetMaximumColumnOrderByListName(string ListName,string UsersUISettingType)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ListName", ListName), new SqlParameter("@UsersUISettingType", UsersUISettingType) };
                return context.Database.SqlQuery<int>("exec [GetMaximumColumnOrderByListName] @ListName,@UsersUISettingType", params1).FirstOrDefault();
            }
        }
        public void MigrateSiteListColumnDetailToNLFForeTurns()
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("EXEC MigrateSiteListColumnDetailToNLFForeTurns");
            }
        }
    }
}
