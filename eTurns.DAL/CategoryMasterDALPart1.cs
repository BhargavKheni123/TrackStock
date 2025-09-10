using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eTurns.DTO;
using System.Data;
using Dynamite;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Collections;
using eTurns.DTO.Resources;
using System.Web;
using System.Data.SqlClient;

namespace eTurns.DAL
{
    public partial class CategoryMasterDAL : eTurnsBaseDAL
    {
        public IEnumerable<CategoryMasterDTO> GetCachedData(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, bool? IsForBom)
        {
            IEnumerable<CategoryMasterDTO> ObjCache;
            if (IsArchived == false && IsDeleted == false)
            {
                //Get Cached-Media
                ObjCache = CacheHelper<IEnumerable<CategoryMasterDTO>>.GetCacheItem("Cached_CategoryMaster_" + CompanyID.ToString());
                if (ObjCache == null)
                {
                    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                    {
                        IEnumerable<CategoryMasterDTO> obj = (from u in context.Database.SqlQuery<CategoryMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,'' AS CreatedDate,'' AS UpdatedDAte FROM CategoryMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.CompanyID = " + CompanyID.ToString())
                                                              select new CategoryMasterDTO
                                                              {
                                                                  ID = u.ID,
                                                                  Category = u.Category,
                                                                  Created = u.Created,
                                                                  Updated = u.Updated,
                                                                  CreatedByName = u.CreatedByName,
                                                                  UpdatedByName = u.UpdatedByName,
                                                                  Room = u.Room,
                                                                  RoomName = u.RoomName,
                                                                  CreatedBy = u.CreatedBy,
                                                                  LastUpdatedBy = u.LastUpdatedBy,
                                                                  CompanyID = u.CompanyID,
                                                                  IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                                                  IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                                                  CategoryColor = u.CategoryColor,
                                                                  GUID = u.GUID,
                                                                  UDF1 = u.UDF1,
                                                                  UDF2 = u.UDF2,
                                                                  UDF3 = u.UDF3,
                                                                  UDF4 = u.UDF4,
                                                                  UDF5 = u.UDF5,
                                                                  isForBOM = u.isForBOM,
                                                                  RefBomId = u.RefBomId,
                                                                  CreatedDate = u.CreatedDate,
                                                                  UpdatedDate = u.UpdatedDate,
                                                                  ReceivedOn = u.ReceivedOn,
                                                                  ReceivedOnWeb = u.ReceivedOnWeb,
                                                                  AddedFrom = u.AddedFrom,
                                                                  EditedFrom = u.EditedFrom,
                                                              }).AsParallel().ToList();
                        ObjCache = CacheHelper<IEnumerable<CategoryMasterDTO>>.AddCacheItem("Cached_CategoryMaster_" + CompanyID.ToString(), obj);
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
                    ObjCache = (from u in context.Database.SqlQuery<CategoryMasterDTO>(@"SELECT A.ID,A.Category,A.isForBOM, A.Created, A.CreatedBy,A.Room, A.LastUpdatedBy, A.Updated,A.CompanyID,A.UDF1,A.UDF2,A.UDF3,A.UDF4,A.UDF5, A.IsDeleted, A.IsArchived, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, A.GUID,'' AS CreatedDate,'' As UpdatedDAte FROM CategoryMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.CompanyID = " + CompanyID.ToString() + " AND " + sSQL)
                                select new CategoryMasterDTO
                                {
                                    ID = u.ID,
                                    Category = u.Category,
                                    Created = u.Created,
                                    Updated = u.Updated,
                                    CreatedByName = u.CreatedByName,
                                    UpdatedByName = u.UpdatedByName,
                                    RoomName = u.RoomName,
                                    CreatedBy = u.CreatedBy,
                                    LastUpdatedBy = u.LastUpdatedBy,
                                    Room = u.Room,
                                    CompanyID = u.CompanyID,
                                    IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                    IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                    GUID = u.GUID,
                                    UDF1 = u.UDF1,
                                    UDF2 = u.UDF2,
                                    UDF3 = u.UDF3,
                                    UDF4 = u.UDF4,
                                    UDF5 = u.UDF5,
                                    CategoryColor = u.CategoryColor,
                                    isForBOM = u.isForBOM,
                                    RefBomId = u.RefBomId,
                                    CreatedDate = u.CreatedDate,
                                    UpdatedDate = u.UpdatedDate,
                                    ReceivedOn = u.ReceivedOn,
                                    ReceivedOnWeb = u.ReceivedOnWeb,
                                    AddedFrom = u.AddedFrom,
                                    EditedFrom = u.EditedFrom,
                                }).AsParallel().ToList();
                }
            }
            if (IsForBom == null)
            {
                return ObjCache;
            }
            else if (IsForBom == true)
            {
                return ObjCache.Where(t => t.Room == 0);
            }
            else
            {
                return ObjCache.Where(t => t.Room == RoomID && t.isForBOM == false);
            }
        }
        public IEnumerable<CategoryMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, bool IsForBom)
        {
            return GetCachedData(RoomID, CompanyId, IsArchived, IsDeleted, IsForBom).OrderBy("ID DESC");
        }
        public IEnumerable<CategoryMasterDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, bool IsForBom)
        {
            IEnumerable<CategoryMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId, IsArchived, IsDeleted, IsForBom);
            if (IsForBom)
            {
                ObjCache = ObjCache.Where(t => (t.IsArchived == IsArchived && t.IsDeleted == IsDeleted && t.isForBOM == IsForBom && t.Room == 0));
            }
            else
            {
                ObjCache = ObjCache.Where(t => (t.IsArchived == IsArchived && t.IsDeleted == IsDeleted));
            }
            if (String.IsNullOrEmpty(SearchTerm))
            {
                //Get Cached-Media
                //IEnumerable<CategoryMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId);
                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media
                //IEnumerable<CategoryMasterDTO> ObjCache = GetCachedData(RoomID,CompanyId);
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                //CreatedBy,UpdatedBy,DateCreatedFrom,DateUpdatedFrom,UDF1,UDF2,UDF3,UDF4,UDF5,[###]admin,niraj$$$$$$$test2$$
                // 1 = createdBy, 2 = updatedby , 3 = CreatedFrom, 4 = CreatedTo, 5=UpdatedFrom,6=UpdatedTo
                if (Fields.Length > 2)
                {
                    if (!string.IsNullOrEmpty(Fields[2]))
                        SearchTerm = Fields[2];
                    else
                        SearchTerm = string.Empty;
                }
                else
                {
                    SearchTerm = string.Empty;
                }
                ObjCache = ObjCache.Where(t =>
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
                    && ((Fields[1].Split('@')[2] == "") || (t.Created.Value >= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[0], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))) && t.Created.Value <= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[1], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"])))))
                    && ((Fields[1].Split('@')[3] == "") || (t.Updated.Value >= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[0], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))) && t.Updated.Value <= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[1], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"])))))
                    && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').ToList().Contains(t.UDF1)))
                    && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').ToList().Contains(t.UDF2)))
                    && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').ToList().Contains(t.UDF3)))
                    && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').ToList().Contains(t.UDF4)))
                    && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').ToList().Contains(t.UDF5)))
                    && (SearchTerm == "" || (t.ID.ToString().Contains(SearchTerm) ||
                        t.Category.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        //------------------------------------------------------------------------------------------------
                        (t.RoomName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.CreatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UpdatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0))
                    );

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                //Get Cached-Media
                //IEnumerable<CategoryMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId);
                TotalCount = ObjCache.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm) ||
                        t.Category.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        //------------------------------------------------------------------------------------------------
                        (t.RoomName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.CreatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UpdatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    //------------------------------------------------------------------------------------------------
                    ).Count();
                return ObjCache.Where
                    (t => t.ID.ToString().Contains(SearchTerm) ||
                        t.Category.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        //------------------------------------------------------------------------------------------------
                        (t.RoomName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.CreatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UpdatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    //------------------------------------------------------------------------------------------------
                    ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }
        }
        public CategoryMasterDTO GetRecord(string Category, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, bool IsForBom)
        {
            return GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted, IsForBom).Where(t => t.Category.ToLower() == Category.ToLower()).FirstOrDefault();
        }
        public bool Delete(Int64 id, Int64 userid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                CategoryMaster obj = context.CategoryMasters.Single(t => t.ID == id);
                obj.IsDeleted = true;
                obj.IsArchived = false;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.LastUpdatedBy = userid;
                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                obj.EditedFrom = "Web";
                context.CategoryMasters.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                //Get Cached-Media
                IEnumerable<CategoryMasterDTO> ObjCache = CacheHelper<IEnumerable<CategoryMasterDTO>>.GetCacheItem("Cached_CategoryMaster_" + obj.CompanyID.ToString());
                if (ObjCache != null)
                {
                    List<CategoryMasterDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => i.ID == id);
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<CategoryMasterDTO>>.AppendToCacheItem("Cached_CategoryMaster_" + obj.CompanyID.ToString(), ObjCache);
                }

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
                        strQuery += "UPDATE CategoryMaster SET Updated = '" + DateTime.Now.ToString() + "' , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1, ReceivedOn=GETUTCDATE(),EditedFrom='Web' WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.Database.ExecuteSqlCommand(strQuery);

                //Get Cached-Media
                IEnumerable<CategoryMasterDTO> ObjCache = CacheHelper<IEnumerable<CategoryMasterDTO>>.GetCacheItem("Cached_CategoryMaster_" + CompanyId.ToString());
                if (ObjCache != null)
                {
                    List<CategoryMasterDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => IDs.Split(',').Contains(i.ID.ToString()));
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<CategoryMasterDTO>>.AppendToCacheItem("Cached_CategoryMaster_" + CompanyId.ToString(), ObjCache);
                }

                return true;
            }
        }

        //public CategoryMasterDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, bool? IsForBom)
        //{
        //    return GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted, IsForBom).Where(t => t.ID == id).SingleOrDefault(); //.Single(t => t.ID == id);

        //    //using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    //{
        //    //    return (from u in context.Database.SqlQuery<CategoryMasterDTO>(@"SELECT A.ID,A.Category, A.Created, A.CreatedBy,A.Room, A.LastUpdatedBy, A.Updated,A.CompanyID,A.UDF1,A.UDF2,A.UDF3,A.UDF4,A.UDF5, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, A.GUID FROM CategoryMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.ID=" + id.ToString())
        //    //            select new CategoryMasterDTO
        //    //            {
        //    //                ID = u.ID,
        //    //                Category = u.Category,
        //    //                Created = u.Created,
        //    //                Updated = u.Updated,
        //    //                CreatedByName = u.CreatedByName,
        //    //                UpdatedByName = u.UpdatedByName,
        //    //                RoomName = u.RoomName,
        //    //                LastUpdatedBy = u.LastUpdatedBy,
        //    //                CreatedBy = u.CreatedBy,
        //    //                Room = u.Room,
        //    //                GUID = u.GUID,
        //    //                CompanyID = u.CompanyID,
        //    //                UDF1 = u.UDF1,
        //    //                UDF2 = u.UDF2,
        //    //                UDF3 = u.UDF3,
        //    //                UDF4 = u.UDF4,
        //    //                UDF5 = u.UDF5
        //    //            }).SingleOrDefault();
        //    //}
        //}

        //public string UpdateData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        //{
        //    using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {
        //        string strQuery = "UPDATE CategoryMaster SET " + columnName + " = '" + value + "', ReceivedOn=GETUTCDATE(),EditedFrom='Web', Updated = getutcdate() WHERE ID=" + id;
        //        context.Database.ExecuteSqlCommand(strQuery);
        //    }
        //    return value;
        //}

        #region [for Service]

        //public CategoryMasterDTO GetRecordForService(Int64 id, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string ConnectionString)
        //{
        //    using (var context = new eTurnsEntities(ConnectionString))
        //    {
        //        return (from u in context.Database.SqlQuery<CategoryMasterDTO>(@"SELECT A.ID,A.Category, A.Created, A.CreatedBy,A.Room, A.LastUpdatedBy, A.Updated,A.CompanyID,A.UDF1,A.UDF2,A.UDF3,A.UDF4,A.UDF5, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, A.GUID FROM CategoryMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.ID=" + id.ToString())
        //                select new CategoryMasterDTO
        //                {
        //                    ID = u.ID,
        //                    Category = u.Category,
        //                    Created = u.Created,
        //                    Updated = u.Updated,
        //                    CreatedByName = u.CreatedByName,
        //                    UpdatedByName = u.UpdatedByName,
        //                    RoomName = u.RoomName,
        //                    LastUpdatedBy = u.LastUpdatedBy,
        //                    CreatedBy = u.CreatedBy,
        //                    Room = u.Room,
        //                    GUID = u.GUID,
        //                    CompanyID = u.CompanyID,
        //                    UDF1 = u.UDF1,
        //                    UDF2 = u.UDF2,
        //                    UDF3 = u.UDF3,
        //                    UDF4 = u.UDF4,
        //                    UDF5 = u.UDF5,
        //                    isForBOM = u.isForBOM,
        //                    RefBomId = u.RefBomId,
        //                    ReceivedOn = u.ReceivedOn,
        //                    ReceivedOnWeb = u.ReceivedOnWeb,
        //                    AddedFrom = u.AddedFrom,
        //                    EditedFrom = u.EditedFrom,
        //                }).SingleOrDefault();
        //    }
        //}

        #endregion
    }
}
