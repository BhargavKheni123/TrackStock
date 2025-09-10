using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Data.SqlClient;

namespace eTurns.DAL
{
    public partial class JobTypeMasterDAL : eTurnsBaseDAL
    {
        public IEnumerable<JobTypeMasterDTO> GetCachedData(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {

            IEnumerable<JobTypeMasterDTO> ObjCache;
            if (IsArchived == false && IsDeleted == false)
            {
                //Get Cached-Media
                ObjCache = CacheHelper<IEnumerable<JobTypeMasterDTO>>.GetCacheItem("Cached_JobTypeMaster_" + CompanyID.ToString());
                if (ObjCache == null)
                {
                    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                    {
                        IEnumerable<JobTypeMasterDTO> obj = (from u in context.ExecuteStoreQuery<JobTypeMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS LastUpdatedByName, D.RoomName FROM JobTypeMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.CompanyID = " + CompanyID.ToString())
                                                             select new JobTypeMasterDTO
                                                             {
                                                                 JobType = u.JobType,
                                                                 Created = u.Created,
                                                                 CreatedBy = u.CreatedBy,
                                                                 ID = u.ID,
                                                                 GUID = u.GUID,
                                                                 LastUpdatedBy = u.LastUpdatedBy,
                                                                 Room = u.Room,
                                                                 LastUpdated = u.LastUpdated,
                                                                 CreatedByName = u.CreatedByName,
                                                                 LastUpdatedByName = u.LastUpdatedByName,
                                                                 RoomName = u.RoomName,
                                                                 CompanyID = u.CompanyID,
                                                                 IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                                                 IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                                                 UDF1 = u.UDF1,
                                                                 UDF2 = u.UDF2,
                                                                 UDF3 = u.UDF3,
                                                                 UDF4 = u.UDF4,
                                                                 UDF5 = u.UDF5
                                                             }).AsParallel().ToList();
                        ObjCache = CacheHelper<IEnumerable<JobTypeMasterDTO>>.AddCacheItem("Cached_JobTypeMaster_" + CompanyID.ToString(), obj);
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
                    sSQL += "A.IsDeleted =1";
                }

                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    ObjCache = (from u in context.ExecuteStoreQuery<JobTypeMasterDTO>(@"SELECT A.ID,A.JobType, A.Created, A.CreatedBy,A.Room, A.LastUpdatedBy, A.LastUpdated,A.CompanyID,A.UDF1,A.UDF2,A.UDF3,A.UDF4,A.UDF5, A.IsDeleted, A.IsArchived, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, A.GUID FROM JobTypeMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.CompanyID = " + CompanyID.ToString() + " AND " + sSQL)
                                select new JobTypeMasterDTO
                                {
                                    JobType = u.JobType,
                                    Created = u.Created,
                                    CreatedBy = u.CreatedBy,
                                    ID = u.ID,
                                    GUID = u.GUID,
                                    LastUpdatedBy = u.LastUpdatedBy,
                                    Room = u.Room,
                                    LastUpdated = u.LastUpdated,
                                    CreatedByName = u.CreatedByName,
                                    LastUpdatedByName = u.LastUpdatedByName,
                                    RoomName = u.RoomName,
                                    CompanyID = u.CompanyID,
                                    IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                    IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
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

        public IEnumerable<JobTypeMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        {
            return GetCachedData(RoomID, CompanyId, IsArchived, IsDeleted).OrderBy("ID DESC");
        }

        public IEnumerable<JobTypeMasterDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        {
            IEnumerable<JobTypeMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId, IsArchived, IsDeleted);
            if (String.IsNullOrEmpty(SearchTerm))
            {
                //Get Cached-Media
                //IEnumerable<JobTypeMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId);
                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media
                //IEnumerable<JobTypeMasterDTO> ObjCache = GetCachedData(RoomID,CompanyId);
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                //CreatedBy,UpdatedBy,DateCreatedFrom,DateUpdatedFrom,UDF1,UDF2,UDF3,UDF4,UDF5,[###]admin,niraj$$$$$$$test2$$
                // 1 = createdBy, 2 = updatedby , 3 = CreatedFrom, 4 = CreatedTo, 5=UpdatedFrom,6=UpdatedTo

                ObjCache = ObjCache.Where(t =>
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.LastUpdatedByName)))
                    && ((Fields[1].Split('@')[2] == "") || (t.Created.Date >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created.Date <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[3] == "") || (t.LastUpdated.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.LastUpdated.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
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
                //IEnumerable<JobTypeMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId);
                TotalCount = ObjCache.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm) ||
                        t.JobType.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).Count();
                return ObjCache.Where
                    (t => t.ID.ToString().Contains(SearchTerm) ||
                        t.JobType.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }
        }

        public JobTypeMasterDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            return GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted).Single(t => t.ID == id);

            //using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            //{

            //    return (from u in context.ExecuteStoreQuery<JobTypeMasterDTO>(@"SELECT A.ID,A.JobType, A.Created, A.CreatedBy,A.Room, A.LastUpdatedBy, A.LastUpdated,A.CompanyID,A.UDF1,A.UDF2,A.UDF3,A.UDF4,A.UDF5, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, A.GUID FROM JobTypeMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.ID=" + id.ToString())
            //            select new JobTypeMasterDTO
            //            {
            //                JobType = u.JobType,
            //                Created = u.Created,
            //                CreatedBy = u.CreatedBy,
            //                ID = u.ID,
            //                GUID = u.GUID,
            //                LastUpdatedBy = u.LastUpdatedBy,
            //                Room = u.Room,
            //                LastUpdated = u.LastUpdated,
            //                CreatedByName = u.CreatedByName,
            //                LastUpdatedByName = u.LastUpdatedByName,
            //                RoomName = u.RoomName,
            //                CompanyID = u.CompanyID,
            //                UDF1 = u.UDF1,
            //                UDF2 = u.UDF2,
            //                UDF3 = u.UDF3,
            //                UDF4 = u.UDF4,
            //                UDF5 = u.UDF5
            //            }).SingleOrDefault();
            //}
        }

        public bool Delete(Int64 id, Int64 userid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                MessageDTO objMSG = new MessageDTO();

                JobTypeMaster obj = context.JobTypeMasters.Single(t => t.ID == id);
                obj.IsDeleted = true;
                obj.IsArchived = false;
                obj.LastUpdated = DateTimeUtility.DateTimeNow;
                obj.LastUpdatedBy = userid;
                context.ObjectStateManager.ChangeObjectState(obj, EntityState.Unchanged);
                context.JobTypeMasters.Attach(obj);
                context.ObjectStateManager.ChangeObjectState(obj, EntityState.Modified);
                context.SaveChanges();

                return true;
            }
        }


        public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyId)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE JobTypeMaster SET LastUpdated = '" + DateTime.Now.ToString() + "' , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.ExecuteStoreCommand(strQuery);

                return true;
            }
        }

        /// <summary>
        /// UpdateData
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <param name="rowId"></param>
        /// <param name="columnPosition"></param>
        /// <param name="columnId"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public string UpdateData(Int64 id, string value, int updateByID, string columnName)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                string strQuery = "UPDATE JobTypeMaster SET " + columnName + " = '" + value + "', LastUpdatedBy= " + updateByID + " LastUpdated = DATETIME ('" + System.DateTime.Now.ToString(Convert.ToString((new System.Configuration.AppSettingsReader()).GetValue("FormatDateTimetoStore", typeof(System.String)))) + "') WHERE ID= " + id + "";
                context.ExecuteStoreCommand(strQuery);
                context.SaveChanges();
            }

            return value;
        }
    }
}
