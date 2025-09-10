using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;


namespace eTurns.DAL
{
    public partial class PollAllFlagMasterDAL : eTurnsBaseDAL
    {

        #region [Class Constructor]

        public PollAllFlagMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public PollAllFlagMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
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
        private IEnumerable<PollAllFlagMasterDTO> GetCachedData(Int64 CompanyID)
        {
            //Get Cached-Media
            IEnumerable<PollAllFlagMasterDTO> ObjCache = null; // CacheHelper<IEnumerable<PollAllFlagMasterDTO>>.GetCacheItem("Cached_PollAllFlagMaster_" + CompanyID.ToString());
                                                               //if (ObjCache == null || ObjCache.Count() <= 0)
                                                               //{
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID) };
                IEnumerable<PollAllFlagMasterDTO> obj = (from u in context.Database.SqlQuery<PollAllFlagMasterDTO>("exec GetPollAllFlagMasterByCompany @CompanyID", params1)
                                                         select new PollAllFlagMasterDTO
                                                         {
                                                             ID = u.ID,
                                                             IsPollALL = u.IsPollALL,
                                                             RoomID = u.RoomID,
                                                             CompanyID = u.CompanyID,
                                                             CreatedBy = u.CreatedBy,
                                                             CreatedOn = u.CreatedOn,
                                                             IsDeleted = u.IsDeleted,
                                                             IsArchived = u.IsArchived,
                                                             LastUpdatedOn = u.LastUpdatedOn,
                                                             LastUpdatedBy = u.LastUpdatedBy,
                                                             CreatedByName = u.CreatedByName,
                                                             UpdatedByName = u.UpdatedByName,
                                                             RoomName = u.RoomName,
                                                         }).AsParallel().ToList();
                ObjCache = obj; // CacheHelper<IEnumerable<PollAllFlagMasterDTO>>.AddCacheItem("Cached_PollAllFlagMaster_" + CompanyID.ToString(), obj);
            }
            //}

            return ObjCache;
        }

        /// <summary>
        /// Get Paged Records from the PollAllFlagMaster Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<PollAllFlagMasterDTO> GetAllRecords(Int64 CompanyId, Int64 RoomID)
        {
            return GetCachedData(CompanyId).Where(x => x.RoomID == RoomID).OrderBy("ID DESC");
        }

        /// <summary>
        /// Get Particullar Record from the PollAllFlagMaster by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public PollAllFlagMasterDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID)
        {
            return GetAllRecords(CompanyID, RoomID).Single(t => t.ID == id);
        }

        /// <summary>
        /// Insert Record in the DataBase PollAllFlagMaster
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 Insert(PollAllFlagMasterDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                PollAllFlagMaster obj = new PollAllFlagMaster();
                obj.ID = 0;
                obj.IsPollALL = objDTO.IsPollALL;
                obj.RoomID = objDTO.RoomID;
                obj.CompanyID = objDTO.CompanyID;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.CreatedOn = DateTimeUtility.DateTimeNow;
                obj.LastUpdatedOn = DateTimeUtility.DateTimeNow;
                obj.IsArchived = false;
                obj.IsDeleted = false;
                context.PollAllFlagMasters.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;

                //if (objDTO.ID > 0)
                //{
                //    //Get Cached-Media
                //    IEnumerable<PollAllFlagMasterDTO> ObjCache = CacheHelper<IEnumerable<PollAllFlagMasterDTO>>.GetCacheItem("Cached_PollAllFlagMaster_" + objDTO.CompanyID.ToString());
                //    if (ObjCache != null)
                //    {
                //        List<PollAllFlagMasterDTO> tempC = new List<PollAllFlagMasterDTO>();
                //        tempC.Add(objDTO);

                //        IEnumerable<PollAllFlagMasterDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                //        CacheHelper<IEnumerable<PollAllFlagMasterDTO>>.AppendToCacheItem("Cached_PollAllFlagMaster_" + objDTO.CompanyID.ToString(), NewCache);
                //    }
                //}

                return obj.ID;
            }

        }

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool Edit(PollAllFlagMasterDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                PollAllFlagMaster obj = context.PollAllFlagMasters.Where(x => x.ID == objDTO.ID).FirstOrDefault();
                obj.IsPollALL = objDTO.IsPollALL;
                obj.LastUpdatedOn = DateTimeUtility.DateTimeNow;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                context.Entry(obj).State = System.Data.Entity.EntityState.Unchanged;
                context.PollAllFlagMasters.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                ////Get Cached-Media
                //IEnumerable<PollAllFlagMasterDTO> ObjCache = CacheHelper<IEnumerable<PollAllFlagMasterDTO>>.GetCacheItem("Cached_PollAllFlagMaster_" + objDTO.CompanyID.ToString());
                //if (ObjCache != null)
                //{
                //    CacheHelper<IEnumerable<PollAllFlagMasterDTO>>.AppendToCacheItem("Cached_PollAllFlagMaster_" + objDTO.CompanyID.ToString(), new List<PollAllFlagMasterDTO>());
                //}


                return true;
            }
        }

        #region [for service]

        

        #endregion


    }
}


