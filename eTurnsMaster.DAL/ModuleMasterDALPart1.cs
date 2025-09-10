using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eTurnsMaster.DAL
{
    public partial class ModuleMasterDAL : eTurnsMasterBaseDAL
    {
        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ModuleMasterDTO> GetCachedData()
        {
            //Get Cached-Media
            IEnumerable<ModuleMasterDTO> ObjCache = CacheHelper<IEnumerable<ModuleMasterDTO>>.GetCacheItem("Cached_ModuleMaster_Master_");
            if (ObjCache == null)
            {
                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<ModuleMasterDTO> obj = (from u in context.ExecuteStoreQuery<ModuleMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName FROM ModuleMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID WHERE A.IsDeleted!=1 and A.IsModule=1")
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
                                                            GUID = u.GUID
                                                        }).AsParallel().ToList();
                    ObjCache = CacheHelper<IEnumerable<ModuleMasterDTO>>.AddCacheItem("Cached_ModuleMaster_Master_", obj);
                }
            }

            return ObjCache;
        }
        public IEnumerable<ParentModuleMasterDTO> GetParentCachedData()
        {
            //Get Cached-Media
            IEnumerable<ParentModuleMasterDTO> ObjCache = CacheHelper<IEnumerable<ParentModuleMasterDTO>>.GetCacheItem("Cached_ParentModuleMaster_");
            if (ObjCache == null)
            {
                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<ParentModuleMasterDTO> obj = (from u in context.ExecuteStoreQuery<ParentModuleMasterDTO>(@"SELECT A.* FROM ParentModuleMaster A")
                                                              select new ParentModuleMasterDTO
                                                              {
                                                                  ID = u.ID,
                                                                  ModuleName = u.ModuleName,
                                                                  GUID = u.GUID
                                                              }).AsParallel().ToList();
                    ObjCache = CacheHelper<IEnumerable<ParentModuleMasterDTO>>.AddCacheItem("Cached_ParentModuleMaster_", obj);
                }
            }
            return ObjCache;
        }


        /// <summary>
        /// Get Paged Records from the Module Master Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<ModuleMasterDTO> GetAllRecords(Int32 ModuleID)
        {
            return GetCachedData().OrderBy("ID DESC");
        }

        public IEnumerable<ModuleMasterDTO> GetAllModuleRecords(Int32 ModuleID)
        {
            return GetCachedData().OrderBy("ID DESC");
        }


        /// <summary>
        /// Get Paged Records from the Module Master Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
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

        /// <summary>
        /// Get Particullar Record from the Module Master by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public ModuleMasterDTO GetRecord(Int64 id)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<ModuleMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, A.GUID FROM ModuleMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID WHERE A.IsDeleted!=1  AND A.ID=" + id.ToString())
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
                            LastUpdatedBy = u.LastUpdatedBy,
                            CreatedBy = u.CreatedBy,
                            GUID = u.GUID
                        }).SingleOrDefault();
            }
        }

        /// <summary>
        /// Insert Record in the DataBase Module Master
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 Insert(ModuleMasterDTO objDTO)
        {
            objDTO.Updated = DateTime.UtcNow;
            objDTO.Created = DateTime.UtcNow;

            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                ModuleMaster obj = new ModuleMaster();
                obj.ID = 0;
                obj.ModuleName = objDTO.ModuleName;
                obj.DisplayName = objDTO.DisplayName;
                obj.ParentID = objDTO.ParentID;
                obj.Value = objDTO.Value;
                obj.Priority = objDTO.Priority;
                obj.IsModule = objDTO.IsModule;
                obj.GroupID = objDTO.GroupId;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Updated = objDTO.Updated;
                obj.Created = objDTO.Created;
                obj.IsDeleted = false;
                obj.GUID = Guid.NewGuid();

                context.AddToModuleMasters(obj);
                context.SaveChanges();

                objDTO.ID = obj.ID;
                if (objDTO.ID > 0)
                {
                    //Get Cached-Media
                    IEnumerable<ModuleMasterDTO> ObjCache = CacheHelper<IEnumerable<ModuleMasterDTO>>.GetCacheItem("Cached_ModuleMaster_Master_");
                    if (ObjCache != null)
                    {
                        List<ModuleMasterDTO> tempC = new List<ModuleMasterDTO>();
                        tempC.Add(objDTO);

                        IEnumerable<ModuleMasterDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                        CacheHelper<IEnumerable<ModuleMasterDTO>>.AppendToCacheItem("Cached_ModuleMaster_Master_", NewCache);
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
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                ModuleMaster obj = context.ModuleMasters.Single(t => t.ID == id);
                obj.IsDeleted = true;
                obj.Updated = DateTime.UtcNow;
                obj.LastUpdatedBy = userid;
                context.ModuleMasters.Attach(obj);
                context.ObjectStateManager.ChangeObjectState(obj, EntityState.Modified);
                context.SaveChanges();

                //Get Cached-Media
                IEnumerable<ModuleMasterDTO> ObjCache = CacheHelper<IEnumerable<ModuleMasterDTO>>.GetCacheItem("Cached_ModuleMaster_Master_");
                if (ObjCache != null)
                {
                    List<ModuleMasterDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => i.ID == id);
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<ModuleMasterDTO>>.AppendToCacheItem("Cached_ModuleMaster_Master_", ObjCache);
                }
                return true;
            }
        }

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool Edit(ModuleMasterDTO objDTO)
        {
            objDTO.Updated = DateTime.UtcNow;

            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                ModuleMaster obj = new ModuleMaster();
                obj.ID = objDTO.ID;
                obj.ModuleName = objDTO.ModuleName;
                obj.DisplayName = objDTO.DisplayName;
                obj.ParentID = objDTO.ParentID;
                obj.Value = objDTO.Value;
                obj.Priority = objDTO.Priority;
                obj.IsModule = objDTO.IsModule;
                obj.GroupID = objDTO.GroupId;

                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Updated = objDTO.Updated;
                obj.Created = objDTO.Created;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.GUID = objDTO.GUID;
                obj.IsDeleted = false;

                context.ModuleMasters.Attach(obj);
                context.ObjectStateManager.ChangeObjectState(obj, EntityState.Modified);
                context.SaveChanges();

                //Get Cached-Media
                IEnumerable<ModuleMasterDTO> ObjCache = CacheHelper<IEnumerable<ModuleMasterDTO>>.GetCacheItem("Cached_ModuleMaster_Master_");
                if (ObjCache != null)
                {
                    List<ModuleMasterDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => i.ID == objDTO.ID);
                    ObjCache = objTemp.AsEnumerable();

                    List<ModuleMasterDTO> tempC = new List<ModuleMasterDTO>();
                    tempC.Add(objDTO);
                    IEnumerable<ModuleMasterDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                    CacheHelper<IEnumerable<ModuleMasterDTO>>.AppendToCacheItem("Cached_ModuleMaster_Master_", NewCache);
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
        public bool DeleteRecords(string IDs, int userid)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE ModuleMaster SET Updated = '" + DateTime.UtcNow.ToString() + "' , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.ExecuteStoreCommand(strQuery);

                //Get Cached-Media
                IEnumerable<ModuleMasterDTO> ObjCache = CacheHelper<IEnumerable<ModuleMasterDTO>>.GetCacheItem("Cached_ModuleMaster_Master_");
                if (ObjCache != null)
                {
                    List<ModuleMasterDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => IDs.Split(',').Contains(i.ID.ToString()));
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<ModuleMasterDTO>>.AppendToCacheItem("Cached_ModuleMaster_Master_", ObjCache);
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
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "UPDATE ModuleMaster SET " + columnName + " = '" + value + "', Updated = DATETIME ('" + System.DateTime.UtcNow.ToString(Convert.ToString((new System.Configuration.AppSettingsReader()).GetValue("FormatDateTimetoStore", typeof(System.String)))) + "') WHERE ID=" + id;
                context.ExecuteStoreCommand(strQuery);
            }
            return value;
        }


        public IEnumerable<ParentModuleMasterDTO> GetParentRecord()
        {
            return GetParentCachedData().OrderBy("ID ASC");
        }

        public string GetModuleNotification(Int32 ModuleId, Int64 UserId)
        {
            //Get Cached-Media
            IEnumerable<ModuleNotificationDTO> ObjCache = null;// CacheHelper<IEnumerable<ModuleNotificationDTO>>.GetCacheItem("Cached_ModuleNotification");
            if (ObjCache == null)
            {
                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<ModuleNotificationDTO> obj = (from u in context.ExecuteStoreQuery<ModuleNotificationDTO>(@"SELECT A.* FROM ModuleNotification A")
                                                              select new ModuleNotificationDTO
                                                              {
                                                                  NotificationID = u.NotificationID,
                                                                  ModuleId = u.ModuleId,
                                                                  UserID = u.UserID,
                                                                  Notification = u.Notification
                                                              }).AsParallel().ToList();
                    ObjCache = CacheHelper<IEnumerable<ModuleNotificationDTO>>.AddCacheItem("Cached_ModuleNotification", obj);
                }
            }

            return ObjCache.Where(c => c.ModuleId == ModuleId).Select(c => Convert.ToString(c.Notification)).SingleOrDefault();

            //            return "";
        }

        /// <summary>
        /// Get Particullar Record from the Module Master by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public ModuleMasterDTO GetRecord(Int64 id)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<ModuleMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, A.GUID FROM ModuleMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID WHERE A.IsDeleted!=1  AND A.ID=" + id.ToString())
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
                            LastUpdatedBy = u.LastUpdatedBy,
                            CreatedBy = u.CreatedBy,
                            GUID = u.GUID
                        }).SingleOrDefault();
            }
        }

        public IEnumerable<ParentModuleMasterDTO> GetParentCachedData()
        {
            //Get Cached-Media
            IEnumerable<ParentModuleMasterDTO> ObjCache = CacheHelper<IEnumerable<ParentModuleMasterDTO>>.GetCacheItem("Cached_ParentModuleMaster_");
            if (ObjCache == null)
            {
                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<ParentModuleMasterDTO> obj = (from u in context.ExecuteStoreQuery<ParentModuleMasterDTO>(@"SELECT A.* FROM ParentModuleMaster A")
                                                              select new ParentModuleMasterDTO
                                                              {
                                                                  ID = u.ID,
                                                                  ModuleName = u.ModuleName,
                                                                  GUID = u.GUID
                                                              }).AsParallel().ToList();
                    ObjCache = CacheHelper<IEnumerable<ParentModuleMasterDTO>>.AddCacheItem("Cached_ParentModuleMaster_", obj);
                }
            }
            return ObjCache;
        }
    }
}
