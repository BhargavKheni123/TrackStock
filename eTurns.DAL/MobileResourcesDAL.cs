using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;


namespace eTurns.DAL
{
    public class MobileResourcesDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        public MobileResourcesDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public MobileResourcesDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion
        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        //public IEnumerable<MobileResourcesDTO> GetCachedData(Int64 CompanyID)
        //{
        //    IEnumerable<MobileResourcesDTO> ObjCache = CacheHelper<IEnumerable<MobileResourcesDTO>>.GetCacheItem("Cached_MobileResources_" + CompanyID.ToString());

        //    if (ObjCache == null || ObjCache.Count() <= 0)
        //    {
        //        using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //        {
        //            IEnumerable<MobileResourcesDTO> obj = (from u in context.Database.SqlQuery<MobileResourcesDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName FROM MobileResources A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.UpdatedBy = C.ID WHERE A.CompanyID = " + CompanyID.ToString()+"  and resourcekey not like 'UDF%'")
        //                                                   select new MobileResourcesDTO
        //                                                   {
        //                                                       ID = u.ID,
        //                                                       ResourceKey = u.ResourceKey,
        //                                                       ResourceValue = u.ResourceValue,
        //                                                       CompanyID = u.CompanyID,
        //                                                       CreatedBy = u.CreatedBy,
        //                                                       UpdatedBy = u.UpdatedBy,
        //                                                       CreatedOn = u.CreatedOn,
        //                                                       UpdatedOn = u.UpdatedOn,
        //                                                       CreatedByName = u.CreatedByName,
        //                                                       UpdatedByName = u.UpdatedByName,
        //                                                       LanguageID = u.LanguageID,
        //                                                       ResourcePageID = u.ResourcePageID,
        //                                                       Roomid=u.Roomid
        //                                                   }).AsParallel().ToList();
        //            ObjCache = CacheHelper<IEnumerable<MobileResourcesDTO>>.AddCacheItem("Cached_MobileResources_" + CompanyID.ToString(), obj);
        //        }
        //    }

        //    return ObjCache;
        //}

        /// <summary>
        /// Get Paged Records from the MobileResources Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        //public IEnumerable<MobileResourcesDTO> GetAllRecords(Int64 CompanyId)
        //{
        //    return GetCachedData(CompanyId).OrderBy("ID DESC");
        //}

