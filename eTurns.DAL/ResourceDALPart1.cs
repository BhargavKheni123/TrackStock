using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using eTurns.DTO.Resources;
using System.Data.SqlClient;

namespace eTurns.DAL
{
    public class ResourceDAL : eTurnsBaseDAL
    {
        public IEnumerable<ResourceLanguageDTO> GetCachedResourceLanguageData(Int64 EnterpriseID)
        {
            //Get Cached-Media
            IEnumerable<ResourceLanguageDTO> ObjCache = CacheHelper<IEnumerable<ResourceLanguageDTO>>.GetCacheItem("Cached_ResourceLaguages_" + EnterpriseID);
            if (ObjCache == null || ObjCache.Count() <= 0)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<ResourceLanguageDTO> obj = (from u in context.ExecuteStoreQuery<ResourceLanguageDTO>(@"SELECT * FROM ResourceLaguages Order by Language")
                                                            select new ResourceLanguageDTO
                                                            {
                                                                ID = u.ID,
                                                                Culture = u.Culture,
                                                                Language = u.Language
                                                            }).AsParallel().ToList();
                    ObjCache = CacheHelper<IEnumerable<ResourceLanguageDTO>>.AddCacheItem("Cached_ResourceLaguages_" + EnterpriseID, obj);
                }
            }

