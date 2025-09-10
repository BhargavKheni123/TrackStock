using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eTurns.DTO;
using System.Data;
using Dynamite;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Data.SqlClient;

namespace eTurnsMaster.DAL
{
    public partial class EnterPriseUserMasterDAL : eTurnsMasterBaseDAL
    {
        /// <summary>
        /// Get Paged Records from the User Master Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<UserMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId)
        {

            //Get Cached-Media
            IEnumerable<UserMasterDTO> ObjCache = CacheHelper<IEnumerable<UserMasterDTO>>.GetCacheItem("Cached_UserMaster_" + CompanyId.ToString());
            if (ObjCache == null)
            {
                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<UserMasterDTO> obj = (from u in context.ExecuteStoreQuery<UserMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM UserMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.CompanyID = " + CompanyId.ToString())
                                                      select new UserMasterDTO
                                                      {
                                                          ID = u.ID,
                                                          GUID = u.GUID,
                                                          UserName = u.UserName,
                                                          Password = u.Password,
                                                          //   ConfirmPassword=u.Password,
                                                          Phone = u.Phone,
                                                          RoleID = u.RoleID,
                                                          Email = u.Email,
                                                          Room = u.Room,
                                                          IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                                          IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                                          CompanyID = u.CompanyID,
                                                          Created = u.Created,
                                                          Updated = u.Updated,
                                                          CreatedByName = u.CreatedByName,
                                                          UpdatedByName = u.UpdatedByName,
                                                          RoomName = u.RoomName
                                                      }).ToList();
                    ObjCache = CacheHelper<IEnumerable<UserMasterDTO>>.AddCacheItem("Cached_UserMaster_" + CompanyId.ToString(), obj);
                }
            }
            if (RoomID == 0)
            {
                return ObjCache;
            }
            return ObjCache.Where(t => t.Room == RoomID);
        }

