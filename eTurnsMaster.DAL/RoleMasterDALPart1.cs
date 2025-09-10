using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eTurns.DTO;
using System.Data;
using Dynamite;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using eTurnsMaster.DAL;
using System.Data.Objects;
using System.Dynamic;
using System.Configuration;
using System.Data.SqlClient;
using System.Xml.Linq;
using System.Transactions;
using System.Threading.Tasks;

namespace eTurnsMaster.DAL
{
    public partial class RoleMasterDAL : eTurnsMasterBaseDAL, IDisposable
    {
        public IEnumerable<RoleMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId)
        {
            //Get Cached-Media
            IEnumerable<RoleMasterDTO> ObjCache = CacheHelper<IEnumerable<RoleMasterDTO>>.GetCacheItem("Cached_RoleMaster_" + CompanyId.ToString());
            if (ObjCache == null)
            {
                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<RoleMasterDTO> obj = (from u in context.ExecuteStoreQuery<RoleMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM RoleMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.CompanyID = " + CompanyId.ToString())
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
                                                          RoomName = u.RoomName
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
                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<RoleMasterDTO> obj = (from u in context.ExecuteStoreQuery<RoleMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM RoleMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.CompanyID = " + CompanyID.ToString() + @" AND A.Room = " + RoomID.ToString() + @" AND A.IsArchived != 1")
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
                                                          RoomName = u.RoomName
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
                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    TotalCount = Convert.ToInt32(context.ExecuteStoreQuery<Int32>(@"SELECT Count(A.ID) FROM RoleMaster as A WHERE A.IsDeleted!=1 AND A.CompanyID = " + CompanyID.ToString() + @" AND A.Room = " + RoomID.ToString() + @" AND A.IsArchived != 1 " + search + "").ToList()[0]);

                    IEnumerable<RoleMasterDTO> obj = (from u in context.ExecuteStoreQuery<RoleMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM RoleMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.CompanyID = " + CompanyID.ToString() + @" AND A.Room = " + RoomID.ToString() + @" AND A.IsArchived != 1 " + search + " ORDER BY " + strSortinitializer)
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
                                                          RoomName = u.RoomName
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
                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    TotalCount = Convert.ToInt32(context.ExecuteStoreQuery<Int32>(@"SELECT Count(A.ID) FROM RoleMaster as A WHERE A.IsArchived != 1").ToList()[0]);

                    IEnumerable<RoleMasterDTO> obj = (from u in context.ExecuteStoreQuery<RoleMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, A.IsDeleted,A.IsArchived FROM RoleMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsArchived != 1  ORDER BY " + strSortinitializer)
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
                                                          RoomName = u.RoomName
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
                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    TotalCount = Convert.ToInt32(context.ExecuteStoreQuery<Int32>(@"SELECT Count(A.ID) FROM RoleMaster as A WHERE A.IsDeleted != 1").ToList()[0]);

                    IEnumerable<RoleMasterDTO> obj = (from u in context.ExecuteStoreQuery<RoleMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, A.IsDeleted,A.IsArchived FROM RoleMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted != 1  ORDER BY " + strSortinitializer)
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
                                                          RoomName = u.RoomName
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
                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    TotalCount = Convert.ToInt32(context.ExecuteStoreQuery<Int32>(@"SELECT Count(A.ID) FROM RoleMaster as A").ToList()[0]);

                    IEnumerable<RoleMasterDTO> obj = (from u in context.ExecuteStoreQuery<RoleMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, A.IsDeleted,A.IsArchived FROM RoleMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID ORDER BY " + strSortinitializer)
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
                                                          RoomName = u.RoomName
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

                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    TotalCount = Convert.ToInt32(context.ExecuteStoreQuery<Int32>(@"SELECT Count(ID) FROM RoleMaster WHERE IsDeleted!=1 AND CompanyID = " + CompanyID.ToString() + @" AND Room = " + RoomID.ToString() + @" AND IsArchived != 1 AND ((RoleName like '%" + SearchTerm + "%') OR (ID like '%" + SearchTerm + "%'))").ToList()[0]);
                    IEnumerable<RoleMasterDTO> obj = (from u in context.ExecuteStoreQuery<RoleMasterDTO>("SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM RoleMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.CompanyID = " + CompanyID.ToString() + @" AND A.Room = " + RoomID.ToString() + @" AND A.IsArchived != 1 AND ((A.RoleName like '%" + SearchTerm + "%') OR (A.ID like '%" + SearchTerm + "%')) ORDER BY " + strSortinitializer)
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
                                                          RoomName = u.RoomName
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

        private List<RolePermissionInfo> GetAllRooms(List<RolePermissionInfo> lstRooms)
        {
            List<RolePermissionInfo> lstreturn = new List<RolePermissionInfo>();
            DataTable DTECR = GetECRTable(lstRooms);
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                //string Connectionstring = DbConHelper.GetMasterDBConnectionString();
                //SqlConnection ChildDbConnection = new SqlConnection(Connectionstring);
                //DataSet Ds = SqlHelper.ExecuteDataset(ChildDbConnection, "GetALLRoomsForPermission", DTECR);

                var parameter = new SqlParameter("@tblECR", SqlDbType.Structured);
                parameter.Value = DTECR;
                parameter.TypeName = "dbo.ECRTable";
                lstreturn = context.ExecuteStoreQuery<RolePermissionInfo>("exec dbo.[GetALLRoomsForPermission] @tblECR", parameter).ToList();
            }
            return lstreturn;
        }

        private List<RoleWiseRoomsAccessDetailsDTO> ConvertPermissions1(List<RoleModuleDetailsDTO> objData, Int64 RoleID, ref string RoomLists)
        {
            List<RoleWiseRoomsAccessDetailsDTO> objRooms = new List<RoleWiseRoomsAccessDetailsDTO>();

            RoleWiseRoomsAccessDetailsDTO objRoleRooms;

            var objTempPermissionList = objData.GroupBy(element => new { element.RoomId, element.CompanyID, element.EnterpriseID })
                                            .OrderBy(g => g.Key.RoomId);

            foreach (var grpData in objTempPermissionList)
            {
                objRoleRooms = new RoleWiseRoomsAccessDetailsDTO();
                objRoleRooms.RoomID = grpData.Key.RoomId;
                objRoleRooms.RoleID = RoleID;
                objRoleRooms.EnterpriseID = grpData.Key.EnterpriseID;
                objRoleRooms.CompanyID = grpData.Key.CompanyID;
                List<RoleModuleDetailsDTO> cps = grpData.ToList();
                if (cps != null)
                {
                    objRoleRooms.PermissionList = cps;
                    objRoleRooms.RoomName = cps[0].RoomName;
                }
                if (objRoleRooms.RoomID > 0)
                {
                    if (string.IsNullOrEmpty(RoomLists))
                    {
                        RoomDTO objRoomDTO = new RoomDTO();
                        EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
                        RolePermissionInfo objRolePermissionInfo = new RolePermissionInfo();
                        objRolePermissionInfo.EnterPriseId = objRoleRooms.EnterpriseID;
                        objRolePermissionInfo.CompanyId = objRoleRooms.CompanyID;
                        objRolePermissionInfo.RoomId = objRoleRooms.RoomID;
                        objRoomDTO = objEnterpriseMasterDAL.GetRoomById(objRolePermissionInfo);
                        objRoleRooms.RoomName = objRoomDTO.RoomName;
                        objRoleRooms.CompanyName = objRoomDTO.CompanyName;
                        objRoleRooms.EnterPriseName = objRoomDTO.EnterpriseName;
                        if (objRoomDTO != null)
                        {
                            RoomLists = objRoleRooms.EnterpriseID + "_" + objRoleRooms.CompanyID + "_" + objRoleRooms.RoomID + "_" + objRoomDTO.RoomName;
                        }
                        else
                        {
                            RoomLists = objRoleRooms.EnterpriseID + "_" + objRoleRooms.CompanyID + "_" + objRoleRooms.RoomID + "_" + "Room" + objRoleRooms.RoomID;
                        }
                    }
                    else
                    {
                        RoomDTO objRoomDTO = new RoomDTO();
                        EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
                        RolePermissionInfo objRolePermissionInfo = new RolePermissionInfo();
                        objRolePermissionInfo.EnterPriseId = objRoleRooms.EnterpriseID;
                        objRolePermissionInfo.CompanyId = objRoleRooms.CompanyID;
                        objRolePermissionInfo.RoomId = objRoleRooms.RoomID;
                        objRoomDTO = objEnterpriseMasterDAL.GetRoomById(objRolePermissionInfo);
                        objRoleRooms.RoomName = objRoomDTO.RoomName;
                        objRoleRooms.CompanyName = objRoomDTO.CompanyName;
                        objRoleRooms.EnterPriseName = objRoomDTO.EnterpriseName;
                        if (objRoomDTO != null)
                        {
                            RoomLists += "," + objRoleRooms.EnterpriseID + "_" + objRoleRooms.CompanyID + "_" + objRoleRooms.RoomID + "_" + objRoomDTO.RoomName;
                        }
                        else
                        {
                            RoomLists += "," + objRoleRooms.EnterpriseID + "_" + objRoleRooms.CompanyID + "_" + objRoleRooms.RoomID + "_" + "Room" + objRoleRooms.RoomID;
                        }

                    }
                }
                objRooms.Add(objRoleRooms);
            }

            return objRooms;
        }

        public IEnumerable<RoleMasterDTO> GetPagedRecords(int UserType, Int64 EnterpriseId, Int64 RoomID, Int64 CompanyID, Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, bool IsArchived, bool IsDeleted)
        {
            IEnumerable<RoleMasterDTO> ObjCache = GetAllRoles(UserType, EnterpriseId, CompanyID, RoomID, IsArchived, IsDeleted).AsEnumerable<RoleMasterDTO>();
            //Get Cached-Media
            //IEnumerable<RoleMasterDTO> ObjCache = GetAllRoles(UserType, EnterpriseId, CompanyID, RoomID).AsEnumerable<RoleMasterDTO>();
            //IEnumerable<RoleMasterDTO> ObjGlobalCache = ObjCache;
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
            //string RoleCreaters = null;
            //string RoleUpdators = null;
            //string CreatedDateFrom = null;
            //string CreatedDateTo = null;
            //string UpdatedDateFrom = null;
            //string UpdatedDateTo = null;
            //string UserTypes = string.Empty;

            if (String.IsNullOrEmpty(SearchTerm))
            {

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media
                //IEnumerable<CartItemDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
                //SearchTerm = string.Empty;


                //if (!string.IsNullOrWhiteSpace(FieldsPara[36]))
                //{
                //    UserTypes = Convert.ToString(FieldsPara[36]).TrimEnd();
                //}

                ObjCache = ObjCache.Where(t =>
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedBy.ToString())))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.LastUpdatedBy.ToString())))
                    && ((Fields[1].Split('@')[36] == "") || (Fields[1].Split('@')[36].Split(',').ToList().Contains(t.UserType.ToString())))
                    && ((Fields[1].Split('@')[2] == "") || (t.Created.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[3] == "") || (t.Updated.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.Updated.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date)));

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                //Get Cached-Media
                //IEnumerable<CartItemDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                ObjCache = ObjCache.Where(
                     t => t.RoleName.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UserTypeName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                        );

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
        }

        //public dynamic ValidateBeforeDeleteRoles(List<RoleMasterDTO> lstRoles)
        //{
        //    using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
        //    {
        //        int InUse = 0;
        //        int CanBeDeleted = 0;
        //        foreach (RoleMasterDTO Roleitem in lstRoles)
        //        {
        //            if (Roleitem.UserType == 1)
        //            {
        //                if (context.UserMasters.Any(t => t.RoleId == Roleitem.ID && t.UserType == Roleitem.UserType))
        //                {
        //                    InUse += 1;
        //                }
        //                else
        //                {
        //                    CanBeDeleted += 1;
        //                }
        //            }
        //            else
        //            {
        //                if (context.UserMasters.Any(t => t.RoleId == Roleitem.ID && t.UserType == Roleitem.UserType && t.EnterpriseId == Roleitem.EnterpriseId))
        //                {
        //                    InUse += 1;
        //                }
        //                else
        //                {
        //                    CanBeDeleted += 1;
        //                }
        //            }

        //        }
        //        dynamic objret = new ExpandoObject();
        //        objret.InUse = InUse;
        //        objret.ToBeDeleted = CanBeDeleted;
        //        return objret;
        //    }
        //}

        public bool UserHasEnterpriseAccess(long UserId, long EnterPriseId)
        {
            bool retVal = false;
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                retVal = context.UserRoomAccesses.Any(t => t.UserId == UserId && t.EnterpriseId == EnterPriseId);
            }
            return retVal;
        }

        public bool UserHasCompanyAccess(long UserId, long EnterPriseId, long CompanyId)
        {
            bool retVal = false;
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                retVal = context.UserRoomAccesses.Any(t => t.UserId == UserId && t.EnterpriseId == EnterPriseId && t.CompanyId == CompanyId);
            }
            return retVal;

        }

