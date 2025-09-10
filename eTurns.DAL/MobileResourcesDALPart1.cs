using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using eTurns.DTO.Resources;

namespace eTurns.DAL
{
    public class MobileResourcesDALPart1 : eTurnsBaseDAL
    {
        #region [Class Constructor]

        //public MobileResourcesDAL(string DbName)
        //{
        //    base.DataBaseName = DbName;
        //}

        #endregion

        public IEnumerable<MobileResourcesDTO> GetCachedData(Int64 CompanyID)
        {
            IEnumerable<MobileResourcesDTO> ObjCache = CacheHelper<IEnumerable<MobileResourcesDTO>>.GetCacheItem("Cached_MobileResources_" + CompanyID.ToString());

            if (ObjCache == null || ObjCache.Count() <= 0)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<MobileResourcesDTO> obj = (from u in context.ExecuteStoreQuery<MobileResourcesDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName FROM MobileResources A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.UpdatedBy = C.ID WHERE A.CompanyID = " + CompanyID.ToString() + "  and resourcekey not like 'UDF%'")
                                                           select new MobileResourcesDTO
                                                           {
                                                               ID = u.ID,
                                                               ResourceKey = u.ResourceKey,
                                                               ResourceValue = u.ResourceValue,
                                                               CompanyID = u.CompanyID,
                                                               CreatedBy = u.CreatedBy,
                                                               UpdatedBy = u.UpdatedBy,
                                                               CreatedOn = u.CreatedOn,
                                                               UpdatedOn = u.UpdatedOn,
                                                               CreatedByName = u.CreatedByName,
                                                               UpdatedByName = u.UpdatedByName,
                                                               LanguageID = u.LanguageID,
                                                               ResourcePageID = u.ResourcePageID,
                                                               Roomid = u.Roomid
                                                           }).AsParallel().ToList();
                    ObjCache = CacheHelper<IEnumerable<MobileResourcesDTO>>.AddCacheItem("Cached_MobileResources_" + CompanyID.ToString(), obj);
                }
            }

