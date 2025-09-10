using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace eTurns.DAL
{
    public class GXPRConsignedJobMasterDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        public GXPRConsignedJobMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public GXPRConsignedJobMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
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
        public IEnumerable<GXPRConsigmentJobMasterDTO> GetCachedData(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            IEnumerable<GXPRConsigmentJobMasterDTO> ObjCache;
            if (IsArchived == false && IsDeleted == false)
            {
                ObjCache = CacheHelper<IEnumerable<GXPRConsigmentJobMasterDTO>>.GetCacheItem("Cached_GXPRConsigmentJobMaster_" + CompanyID.ToString());
                if (ObjCache == null)
                {
                    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                    {
                        IEnumerable<GXPRConsigmentJobMasterDTO> obj = (from u in context.Database.SqlQuery<GXPRConsigmentJobMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM GXPRConsigmentJobMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.CompanyID = " + CompanyID.ToString())
                                                                       select new GXPRConsigmentJobMasterDTO
                                                                       {
                                                                           ID = u.ID,
                                                                           GXPRConsigmentJob = u.GXPRConsigmentJob,
                                                                           Created = u.Created,
                                                                           CreatedBy = u.CreatedBy,
                                                                           LastUpdatedBy = u.LastUpdatedBy,
                                                                           Room = u.Room,
                                                                           Updated = u.Updated,
                                                                           CreatedByName = u.CreatedByName,
                                                                           UpdatedByName = u.UpdatedByName,
                                                                           RoomName = u.RoomName,
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
                        ObjCache = CacheHelper<IEnumerable<GXPRConsigmentJobMasterDTO>>.AddCacheItem("Cached_GXPRConsigmentJobMaster_" + CompanyID.ToString(), obj);
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
                    ObjCache = (from u in context.Database.SqlQuery<GXPRConsigmentJobMasterDTO>(@"SELECT A.ID,A.GXPRConsigmentJob, A.Created, A.CreatedBy,A.Room,A.UDF1,A.UDF2,A.UDF3,A.UDF4,A.UDF5,A.IsDeleted, A.IsArchived, A.LastUpdatedBy, A.Updated,A.CompanyID, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, A.GUID FROM GXPRConsigmentJobMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.CompanyID = " + CompanyID.ToString() + " AND " + sSQL)
                                select new GXPRConsigmentJobMasterDTO
                                {
                                    ID = u.ID,
                                    GXPRConsigmentJob = u.GXPRConsigmentJob,
                                    Created = u.Created,
                                    CreatedBy = u.CreatedBy,
                                    LastUpdatedBy = u.LastUpdatedBy,
                                    Room = u.Room,
                                    Updated = u.Updated,
                                    CreatedByName = u.CreatedByName,
                                    UpdatedByName = u.UpdatedByName,
                                    RoomName = u.RoomName,
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

            //Get Cached-Media
            return ObjCache.Where(t => t.Room == RoomID);
        }

        /// <summary>
        /// Get Paged Records from the GXPRConsigmentJob Master Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<GXPRConsigmentJobMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        {
            return GetCachedData(RoomID, CompanyId, IsArchived, IsDeleted).OrderBy("ID DESC");

            //using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            //{
            //    return (from u in context.Database.SqlQuery<GXPRConsigmentJobMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM GXPRConsigmentJobMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.CompanyID = " + CompanyId.ToString() + @" AND A.Room = " + RoomID.ToString())
            //            select new GXPRConsigmentJobMasterDTO
            //            {
            //                ID = u.ID,
            //                GXPRConsigmentJob = u.GXPRConsigmentJob,
            //                Created = u.Created,
            //                Updated = u.Updated,
            //                CreatedByName = u.CreatedByName,
            //                UpdatedByName = u.UpdatedByName,
            //                RoomName = u.RoomName
            //            }).AsParallel().ToList();

            //}
        }

        /// <summary>
        /// Get Paged Records from the GXPRConsigmentJob Master Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<GXPRConsigmentJobMasterDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        {
            IEnumerable<GXPRConsigmentJobMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId, IsArchived, IsDeleted);

            if (String.IsNullOrEmpty(SearchTerm))
            {
                //Get Cached-Media
                //IEnumerable<GXPRConsigmentJobMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId);
                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media
                //IEnumerable<GXPRConsigmentJobMasterDTO> ObjCache = GetCachedData(RoomID,CompanyId);
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                //CreatedBy,UpdatedBy,DateCreatedFrom,DateUpdatedFrom,UDF1,UDF2,UDF3,UDF4,UDF5,[###]admin,niraj$$$$$$$test2$$
                // 1 = createdBy, 2 = updatedby , 3 = CreatedFrom, 4 = CreatedTo, 5=UpdatedFrom,6=UpdatedTo

                ObjCache = ObjCache.Where(t =>
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
                    && ((Fields[1].Split('@')[2] == "") || (t.Created.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[3] == "") || (t.Updated.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.Updated.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
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
                //IEnumerable<GXPRConsigmentJobMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId);
                TotalCount = ObjCache.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm) ||
                        t.GXPRConsigmentJob.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).Count();
                return ObjCache.Where
                    (t => t.ID.ToString().Contains(SearchTerm) ||
                        t.GXPRConsigmentJob.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }

            #region Previous Code
            //using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            //{
            //    string strSortinitializer = "";

            //    if (sortColumnName.ToUpper().Contains("CREATEDBYNAME"))
            //    {
            //        sortColumnName = "UserName";
            //        strSortinitializer = "B";
            //    }
            //    else if (sortColumnName.ToUpper().Contains("UPDATEDBYNAME"))
            //    {
            //        sortColumnName = "UserName";
            //        strSortinitializer = "C";
            //    }
            //    else if (sortColumnName.ToUpper().Contains("ROOMNAME"))
            //    {
            //        strSortinitializer = "D";
            //    }
            //    else
            //    {
            //        strSortinitializer = "A";
            //    }

            //    strSortinitializer = strSortinitializer + "." + sortColumnName;


            //    if (String.IsNullOrEmpty(SearchTerm))
            //    {
            //        TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM GXPRConsigmentJobMaster WHERE IsDeleted!=1 AND IsArchived != 1 AND CompanyID = " + CompanyId.ToString() + @" AND Room = " + RoomID.ToString()).ToList()[0]);

            //        return (from u in context.Database.SqlQuery<GXPRConsigmentJobMasterDTO>(@"SELECT A.ID,A.GXPRConsigmentJob, A.Created, A.Updated,A.UDF1,A.UDF2,A.UDF3,A.UDF4,A.UDF5, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM GXPRConsigmentJobMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1  AND A.CompanyID = " + CompanyId.ToString() + @" AND A.Room = " + RoomID.ToString() + @" AND A.IsArchived != 1 ORDER BY " + strSortinitializer)
            //                select new GXPRConsigmentJobMasterDTO
            //                {
            //                    ID = u.ID,
            //                    GXPRConsigmentJob = u.GXPRConsigmentJob,
            //                    Created = u.Created,
            //                    Updated = u.Updated,
            //                    CreatedByName = u.CreatedByName,
            //                    UpdatedByName = u.UpdatedByName,
            //                    RoomName = u.RoomName,
            //                    UDF1 = u.UDF1,
            //                    UDF2 = u.UDF2,
            //                    UDF3 = u.UDF3,
            //                    UDF4 = u.UDF4,
            //                    UDF5 = u.UDF5
            //                }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();
            //    }
            //    else if (SearchTerm.Contains("STARTWITH#"))
            //    {
            //        string search = "";
            //        string[] dd = SearchTerm.Replace("STARTWITH#", "$").Split('$');
            //        string[] stringSeparators = new string[] { "[###]" };

            //        if (dd != null && dd.Length > 0)
            //        {
            //            string[] Fields = dd[1].Split(stringSeparators, StringSplitOptions.None);
            //            if (Fields != null && Fields.Length > 0)
            //            {
            //                // 6 counts for fields based on that prepare the search string
            //                // 1 = createdBy, 2 = updatedby , 3 = CreatedFrom, 4 = CreatedTo, 5=UpdatedFrom,6=UpdatedTo
            //                foreach (var item in Fields)
            //                {
            //                    if (item.Length > 0)
            //                    {
            //                        if (item.Contains("CreatedBy"))
            //                        {
            //                            search += " A.CreatedBy in (" + item.Split('#')[1] + ")";
            //                        }
            //                        if (item.Contains("UpdatedBy"))
            //                        {
            //                            if (search.Length > 0)
            //                                search += " AND ";
            //                            search += " A.LastUpdatedBy in (" + item.Split('#')[1] + ")";
            //                        }
            //                        if (item.Contains("DateCreatedFrom"))
            //                        {
            //                            if (search.Length > 0)
            //                                search += " AND ";
            //                            string[] CreatedDateFrom = item.Split('#');
            //                            search += " (A.Created >= DATETIME('" + CreatedDateFrom[1] + "') AND A.Created <= DATETIME('" + CreatedDateFrom[3] + "'))";
            //                        }
            //                        if (item.Contains("DateUpdatedFrom"))
            //                        {
            //                            if (search.Length > 0)
            //                                search += " AND ";
            //                            string[] UpdatedDateFrom = item.Split('#');
            //                            search += " (A.Updated >= DATETIME('" + UpdatedDateFrom[1] + "') AND A.Updated <= DATETIME('" + UpdatedDateFrom[3] + "'))";
            //                        }
            //                    }
            //                }
            //            }
            //        }

            //        if (search.Length > 0)
            //        {
            //            search = " AND (" + search + " )";
            //        }
            //        TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(A.ID) FROM GXPRConsigmentJobMaster as A WHERE A.IsDeleted!=1  AND A.CompanyID = " + CompanyId.ToString() + @" AND A.Room = " + RoomID.ToString() + @" AND A.IsArchived != 1 " + search + "").ToList()[0]);

            //        return (from u in context.Database.SqlQuery<GXPRConsigmentJobMasterDTO>("SELECT A.ID,A.GXPRConsigmentJob, A.Created, A.Updated,A.UDF1,A.UDF2,A.UDF3,A.UDF4,A.UDF5, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM GXPRConsigmentJobMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1  AND A.CompanyID = " + CompanyId.ToString() + @" AND A.Room = " + RoomID.ToString() + @" AND A.IsArchived != 1 " + search + " ORDER BY " + strSortinitializer)
            //                select new GXPRConsigmentJobMasterDTO
            //                {
            //                    ID = u.ID,
            //                    GXPRConsigmentJob = u.GXPRConsigmentJob,
            //                    Created = u.Created,
            //                    Updated = u.Updated,
            //                    CreatedByName = u.CreatedByName,
            //                    UpdatedByName = u.UpdatedByName,
            //                    RoomName = u.RoomName,
            //                    UDF1 = u.UDF1,
            //                    UDF2 = u.UDF2,
            //                    UDF3 = u.UDF3,
            //                    UDF4 = u.UDF4,
            //                    UDF5 = u.UDF5
            //                }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();
            //    }
            //    else
            //    {
            //        TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM GXPRConsigmentJobMaster WHERE IsDeleted!=1  AND CompanyID = " + CompanyId.ToString() + @" AND Room = " + RoomID.ToString() + @" AND IsArchived != 1 AND ((GXPRConsigmentJob like '%" + SearchTerm + "%') OR (ID like '%" + SearchTerm + "%'))").ToList()[0]);

            //        return (from u in context.Database.SqlQuery<GXPRConsigmentJobMasterDTO>("SELECT A.ID,A.GXPRConsigmentJob, A.Created, A.Updated,A.UDF1,A.UDF2,A.UDF3,A.UDF4,A.UDF5, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM GXPRConsigmentJobMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1  AND A.CompanyID = " + CompanyId.ToString() + @" AND A.Room = " + RoomID.ToString() + @" AND A.IsArchived != 1 AND ((A.GXPRConsigmentJob like '%" + SearchTerm + "%') OR (A.ID like '%" + SearchTerm + "%')) ORDER BY " + strSortinitializer)
            //                select new GXPRConsigmentJobMasterDTO
            //                {
            //                    ID = u.ID,
            //                    GXPRConsigmentJob = u.GXPRConsigmentJob,
            //                    Created = u.Created,
            //                    Updated = u.Updated,
            //                    CreatedByName = u.CreatedByName,
            //                    UpdatedByName = u.UpdatedByName,
            //                    RoomName = u.RoomName,
            //                    UDF1 = u.UDF1,
            //                    UDF2 = u.UDF2,
            //                    UDF3 = u.UDF3,
            //                    UDF4 = u.UDF4,
            //                    UDF5 = u.UDF5
            //                }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();
            //    }
            //}
            #endregion
        }

        /// <summary>
        /// Get Particullar Record from the GXPRConsignedJob Master by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public GXPRConsigmentJobMasterDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            return GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted).Single(t => t.ID == id);

            //using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            //{
            //    return (from u in context.Database.SqlQuery<GXPRConsigmentJobMasterDTO>(@"SELECT A.ID,A.GXPRConsigmentJob, A.Created, A.CreatedBy,A.Room,A.UDF1,A.UDF2,A.UDF3,A.UDF4,A.UDF5, A.LastUpdatedBy, A.Updated,A.CompanyID, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, A.GUID FROM GXPRConsigmentJobMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.ID=" + id.ToString())
            //            select new GXPRConsigmentJobMasterDTO
            //            {
            //                ID = u.ID,
            //                GXPRConsigmentJob = u.GXPRConsigmentJob,
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
        /// Insert Record in the DataBase GXPRConsigmentJob Master
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 Insert(GXPRConsigmentJobMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;

            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.Created = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                GXPRConsigmentJobMaster obj = new GXPRConsigmentJobMaster();
                obj.ID = 0;
                obj.GXPRConsigmentJob = objDTO.GXPRConsigmentJob;
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
                context.GXPRConsigmentJobMasters.Add(obj);
                context.SaveChanges();

                objDTO.ID = obj.ID;
                if (objDTO.ID > 0)
                {
                    //Get Cached-Media
                    IEnumerable<GXPRConsigmentJobMasterDTO> ObjCache = CacheHelper<IEnumerable<GXPRConsigmentJobMasterDTO>>.GetCacheItem("Cached_GXPRConsigmentJobMaster_" + objDTO.CompanyID.ToString());
                    if (ObjCache != null)
                    {
                        List<GXPRConsigmentJobMasterDTO> tempC = new List<GXPRConsigmentJobMasterDTO>();
                        tempC.Add(objDTO);

                        IEnumerable<GXPRConsigmentJobMasterDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                        CacheHelper<IEnumerable<GXPRConsigmentJobMasterDTO>>.AppendToCacheItem("Cached_GXPRConsigmentJobMaster_" + objDTO.CompanyID.ToString(), NewCache);
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
                GXPRConsigmentJobMaster obj = context.GXPRConsigmentJobMasters.Single(t => t.ID == id);
                obj.IsDeleted = true;
                obj.IsArchived = false;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.LastUpdatedBy = userid;
                context.Entry(obj).State = System.Data.Entity.EntityState.Unchanged;
                context.GXPRConsigmentJobMasters.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                //Get Cached-Media
                IEnumerable<GXPRConsigmentJobMasterDTO> ObjCache = CacheHelper<IEnumerable<GXPRConsigmentJobMasterDTO>>.GetCacheItem("Cached_GXPRConsigmentJobMaster_" + obj.CompanyID.ToString());
                if (ObjCache != null)
                {
                    List<GXPRConsigmentJobMasterDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => i.ID == id);
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<GXPRConsigmentJobMasterDTO>>.AppendToCacheItem("Cached_GXPRConsigmentJobMaster_" + obj.CompanyID.ToString(), ObjCache);
                }

                return true;
            }
        }

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool Edit(GXPRConsigmentJobMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
            objDTO.Updated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                GXPRConsigmentJobMaster obj = new GXPRConsigmentJobMaster();
                obj.ID = objDTO.ID;
                obj.GXPRConsigmentJob = objDTO.GXPRConsigmentJob;

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


                context.GXPRConsigmentJobMasters.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                //Get Cached-Media
                IEnumerable<GXPRConsigmentJobMasterDTO> ObjCache = CacheHelper<IEnumerable<GXPRConsigmentJobMasterDTO>>.GetCacheItem("Cached_GXPRConsigmentJobMaster_" + objDTO.CompanyID.ToString());
                if (ObjCache != null)
                {
                    List<GXPRConsigmentJobMasterDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => i.ID == objDTO.ID);
                    ObjCache = objTemp.AsEnumerable();

                    List<GXPRConsigmentJobMasterDTO> tempC = new List<GXPRConsigmentJobMasterDTO>();
                    tempC.Add(objDTO);
                    IEnumerable<GXPRConsigmentJobMasterDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                    CacheHelper<IEnumerable<GXPRConsigmentJobMasterDTO>>.AppendToCacheItem("Cached_GXPRConsigmentJobMaster_" + objDTO.CompanyID.ToString(), NewCache);
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
                        strQuery += "UPDATE GXPRConsigmentJobMaster SET Updated = '" + DateTime.Now.ToString() + "' , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.Database.ExecuteSqlCommand(strQuery);

                //Get Cached-Media
                IEnumerable<GXPRConsigmentJobMasterDTO> ObjCache = CacheHelper<IEnumerable<GXPRConsigmentJobMasterDTO>>.GetCacheItem("Cached_GXPRConsigmentJobMaster_" + CompanyId.ToString());
                if (ObjCache != null)
                {
                    List<GXPRConsigmentJobMasterDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => IDs.Split(',').Contains(i.ID.ToString()));
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<GXPRConsigmentJobMasterDTO>>.AppendToCacheItem("Cached_GXPRConsigmentJobMaster_" + CompanyId.ToString(), ObjCache);
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
                string strQuery = "UPDATE GXPRConsigmentJobMaster SET " + columnName + " = '" + value + "', Updated = DATETIME ('" + System.DateTime.Now.ToString(Convert.ToString((new System.Configuration.AppSettingsReader()).GetValue("FormatDateTimetoStore", typeof(System.String)))) + "') WHERE ID=" + id;
                context.Database.ExecuteSqlCommand(strQuery);
            }
            return value;
        }


    }
}
