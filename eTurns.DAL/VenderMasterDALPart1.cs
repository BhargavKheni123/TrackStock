using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Web;
using eTurns.DTO.Resources;
using System.Data.SqlClient;

namespace eTurns.DAL
{
    public partial class VenderMasterDAL : eTurnsBaseDAL
    {
        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public IEnumerable<VenderMasterDTO> GetCachedData(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            //Get Cached-Media
            IEnumerable<VenderMasterDTO> ObjCache = null;
            if (IsArchived == false && IsDeleted == false)
            {
                //ObjCache = CacheHelper<IEnumerable<VenderMasterDTO>>.GetCacheItem("Cached_VenderMaster_" + CompanyID.ToString());
                if (ObjCache == null)
                {
                    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                    {
                        IEnumerable<VenderMasterDTO> obj = (from u in context.ExecuteStoreQuery<VenderMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName as RoomName FROM VenderMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.CompanyID = " + CompanyID.ToString() + @" AND A.Room = " + RoomID.ToString())
                                                            select new VenderMasterDTO
                                                            {
                                                                ID = u.ID,
                                                                Vender = u.Vender,
                                                                Created = u.Created,
                                                                Updated = u.Updated,
                                                                CreatedBy = u.CreatedBy,
                                                                LastUpdatedBy = u.LastUpdatedBy,
                                                                Room = u.Room,
                                                                IsDeleted = u.IsDeleted,
                                                                IsArchived = u.IsArchived,
                                                                CompanyID = u.CompanyID,
                                                                GUID = u.GUID,
                                                                UDF1 = u.UDF1,
                                                                UDF2 = u.UDF2,
                                                                UDF3 = u.UDF3,
                                                                UDF4 = u.UDF4,
                                                                UDF5 = u.UDF5,
                                                                CreatedByName = u.CreatedByName,
                                                                UpdatedByName = u.UpdatedByName,
                                                                RoomName = u.RoomName,
                                                            }).AsParallel().ToList();
                        ObjCache = CacheHelper<IEnumerable<VenderMasterDTO>>.AddCacheItem("Cached_VenderMaster_" + CompanyID.ToString(), obj);
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
                    ObjCache = (from u in context.ExecuteStoreQuery<VenderMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM VenderMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.CompanyID = " + CompanyID.ToString() + @" AND A.Room = " + RoomID.ToString() + " AND " + sSQL)
                                select new VenderMasterDTO
                                {
                                    ID = u.ID,
                                    Vender = u.Vender,
                                    Created = u.Created,
                                    Updated = u.Updated,
                                    CreatedBy = u.CreatedBy,
                                    LastUpdatedBy = u.LastUpdatedBy,
                                    Room = u.Room,
                                    IsDeleted = u.IsDeleted,
                                    IsArchived = u.IsArchived,
                                    CompanyID = u.CompanyID,
                                    GUID = u.GUID,
                                    UDF1 = u.UDF1,
                                    UDF2 = u.UDF2,
                                    UDF3 = u.UDF3,
                                    UDF4 = u.UDF4,
                                    UDF5 = u.UDF5,
                                    CreatedByName = u.CreatedByName,
                                    UpdatedByName = u.UpdatedByName,
                                    RoomName = u.RoomName,
                                }).AsParallel().ToList();

                }

            }
            return ObjCache.Where(t => t.Room == RoomID);
        }

        /// <summary>
        /// Get Paged Records from the VenderMaster Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<VenderMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId)
        {
            return GetCachedData(RoomID, CompanyId, false, false).OrderBy("ID DESC");
        }

        /// <summary>
        /// Get Paged Records from the VenderMaster Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>

        public IEnumerable<VenderMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId, bool IsDeleted, bool IsArchived)
        {
            return GetCachedData(RoomID, CompanyId, IsArchived, IsDeleted).Where(x => x.IsArchived == IsArchived && x.IsDeleted == IsDeleted).OrderBy("ID DESC");
        }

        /// <summary>
        /// Get Paged Records from the VenderMaster Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<VenderMasterDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            //Get Cached-Media
            IEnumerable<VenderMasterDTO> ObjCache = GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted);
            IEnumerable<VenderMasterDTO> ObjGlobalCache = ObjCache;
            ObjCache = ObjCache.Where(t => (t.IsArchived == false && t.IsDeleted == false));

