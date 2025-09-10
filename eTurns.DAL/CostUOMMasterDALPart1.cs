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
using System.Web;
using eTurns.DTO.Resources;


namespace eTurns.DAL
{
    public partial class CostUOMMasterDAL : eTurnsBaseDAL
    {
        public CostUOMMasterDTO GetHistoryRecord(Int64 id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.Database.SqlQuery<CostUOMMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,'' AS CreatedDate,'' As UpdatedDate FROM CostUOMMaster_History A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.HistoryID=" + id.ToString())
                        select new CostUOMMasterDTO
                        {
                            ID = u.ID,
                            CostUOM = u.CostUOM,
                            CostUOMValue = u.CostUOMValue,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            GUID = u.GUID,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            CompanyID = u.CompanyID,
                            Room = u.Room,
                            CreatedDate = u.CreatedDate,
                            UpdatedDate = u.UpdatedDate,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            isForBOM = u.isForBOM,
                        }).SingleOrDefault();
            }
        }
        public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE CostUOMMaster SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1, ReceivedOn=GETUTCDATE(),EditedFrom='Web' WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.Database.ExecuteSqlCommand(strQuery);


                //Get Cached-Media
                IEnumerable<CostUOMMasterDTO> ObjCache = CacheHelper<IEnumerable<CostUOMMasterDTO>>.GetCacheItem("Cached_CostUOMMaster_" + CompanyID.ToString());
                if (ObjCache != null)
                {
                    List<CostUOMMasterDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => IDs.Split(',').Contains(i.ID.ToString()));
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<CostUOMMasterDTO>>.AppendToCacheItem("Cached_CostUOMMaster_" + CompanyID.ToString(), ObjCache);
                }

                return true;
            }
        }
        public IEnumerable<CostUOMMasterDTO> GetCachedData(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, bool IsForBOM = false)
        {
            //Get Cached-Media
            IEnumerable<CostUOMMasterDTO> ObjCache;
            if (IsArchived == false && IsDeleted == false)
            {
                //Get Cached-Media
                ObjCache = CacheHelper<IEnumerable<CostUOMMasterDTO>>.GetCacheItem("Cached_CostUOMMaster_" + CompanyID.ToString());

                if (ObjCache == null)
                {
                    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                    {
                        IEnumerable<CostUOMMasterDTO> obj = (from u in context.Database.SqlQuery<CostUOMMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,'' As CreatedDate,'' As UpdatedDate FROM CostUOMMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.CompanyID = " + CompanyID.ToString())
                                                             select new CostUOMMasterDTO
                                                             {
                                                                 ID = u.ID,
                                                                 CostUOM = u.CostUOM,
                                                                 CostUOMValue = u.CostUOMValue,
                                                                 UDF1 = u.UDF1,
                                                                 UDF2 = u.UDF2,
                                                                 UDF3 = u.UDF3,
                                                                 UDF4 = u.UDF4,
                                                                 UDF5 = u.UDF5,
                                                                 GUID = u.GUID,
                                                                 Created = u.Created,
                                                                 Updated = u.Updated,
                                                                 CreatedBy = u.CreatedBy,
                                                                 LastUpdatedBy = u.LastUpdatedBy,
                                                                 IsDeleted = u.IsDeleted,
                                                                 IsArchived = u.IsArchived,
                                                                 CompanyID = u.CompanyID,
                                                                 Room = u.Room,
                                                                 CreatedByName = u.CreatedByName,
                                                                 UpdatedByName = u.UpdatedByName,
                                                                 RoomName = u.RoomName,
                                                                 CreatedDate = u.CreatedDate,
                                                                 UpdatedDate = u.UpdatedDate,
                                                                 ReceivedOn = u.ReceivedOn,
                                                                 ReceivedOnWeb = u.ReceivedOnWeb,
                                                                 AddedFrom = u.AddedFrom,
                                                                 EditedFrom = u.EditedFrom,
                                                                 isForBOM = u.isForBOM,
                                                             }).AsParallel().ToList();
                        ObjCache = CacheHelper<IEnumerable<CostUOMMasterDTO>>.AddCacheItem("Cached_CostUOMMaster_" + CompanyID.ToString(), obj);
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
                    ObjCache = (from u in context.Database.SqlQuery<CostUOMMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,'' AS CreatedDate,'' AS UpdatedDate FROM CostUOMMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.CompanyID = " + CompanyID.ToString() + " AND " + sSQL)
                                select new CostUOMMasterDTO
                                {
                                    ID = u.ID,
                                    CostUOM = u.CostUOM,
                                    CostUOMValue = u.CostUOMValue,
                                    UDF1 = u.UDF1,
                                    UDF2 = u.UDF2,
                                    UDF3 = u.UDF3,
                                    UDF4 = u.UDF4,
                                    UDF5 = u.UDF5,
                                    GUID = u.GUID,
                                    Created = u.Created,
                                    Updated = u.Updated,
                                    CreatedBy = u.CreatedBy,
                                    LastUpdatedBy = u.LastUpdatedBy,
                                    IsDeleted = u.IsDeleted,
                                    IsArchived = u.IsArchived,
                                    CompanyID = u.CompanyID,
                                    Room = u.Room,
                                    CreatedDate = u.CreatedDate,
                                    UpdatedDate = u.UpdatedDate,
                                    ReceivedOn = u.ReceivedOn,
                                    ReceivedOnWeb = u.ReceivedOnWeb,
                                    AddedFrom = u.AddedFrom,
                                    EditedFrom = u.EditedFrom,
                                    isForBOM = u.isForBOM,
                                }).AsParallel().ToList();
                }
            }
            if (IsForBOM == true)
            {
                return ObjCache.Where(t => t.Room == 0);
            }
            else
            {
                return ObjCache.Where(t => t.Room == RoomID && t.isForBOM == false);
            }
        }
        public IEnumerable<CostUOMMasterDTO> GetAllBOMRecords(Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        {
            return GetCachedData(RoomID, CompanyId, IsArchived, IsDeleted, true).OrderBy("ID DESC");
        }
        public IEnumerable<CostUOMMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        {
            return GetCachedData(RoomID, CompanyId, IsArchived, IsDeleted).OrderBy("ID DESC");
        }
        public IEnumerable<CostUOMMasterDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, bool IsForBom)
        {
            //Get Cached-Media
            IEnumerable<CostUOMMasterDTO> ObjCache = GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted, IsForBom);
            if (IsForBom)
            {
                ObjCache = ObjCache.Where(t => t.isForBOM == true);
            }
            else
            {
                ObjCache = ObjCache.Where(t => t.isForBOM == false);
            }
            IEnumerable<CostUOMMasterDTO> ObjGlobalCache = ObjCache;
            //ObjCache = ObjCache.Where(t => (t.IsArchived == false && t.IsDeleted == false));

            //if (IsArchived && IsDeleted)
            //{
            //    ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsArchived == IsArchived || t.IsDeleted == IsDeleted));
            //    ObjCache = ObjCache.Concat(ObjGlobalCache);
            //}
            //else if (IsArchived)
            //{
            //    ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsArchived == IsArchived));
            //    ObjCache = ObjCache.Concat(ObjGlobalCache);
            //}
            //else if (IsDeleted)
            //{
            //    ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsDeleted == IsDeleted));
            //    ObjCache = ObjCache.Concat(ObjGlobalCache);
            //}

            if (String.IsNullOrEmpty(SearchTerm))
            {

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media
                //IEnumerable<CostUOMMasterDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
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
                    (t.CostUOM ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||

                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0))
                    );

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                //Get Cached-Media
                //IEnumerable<CostUOMMasterDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                TotalCount = ObjCache.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm) ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).Count();
                return ObjCache.Where
                    (t => t.ID.ToString().Contains(SearchTerm) ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }
        }
        public CostUOMMasterDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, bool? isForBom = false)
        {
            return GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted, isForBom.GetValueOrDefault(false)).Where(t => t.ID == id).SingleOrDefault();
        }
        public CostUOMMasterDTO GetRecord(string CostUOM, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            return GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted).Where(t => t.CostUOM.ToLower() == CostUOM.ToLower()).FirstOrDefault();
        }
        public string UpdateData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "UPDATE CostUOMMaster SET " + columnName + " = '" + value + "', ReceivedOn=GETUTCDATE(),EditedFrom='Web', Updated = DATETIME ('" + System.DateTime.Now.ToString(Convert.ToString((new System.Configuration.AppSettingsReader()).GetValue("FormatDateTimetoStore", typeof(System.String)))) + "') WHERE ID=" + id;
                context.Database.ExecuteSqlCommand(strQuery);
            }
            return value;
        }

    }
}