        public List<MobileResourcesDTO> GetAllMobileResources(long CompanyID, long ResourcePageID, long LanguageID, string ResourceKey)
        {
            List<MobileResourcesDTO> objDTO = new List<MobileResourcesDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var sqlParams = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID)
                                                ,new SqlParameter("@ResourcePageID", ResourcePageID)
                                                ,new SqlParameter("@LanguageID", LanguageID)
                                                ,new SqlParameter("@ResourceKey", ResourceKey)};

                objDTO = (from u in context.Database.SqlQuery<MobileResourcesDTO>("exec [GetAllMobileResources] @CompanyID,@ResourcePageID,@LanguageID,@ResourceKey", sqlParams)
                          select new MobileResourcesDTO
                          {
                              ID = u.ID,
                              ResourcePageID = u.ResourcePageID,
                              ResourceKey = u.ResourceKey,
                              ResourceValue = u.ResourceValue,
                              LanguageID = u.LanguageID,
                              CompanyID = u.CompanyID,
                              CreatedBy = u.CreatedBy,
                              UpdatedBy = u.UpdatedBy,
                              CreatedOn = u.CreatedOn,
                              UpdatedOn = u.UpdatedOn,
                              CreatedByName = u.CreatedByName,
                              UpdatedByName = u.UpdatedByName,
                              Roomid = u.Roomid
                          }).AsParallel().ToList();
            }
            return objDTO;
        }

        /// <summary>
        /// Get Paged Records from the MobileResources Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        //public IEnumerable<MobileResourcesDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, bool IsDeleted, bool IsArchived, Int64 CompanyID)
        //{
        //    //Get Cached-Media
        //    IEnumerable<MobileResourcesDTO> ObjCache = GetCachedData(CompanyID);

        //    if (String.IsNullOrEmpty(SearchTerm))
        //    {

        //        TotalCount = ObjCache.Count();
        //        return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
        //    }
        //    else if (SearchTerm.Contains("[###]"))
        //    {
        //        //Get Cached-Media
        //        //IEnumerable<MobileResourcesDTO> ObjCache = GetCachedData(RoomID, CompanyID);
        //        string[] stringSeparators = new string[] { "[###]" };
        //        string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

        //        ObjCache = ObjCache.Where(t =>
        //               ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
        //            && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
        //            && ((Fields[1].Split('@')[2] == "") || (t.CreatedOn.Date >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.CreatedOn.Date <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
        //            && ((Fields[1].Split('@')[3] == "") || (t.UpdatedOn.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.UpdatedOn.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
        //            );

        //        TotalCount = ObjCache.Count();
        //        return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
        //    }
        //    else
        //    {
        //        //Get Cached-Media
        //        //IEnumerable<MobileResourcesDTO> ObjCache = GetCachedData(RoomID, CompanyID);
        //        TotalCount = ObjCache.Where
        //            (
        //                t => t.ID.ToString().Contains(SearchTerm) ||
        //                         (t.ResourceKey ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                         (t.ResourceValue ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
        //            ).Count();
        //        return ObjCache.Where
        //            (t => t.ID.ToString().Contains(SearchTerm) ||
        //             (t.ResourceKey ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //             (t.ResourceValue ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
        //            ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

        //    }
        //}

        /// <summary>
        /// Get Particullar Record from the MobileResources by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        //public MobileResourcesDTO GetRecord(Int64 id, Int64 CompanyID)
        //{
        //    return GetCachedData(CompanyID).Single(t => t.ID == id);
        //}

        /// <summary>
        /// Insert Record in the DataBase MobileResources
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 Insert(MobileResourcesDTO objDTO)
        {


            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                MobileResource obj = new MobileResource();
                obj.ID = 0;
                obj.ResourceKey = objDTO.ResourceKey;
                obj.ResourceValue = objDTO.ResourceValue;
                obj.CompanyID = objDTO.CompanyID;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.UpdatedBy = objDTO.UpdatedBy;
                obj.CreatedOn = objDTO.CreatedOn;
                obj.UpdatedOn = objDTO.UpdatedOn;
                obj.Roomid = objDTO.Roomid;
                obj.LanguageID = objDTO.LanguageID;
                obj.ResourcePageID = objDTO.ResourcePageID;

                context.MobileResources.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;


                return obj.ID;
            }

        }

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool Edit(MobileResourcesDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                MobileResource obj = context.MobileResources.FirstOrDefault(x => x.ID == objDTO.ID);
                obj.ID = objDTO.ID;

                obj.ResourceKey = objDTO.ResourceKey;
                obj.ResourceValue = objDTO.ResourceValue;
                obj.UpdatedBy = objDTO.UpdatedBy;
                obj.UpdatedOn = DateTimeUtility.DateTimeNow;
                obj.Roomid = objDTO.Roomid;

                //context.Entry(obj).State = System.Data.Entity.EntityState.Unchanged;
                //context.MobileResources.Attach(obj);
                //context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();


                return true;
            }
        }

        

        /// <summary>
        /// Delete Multiple Records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool ResetMobileResource(Int64 PageID, Int64 LangugeID, Int64 userid, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "EXEC [dbo].[ResetMobileResource] " + PageID + "," + LangugeID + "," + userid + "," + CompanyID + "";
                context.Database.ExecuteSqlCommand(strQuery);
                return true;
            }
        }

        /// <summary>
        /// Delete Multiple Records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool InsertMobileResourceForCompany(Int64 userid, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "EXEC [dbo].[InsertMobileResourceForCompany] " + userid + "," + CompanyID + "";
                context.Database.ExecuteSqlCommand(strQuery);
                return true;
            }
        }

        public bool UpdateResourceFromImport(MobileResourcesDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                MobileResource obj = context.MobileResources.FirstOrDefault(x => x.ResourcePageID == objDTO.ResourcePageID && x.LanguageID == objDTO.LanguageID && x.ResourceKey == objDTO.ResourceKey && x.CompanyID == objDTO.CompanyID);
                if (obj != null)
                {
                    obj.ResourceValue = objDTO.ResourceValue;
                    obj.UpdatedBy = objDTO.UpdatedBy;
                    obj.UpdatedOn = DateTimeUtility.DateTimeNow;
                    context.SaveChanges();
                    //CacheHelper<IEnumerable<MobileResourcesDTO>>.AppendToCacheItem("Cached_MobileResources_" + objDTO.CompanyID.ToString(), new List<MobileResourcesDTO>());
                }
                else
                {
                    obj = new MobileResource();
                    obj.ID = 0;
                    obj.ResourceKey = objDTO.ResourceKey;
                    obj.ResourceValue = objDTO.ResourceValue;
                    obj.CompanyID = objDTO.CompanyID;
                    obj.CreatedBy = objDTO.CreatedBy;
                    obj.UpdatedBy = objDTO.UpdatedBy;
                    obj.CreatedOn = objDTO.CreatedOn;
                    obj.UpdatedOn = objDTO.UpdatedOn;
                    obj.Roomid = objDTO.Roomid;
                    obj.LanguageID = objDTO.LanguageID;
                    obj.ResourcePageID = objDTO.ResourcePageID;

                    context.MobileResources.Add(obj);
                    context.SaveChanges();
                    objDTO.ID = obj.ID;

                    if (objDTO.ID > 0)
                    {
                        //CacheHelper<IEnumerable<MobileResourcesDTO>>.AppendToCacheItem("Cached_MobileResources_" + objDTO.CompanyID.ToString(), new List<MobileResourcesDTO>());
                    }
                }
                return true;
            }
        }

        public ResourceModuleDetailsDTO GetResourceModuleDetailData_Mobile(Int64 ResourceModuleID, Int64 PageID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var sqlParams = new SqlParameter[] { new SqlParameter("@ResourceModuleID", ResourceModuleID),
                                                     new SqlParameter("@PageID", PageID)};

                ResourceModuleDetailsDTO obj = (from u in context.Database.SqlQuery<ResourceModuleDetailsDTO>("exec [GetResourceModuleDetailData_Mobile] @ResourceModuleID,@PageID", sqlParams)
                                                             select new ResourceModuleDetailsDTO
                                                             {
                                                                 ID = u.ID,
                                                                 DisplayPageName = u.DisplayPageName,
                                                                 FileName = u.FileName,
                                                                 ResourceModuleID = u.ResourceModuleID,
                                                                 ResourceModuleName = u.ResourceModuleName,
                                                                 PageName = u.PageName,
                                                             }).AsParallel().FirstOrDefault();
                return obj;

            }
        }

        public IEnumerable<MobileResourcesDTO> GetAllMobileResourceRecords(Int64 ResourcePageID, Int64 LanguageID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var sqlParams = new SqlParameter[] { new SqlParameter("@ResourcePageID", ResourcePageID)
										            ,new SqlParameter("@LanguageID", LanguageID) };

                IEnumerable<MobileResourcesDTO> obj = (from u in context.Database.SqlQuery<MobileResourcesDTO>("exec [GetAllMobileResourceRecords] @ResourcePageID,@LanguageID", sqlParams)
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
        public IEnumerable<MobileResourcesDTO> GetAllMobileResourceRecordsFiltered(long Room, long companyid, long languageid, long resourcepageid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var sqlParams = new SqlParameter[] { new SqlParameter("@RoomID", Room)
										,new SqlParameter("@CompanyID", companyid)
										,new SqlParameter("@LanguageID", languageid)
										,new SqlParameter("@ResourcePageID", resourcepageid)};

                IEnumerable<MobileResourcesDTO> obj = (from u in context.Database.SqlQuery<MobileResourcesDTO>("exec [GetAllMobileResourceRecordsFiltered] @RoomID,@CompanyID,@LanguageID,@ResourcePageID", sqlParams)
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

    }
}