            return ObjCache;
        }

        public IEnumerable<ResourceLanguageDTO> GeteTurnsLanguages()
        {
            List<ResourceLanguageDTO> lstLans = new List<ResourceLanguageDTO>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstLans = (from u in context.ExecuteStoreQuery<ResourceLanguageDTO>(@"SELECT * FROM ResourceLaguages Order by Language")
                           select new ResourceLanguageDTO
                           {
                               ID = u.ID,
                               Culture = u.Culture,
                               Language = u.Language
                           }).AsParallel().ToList();

            }


            return lstLans;
        }

        public IEnumerable<ResourceLanguageDTO> OnlyeTurnsLanguages()
        {
            List<ResourceLanguageDTO> lstLans = new List<ResourceLanguageDTO>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstLans = (from u in context.ExecuteStoreQuery<ResourceLanguageDTO>(@"SELECT * FROM " + CommonDAL.GeteTurnsDatabase() + ".dbo.ResourceLaguages Order by Language")
                           select new ResourceLanguageDTO
                           {
                               ID = u.ID,
                               Culture = u.Culture,
                               Language = u.Language
                           }).AsParallel().ToList();

            }


            return lstLans;
        }

        public IEnumerable<ResourceModuleMasterDTO> GetCachedResourceModuleMasterData(Int64 CompanyID)
        {
            //Get Cached-Media
            IEnumerable<ResourceModuleMasterDTO> ObjCache = CacheHelper<IEnumerable<ResourceModuleMasterDTO>>.GetCacheItem("Cached_ResourceLaguages_" + CompanyID);
            if (ObjCache == null || ObjCache.Count() <= 0)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<ResourceModuleMasterDTO> obj = (from u in context.ExecuteStoreQuery<ResourceModuleMasterDTO>(@"SELECT * FROM ResourceModuleMaster Order by ModuleName")
                                                                select new ResourceModuleMasterDTO
                                                                {
                                                                    ID = u.ID,
                                                                    ModuleName = u.ModuleName,
                                                                    ResModuleKey = u.ResModuleKey
                                                                }).AsParallel().ToList();
                    ObjCache = CacheHelper<IEnumerable<ResourceModuleMasterDTO>>.AddCacheItem("Cached_ResourceLaguages_" + CompanyID, obj);
                }
            }

            return ObjCache;
        }

        public IEnumerable<ResourceModuleDetailsDTO> GetCachedResourceModuleDetailData(Int64 CompanyID, int ResourceModuleID)
        {
            //Get Cached-Media
            IEnumerable<ResourceModuleDetailsDTO> ObjCache = CacheHelper<IEnumerable<ResourceModuleDetailsDTO>>.GetCacheItem("Cached_ResourceModuleDetail_" + CompanyID + "_" + ResourceModuleID);
            if (ObjCache == null)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<ResourceModuleDetailsDTO> obj = (from u in context.ExecuteStoreQuery<ResourceModuleDetailsDTO>(@"SELECT A.*,B.ModuleName  as ResourceModuleName FROM ResourceModuleDetails A INNER  JOIN ResourceModuleMaster B On A.ResourceModuleID = B.ID WHERE  A.ResourceModuleID = " + ResourceModuleID + " Order by A.DisplayFileName")
                                                                 select new ResourceModuleDetailsDTO
                                                                 {
                                                                     ID = u.ID,
                                                                     DisplayFileName = u.DisplayFileName,
                                                                     FileName = u.FileName,
                                                                     ResourceModuleID = u.ResourceModuleID,
                                                                     ResourceModuleName = u.ResourceModuleName,
                                                                     ResModuleDetailsKey = u.ResModuleDetailsKey
                                                                 }).AsParallel().ToList();
                    ObjCache = CacheHelper<IEnumerable<ResourceModuleDetailsDTO>>.AddCacheItem("Cached_ResourceModuleDetail_" + CompanyID + "_" + ResourceModuleID, obj);
                }
            }

            return ObjCache;
        }

        public IEnumerable<ResourceModuleDetailsDTO> GetCachedResourceModuleDetailData_Mobile(Int64 CompanyID, Int64 ResourceModuleID)
        {
            //Get Cached-Media
            IEnumerable<ResourceModuleDetailsDTO> ObjCache = CacheHelper<IEnumerable<ResourceModuleDetailsDTO>>.GetCacheItem("Cached_ResourceModuleDetail_Mobile_" + CompanyID + "_" + ResourceModuleID);
            if (ObjCache == null)
            {
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
                                                                     ResMobModuleDetailsKey = u.ResMobModuleDetailsKey
                                                                 }).AsParallel().ToList();
                    ObjCache = CacheHelper<IEnumerable<ResourceModuleDetailsDTO>>.AddCacheItem("Cached_ResourceModuleDetail_Mobile_" + CompanyID + "_" + ResourceModuleID, obj);
                }
            }

            return ObjCache;
        }

        public IEnumerable<ResourceModuleDetailsDTO> GetResourceModuleDetailData_Mobile(Int64 ResourceModuleID)
        {
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
                                                                 ResMobModuleDetailsKey = u.ResMobModuleDetailsKey
                                                             }).AsParallel().ToList();
                return obj;

            }
        }

        public IEnumerable<ResourceLanguageDTO> GetETurnsResourceLanguageData()
        {
            eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString);

            IEnumerable<ResourceLanguageDTO> obj = (from u in context.ExecuteStoreQuery<ResourceLanguageDTO>(@"SELECT * FROM [" + DbConnectionHelper.GeteTurnsDBName() + "].[dbo].[ResourceLaguages] Order by Language")
                                                    select new ResourceLanguageDTO
                                                    {
                                                        ID = u.ID,
                                                        Culture = u.Culture,
                                                        Language = u.Language
                                                    }).AsParallel().ToList();
            return obj;
        }

        public void DeleteETurnsResourceLanguageData(ResourceLanguageDTO objResourceLanguageDTO)  
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                string strCommand = "Delete from [" + DbConnectionHelper.GeteTurnsDBName() + "].[dbo].[EmailTemplateDetail] where CompanyID = 0 and [ResourceLaguageId] = " + objResourceLanguageDTO.ID;
                context.ExecuteStoreCommand(strCommand);

                strCommand = string.Empty;
                strCommand = "Delete from [" + DbConnectionHelper.GeteTurnsDBName() + "].[dbo].[MobileResources] where CompanyID = 0 and [LanguageID] = " + objResourceLanguageDTO.ID;
                context.ExecuteStoreCommand(strCommand);

                strCommand = string.Empty;
                strCommand = "Delete from [" + DbConnectionHelper.GeteTurnsDBName() + "].[dbo].[ResourceLaguages] where [Language] = '" + objResourceLanguageDTO.Language + "'";
                context.ExecuteStoreCommand(strCommand);





            }
        }

        public IEnumerable<ResourceLanguageDTO> GetResourceLanguageData()
        {
            eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString);

            IEnumerable<ResourceLanguageDTO> obj = (from u in context.ExecuteStoreQuery<ResourceLanguageDTO>(@"SELECT * FROM [ResourceLaguages] Order by Language")
                                                    select new ResourceLanguageDTO
                                                    {
                                                        ID = u.ID,
                                                        Culture = u.Culture,
                                                        Language = u.Language
                                                    }).AsParallel().ToList();
            return obj;
        }

        public void DeleteResourceLanguageData(ResourceLanguageDTO objResourceLanguageDTO, int CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strCommand = string.Empty;
                if (CompanyID != 0)
                {
                    strCommand = "Delete from [MobileResources] where CompanyID = " + CompanyID + " And [LanguageID] = " + objResourceLanguageDTO.ID;

                }
                else
                {
                    strCommand = "Delete from [MobileResources] where  [LanguageID] = " + objResourceLanguageDTO.ID;

                }
                context.ExecuteStoreCommand(strCommand);
                strCommand = string.Empty;
                if (CompanyID != 0)
                {
                    strCommand = "Delete from [EmailTemplateDetail] where CompanyID = " + CompanyID + " and [ResourceLaguageId] = " + objResourceLanguageDTO.ID;

                }
                else
                {
                    strCommand = "Delete from [EmailTemplateDetail] where [ResourceLaguageId] = " + objResourceLanguageDTO.ID;

                }

                context.ExecuteStoreCommand(strCommand);
                strCommand = string.Empty;
                if (CompanyID == 0)
                {
                    strCommand = "Delete from [ResourceLaguages] where [Language] = '" + objResourceLanguageDTO.Language + "'";
                    context.ExecuteStoreCommand(strCommand);
                }




            }
        }


        public ResourceModuleMasterDTO GetResourceModuleByID(Int64 ModuleID)
        {
            //Get Cached-Media

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ResourceModuleMasterDTO obj = (from u in context.ExecuteStoreQuery<ResourceModuleMasterDTO>(@"SELECT * FROM ResourceModuleMaster Where ID =" + ModuleID)
                                               select new ResourceModuleMasterDTO
                                               {
                                                   ID = u.ID,
                                                   ModuleName = u.ModuleName
                                               }).FirstOrDefault();

                return obj;

            }



        }

        public ResourceModuleDetailsDTO GetResourceModuleDetail(Int64 ModuleID, string ResourcePageName)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ResourceModuleDetailsDTO obj = (from u in context.ExecuteStoreQuery<ResourceModuleDetailsDTO>(@"SELECT A.*,B.ModuleName  as ResourceModuleName FROM ResourceModuleDetails A INNER  JOIN ResourceModuleMaster B On A.ResourceModuleID = B.ID WHERE  A.ResourceModuleID = " + ModuleID + " And A.FileName = '" + ResourcePageName + "' Order by A.DisplayFileName")
                                                select new ResourceModuleDetailsDTO
                                                {
                                                    ID = u.ID,
                                                    DisplayFileName = u.DisplayFileName,
                                                    FileName = u.FileName,
                                                    ResourceModuleID = u.ResourceModuleID,
                                                    ResourceModuleName = u.ResourceModuleName
                                                }).FirstOrDefault();



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
                                                       }).AsParallel().ToList();
                return obj;
            }
        }

        public bool SaveRoomWiseEmailTemplate(long CompanyId, long RoomId, long UserID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";

                strQuery += @"DELETE FROM EmailTemplateDetail WHERE RoomID=" + RoomId + " and  CompanyID = " + CompanyId + @"; 
                             INSERT INTO EmailTemplateDetail
                             SELECT  [EmailTemplateId]
                                    ,[MailBodyText]
                                    ,[ResourceLaguageId]
                                    ,getutcdate() [Created]
                                    ,getutcdate() [Updated]
                                    ," + UserID + @" [CreatedBy]
                                    ," + UserID + @" [LastUpdatedBy]
                                    ," + CompanyId + @" [CompanyID]
                                    ," + RoomId + @" [RoomID]
                                    ,[MailSubject]
                                    ,null
                                    ,ReportId
                              FROM  EmailTemplateDetail where CompanyID = 0 and RoomID=0";

                context.ExecuteStoreCommand(strQuery);
                return true;
            }
        }
    }
}
