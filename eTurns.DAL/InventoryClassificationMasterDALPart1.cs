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
using System.Data.SqlClient;

namespace eTurns.DAL
{
    public partial class InventoryClassificationMasterDAL : eTurnsBaseDAL
    {
        /// <summary>
        /// Get Particullar Record from the InventoryClassificationMaster by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public InventoryClassificationMasterDTO GetHistoryRecord(Int64 id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<InventoryClassificationMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM InventoryClassificationMaster_History A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.HistoryID=" + id.ToString())
                        select new InventoryClassificationMasterDTO
                        {
                            ID = u.ID,
                            InventoryClassification = u.InventoryClassification,
                            BaseOfInventory = u.BaseOfInventory,
                            RangeStart = u.RangeStart,
                            RangeEnd = u.RangeEnd,
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
                            isForBOM = u.isForBOM,
                            RefBomId = u.RefBomId,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                        }).SingleOrDefault();
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
                        strQuery += "UPDATE InventoryClassificationMaster SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1, ReceivedOn=GETUTCDATE(),EditedFrom='Web' WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.ExecuteStoreCommand(strQuery);


                //Get Cached-Media
                IEnumerable<InventoryClassificationMasterDTO> ObjCache = CacheHelper<IEnumerable<InventoryClassificationMasterDTO>>.GetCacheItem("Cached_InventoryClassificationMaster_" + CompanyID.ToString());
                if (ObjCache != null)
                {
                    List<InventoryClassificationMasterDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => IDs.Split(',').Contains(i.ID.ToString()));
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<InventoryClassificationMasterDTO>>.AppendToCacheItem("Cached_InventoryClassificationMaster_" + CompanyID.ToString(), ObjCache);
                }

                return true;
            }
        }


        /// <summary>
        /// Get Paged Records from the InventoryClassificationMaster Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<InventoryClassificationMasterDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, bool IsForBom)
        {
            //Get Cached-Media
            IEnumerable<InventoryClassificationMasterDTO> ObjCache = GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted, IsForBom);
            IEnumerable<InventoryClassificationMasterDTO> ObjGlobalCache = ObjCache;
            // ObjCache = ObjCache.Where(t => (t.IsArchived == false && t.IsDeleted == false));

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
            if (IsForBom)
            {
                ObjCache = ObjCache.Where(t => t.isForBOM == true);
            }
            else
            {
                ObjCache = ObjCache.Where(t => t.isForBOM == false);
            }
            if (String.IsNullOrEmpty(SearchTerm))
            {

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media
                //IEnumerable<InventoryClassificationMasterDTO> ObjCache = GetCachedData(RoomID, CompanyID);
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
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains((t.CreatedBy ?? 0).ToString())))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains((t.LastUpdatedBy ?? 0).ToString())))
                    && ((Fields[1].Split('@')[2] == "") || (t.Created.Value >= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[0], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))) && t.Created.Value <= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[1], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"])))))
                    && ((Fields[1].Split('@')[3] == "") || (t.Updated.Value >= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[0], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))) && t.Updated.Value <= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[1], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"])))))
                    && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').ToList().Contains(t.UDF1)))
                    && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').ToList().Contains(t.UDF2)))
                    && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').ToList().Contains(t.UDF3)))
                    && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').ToList().Contains(t.UDF4)))
                    && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').ToList().Contains(t.UDF5)))
                    && (SearchTerm == "" || (t.ID.ToString().Contains(SearchTerm) ||
                                                (t.InventoryClassification ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.BaseOfInventory ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
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
                //IEnumerable<InventoryClassificationMasterDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                TotalCount = ObjCache.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm) ||
                        (t.InventoryClassification ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.BaseOfInventory ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).Count();
                return ObjCache.Where
                    (t => t.ID.ToString().Contains(SearchTerm) ||
                                                (t.InventoryClassification ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.BaseOfInventory ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }
        }




        /// <summary>
        /// Get Paged Records from the InventoryClassificationMaster Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<InventoryClassificationMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, bool IsForBom)
        {
            return GetCachedData(RoomID, CompanyId, IsArchived, IsDeleted, IsForBom).OrderBy("ID DESC");
        }


        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public IEnumerable<InventoryClassificationMasterDTO> GetCachedData(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, bool? IsForBom)
        {
            //Get Cached-Media
            IEnumerable<InventoryClassificationMasterDTO> ObjCache;
            if (IsArchived == false && IsDeleted == false)
            {
                ObjCache = CacheHelper<IEnumerable<InventoryClassificationMasterDTO>>.GetCacheItem("Cached_InventoryClassificationMaster_" + CompanyID.ToString());
                if (ObjCache == null)
                {
                    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                    {
                        IEnumerable<InventoryClassificationMasterDTO> obj = (from u in context.ExecuteStoreQuery<InventoryClassificationMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, E.CycleCountFrequency FROM InventoryClassificationMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID LEFT OUTER JOIN CycleCountSetUp E on A.[GUID] = E.ClassificationGUID WHERE  A.IsDeleted!=1 AND A.IsArchived != 1 AND A.CompanyID = " + CompanyID.ToString())
                                                                             select new InventoryClassificationMasterDTO
                                                                             {
                                                                                 ID = u.ID,
                                                                                 InventoryClassification = u.InventoryClassification,
                                                                                 BaseOfInventory = u.BaseOfInventory,
                                                                                 RangeStart = u.RangeStart,
                                                                                 RangeEnd = u.RangeEnd,
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
                                                                                 isForBOM = u.isForBOM,
                                                                                 RefBomId = u.RefBomId,
                                                                                 ReceivedOn = u.ReceivedOn,
                                                                                 ReceivedOnWeb = u.ReceivedOnWeb,
                                                                                 AddedFrom = u.AddedFrom,
                                                                                 EditedFrom = u.EditedFrom,
                                                                                 CycleCountFrequency = u.CycleCountFrequency
                                                                             }).AsParallel().ToList();
                        ObjCache = CacheHelper<IEnumerable<InventoryClassificationMasterDTO>>.AddCacheItem("Cached_InventoryClassificationMaster_" + CompanyID.ToString(), obj);
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
                    ObjCache = (from u in context.ExecuteStoreQuery<InventoryClassificationMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM InventoryClassificationMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.CompanyID = " + CompanyID.ToString() + " AND " + sSQL)
                                select new InventoryClassificationMasterDTO
                                {
                                    ID = u.ID,
                                    InventoryClassification = u.InventoryClassification,
                                    BaseOfInventory = u.BaseOfInventory,
                                    RangeStart = u.RangeStart,
                                    RangeEnd = u.RangeEnd,
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
                                    isForBOM = u.isForBOM,
                                    RefBomId = u.RefBomId,
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
                return ObjCache.Where(t => t.Room == RoomID);
            }
        }




        public List<InventoryClassificationMasterDTO> GetClassificationByRangeOrder(long RoomId, long CompanyId)
        {
            List<InventoryClassificationMasterDTO> lstClassification = new List<InventoryClassificationMasterDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstClassification = (from u in context.InventoryClassificationMasters
                                     where u.Room == RoomId && u.CompanyID == CompanyId && u.isForBOM == false && (u.IsDeleted ?? false) == false && (u.IsArchived ?? false) == false //&& ((u.RangeStart ?? 0) + (u.RangeStart ?? 0)) > 0 
                                     select new InventoryClassificationMasterDTO
                                     {
                                         ID = u.ID,
                                         InventoryClassification = u.InventoryClassification,
                                         BaseOfInventory = u.BaseOfInventory,
                                         RangeStart = u.RangeStart ?? 0,
                                         RangeEnd = u.RangeEnd ?? 0,
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
                                         isForBOM = u.isForBOM,
                                         RefBomId = u.RefBomId,
                                         ReceivedOn = u.ReceivedOn,
                                         ReceivedOnWeb = u.ReceivedOnWeb,
                                         AddedFrom = u.AddedFrom,
                                         EditedFrom = u.EditedFrom,
                                     }).OrderBy(t => (t.RangeStart ?? 0)).ThenBy(t => ((t.RangeStart ?? 0) + (t.RangeEnd ?? 0))).ThenBy(t => t.ID).ToList();
            }
            return lstClassification;

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
                string strQuery = "UPDATE InventoryClassificationMaster SET " + columnName + " = '" + value + "', ReceivedOn=GETUTCDATE(),EditedFrom='Web', Updated = DATETIME ('" + System.DateTime.Now.ToString(Convert.ToString((new System.Configuration.AppSettingsReader()).GetValue("FormatDateTimetoStore", typeof(System.String)))) + "') WHERE ID=" + id;
                context.ExecuteStoreCommand(strQuery);
            }
            return value;
        }

        public InventoryClassificationMasterDTO GetRecord(string InventoryClassification, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, bool? IsForBom)
        {
            return GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted, IsForBom).Single(t => t.InventoryClassification == InventoryClassification);
        }


        /// <summary>
        /// Get Particullar Record from the InventoryClassificationMaster by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public InventoryClassificationMasterDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, bool? IsForBom)
        {
            return GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted, IsForBom).Single(t => t.ID == id);
        }




        /// <summary>
        /// Get Particullar Record from the InventoryClassificationMaster by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public List<InventoryClassificationMasterDTO> GetRangeRecord(double range, Int64 RoomId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<InventoryClassificationMasterDTO>(@"Select * from InventoryClassificationMaster Where Room = " + RoomId + " and " + range + " between Rangestart and RangeEnd and Isdeleted = 0 and IsArchived = 0")
                        select new InventoryClassificationMasterDTO
                        {
                            ID = u.ID,
                            InventoryClassification = u.InventoryClassification,
                            BaseOfInventory = u.BaseOfInventory,
                            RangeStart = u.RangeStart,
                            RangeEnd = u.RangeEnd,
                        }).ToList();
            }
        }
    }
}
