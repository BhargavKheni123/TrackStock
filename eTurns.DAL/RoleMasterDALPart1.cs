using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Data.SqlClient;
using Svg;
using System.Transactions;

namespace eTurns.DAL
{
    public class RoleMasterDAL : eTurnsBaseDAL
    {
        public IEnumerable<RoleMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId)
        {
            //Get Cached-Media
            IEnumerable<RoleMasterDTO> ObjCache = CacheHelper<IEnumerable<RoleMasterDTO>>.GetCacheItem("Cached_RoleMaster_" + CompanyId.ToString());
            if (ObjCache == null)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<RoleMasterDTO> obj = (from u in context.ExecuteStoreQuery<RoleMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,'' AS CreatedDate,'' As UpdatedDate FROM RoleMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.CompanyID = " + CompanyId.ToString())
                                                      select new RoleMasterDTO
                                                      {
                                                          GUID = u.GUID,
                                                          ID = u.ID,
                                                          RoleName = u.RoleName,
                                                          Description = u.Description,
                                                          Room = u.Room,
                                                          IsActive = u.IsActive,
                                                          CompanyID = u.CompanyID,

                                                          Created = u.Created,
                                                          Updated = u.Updated,
                                                          CreatedByName = u.CreatedByName,
                                                          UpdatedByName = u.UpdatedByName,
                                                          RoomName = u.RoomName,
                                                          CreatedDate = u.CreatedDate,
                                                          UpdatedDate = u.UpdatedDate
                                                      }).ToList();
                    ObjCache = CacheHelper<IEnumerable<RoleMasterDTO>>.AddCacheItem("Cached_RoleMaster_" + CompanyId.ToString(), obj);
                }
            }
            return ObjCache.Where(t => t.Room == RoomID);
        }

        public IEnumerable<RoleMasterDTO> GetPagedRecords(Int64 CompanyID, Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID)
        {
            //Get Cached-Media
            IEnumerable<RoleMasterDTO> ObjCache = CacheHelper<IEnumerable<RoleMasterDTO>>.GetCacheItem("Cached_RoleMaster_" + CompanyID.ToString());

            string strSortinitializer = "";
            if (sortColumnName.ToUpper().Contains("CREATEDBYNAME"))
            {
                strSortinitializer = "B";
                sortColumnName = "UserName";
            }
            else if (sortColumnName.ToUpper().Contains("UPDATEDBYNAME"))
            {
                strSortinitializer = "C";
                sortColumnName = "UserName";
            }
            else if (sortColumnName.ToUpper().Contains("ROOMNAME"))
            {
                strSortinitializer = "D";
            }
            else
            {
                strSortinitializer = "A";
            }

            strSortinitializer = strSortinitializer + "." + sortColumnName;

            if (String.IsNullOrEmpty(SearchTerm))
            {
                //if (ObjCache == null)
                //{
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<RoleMasterDTO> obj = (from u in context.ExecuteStoreQuery<RoleMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,'' As CreatedDate,'' AS UpdatedDate FROM RoleMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.CompanyID = " + CompanyID.ToString() + @" AND A.Room = " + RoomID.ToString() + @" AND A.IsArchived != 1")
                                                      select new RoleMasterDTO
                                                      {
                                                          GUID = u.GUID,
                                                          ID = u.ID,
                                                          RoleName = u.RoleName,
                                                          Description = u.Description,
                                                          Room = u.Room,
                                                          IsActive = u.IsActive,
                                                          CompanyID = u.CompanyID,

                                                          Created = u.Created,
                                                          Updated = u.Updated,
                                                          CreatedByName = u.CreatedByName,
                                                          UpdatedByName = u.UpdatedByName,
                                                          RoomName = u.RoomName,
                                                          CreatedDate = u.CreatedDate,
                                                          UpdatedDate = u.UpdatedDate
                                                      }).ToList();

                    foreach (RoleMasterDTO item in obj)
                    {
                        if (item.ID > 0)
                        {
                            item.PermissionList = null;// GetRoleModuleDetailsRecord(item.ID, item.Room);
                            item.ReplenishingRooms = GetRoleRoomReplanishmentDetailsRecord(item.ID);
                        }
                    }
                    ObjCache = CacheHelper<IEnumerable<RoleMasterDTO>>.AddCacheItem("Cached_RoleMaster_" + CompanyID.ToString(), obj);
                }
                // }
                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows).ToList();
            }
            else if (SearchTerm.Contains("[###]"))
            {

                string search = "";
                string[] dd = SearchTerm.Replace("STARTWITH#", "$").Split('$');
                string[] stringSeparators = new string[] { "[###]" };

                if (dd != null && dd.Length > 0)
                {
                    string[] Fields = dd[1].Split(stringSeparators, StringSplitOptions.None);
                    if (Fields != null && Fields.Length > 0)
                    {
                        // 6 counts for fields based on that prepare the search string
                        // 1 = createdBy, 2 = updatedby , 3 = CreatedFrom, 4 = CreatedTo, 5=UpdatedFrom,6=UpdatedTo
                        foreach (var item in Fields)
                        {
                            if (item.Length > 0)
                            {
                                if (item.Contains("CreatedBy"))
                                {
                                    search += " A.CreatedBy in (" + item.Split('#')[1] + ")";
                                    //int[] values = new int[] { 1, 2, 3 };
                                    //ObjCache = ObjCache.AsQueryable().Where("CreatedBy in (@0)", "1,2");
                                }
                                if (item.Contains("UpdatedBy"))
                                {
                                    if (search.Length > 0)
                                        search += " AND ";
                                    search += " A.LastUpdatedBy in (" + item.Split('#')[1] + ")";
                                }
                                if (item.Contains("DateCreatedFrom"))
                                {
                                    if (search.Length > 0)
                                        search += " AND ";
                                    string[] CreatedDateFrom = item.Split('#');
                                    search += " (A.Created >= DATETIME('" + CreatedDateFrom[1] + "') AND A.Created <= DATETIME('" + CreatedDateFrom[3] + "'))";
                                }
                                if (item.Contains("DateUpdatedFrom"))
                                {
                                    if (search.Length > 0)
                                        search += " AND ";
                                    string[] UpdatedDateFrom = item.Split('#');
                                    search += " (A.Updated >= DATETIME('" + UpdatedDateFrom[1] + "') AND A.Updated <= DATETIME('" + UpdatedDateFrom[3] + "'))";
                                }
                            }
                        }
                    }
                }

                if (search.Length > 0)
                {
                    search = " AND (" + search + " )";
                }
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    TotalCount = Convert.ToInt32(context.ExecuteStoreQuery<Int32>(@"SELECT Count(A.ID) FROM RoleMaster as A WHERE A.IsDeleted!=1 AND A.CompanyID = " + CompanyID.ToString() + @" AND A.Room = " + RoomID.ToString() + @" AND A.IsArchived != 1 " + search + "").ToList()[0]);

                    IEnumerable<RoleMasterDTO> obj = (from u in context.ExecuteStoreQuery<RoleMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,'' AS CreatedDate,'' AS UpdatedDate FROM RoleMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.CompanyID = " + CompanyID.ToString() + @" AND A.Room = " + RoomID.ToString() + @" AND A.IsArchived != 1 " + search + " ORDER BY " + strSortinitializer)
                                                      select new RoleMasterDTO
                                                      {
                                                          GUID = u.GUID,
                                                          ID = u.ID,
                                                          RoleName = u.RoleName,
                                                          Description = u.Description,
                                                          Room = u.Room,
                                                          IsActive = u.IsActive,
                                                          CompanyID = u.CompanyID,

                                                          Created = u.Created,
                                                          Updated = u.Updated,
                                                          CreatedByName = u.CreatedByName,
                                                          UpdatedByName = u.UpdatedByName,
                                                          RoomName = u.RoomName,
                                                          CreatedDate = u.CreatedDate,
                                                          UpdatedDate = u.UpdatedDate
                                                      }).Skip(StartRowIndex).Take(MaxRows).ToList();

                    foreach (RoleMasterDTO item in obj)
                    {
                        if (item.ID > 0)
                        {
                            item.PermissionList = null;// GetRoleModuleDetailsRecord(item.ID);
                            item.ReplenishingRooms = GetRoleRoomReplanishmentDetailsRecord(item.ID);
                        }
                    }
                    return obj;
                }
            }
            else if (SearchTerm.Contains("#IsDeletedRecords#"))
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    TotalCount = Convert.ToInt32(context.ExecuteStoreQuery<Int32>(@"SELECT Count(A.ID) FROM RoleMaster as A WHERE A.IsArchived != 1").ToList()[0]);

                    IEnumerable<RoleMasterDTO> obj = (from u in context.ExecuteStoreQuery<RoleMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, A.IsDeleted,A.IsArchived,'' AS CreatedDate,'' AS UpdatedDate FROM RoleMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsArchived != 1  ORDER BY " + strSortinitializer)
                                                      select new RoleMasterDTO
                                                      {
                                                          GUID = u.GUID,
                                                          ID = u.ID,
                                                          RoleName = u.RoleName,
                                                          Description = u.Description,
                                                          Room = u.Room,
                                                          IsActive = u.IsActive,
                                                          CompanyID = u.CompanyID,

                                                          Created = u.Created,
                                                          Updated = u.Updated,
                                                          CreatedByName = u.CreatedByName,
                                                          UpdatedByName = u.UpdatedByName,
                                                          RoomName = u.RoomName,
                                                          CreatedDate = u.CreatedDate,
                                                          UpdatedDate = u.UpdatedDate
                                                      }).Skip(StartRowIndex).Take(MaxRows).ToList();

                    foreach (RoleMasterDTO item in obj)
                    {
                        if (item.ID > 0)
                        {
                            item.PermissionList = null;// GetRoleModuleDetailsRecord(item.ID);
                            item.ReplenishingRooms = GetRoleRoomReplanishmentDetailsRecord(item.ID);
                        }
                    }
                    return obj;


                }
            }
            else if (SearchTerm.Contains("#IsArchivedRecords#"))
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    TotalCount = Convert.ToInt32(context.ExecuteStoreQuery<Int32>(@"SELECT Count(A.ID) FROM RoleMaster as A WHERE A.IsDeleted != 1").ToList()[0]);

                    IEnumerable<RoleMasterDTO> obj = (from u in context.ExecuteStoreQuery<RoleMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, A.IsDeleted,A.IsArchived,'' AS CreatedDate,'' as UpdatedDate FROM RoleMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted != 1  ORDER BY " + strSortinitializer)
                                                      select new RoleMasterDTO
                                                      {
                                                          GUID = u.GUID,
                                                          ID = u.ID,
                                                          RoleName = u.RoleName,
                                                          Description = u.Description,
                                                          Room = u.Room,
                                                          IsActive = u.IsActive,
                                                          CompanyID = u.CompanyID,

                                                          Created = u.Created,
                                                          Updated = u.Updated,
                                                          CreatedByName = u.CreatedByName,
                                                          UpdatedByName = u.UpdatedByName,
                                                          RoomName = u.RoomName,
                                                          CreatedDate = u.CreatedDate,
                                                          UpdatedDate = u.UpdatedDate
                                                      }).Skip(StartRowIndex).Take(MaxRows).ToList();

                    foreach (RoleMasterDTO item in obj)
                    {
                        if (item.ID > 0)
                        {
                            item.PermissionList = null;//  GetRoleModuleDetailsRecord(item.ID);
                            item.ReplenishingRooms = GetRoleRoomReplanishmentDetailsRecord(item.ID);
                        }
                    }
                    return obj;
                }
            }
            else if (SearchTerm.Contains("#BOTH#"))
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    TotalCount = Convert.ToInt32(context.ExecuteStoreQuery<Int32>(@"SELECT Count(A.ID) FROM RoleMaster as A").ToList()[0]);

                    IEnumerable<RoleMasterDTO> obj = (from u in context.ExecuteStoreQuery<RoleMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, A.IsDeleted,A.IsArchived,'' As CreatedDate,'' AS UpdatedDate FROM RoleMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID ORDER BY " + strSortinitializer)
                                                      select new RoleMasterDTO
                                                      {
                                                          GUID = u.GUID,
                                                          ID = u.ID,
                                                          RoleName = u.RoleName,
                                                          Description = u.Description,
                                                          Room = u.Room,
                                                          IsActive = u.IsActive,
                                                          CompanyID = u.CompanyID,

                                                          Created = u.Created,
                                                          Updated = u.Updated,
                                                          CreatedByName = u.CreatedByName,
                                                          UpdatedByName = u.UpdatedByName,
                                                          RoomName = u.RoomName,
                                                          CreatedDate = u.CreatedDate,
                                                          UpdatedDate = u.UpdatedDate
                                                      }).Skip(StartRowIndex).Take(MaxRows).ToList();

                    foreach (RoleMasterDTO item in obj)
                    {
                        if (item.ID > 0)
                        {
                            item.PermissionList = null;//  GetRoleModuleDetailsRecord(item.ID);
                            item.ReplenishingRooms = GetRoleRoomReplanishmentDetailsRecord(item.ID);
                        }
                    }
                    return obj;
                }
            }
            else
            {

                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    TotalCount = Convert.ToInt32(context.ExecuteStoreQuery<Int32>(@"SELECT Count(ID) FROM RoleMaster WHERE IsDeleted!=1 AND CompanyID = " + CompanyID.ToString() + @" AND Room = " + RoomID.ToString() + @" AND IsArchived != 1 AND ((RoleName like '%" + SearchTerm + "%') OR (ID like '%" + SearchTerm + "%'))").ToList()[0]);
                    IEnumerable<RoleMasterDTO> obj = (from u in context.ExecuteStoreQuery<RoleMasterDTO>("SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,'' AS CreatedDate,'' AS UpdatedDate FROM RoleMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.CompanyID = " + CompanyID.ToString() + @" AND A.Room = " + RoomID.ToString() + @" AND A.IsArchived != 1 AND ((A.RoleName like '%" + SearchTerm + "%') OR (A.ID like '%" + SearchTerm + "%')) ORDER BY " + strSortinitializer)
                                                      select new RoleMasterDTO
                                                      {
                                                          GUID = u.GUID,
                                                          ID = u.ID,
                                                          RoleName = u.RoleName,
                                                          Description = u.Description,
                                                          Room = u.Room,
                                                          IsActive = u.IsActive,
                                                          CompanyID = u.CompanyID,

                                                          Created = u.Created,
                                                          Updated = u.Updated,
                                                          CreatedByName = u.CreatedByName,
                                                          UpdatedByName = u.UpdatedByName,
                                                          RoomName = u.RoomName,
                                                          CreatedDate = u.CreatedDate,
                                                          UpdatedDate = u.UpdatedDate
                                                      }).Skip(StartRowIndex).Take(MaxRows).ToList();

                    foreach (RoleMasterDTO item in obj)
                    {
                        if (item.ID > 0)
                        {
                            item.PermissionList = null;// GetRoleModuleDetailsRecord(item.ID);
                            item.ReplenishingRooms = GetRoleRoomReplanishmentDetailsRecord(item.ID);
                        }
                    }
                    return obj;
                }
            }

        }

        public RoleMasterDTO GetRecord(Int64 id)
        {
            RoleMasterDTO objresult = new RoleMasterDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objresult = (from u in context.ExecuteStoreQuery<RoleMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, A.GUID FROM RoleMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE  A.ID=" + id.ToString())
                             select new RoleMasterDTO
                             {
                                 ID = u.ID,
                                 RoleName = u.RoleName,
                                 Description = u.Description,
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
                                 EnterpriseId = u.EnterpriseId,
                                 UserType = u.UserType,
                                 IsDeleted = u.IsDeleted,
                                 IsArchived = u.IsArchived
                             }).SingleOrDefault();

                objresult.PermissionList = GetRoleModuleDetailsRecord(id, 0);
                string RoomLists = "";
                if (objresult.PermissionList != null && objresult.PermissionList.Count > 0)
                {
                    objresult.RoleWiseRoomsAccessDetails = ConvertPermissions(objresult.PermissionList, id, ref RoomLists);
                    objresult.SelectedRoomAccessValue = RoomLists;
                }
                objresult.lstAccess = (from rra in context.RoleRoomAccesses
                                       join cm in context.CompanyMasters on rra.CompanyId equals cm.ID into rra_cm_join
                                       from rra_cm in rra_cm_join.DefaultIfEmpty()
                                       join rm in context.Rooms on rra.RoomId equals rm.ID into rra_rm_join
                                       from rra_rm in rra_rm_join.DefaultIfEmpty()
                                       where rra.RoleId == id
                                       select new UserAccessDTO
                                       {
                                           CompanyId = rra.CompanyId,
                                           EnterpriseId = rra.EnterpriseId,
                                           ID = rra.ID,
                                           RoleId = id,
                                           RoomId = rra.RoomId,
                                           UserId = 0,
                                           EnterpriseName = string.Empty,
                                           CompanyName = rra_cm.Name,
                                           RoomName = rra_rm.RoomName
                                       }).ToList();
                //objresult.ReplenishingRooms = GetRoleRoomReplanishmentDetailsRecord(id);
                //if (objresult.ReplenishingRooms != null && objresult.ReplenishingRooms.Count > 0)
                //{
                //    string ReplenishRoomLists = "";
                //    foreach (RoleRoomReplanishmentDetailsDTO item in objresult.ReplenishingRooms)
                //    {
                //        if (string.IsNullOrEmpty(ReplenishRoomLists))
                //        {
                //            ReplenishRoomLists = item.EnterpriseId + "_" + item.CompanyId + "_" + item.RoomID.ToString();// +"_" + item.RoomName;
                //        }
                //        else
                //            ReplenishRoomLists += "," + item.EnterpriseId + "_" + item.CompanyId + "_" + item.RoomID.ToString();// +"_" + item.RoomName;
                //    }

                //    objresult.SelectedRoomReplanishmentValue = ReplenishRoomLists;
                //}

            }
            return objresult;
        }

        public RoleMasterDTO GetRecord(Int64 id, long EnterpriseID, long CompanyID, long RoomID)
        {
            RoleMasterDTO objresult = new RoleMasterDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objresult = (from u in context.ExecuteStoreQuery<RoleMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, A.GUID FROM RoleMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.ID=" + id.ToString())
                             select new RoleMasterDTO
                             {
                                 ID = u.ID,
                                 RoleName = u.RoleName,
                                 Description = u.Description,
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
                                 EnterpriseId = u.EnterpriseId,
                                 UserType = u.UserType
                             }).SingleOrDefault();

                objresult.PermissionList = GetRoleModuleDetailsRecord(id, RoomID);
                string RoomLists = "";
                if (objresult.PermissionList != null && objresult.PermissionList.Count > 0)
                {
                    objresult.RoleWiseRoomsAccessDetails = ConvertPermissions(objresult.PermissionList, id, ref RoomLists);
                    objresult.SelectedRoomAccessValue = RoomLists;
                }
                objresult.lstAccess = (from rra in context.RoleRoomAccesses
                                       join cm in context.CompanyMasters on rra.CompanyId equals cm.ID into rra_cm_join
                                       from rra_cm in rra_cm_join.DefaultIfEmpty()
                                       join rm in context.Rooms on rra.RoomId equals rm.ID into rra_rm_join
                                       from rra_rm in rra_rm_join.DefaultIfEmpty()
                                       where rra.RoleId == id
                                       select new UserAccessDTO
                                       {
                                           CompanyId = rra.CompanyId,
                                           EnterpriseId = rra.EnterpriseId,
                                           ID = rra.ID,
                                           RoleId = id,
                                           RoomId = rra.RoomId,
                                           UserId = 0,
                                           EnterpriseName = string.Empty,
                                           CompanyName = rra_cm.Name,
                                           RoomName = rra_rm.RoomName
                                       }).ToList();
                //objresult.ReplenishingRooms = GetRoleRoomReplanishmentDetailsRecord(id);
                //if (objresult.ReplenishingRooms != null && objresult.ReplenishingRooms.Count > 0)
                //{
                //    string ReplenishRoomLists = "";
                //    foreach (RoleRoomReplanishmentDetailsDTO item in objresult.ReplenishingRooms)
                //    {
                //        if (string.IsNullOrEmpty(ReplenishRoomLists))
                //        {
                //            ReplenishRoomLists = item.EnterpriseId + "_" + item.CompanyId + "_" + item.RoomID.ToString();// +"_" + item.RoomName;
                //        }
                //        else
                //            ReplenishRoomLists += "," + item.EnterpriseId + "_" + item.CompanyId + "_" + item.RoomID.ToString();// +"_" + item.RoomName;
                //    }

                //    objresult.SelectedRoomReplanishmentValue = ReplenishRoomLists;
                //}

            }
            return objresult;
        }

        public RoleMasterDTO GetHistoryRecord(Int64 id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<RoleMasterDTO>(@"SELECT A.ID,A.HistoryID,A.Role, A.Created, A.CreatedBy,A.Room, A.LastUpdatedBy, A.Updated, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, A.GUID FROM RoleMaster_History A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.HistoryID=" + id.ToString())
                        select new RoleMasterDTO
                        {
                            ID = u.ID,
                            //    HistoryID = u.HistoryID,
                            RoleName = u.RoleName,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            RoomName = u.RoomName,
                            LastUpdatedBy = u.LastUpdatedBy,
                            CreatedBy = u.CreatedBy,
                            Room = u.Room,
                            GUID = u.GUID
                        }).SingleOrDefault();
            }
        }

        public bool Edit(RoleMasterDTO objDTO)
        {
            RoleMaster obj = new RoleMaster();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                obj = context.RoleMasters.FirstOrDefault(t => t.ID == objDTO.ID);
                if (obj != null)
                {
                    obj.ID = objDTO.ID;
                    obj.RoleName = objDTO.RoleName;
                    obj.Description = objDTO.Description;
                    obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                    obj.Updated = DateTimeUtility.DateTimeNow;
                    context.SaveChanges();
                }
                string strQuery = "DELETE from RoleModuleDetails where RoleID=" + objDTO.ID;
                context.ExecuteStoreCommand(strQuery);
                if (objDTO.UserType > 1)
                {
                    if (objDTO.RoleWiseRoomsAccessDetails != null && objDTO.RoleWiseRoomsAccessDetails.Count > 0)
                    {
                        foreach (RoleWiseRoomsAccessDetailsDTO item in objDTO.RoleWiseRoomsAccessDetails)
                        {

                            if (item.PermissionList != null && item.PermissionList.Count > 0)
                            {
                                if (objDTO.UserType == 3)
                                {
                                    item.PermissionList = item.PermissionList.Where(x => x.ModuleID != 39).ToList();
                                }

                                foreach (RoleModuleDetailsDTO InternalItem in item.PermissionList)
                                {
                                    RoleModuleDetail objDetails = new RoleModuleDetail();
                                    objDetails.GUID = Guid.NewGuid();
                                    objDetails.IsChecked = InternalItem.IsChecked;
                                    objDetails.IsDelete = InternalItem.IsDelete;
                                    objDetails.IsInsert = InternalItem.IsInsert;
                                    objDetails.IsUpdate = InternalItem.IsUpdate;
                                    objDetails.IsView = InternalItem.IsView;

                                    objDetails.ShowDeleted = InternalItem.ShowDeleted;
                                    objDetails.ShowArchived = InternalItem.ShowArchived;
                                    objDetails.ShowUDF = InternalItem.ShowUDF;
                                    objDetails.ShowChangeLog = InternalItem.ShowChangeLog;
                                    objDetails.ModuleID = InternalItem.ModuleID;
                                    objDetails.ModuleValue = InternalItem.ModuleValue;
                                    objDetails.RoleId = obj.ID;
                                    objDetails.RoomId = item.RoomID;
                                    objDetails.CompanyID = item.CompanyID;
                                    objDetails.EnterpriseID = item.EnterpriseID;
                                    context.RoleModuleDetails.AddObject(objDetails);
                                    // context.SaveChanges();
                                }
                            }
                        }
                    }
                    IQueryable<RoleRoomAccess> lstExisting = context.RoleRoomAccesses.Where(t => t.RoleId == obj.ID);
                    if (lstExisting.Any())
                    {
                        foreach (var item in lstExisting)
                        {
                            context.RoleRoomAccesses.DeleteObject(item);
                        }
                    }
                    if (objDTO.lstAccess != null && objDTO.lstAccess.Count > 0)
                    {
                        foreach (var item in objDTO.lstAccess)
                        {
                            if (item.RoomId > 0)
                            {
                                RoleRoomAccess objRoleRoomAccess = new RoleRoomAccess();
                                objRoleRoomAccess.RoleId = obj.ID;
                                objRoleRoomAccess.EnterpriseId = item.EnterpriseId;
                                objRoleRoomAccess.CompanyId = item.CompanyId;
                                objRoleRoomAccess.RoomId = item.RoomId;
                                context.RoleRoomAccesses.AddObject(objRoleRoomAccess);
                            }
                        }
                    }
                    //strQuery = "DELETE from RoleRoomsDetails where RoleID=" + objDTO.ID;
                    //context.ExecuteStoreCommand(strQuery);

                    //if (objDTO.ReplenishingRooms != null && objDTO.ReplenishingRooms.Count > 0)
                    //{
                    //    foreach (RoleRoomReplanishmentDetailsDTO item in objDTO.ReplenishingRooms)
                    //    {

                    //        RoleRoomsDetail objDetails = new RoleRoomsDetail();
                    //        objDetails.GUID = Guid.NewGuid();
                    //        objDetails.RoleId = obj.ID;
                    //        objDetails.RoomId = item.RoomID;
                    //        objDetails.EnterpriseId = item.EnterpriseId;
                    //        objDetails.CompanyId = item.CompanyId;
                    //        objDetails.IsRoomAccess = false;
                    //        context.RoleRoomsDetails(objDetails);
                    //        // context.SaveChanges();
                    //    }
                    //}
                    context.SaveChanges();
                }
                return true;
            }
        }

        public bool DeleteRecords(string IDs, long userid)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE RoleMaster SET Updated = '" + DateTime.Now.ToString() + "' , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.ExecuteStoreCommand(strQuery);
                return true;
            }
        }

        public string UpdateData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "UPDATE RoleMaster SET " + columnName + " = '" + value + "', Updated = DATETIME ('" + System.DateTime.Now.ToString(Convert.ToString((new System.Configuration.AppSettingsReader()).GetValue("FormatDateTimetoStore", typeof(System.String)))) + "') WHERE ID=" + id;
                context.ExecuteStoreCommand(strQuery);
            }
            return value;
        }



    }
}
