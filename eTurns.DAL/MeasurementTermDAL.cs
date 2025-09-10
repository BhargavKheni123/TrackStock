using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace eTurns.DAL
{
    public class MeasurementTermDAL : eTurnsBaseDAL
    {

        #region [Class Constructor]

        public MeasurementTermDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public MeasurementTermDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
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
        public IEnumerable<MeasurementTermMasterDTO> GetCachedData(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            IEnumerable<MeasurementTermMasterDTO> ObjCache;
            if (IsArchived == false && IsDeleted == false)
            {
                //Get Cached-Media
                ObjCache = CacheHelper<IEnumerable<MeasurementTermMasterDTO>>.GetCacheItem("Cached_MeasurementTermMaster_" + CompanyID.ToString());
                if (ObjCache == null)
                {
                    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                    {
                        IEnumerable<MeasurementTermMasterDTO> obj = (from u in context.Database.SqlQuery<MeasurementTermMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM MeasurementTermMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.CompanyID = " + CompanyID.ToString())
                                                                     select new MeasurementTermMasterDTO
                                                                     {
                                                                         ID = u.ID,
                                                                         MeasurementTerm = u.MeasurementTerm,
                                                                         Created = u.Created,
                                                                         Updated = u.Updated,
                                                                         CreatedBy = u.CreatedBy,
                                                                         LastUpdatedBy = u.LastUpdatedBy,
                                                                         CreatedByName = u.CreatedByName,
                                                                         UpdatedByName = u.UpdatedByName,
                                                                         RoomName = u.RoomName,
                                                                         Room = u.Room,
                                                                         CompanyID = u.CompanyID,
                                                                         IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                                                         IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                                                         GUID = u.GUID,
                                                                         UDF1 = u.UDF1,
                                                                         UDF2 = u.UDF2,
                                                                         UDF3 = u.UDF3,
                                                                         UDF4 = u.UDF4,
                                                                         UDF5 = u.UDF5
                                                                     }).AsParallel().ToList();
                        ObjCache = CacheHelper<IEnumerable<MeasurementTermMasterDTO>>.AddCacheItem("Cached_MeasurementTermMaster_" + CompanyID.ToString(), obj);
                    }
                }
            }
            else
            {
                string sSQL = "";
                if (IsArchived && IsDeleted)
                {
                    sSQL += "A.IsDeleted = 1 AND A.IsArchived = 1";
                }
                else if (IsArchived)
                {
                    sSQL += "A.IsArchived = 1";
                }
                else if (IsDeleted)
                {
                    sSQL += "A.IsDeleted = 1";
                }

                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    ObjCache = (from u in context.Database.SqlQuery<MeasurementTermMasterDTO>(@"SELECT A.ID,A.MeasurementTerm, A.Created, A.CreatedBy,A.Room,A.UDF1,A.UDF2,A.UDF3,A.UDF4,A.UDF5, A.IsDeleted, A.IsArchived, A.LastUpdatedBy, A.Updated,A.CompanyID, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, A.GUID FROM MeasurementTermMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.CompanyID = " + CompanyID.ToString() + @" AND " + sSQL)
                                select new MeasurementTermMasterDTO
                                {
                                    ID = u.ID,
                                    MeasurementTerm = u.MeasurementTerm,
                                    Created = u.Created,
                                    Updated = u.Updated,
                                    CreatedBy = u.CreatedBy,
                                    LastUpdatedBy = u.LastUpdatedBy,
                                    CreatedByName = u.CreatedByName,
                                    UpdatedByName = u.UpdatedByName,
                                    RoomName = u.RoomName,
                                    Room = u.Room,
                                    CompanyID = u.CompanyID,
                                    IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                    IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                    GUID = u.GUID,
                                    UDF1 = u.UDF1,
                                    UDF2 = u.UDF2,
                                    UDF3 = u.UDF3,
                                    UDF4 = u.UDF4,
                                    UDF5 = u.UDF5
                                }).AsParallel().ToList();
                }

            }

            return ObjCache.Where(t => t.Room == RoomID);
        }

        /// <summary>
        /// Get Paged Records from the MeasurementTerm Master Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<MeasurementTermMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        {
            return GetCachedData(RoomID, CompanyId, IsArchived, IsDeleted).OrderBy("ID DESC");
        }

        /// <summary>
        /// Get Paged Records from the MeasurementTerm Master Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<MeasurementTermMasterDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            IEnumerable<MeasurementTermMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId, IsArchived, IsDeleted);

            if (String.IsNullOrEmpty(SearchTerm))
            {
                //Get Cached-Media
                //IEnumerable<MeasurementTermMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId);
                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media
                //IEnumerable<MeasurementTermMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId);
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                //CreatedBy,UpdatedBy,DateCreatedFrom,DateUpdatedFrom,UDF1,UDF2,UDF3,UDF4,UDF5,[###]admin,niraj$$$$$$$test2$$
                // 1 = createdBy, 2 = updatedby , 3 = CreatedFrom, 4 = CreatedTo, 5=UpdatedFrom,6=UpdatedTo

                ObjCache = ObjCache.Where(t =>
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
                    && ((Fields[1].Split('@')[2] == "") || (t.Created.Value >= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone) && t.Created.Value <= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone)))
                    && ((Fields[1].Split('@')[3] == "") || (t.Updated.Value >= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone) && t.Updated.Value <= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone)))
                    && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').ToList().Contains(t.UDF1)))
                    && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').ToList().Contains(t.UDF2)))
                    && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').ToList().Contains(t.UDF3)))
                    && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').ToList().Contains(t.UDF4)))
                    && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').ToList().Contains(t.UDF5)))
                    );

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                //Get Cached-Media
                //IEnumerable<MeasurementTermMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId);
                TotalCount = ObjCache.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm) ||
                        t.MeasurementTerm.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).Count();
                return ObjCache.Where
                    (t => t.ID.ToString().Contains(SearchTerm) ||
                        t.MeasurementTerm.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }
        }

        /// <summary>
        /// Get Particullar Record from the MeasurementTerm Master by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public MeasurementTermMasterDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            return GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted).Single(t => t.ID == id);

            //using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            //{
            //    return (from u in context.Database.SqlQuery<MeasurementTermMasterDTO>(@"SELECT A.ID,A.MeasurementTerm, A.Created, A.CreatedBy,A.Room,A.UDF1,A.UDF2,A.UDF3,A.UDF4,A.UDF5, A.LastUpdatedBy, A.Updated,A.CompanyID, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, A.GUID FROM MeasurementTermMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.ID=" + id.ToString())
            //            select new MeasurementTermMasterDTO
            //            {
            //                ID = u.ID,
            //                MeasurementTerm = u.MeasurementTerm,
            //                Created = u.Created,
            //                Updated = u.Updated,
            //                CreatedByName = u.CreatedByName,
            //                UpdatedByName = u.UpdatedByName,
            //                RoomName = u.RoomName,
            //                LastUpdatedBy = u.LastUpdatedBy,
            //                CreatedBy = u.CreatedBy,
            //                Room = u.Room,
            //                GUID = u.GUID,
            //                CompanyID = u.CompanyID,
            //                UDF1 = u.UDF1,
            //                UDF2 = u.UDF2,
            //                UDF3 = u.UDF3,
            //                UDF4 = u.UDF4,
            //                UDF5 = u.UDF5
            //            }).SingleOrDefault();
            //}
        }

        /// <summary>
        /// Insert Record in the DataBase MeasurementTerm Master
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 Insert(MeasurementTermMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;

            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.Created = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                MeasurementTermMaster obj = new MeasurementTermMaster();
                obj.ID = 0;
                obj.MeasurementTerm = objDTO.MeasurementTerm;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.CompanyID = objDTO.CompanyID;
                obj.Updated = objDTO.Updated;
                obj.Created = objDTO.Created;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
                obj.GUID = Guid.NewGuid();
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                context.MeasurementTermMasters.Add(obj);
                context.SaveChanges();

                objDTO.ID = obj.ID;
                if (objDTO.ID > 0)
                {
                    //Get Cached-Media
                    IEnumerable<MeasurementTermMasterDTO> ObjCache = CacheHelper<IEnumerable<MeasurementTermMasterDTO>>.GetCacheItem("Cached_MeasurementTermMaster_" + objDTO.CompanyID.ToString());
                    if (ObjCache != null)
                    {
                        List<MeasurementTermMasterDTO> tempC = new List<MeasurementTermMasterDTO>();
                        tempC.Add(objDTO);

                        IEnumerable<MeasurementTermMasterDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                        CacheHelper<IEnumerable<MeasurementTermMasterDTO>>.AppendToCacheItem("Cached_MeasurementTermMaster_" + objDTO.CompanyID.ToString(), NewCache);
                    }
                }

                return obj.ID;
            }
        }

        /// <summary>
        /// Delete Particullar Record
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool Delete(Int64 id, Int64 userid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                MeasurementTermMaster obj = context.MeasurementTermMasters.Single(t => t.ID == id);
                obj.IsDeleted = true;
                obj.IsArchived = false;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.LastUpdatedBy = userid;
                context.Entry(obj).State = System.Data.Entity.EntityState.Unchanged;
                context.MeasurementTermMasters.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                //Get Cached-Media
                IEnumerable<MeasurementTermMasterDTO> ObjCache = CacheHelper<IEnumerable<MeasurementTermMasterDTO>>.GetCacheItem("Cached_MeasurementTermMaster_" + obj.CompanyID.ToString());
                if (ObjCache != null)
                {
                    List<MeasurementTermMasterDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => i.ID == id);
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<MeasurementTermMasterDTO>>.AppendToCacheItem("Cached_MeasurementTermMaster_" + obj.CompanyID.ToString(), ObjCache);
                }

                return true;
            }
        }

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool Edit(MeasurementTermMasterDTO objDTO)
        {

            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
            objDTO.Updated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                MeasurementTermMaster obj = new MeasurementTermMaster();
                obj.ID = objDTO.ID;
                obj.MeasurementTerm = objDTO.MeasurementTerm;

                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.CompanyID = objDTO.CompanyID;
                obj.Updated = objDTO.Updated;
                obj.Created = objDTO.Created;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.GUID = objDTO.GUID;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;

                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;

                context.MeasurementTermMasters.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                //Get Cached-Media
                IEnumerable<MeasurementTermMasterDTO> ObjCache = CacheHelper<IEnumerable<MeasurementTermMasterDTO>>.GetCacheItem("Cached_MeasurementTermMaster_" + objDTO.CompanyID.ToString());
                if (ObjCache != null)
                {
                    List<MeasurementTermMasterDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => i.ID == objDTO.ID);
                    ObjCache = objTemp.AsEnumerable();

                    List<MeasurementTermMasterDTO> tempC = new List<MeasurementTermMasterDTO>();
                    tempC.Add(objDTO);
                    IEnumerable<MeasurementTermMasterDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                    CacheHelper<IEnumerable<MeasurementTermMasterDTO>>.AppendToCacheItem("Cached_MeasurementTermMaster_" + objDTO.CompanyID.ToString(), NewCache);
                }

                return true;
            }
        }

        /// <summary>
        /// Delete Multiple Records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyId)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE MeasurementTermMaster SET Updated = '" + DateTime.Now.ToString() + "' , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.Database.ExecuteSqlCommand(strQuery);

                //Get Cached-Media
                IEnumerable<MeasurementTermMasterDTO> ObjCache = CacheHelper<IEnumerable<MeasurementTermMasterDTO>>.GetCacheItem("Cached_MeasurementTermMaster_" + CompanyId.ToString());
                if (ObjCache != null)
                {
                    List<MeasurementTermMasterDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => IDs.Split(',').Contains(i.ID.ToString()));
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<MeasurementTermMasterDTO>>.AppendToCacheItem("Cached_MeasurementTermMaster_" + CompanyId.ToString(), ObjCache);
                }

                return true;
            }
        }

        /// <summary>
        /// Update Data - Grid Update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <param name="rowId"></param>
        /// <param name="columnPosition"></param>
        /// <param name="columnId"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public string UpdateData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "UPDATE MeasurementTermMaster SET " + columnName + " = '" + value + "', Updated = DATETIME ('" + System.DateTime.Now.ToString(Convert.ToString((new System.Configuration.AppSettingsReader()).GetValue("FormatDateTimetoStore", typeof(System.String)))) + "') WHERE ID=" + id;
                context.Database.ExecuteSqlCommand(strQuery);
            }
            return value;
        }

    }
}