            if (IsArchived && IsDeleted)
            {
                ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsArchived == IsArchived || t.IsDeleted == IsDeleted));
                ObjCache = ObjCache.Concat(ObjGlobalCache);
            }
            else if (IsArchived)
            {
                ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsArchived == IsArchived));
                ObjCache = ObjCache.Concat(ObjGlobalCache);
            }
            else if (IsDeleted)
            {
                ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsDeleted == IsDeleted));
                ObjCache = ObjCache.Concat(ObjGlobalCache);
            }


            if (String.IsNullOrEmpty(SearchTerm))
            {
                //Get Cached-Media
                //IEnumerable<AssetCategoryMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId);
                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media
                //IEnumerable<VenderMasterDTO> ObjCache = GetCachedData(RoomID, CompanyID);
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
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').Select(long.Parse).ToList().Contains(t.CreatedBy.GetValueOrDefault(0))))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').Select(long.Parse).ToList().Contains(t.LastUpdatedBy.GetValueOrDefault(0))))
                    && ((Fields[1].Split('@')[2] == "") || (t.Created >= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[0], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))) && t.Created <= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[1], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"])))))
                    && ((Fields[1].Split('@')[3] == "") || (t.Updated >= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[0], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))) && t.Updated <= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[1], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"])))))
                    && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').Select(HttpUtility.UrlDecode).ToList().Contains(t.UDF1)))
                    && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').Select(HttpUtility.UrlDecode).ToList().Contains(t.UDF2)))
                    && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').Select(HttpUtility.UrlDecode).ToList().Contains(t.UDF3)))
                    && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').Select(HttpUtility.UrlDecode).ToList().Contains(t.UDF4)))
                    && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').Select(HttpUtility.UrlDecode).ToList().Contains(t.UDF5)))
                    && (SearchTerm == "" || (t.ID.ToString().Contains(SearchTerm) ||
                    (t.Vender ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
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
                //IEnumerable<VenderMasterDTO> ObjCache = GetCachedData(RoomID, CompanyID);
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

        /// <summary>
        /// Get Particullar Record from the VenderMaster by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public VenderMasterDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID)
        {
            return GetCachedData(RoomID, CompanyID, false, false).Single(t => t.ID == id);
        }
        public VenderMasterDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID, bool IsDeleted, bool IsArchived)
        {
            return GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted).Where(x => x.IsDeleted == IsDeleted && x.IsArchived == IsArchived && x.ID == id).SingleOrDefault();
        }

        /// <summary>
        /// Get Particullar Record from the VenderMaster by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public VenderMasterDTO GetHistoryRecord(Int64 id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<VenderMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM VenderMaster_History A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.HistoryID=" + id.ToString())
                        select new VenderMasterDTO
                        {
                            ID = u.ID,
                            Vender = u.Vender,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            Room = u.Room,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            CompanyID = u.CompanyID,
                            GUID = u.GUID,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                        }).SingleOrDefault();
            }
        }


        public List<VenderMasterDTO> GetVendorByName(string Name, long RoomID, long CompanyID)
        {
            List<VenderMasterDTO> lstVendors = new List<VenderMasterDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstVendors = (from u in context.VenderMasters
                              where u.Vender == Name && u.Room == RoomID && u.CompanyID == CompanyID && u.IsDeleted == false
                              select new VenderMasterDTO
                              {
                                  ID = u.ID,
                                  Vender = u.Vender,
                                  Created = u.Created,
                                  Updated = u.Updated,
                                  CreatedBy = u.CreatedBy,
                                  LastUpdatedBy = u.LastUpdatedBy,
                                  Room = u.Room,
                                  IsDeleted = u.IsDeleted,
                                  IsArchived = u.IsArchived,
                                  CompanyID = u.CompanyID,
                                  GUID = u.GUID,
                                  UDF1 = u.UDF1,
                                  UDF2 = u.UDF2,
                                  UDF3 = u.UDF3,
                                  UDF4 = u.UDF4,
                                  UDF5 = u.UDF5

                              }).ToList();

            }
            return lstVendors;

        }

    }
}
