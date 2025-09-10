using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;

namespace eTurns.DAL
{
    public partial class ModuleMasterDAL : eTurnsBaseDAL
    {

        public IEnumerable<ModuleMasterDTO> GetCachedData()
        {
            //Get Cached-Media
            IEnumerable<ModuleMasterDTO> ObjCache = CacheHelper<IEnumerable<ModuleMasterDTO>>.GetCacheItem("Cached_ModuleMaster_");
            if (ObjCache == null)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<ModuleMasterDTO> obj = (from u in context.ExecuteStoreQuery<ModuleMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM ModuleMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 and A.IsModule=1")
                                                        select new ModuleMasterDTO
                                                        {
                                                            ID = u.ID,
                                                            ModuleName = u.ModuleName,
                                                            Created = u.Created,
                                                            Updated = u.Updated,
                                                            CreatedBy = u.CreatedBy,
                                                            LastUpdatedBy = u.LastUpdatedBy,
                                                            CreatedByName = u.CreatedByName,
                                                            UpdatedByName = u.UpdatedByName,
                                                            RoomName = u.RoomName,
                                                            GUID = u.GUID,
                                                            resourcekey = u.resourcekey
                                                        }).AsParallel().ToList();
                    ObjCache = CacheHelper<IEnumerable<ModuleMasterDTO>>.AddCacheItem("Cached_ModuleMaster_", obj);
                }
            }