        public bool UserHasRoomAccess(long UserId, long EnterPriseId, long CompanyId, long RoomId)
        {
            bool retVal = false;
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                retVal = context.UserRoomAccesses.Any(t => t.UserId == UserId && t.EnterpriseId == EnterPriseId && t.CompanyId == CompanyId && t.RoomId == RoomId);
            }
            return retVal;

        }

        public RoleMasterDTO GetHistoryRecord(Int64 id)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
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

        public bool DeleteRecords(string IDs, int userid)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE RoleMaster SET Updated = '" + DateTime.UtcNow.ToString() + "' , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.ExecuteStoreCommand(strQuery);
                return true;
            }
        }
        public string UpdateData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "UPDATE RoleMaster SET " + columnName + " = '" + value + "', Updated = DATETIME ('" + System.DateTime.UtcNow.ToString(Convert.ToString((new System.Configuration.AppSettingsReader()).GetValue("FormatDateTimetoStore", typeof(System.String)))) + "') WHERE ID=" + id;
                context.ExecuteStoreCommand(strQuery);
            }
            return value;
        }

        public List<RoleMasterDTO> GetAllRoles(int UserType, long? EnterpriseId, long? CompanyId, long? RoomId, bool IsArchived, bool IsDeleted)
        {
            List<RoleMasterDTO> lstRoles = GetAllRoles(UserType, EnterpriseId, CompanyId, RoomId);
            if (IsArchived == false && IsDeleted == false)
            {
                return lstRoles.Where(t => t.IsDeleted == false && t.IsArchived == false).ToList();
            }
            else
            {

                if (IsArchived && IsDeleted)
                {
                    return lstRoles.Where(t => t.IsDeleted == true && t.IsArchived == true).ToList();
                }
                else if (IsArchived)
                {
                    return lstRoles.Where(t => t.IsArchived == true).ToList();
                }
                else if (IsDeleted)
                {
                    return lstRoles.Where(t => t.IsDeleted == true).ToList();
                }
                else
                {
                    return lstRoles;
                }
            }
        }

        public List<RoleMasterDTO> GetAllRoles(int UserType, long? EnterpriseId, long? CompanyId, long? RoomId)
        {
            List<RoleMasterDTO> lstAllRoles = new List<RoleMasterDTO>();
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                if (UserType == 1)
                {
                    lstAllRoles = (from rls in context.RoleMasters
                                   join usrmst in context.UserMasters on rls.CreatedBy equals usrmst.ID into rls_usrmst_join
                                   from rls_usrmst in rls_usrmst_join.DefaultIfEmpty()
                                   join usrmstup in context.UserMasters on rls.LastUpdatedBy equals usrmstup.ID into rls_usrmstup_join
                                   from rls_usrmstup in rls_usrmstup_join.DefaultIfEmpty()
                                   join utm in context.UserTypeMasters on rls.UserType equals utm.UserTypeID into rls_utm_join
                                   from rls_utm in rls_utm_join.DefaultIfEmpty()
                                   select new RoleMasterDTO
                                   {
                                       CompanyID = 0,
                                       Room = 0,
                                       RoleName = rls.RoleName,
                                       CreatedBy = rls.CreatedBy,
                                       CreatedByName = rls_usrmst.UserName,
                                       Description = rls.Description,
                                       EnterpriseId = 0,
                                       GUID = rls.GUID ?? Guid.Empty,
                                       ID = rls.ID,
                                       IsActive = rls.IsActive ?? false,
                                       IsArchived = rls.IsArchived ?? false,
                                       IsDeleted = rls.IsDeleted ?? false,
                                       LastUpdatedBy = rls.LastUpdatedBy,
                                       Updated = rls.Updated,
                                       UpdatedByName = rls_usrmstup.UserName,
                                       UserType = rls.UserType ?? 0,
                                       UserTypeName = rls_utm.UserTypeName
                                   }).ToList();

                    if (EnterpriseId.HasValue && EnterpriseId.Value > 0)
                    {
                        EnterpriseDTO objEnterpriseDTO = new EnterpriseDTO();
                        EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
                        objEnterpriseDTO = objEnterpriseMasterDAL.GetEnterpriseByIdPlain(EnterpriseId.Value);
                        if (objEnterpriseDTO != null && !string.IsNullOrWhiteSpace(objEnterpriseDTO.EnterpriseDBName))
                        {
                            DataSet dsRoles = new DataSet();
                            //string EnterpriseDbConnection = objEnterpriseMasterDAL.GetEnterpriseConnectionstring(objEnterpriseDTO.EnterpriseDBName);
                            string EnterpriseDbConnection = MasterDbConnectionHelper.GeteTurnsSQLConnectionString(objEnterpriseDTO.EnterpriseDBName, MasterDbConnectionType.GeneralReadWrite.ToString("F"));
                            string Qry = @"Select 
	                                    rm.ID
	                                    ,RM.RoleName
	                                    ,RM.Description
	                                    ,rm.IsActive
	                                    ,RM.Created
	                                    ,RM.Updated 
	                                    ,RM.CreatedBy
	                                    ,RM.LastUpdatedBy
	                                    ,RM.IsDeleted
	                                    ,RM.IsArchived
	                                    ,RM.Room
	                                    ,RM.CompanyID
	                                    ,RM.GUID
	                                    ,RM.UDF1
	                                    ,RM.UDF2
	                                    ,RM.UDF3
	                                    ,RM.UDF4
	                                    ,RM.UDF5
	                                    ,RM.UDF6
	                                    ,RM.UDF7
	                                    ,RM.UDF8
	                                    ,RM.UDF9
	                                    ,RM.UDF10
	                                    ,RM.EnterpriseId
	                                    ,RM.UserType
	                                    ,rtm.UserTypeName
	                                    ,ISNULL(em.Name,'') as Enterprisename
	                                    ,ISNULL(cm.Name,'') as CompanyName
                                        ,ISNULL(rmm.RoomName,'') as RoomName
	                                    ,ISNULL(um.UserName,'') as CreatedByName
	                                    ,ISNULL(umup.UserName,'') as LastUpdatedByName
	                                    FROM RoleMaster as RM 
                                        LEFT JOIN Enterprise as em on RM.EnterpriseId = em.ID
                                        left JOIN Room as rmm on RM.Room = rmm.ID
                                        left JOIN CompanyMaster as cm on RM.CompanyID = cm.ID
                                        left JOIN UserMaster as um ON RM.CreatedBy = um.ID
                                        left JOIN UserMaster as umup ON RM.LastUpdatedBy = umup.ID
                                        left JOIN UserTypeMaster as rtm ON RM.UserType = rtm.UserTypeID";


                            dsRoles = SqlHelper.ExecuteDataset(EnterpriseDbConnection, CommandType.Text, Qry);
                            if (dsRoles != null && dsRoles.Tables.Count > 0)
                            {
                                DataTable dtRoles = dsRoles.Tables[0];
                                if (dtRoles.Rows.Count > 0)
                                {
                                    foreach (DataRow dr in dtRoles.Rows)
                                    {
                                        RoleMasterDTO objRoleMasterDTO = new RoleMasterDTO();
                                        if (dtRoles.Columns.Contains("ID"))
                                        {
                                            long templong = 0;
                                            long.TryParse(Convert.ToString(dr["ID"]), out templong);
                                            objRoleMasterDTO.ID = templong;
                                        }
                                        if (dtRoles.Columns.Contains("RoleName"))
                                        {
                                            objRoleMasterDTO.RoleName = Convert.ToString(dr["RoleName"]);
                                        }
                                        if (dtRoles.Columns.Contains("Description"))
                                        {
                                            objRoleMasterDTO.Description = Convert.ToString(dr["Description"]);
                                        }
                                        if (dtRoles.Columns.Contains("IsActive"))
                                        {
                                            bool tempboolvar = false;
                                            bool.TryParse(Convert.ToString(dr["IsActive"]), out tempboolvar);
                                            objRoleMasterDTO.IsActive = tempboolvar;
                                        }
                                        if (dtRoles.Columns.Contains("Created"))
                                        {
                                            DateTime tempvar = DateTime.MinValue;
                                            DateTime.TryParse(Convert.ToString(dr["Created"]), out tempvar);
                                            objRoleMasterDTO.Created = tempvar;
                                        }
                                        if (dtRoles.Columns.Contains("Updated"))
                                        {
                                            DateTime tempvar = DateTime.MinValue;
                                            DateTime.TryParse(Convert.ToString(dr["Updated"]), out tempvar);
                                            objRoleMasterDTO.Updated = tempvar;
                                        }
                                        if (dtRoles.Columns.Contains("CreatedBy"))
                                        {
                                            long templong = 0;
                                            long.TryParse(Convert.ToString(dr["CreatedBy"]), out templong);
                                            objRoleMasterDTO.CreatedBy = templong;
                                        }
                                        if (dtRoles.Columns.Contains("LastUpdatedBy"))
                                        {
                                            long templong = 0;
                                            long.TryParse(Convert.ToString(dr["LastUpdatedBy"]), out templong);
                                            objRoleMasterDTO.LastUpdatedBy = templong;
                                        }
                                        if (dtRoles.Columns.Contains("IsDeleted"))
                                        {
                                            bool tempboolvar = false;
                                            bool.TryParse(Convert.ToString(dr["IsDeleted"]), out tempboolvar);
                                            objRoleMasterDTO.IsDeleted = tempboolvar;
                                        }
                                        if (dtRoles.Columns.Contains("IsArchived"))
                                        {
                                            bool tempboolvar = false;
                                            bool.TryParse(Convert.ToString(dr["IsArchived"]), out tempboolvar);
                                            objRoleMasterDTO.IsArchived = tempboolvar;
                                        }
                                        if (dtRoles.Columns.Contains("IsArchived"))
                                        {
                                            bool tempboolvar = false;
                                            bool.TryParse(Convert.ToString(dr["IsArchived"]), out tempboolvar);
                                            objRoleMasterDTO.IsArchived = tempboolvar;
                                        }
                                        if (dtRoles.Columns.Contains("Room"))
                                        {
                                            long templong = 0;
                                            long.TryParse(Convert.ToString(dr["Room"]), out templong);
                                            objRoleMasterDTO.Room = templong;
                                        }
                                        if (dtRoles.Columns.Contains("CompanyID"))
                                        {
                                            long templong = 0;
                                            long.TryParse(Convert.ToString(dr["CompanyID"]), out templong);
                                            objRoleMasterDTO.CompanyID = templong;
                                        }
                                        if (dtRoles.Columns.Contains("GUID"))
                                        {
                                            Guid templong = Guid.Empty;
                                            Guid.TryParse(Convert.ToString(dr["GUID"]), out templong);
                                            objRoleMasterDTO.GUID = templong;
                                        }
                                        if (dtRoles.Columns.Contains("EnterpriseId"))
                                        {
                                            long templong = 0;
                                            long.TryParse(Convert.ToString(dr["EnterpriseId"]), out templong);
                                            objRoleMasterDTO.EnterpriseId = templong;
                                        }
                                        if (dtRoles.Columns.Contains("UserType"))
                                        {
                                            int templong = 0;
                                            int.TryParse(Convert.ToString(dr["UserType"]), out templong);
                                            objRoleMasterDTO.UserType = templong;
                                        }
                                        if (dtRoles.Columns.Contains("UserTypeName"))
                                        {
                                            objRoleMasterDTO.UserTypeName = Convert.ToString(dr["UserTypeName"]);
                                        }
                                        if (dtRoles.Columns.Contains("Enterprisename"))
                                        {
                                            objRoleMasterDTO.EnterpriseName = Convert.ToString(dr["Enterprisename"]);
                                        }
                                        if (dtRoles.Columns.Contains("CompanyName"))
                                        {
                                            objRoleMasterDTO.CompanyName = Convert.ToString(dr["CompanyName"]);
                                        }
                                        if (dtRoles.Columns.Contains("RoomName"))
                                        {
                                            objRoleMasterDTO.RoomName = Convert.ToString(dr["RoomName"]);
                                        }
                                        if (dtRoles.Columns.Contains("CreatedByName"))
                                        {
                                            objRoleMasterDTO.CreatedByName = Convert.ToString(dr["CreatedByName"]);
                                        }
                                        if (dtRoles.Columns.Contains("LastUpdatedByName"))
                                        {
                                            objRoleMasterDTO.UpdatedByName = Convert.ToString(dr["LastUpdatedByName"]);
                                        }
                                        lstAllRoles.Add(objRoleMasterDTO);
                                    }
                                }
                            }
                        }
                    }
                }
                if (UserType == 2)
                {
                    if (EnterpriseId.HasValue && EnterpriseId.Value > 0)
                    {
                        EnterpriseDTO objEnterpriseDTO = new EnterpriseDTO();
                        EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
                        objEnterpriseDTO = objEnterpriseMasterDAL.GetEnterpriseByIdPlain(EnterpriseId.Value);
                        if (objEnterpriseDTO != null && !string.IsNullOrWhiteSpace(objEnterpriseDTO.EnterpriseDBName))
                        {
                            DataSet dsRoles = new DataSet();
                            //string EnterpriseDbConnection = objEnterpriseMasterDAL.GetEnterpriseConnectionstring(objEnterpriseDTO.EnterpriseDBName);
                            string EnterpriseDbConnection = MasterDbConnectionHelper.GeteTurnsSQLConnectionString(objEnterpriseDTO.EnterpriseDBName, MasterDbConnectionType.GeneralReadWrite.ToString("F"));
                            string Qry = @"Select 
	                                    rm.ID
	                                    ,RM.RoleName
	                                    ,RM.Description
	                                    ,rm.IsActive
	                                    ,RM.Created
	                                    ,RM.Updated 
	                                    ,RM.CreatedBy
	                                    ,RM.LastUpdatedBy
	                                    ,RM.IsDeleted
	                                    ,RM.IsArchived
	                                    ,RM.Room
	                                    ,RM.CompanyID
	                                    ,RM.GUID
	                                    ,RM.UDF1
	                                    ,RM.UDF2
	                                    ,RM.UDF3
	                                    ,RM.UDF4
	                                    ,RM.UDF5
	                                    ,RM.UDF6
	                                    ,RM.UDF7
	                                    ,RM.UDF8
	                                    ,RM.UDF9
	                                    ,RM.UDF10
	                                    ,RM.EnterpriseId
	                                    ,RM.UserType
	                                    ,rtm.UserTypeName
	                                    ,ISNULL(em.Name,'') as Enterprisename
	                                    ,ISNULL(cm.Name,'') as CompanyName
                                        ,ISNULL(rmm.RoomName,'') as RoomName
	                                    ,ISNULL(um.UserName,'') as CreatedByName
	                                    ,ISNULL(umup.UserName,'') as LastUpdatedByName
	                                    FROM RoleMaster as RM 
                                        LEFT JOIN Enterprise as em on RM.EnterpriseId = em.ID
                                        left JOIN Room as rmm on RM.Room = rmm.ID
                                        left JOIN CompanyMaster as cm on RM.CompanyID = cm.ID
                                        left JOIN UserMaster as um ON RM.CreatedBy = um.ID
                                        left JOIN UserMaster as umup ON RM.LastUpdatedBy = umup.ID
                                        left JOIN UserTypeMaster as rtm ON RM.UserType = rtm.UserTypeID";


                            dsRoles = SqlHelper.ExecuteDataset(EnterpriseDbConnection, CommandType.Text, Qry);
                            if (dsRoles != null && dsRoles.Tables.Count > 0)
                            {
                                DataTable dtRoles = dsRoles.Tables[0];
                                if (dtRoles.Rows.Count > 0)
                                {
                                    foreach (DataRow dr in dtRoles.Rows)
                                    {
                                        RoleMasterDTO objRoleMasterDTO = new RoleMasterDTO();
                                        if (dtRoles.Columns.Contains("ID"))
                                        {
                                            long templong = 0;
                                            long.TryParse(Convert.ToString(dr["ID"]), out templong);
                                            objRoleMasterDTO.ID = templong;
                                        }
                                        if (dtRoles.Columns.Contains("RoleName"))
                                        {
                                            objRoleMasterDTO.RoleName = Convert.ToString(dr["RoleName"]);
                                        }
                                        if (dtRoles.Columns.Contains("Description"))
                                        {
                                            objRoleMasterDTO.Description = Convert.ToString(dr["Description"]);
                                        }
                                        if (dtRoles.Columns.Contains("IsActive"))
                                        {
                                            bool tempboolvar = false;
                                            bool.TryParse(Convert.ToString(dr["IsActive"]), out tempboolvar);
                                            objRoleMasterDTO.IsActive = tempboolvar;
                                        }
                                        if (dtRoles.Columns.Contains("Created"))
                                        {
                                            DateTime tempvar = DateTime.MinValue;
                                            DateTime.TryParse(Convert.ToString(dr["Created"]), out tempvar);
                                            objRoleMasterDTO.Created = tempvar;
                                        }
                                        if (dtRoles.Columns.Contains("Updated"))
                                        {
                                            DateTime tempvar = DateTime.MinValue;
                                            DateTime.TryParse(Convert.ToString(dr["Updated"]), out tempvar);
                                            objRoleMasterDTO.Updated = tempvar;
                                        }
                                        if (dtRoles.Columns.Contains("CreatedBy"))
                                        {
                                            long templong = 0;
                                            long.TryParse(Convert.ToString(dr["CreatedBy"]), out templong);
                                            objRoleMasterDTO.CreatedBy = templong;
                                        }
                                        if (dtRoles.Columns.Contains("LastUpdatedBy"))
                                        {
                                            long templong = 0;
                                            long.TryParse(Convert.ToString(dr["LastUpdatedBy"]), out templong);
                                            objRoleMasterDTO.LastUpdatedBy = templong;
                                        }
                                        if (dtRoles.Columns.Contains("IsDeleted"))
                                        {
                                            bool tempboolvar = false;
                                            bool.TryParse(Convert.ToString(dr["IsDeleted"]), out tempboolvar);
                                            objRoleMasterDTO.IsDeleted = tempboolvar;
                                        }
                                        if (dtRoles.Columns.Contains("IsArchived"))
                                        {
                                            bool tempboolvar = false;
                                            bool.TryParse(Convert.ToString(dr["IsArchived"]), out tempboolvar);
                                            objRoleMasterDTO.IsArchived = tempboolvar;
                                        }
                                        if (dtRoles.Columns.Contains("IsArchived"))
                                        {
                                            bool tempboolvar = false;
                                            bool.TryParse(Convert.ToString(dr["IsArchived"]), out tempboolvar);
                                            objRoleMasterDTO.IsArchived = tempboolvar;
                                        }
                                        if (dtRoles.Columns.Contains("Room"))
                                        {
                                            long templong = 0;
                                            long.TryParse(Convert.ToString(dr["Room"]), out templong);
                                            objRoleMasterDTO.Room = templong;
                                        }
                                        if (dtRoles.Columns.Contains("CompanyID"))
                                        {
                                            long templong = 0;
                                            long.TryParse(Convert.ToString(dr["CompanyID"]), out templong);
                                            objRoleMasterDTO.CompanyID = templong;
                                        }
                                        if (dtRoles.Columns.Contains("GUID"))
                                        {
                                            Guid templong = Guid.Empty;
                                            Guid.TryParse(Convert.ToString(dr["GUID"]), out templong);
                                            objRoleMasterDTO.GUID = templong;
                                        }
                                        if (dtRoles.Columns.Contains("EnterpriseId"))
                                        {
                                            long templong = 0;
                                            long.TryParse(Convert.ToString(dr["EnterpriseId"]), out templong);
                                            objRoleMasterDTO.EnterpriseId = templong;
                                        }
                                        if (dtRoles.Columns.Contains("UserType"))
                                        {
                                            int templong = 0;
                                            int.TryParse(Convert.ToString(dr["UserType"]), out templong);
                                            objRoleMasterDTO.UserType = templong;
                                        }
                                        if (dtRoles.Columns.Contains("UserTypeName"))
                                        {
                                            objRoleMasterDTO.UserTypeName = Convert.ToString(dr["UserTypeName"]);
                                        }
                                        if (dtRoles.Columns.Contains("Enterprisename"))
                                        {
                                            objRoleMasterDTO.EnterpriseName = Convert.ToString(dr["Enterprisename"]);
                                        }
                                        if (dtRoles.Columns.Contains("CompanyName"))
                                        {
                                            objRoleMasterDTO.CompanyName = Convert.ToString(dr["CompanyName"]);
                                        }
                                        if (dtRoles.Columns.Contains("RoomName"))
                                        {
                                            objRoleMasterDTO.RoomName = Convert.ToString(dr["RoomName"]);
                                        }
                                        if (dtRoles.Columns.Contains("CreatedByName"))
                                        {
                                            objRoleMasterDTO.CreatedByName = Convert.ToString(dr["CreatedByName"]);
                                        }
                                        if (dtRoles.Columns.Contains("LastUpdatedByName"))
                                        {
                                            objRoleMasterDTO.UpdatedByName = Convert.ToString(dr["LastUpdatedByName"]);
                                        }
                                        lstAllRoles.Add(objRoleMasterDTO);
                                    }
                                }
                            }
                        }
                    }
                }
                if (UserType == 3)
                {
                    {
                        EnterpriseDTO objEnterpriseDTO = new EnterpriseDTO();
                        EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
                        objEnterpriseDTO = objEnterpriseMasterDAL.GetEnterpriseByIdPlain(EnterpriseId.Value);
                        if (objEnterpriseDTO != null && !string.IsNullOrWhiteSpace(objEnterpriseDTO.EnterpriseDBName))
                        {
                            DataSet dsRoles = new DataSet();
                            string EnterpriseDbConnection = MasterDbConnectionHelper.GeteTurnsSQLConnectionString(objEnterpriseDTO.EnterpriseDBName, MasterDbConnectionType.GeneralReadWrite.ToString("F")); //objEnterpriseMasterDAL.GetEnterpriseConnectionstring(objEnterpriseDTO.EnterpriseDBName);
                            string Qry = @"Select 
	                                    rm.ID
	                                    ,RM.RoleName
	                                    ,RM.Description
	                                    ,rm.IsActive
	                                    ,RM.Created
	                                    ,RM.Updated 
	                                    ,RM.CreatedBy
	                                    ,RM.LastUpdatedBy
	                                    ,RM.IsDeleted
	                                    ,RM.IsArchived
	                                    ,RM.Room
	                                    ,RM.CompanyID
	                                    ,RM.GUID
	                                    ,RM.UDF1
	                                    ,RM.UDF2
	                                    ,RM.UDF3
	                                    ,RM.UDF4
	                                    ,RM.UDF5
	                                    ,RM.UDF6
	                                    ,RM.UDF7
	                                    ,RM.UDF8
	                                    ,RM.UDF9
	                                    ,RM.UDF10
	                                    ,RM.EnterpriseId
	                                    ,RM.UserType
	                                    ,rtm.UserTypeName
	                                    ,ISNULL(em.Name,'') as Enterprisename
	                                    ,ISNULL(cm.Name,'') as CompanyName
                                        ,ISNULL(rmm.RoomName,'') as RoomName
	                                    ,ISNULL(um.UserName,'') as CreatedByName
	                                    ,ISNULL(umup.UserName,'') as LastUpdatedByName
	                                    FROM RoleMaster as RM 
                                        LEFT JOIN Enterprise as em on RM.EnterpriseId = em.ID
                                        left JOIN Room as rmm on RM.Room = rmm.ID
                                        left JOIN CompanyMaster as cm on RM.CompanyID = cm.ID
                                        left JOIN UserMaster as um ON RM.CreatedBy = um.ID
                                        left JOIN UserMaster as umup ON RM.LastUpdatedBy = umup.ID
                                        left JOIN UserTypeMaster as rtm ON RM.UserType = rtm.UserTypeID
                                         Where RM.CompanyID =" + CompanyId.Value + " and RM.UserType=" + UserType;


                            dsRoles = SqlHelper.ExecuteDataset(EnterpriseDbConnection, CommandType.Text, Qry);
                            if (dsRoles != null && dsRoles.Tables.Count > 0)
                            {
                                DataTable dtRoles = dsRoles.Tables[0];
                                if (dtRoles.Rows.Count > 0)
                                {
                                    foreach (DataRow dr in dtRoles.Rows)
                                    {
                                        RoleMasterDTO objRoleMasterDTO = new RoleMasterDTO();
                                        if (dtRoles.Columns.Contains("ID"))
                                        {
                                            long templong = 0;
                                            long.TryParse(Convert.ToString(dr["ID"]), out templong);
                                            objRoleMasterDTO.ID = templong;
                                        }
                                        if (dtRoles.Columns.Contains("RoleName"))
                                        {
                                            objRoleMasterDTO.RoleName = Convert.ToString(dr["RoleName"]);
                                        }
                                        if (dtRoles.Columns.Contains("Description"))
                                        {
                                            objRoleMasterDTO.Description = Convert.ToString(dr["Description"]);
                                        }
                                        if (dtRoles.Columns.Contains("IsActive"))
                                        {
                                            bool tempboolvar = false;
                                            bool.TryParse(Convert.ToString(dr["IsActive"]), out tempboolvar);
                                            objRoleMasterDTO.IsActive = tempboolvar;
                                        }
                                        if (dtRoles.Columns.Contains("Created"))
                                        {
                                            DateTime tempvar = DateTime.MinValue;
                                            DateTime.TryParse(Convert.ToString(dr["Created"]), out tempvar);
                                            objRoleMasterDTO.Created = tempvar;
                                        }
                                        if (dtRoles.Columns.Contains("Updated"))
                                        {
                                            DateTime tempvar = DateTime.MinValue;
                                            DateTime.TryParse(Convert.ToString(dr["Updated"]), out tempvar);
                                            objRoleMasterDTO.Updated = tempvar;
                                        }
                                        if (dtRoles.Columns.Contains("CreatedBy"))
                                        {
                                            long templong = 0;
                                            long.TryParse(Convert.ToString(dr["CreatedBy"]), out templong);
                                            objRoleMasterDTO.CreatedBy = templong;
                                        }
                                        if (dtRoles.Columns.Contains("LastUpdatedBy"))
                                        {
                                            long templong = 0;
                                            long.TryParse(Convert.ToString(dr["LastUpdatedBy"]), out templong);
                                            objRoleMasterDTO.LastUpdatedBy = templong;
                                        }
                                        if (dtRoles.Columns.Contains("IsDeleted"))
                                        {
                                            bool tempboolvar = false;
                                            bool.TryParse(Convert.ToString(dr["IsDeleted"]), out tempboolvar);
                                            objRoleMasterDTO.IsDeleted = tempboolvar;
                                        }
                                        if (dtRoles.Columns.Contains("IsArchived"))
                                        {
                                            bool tempboolvar = false;
                                            bool.TryParse(Convert.ToString(dr["IsArchived"]), out tempboolvar);
                                            objRoleMasterDTO.IsArchived = tempboolvar;
                                        }
                                        if (dtRoles.Columns.Contains("IsArchived"))
                                        {
                                            bool tempboolvar = false;
                                            bool.TryParse(Convert.ToString(dr["IsArchived"]), out tempboolvar);
                                            objRoleMasterDTO.IsArchived = tempboolvar;
                                        }
                                        if (dtRoles.Columns.Contains("Room"))
                                        {
                                            long templong = 0;
                                            long.TryParse(Convert.ToString(dr["Room"]), out templong);
                                            objRoleMasterDTO.Room = templong;
                                        }
                                        if (dtRoles.Columns.Contains("CompanyID"))
                                        {
                                            long templong = 0;
                                            long.TryParse(Convert.ToString(dr["CompanyID"]), out templong);
                                            objRoleMasterDTO.CompanyID = templong;
                                        }
                                        if (dtRoles.Columns.Contains("GUID"))
                                        {
                                            Guid templong = Guid.Empty;
                                            Guid.TryParse(Convert.ToString(dr["GUID"]), out templong);
                                            objRoleMasterDTO.GUID = templong;
                                        }
                                        if (dtRoles.Columns.Contains("EnterpriseId"))
                                        {
                                            long templong = 0;
                                            long.TryParse(Convert.ToString(dr["EnterpriseId"]), out templong);
                                            objRoleMasterDTO.EnterpriseId = templong;
                                        }
                                        if (dtRoles.Columns.Contains("UserType"))
                                        {
                                            int templong = 0;
                                            int.TryParse(Convert.ToString(dr["UserType"]), out templong);
                                            objRoleMasterDTO.UserType = templong;
                                        }
                                        if (dtRoles.Columns.Contains("UserTypeName"))
                                        {
                                            objRoleMasterDTO.UserTypeName = Convert.ToString(dr["UserTypeName"]);
                                        }
                                        if (dtRoles.Columns.Contains("Enterprisename"))
                                        {
                                            objRoleMasterDTO.EnterpriseName = Convert.ToString(dr["Enterprisename"]);
                                        }
                                        if (dtRoles.Columns.Contains("CompanyName"))
                                        {
                                            objRoleMasterDTO.CompanyName = Convert.ToString(dr["CompanyName"]);
                                        }
                                        if (dtRoles.Columns.Contains("RoomName"))
                                        {
                                            objRoleMasterDTO.RoomName = Convert.ToString(dr["RoomName"]);
                                        }
                                        if (dtRoles.Columns.Contains("CreatedByName"))
                                        {
                                            objRoleMasterDTO.CreatedByName = Convert.ToString(dr["CreatedByName"]);
                                        }
                                        if (dtRoles.Columns.Contains("LastUpdatedByName"))
                                        {
                                            objRoleMasterDTO.UpdatedByName = Convert.ToString(dr["LastUpdatedByName"]);
                                        }
                                        lstAllRoles.Add(objRoleMasterDTO);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return lstAllRoles;
        }
    }
}
