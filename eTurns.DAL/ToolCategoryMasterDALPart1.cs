using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eTurns.DAL
{
    public partial class ToolCategoryMasterDAL : eTurnsBaseDAL
    {

        public IEnumerable<ToolCategoryMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId, bool IsDeleted, bool IsArchived, string DBConnectionstring)
        {
            return GetCachedData(RoomID, CompanyId, IsArchived, IsDeleted, DBConnectionstring).Where(x => x.IsArchived == IsArchived && x.IsDeleted == IsDeleted).OrderBy("ID DESC");
        }

        public ToolCategoryMasterDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID)
        {
            return GetCachedData(RoomID, CompanyID, false, false).SingleOrDefault(t => t.ID == id);
        }

        public ToolCategoryMasterDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID, bool IsDeleted, bool IsArchived)
        {
            return GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted).Where(x => x.IsDeleted == IsDeleted && x.IsArchived == IsArchived && x.ID == id).SingleOrDefault();
        }


        public IEnumerable<ToolCategoryMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId, bool IsDeleted, bool IsArchived)
        {
            return GetCachedData(RoomID, CompanyId, IsArchived, IsDeleted).Where(x => x.IsArchived == IsArchived && x.IsDeleted == IsDeleted).OrderBy("ID DESC");
        }

        public IEnumerable<ToolCategoryMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId)
        {
            return GetCachedData(RoomID, CompanyId, false, false).OrderBy("ID DESC");
        }


        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ToolCategoryMasterDTO> GetCachedData(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            //Get Cached-Media
            IEnumerable<ToolCategoryMasterDTO> ObjCache;
            if (IsArchived == false && IsDeleted == false)
            {
                ObjCache = CacheHelper<IEnumerable<ToolCategoryMasterDTO>>.GetCacheItem("Cached_ToolCategoryMaster_" + CompanyID.ToString());
                if (true)
                {
                    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                    {
                        IEnumerable<ToolCategoryMasterDTO> obj = (from u in context.ExecuteStoreQuery<ToolCategoryMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM ToolCategoryMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.CompanyID = " + CompanyID.ToString())
                                                                  select new ToolCategoryMasterDTO
                                                                  {
                                                                      ID = u.ID,
                                                                      ToolCategory = u.ToolCategory,
                                                                      Created = u.Created,
                                                                      Updated = u.Updated,
                                                                      CreatedByName = u.CreatedByName,
                                                                      UpdatedByName = u.UpdatedByName,
                                                                      RoomName = u.RoomName,
                                                                      LastUpdatedBy = u.LastUpdatedBy,
                                                                      CreatedBy = u.CreatedBy,
                                                                      Room = u.Room,
                                                                      GUID = u.GUID,
                                                                      CompanyID = u.CompanyID,
                                                                      IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                                                      IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                                                      UDF1 = u.UDF1,
                                                                      UDF2 = u.UDF2,
                                                                      UDF3 = u.UDF3,
                                                                      UDF4 = u.UDF4,
                                                                      UDF5 = u.UDF5,
                                                                      AddedFrom = u.AddedFrom,
                                                                      EditedFrom = u.EditedFrom,
                                                                      ReceivedOn = u.ReceivedOn,
                                                                      ReceivedOnWeb = u.ReceivedOnWeb
                                                                  }).AsParallel().ToList();
                        ObjCache = CacheHelper<IEnumerable<ToolCategoryMasterDTO>>.AddCacheItem("Cached_ToolCategoryMaster_" + CompanyID.ToString(), obj);
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
                    ObjCache = (from u in context.ExecuteStoreQuery<ToolCategoryMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM ToolCategoryMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.CompanyID = " + CompanyID.ToString() + @" AND A.Room = " + RoomID.ToString() + " AND " + sSQL)
                                select new ToolCategoryMasterDTO
                                {
                                    ID = u.ID,
                                    ToolCategory = u.ToolCategory,
                                    Created = u.Created,
                                    Updated = u.Updated,
                                    CreatedByName = u.CreatedByName,
                                    UpdatedByName = u.UpdatedByName,
                                    RoomName = u.RoomName,
                                    LastUpdatedBy = u.LastUpdatedBy,
                                    CreatedBy = u.CreatedBy,
                                    Room = u.Room,
                                    GUID = u.GUID,
                                    CompanyID = u.CompanyID,
                                    IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                    IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                    UDF1 = u.UDF1,
                                    UDF2 = u.UDF2,
                                    UDF3 = u.UDF3,
                                    UDF4 = u.UDF4,
                                    UDF5 = u.UDF5,
                                    AddedFrom = u.AddedFrom,
                                    EditedFrom = u.EditedFrom,
                                    ReceivedOn = u.ReceivedOn,
                                    ReceivedOnWeb = u.ReceivedOnWeb
                                }).AsParallel().ToList();
                }
            }
            return ObjCache.Where(t => t.Room == RoomID);
        }
        public IEnumerable<ToolCategoryMasterDTO> GetCachedData(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string DBConnectionstring)
        {
            //Get Cached-Media
            IEnumerable<ToolCategoryMasterDTO> ObjCache = null;
            if (IsArchived == false && IsDeleted == false)
            {

                if (ObjCache == null)
                {
                    using (var context = new eTurnsEntities(DBConnectionstring))
                    {
                        IEnumerable<ToolCategoryMasterDTO> obj = (from u in context.ExecuteStoreQuery<ToolCategoryMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM ToolCategoryMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.CompanyID = " + CompanyID.ToString())
                                                                  select new ToolCategoryMasterDTO
                                                                  {
                                                                      ID = u.ID,
                                                                      ToolCategory = u.ToolCategory,
                                                                      Created = u.Created,
                                                                      Updated = u.Updated,
                                                                      CreatedByName = u.CreatedByName,
                                                                      UpdatedByName = u.UpdatedByName,
                                                                      RoomName = u.RoomName,
                                                                      LastUpdatedBy = u.LastUpdatedBy,
                                                                      CreatedBy = u.CreatedBy,
                                                                      Room = u.Room,
                                                                      GUID = u.GUID,
                                                                      CompanyID = u.CompanyID,
                                                                      IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                                                      IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                                                      UDF1 = u.UDF1,
                                                                      UDF2 = u.UDF2,
                                                                      UDF3 = u.UDF3,
                                                                      UDF4 = u.UDF4,
                                                                      UDF5 = u.UDF5,
                                                                      AddedFrom = u.AddedFrom,
                                                                      EditedFrom = u.EditedFrom,
                                                                      ReceivedOn = u.ReceivedOn,
                                                                      ReceivedOnWeb = u.ReceivedOnWeb
                                                                  }).AsParallel().ToList();
                        ObjCache = obj;

                    }
                }
            }


            return ObjCache.Where(t => t.Room == RoomID);
        }


        public bool Delete(Int64 id, Int64 userID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                //return false;
                ToolCategoryMaster obj = context.ToolCategoryMasters.Single(t => t.ID == id);
                obj.IsDeleted = true;
                obj.IsArchived = false;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.LastUpdatedBy = userID;
                obj.EditedFrom = "Web";
                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                context.ToolCategoryMasters.Attach(obj);
                context.ObjectStateManager.ChangeObjectState(obj, EntityState.Modified);
                context.SaveChanges();

                //Get Cached-Media
                IEnumerable<ToolCategoryMasterDTO> ObjCache = CacheHelper<IEnumerable<ToolCategoryMasterDTO>>.GetCacheItem("Cached_ToolCategoryMaster_" + obj.CompanyID.ToString());
                if (ObjCache != null)
                {
                    List<ToolCategoryMasterDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => i.ID == id);
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<ToolCategoryMasterDTO>>.AppendToCacheItem("Cached_ToolCategoryMaster_" + obj.CompanyID.ToString(), ObjCache);
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
                        strQuery += "UPDATE ToolCategoryMaster SET Updated = '" + DateTime.Now.ToString() + "' , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1,ReceivedOn='" + DateTimeUtility.DateTimeNow + "',EditedFrom='Web' WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.ExecuteStoreCommand(strQuery);

                //Get Cached-Media
                IEnumerable<ToolCategoryMasterDTO> ObjCache = CacheHelper<IEnumerable<ToolCategoryMasterDTO>>.GetCacheItem("Cached_ToolCategoryMaster_" + CompanyId.ToString());
                if (ObjCache != null)
                {
                    List<ToolCategoryMasterDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => IDs.Split(',').Contains(i.ID.ToString()));
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<ToolCategoryMasterDTO>>.AppendToCacheItem("Cached_ToolCategoryMaster_" + CompanyId.ToString(), ObjCache);
                }


                return true;
            }
        }

        public string UpdateData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                string strQuery = "UPDATE ToolCategoryMaster SET " + columnName + " = '" + value + "', Updated = DATETIME ('" + System.DateTime.Now.ToString(Convert.ToString((new System.Configuration.AppSettingsReader()).GetValue("FormatDateTimetoStore", typeof(System.String)))) + "'),ReceivedOn='" + DateTimeUtility.DateTimeNow + "',EditedFrom='Web' WHERE ID= '" + id + "'";
                context.ExecuteStoreCommand(strQuery);
                context.SaveChanges();
            }

            return value;
        }

        /// <summary>
        /// GetPagedRecords
        /// </summary>
        /// <param name="StartRowIndex"></param>
        /// <param name="MaxRows"></param>
        /// <param name="TotalCount"></param>
        /// <param name="SearchTerm"></param>
        /// <param name="sortColumnName"></param>
        /// <returns></returns>
        public IEnumerable<ToolCategoryMasterDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        {

            IEnumerable<ToolCategoryMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId, IsArchived, IsDeleted);
            IEnumerable<ToolCategoryMasterDTO> ObjGlobalCache = ObjCache;
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
                //IEnumerable<ToolCategoryMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId);
                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media
                //IEnumerable<ToolCategoryMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId);

                string[] dd = SearchTerm.Replace("STARTWITH#", "$").Split('$');
                string[] stringSeparators = new string[] { "[###]" };

                if (dd != null && dd.Length > 0)
                {
                    string[] Fields = dd[0].Split(stringSeparators, StringSplitOptions.None);
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
                                    && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').Select(HttpUtility.UrlDecode).ToList().Contains(t.UDF1)))
                                    && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').Select(HttpUtility.UrlDecode).ToList().Contains(t.UDF2)))
                                    && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').Select(HttpUtility.UrlDecode).ToList().Contains(t.UDF3)))
                                    && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').Select(HttpUtility.UrlDecode).ToList().Contains(t.UDF4)))
                                    && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').Select(HttpUtility.UrlDecode).ToList().Contains(t.UDF5)))
                                     && (SearchTerm == "" || (t.ID.ToString().Contains(SearchTerm) ||
                        t.ToolCategory.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0))
                                     );

                    // 6 counts for fields based on that prepare the search string
                    // 1 = createdBy, 2 = updatedby , 3 = CreatedFrom, 4 = CreatedTo, 5=UpdatedFrom,6=UpdatedTo
                    //foreach (var item in Fields)
                    //{
                    //    if (item.Length > 0)
                    //    {
                    //        if (item.Contains("CreatedBy"))
                    //        {
                    //            ObjCache = ObjCache.Where(t => item.Split('#')[1].Split(',').ToList().Contains(t.CreatedByName.ToString()));
                    //        }
                    //        else if (item.Contains("UpdatedBy"))
                    //        {
                    //            ObjCache = ObjCache.Where(t => item.Split('#')[1].Split(',').ToList().Contains(t.UpdatedByName.ToString()));
                    //        }
                    //        else if (item.Contains("DateCreatedFrom"))
                    //        {
                    //            ObjCache = ObjCache.Where(t => t.Created.Value.Date >= Convert.ToDateTime(item.Split('#')[1]).Date && t.Created.Value.Date <= Convert.ToDateTime(item.Split('#')[3]).Date);
                    //        }
                    //        else if (item.Contains("DateUpdatedFrom"))
                    //        {
                    //            ObjCache = ObjCache.Where(t => t.Updated.Value.Date >= Convert.ToDateTime(item.Split('#')[1]).Date && t.Updated.Value.Date <= Convert.ToDateTime(item.Split('#')[3]).Date);
                    //        }
                    //        else if (item.Contains("UDF1"))
                    //        {
                    //            ObjCache = ObjCache.Where(t => item.Split('#')[1].Split(',').ToList().Contains(t.UDF1));
                    //        }
                    //        else if (item.Contains("UDF2"))
                    //        {
                    //            ObjCache = ObjCache.Where(t => item.Split('#')[1].Split(',').ToList().Contains(t.UDF2));
                    //        }
                    //        else if (item.Contains("UDF3"))
                    //        {
                    //            ObjCache = ObjCache.Where(t => item.Split('#')[1].Split(',').ToList().Contains(t.UDF3));
                    //        }
                    //        else if (item.Contains("UDF4"))
                    //        {
                    //            ObjCache = ObjCache.Where(t => item.Split('#')[1].Split(',').ToList().Contains(t.UDF4));
                    //        }
                    //        else if (item.Contains("UDF5"))
                    //        {
                    //            ObjCache = ObjCache.Where(t => item.Split('#')[1].Split(',').ToList().Contains(t.UDF5));
                    //        }
                    //    }
                    //}
                }
                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                //Get Cached-Media
                //IEnumerable<ToolCategoryMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId);
                TotalCount = ObjCache.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm) ||
                        t.ToolCategory.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).Count();
                return ObjCache.Where
                    (t => t.ID.ToString().Contains(SearchTerm) ||
                        t.ToolCategory.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }

        }



    }
}
