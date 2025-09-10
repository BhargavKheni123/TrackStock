using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace eTurns.DAL
{
    public class GlobalUISettingsDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        public GlobalUISettingsDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public GlobalUISettingsDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        public GlobalUISettingsDTO GetRecord(Int64 ID, String SearchType)
        {

            //Get Cached-Media
            IEnumerable<GlobalUISettingsDTO> ObjCache = CacheHelper<IEnumerable<GlobalUISettingsDTO>>.GetCacheItem("Cached_GlobalUISettingsDTO_" + SearchType + "_" + ID.ToString());
            if (ObjCache == null)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    if (SearchType.ToLower() == "user")
                    {

                        var obj = (from u in context.GlobalUISettings
                                   where u.UserID == ID
                                   select new GlobalUISettingsDTO
                                   {
                                       ID = u.ID,
                                       CompanyID = u.CompanyID,
                                       UserID = u.UserID,
                                       GridRefreshTimeInSecond = u.GridRefreshTimeInSecond
                                   });
                        ObjCache = CacheHelper<IEnumerable<GlobalUISettingsDTO>>.AddCacheItem("Cached_GlobalUISettingsDTO_" + SearchType + "_" + ID.ToString(), obj);
                    }
                    else
                    {
                        var obj = (from u in context.GlobalUISettings
                                   where u.CompanyID == ID
                                   select new GlobalUISettingsDTO
                                   {
                                       ID = u.ID,
                                       CompanyID = u.CompanyID,
                                       UserID = u.UserID,
                                       GridRefreshTimeInSecond = u.GridRefreshTimeInSecond
                                   });

                        ObjCache = CacheHelper<IEnumerable<GlobalUISettingsDTO>>.AddCacheItem("Cached_GlobalUISettingsDTO_" + SearchType + "_" + ID.ToString(), obj);
                    }


                }
            }

            if (SearchType.ToLower() == "user")
            {
                return ObjCache.SingleOrDefault(t => t.UserID == ID);
            }
            else
            {
                return ObjCache.SingleOrDefault(t => t.CompanyID == ID);
            }
        }
    }
}