            return ObjCache;
        }

        public IEnumerable<MobileResourcesDTO> GetAllRecords(Int64 CompanyId)
        {
            return GetCachedData(CompanyId).OrderBy("ID DESC");
        }

        public bool ResetMobileResource(Int64 PageID, Int64 LangugeID, Int64 userid, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";

                strQuery += @"Update MobileResources 

                                    Set UpdatedBy=" + userid + @", UpdatedOn= getutcdate(), ResourceValue = (SELECT Top 1 ResourceValue FROM MobileResources AS B 
						                                WHERE B.ResourceKey= A.ResourceKey AND B.ResourcePageID = " + PageID + @" 
                                                              AND B.LanguageID = " + LangugeID + @" and B.CompanyID = 0)
                               FROM MobileResources A
                               WHERE  ResourcePageID = " + PageID + @" AND LanguageID = " + LangugeID + @" and CompanyID = " + CompanyID;

                context.ExecuteStoreCommand(strQuery);
                IEnumerable<MobileResourcesDTO> ObjCache = CacheHelper<IEnumerable<MobileResourcesDTO>>.GetCacheItem("Cached_MobileResources_" + CompanyID.ToString());
                if (ObjCache != null)
                {
                    CacheHelper<IEnumerable<MobileResourcesDTO>>.AppendToCacheItem("Cached_MobileResources_" + CompanyID.ToString(), new List<MobileResourcesDTO>());
                } 

                return true;
            }
        }

        public bool InsertMobileResourceForCompany(Int64 userid, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                string strQuery = "";

                strQuery += @"DELETE FROM MobileResources WHERE CompanyID = " + CompanyID + @"; 
                             INSERT INTO MobileResources (ResourcePageID,ResourceKey,ResourceValue,LanguageID,CompanyID,CreatedBy,UpdatedBy,CreatedOn,UpdatedOn)
                             SELECT [ResourcePageID]
                                  ,[ResourceKey]
                                  ,[ResourceValue]
                                  ,[LanguageID]
                                  ," + CompanyID + @" [CompanyID]
                                  ," + userid + @" [CreatedBy]
                                  ," + userid + @" [UpdatedBy]
                                  ,getutcdate() [CreatedOn]
                                  ,getutcdate() [UpdatedOn]
                              FROM  MobileResources where CompanyID = 0 ";

                context.ExecuteStoreCommand(strQuery);
                IEnumerable<MobileResourcesDTO> ObjCache = CacheHelper<IEnumerable<MobileResourcesDTO>>.GetCacheItem("Cached_MobileResources_" + CompanyID.ToString());
                if (ObjCache != null)
                {
                    CacheHelper<IEnumerable<MobileResourcesDTO>>.AppendToCacheItem("Cached_MobileResources_" + CompanyID.ToString(), new List<MobileResourcesDTO>());
                }

                return true;
            }
        }

        public IEnumerable<ResourceModuleDetailsDTO> GetResourceModuleDetailData_Mobile(Int64 ResourceModuleID)
        {
            //Get Cached-Media
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<ResourceModuleDetailsDTO> obj = (from u in context.ExecuteStoreQuery<ResourceModuleDetailsDTO>(@"SELECT A.*,B.ModuleName  as ResourceModuleName FROM ResourceModuleDetails_Mobile A INNER  JOIN ResourceModuleMaster B On A.ResourceModuleID = B.ID WHERE  A.ResourceModuleID = " + ResourceModuleID + " Order by A.DisplayPageName")
                                                             select new ResourceModuleDetailsDTO
                                                             {
                                                                 ID = u.ID,
                                                                 DisplayPageName = u.DisplayPageName,
                                                                 FileName = u.FileName,
                                                                 ResourceModuleID = u.ResourceModuleID,
                                                                 ResourceModuleName = u.ResourceModuleName,
                                                                 PageName = u.PageName,
                                                             }).AsParallel().ToList();
                return obj;

            }
        }

        public IEnumerable<MobileResourcesDTO> GetAllMobileResourceRecords()
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<MobileResourcesDTO> obj = (from u in context.ExecuteStoreQuery<MobileResourcesDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName FROM MobileResources A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.UpdatedBy = C.ID WHERE ISNULL(A.CompanyID,0) = 0")
                                                       select new MobileResourcesDTO
                                                       {
                                                           ID = u.ID,
                                                           ResourceKey = u.ResourceKey,
                                                           ResourceValue = u.ResourceValue,
                                                           CompanyID = u.CompanyID,
                                                           CreatedBy = u.CreatedBy,
                                                           UpdatedBy = u.UpdatedBy,
                                                           CreatedOn = u.CreatedOn,
                                                           UpdatedOn = u.UpdatedOn,
                                                           CreatedByName = u.CreatedByName,
                                                           UpdatedByName = u.UpdatedByName,
                                                           LanguageID = u.LanguageID,
                                                           ResourcePageID = u.ResourcePageID,
                                                           Roomid = u.Roomid
                                                       }).AsParallel().ToList();
                return obj;
            }
        }
		
		/// <summary>
        /// Delete Multiple Records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE MobileResources SET UpdatedOn = getutcdate() , UpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.ExecuteStoreCommand(strQuery);


                //Get Cached-Media
                //IEnumerable<MobileResourcesDTO> ObjCache = CacheHelper<IEnumerable<MobileResourcesDTO>>.GetCacheItem("Cached_MobileResources_" + CompanyID.ToString());
                //if (ObjCache != null)
                //{
                //    List<MobileResourcesDTO> objTemp = ObjCache.ToList();
                //    objTemp.RemoveAll(i => IDs.Split(',').Contains(i.ID.ToString()));
                //    ObjCache = objTemp.AsEnumerable();
                //    CacheHelper<IEnumerable<MobileResourcesDTO>>.AppendToCacheItem("Cached_MobileResources_" + CompanyID.ToString(), ObjCache);
                //}

                return true;
            }
        }


    }
}
