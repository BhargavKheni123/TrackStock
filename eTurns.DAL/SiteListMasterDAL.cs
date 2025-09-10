using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;

namespace eTurns.DAL
{
    public class SiteListMasterDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        //public SiteListMasterDAL(base.DataBaseName)
        //{

        //}

        public SiteListMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public SiteListMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        public IEnumerable<SiteListMasterDTO> GetAllItems(string UserUISettingType)
        {
            var para = new List<SqlParameter> { new SqlParameter("@UserUISettingType", UserUISettingType) };
            using (var dbcntx = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return dbcntx.Database.SqlQuery<SiteListMasterDTO>("exec [GetAllSiteListMastersByUserUISettingType] @UserUISettingType", para.ToArray()).ToList();
            }
        }

        public IEnumerable<SiteListMasterDTO> GetAllNonEmptyItems()
        {
            using (var dbcntx = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<SiteListMasterDTO> obj = (from scm in dbcntx.SiteListMasters
                                                      where !string.IsNullOrEmpty(scm.JSONDATA)
                                                      select new SiteListMasterDTO
                                                      {
                                                          ID = scm.ID,
                                                          ListName = scm.ListName,
                                                          JSONDATA = scm.JSONDATA
                                                      }).AsParallel().ToList();
                return obj;
            }

        }

        /// <summary>
        /// This method is used to get the json from the SiteListMaster based on ListName
        /// </summary>
        /// <param name="ListName"></param>
        /// <returns></returns>
        public string GetSiteListMasterDataByListName(string ListName, string UserUISettingType)
        {
            using (var dbcntx = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ListName", ListName),
                                                   new SqlParameter("@UserUISettingType", UserUISettingType)};
                var siteListMasters = dbcntx.Database.SqlQuery<UsersUISettingsDTO>("exec [GetSiteListMasterByListName] @ListName,@UserUISettingType", params1).FirstOrDefault();
                string jsonData = string.Empty;
                //var siteListMasters = dbcntx.SiteListMasters.Where(t => (t.ListName ?? string.Empty) == ListName && !string.IsNullOrEmpty(t.JSONDATA ?? string.Empty)).FirstOrDefault();//?.JSONDATA ?? string.Empty;

                if (siteListMasters != null && !string.IsNullOrEmpty(siteListMasters.JSONDATA))
                {
                    jsonData = siteListMasters.JSONDATA;
                }

                return jsonData;
            }
        }

        public SiteListMasterDTO GetAllItemsById(Int64 id,string UsersUISettingType)
        {
            using (var dbcntx = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ID", id), new SqlParameter("@UserUISettingType", UsersUISettingType) };
                SiteListMasterDTO obj = (from scm in dbcntx.Database.SqlQuery<SiteListMasterDTO>("exec [GetAllItemsById] @ID,@UserUISettingType", params1)
                                         select new SiteListMasterDTO
                                         {
                                             ID = scm.ID,
                                             ListName = scm.ListName,
                                             JSONDATA = scm.JSONDATA,
                                             ListDetails = scm.ListDetails
                                         }).AsParallel().FirstOrDefault();
                return obj;
            }
        }
        public bool UpdateJSONData(Int64 id, string JSONData,string UserUISettingType)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var para = new List<SqlParameter> {
                    new SqlParameter("@ID", id),
                    new SqlParameter("@JSONDATA", JSONData),
                    new SqlParameter("@UserUISettingType", UserUISettingType)
                };
                context.Database.ExecuteSqlCommand("exec [UpdateSiteListMastersJSONData] @ID,@JSONDATA,@UserUISettingType", para.ToArray());
                return true;
            }
        }

        #region DataMigrationHelper
        public SiteListMasterDTO GetSiteListDataByListName(string ListName)
        {
            using (var dbcntx = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                SiteListMasterDTO obj = (from scm in dbcntx.SiteListMasters
                                         where (scm.ListName ?? string.Empty) == ListName 
                                         && !string.IsNullOrEmpty(scm.JSONDATA ?? string.Empty)
                                         select new SiteListMasterDTO
                                         {
                                             ID = scm.ID,
                                             ListName = scm.ListName,
                                             JSONDATA = scm.JSONDATA,
                                             ListDetails = scm.ListDetails
                                         }).AsParallel().FirstOrDefault();
                return obj;
            }
        }

        public List<SiteListMasterDTO> GetAllSiteListMaster()
        {
            using (var dbcntx = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return dbcntx.Database.SqlQuery<SiteListMasterDTO>("exec [GetAllSiteListMaster]").ToList();
            }
        }
        public SiteListMasterDTO InsertUpdateSiteListMasterNLF(SiteListMasterDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var para = new List<SqlParameter> {
                    new SqlParameter("@ListName", objDTO.ListName),
                    new SqlParameter("@JSONDATA", (string.IsNullOrWhiteSpace(objDTO.JSONDATA) ? (object) DBNull.Value : objDTO.JSONDATA)),
                    new SqlParameter("@ListDetails", objDTO.ListDetails)
                };
                long id = context.Database
                    .SqlQuery<long>("exec InsertUpdateSiteListMasterNLF " +
                    "@ListName,@JSONDATA,@ListDetails", para.ToArray()).FirstOrDefault();

                objDTO.ID = id;
            }
            return objDTO;
        }
        #endregion
    }
}