        /// <summary>
        /// Get Paged Records from the Role Master Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<UserMasterDTO> GetPagedRecords(Int64 CompanyID, Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID)
        {
            //Get Cached-Media
            IEnumerable<UserMasterDTO> ObjCache = CacheHelper<IEnumerable<UserMasterDTO>>.GetCacheItem("Cached_UserMaster_" + CompanyID.ToString());

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
                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<UserMasterDTO> obj = (from u in context.ExecuteStoreQuery<UserMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM UserMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.CompanyID = " + CompanyID.ToString() + @" AND A.Room = " + RoomID.ToString() + @" AND A.IsArchived != 1")
                                                      select new UserMasterDTO
                                                      {
                                                          ID = u.ID,
                                                          GUID = u.GUID,
                                                          UserName = u.UserName,
                                                          Password = u.Password,
                                                          //ConfirmPassword = u.Password,
                                                          Phone = u.Phone,
                                                          RoleID = u.RoleID,
                                                          Email = u.Email,
                                                          Room = u.Room,
                                                          IsDeleted = u.IsDeleted,
                                                          IsArchived = u.IsArchived,
                                                          CompanyID = u.CompanyID,

                                                          Created = u.Created,
                                                          Updated = u.Updated,
                                                          CreatedByName = u.CreatedByName,
                                                          UpdatedByName = u.UpdatedByName,
                                                          RoomName = u.RoomName
                                                      }).ToList();

                    foreach (UserMasterDTO item in obj)
                    {
                        if (item.ID > 0)
                        {
                            item.PermissionList = null;// GetRoleModuleDetailsRecord(item.ID, item.Room);
                            item.ReplenishingRooms = GetUserRoomReplanishmentDetailsRecord(item.ID);
                        }
                    }
                    ObjCache = CacheHelper<IEnumerable<UserMasterDTO>>.AddCacheItem("Cached_UserMaster_" + CompanyID.ToString(), obj);
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
                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    TotalCount = Convert.ToInt32(context.ExecuteStoreQuery<Int32>(@"SELECT Count(A.ID) FROM UserMaster as A WHERE A.IsDeleted!=1 AND A.CompanyID = " + CompanyID.ToString() + @" AND A.Room = " + RoomID.ToString() + @" AND A.IsArchived != 1 " + search + "").ToList()[0]);

                    IEnumerable<UserMasterDTO> obj = (from u in context.ExecuteStoreQuery<UserMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM UserMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.CompanyID = " + CompanyID.ToString() + @" AND A.Room = " + RoomID.ToString() + @" AND A.IsArchived != 1 " + search + " ORDER BY " + strSortinitializer)
                                                      select new UserMasterDTO
                                                      {
                                                          ID = u.ID,
                                                          GUID = u.GUID,
                                                          UserName = u.UserName,
                                                          Password = u.Password,
                                                          //ConfirmPassword = u.Password,
                                                          Phone = u.Phone,
                                                          RoleID = u.RoleID,
                                                          Email = u.Email,
                                                          Room = u.Room,
                                                          IsDeleted = u.IsDeleted,
                                                          IsArchived = u.IsArchived,
                                                          CompanyID = u.CompanyID,

                                                          Created = u.Created,
                                                          Updated = u.Updated,
                                                          CreatedByName = u.CreatedByName,
                                                          UpdatedByName = u.UpdatedByName,
                                                          RoomName = u.RoomName
                                                      }).Skip(StartRowIndex).Take(MaxRows).ToList();

                    foreach (UserMasterDTO item in obj)
                    {
                        if (item.ID > 0)
                        {
                            item.PermissionList = null;// GetRoleModuleDetailsRecord(item.ID);
                            item.ReplenishingRooms = GetUserRoomReplanishmentDetailsRecord(item.ID);
                        }
                    }
                    return obj;
                }
            }
            else if (SearchTerm.Contains("#IsDeletedRecords#"))
            {
                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    TotalCount = Convert.ToInt32(context.ExecuteStoreQuery<Int32>(@"SELECT Count(A.ID) FROM UserMaster as A WHERE A.IsArchived != 1").ToList()[0]);

                    IEnumerable<UserMasterDTO> obj = (from u in context.ExecuteStoreQuery<UserMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, A.IsDeleted,A.IsArchived FROM UserMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsArchived != 1  ORDER BY " + strSortinitializer)
                                                      select new UserMasterDTO
                                                      {
                                                          ID = u.ID,
                                                          GUID = u.GUID,
                                                          UserName = u.UserName,
                                                          Password = u.Password,
                                                          //ConfirmPassword = u.Password,
                                                          Phone = u.Phone,
                                                          RoleID = u.RoleID,
                                                          Email = u.Email,
                                                          Room = u.Room,
                                                          IsDeleted = u.IsDeleted,
                                                          IsArchived = u.IsArchived,
                                                          CompanyID = u.CompanyID,

                                                          Created = u.Created,
                                                          Updated = u.Updated,
                                                          CreatedByName = u.CreatedByName,
                                                          UpdatedByName = u.UpdatedByName,
                                                          RoomName = u.RoomName
                                                      }).Skip(StartRowIndex).Take(MaxRows).ToList();

                    foreach (UserMasterDTO item in obj)
                    {
                        if (item.ID > 0)
                        {
                            item.PermissionList = null;// GetRoleModuleDetailsRecord(item.ID);
                            item.ReplenishingRooms = GetUserRoomReplanishmentDetailsRecord(item.ID);
                        }
                    }
                    return obj;


                }
            }
            else if (SearchTerm.Contains("#IsArchivedRecords#"))
            {
                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    TotalCount = Convert.ToInt32(context.ExecuteStoreQuery<Int32>(@"SELECT Count(A.ID) FROM UserMaster as A WHERE A.IsDeleted != 1").ToList()[0]);

                    IEnumerable<UserMasterDTO> obj = (from u in context.ExecuteStoreQuery<UserMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, A.IsDeleted,A.IsArchived FROM UserMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted != 1  ORDER BY " + strSortinitializer)
                                                      select new UserMasterDTO
                                                      {
                                                          ID = u.ID,
                                                          GUID = u.GUID,
                                                          UserName = u.UserName,
                                                          Password = u.Password,
                                                          //ConfirmPassword = u.Password,
                                                          Phone = u.Phone,
                                                          RoleID = u.RoleID,
                                                          Email = u.Email,
                                                          Room = u.Room,
                                                          IsDeleted = u.IsDeleted,
                                                          IsArchived = u.IsArchived,
                                                          CompanyID = u.CompanyID,

                                                          Created = u.Created,
                                                          Updated = u.Updated,
                                                          CreatedByName = u.CreatedByName,
                                                          UpdatedByName = u.UpdatedByName,
                                                          RoomName = u.RoomName
                                                      }).Skip(StartRowIndex).Take(MaxRows).ToList();

                    foreach (UserMasterDTO item in obj)
                    {
                        if (item.ID > 0)
                        {
                            item.PermissionList = null;//  GetRoleModuleDetailsRecord(item.ID);
                            item.ReplenishingRooms = GetUserRoomReplanishmentDetailsRecord(item.ID);
                        }
                    }
                    return obj;
                }
            }
            else if (SearchTerm.Contains("#BOTH#"))
            {
                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    TotalCount = Convert.ToInt32(context.ExecuteStoreQuery<Int32>(@"SELECT Count(A.ID) FROM UserMaster as A").ToList()[0]);

                    IEnumerable<UserMasterDTO> obj = (from u in context.ExecuteStoreQuery<UserMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, A.IsDeleted,A.IsArchived FROM UserMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID ORDER BY " + strSortinitializer)
                                                      select new UserMasterDTO
                                                      {
                                                          ID = u.ID,
                                                          GUID = u.GUID,
                                                          UserName = u.UserName,
                                                          Password = u.Password,
                                                          //ConfirmPassword = u.Password,
                                                          Phone = u.Phone,
                                                          RoleID = u.RoleID,
                                                          Email = u.Email,
                                                          Room = u.Room,
                                                          IsDeleted = u.IsDeleted,
                                                          IsArchived = u.IsArchived,
                                                          CompanyID = u.CompanyID,

                                                          Created = u.Created,
                                                          Updated = u.Updated,
                                                          CreatedByName = u.CreatedByName,
                                                          UpdatedByName = u.UpdatedByName,
                                                          RoomName = u.RoomName
                                                      }).Skip(StartRowIndex).Take(MaxRows).ToList();

                    foreach (UserMasterDTO item in obj)
                    {
                        if (item.ID > 0)
                        {
                            item.PermissionList = null;//  GetRoleModuleDetailsRecord(item.ID);
                            item.ReplenishingRooms = GetUserRoomReplanishmentDetailsRecord(item.ID);
                        }
                    }
                    return obj;
                }
            }
            else
            {

                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    TotalCount = Convert.ToInt32(context.ExecuteStoreQuery<Int32>(@"SELECT Count(ID) FROM UserMaster WHERE IsDeleted!=1 AND CompanyID = " + CompanyID.ToString() + @" AND Room = " + RoomID.ToString() + @" AND IsArchived != 1 AND ((UserName like '%" + SearchTerm + "%') OR (ID like '%" + SearchTerm + "%'))").ToList()[0]);
                    IEnumerable<UserMasterDTO> obj = (from u in context.ExecuteStoreQuery<UserMasterDTO>("SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM UserMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.CompanyID = " + CompanyID.ToString() + @" AND A.Room = " + RoomID.ToString() + @" AND A.IsArchived != 1 AND ((A.UserName like '%" + SearchTerm + "%') OR (A.ID like '%" + SearchTerm + "%')) ORDER BY " + strSortinitializer)
                                                      select new UserMasterDTO
                                                      {
                                                          ID = u.ID,
                                                          GUID = u.GUID,
                                                          UserName = u.UserName,
                                                          Password = u.Password,
                                                          //ConfirmPassword = u.Password,
                                                          Phone = u.Phone,
                                                          RoleID = u.RoleID,
                                                          Email = u.Email,
                                                          Room = u.Room,
                                                          IsDeleted = u.IsDeleted,
                                                          IsArchived = u.IsArchived,
                                                          CompanyID = u.CompanyID,

                                                          Created = u.Created,
                                                          Updated = u.Updated,
                                                          CreatedByName = u.CreatedByName,
                                                          UpdatedByName = u.UpdatedByName,
                                                          RoomName = u.RoomName
                                                      }).Skip(StartRowIndex).Take(MaxRows).ToList();

                    foreach (UserMasterDTO item in obj)
                    {
                        if (item.ID > 0)
                        {
                            item.PermissionList = null;// GetRoleModuleDetailsRecord(item.ID);
                            item.ReplenishingRooms = GetUserRoomReplanishmentDetailsRecord(item.ID);
                        }
                    }
                    return obj;
                }
            }

        }

        /// <summary>
        /// Get Paged Records from the User Master Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<UserMasterDTO> GetAllUsers()
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<UserMasterDTO> obj = (from u in context.ExecuteStoreQuery<UserMasterDTO>(@"SELECT A.*,A.RoomId as Room FROM UserMaster A WHERE A.IsDeleted!=1 AND A.IsArchived != 1")
                                                  select new UserMasterDTO
                                                  {
                                                      ID = u.ID,
                                                      GUID = u.GUID,
                                                      UserName = u.UserName,
                                                      Password = u.Password,
                                                      Phone = u.Phone,
                                                      RoleID = u.RoleID,
                                                      Email = u.Email,
                                                      Room = u.Room,
                                                      IsDeleted = u.IsDeleted ?? false,
                                                      IsArchived = u.IsArchived ?? false,
                                                      CompanyID = u.CompanyID

                                                  }).ToList();
                return obj;
            }

        }

        /// <summary>
        /// Get Particullar Record from the User Master by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public UserMasterDTO GetRecord(Int64 id)
        {
            UserMasterDTO objresult = new UserMasterDTO();
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                objresult = (from u in context.ExecuteStoreQuery<UserMasterDTO>(@"select um.*,um.CompanyID,um.RoomId as Room,em.EnterpriseDBName FROM UserMaster as um left join EnterpriseMaster as em on um.EnterpriseId = em.ID WHERE um.IsDeleted!=1 AND um.IsArchived != 1 AND um.ID=" + id.ToString())
                             select new UserMasterDTO
                             {
                                 ID = u.ID,
                                 GUID = u.GUID,
                                 UserName = u.UserName,
                                 Password = u.Password,
                                 //ConfirmPassword = u.Password,
                                 Phone = u.Phone,
                                 RoleID = u.RoleID,
                                 Email = u.Email,
                                 Room = u.Room,
                                 IsDeleted = u.IsDeleted,
                                 IsArchived = u.IsArchived,
                                 CompanyID = u.CompanyID,
                                 EnterpriseDbName = u.EnterpriseDbName,
                                 Created = u.Created,
                                 Updated = u.Updated,
                                 CreatedByName = u.CreatedByName,
                                 UpdatedByName = u.UpdatedByName,
                                 RoomName = u.RoomName,
                                 LastUpdatedBy = u.LastUpdatedBy,
                                 CreatedBy = u.CreatedBy,
                                 UserType = u.UserType,
                                 EnterpriseId = u.EnterpriseId
                             }).SingleOrDefault();

                objresult.PermissionList = GetUserRoleModuleDetailsRecord(id, objresult.RoleID, 0);
                string RoomLists = "";
                if (objresult.PermissionList != null && objresult.PermissionList.Count > 0)
                {
                    objresult.UserWiseAllRoomsAccessDetails = ConvertUserPermissions(objresult.PermissionList, objresult.RoleID, ref RoomLists);
                    objresult.SelectedRoomAccessValue = RoomLists;
                }
                objresult.ReplenishingRooms = GetUserRoomReplanishmentDetailsRecord(id);
            }
            return objresult;
        }

        public UserMasterDTO GetRecordForService(Int64 id)
        {
            UserMasterDTO objresult = new UserMasterDTO();
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                objresult = (from u in context.ExecuteStoreQuery<UserMasterDTO>(@"select um.*,um.CompanyID,um.RoomId as Room,em.EnterpriseDBName FROM UserMaster as um left join EnterpriseMaster as em on um.EnterpriseId = em.ID WHERE um.IsDeleted!=1 AND um.IsArchived != 1 AND um.ID=" + id.ToString())
                             select new UserMasterDTO
                             {
                                 ID = u.ID,
                                 GUID = u.GUID,
                                 UserName = u.UserName,
                                 Password = u.Password,
                                 //ConfirmPassword = u.Password,
                                 Phone = u.Phone,
                                 RoleID = u.RoleID,
                                 Email = u.Email,
                                 Room = u.Room,
                                 IsDeleted = u.IsDeleted,
                                 IsArchived = u.IsArchived,
                                 CompanyID = u.CompanyID,
                                 EnterpriseDbName = u.EnterpriseDbName,
                                 Created = u.Created,
                                 Updated = u.Updated,
                                 CreatedByName = u.CreatedByName,
                                 UpdatedByName = u.UpdatedByName,
                                 RoomName = u.RoomName,
                                 LastUpdatedBy = u.LastUpdatedBy,
                                 CreatedBy = u.CreatedBy,
                                 UserType = u.UserType,
                                 EnterpriseId = u.EnterpriseId
                             }).SingleOrDefault();
            }
            return objresult;
        }

        private List<UserWiseRoomsAccessDetailsDTO> ConvertUserPermissions(List<UserRoleModuleDetailsDTO> objData, Int64 RoleID, ref string RoomLists)
        {
            List<UserWiseRoomsAccessDetailsDTO> objRooms = new List<UserWiseRoomsAccessDetailsDTO>();

            UserWiseRoomsAccessDetailsDTO objRoleRooms;

            var objTempPermissionList = objData.GroupBy(element => new { element.RoomId })
                                            .OrderBy(g => g.Key.RoomId);

            foreach (var grpData in objTempPermissionList)
            {
                if (grpData.Key.RoomId != 0)
                {
                    objRoleRooms = new UserWiseRoomsAccessDetailsDTO();
                    objRoleRooms.RoomID = grpData.Key.RoomId;
                    objRoleRooms.RoleID = RoleID;
                    List<UserRoleModuleDetailsDTO> cps = grpData.ToList();
                    if (cps != null)
                    {
                        objRoleRooms.PermissionList = cps;
                        objRoleRooms.RoomName = cps[0].RoomName;
                    }
                    if (objRoleRooms.RoomID > 0)
                    {
                        if (string.IsNullOrEmpty(RoomLists))
                            RoomLists = objRoleRooms.RoomID + "_" + objRoleRooms.RoomName;
                        else
                            RoomLists += "," + objRoleRooms.RoomID + "_" + objRoleRooms.RoomName;
                    }
                    objRooms.Add(objRoleRooms);
                }
            }

            return objRooms;
        }

        /// <summary>
        /// Get Particullar Record from the User Master by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public List<UserRoomReplanishmentDetailsDTO> GetUserRoomReplanishmentDetailsRecord(Int64 UserID)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                if (UserID > 0)
                {
                    List<UserRoomReplanishmentDetailsDTO> objRe = new List<UserRoomReplanishmentDetailsDTO>();

                    objRe = (from u in context.ExecuteStoreQuery<UserRoomReplanishmentDetailsDTO>(@"SELECT ID,RoleID,RoomId,UserID,isnull(IsRoomAccess,0) as IsRoomAccess FROM UserRoomsDetails where UserID='" + UserID + "' ")
                             select new UserRoomReplanishmentDetailsDTO
                             {
                                 ID = u.ID,
                                 RoleID = u.RoleID,
                                 RoomID = u.RoomID,
                                 UserID = u.UserID
                             }).ToList();
                    return objRe;
                }

            }
            return null;
        }

        /// <summary>
        /// Get Particullar Record from the User Master by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public List<UserRoleModuleDetailsDTO> GetUserRoleModuleDetailsRecord(Int64 UserID, Int64 RoleID, Int64 RoomID)
        {
            UserRoleModuleDetailsDTO objresult = new UserRoleModuleDetailsDTO();
            string qry = "";
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                if (RoleID > 0)
                {
                    List<UserRoleModuleDetailsDTO> objRe = new List<UserRoleModuleDetailsDTO>();
                    if (UserID > 0)
                        qry = @"SELECT M.ID as ModuleID,M.DisplayName as ModuleName,M.IsModule,M.GroupID,ISNULL(A.ID,0) as ID ,ISnull(A.RoleID,0) as RoleID ,ISNULL(A.RoomID,0) as RoomID,ISNULL(R.RoomName,'') as RoomName,ISNULL(A.IsInsert,0) as IsInsert,ISNULL(A.IsUpdate,0)  as IsUpdate,ISNULL(A.ISDelete,0) as ISDelete ,ISNULL(A.IsView,0) as IsView ,ISNULL(A.IsChecked,0) as IsChecked,ModuleValue,M.ParentID,M.Value as ModuleURL,M.ImageName as ImageName,ISNULL(A.ShowDeleted,0) as ShowDeleted,ISNULL(A.ShowArchived,0) as ShowArchived,ISNULL(A.ShowUDF,0) as ShowUDF FROM ModuleMaster M LEFT OUTER  JOIN UserRoleDetails A ON M.ID = A.ModuleID AND A.RoleId='" + RoleID + "' and A.UserID='" + UserID + "'   LEFT OUTER  JOIN Room R ON R.ID = A.RoomID  WHERE ISNULL(M.IsDeleted,0) = 0  ";
                    else
                        qry = @"SELECT M.ID as ModuleID,M.DisplayName as ModuleName,M.IsModule,M.GroupID,ISNULL(A.ID,0) as ID ,ISnull(A.RoleID,0) as RoleID ,ISNULL(A.RoomID,0) as RoomID,ISNULL(R.RoomName,'') as RoomName,ISNULL(A.IsInsert,0) as IsInsert,ISNULL(A.IsUpdate,0)  as IsUpdate,ISNULL(A.ISDelete,0) as ISDelete ,ISNULL(A.IsView,0) as IsView,ISNULL(A.IsChecked,0) as IsChecked,ModuleValue,M.ParentID,M.Value as ModuleURL,M.ImageName as ImageName,ISNULL(A.ShowDeleted,0) as ShowDeleted,ISNULL(A.ShowArchived,0) as ShowArchived,ISNULL(A.ShowUDF,0) as ShowUDF FROM ModuleMaster M LEFT OUTER  JOIN UserRoleDetails A ON M.ID = A.ModuleID  AND A.RoleId='" + RoleID + "' LEFT OUTER  JOIN Room R ON R.ID = A.RoomID  WHERE ISNULL(M.IsDeleted,0) = 0   ";

                    objRe = (from u in context.ExecuteStoreQuery<UserRoleModuleDetailsDTO>(qry)
                             select new UserRoleModuleDetailsDTO
                             {
                                 ID = u.ID,
                                 RoleID = u.RoleID,
                                 RoomId = u.RoomId,
                                 UserID = u.UserID,
                                 RoomName = u.RoomName,
                                 ModuleID = u.ModuleID,
                                 ModuleName = u.ModuleName,
                                 IsInsert = u.IsInsert,
                                 IsUpdate = u.IsUpdate,
                                 IsDelete = u.IsDelete,
                                 IsChecked = u.IsChecked,
                                 IsView = u.IsView,
                                 ShowDeleted = u.ShowDeleted,
                                 ShowArchived = u.ShowArchived,
                                 ShowUDF = u.ShowUDF,
                                 IsModule = u.IsModule,
                                 ModuleValue = u.ModuleValue,
                                 GroupId = u.GroupId,
                                 ParentID = u.ParentID,
                                 ModuleURL = u.ModuleURL,
                                 ImageName = u.ImageName
                             }).ToList();
                    return objRe;
                }
                else
                {
                    return (from u in context.ExecuteStoreQuery<UserRoleModuleDetailsDTO>(@"SELECT M.ID as ModuleID,M.DisplayName as ModuleName,M.IsModule,M.GroupID,ISNULL(A.ID,0) as ID ,ISnull(A.RoleID,0) as RoleID,ISNULL(A.RoomID,0) as RoomID,ISNULL(R.RoomName,'') as RoomName,ISNULL(A.IsInsert,0)  as IsInsert,ISNULL(A.IsUpdate,0) as IsUpdate ,ISNULL(A.ISDelete,0) as ISDelete,ISNULL(A.IsView,0) as IsView,ISNULL(A.IsChecked,0) as IsChecked,ModuleValue,M.ParentID,M.Value as ModuleURL,M.ImageName as ImageName,ISNULL(A.ShowDeleted,0) as ShowDeleted,ISNULL(A.ShowArchived,0) as ShowArchived,ISNULL(A.ShowUDF,0) as ShowUDF  FROM ModuleMaster M LEFT OUTER  JOIN UserRoleDetails A ON M.ID = A.ModuleID AND A.RoleId='0'  and A.UserID='0' LEFT OUTER  JOIN Room R ON R.ID = A.RoomID  WHERE ISNULL(M.IsDeleted,0) = 0  ")
                            select new UserRoleModuleDetailsDTO
                            {
                                ID = u.ID,
                                RoleID = u.RoleID,
                                RoomId = u.RoomId,
                                RoomName = u.RoomName,
                                ModuleID = u.ModuleID,
                                ModuleName = u.ModuleName,
                                IsInsert = u.IsInsert,
                                IsUpdate = u.IsUpdate,
                                IsDelete = u.IsDelete,
                                IsView = u.IsView,
                                ShowDeleted = u.ShowDeleted,
                                ShowArchived = u.ShowArchived,
                                ShowUDF = u.ShowUDF,
                                IsModule = u.IsModule,
                                ModuleValue = u.ModuleValue,
                                UserID = u.UserID,
                                GroupId = u.GroupId,
                                ParentID = u.ParentID,
                                ModuleURL = u.ModuleURL,
                                ImageName = u.ImageName
                            }).ToList();
                }

            }
        }

        /// <summary>
        /// Get Particullar Record from the Role Master by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public UserMasterDTO GetHistoryRecord(Int64 id)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<UserMasterDTO>(@"SELECT A.ID,A.HistoryID,A.Role, A.Created, A.CreatedBy,A.Room, A.LastUpdatedBy, A.Updated, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, A.GUID FROM RoleMaster_History A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.HistoryID=" + id.ToString())
                        select new UserMasterDTO
                        {

                            //    HistoryID = u.HistoryID,
                            ID = u.ID,
                            GUID = u.GUID,
                            UserName = u.UserName,
                            Password = u.Password,
                            //ConfirmPassword = u.Password,
                            Phone = u.Phone,
                            RoleID = u.RoleID,
                            Email = u.Email,
                            Room = u.Room,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            CompanyID = u.CompanyID,

                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            RoomName = u.RoomName,
                            LastUpdatedBy = u.LastUpdatedBy,
                            CreatedBy = u.CreatedBy

                        }).SingleOrDefault();
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
                UserMaster obj = context.UserMasters.Single(t => t.ID == id);
                obj.IsDeleted = true;
                obj.IsArchived = false;
                obj.Updated = DateTime.UtcNow;
                obj.LastUpdatedBy = userid;
                context.UserMasters.Attach(obj);
                context.ObjectStateManager.ChangeObjectState(obj, EntityState.Modified);
                context.SaveChanges();
                return true;
            }
        }

        public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyId)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE UserMaster SET Updated = '" + DateTime.UtcNow.ToString() + "' , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.ExecuteStoreCommand(strQuery);
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
                string strQuery = "UPDATE UserMaster SET " + columnName + " = '" + value + "', Updated = DATETIME ('" + System.DateTime.UtcNow.ToString(Convert.ToString((new System.Configuration.AppSettingsReader()).GetValue("FormatDateTimetoStore", typeof(System.String)))) + "') WHERE ID=" + id;
                context.ExecuteStoreCommand(strQuery);
            }
            return value;
        }

        /// <summary>
        /// Get Particullar Record from the User Master by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public UserMasterDTO CheckAuthantication(string Email, string Password)
        {

            UserMasterDTO objresult = new UserMasterDTO();
            if (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password))
            {
                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    objresult = (from u in context.ExecuteStoreQuery<UserMasterDTO>(@"select um.*,um.CompanyID,um.RoomId as Room,em.EnterpriseDBName FROM UserMaster as um left join EnterpriseMaster as em on um.EnterpriseId = em.ID WHERE um.IsDeleted!=1 AND um.IsArchived != 1 AND um.UserName='" + Email + "' AND um.Password ='" + Password + "'")
                                 select new UserMasterDTO
                                 {
                                     ID = u.ID,
                                     GUID = u.GUID,
                                     UserName = u.UserName,
                                     Password = u.Password,
                                     //ConfirmPassword = u.Password,
                                     Phone = u.Phone,
                                     RoleID = u.RoleID,
                                     Email = u.Email,
                                     Room = u.Room,
                                     IsDeleted = u.IsDeleted,
                                     IsArchived = u.IsArchived,
                                     CompanyID = u.CompanyID,
                                     EnterpriseDbName = u.EnterpriseDbName,
                                     Created = u.Created,
                                     Updated = u.Updated,
                                     CreatedByName = u.CreatedByName,
                                     UpdatedByName = u.UpdatedByName,
                                     RoomName = u.RoomName,
                                     LastUpdatedBy = u.LastUpdatedBy,
                                     CreatedBy = u.CreatedBy,
                                     UserType = u.UserType,
                                     EnterpriseId = u.EnterpriseId
                                 }).FirstOrDefault();
                }
            }
            return objresult;
        }

        public UserMasterDTO CheckAuthanticationUserName(string UserName, string Password)
        {
            UserMasterDTO objresult = new UserMasterDTO();
            EulaMasterDTO objEulaMaster = new EulaMasterDTO();
            if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
            {
                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    //objresult = (from u in context.ExecuteStoreQuery<UserMasterDTO>(@";With Userlist as
                    //                                           ( select		 um.*
                    //                                           ,um.RoomId as Room
                    //                                           ,em.EnterpriseDBName
                    //                                         FROM			  UserMaster as um 
                    //                                           left join EnterpriseMaster as em on um.EnterpriseId = em.ID  
                    //                                         WHERE		um.IsDeleted!=1 
                    //                                           AND um.IsArchived != 1 
                    //                                           AND um.UserName='" + UserName + "' AND um.Password ='" + Password + "') " +
                    //                                        " ,LicenceAccept as " +
                    //                                        " (select		LicenceAcceptDate,isnull(datediff(day,max(Ul.LicenceAcceptDate),getutcdate()),0) as DaysRemains  " +
                    //                                        " 			,UL.userid,max(Ul.LicenceAcceptDate) as LastLicenceAccept ,isnull(count(*),0) as Acceptcount" +
                    //                                        " from		UserMaster as um   " +
                    //                                        " 	left join dbo.UserLicenceAccept UL on UL.userid=um.id " +
                    //                                        " group by UL.userid ,Ul.LicenceAcceptDate" +
                    //                                        " " +
                    //                                        " ) " +
                    //                                        " select  top 1  UL.*,isnull(LA.DaysRemains,0) as DaysRemains,LA.LastLicenceAccept,isnull(La.Acceptcount,0) as Acceptcount from Userlist as UL " +
                    //                                        " left join LicenceAccept as LA  " +
                    //                                        " on ul.ID = LA.UserID  order by LA.LicenceAcceptDate desc ")
                    //             select new UserMasterDTO
                    //             {
                    //                 ID = u.ID,
                    //                 GUID = u.GUID,
                    //                 UserName = u.UserName,
                    //                 Password = u.Password,
                    //                 //ConfirmPassword = u.Password,
                    //                 Phone = u.Phone,
                    //                 RoleID = u.RoleID,
                    //                 Email = u.Email,
                    //                 Room = u.Room,
                    //                 IsDeleted = u.IsDeleted,
                    //                 IsArchived = u.IsArchived,
                    //                 CompanyID = u.CompanyID,
                    //                 EnterpriseDbName = u.EnterpriseDbName,
                    //                 Created = u.Created,
                    //                 Updated = u.Updated,
                    //                 CreatedByName = u.CreatedByName,
                    //                 UpdatedByName = u.UpdatedByName,
                    //                 RoomName = u.RoomName,
                    //                 LastUpdatedBy = u.LastUpdatedBy,
                    //                 CreatedBy = u.CreatedBy,
                    //                 UserType = u.UserType,
                    //                 EnterpriseId = u.EnterpriseId,
                    //                 IsLicenceAccepted = u.IsLicenceAccepted,
                    //                 IsLocked = u.IsLocked,
                    //                 LastLicenceAccept = u.LastLicenceAccept,
                    //                 DaysRemains = u.DaysRemains,
                    //                 TotalRecords = 1,
                    //             }).FirstOrDefault();

                    objresult = AuthenticateUserWithUserNamePass(UserName, Password);

                    objEulaMaster = (from u in context.ExecuteStoreQuery<EulaMasterDTO>(@"select top 1 Created as CreatedOn,*
                                                                        from Eulamaster EM
                                                                        where isnull(IsDeleted,0) != 1
                                                                        Order by Id desc")
                                     select new EulaMasterDTO
                                     {
                                         CreatedOn = u.CreatedOn,
                                         EulaFileName = u.EulaFileName
                                     }).FirstOrDefault();

                    if (objEulaMaster != null && objresult != null)
                    {
                        if (objresult.RoleID != -1)
                        {
                            if (objresult.LastLicenceAccept == null || objresult.LastLicenceAccept < objEulaMaster.CreatedOn)
                            {
                                objresult.NewEulaAccept = false;
                            }
                            else
                            {
                                objresult.NewEulaAccept = true;
                            }
                        }
                        else
                        {
                            objresult.NewEulaAccept = true;
                        }
                    }
                    else
                    {
                        if (objresult != null)
                        {
                            objresult.NewEulaAccept = true;
                        }
                    }
                    if (objresult != null)
                    {
                        objresult.HasChangedFirstPassword = context.UserPasswordChangeHistories.Any(t => t.UserId == objresult.ID);
                    }

                }

            }
            return objresult;
        }

        public string GetJwtToken(long UserId)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.ExecuteStoreQuery<string>(@"select BearerToken from UserWebApiToken UA 
                                                            INNER JOIN UserMaster UM ON UA.Guid = UM.GUID where UM.ID = " + UserId + @" and UA.IsActive = 1
                                                            ").FirstOrDefault();
            }
        }

        public UserMasterDTO CheckAuthanticationUserNameOnly(string UserName)
        {
            UserMasterDTO objresult = new UserMasterDTO();
            EulaMasterDTO objEulaMaster = new EulaMasterDTO();
            if (!string.IsNullOrEmpty(UserName))
            {
                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    //objresult = (from u in context.ExecuteStoreQuery<UserMasterDTO>(@";With Userlist as
                    //                                           ( select		 um.*
                    //                                           ,um.RoomId as Room
                    //                                           ,em.EnterpriseDBName
                    //                                         FROM			  UserMaster as um 
                    //                                           left join EnterpriseMaster as em on um.EnterpriseId = em.ID  
                    //                                         WHERE		um.IsDeleted!=1 
                    //                                           AND um.IsArchived != 1 
                    //                                           AND um.UserName='" + UserName + "') " +
                    //                                        " ,LicenceAccept as " +
                    //                                        " (select		LicenceAcceptDate,isnull(datediff(day,max(Ul.LicenceAcceptDate),getutcdate()),0) as DaysRemains  " +
                    //                                        " 			,UL.userid,max(Ul.LicenceAcceptDate) as LastLicenceAccept ,isnull(count(*),0) as Acceptcount" +
                    //                                        " from		UserMaster as um   " +
                    //                                        " 	left join dbo.UserLicenceAccept UL on UL.userid=um.id " +
                    //                                        " group by UL.userid ,Ul.LicenceAcceptDate" +
                    //                                        " " +
                    //                                        " ) " +
                    //                                        " select  top 1  UL.*,isnull(LA.DaysRemains,0) as DaysRemains,LA.LastLicenceAccept,isnull(La.Acceptcount,0) as Acceptcount from Userlist as UL " +
                    //                                        " left join LicenceAccept as LA  " +
                    //                                        " on ul.ID = LA.UserID  order by LA.LicenceAcceptDate desc ")
                    //             select new UserMasterDTO
                    //             {
                    //                 ID = u.ID,
                    //                 GUID = u.GUID,
                    //                 UserName = u.UserName,
                    //                 Password = u.Password,
                    //                 //ConfirmPassword = u.Password,
                    //                 Phone = u.Phone,
                    //                 RoleID = u.RoleID,
                    //                 Email = u.Email,
                    //                 Room = u.Room,
                    //                 IsDeleted = u.IsDeleted,
                    //                 IsArchived = u.IsArchived,
                    //                 CompanyID = u.CompanyID,
                    //                 EnterpriseDbName = u.EnterpriseDbName,
                    //                 Created = u.Created,
                    //                 Updated = u.Updated,
                    //                 CreatedByName = u.CreatedByName,
                    //                 UpdatedByName = u.UpdatedByName,
                    //                 RoomName = u.RoomName,
                    //                 LastUpdatedBy = u.LastUpdatedBy,
                    //                 CreatedBy = u.CreatedBy,
                    //                 UserType = u.UserType,
                    //                 EnterpriseId = u.EnterpriseId,
                    //                 IsLicenceAccepted = u.IsLicenceAccepted,
                    //                 IsLocked = u.IsLocked,
                    //                 LastLicenceAccept = u.LastLicenceAccept,
                    //                 DaysRemains = u.DaysRemains,
                    //                 TotalRecords = 1,
                    //             }).FirstOrDefault();
                    objresult = AuthenticateUserWithUserNameOnly(UserName);
                    objEulaMaster = (from u in context.ExecuteStoreQuery<EulaMasterDTO>(@"select top 1 Created as CreatedOn,*
                                                                        from Eulamaster EM
                                                                        where isnull(IsDeleted,0) != 1
                                                                        Order by Id desc")
                                     select new EulaMasterDTO
                                     {
                                         CreatedOn = u.CreatedOn,
                                         EulaFileName = u.EulaFileName
                                     }).FirstOrDefault();

                    if (objEulaMaster != null && objresult != null)
                    {
                        if (objresult.RoleID != -1)
                        {
                            if (objresult.LastLicenceAccept == null || objresult.LastLicenceAccept < objEulaMaster.CreatedOn)
                            {
                                objresult.NewEulaAccept = false;
                            }
                            else
                            {
                                objresult.NewEulaAccept = true;
                            }
                        }
                        else
                        {
                            objresult.NewEulaAccept = true;
                        }
                    }
                    else
                    {
                        if (objresult != null)
                        {
                            objresult.NewEulaAccept = true;
                        }
                    }
                    if (objresult != null)
                    {
                        objresult.HasChangedFirstPassword = context.UserPasswordChangeHistories.Any(t => t.UserId == objresult.ID);
                    }

                }

            }
            return objresult;
        }

        /// <summary>
        /// Get Particullar Record from the User Master by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public UserMasterDTO GetRecordID(Int64 id)
        {
            UserMasterDTO objresult = new UserMasterDTO();

            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                objresult = (from u in context.ExecuteStoreQuery<UserMasterDTO>(@"select um.*,um.CompanyID,um.RoomId as Room,em.EnterpriseDBName FROM UserMaster as um left join EnterpriseMaster as em on um.EnterpriseId = em.ID WHERE um.IsDeleted!=1 AND um.IsArchived != 1 AND um.ID=" + id)
                             select new UserMasterDTO
                             {
                                 ID = u.ID,
                                 GUID = u.GUID,
                                 UserName = u.UserName,
                                 Password = u.Password,
                                 //ConfirmPassword = u.Password,
                                 Phone = u.Phone,
                                 RoleID = u.RoleID,
                                 Email = u.Email,
                                 Room = u.Room,
                                 IsDeleted = u.IsDeleted,
                                 IsArchived = u.IsArchived,
                                 CompanyID = u.CompanyID,
                                 EnterpriseDbName = u.EnterpriseDbName,
                                 Created = u.Created,
                                 Updated = u.Updated,
                                 CreatedByName = u.CreatedByName,
                                 UpdatedByName = u.UpdatedByName,
                                 RoomName = u.RoomName,
                                 LastUpdatedBy = u.LastUpdatedBy,
                                 CreatedBy = u.CreatedBy,
                                 UserType = u.UserType,
                                 EnterpriseId = u.EnterpriseId
                             }).FirstOrDefault();
            }

            return objresult;
        }

        public UserMasterDTO GetRecordByRoleID(Int64 RoleID, Int64 EnterpriseId)
        {
            UserMasterDTO objresult = new UserMasterDTO();
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                objresult = (from u in context.ExecuteStoreQuery<UserMasterDTO>(@"select um.*,um.CompanyID,um.RoomId as Room,em.EnterpriseDBName FROM UserMaster as um left join EnterpriseMaster as em on um.EnterpriseId = em.ID WHERE um.IsDeleted!=1 AND um.IsArchived != 1 AND um.RoleId=" + RoleID + " And EnterpriseId=" + EnterpriseId)
                             select new UserMasterDTO
                             {
                                 ID = u.ID,
                                 GUID = u.GUID,
                                 UserName = u.UserName,
                                 Password = u.Password,
                                 //ConfirmPassword = u.Password,
                                 Phone = u.Phone,
                                 RoleID = u.RoleID,
                                 Email = u.Email,
                                 Room = u.Room,
                                 IsDeleted = u.IsDeleted,
                                 IsArchived = u.IsArchived,
                                 CompanyID = u.CompanyID,
                                 EnterpriseDbName = u.EnterpriseDbName,
                                 Created = u.Created,
                                 Updated = u.Updated,
                                 CreatedByName = u.CreatedByName,
                                 UpdatedByName = u.UpdatedByName,
                                 RoomName = u.RoomName,
                                 LastUpdatedBy = u.LastUpdatedBy,
                                 CreatedBy = u.CreatedBy,
                                 UserType = u.UserType,
                                 EnterpriseId = u.EnterpriseId
                             }).FirstOrDefault();
            }
            return objresult;
        }

        public List<UserMasterDTO> GetModuleWiseUsers(Int64 ModuleID, Int64 RoomID, Int64 CompanyId)
        {

            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<UserMasterDTO>(@"select UM.* from UserRoleDetails URD 
                                                                                            Inner join UserMaster UM ON URD.UserID = UM.ID
                                                                                where URD.ModuleID=" + ModuleID + @" and URD.RoomId=" + RoomID + @" and UM.CompanyID = " + CompanyId)
                        select new UserMasterDTO
                        {
                            ID = u.ID,
                            GUID = u.GUID,
                            UserName = u.UserName,
                            Password = u.Password,
                            Phone = u.Phone,
                            RoleID = u.RoleID,
                            Email = u.Email,
                            Room = u.Room,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            CompanyID = u.CompanyID,

                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            RoomName = u.RoomName,
                            LastUpdatedBy = u.LastUpdatedBy,
                            CreatedBy = u.CreatedBy
                        }).ToList();
            }
        }

        public UserMasterDTO GetRecordBYUserName(string UserName)
        {
            UserMasterDTO objresult = new UserMasterDTO();
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                objresult = (from u in context.ExecuteStoreQuery<UserMasterDTO>(@"select um.*,um.CompanyID,um.RoomId as Room,em.EnterpriseDBName,isnull(em.EnterPriseDomainURL,'') as EnterPriseDomainURL FROM UserMaster as um left join EnterpriseMaster as em on um.EnterpriseId = em.ID WHERE um.IsDeleted!=1 AND um.IsArchived != 1 AND um.UserName='" + UserName + "'")
                             select new UserMasterDTO
                             {
                                 ID = u.ID,
                                 GUID = u.GUID,
                                 UserName = u.UserName,
                                 Password = u.Password,
                                 //ConfirmPassword = u.Password,
                                 Phone = u.Phone,
                                 RoleID = u.RoleID,
                                 Email = u.Email,
                                 Room = u.Room,
                                 IsDeleted = u.IsDeleted,
                                 IsArchived = u.IsArchived,
                                 CompanyID = u.CompanyID,

                                 Created = u.Created,
                                 Updated = u.Updated,
                                 CreatedByName = u.CreatedByName,
                                 UpdatedByName = u.UpdatedByName,
                                 RoomName = u.RoomName,
                                 LastUpdatedBy = u.LastUpdatedBy,
                                 CreatedBy = u.CreatedBy,
                                 EnterpriseId = u.EnterpriseId,
                                 UserType = u.UserType,
                                 EnterpriseDbName = u.EnterpriseDbName,
                                 EnterPriseDomainURL = u.EnterPriseDomainURL
                             }).SingleOrDefault();


            }
            return objresult;
        }
    }
}