            return ObjCache;
        }

        public IEnumerable<ModuleMasterDTO> GetPagedRecords(Int32 ModuleID, Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName)
        {

            if (String.IsNullOrEmpty(SearchTerm))
            {
                //Get Cached-Media
                IEnumerable<ModuleMasterDTO> ObjCache = GetCachedData();
                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media
                IEnumerable<ModuleMasterDTO> ObjCache = GetCachedData();

                string[] dd = SearchTerm.Replace("STARTWITH#", "$").Split('$');
                string[] stringSeparators = new string[] { "[###]" };

                if (dd != null && dd.Length > 0)
                {
                    string[] Fields = dd[1].Split(stringSeparators, StringSplitOptions.None);
                    // 6 counts for fields based on that prepare the search string
                    // 1 = createdBy, 2 = updatedby , 3 = CreatedFrom, 4 = CreatedTo, 5=UpdatedFrom,6=UpdatedTo
                    foreach (var item in Fields)
                    {
                        if (item.Length > 0)
                        {
                            if (item.Contains("CreatedBy"))
                            {
                                ObjCache = ObjCache.Where(t => item.Split('#')[1].Split(',').ToList().Contains(t.CreatedByName.ToString()));
                            }
                            else if (item.Contains("UpdatedBy"))
                            {
                                ObjCache = ObjCache.Where(t => item.Split('#')[1].Split(',').ToList().Contains(t.UpdatedByName.ToString()));
                            }
                            else if (item.Contains("DateCreatedFrom"))
                            {
                                ObjCache = ObjCache.Where(t => t.Created.Value.Date >= Convert.ToDateTime(item.Split('#')[1]).Date && t.Created.Value.Date <= Convert.ToDateTime(item.Split('#')[3]).Date);
                            }
                            else if (item.Contains("DateUpdatedFrom"))
                            {
                                ObjCache = ObjCache.Where(t => t.Updated.Value.Date >= Convert.ToDateTime(item.Split('#')[1]).Date && t.Updated.Value.Date <= Convert.ToDateTime(item.Split('#')[3]).Date);
                            }
                        }
                    }
                }
                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                //Get Cached-Media
                IEnumerable<ModuleMasterDTO> ObjCache = GetCachedData();
                TotalCount = ObjCache.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm) ||
                        t.ModuleName.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).Count();
                return ObjCache.Where
                    (t => t.ID.ToString().Contains(SearchTerm) ||
                        t.ModuleName.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
        }
        public IEnumerable<ModuleMasterDTO> GetAllRecords(Int64 ModuleID)
        {
            return GetCachedData().OrderBy("ID DESC");
        }
        public IEnumerable<ModuleMasterDTO> GetAllModuleRecords(Int32 ModuleID)
        {
            return GetCachedData().OrderBy("ID DESC");
        }
        public ModuleMasterDTO GetModuleNameByName(string ModuleName, Int64 RoomID)
        {
            return GetCachedData().ToList().Where(t => t.ModuleName.Trim().ToLower() == ModuleName.Trim().ToLower()).FirstOrDefault();
        }

        public IEnumerable<ParentModuleMasterDTO> GetParentCachedData()
        {
            //Get Cached-Media
            IEnumerable<ParentModuleMasterDTO> ObjCache = CacheHelper<IEnumerable<ParentModuleMasterDTO>>.GetCacheItem("Cached_ParentModuleMaster_");
            if (ObjCache == null)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<ParentModuleMasterDTO> obj = (from u in context.ExecuteStoreQuery<ParentModuleMasterDTO>(@"SELECT A.* FROM ParentModuleMaster A")
                                                              select new ParentModuleMasterDTO
                                                              {
                                                                  ID = u.ID,
                                                                  ModuleName = u.ModuleName,
                                                                  ParentModuleName = u.ParentModuleName,
                                                                  GUID = u.GUID
                                                              }).AsParallel().ToList();
                    ObjCache = CacheHelper<IEnumerable<ParentModuleMasterDTO>>.AddCacheItem("Cached_ParentModuleMaster_", obj);
                }
            }
            return ObjCache;
        }

        public IEnumerable<ParentModuleMasterDTO> GetParentRecord()
        {
            return GetParentCachedData().OrderBy("ID ASC");
        }
        public ModuleMasterDTO GetRecord(Int64 id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<ModuleMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, A.GUID FROM ModuleMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1  AND A.ID=" + id.ToString())
                        select new ModuleMasterDTO
                        {
                            ID = u.ID,
                            ModuleName = u.ModuleName,
                            DisplayName = u.DisplayName,
                            ParentID = u.ParentID,
                            Value = u.Value,
                            Priority = u.Priority,
                            IsModule = u.IsModule,
                            GroupId = u.GroupId,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            RoomName = u.RoomName,

                            LastUpdatedBy = u.LastUpdatedBy,
                            CreatedBy = u.CreatedBy,
                            Room = u.Room,
                            GUID = u.GUID,
                            resourcekey = u.resourcekey
                        }).SingleOrDefault();
            }
        }

        public bool Delete(Int64 id, Int64 userid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ModuleMaster obj = context.ModuleMasters.Single(t => t.ID == id);
                obj.IsDeleted = true;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.LastUpdatedBy = userid;
                context.ModuleMasters.Attach(obj);
                context.ObjectStateManager.ChangeObjectState(obj, EntityState.Modified);
                context.SaveChanges();

                return true;
            }
        }

        public bool DeleteRecords(string IDs, int userid)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE ModuleMaster SET Updated = '" + DateTime.Now.ToString() + "' , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.ExecuteStoreCommand(strQuery);

                //Get Cached-Media
                IEnumerable<ModuleMasterDTO> ObjCache = CacheHelper<IEnumerable<ModuleMasterDTO>>.GetCacheItem("Cached_ModuleMaster_");
                if (ObjCache != null)
                {
                    List<ModuleMasterDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => IDs.Split(',').Contains(i.ID.ToString()));
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<ModuleMasterDTO>>.AppendToCacheItem("Cached_ModuleMaster_", ObjCache);
                }


                return true;
            }
        }

        public string UpdateData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "UPDATE ModuleMaster SET " + columnName + " = '" + value + "', Updated = DATETIME ('" + System.DateTime.Now.ToString(Convert.ToString((new System.Configuration.AppSettingsReader()).GetValue("FormatDateTimetoStore", typeof(System.String)))) + "') WHERE ID=" + id;
                context.ExecuteStoreCommand(strQuery);
            }
            return value;
        }
        public string GetModuleNotification(Int32 ModuleId, Int64 RoomId, Int64 UserId)
        {
            //Get Cached-Media
            IEnumerable<ModuleNotificationDTO> ObjCache = null;// CacheHelper<IEnumerable<ModuleNotificationDTO>>.GetCacheItem("Cached_ModuleNotification");
            if (ObjCache == null)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<ModuleNotificationDTO> obj = (from u in context.ExecuteStoreQuery<ModuleNotificationDTO>(@"SELECT A.* FROM ModuleNotification A")
                                                              select new ModuleNotificationDTO
                                                              {
                                                                  NotificationID = u.NotificationID,
                                                                  ModuleId = u.ModuleId,
                                                                  RoomID = u.RoomID,
                                                                  UserID = u.UserID,
                                                                  Notification = u.Notification
                                                              }).AsParallel().ToList();
                    ObjCache = CacheHelper<IEnumerable<ModuleNotificationDTO>>.AddCacheItem("Cached_ModuleNotification", obj);
                }
            }

            return ObjCache.Where(c => c.ModuleId == ModuleId && c.RoomID == RoomId).Select(c => Convert.ToString(c.Notification)).SingleOrDefault();

            //            return "";
        }
    }
}
