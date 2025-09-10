using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Data.SqlClient;

namespace eTurnsMaster.DAL
{
    public partial class UserMasterDAL : eTurnsMasterBaseDAL
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
        public IEnumerable<UserMasterDTO> GetAllUsers()
        {
            IEnumerable<UserMasterDTO> ObjCache = null;
            if (ObjCache == null)
            {
                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<UserMasterDTO> obj = (from u in context.ExecuteStoreQuery<UserMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName FROM UserMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1")
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
                                                          IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                                          IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                                          CompanyID = u.CompanyID,
                                                          Created = u.Created,
                                                          Updated = u.Updated,
                                                          CreatedByName = u.CreatedByName,
                                                          UpdatedByName = u.UpdatedByName,
                                                          EnterpriseId = u.EnterpriseId,
                                                          Prefix = u.Prefix,
                                                          FirstName = u.FirstName,
                                                          MiddleName = u.MiddleName,
                                                          LastName = u.LastName,
                                                          Gender = u.Gender,
                                                          Phone2 = u.Phone2,
                                                          EmployeeNumber = u.EmployeeNumber,
                                                          CostCenter = u.CostCenter,
                                                          JobTitle = u.JobTitle,
                                                          Address = u.Address,
                                                          Country = u.Country,
                                                          State = u.State,
                                                          City = u.City,
                                                          PostalCode = u.PostalCode,
                                                          UDF1 = u.UDF1,
                                                          UDF2 = u.UDF2,
                                                          UDF3 = u.UDF3,
                                                          UDF4 = u.UDF4,
                                                          UDF5 = u.UDF5,
                                                          UDF6 = u.UDF6,
                                                          UDF7 = u.UDF7,
                                                          UDF8 = u.UDF8,
                                                          UDF9 = u.UDF9,
                                                          UDF10 = u.UDF10
                                                      }).ToList();
                    ObjCache = obj;
                }
            }
            return ObjCache;
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
        public IEnumerable<UserMasterDTO> GetAllRecords()
        {

            //Get Cached-Media
            IEnumerable<UserMasterDTO> ObjCache = CacheHelper<IEnumerable<UserMasterDTO>>.GetCacheItem("Cached_UserMaster_Master_");
            if (ObjCache == null)
            {
                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<UserMasterDTO> obj = (from u in context.ExecuteStoreQuery<UserMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName FROM UserMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 ")
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
                                                          IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                                          IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                                          CompanyID = u.CompanyID,
                                                          Created = u.Created,
                                                          Updated = u.Updated,
                                                          CreatedByName = u.CreatedByName,
                                                          UpdatedByName = u.UpdatedByName,
                                                          EnterpriseId = u.EnterpriseId,
                                                          UserType = u.UserType,
                                                          Prefix = u.Prefix,
                                                          FirstName = u.FirstName,
                                                          MiddleName = u.MiddleName,
                                                          LastName = u.LastName,
                                                          Gender = u.Gender,
                                                          Phone2 = u.Phone2,
                                                          EmployeeNumber = u.EmployeeNumber,
                                                          CostCenter = u.CostCenter,
                                                          JobTitle = u.JobTitle,
                                                          Address = u.Address,
                                                          Country = u.Country,
                                                          State = u.State,
                                                          City = u.City,
                                                          PostalCode = u.PostalCode,
                                                          UDF1 = u.UDF1,
                                                          UDF2 = u.UDF2,
                                                          UDF3 = u.UDF3,
                                                          UDF4 = u.UDF4,
                                                          UDF5 = u.UDF5,
                                                          UDF6 = u.UDF6,
                                                          UDF7 = u.UDF7,
                                                          UDF8 = u.UDF8,
                                                          UDF9 = u.UDF9,
                                                          UDF10 = u.UDF10
                                                      }).ToList();
                    ObjCache = CacheHelper<IEnumerable<UserMasterDTO>>.AddCacheItem("Cached_UserMaster_Master_", obj);
                }
            }

            return ObjCache;//.Where(t => t.Room == RoomID);
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
        public IEnumerable<UserMasterDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName)
        {
            //Get Cached-Media
            IEnumerable<UserMasterDTO> ObjCache = CacheHelper<IEnumerable<UserMasterDTO>>.GetCacheItem("Cached_UserMaster_Master_");

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
                    IEnumerable<UserMasterDTO> obj = (from u in context.ExecuteStoreQuery<UserMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName FROM UserMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1")
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
                                                          IsDeleted = u.IsDeleted,
                                                          IsArchived = u.IsArchived,
                                                          CompanyID = u.CompanyID,

                                                          Created = u.Created,
                                                          Updated = u.Updated,
                                                          CreatedByName = u.CreatedByName,
                                                          UpdatedByName = u.UpdatedByName,
                                                          Prefix = u.Prefix,
                                                          FirstName = u.FirstName,
                                                          MiddleName = u.MiddleName,
                                                          LastName = u.LastName,
                                                          Gender = u.Gender,
                                                          Phone2 = u.Phone2,
                                                          EmployeeNumber = u.EmployeeNumber,
                                                          CostCenter = u.CostCenter,
                                                          JobTitle = u.JobTitle,
                                                          Address = u.Address,
                                                          Country = u.Country,
                                                          State = u.State,
                                                          City = u.City,
                                                          PostalCode = u.PostalCode,
                                                          UDF1 = u.UDF1,
                                                          UDF2 = u.UDF2,
                                                          UDF3 = u.UDF3,
                                                          UDF4 = u.UDF4,
                                                          UDF5 = u.UDF5,
                                                          UDF6 = u.UDF6,
                                                          UDF7 = u.UDF7,
                                                          UDF8 = u.UDF8,
                                                          UDF9 = u.UDF9,
                                                          UDF10 = u.UDF10
                                                      }).ToList();

                    foreach (UserMasterDTO item in obj)
                    {
                        if (item.ID > 0)
                        {
                            item.PermissionList = null;// GetRoleModuleDetailsRecord(item.ID, item.Room);
                            item.ReplenishingRooms = null;// GetUserRoomReplanishmentDetailsRecord(item.ID);
                        }
                    }
                    ObjCache = CacheHelper<IEnumerable<UserMasterDTO>>.AddCacheItem("Cached_UserMaster_Master_", obj);
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
                    TotalCount = Convert.ToInt32(context.ExecuteStoreQuery<Int32>(@"SELECT Count(A.ID) FROM UserMaster as A WHERE A.IsDeleted!=1  AND A.IsArchived != 1 " + search + "").ToList()[0]);

                    IEnumerable<UserMasterDTO> obj = (from u in context.ExecuteStoreQuery<UserMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName FROM UserMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 " + search + " ORDER BY " + strSortinitializer)
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
                                                          IsDeleted = u.IsDeleted,
                                                          IsArchived = u.IsArchived,
                                                          CompanyID = u.CompanyID,

                                                          Created = u.Created,
                                                          Updated = u.Updated,
                                                          CreatedByName = u.CreatedByName,
                                                          UpdatedByName = u.UpdatedByName,
                                                          Prefix = u.Prefix,
                                                          FirstName = u.FirstName,
                                                          MiddleName = u.MiddleName,
                                                          LastName = u.LastName,
                                                          Gender = u.Gender,
                                                          Phone2 = u.Phone2,
                                                          EmployeeNumber = u.EmployeeNumber,
                                                          CostCenter = u.CostCenter,
                                                          JobTitle = u.JobTitle,
                                                          Address = u.Address,
                                                          Country = u.Country,
                                                          State = u.State,
                                                          City = u.City,
                                                          PostalCode = u.PostalCode,
                                                          UDF1 = u.UDF1,
                                                          UDF2 = u.UDF2,
                                                          UDF3 = u.UDF3,
                                                          UDF4 = u.UDF4,
                                                          UDF5 = u.UDF5,
                                                          UDF6 = u.UDF6,
                                                          UDF7 = u.UDF7,
                                                          UDF8 = u.UDF8,
                                                          UDF9 = u.UDF9,
                                                          UDF10 = u.UDF10
                                                      }).Skip(StartRowIndex).Take(MaxRows).ToList();

                    foreach (UserMasterDTO item in obj)
                    {
                        if (item.ID > 0)
                        {
                            item.PermissionList = null;// GetRoleModuleDetailsRecord(item.ID);
                            item.ReplenishingRooms = null;// GetUserRoomReplanishmentDetailsRecord(item.ID);
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

                    IEnumerable<UserMasterDTO> obj = (from u in context.ExecuteStoreQuery<UserMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, A.IsDeleted,A.IsArchived FROM UserMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID  WHERE A.IsArchived != 1  ORDER BY " + strSortinitializer)
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
                                                          IsDeleted = u.IsDeleted,
                                                          IsArchived = u.IsArchived,
                                                          CompanyID = u.CompanyID,

                                                          Created = u.Created,
                                                          Updated = u.Updated,
                                                          CreatedByName = u.CreatedByName,
                                                          UpdatedByName = u.UpdatedByName,
                                                          Prefix = u.Prefix,
                                                          FirstName = u.FirstName,
                                                          MiddleName = u.MiddleName,
                                                          LastName = u.LastName,
                                                          Gender = u.Gender,
                                                          Phone2 = u.Phone2,
                                                          EmployeeNumber = u.EmployeeNumber,
                                                          CostCenter = u.CostCenter,
                                                          JobTitle = u.JobTitle,
                                                          Address = u.Address,
                                                          Country = u.Country,
                                                          State = u.State,
                                                          City = u.City,
                                                          PostalCode = u.PostalCode,
                                                          UDF1 = u.UDF1,
                                                          UDF2 = u.UDF2,
                                                          UDF3 = u.UDF3,
                                                          UDF4 = u.UDF4,
                                                          UDF5 = u.UDF5,
                                                          UDF6 = u.UDF6,
                                                          UDF7 = u.UDF7,
                                                          UDF8 = u.UDF8,
                                                          UDF9 = u.UDF9,
                                                          UDF10 = u.UDF10
                                                      }).Skip(StartRowIndex).Take(MaxRows).ToList();

                    foreach (UserMasterDTO item in obj)
                    {
                        if (item.ID > 0)
                        {
                            item.PermissionList = null;// GetRoleModuleDetailsRecord(item.ID);
                            item.ReplenishingRooms = null;// GetUserRoomReplanishmentDetailsRecord(item.ID);
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

                    IEnumerable<UserMasterDTO> obj = (from u in context.ExecuteStoreQuery<UserMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, A.IsDeleted,A.IsArchived FROM UserMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID WHERE A.IsDeleted != 1  ORDER BY " + strSortinitializer)
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
                                                          IsDeleted = u.IsDeleted,
                                                          IsArchived = u.IsArchived,
                                                          CompanyID = u.CompanyID,

                                                          Created = u.Created,
                                                          Updated = u.Updated,
                                                          CreatedByName = u.CreatedByName,
                                                          UpdatedByName = u.UpdatedByName,
                                                          Prefix = u.Prefix,
                                                          FirstName = u.FirstName,
                                                          MiddleName = u.MiddleName,
                                                          LastName = u.LastName,
                                                          Gender = u.Gender,
                                                          Phone2 = u.Phone2,
                                                          EmployeeNumber = u.EmployeeNumber,
                                                          CostCenter = u.CostCenter,
                                                          JobTitle = u.JobTitle,
                                                          Address = u.Address,
                                                          Country = u.Country,
                                                          State = u.State,
                                                          City = u.City,
                                                          PostalCode = u.PostalCode,
                                                          UDF1 = u.UDF1,
                                                          UDF2 = u.UDF2,
                                                          UDF3 = u.UDF3,
                                                          UDF4 = u.UDF4,
                                                          UDF5 = u.UDF5,
                                                          UDF6 = u.UDF6,
                                                          UDF7 = u.UDF7,
                                                          UDF8 = u.UDF8,
                                                          UDF9 = u.UDF9,
                                                          UDF10 = u.UDF10
                                                      }).Skip(StartRowIndex).Take(MaxRows).ToList();

                    foreach (UserMasterDTO item in obj)
                    {
                        if (item.ID > 0)
                        {
                            item.PermissionList = null;//  GetRoleModuleDetailsRecord(item.ID);
                            item.ReplenishingRooms = null;// GetUserRoomReplanishmentDetailsRecord(item.ID);
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

                    IEnumerable<UserMasterDTO> obj = (from u in context.ExecuteStoreQuery<UserMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, A.IsDeleted,A.IsArchived FROM UserMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID ORDER BY " + strSortinitializer)
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
                                                          IsDeleted = u.IsDeleted,
                                                          IsArchived = u.IsArchived,
                                                          CompanyID = u.CompanyID,

                                                          Created = u.Created,
                                                          Updated = u.Updated,
                                                          CreatedByName = u.CreatedByName,
                                                          UpdatedByName = u.UpdatedByName,
                                                          Prefix = u.Prefix,
                                                          FirstName = u.FirstName,
                                                          MiddleName = u.MiddleName,
                                                          LastName = u.LastName,
                                                          Gender = u.Gender,
                                                          Phone2 = u.Phone2,
                                                          EmployeeNumber = u.EmployeeNumber,
                                                          CostCenter = u.CostCenter,
                                                          JobTitle = u.JobTitle,
                                                          Address = u.Address,
                                                          Country = u.Country,
                                                          State = u.State,
                                                          City = u.City,
                                                          PostalCode = u.PostalCode,
                                                          UDF1 = u.UDF1,
                                                          UDF2 = u.UDF2,
                                                          UDF3 = u.UDF3,
                                                          UDF4 = u.UDF4,
                                                          UDF5 = u.UDF5,
                                                          UDF6 = u.UDF6,
                                                          UDF7 = u.UDF7,
                                                          UDF8 = u.UDF8,
                                                          UDF9 = u.UDF9,
                                                          UDF10 = u.UDF10
                                                      }).Skip(StartRowIndex).Take(MaxRows).ToList();

                    foreach (UserMasterDTO item in obj)
                    {
                        if (item.ID > 0)
                        {
                            item.PermissionList = null;//  GetRoleModuleDetailsRecord(item.ID);
                            item.ReplenishingRooms = null;// GetUserRoomReplanishmentDetailsRecord(item.ID);
                        }
                    }
                    return obj;
                }
            }
            else
            {

                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    TotalCount = Convert.ToInt32(context.ExecuteStoreQuery<Int32>(@"SELECT Count(ID) FROM UserMaster WHERE IsDeleted!=1 AND IsArchived != 1 AND ((UserName like '%" + SearchTerm + "%') OR (ID like '%" + SearchTerm + "%'))").ToList()[0]);
                    IEnumerable<UserMasterDTO> obj = (from u in context.ExecuteStoreQuery<UserMasterDTO>("SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName FROM UserMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND ((A.UserName like '%" + SearchTerm + "%') OR (A.ID like '%" + SearchTerm + "%')) ORDER BY " + strSortinitializer)
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
                                                          IsDeleted = u.IsDeleted,
                                                          IsArchived = u.IsArchived,
                                                          CompanyID = u.CompanyID,

                                                          Created = u.Created,
                                                          Updated = u.Updated,
                                                          CreatedByName = u.CreatedByName,
                                                          UpdatedByName = u.UpdatedByName,
                                                          Prefix = u.Prefix,
                                                          FirstName = u.FirstName,
                                                          MiddleName = u.MiddleName,
                                                          LastName = u.LastName,
                                                          Gender = u.Gender,
                                                          Phone2 = u.Phone2,
                                                          EmployeeNumber = u.EmployeeNumber,
                                                          CostCenter = u.CostCenter,
                                                          JobTitle = u.JobTitle,
                                                          Address = u.Address,
                                                          Country = u.Country,
                                                          State = u.State,
                                                          City = u.City,
                                                          PostalCode = u.PostalCode,
                                                          UDF1 = u.UDF1,
                                                          UDF2 = u.UDF2,
                                                          UDF3 = u.UDF3,
                                                          UDF4 = u.UDF4,
                                                          UDF5 = u.UDF5,
                                                          UDF6 = u.UDF6,
                                                          UDF7 = u.UDF7,
                                                          UDF8 = u.UDF8,
                                                          UDF9 = u.UDF9,
                                                          UDF10 = u.UDF10
                                                      }).Skip(StartRowIndex).Take(MaxRows).ToList();

                    foreach (UserMasterDTO item in obj)
                    {
                        if (item.ID > 0)
                        {
                            item.PermissionList = null;// GetRoleModuleDetailsRecord(item.ID);
                            item.ReplenishingRooms = null;// GetUserRoomReplanishmentDetailsRecord(item.ID);
                        }
                    }
                    return obj;
                }
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
                objresult = (from u in context.ExecuteStoreQuery<UserMasterDTO>(@";with Userlist as (
                                                                SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName,us.RedirectURL 
                                                                FROM UserMaster A 
                                                                LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID
                                                                LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID  
                                                                LEFT OUTER join UserSetting as Us on A.id = Us.UserId 
                                                                WHERE A.IsDeleted!=1 AND A.IsArchived != 1  )
                                                                ,Useraccept as
                                                                (
                                                                select min( LicenceAcceptDate)   as FirstLicenceAccept,
                                                                 max( LicenceAcceptDate)    as LastLicenceAccept
                                                                , isnull(count(1) ,0)  as Acceptcount,UA.UserID 
                                                                from dbo.[UserLicenceAccept]  as UA
                                                                inner join UserMaster U on U.ID = Ua.UserID
                                                                group by UA.UserID
                                                                )
                                                                SELECT UL.*,UA.FirstLicenceAccept,UA.LastLicenceAccept,isnull(UA.Acceptcount,0) as Acceptcount
                                                                FROM Userlist as UL
                                                                left JOIN Useraccept as UA
                                                                on Ul.id= UA.UserID
                                                                where  UL.ID=" + id.ToString())
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
                                 IsDeleted = u.IsDeleted,
                                 IsArchived = u.IsArchived,
                                 CompanyID = u.CompanyID,
                                 UserType = u.UserType,
                                 Created = u.Created,
                                 Updated = u.Updated,
                                 CreatedByName = u.CreatedByName,
                                 UpdatedByName = u.UpdatedByName,
                                 LastUpdatedBy = u.LastUpdatedBy,
                                 CreatedBy = u.CreatedBy,
                                 RedirectURL = u.RedirectURL,
                                 Room = u.Room,
                                 EnterpriseId = u.EnterpriseId,
                                 FirstLicenceAccept = u.FirstLicenceAccept,
                                 LastLicenceAccept = u.LastLicenceAccept,
                                 Acceptcount = u.Acceptcount,
                                 EnforceRolePermission = u.EnforceRolePermission,
                                 IseTurnsAdmin = u.IseTurnsAdmin,
                                 Prefix = u.Prefix,
                                 FirstName = u.FirstName,
                                 MiddleName = u.MiddleName,
                                 LastName = u.LastName,
                                 Gender = u.Gender,
                                 Phone2 = u.Phone2,
                                 EmployeeNumber = u.EmployeeNumber,
                                 CostCenter = u.CostCenter,
                                 JobTitle = u.JobTitle,
                                 Address = u.Address,
                                 Country = u.Country,
                                 State = u.State,
                                 City = u.City,
                                 PostalCode = u.PostalCode,
                                 UDF1 = u.UDF1,
                                 UDF2 = u.UDF2,
                                 UDF3 = u.UDF3,
                                 UDF4 = u.UDF4,
                                 UDF5 = u.UDF5,
                                 UDF6 = u.UDF6,
                                 UDF7 = u.UDF7,
                                 UDF8 = u.UDF8,
                                 UDF9 = u.UDF9,
                                 UDF10 = u.UDF10
                             }).SingleOrDefault();

                objresult.PermissionList = GetUserRoleModuleDetailsRecord(id, objresult.RoleID, objresult.UserType);
                //objresult.PermissionList = objresult.PermissionList.Where(t => t.ModuleID == 77).ToList();
                string RoomLists = "";
                if (objresult.PermissionList != null && objresult.PermissionList.Count > 0)
                {
                    objresult.UserWiseAllRoomsAccessDetails = ConvertUserPermissions(objresult.PermissionList, objresult.RoleID, ref RoomLists);
                    if (objresult.UserWiseAllRoomsAccessDetails != null && objresult.UserWiseAllRoomsAccessDetails.Count > 0)
                    {
                        objresult.SelectedEnterpriseAccessValue = string.Join(",", objresult.UserWiseAllRoomsAccessDetails.Select(t => t.EnterpriseId).Distinct().ToArray());
                        var qCompanies = (from itm in objresult.UserWiseAllRoomsAccessDetails
                                          group itm by new { itm.EnterpriseId, itm.CompanyId } into groupedEnterpriseCompanies
                                          select new RolePermissionInfo
                                          {
                                              EnterPriseId = groupedEnterpriseCompanies.Key.EnterpriseId,
                                              CompanyId = groupedEnterpriseCompanies.Key.CompanyId,
                                              EnterPriseId_CompanyId = groupedEnterpriseCompanies.Key.EnterpriseId + "_" + groupedEnterpriseCompanies.Key.CompanyId
                                          });
                        objresult.SelectedCompanyAccessValue = string.Join(",", qCompanies.Select(t => t.EnterPriseId_CompanyId).ToArray());
                    }

                    objresult.SelectedRoomAccessValue = RoomLists;
                }
                objresult.ReplenishingRooms = GetUserRoomReplanishmentDetailsRecord(id);
            }
            return objresult;
        }

        public UserMasterDTO getUserDetails(Int64 id)
        {
            UserMasterDTO objresult = new UserMasterDTO();
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                objresult = (from u in context.ExecuteStoreQuery<UserMasterDTO>(@";with Userlist as (
                                                                SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName,us.RedirectURL 
                                                                FROM UserMaster A 
                                                                LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID
                                                                LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID  
                                                                LEFT OUTER join UserSetting as Us on A.id = Us.UserId 
                                                                WHERE A.IsDeleted!=1 AND A.IsArchived != 1  )
                                                                ,Useraccept as
                                                                (
                                                                select min( LicenceAcceptDate)   as FirstLicenceAccept,
                                                                 max( LicenceAcceptDate)    as LastLicenceAccept
                                                                , isnull(count(1) ,0)  as Acceptcount,UA.UserID 
                                                                from dbo.[UserLicenceAccept]  as UA
                                                                inner join UserMaster U on U.ID = Ua.UserID
                                                                group by UA.UserID
                                                                )
                                                                SELECT UL.*,UA.FirstLicenceAccept,UA.LastLicenceAccept,isnull(UA.Acceptcount,0) as Acceptcount
                                                                FROM Userlist as UL
                                                                left JOIN Useraccept as UA
                                                                on Ul.id= UA.UserID
                                                                where  UL.ID=" + id.ToString())
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
                                 IsDeleted = u.IsDeleted,
                                 IsArchived = u.IsArchived,
                                 CompanyID = u.CompanyID,
                                 UserType = u.UserType,
                                 Created = u.Created,
                                 Updated = u.Updated,
                                 CreatedByName = u.CreatedByName,
                                 UpdatedByName = u.UpdatedByName,
                                 LastUpdatedBy = u.LastUpdatedBy,
                                 CreatedBy = u.CreatedBy,
                                 RedirectURL = u.RedirectURL,
                                 Room = u.Room,
                                 EnterpriseId = u.EnterpriseId,
                                 FirstLicenceAccept = u.FirstLicenceAccept,
                                 LastLicenceAccept = u.LastLicenceAccept,
                                 Acceptcount = u.Acceptcount,
                                 EnforceRolePermission = u.EnforceRolePermission,
                                 IseTurnsAdmin = u.IseTurnsAdmin,
                                 Prefix = u.Prefix,
                                 FirstName = u.FirstName,
                                 MiddleName = u.MiddleName,
                                 LastName = u.LastName,
                                 Gender = u.Gender,
                                 Phone2 = u.Phone2,
                                 EmployeeNumber = u.EmployeeNumber,
                                 CostCenter = u.CostCenter,
                                 JobTitle = u.JobTitle,
                                 Address = u.Address,
                                 Country = u.Country,
                                 State = u.State,
                                 City = u.City,
                                 PostalCode = u.PostalCode,
                                 UDF1 = u.UDF1,
                                 UDF2 = u.UDF2,
                                 UDF3 = u.UDF3,
                                 UDF4 = u.UDF4,
                                 UDF5 = u.UDF5,
                                 UDF6 = u.UDF6,
                                 UDF7 = u.UDF7,
                                 UDF8 = u.UDF8,
                                 UDF9 = u.UDF9,
                                 UDF10 = u.UDF10
                             }).SingleOrDefault();


            }
            return objresult;
        }

        public List<UserWiseRoomsAccessDetailsDTO> ConvertUserPermissions1(List<UserRoleModuleDetailsDTO> objData, Int64 RoleID, ref string RoomLists)
        {
            EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
            List<EnterpriseDTO> lstEnterprises = new List<EnterpriseDTO>();
            List<UserWiseRoomsAccessDetailsDTO> objRooms = new List<UserWiseRoomsAccessDetailsDTO>();
            if (RoleID == -1)
            {
                UserWiseRoomsAccessDetailsDTO obj = new UserWiseRoomsAccessDetailsDTO();
                obj.PermissionList = objData.ToList();
                obj.RoomID = 0;
                objRooms.Add(obj);
            }
            else
            {
                UserWiseRoomsAccessDetailsDTO objRoleRooms;

                var objTempPermissionList = objData.Where(t => t.EnteriseId > 0 && t.CompanyId > 0 && t.RoomId > 0).GroupBy(element => new { element.RoomId, element.CompanyId, element.EnteriseId })
                                                .OrderBy(g => g.Key.EnteriseId);
                if (objTempPermissionList.Any())
                {
                    lstEnterprises = objEnterpriseMasterDAL.GetEnterprises(objTempPermissionList.Select(t => t.Key.EnteriseId).ToArray());
                }
                foreach (var grpData in objTempPermissionList)
                {
                    if (grpData.Key.RoomId > 0 && grpData.Key.CompanyId > 0 && grpData.Key.EnteriseId > 0)
                    {
                        objRoleRooms = new UserWiseRoomsAccessDetailsDTO();
                        objRoleRooms.EnterpriseId = grpData.Key.EnteriseId;
                        objRoleRooms.IsEnterpriseActive = lstEnterprises.FirstOrDefault(t => t.ID == grpData.Key.EnteriseId).IsActive;
                        objRoleRooms.CompanyId = grpData.Key.CompanyId;
                        objRoleRooms.RoomID = grpData.Key.RoomId;
                        objRoleRooms.RoleID = RoleID;
                        List<UserRoleModuleDetailsDTO> cps = objData.Where(t => t.RoomId == grpData.Key.RoomId && t.CompanyId == grpData.Key.CompanyId && t.EnteriseId == grpData.Key.EnteriseId).ToList();
                        //List<UserRoleModuleDetailsDTO>  cps1 = cps.Where(t => t.ModuleID == 77).ToList();
                        if (cps != null)
                        {
                            objRoleRooms.PermissionList = cps;
                            objRoleRooms.RoomName = cps[0].RoomName;
                        }
                        if (objRoleRooms.RoomID > 0)
                        {
                            RoomDTO objRoomDTO = new RoomDTO();

                            RolePermissionInfo objRolePermissionInfo = new RolePermissionInfo();
                            objRolePermissionInfo.EnterPriseId = grpData.Key.EnteriseId;
                            objRolePermissionInfo.CompanyId = grpData.Key.CompanyId;
                            objRolePermissionInfo.RoomId = grpData.Key.RoomId;
                            objRoomDTO = objEnterpriseMasterDAL.GetRoomById(objRolePermissionInfo);
                            objRoleRooms.RoomName = objRoomDTO.RoomName;
                            objRoleRooms.IsRoomActive = objRoomDTO.IsRoomActive;
                            objRoleRooms.CompanyName = objRoomDTO.CompanyName;
                            objRoleRooms.IsCompanyActive = objRoomDTO.IsCompanyActive;
                            objRoleRooms.EnterpriseName = objRoomDTO.EnterpriseName;

                            if (objRoomDTO != null)
                            {
                                RoomLists = objRoleRooms.EnterpriseId + "_" + objRoleRooms.CompanyId + "_" + objRoleRooms.RoomID + "_" + objRoomDTO.RoomName;
                            }
                            else
                            {
                                RoomLists = objRoleRooms.EnterpriseId + "_" + objRoleRooms.CompanyId + "_" + objRoleRooms.RoomID + "_" + "Room" + objRoleRooms.RoomID;
                            }
                        }
                        objRooms.Add(objRoleRooms);
                    }
                }
            }

            return objRooms;
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
                    objresult = (from u in context.ExecuteStoreQuery<UserMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, A.GUID FROM UserMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.Email='" + Email + "' AND A.Password ='" + Password + "' ")
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
                                     IsDeleted = u.IsDeleted,
                                     IsArchived = u.IsArchived,
                                     CompanyID = u.CompanyID,

                                     Created = u.Created,
                                     Updated = u.Updated,
                                     CreatedByName = u.CreatedByName,
                                     UpdatedByName = u.UpdatedByName,
                                     LastUpdatedBy = u.LastUpdatedBy,
                                     CreatedBy = u.CreatedBy,
                                     Prefix = u.Prefix,
                                     FirstName = u.FirstName,
                                     MiddleName = u.MiddleName,
                                     LastName = u.LastName,
                                     Gender = u.Gender,
                                     Phone2 = u.Phone2,
                                     EmployeeNumber = u.EmployeeNumber,
                                     CostCenter = u.CostCenter,
                                     JobTitle = u.JobTitle,
                                     Address = u.Address,
                                     Country = u.Country,
                                     State = u.State,
                                     City = u.City,
                                     PostalCode = u.PostalCode,
                                     UDF1 = u.UDF1,
                                     UDF2 = u.UDF2,
                                     UDF3 = u.UDF3,
                                     UDF4 = u.UDF4,
                                     UDF5 = u.UDF5,
                                     UDF6 = u.UDF6,
                                     UDF7 = u.UDF7,
                                     UDF8 = u.UDF8,
                                     UDF9 = u.UDF9,
                                     UDF10 = u.UDF10
                                 }).SingleOrDefault();
                    if (objresult != null)
                    {
                        objresult.PermissionList = GetUserRoleModuleDetailsRecord(objresult.ID, objresult.RoleID, objresult.UserType);
                        string RoomLists = "";
                        if (objresult.PermissionList != null && objresult.PermissionList.Count > 0)
                        {
                            objresult.UserWiseAllRoomsAccessDetails = ConvertUserPermissions(objresult.PermissionList, objresult.RoleID, ref RoomLists);
                            objresult.SelectedRoomAccessValue = RoomLists;
                        }
                        objresult.ReplenishingRooms = GetUserRoomReplanishmentDetailsRecord(objresult.ID);
                    }
                }
            }
            return objresult;
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

        /// <summary>
        /// Delete Multiple Records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool DeleteRecords(string IDs, Int64 userid)
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

        public List<UserMasterDTO> GetModuleWiseUsers(Int64 ModuleID)
        {

            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<UserMasterDTO>(@"select UM.* from UserRoleDetails URD 
                                                                                            Inner join UserMaster UM ON URD.UserID = UM.ID
                                                                                where URD.ModuleID=" + ModuleID)
                        select new UserMasterDTO
                        {
                            ID = u.ID,
                            GUID = u.GUID,
                            UserName = u.UserName,
                            Password = u.Password,
                            Phone = u.Phone,
                            RoleID = u.RoleID,
                            Email = u.Email,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            CompanyID = u.CompanyID,

                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            LastUpdatedBy = u.LastUpdatedBy,
                            CreatedBy = u.CreatedBy,
                            Prefix = u.Prefix,
                            FirstName = u.FirstName,
                            MiddleName = u.MiddleName,
                            LastName = u.LastName,
                            Gender = u.Gender,
                            Phone2 = u.Phone2,
                            EmployeeNumber = u.EmployeeNumber,
                            CostCenter = u.CostCenter,
                            JobTitle = u.JobTitle,
                            Address = u.Address,
                            Country = u.Country,
                            State = u.State,
                            City = u.City,
                            PostalCode = u.PostalCode,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            UDF6 = u.UDF6,
                            UDF7 = u.UDF7,
                            UDF8 = u.UDF8,
                            UDF9 = u.UDF9,
                            UDF10 = u.UDF10
                        }).ToList();
            }
        }

        public List<UserMasterDTO> GetAllUsers(int UserType, long? EnterpriseId, long? CompanyId, long? RoomId, bool IsArchived, bool IsDeleted, long LoggedinUserId)
        {
            List<UserMasterDTO> lstAllUsers = new List<UserMasterDTO>();
            List<UserMasterDTO> lstAllUsers1 = new List<UserMasterDTO>();
            List<UserMasterDTO> lstAllUsers2 = new List<UserMasterDTO>();

            if (IsArchived == false && IsDeleted == false)
            {
                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    if (UserType == 1)
                    {
                        lstAllUsers1 = (from rls in context.UserMasters
                                        join usrmst in context.UserMasters on rls.CreatedBy equals usrmst.ID into rls_usrmst_join
                                        from rls_usrmst in rls_usrmst_join.DefaultIfEmpty()
                                        join usrmstup in context.UserMasters on rls.LastUpdatedBy equals usrmstup.ID into rls_usrmstup_join
                                        from rls_usrmstup in rls_usrmstup_join.DefaultIfEmpty()
                                        join utm in context.UserTypeMasters on rls.UserType equals utm.UserTypeID into rls_utm_join
                                        from rls_utm in rls_utm_join.DefaultIfEmpty()
                                        where rls.UserType == 1 && rls.IsArchived == false && rls.IsDeleted == false
                                        select new UserMasterDTO
                                        {
                                            CompanyID = rls.CompanyID,
                                            Created = rls.Created ?? DateTime.MinValue,
                                            CreatedBy = rls.CreatedBy,
                                            CreatedByName = rls_usrmst.UserName,
                                            Email = rls.Email,
                                            EnterpriseId = rls.EnterpriseId,
                                            GUID = rls.GUID,
                                            ID = rls.ID,
                                            IsArchived = rls.IsArchived,
                                            IsDeleted = rls.IsDeleted,
                                            LastUpdatedBy = rls.LastUpdatedBy,
                                            Password = rls.Password,
                                            Phone = rls.Phone,
                                            RoleID = rls.RoleId,
                                            Room = rls.RoomId,
                                            Updated = rls.Updated,
                                            UpdatedByName = rls_usrmstup.UserName,
                                            UserName = rls.UserName,
                                            UserType = rls.UserType ?? 0,
                                            UserTypeName = rls_utm.UserTypeName,
                                            Prefix = rls.Prefix,
                                            FirstName = rls.FirstName,
                                            MiddleName = rls.MiddleName,
                                            LastName = rls.LastName,
                                            Gender = rls.Gender,
                                            Phone2 = rls.Phone2,
                                            EmployeeNumber = rls.EmployeeNumber,
                                            CostCenter = rls.CostCenter,
                                            JobTitle = rls.JobTitle,
                                            Address = rls.Address,
                                            Country = rls.Country,
                                            State = rls.State,
                                            City = rls.City,
                                            PostalCode = rls.PostalCode,
                                            UDF1 = rls.UDF1,
                                            UDF2 = rls.UDF2,
                                            UDF3 = rls.UDF3,
                                            UDF4 = rls.UDF4,
                                            UDF5 = rls.UDF5,
                                            UDF6 = rls.UDF6,
                                            UDF7 = rls.UDF7,
                                            UDF8 = rls.UDF8,
                                            UDF9 = rls.UDF9,
                                            UDF10 = rls.UDF10
                                        }).ToList();

                        lstAllUsers2 = (from rls in context.UserMasters
                                        join usrmst in context.UserMasters on rls.CreatedBy equals usrmst.ID into rls_usrmst_join
                                        from rls_usrmst in rls_usrmst_join.DefaultIfEmpty()
                                        join usrmstup in context.UserMasters on rls.LastUpdatedBy equals usrmstup.ID into rls_usrmstup_join
                                        from rls_usrmstup in rls_usrmstup_join.DefaultIfEmpty()
                                        join utm in context.UserTypeMasters on rls.UserType equals utm.UserTypeID into rls_utm_join
                                        from rls_utm in rls_utm_join.DefaultIfEmpty()
                                        where rls.EnterpriseId == EnterpriseId && rls.CompanyID == CompanyId && rls.UserType != 1 && rls.IsArchived == false && rls.IsDeleted == false
                                        select new UserMasterDTO
                                        {
                                            CompanyID = rls.CompanyID,
                                            Created = rls.Created ?? DateTime.MinValue,
                                            CreatedBy = rls.CreatedBy,
                                            CreatedByName = rls_usrmst.UserName,
                                            Email = rls.Email,
                                            EnterpriseId = rls.EnterpriseId,
                                            GUID = rls.GUID,
                                            ID = rls.ID,
                                            IsArchived = rls.IsArchived,
                                            IsDeleted = rls.IsDeleted,
                                            LastUpdatedBy = rls.LastUpdatedBy,
                                            Password = rls.Password,
                                            Phone = rls.Phone,
                                            RoleID = rls.RoleId,
                                            Room = rls.RoomId,
                                            Updated = rls.Updated,
                                            UpdatedByName = rls_usrmstup.UserName,
                                            UserName = rls.UserName,
                                            UserType = rls.UserType ?? 0,
                                            UserTypeName = rls_utm.UserTypeName,
                                            Prefix = rls.Prefix,
                                            FirstName = rls.FirstName,
                                            MiddleName = rls.MiddleName,
                                            LastName = rls.LastName,
                                            Gender = rls.Gender,
                                            Phone2 = rls.Phone2,
                                            EmployeeNumber = rls.EmployeeNumber,
                                            CostCenter = rls.CostCenter,
                                            JobTitle = rls.JobTitle,
                                            Address = rls.Address,
                                            Country = rls.Country,
                                            State = rls.State,
                                            City = rls.City,
                                            PostalCode = rls.PostalCode,
                                            UDF1 = rls.UDF1,
                                            UDF2 = rls.UDF2,
                                            UDF3 = rls.UDF3,
                                            UDF4 = rls.UDF4,
                                            UDF5 = rls.UDF5,
                                            UDF6 = rls.UDF6,
                                            UDF7 = rls.UDF7,
                                            UDF8 = rls.UDF8,
                                            UDF9 = rls.UDF9,
                                            UDF10 = rls.UDF10
                                        }).ToList();
                        lstAllUsers = lstAllUsers1.Union(lstAllUsers2).ToList();
                    }
                    if (UserType == 2)
                    {
                        lstAllUsers1 = (from rls in context.UserMasters
                                        join usrmst in context.UserMasters on rls.CreatedBy equals usrmst.ID into rls_usrmst_join
                                        from rls_usrmst in rls_usrmst_join.DefaultIfEmpty()
                                        join usrmstup in context.UserMasters on rls.LastUpdatedBy equals usrmstup.ID into rls_usrmstup_join
                                        from rls_usrmstup in rls_usrmstup_join.DefaultIfEmpty()
                                        join utm in context.UserTypeMasters on rls.UserType equals utm.UserTypeID into rls_utm_join
                                        from rls_utm in rls_utm_join.DefaultIfEmpty()
                                        where rls.EnterpriseId == EnterpriseId && rls.UserType == 2 && rls.IsArchived == false && rls.IsDeleted == false
                                        select new UserMasterDTO
                                        {
                                            CompanyID = rls.CompanyID,
                                            Created = rls.Created ?? DateTime.MinValue,
                                            CreatedBy = rls.CreatedBy,
                                            CreatedByName = rls_usrmst.UserName,
                                            Email = rls.Email,
                                            EnterpriseId = rls.EnterpriseId,
                                            GUID = rls.GUID,
                                            ID = rls.ID,
                                            IsArchived = rls.IsArchived,
                                            IsDeleted = rls.IsDeleted,
                                            LastUpdatedBy = rls.LastUpdatedBy,
                                            Password = rls.Password,
                                            Phone = rls.Phone,
                                            RoleID = rls.RoleId,
                                            Room = rls.RoomId,
                                            Updated = rls.Updated,
                                            UpdatedByName = rls_usrmstup.UserName,
                                            UserName = rls.UserName,
                                            UserType = rls.UserType ?? 0,
                                            UserTypeName = rls_utm.UserTypeName,
                                            Prefix = rls.Prefix,
                                            FirstName = rls.FirstName,
                                            MiddleName = rls.MiddleName,
                                            LastName = rls.LastName,
                                            Gender = rls.Gender,
                                            Phone2 = rls.Phone2,
                                            EmployeeNumber = rls.EmployeeNumber,
                                            CostCenter = rls.CostCenter,
                                            JobTitle = rls.JobTitle,
                                            Address = rls.Address,
                                            Country = rls.Country,
                                            State = rls.State,
                                            City = rls.City,
                                            PostalCode = rls.PostalCode,
                                            UDF1 = rls.UDF1,
                                            UDF2 = rls.UDF2,
                                            UDF3 = rls.UDF3,
                                            UDF4 = rls.UDF4,
                                            UDF5 = rls.UDF5,
                                            UDF6 = rls.UDF6,
                                            UDF7 = rls.UDF7,
                                            UDF8 = rls.UDF8,
                                            UDF9 = rls.UDF9,
                                            UDF10 = rls.UDF10
                                        }).ToList();
                        lstAllUsers2 = (from rls in context.UserMasters
                                        join usrmst in context.UserMasters on rls.CreatedBy equals usrmst.ID into rls_usrmst_join
                                        from rls_usrmst in rls_usrmst_join.DefaultIfEmpty()
                                        join usrmstup in context.UserMasters on rls.LastUpdatedBy equals usrmstup.ID into rls_usrmstup_join
                                        from rls_usrmstup in rls_usrmstup_join.DefaultIfEmpty()
                                        join utm in context.UserTypeMasters on rls.UserType equals utm.UserTypeID into rls_utm_join
                                        from rls_utm in rls_utm_join.DefaultIfEmpty()
                                        where rls.EnterpriseId == EnterpriseId && rls.CompanyID == CompanyId && rls.UserType > 2
                                        select new UserMasterDTO
                                        {
                                            CompanyID = rls.CompanyID,
                                            Created = rls.Created ?? DateTime.MinValue,
                                            CreatedBy = rls.CreatedBy,
                                            CreatedByName = rls_usrmst.UserName,
                                            Email = rls.Email,
                                            EnterpriseId = rls.EnterpriseId,
                                            GUID = rls.GUID,
                                            ID = rls.ID,
                                            IsArchived = rls.IsArchived,
                                            IsDeleted = rls.IsDeleted,
                                            LastUpdatedBy = rls.LastUpdatedBy,
                                            Password = rls.Password,
                                            Phone = rls.Phone,
                                            RoleID = rls.RoleId,
                                            Room = rls.RoomId,
                                            Updated = rls.Updated,
                                            UpdatedByName = rls_usrmstup.UserName,
                                            UserName = rls.UserName,
                                            UserType = rls.UserType ?? 0,
                                            UserTypeName = rls_utm.UserTypeName,
                                            Prefix = rls.Prefix,
                                            FirstName = rls.FirstName,
                                            MiddleName = rls.MiddleName,
                                            LastName = rls.LastName,
                                            Gender = rls.Gender,
                                            Phone2 = rls.Phone2,
                                            EmployeeNumber = rls.EmployeeNumber,
                                            CostCenter = rls.CostCenter,
                                            JobTitle = rls.JobTitle,
                                            Address = rls.Address,
                                            Country = rls.Country,
                                            State = rls.State,
                                            City = rls.City,
                                            PostalCode = rls.PostalCode,
                                            UDF1 = rls.UDF1,
                                            UDF2 = rls.UDF2,
                                            UDF3 = rls.UDF3,
                                            UDF4 = rls.UDF4,
                                            UDF5 = rls.UDF5,
                                            UDF6 = rls.UDF6,
                                            UDF7 = rls.UDF7,
                                            UDF8 = rls.UDF8,
                                            UDF9 = rls.UDF9,
                                            UDF10 = rls.UDF10
                                        }).ToList();
                        lstAllUsers = lstAllUsers1.Union(lstAllUsers2).ToList();
                    }
                    if (UserType == 3)
                    {
                        lstAllUsers = (from rls in context.UserMasters
                                       join usrmst in context.UserMasters on rls.CreatedBy equals usrmst.ID into rls_usrmst_join
                                       from rls_usrmst in rls_usrmst_join.DefaultIfEmpty()
                                       join usrmstup in context.UserMasters on rls.LastUpdatedBy equals usrmstup.ID into rls_usrmstup_join
                                       from rls_usrmstup in rls_usrmstup_join.DefaultIfEmpty()
                                       join utm in context.UserTypeMasters on rls.UserType equals utm.UserTypeID into rls_utm_join
                                       from rls_utm in rls_utm_join.DefaultIfEmpty()
                                       where rls.EnterpriseId == EnterpriseId && rls.CompanyID == CompanyId && rls.UserType == 3 && rls.IsArchived == false && rls.IsDeleted == false
                                       select new UserMasterDTO
                                       {
                                           CompanyID = rls.CompanyID,
                                           Created = rls.Created ?? DateTime.MinValue,
                                           CreatedBy = rls.CreatedBy,
                                           CreatedByName = rls_usrmst.UserName,
                                           Email = rls.Email,
                                           EnterpriseId = rls.EnterpriseId,
                                           GUID = rls.GUID,
                                           ID = rls.ID,
                                           IsArchived = rls.IsArchived,
                                           IsDeleted = rls.IsDeleted,
                                           LastUpdatedBy = rls.LastUpdatedBy,
                                           Password = rls.Password,
                                           Phone = rls.Phone,
                                           RoleID = rls.RoleId,
                                           Room = rls.RoomId,
                                           Updated = rls.Updated,
                                           UpdatedByName = rls_usrmstup.UserName,
                                           UserName = rls.UserName,
                                           UserType = rls.UserType ?? 0,
                                           UserTypeName = rls_utm.UserTypeName,
                                           Prefix = rls.Prefix,
                                           FirstName = rls.FirstName,
                                           MiddleName = rls.MiddleName,
                                           LastName = rls.LastName,
                                           Gender = rls.Gender,
                                           Phone2 = rls.Phone2,
                                           EmployeeNumber = rls.EmployeeNumber,
                                           CostCenter = rls.CostCenter,
                                           JobTitle = rls.JobTitle,
                                           Address = rls.Address,
                                           Country = rls.Country,
                                           State = rls.State,
                                           City = rls.City,
                                           PostalCode = rls.PostalCode,
                                           UDF1 = rls.UDF1,
                                           UDF2 = rls.UDF2,
                                           UDF3 = rls.UDF3,
                                           UDF4 = rls.UDF4,
                                           UDF5 = rls.UDF5,
                                           UDF6 = rls.UDF6,
                                           UDF7 = rls.UDF7,
                                           UDF8 = rls.UDF8,
                                           UDF9 = rls.UDF9,
                                           UDF10 = rls.UDF10
                                       }).ToList();
                    }
                }
            }
            else
            {

                if (IsArchived && IsDeleted)
                {
                    using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                    {
                        if (UserType == 1)
                        {
                            lstAllUsers1 = (from rls in context.UserMasters
                                            join usrmst in context.UserMasters on rls.CreatedBy equals usrmst.ID into rls_usrmst_join
                                            from rls_usrmst in rls_usrmst_join.DefaultIfEmpty()
                                            join usrmstup in context.UserMasters on rls.LastUpdatedBy equals usrmstup.ID into rls_usrmstup_join
                                            from rls_usrmstup in rls_usrmstup_join.DefaultIfEmpty()
                                            join utm in context.UserTypeMasters on rls.UserType equals utm.UserTypeID into rls_utm_join
                                            from rls_utm in rls_utm_join.DefaultIfEmpty()
                                            where rls.UserType == 1 && rls.IsDeleted == true && rls.IsArchived == true
                                            select new UserMasterDTO
                                            {
                                                CompanyID = rls.CompanyID,
                                                Created = rls.Created ?? DateTime.MinValue,
                                                CreatedBy = rls.CreatedBy,
                                                CreatedByName = rls_usrmst.UserName,
                                                Email = rls.Email,
                                                EnterpriseId = rls.EnterpriseId,
                                                GUID = rls.GUID,
                                                ID = rls.ID,
                                                IsArchived = rls.IsArchived,
                                                IsDeleted = rls.IsDeleted,
                                                LastUpdatedBy = rls.LastUpdatedBy,
                                                Password = rls.Password,
                                                Phone = rls.Phone,
                                                RoleID = rls.RoleId,
                                                Room = rls.RoomId,
                                                Updated = rls.Updated,
                                                UpdatedByName = rls_usrmstup.UserName,
                                                UserName = rls.UserName,
                                                UserType = rls.UserType ?? 0,
                                                UserTypeName = rls_utm.UserTypeName,
                                                Prefix = rls.Prefix,
                                                FirstName = rls.FirstName,
                                                MiddleName = rls.MiddleName,
                                                LastName = rls.LastName,
                                                Gender = rls.Gender,
                                                Phone2 = rls.Phone2,
                                                EmployeeNumber = rls.EmployeeNumber,
                                                CostCenter = rls.CostCenter,
                                                JobTitle = rls.JobTitle,
                                                Address = rls.Address,
                                                Country = rls.Country,
                                                State = rls.State,
                                                City = rls.City,
                                                PostalCode = rls.PostalCode,
                                                UDF1 = rls.UDF1,
                                                UDF2 = rls.UDF2,
                                                UDF3 = rls.UDF3,
                                                UDF4 = rls.UDF4,
                                                UDF5 = rls.UDF5,
                                                UDF6 = rls.UDF6,
                                                UDF7 = rls.UDF7,
                                                UDF8 = rls.UDF8,
                                                UDF9 = rls.UDF9,
                                                UDF10 = rls.UDF10
                                            }).ToList();

                            lstAllUsers2 = (from rls in context.UserMasters
                                            join usrmst in context.UserMasters on rls.CreatedBy equals usrmst.ID into rls_usrmst_join
                                            from rls_usrmst in rls_usrmst_join.DefaultIfEmpty()
                                            join usrmstup in context.UserMasters on rls.LastUpdatedBy equals usrmstup.ID into rls_usrmstup_join
                                            from rls_usrmstup in rls_usrmstup_join.DefaultIfEmpty()
                                            join utm in context.UserTypeMasters on rls.UserType equals utm.UserTypeID into rls_utm_join
                                            from rls_utm in rls_utm_join.DefaultIfEmpty()
                                            where rls.EnterpriseId == EnterpriseId && rls.CompanyID == CompanyId && rls.UserType != 1 && rls.IsDeleted == true && rls.IsArchived == true
                                            select new UserMasterDTO
                                            {
                                                CompanyID = rls.CompanyID,
                                                Created = rls.Created ?? DateTime.MinValue,
                                                CreatedBy = rls.CreatedBy,
                                                CreatedByName = rls_usrmst.UserName,
                                                Email = rls.Email,
                                                EnterpriseId = rls.EnterpriseId,
                                                GUID = rls.GUID,
                                                ID = rls.ID,
                                                IsArchived = rls.IsArchived,
                                                IsDeleted = rls.IsDeleted,
                                                LastUpdatedBy = rls.LastUpdatedBy,
                                                Password = rls.Password,
                                                Phone = rls.Phone,
                                                RoleID = rls.RoleId,
                                                Room = rls.RoomId,
                                                Updated = rls.Updated,
                                                UpdatedByName = rls_usrmstup.UserName,
                                                UserName = rls.UserName,
                                                UserType = rls.UserType ?? 0,
                                                UserTypeName = rls_utm.UserTypeName,
                                                Prefix = rls.Prefix,
                                                FirstName = rls.FirstName,
                                                MiddleName = rls.MiddleName,
                                                LastName = rls.LastName,
                                                Gender = rls.Gender,
                                                Phone2 = rls.Phone2,
                                                EmployeeNumber = rls.EmployeeNumber,
                                                CostCenter = rls.CostCenter,
                                                JobTitle = rls.JobTitle,
                                                Address = rls.Address,
                                                Country = rls.Country,
                                                State = rls.State,
                                                City = rls.City,
                                                PostalCode = rls.PostalCode,
                                                UDF1 = rls.UDF1,
                                                UDF2 = rls.UDF2,
                                                UDF3 = rls.UDF3,
                                                UDF4 = rls.UDF4,
                                                UDF5 = rls.UDF5,
                                                UDF6 = rls.UDF6,
                                                UDF7 = rls.UDF7,
                                                UDF8 = rls.UDF8,
                                                UDF9 = rls.UDF9,
                                                UDF10 = rls.UDF10
                                            }).ToList();

                            lstAllUsers = lstAllUsers1.Union(lstAllUsers2).ToList();
                        }
                        if (UserType == 2)
                        {
                            lstAllUsers1 = (from rls in context.UserMasters
                                            join usrmst in context.UserMasters on rls.CreatedBy equals usrmst.ID into rls_usrmst_join
                                            from rls_usrmst in rls_usrmst_join.DefaultIfEmpty()
                                            join usrmstup in context.UserMasters on rls.LastUpdatedBy equals usrmstup.ID into rls_usrmstup_join
                                            from rls_usrmstup in rls_usrmstup_join.DefaultIfEmpty()
                                            join utm in context.UserTypeMasters on rls.UserType equals utm.UserTypeID into rls_utm_join
                                            from rls_utm in rls_utm_join.DefaultIfEmpty()
                                            where rls.EnterpriseId == EnterpriseId && rls.UserType == 2 && rls.IsDeleted == true && rls.IsArchived == true
                                            select new UserMasterDTO
                                            {
                                                CompanyID = rls.CompanyID,
                                                Created = rls.Created ?? DateTime.MinValue,
                                                CreatedBy = rls.CreatedBy,
                                                CreatedByName = rls_usrmst.UserName,
                                                Email = rls.Email,
                                                EnterpriseId = rls.EnterpriseId,
                                                GUID = rls.GUID,
                                                ID = rls.ID,
                                                IsArchived = rls.IsArchived,
                                                IsDeleted = rls.IsDeleted,
                                                LastUpdatedBy = rls.LastUpdatedBy,
                                                Password = rls.Password,
                                                Phone = rls.Phone,
                                                RoleID = rls.RoleId,
                                                Room = rls.RoomId,
                                                Updated = rls.Updated,
                                                UpdatedByName = rls_usrmstup.UserName,
                                                UserName = rls.UserName,
                                                UserType = rls.UserType ?? 0,
                                                UserTypeName = rls_utm.UserTypeName,
                                                Prefix = rls.Prefix,
                                                FirstName = rls.FirstName,
                                                MiddleName = rls.MiddleName,
                                                LastName = rls.LastName,
                                                Gender = rls.Gender,
                                                Phone2 = rls.Phone2,
                                                EmployeeNumber = rls.EmployeeNumber,
                                                CostCenter = rls.CostCenter,
                                                JobTitle = rls.JobTitle,
                                                Address = rls.Address,
                                                Country = rls.Country,
                                                State = rls.State,
                                                City = rls.City,
                                                PostalCode = rls.PostalCode,
                                                UDF1 = rls.UDF1,
                                                UDF2 = rls.UDF2,
                                                UDF3 = rls.UDF3,
                                                UDF4 = rls.UDF4,
                                                UDF5 = rls.UDF5,
                                                UDF6 = rls.UDF6,
                                                UDF7 = rls.UDF7,
                                                UDF8 = rls.UDF8,
                                                UDF9 = rls.UDF9,
                                                UDF10 = rls.UDF10
                                            }).ToList();

                            lstAllUsers2 = (from rls in context.UserMasters
                                            join usrmst in context.UserMasters on rls.CreatedBy equals usrmst.ID into rls_usrmst_join
                                            from rls_usrmst in rls_usrmst_join.DefaultIfEmpty()
                                            join usrmstup in context.UserMasters on rls.LastUpdatedBy equals usrmstup.ID into rls_usrmstup_join
                                            from rls_usrmstup in rls_usrmstup_join.DefaultIfEmpty()
                                            join utm in context.UserTypeMasters on rls.UserType equals utm.UserTypeID into rls_utm_join
                                            from rls_utm in rls_utm_join.DefaultIfEmpty()
                                            where rls.EnterpriseId == EnterpriseId && rls.CompanyID == CompanyId && rls.UserType > 2 && rls.IsDeleted == true && rls.IsArchived == true
                                            select new UserMasterDTO
                                            {
                                                CompanyID = rls.CompanyID,
                                                Created = rls.Created ?? DateTime.MinValue,
                                                CreatedBy = rls.CreatedBy,
                                                CreatedByName = rls_usrmst.UserName,
                                                Email = rls.Email,
                                                EnterpriseId = rls.EnterpriseId,
                                                GUID = rls.GUID,
                                                ID = rls.ID,
                                                IsArchived = rls.IsArchived,
                                                IsDeleted = rls.IsDeleted,
                                                LastUpdatedBy = rls.LastUpdatedBy,
                                                Password = rls.Password,
                                                Phone = rls.Phone,
                                                RoleID = rls.RoleId,
                                                Room = rls.RoomId,
                                                Updated = rls.Updated,
                                                UpdatedByName = rls_usrmstup.UserName,
                                                UserName = rls.UserName,
                                                UserType = rls.UserType ?? 0,
                                                UserTypeName = rls_utm.UserTypeName,
                                                Prefix = rls.Prefix,
                                                FirstName = rls.FirstName,
                                                MiddleName = rls.MiddleName,
                                                LastName = rls.LastName,
                                                Gender = rls.Gender,
                                                Phone2 = rls.Phone2,
                                                EmployeeNumber = rls.EmployeeNumber,
                                                CostCenter = rls.CostCenter,
                                                JobTitle = rls.JobTitle,
                                                Address = rls.Address,
                                                Country = rls.Country,
                                                State = rls.State,
                                                City = rls.City,
                                                PostalCode = rls.PostalCode,
                                                UDF1 = rls.UDF1,
                                                UDF2 = rls.UDF2,
                                                UDF3 = rls.UDF3,
                                                UDF4 = rls.UDF4,
                                                UDF5 = rls.UDF5,
                                                UDF6 = rls.UDF6,
                                                UDF7 = rls.UDF7,
                                                UDF8 = rls.UDF8,
                                                UDF9 = rls.UDF9,
                                                UDF10 = rls.UDF10
                                            }).ToList();
                            lstAllUsers = lstAllUsers1.Union(lstAllUsers2).ToList();
                        }
                        if (UserType == 3)
                        {
                            lstAllUsers = (from rls in context.UserMasters
                                           join usrmst in context.UserMasters on rls.CreatedBy equals usrmst.ID into rls_usrmst_join
                                           from rls_usrmst in rls_usrmst_join.DefaultIfEmpty()
                                           join usrmstup in context.UserMasters on rls.LastUpdatedBy equals usrmstup.ID into rls_usrmstup_join
                                           from rls_usrmstup in rls_usrmstup_join.DefaultIfEmpty()
                                           join utm in context.UserTypeMasters on rls.UserType equals utm.UserTypeID into rls_utm_join
                                           from rls_utm in rls_utm_join.DefaultIfEmpty()
                                           where rls.EnterpriseId == EnterpriseId && rls.CompanyID == CompanyId && rls.UserType == 3 && rls.IsDeleted == true && rls.IsArchived == true
                                           select new UserMasterDTO
                                           {
                                               CompanyID = rls.CompanyID,
                                               Created = rls.Created ?? DateTime.MinValue,
                                               CreatedBy = rls.CreatedBy,
                                               CreatedByName = rls_usrmst.UserName,
                                               Email = rls.Email,
                                               EnterpriseId = rls.EnterpriseId,
                                               GUID = rls.GUID,
                                               ID = rls.ID,
                                               IsArchived = rls.IsArchived,
                                               IsDeleted = rls.IsDeleted,
                                               LastUpdatedBy = rls.LastUpdatedBy,
                                               Password = rls.Password,
                                               Phone = rls.Phone,
                                               RoleID = rls.RoleId,
                                               Room = rls.RoomId,
                                               Updated = rls.Updated,
                                               UpdatedByName = rls_usrmstup.UserName,
                                               UserName = rls.UserName,
                                               UserType = rls.UserType ?? 0,
                                               UserTypeName = rls_utm.UserTypeName,
                                               Prefix = rls.Prefix,
                                               FirstName = rls.FirstName,
                                               MiddleName = rls.MiddleName,
                                               LastName = rls.LastName,
                                               Gender = rls.Gender,
                                               Phone2 = rls.Phone2,
                                               EmployeeNumber = rls.EmployeeNumber,
                                               CostCenter = rls.CostCenter,
                                               JobTitle = rls.JobTitle,
                                               Address = rls.Address,
                                               Country = rls.Country,
                                               State = rls.State,
                                               City = rls.City,
                                               PostalCode = rls.PostalCode,
                                               UDF1 = rls.UDF1,
                                               UDF2 = rls.UDF2,
                                               UDF3 = rls.UDF3,
                                               UDF4 = rls.UDF4,
                                               UDF5 = rls.UDF5,
                                               UDF6 = rls.UDF6,
                                               UDF7 = rls.UDF7,
                                               UDF8 = rls.UDF8,
                                               UDF9 = rls.UDF9,
                                               UDF10 = rls.UDF10
                                           }).ToList();
                        }
                    }
                }
                else if (IsArchived)
                {
                    using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                    {
                        if (UserType == 1)
                        {
                            lstAllUsers1 = (from rls in context.UserMasters
                                            join usrmst in context.UserMasters on rls.CreatedBy equals usrmst.ID into rls_usrmst_join
                                            from rls_usrmst in rls_usrmst_join.DefaultIfEmpty()
                                            join usrmstup in context.UserMasters on rls.LastUpdatedBy equals usrmstup.ID into rls_usrmstup_join
                                            from rls_usrmstup in rls_usrmstup_join.DefaultIfEmpty()
                                            join utm in context.UserTypeMasters on rls.UserType equals utm.UserTypeID into rls_utm_join
                                            from rls_utm in rls_utm_join.DefaultIfEmpty()
                                            where rls.UserType == 1 && rls.IsArchived == true
                                            select new UserMasterDTO
                                            {
                                                CompanyID = rls.CompanyID,
                                                Created = rls.Created ?? DateTime.MinValue,
                                                CreatedBy = rls.CreatedBy,
                                                CreatedByName = rls_usrmst.UserName,
                                                Email = rls.Email,
                                                EnterpriseId = rls.EnterpriseId,
                                                GUID = rls.GUID,
                                                ID = rls.ID,
                                                IsArchived = rls.IsArchived,
                                                IsDeleted = rls.IsDeleted,
                                                LastUpdatedBy = rls.LastUpdatedBy,
                                                Password = rls.Password,
                                                Phone = rls.Phone,
                                                RoleID = rls.RoleId,
                                                Room = rls.RoomId,
                                                Updated = rls.Updated,
                                                UpdatedByName = rls_usrmstup.UserName,
                                                UserName = rls.UserName,
                                                UserType = rls.UserType ?? 0,
                                                UserTypeName = rls_utm.UserTypeName,
                                                Prefix = rls.Prefix,
                                                FirstName = rls.FirstName,
                                                MiddleName = rls.MiddleName,
                                                LastName = rls.LastName,
                                                Gender = rls.Gender,
                                                Phone2 = rls.Phone2,
                                                EmployeeNumber = rls.EmployeeNumber,
                                                CostCenter = rls.CostCenter,
                                                JobTitle = rls.JobTitle,
                                                Address = rls.Address,
                                                Country = rls.Country,
                                                State = rls.State,
                                                City = rls.City,
                                                PostalCode = rls.PostalCode,
                                                UDF1 = rls.UDF1,
                                                UDF2 = rls.UDF2,
                                                UDF3 = rls.UDF3,
                                                UDF4 = rls.UDF4,
                                                UDF5 = rls.UDF5,
                                                UDF6 = rls.UDF6,
                                                UDF7 = rls.UDF7,
                                                UDF8 = rls.UDF8,
                                                UDF9 = rls.UDF9,
                                                UDF10 = rls.UDF10
                                            }).ToList();

                            lstAllUsers2 = (from rls in context.UserMasters
                                            join usrmst in context.UserMasters on rls.CreatedBy equals usrmst.ID into rls_usrmst_join
                                            from rls_usrmst in rls_usrmst_join.DefaultIfEmpty()
                                            join usrmstup in context.UserMasters on rls.LastUpdatedBy equals usrmstup.ID into rls_usrmstup_join
                                            from rls_usrmstup in rls_usrmstup_join.DefaultIfEmpty()
                                            join utm in context.UserTypeMasters on rls.UserType equals utm.UserTypeID into rls_utm_join
                                            from rls_utm in rls_utm_join.DefaultIfEmpty()
                                            where rls.EnterpriseId == EnterpriseId && rls.CompanyID == CompanyId && rls.UserType != 1 && rls.IsArchived == true
                                            select new UserMasterDTO
                                            {
                                                CompanyID = rls.CompanyID,
                                                Created = rls.Created ?? DateTime.MinValue,
                                                CreatedBy = rls.CreatedBy,
                                                CreatedByName = rls_usrmst.UserName,
                                                Email = rls.Email,
                                                EnterpriseId = rls.EnterpriseId,
                                                GUID = rls.GUID,
                                                ID = rls.ID,
                                                IsArchived = rls.IsArchived,
                                                IsDeleted = rls.IsDeleted,
                                                LastUpdatedBy = rls.LastUpdatedBy,
                                                Password = rls.Password,
                                                Phone = rls.Phone,
                                                RoleID = rls.RoleId,
                                                Room = rls.RoomId,
                                                Updated = rls.Updated,
                                                UpdatedByName = rls_usrmstup.UserName,
                                                UserName = rls.UserName,
                                                UserType = rls.UserType ?? 0,
                                                UserTypeName = rls_utm.UserTypeName,
                                                Prefix = rls.Prefix,
                                                FirstName = rls.FirstName,
                                                MiddleName = rls.MiddleName,
                                                LastName = rls.LastName,
                                                Gender = rls.Gender,
                                                Phone2 = rls.Phone2,
                                                EmployeeNumber = rls.EmployeeNumber,
                                                CostCenter = rls.CostCenter,
                                                JobTitle = rls.JobTitle,
                                                Address = rls.Address,
                                                Country = rls.Country,
                                                State = rls.State,
                                                City = rls.City,
                                                PostalCode = rls.PostalCode,
                                                UDF1 = rls.UDF1,
                                                UDF2 = rls.UDF2,
                                                UDF3 = rls.UDF3,
                                                UDF4 = rls.UDF4,
                                                UDF5 = rls.UDF5,
                                                UDF6 = rls.UDF6,
                                                UDF7 = rls.UDF7,
                                                UDF8 = rls.UDF8,
                                                UDF9 = rls.UDF9,
                                                UDF10 = rls.UDF10
                                            }).ToList();

                            lstAllUsers = lstAllUsers1.Union(lstAllUsers2).ToList();
                        }
                        if (UserType == 2)
                        {
                            lstAllUsers1 = (from rls in context.UserMasters
                                            join usrmst in context.UserMasters on rls.CreatedBy equals usrmst.ID into rls_usrmst_join
                                            from rls_usrmst in rls_usrmst_join.DefaultIfEmpty()
                                            join usrmstup in context.UserMasters on rls.LastUpdatedBy equals usrmstup.ID into rls_usrmstup_join
                                            from rls_usrmstup in rls_usrmstup_join.DefaultIfEmpty()
                                            join utm in context.UserTypeMasters on rls.UserType equals utm.UserTypeID into rls_utm_join
                                            from rls_utm in rls_utm_join.DefaultIfEmpty()
                                            where rls.EnterpriseId == EnterpriseId && rls.UserType == 2 && rls.IsArchived == true
                                            select new UserMasterDTO
                                            {
                                                CompanyID = rls.CompanyID,
                                                Created = rls.Created ?? DateTime.MinValue,
                                                CreatedBy = rls.CreatedBy,
                                                CreatedByName = rls_usrmst.UserName,
                                                Email = rls.Email,
                                                EnterpriseId = rls.EnterpriseId,
                                                GUID = rls.GUID,
                                                ID = rls.ID,
                                                IsArchived = rls.IsArchived,
                                                IsDeleted = rls.IsDeleted,
                                                LastUpdatedBy = rls.LastUpdatedBy,
                                                Password = rls.Password,
                                                Phone = rls.Phone,
                                                RoleID = rls.RoleId,
                                                Room = rls.RoomId,
                                                Updated = rls.Updated,
                                                UpdatedByName = rls_usrmstup.UserName,
                                                UserName = rls.UserName,
                                                UserType = rls.UserType ?? 0,
                                                UserTypeName = rls_utm.UserTypeName,
                                                Prefix = rls.Prefix,
                                                FirstName = rls.FirstName,
                                                MiddleName = rls.MiddleName,
                                                LastName = rls.LastName,
                                                Gender = rls.Gender,
                                                Phone2 = rls.Phone2,
                                                EmployeeNumber = rls.EmployeeNumber,
                                                CostCenter = rls.CostCenter,
                                                JobTitle = rls.JobTitle,
                                                Address = rls.Address,
                                                Country = rls.Country,
                                                State = rls.State,
                                                City = rls.City,
                                                PostalCode = rls.PostalCode,
                                                UDF1 = rls.UDF1,
                                                UDF2 = rls.UDF2,
                                                UDF3 = rls.UDF3,
                                                UDF4 = rls.UDF4,
                                                UDF5 = rls.UDF5,
                                                UDF6 = rls.UDF6,
                                                UDF7 = rls.UDF7,
                                                UDF8 = rls.UDF8,
                                                UDF9 = rls.UDF9,
                                                UDF10 = rls.UDF10
                                            }).ToList();

                            lstAllUsers2 = (from rls in context.UserMasters
                                            join usrmst in context.UserMasters on rls.CreatedBy equals usrmst.ID into rls_usrmst_join
                                            from rls_usrmst in rls_usrmst_join.DefaultIfEmpty()
                                            join usrmstup in context.UserMasters on rls.LastUpdatedBy equals usrmstup.ID into rls_usrmstup_join
                                            from rls_usrmstup in rls_usrmstup_join.DefaultIfEmpty()
                                            join utm in context.UserTypeMasters on rls.UserType equals utm.UserTypeID into rls_utm_join
                                            from rls_utm in rls_utm_join.DefaultIfEmpty()
                                            where rls.EnterpriseId == EnterpriseId && rls.CompanyID == CompanyId && rls.UserType > 2 && rls.IsArchived == true
                                            select new UserMasterDTO
                                            {
                                                CompanyID = rls.CompanyID,
                                                Created = rls.Created ?? DateTime.MinValue,
                                                CreatedBy = rls.CreatedBy,
                                                CreatedByName = rls_usrmst.UserName,
                                                Email = rls.Email,
                                                EnterpriseId = rls.EnterpriseId,
                                                GUID = rls.GUID,
                                                ID = rls.ID,
                                                IsArchived = rls.IsArchived,
                                                IsDeleted = rls.IsDeleted,
                                                LastUpdatedBy = rls.LastUpdatedBy,
                                                Password = rls.Password,
                                                Phone = rls.Phone,
                                                RoleID = rls.RoleId,
                                                Room = rls.RoomId,
                                                Updated = rls.Updated,
                                                UpdatedByName = rls_usrmstup.UserName,
                                                UserName = rls.UserName,
                                                UserType = rls.UserType ?? 0,
                                                UserTypeName = rls_utm.UserTypeName,
                                                Prefix = rls.Prefix,
                                                FirstName = rls.FirstName,
                                                MiddleName = rls.MiddleName,
                                                LastName = rls.LastName,
                                                Gender = rls.Gender,
                                                Phone2 = rls.Phone2,
                                                EmployeeNumber = rls.EmployeeNumber,
                                                CostCenter = rls.CostCenter,
                                                JobTitle = rls.JobTitle,
                                                Address = rls.Address,
                                                Country = rls.Country,
                                                State = rls.State,
                                                City = rls.City,
                                                PostalCode = rls.PostalCode,
                                                UDF1 = rls.UDF1,
                                                UDF2 = rls.UDF2,
                                                UDF3 = rls.UDF3,
                                                UDF4 = rls.UDF4,
                                                UDF5 = rls.UDF5,
                                                UDF6 = rls.UDF6,
                                                UDF7 = rls.UDF7,
                                                UDF8 = rls.UDF8,
                                                UDF9 = rls.UDF9,
                                                UDF10 = rls.UDF10
                                            }).ToList();
                            lstAllUsers = lstAllUsers1.Union(lstAllUsers2).ToList();
                        }
                        if (UserType == 3)
                        {
                            lstAllUsers = (from rls in context.UserMasters
                                           join usrmst in context.UserMasters on rls.CreatedBy equals usrmst.ID into rls_usrmst_join
                                           from rls_usrmst in rls_usrmst_join.DefaultIfEmpty()
                                           join usrmstup in context.UserMasters on rls.LastUpdatedBy equals usrmstup.ID into rls_usrmstup_join
                                           from rls_usrmstup in rls_usrmstup_join.DefaultIfEmpty()
                                           join utm in context.UserTypeMasters on rls.UserType equals utm.UserTypeID into rls_utm_join
                                           from rls_utm in rls_utm_join.DefaultIfEmpty()
                                           where rls.EnterpriseId == EnterpriseId && rls.CompanyID == CompanyId && rls.UserType == 3 && rls.IsArchived == true
                                           select new UserMasterDTO
                                           {
                                               CompanyID = rls.CompanyID,
                                               Created = rls.Created ?? DateTime.MinValue,
                                               CreatedBy = rls.CreatedBy,
                                               CreatedByName = rls_usrmst.UserName,
                                               Email = rls.Email,
                                               EnterpriseId = rls.EnterpriseId,
                                               GUID = rls.GUID,
                                               ID = rls.ID,
                                               IsArchived = rls.IsArchived,
                                               IsDeleted = rls.IsDeleted,
                                               LastUpdatedBy = rls.LastUpdatedBy,
                                               Password = rls.Password,
                                               Phone = rls.Phone,
                                               RoleID = rls.RoleId,
                                               Room = rls.RoomId,
                                               Updated = rls.Updated,
                                               UpdatedByName = rls_usrmstup.UserName,
                                               UserName = rls.UserName,
                                               UserType = rls.UserType ?? 0,
                                               UserTypeName = rls_utm.UserTypeName,
                                               Prefix = rls.Prefix,
                                               FirstName = rls.FirstName,
                                               MiddleName = rls.MiddleName,
                                               LastName = rls.LastName,
                                               Gender = rls.Gender,
                                               Phone2 = rls.Phone2,
                                               EmployeeNumber = rls.EmployeeNumber,
                                               CostCenter = rls.CostCenter,
                                               JobTitle = rls.JobTitle,
                                               Address = rls.Address,
                                               Country = rls.Country,
                                               State = rls.State,
                                               City = rls.City,
                                               PostalCode = rls.PostalCode,
                                               UDF1 = rls.UDF1,
                                               UDF2 = rls.UDF2,
                                               UDF3 = rls.UDF3,
                                               UDF4 = rls.UDF4,
                                               UDF5 = rls.UDF5,
                                               UDF6 = rls.UDF6,
                                               UDF7 = rls.UDF7,
                                               UDF8 = rls.UDF8,
                                               UDF9 = rls.UDF9,
                                               UDF10 = rls.UDF10
                                           }).ToList();
                        }
                    }
                }
                else if (IsDeleted)
                {
                    using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                    {
                        if (UserType == 1)
                        {
                            lstAllUsers1 = (from rls in context.UserMasters
                                            join usrmst in context.UserMasters on rls.CreatedBy equals usrmst.ID into rls_usrmst_join
                                            from rls_usrmst in rls_usrmst_join.DefaultIfEmpty()
                                            join usrmstup in context.UserMasters on rls.LastUpdatedBy equals usrmstup.ID into rls_usrmstup_join
                                            from rls_usrmstup in rls_usrmstup_join.DefaultIfEmpty()
                                            join utm in context.UserTypeMasters on rls.UserType equals utm.UserTypeID into rls_utm_join
                                            from rls_utm in rls_utm_join.DefaultIfEmpty()
                                            where rls.UserType == 1 && rls.IsDeleted == true
                                            select new UserMasterDTO
                                            {
                                                CompanyID = rls.CompanyID,
                                                Created = rls.Created ?? DateTime.MinValue,
                                                CreatedBy = rls.CreatedBy,
                                                CreatedByName = rls_usrmst.UserName,
                                                Email = rls.Email,
                                                EnterpriseId = rls.EnterpriseId,
                                                GUID = rls.GUID,
                                                ID = rls.ID,
                                                IsArchived = rls.IsArchived,
                                                IsDeleted = rls.IsDeleted,
                                                LastUpdatedBy = rls.LastUpdatedBy,
                                                Password = rls.Password,
                                                Phone = rls.Phone,
                                                RoleID = rls.RoleId,
                                                Room = rls.RoomId,
                                                Updated = rls.Updated,
                                                UpdatedByName = rls_usrmstup.UserName,
                                                UserName = rls.UserName,
                                                UserType = rls.UserType ?? 0,
                                                UserTypeName = rls_utm.UserTypeName,
                                                Prefix = rls.Prefix,
                                                FirstName = rls.FirstName,
                                                MiddleName = rls.MiddleName,
                                                LastName = rls.LastName,
                                                Gender = rls.Gender,
                                                Phone2 = rls.Phone2,
                                                EmployeeNumber = rls.EmployeeNumber,
                                                CostCenter = rls.CostCenter,
                                                JobTitle = rls.JobTitle,
                                                Address = rls.Address,
                                                Country = rls.Country,
                                                State = rls.State,
                                                City = rls.City,
                                                PostalCode = rls.PostalCode,
                                                UDF1 = rls.UDF1,
                                                UDF2 = rls.UDF2,
                                                UDF3 = rls.UDF3,
                                                UDF4 = rls.UDF4,
                                                UDF5 = rls.UDF5,
                                                UDF6 = rls.UDF6,
                                                UDF7 = rls.UDF7,
                                                UDF8 = rls.UDF8,
                                                UDF9 = rls.UDF9,
                                                UDF10 = rls.UDF10
                                            }).ToList();

                            lstAllUsers2 = (from rls in context.UserMasters
                                            join usrmst in context.UserMasters on rls.CreatedBy equals usrmst.ID into rls_usrmst_join
                                            from rls_usrmst in rls_usrmst_join.DefaultIfEmpty()
                                            join usrmstup in context.UserMasters on rls.LastUpdatedBy equals usrmstup.ID into rls_usrmstup_join
                                            from rls_usrmstup in rls_usrmstup_join.DefaultIfEmpty()
                                            join utm in context.UserTypeMasters on rls.UserType equals utm.UserTypeID into rls_utm_join
                                            from rls_utm in rls_utm_join.DefaultIfEmpty()
                                            where rls.EnterpriseId == EnterpriseId && rls.CompanyID == CompanyId && rls.UserType != 1 && rls.IsDeleted == true
                                            select new UserMasterDTO
                                            {
                                                CompanyID = rls.CompanyID,
                                                Created = rls.Created ?? DateTime.MinValue,
                                                CreatedBy = rls.CreatedBy,
                                                CreatedByName = rls_usrmst.UserName,
                                                Email = rls.Email,
                                                EnterpriseId = rls.EnterpriseId,
                                                GUID = rls.GUID,
                                                ID = rls.ID,
                                                IsArchived = rls.IsArchived,
                                                IsDeleted = rls.IsDeleted,
                                                LastUpdatedBy = rls.LastUpdatedBy,
                                                Password = rls.Password,
                                                Phone = rls.Phone,
                                                RoleID = rls.RoleId,
                                                Room = rls.RoomId,
                                                Updated = rls.Updated,
                                                UpdatedByName = rls_usrmstup.UserName,
                                                UserName = rls.UserName,
                                                UserType = rls.UserType ?? 0,
                                                UserTypeName = rls_utm.UserTypeName,
                                                Prefix = rls.Prefix,
                                                FirstName = rls.FirstName,
                                                MiddleName = rls.MiddleName,
                                                LastName = rls.LastName,
                                                Gender = rls.Gender,
                                                Phone2 = rls.Phone2,
                                                EmployeeNumber = rls.EmployeeNumber,
                                                CostCenter = rls.CostCenter,
                                                JobTitle = rls.JobTitle,
                                                Address = rls.Address,
                                                Country = rls.Country,
                                                State = rls.State,
                                                City = rls.City,
                                                PostalCode = rls.PostalCode,
                                                UDF1 = rls.UDF1,
                                                UDF2 = rls.UDF2,
                                                UDF3 = rls.UDF3,
                                                UDF4 = rls.UDF4,
                                                UDF5 = rls.UDF5,
                                                UDF6 = rls.UDF6,
                                                UDF7 = rls.UDF7,
                                                UDF8 = rls.UDF8,
                                                UDF9 = rls.UDF9,
                                                UDF10 = rls.UDF10
                                            }).ToList();

                            lstAllUsers = lstAllUsers1.Union(lstAllUsers2).ToList();
                        }
                        if (UserType == 2)
                        {
                            lstAllUsers1 = (from rls in context.UserMasters
                                            join usrmst in context.UserMasters on rls.CreatedBy equals usrmst.ID into rls_usrmst_join
                                            from rls_usrmst in rls_usrmst_join.DefaultIfEmpty()
                                            join usrmstup in context.UserMasters on rls.LastUpdatedBy equals usrmstup.ID into rls_usrmstup_join
                                            from rls_usrmstup in rls_usrmstup_join.DefaultIfEmpty()
                                            join utm in context.UserTypeMasters on rls.UserType equals utm.UserTypeID into rls_utm_join
                                            from rls_utm in rls_utm_join.DefaultIfEmpty()
                                            where rls.EnterpriseId == EnterpriseId && rls.UserType == 2 && rls.IsDeleted == true
                                            select new UserMasterDTO
                                            {
                                                CompanyID = rls.CompanyID,
                                                Created = rls.Created ?? DateTime.MinValue,
                                                CreatedBy = rls.CreatedBy,
                                                CreatedByName = rls_usrmst.UserName,
                                                Email = rls.Email,
                                                EnterpriseId = rls.EnterpriseId,
                                                GUID = rls.GUID,
                                                ID = rls.ID,
                                                IsArchived = rls.IsArchived,
                                                IsDeleted = rls.IsDeleted,
                                                LastUpdatedBy = rls.LastUpdatedBy,
                                                Password = rls.Password,
                                                Phone = rls.Phone,
                                                RoleID = rls.RoleId,
                                                Room = rls.RoomId,
                                                Updated = rls.Updated,
                                                UpdatedByName = rls_usrmstup.UserName,
                                                UserName = rls.UserName,
                                                UserType = rls.UserType ?? 0,
                                                UserTypeName = rls_utm.UserTypeName,
                                                Prefix = rls.Prefix,
                                                FirstName = rls.FirstName,
                                                MiddleName = rls.MiddleName,
                                                LastName = rls.LastName,
                                                Gender = rls.Gender,
                                                Phone2 = rls.Phone2,
                                                EmployeeNumber = rls.EmployeeNumber,
                                                CostCenter = rls.CostCenter,
                                                JobTitle = rls.JobTitle,
                                                Address = rls.Address,
                                                Country = rls.Country,
                                                State = rls.State,
                                                City = rls.City,
                                                PostalCode = rls.PostalCode,
                                                UDF1 = rls.UDF1,
                                                UDF2 = rls.UDF2,
                                                UDF3 = rls.UDF3,
                                                UDF4 = rls.UDF4,
                                                UDF5 = rls.UDF5,
                                                UDF6 = rls.UDF6,
                                                UDF7 = rls.UDF7,
                                                UDF8 = rls.UDF8,
                                                UDF9 = rls.UDF9,
                                                UDF10 = rls.UDF10
                                            }).ToList();

                            lstAllUsers2 = (from rls in context.UserMasters
                                            join usrmst in context.UserMasters on rls.CreatedBy equals usrmst.ID into rls_usrmst_join
                                            from rls_usrmst in rls_usrmst_join.DefaultIfEmpty()
                                            join usrmstup in context.UserMasters on rls.LastUpdatedBy equals usrmstup.ID into rls_usrmstup_join
                                            from rls_usrmstup in rls_usrmstup_join.DefaultIfEmpty()
                                            join utm in context.UserTypeMasters on rls.UserType equals utm.UserTypeID into rls_utm_join
                                            from rls_utm in rls_utm_join.DefaultIfEmpty()
                                            where rls.EnterpriseId == EnterpriseId && rls.CompanyID == CompanyId && rls.UserType > 2 && rls.IsDeleted == true
                                            select new UserMasterDTO
                                            {
                                                CompanyID = rls.CompanyID,
                                                Created = rls.Created ?? DateTime.MinValue,
                                                CreatedBy = rls.CreatedBy,
                                                CreatedByName = rls_usrmst.UserName,
                                                Email = rls.Email,
                                                EnterpriseId = rls.EnterpriseId,
                                                GUID = rls.GUID,
                                                ID = rls.ID,
                                                IsArchived = rls.IsArchived,
                                                IsDeleted = rls.IsDeleted,
                                                LastUpdatedBy = rls.LastUpdatedBy,
                                                Password = rls.Password,
                                                Phone = rls.Phone,
                                                RoleID = rls.RoleId,
                                                Room = rls.RoomId,
                                                Updated = rls.Updated,
                                                UpdatedByName = rls_usrmstup.UserName,
                                                UserName = rls.UserName,
                                                UserType = rls.UserType ?? 0,
                                                UserTypeName = rls_utm.UserTypeName,
                                                Prefix = rls.Prefix,
                                                FirstName = rls.FirstName,
                                                MiddleName = rls.MiddleName,
                                                LastName = rls.LastName,
                                                Gender = rls.Gender,
                                                Phone2 = rls.Phone2,
                                                EmployeeNumber = rls.EmployeeNumber,
                                                CostCenter = rls.CostCenter,
                                                JobTitle = rls.JobTitle,
                                                Address = rls.Address,
                                                Country = rls.Country,
                                                State = rls.State,
                                                City = rls.City,
                                                PostalCode = rls.PostalCode,
                                                UDF1 = rls.UDF1,
                                                UDF2 = rls.UDF2,
                                                UDF3 = rls.UDF3,
                                                UDF4 = rls.UDF4,
                                                UDF5 = rls.UDF5,
                                                UDF6 = rls.UDF6,
                                                UDF7 = rls.UDF7,
                                                UDF8 = rls.UDF8,
                                                UDF9 = rls.UDF9,
                                                UDF10 = rls.UDF10
                                            }).ToList();
                            lstAllUsers = lstAllUsers1.Union(lstAllUsers2).ToList();
                        }
                        if (UserType == 3)
                        {
                            lstAllUsers = (from rls in context.UserMasters
                                           join usrmst in context.UserMasters on rls.CreatedBy equals usrmst.ID into rls_usrmst_join
                                           from rls_usrmst in rls_usrmst_join.DefaultIfEmpty()
                                           join usrmstup in context.UserMasters on rls.LastUpdatedBy equals usrmstup.ID into rls_usrmstup_join
                                           from rls_usrmstup in rls_usrmstup_join.DefaultIfEmpty()
                                           join utm in context.UserTypeMasters on rls.UserType equals utm.UserTypeID into rls_utm_join
                                           from rls_utm in rls_utm_join.DefaultIfEmpty()
                                           where rls.EnterpriseId == EnterpriseId && rls.CompanyID == CompanyId && rls.UserType == 3 && rls.IsDeleted == true
                                           select new UserMasterDTO
                                           {
                                               CompanyID = rls.CompanyID,
                                               Created = rls.Created ?? DateTime.MinValue,
                                               CreatedBy = rls.CreatedBy,
                                               CreatedByName = rls_usrmst.UserName,
                                               Email = rls.Email,
                                               EnterpriseId = rls.EnterpriseId,
                                               GUID = rls.GUID,
                                               ID = rls.ID,
                                               IsArchived = rls.IsArchived,
                                               IsDeleted = rls.IsDeleted,
                                               LastUpdatedBy = rls.LastUpdatedBy,
                                               Password = rls.Password,
                                               Phone = rls.Phone,
                                               RoleID = rls.RoleId,
                                               Room = rls.RoomId,
                                               Updated = rls.Updated,
                                               UpdatedByName = rls_usrmstup.UserName,
                                               UserName = rls.UserName,
                                               UserType = rls.UserType ?? 0,
                                               UserTypeName = rls_utm.UserTypeName,
                                               Prefix = rls.Prefix,
                                               FirstName = rls.FirstName,
                                               MiddleName = rls.MiddleName,
                                               LastName = rls.LastName,
                                               Gender = rls.Gender,
                                               Phone2 = rls.Phone2,
                                               EmployeeNumber = rls.EmployeeNumber,
                                               CostCenter = rls.CostCenter,
                                               JobTitle = rls.JobTitle,
                                               Address = rls.Address,
                                               Country = rls.Country,
                                               State = rls.State,
                                               City = rls.City,
                                               PostalCode = rls.PostalCode,
                                               UDF1 = rls.UDF1,
                                               UDF2 = rls.UDF2,
                                               UDF3 = rls.UDF3,
                                               UDF4 = rls.UDF4,
                                               UDF5 = rls.UDF5,
                                               UDF6 = rls.UDF6,
                                               UDF7 = rls.UDF7,
                                               UDF8 = rls.UDF8,
                                               UDF9 = rls.UDF9,
                                               UDF10 = rls.UDF10
                                           }).ToList();
                        }
                    }
                }


            }


            return lstAllUsers.Where(t => t.ID != LoggedinUserId).ToList();
        }

        public IEnumerable<UserMasterDTO> GetPagedUsers(int UserType, Int64 EnterpriseId, Int64 RoomID, Int64 CompanyID, Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount,
                                                          string SearchTerm, string sortColumnName, bool IsArchived, bool IsDeleted, long LoggedInUserId)
        {
            //Get Cached-Media
            IEnumerable<UserMasterDTO> ObjCache = GetAllUsers(UserType, EnterpriseId, CompanyID, RoomID, IsArchived, IsDeleted, LoggedInUserId).AsEnumerable<UserMasterDTO>();
            //IEnumerable<UserMasterDTO> ObjGlobalCache = ObjCache;
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
                //IEnumerable<CartItemDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

                ObjCache = ObjCache.Where(t =>
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedBy.ToString())))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.LastUpdatedBy.ToString())))
                    && ((Fields[1].Split('@')[2] == "") || (t.Created.Date >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created.Date <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[3] == "") || (t.Updated.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.Updated.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date)));

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                ObjCache = ObjCache.Where(t => t.UserName.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.RoleName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UserTypeName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0);
                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
        }

        public List<EnterpriseDTO> GetActiveEnterprises(long[] arrID)
        {
            List<EnterpriseDTO> lstRetList = new List<EnterpriseDTO>();
            if (arrID != null && arrID.Length > 0)
            {
                using (var contect = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    lstRetList = (from em in contect.EnterpriseMasters
                                  where arrID.Contains(em.ID) && em.IsActive == true && em.IsDeleted == false
                                  select new EnterpriseDTO
                                  {
                                      ID = em.ID,
                                      Name = em.Name
                                  }).OrderBy(t => t.Name).ToList();
                }
            }
            return lstRetList;
        }

        public List<UserLoginHistoryDTO> GetUserActionDetails(long UserId)
        {
            List<UserLoginHistoryDTO> lstUserLoginHistory = new List<UserLoginHistoryDTO>();
            //string MasterDbConnectionString = ConfigurationManager.ConnectionStrings["eTurnsMasterDbConnection"].ConnectionString.ToString();
            string MasterDbConnectionString = MasterDbConnectionHelper.GeteTurnsMasterSQLConnectionString(MasterDbConnectionHelper.GetETurnsMasterDBName(), MasterDbConnectionType.GeneralReadWrite.ToString("F"));
            SqlConnection EturnsMasterConnection = new SqlConnection(MasterDbConnectionString);
            DataSet dsHistory = new DataSet();
            dsHistory = SqlHelper.ExecuteDataset(EturnsMasterConnection, "GetUserActionDetails", UserId);
            if (dsHistory != null && dsHistory.Tables.Count > 0)
            {
                DataTable dtHistory = dsHistory.Tables[0];
                if (dtHistory != null && dtHistory.Rows.Count > 0)
                {
                    lstUserLoginHistory = (from ulh in dtHistory.AsEnumerable()
                                           select new UserLoginHistoryDTO
                                           {
                                               CompanyId = ulh.Field<long>("CompanyId"),
                                               EnterpriseId = ulh.Field<long>("EnterpriseId"),
                                               EventDate = ulh.Field<DateTime>("EventDate"),
                                               EventType = ulh.Field<byte>("EventType"),
                                               ID = ulh.Field<long>("ID"),
                                               IpAddress = ulh.Field<string>("IpAddress"),
                                               RoomId = ulh.Field<long>("RoomId"),
                                               UserId = ulh.Field<long>("UserId")
                                           }).ToList();
                }
            }
            return lstUserLoginHistory;
        }

        public Int64 SetEnterpriseSuperAdmin(EnterpriseDTO objDTO)
        {
            Int64 EnterpriseSuperadmin = 0;
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                UserMaster objCurrentEnterpriseSU = context.UserMasters.FirstOrDefault(t => t.RoleId == -2 && t.UserType == 2 && t.IsDeleted == false && t.EnterpriseId == objDTO.ID);
                if (objCurrentEnterpriseSU != null && objDTO.UserName != objCurrentEnterpriseSU.UserName)
                {
                    objCurrentEnterpriseSU.IsDeleted = true;
                    objCurrentEnterpriseSU.RoleId = 0;
                    objCurrentEnterpriseSU.UserType = 3;

                    UserMaster ExistingUser = context.UserMasters.FirstOrDefault(t => t.UserName == objDTO.UserName && t.IsDeleted == false && t.EnterpriseId == objDTO.ID);
                    if (ExistingUser != null)
                    {
                        ExistingUser.RoleId = -2;
                        ExistingUser.UserType = 2;
                        ExistingUser.RoomId = 0;
                        ExistingUser.CompanyID = 0;
                        if (objDTO.IsPasswordChanged == "1")
                        {
                            ExistingUser.Password = objDTO.EnterpriseUserPassword;
                        }
                        EnterpriseMaster objEnterpriseMaster = context.EnterpriseMasters.FirstOrDefault(t => t.ID == objDTO.ID);
                        objEnterpriseMaster.EnterpriseSuperAdmin = ExistingUser.ID;
                        EnterpriseSuperadmin = ExistingUser.ID;
                    }
                    else
                    {
                        //ExistingUser = new UserMaster();
                        //ExistingUser.CompanyID = 0;
                        //ExistingUser.Created = DateTimeUtility.DateTimeNow;
                        //ExistingUser.CreatedBy = objDTO.CreatedBy;
                        //ExistingUser.Email = objDTO.EnterpriseUserEmail;
                        //ExistingUser.IsArchived = false;
                        //ExistingUser.IsDeleted = false;

                        //ExistingUser.IsLicenceAccepted = false;
                        //ExistingUser.IsLocked = false;
                        //ExistingUser.LastUpdatedBy = objDTO.LastUpdatedBy;
                        //ExistingUser.Password = objDTO.EnterpriseUserPassword;
                        //ExistingUser.RoleId = -2;
                        //ExistingUser.Room = 0;
                        //ExistingUser.GUID = Guid.NewGuid();
                        //ExistingUser.Updated = DateTimeUtility.DateTimeNow;
                        //ExistingUser.UserName = objDTO.UserName;
                        //ExistingUser.UserType = 2;
                        //context.UserMasters.AddObject(ExistingUser);
                        //context.SaveChanges();
                    }
                    context.SaveChanges();


                }
            }
            return EnterpriseSuperadmin;
        }

        public UserMasterDTO GetUserByNameAndEnterpriseID(string UserName, long EnterpriseID)
        {
            UserMasterDTO objresult = new UserMasterDTO();
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                objresult = (from u in context.ExecuteStoreQuery<UserMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, A.GUID FROM UserMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.EnterpriseId = " + EnterpriseID + "  AND A.UserName='" + UserName + "'")
                             select new UserMasterDTO
                             {
                                 ID = u.ID,
                                 GUID = u.GUID,
                                 UserName = u.UserName,
                                 Password = u.Password,

                                 Phone = u.Phone,
                                 RoleID = u.RoleID,
                                 Email = u.Email,
                                 IsDeleted = u.IsDeleted,
                                 IsArchived = u.IsArchived,
                                 CompanyID = u.CompanyID,
                                 UserType = u.UserType,
                                 Created = u.Created,
                                 Updated = u.Updated,
                                 CreatedByName = u.CreatedByName,
                                 UpdatedByName = u.UpdatedByName,
                                 LastUpdatedBy = u.LastUpdatedBy,
                                 CreatedBy = u.CreatedBy,
                                 IsLicenceAccepted = u.IsLicenceAccepted,
                                 Prefix = u.Prefix,
                                 FirstName = u.FirstName,
                                 MiddleName = u.MiddleName,
                                 LastName = u.LastName,
                                 Gender = u.Gender,
                                 Phone2 = u.Phone2,
                                 EmployeeNumber = u.EmployeeNumber,
                                 CostCenter = u.CostCenter,
                                 JobTitle = u.JobTitle,
                                 Address = u.Address,
                                 Country = u.Country,
                                 State = u.State,
                                 City = u.City,
                                 PostalCode = u.PostalCode,
                                 UDF1 = u.UDF1,
                                 UDF2 = u.UDF2,
                                 UDF3 = u.UDF3,
                                 UDF4 = u.UDF4,
                                 UDF5 = u.UDF5,
                                 UDF6 = u.UDF6,
                                 UDF7 = u.UDF7,
                                 UDF8 = u.UDF8,
                                 UDF9 = u.UDF9,
                                 UDF10 = u.UDF10,
                                 EnterpriseId = u.EnterpriseId,
                             }).FirstOrDefault();
            }
            return objresult;

        }

        public List<UserAccessDTO> GetUserRoomAccess(long UserId)
        {
            List<UserAccessDTO> lstAccess = new List<UserAccessDTO>();
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                lstAccess = (from ura in context.UserRoomAccesses
                             where ura.UserId == UserId
                             select new UserAccessDTO
                             {
                                 CompanyId = ura.CompanyId,
                                 CompanyName = string.Empty,
                                 EnterpriseId = ura.EnterpriseId,
                                 EnterpriseName = string.Empty,
                                 ID = ura.ID,
                                 RoleId = ura.RoleId,
                                 RoomId = ura.RoomId,
                                 RoomName = string.Empty,
                                 UserId = ura.UserId
                             }).ToList();
            }
            return lstAccess;

        }
        public List<UserAccessDTO> GetRoleRoomAccess(long RoleId)
        {
            List<UserAccessDTO> lstAccess = new List<UserAccessDTO>();
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                lstAccess = (from ura in context.RoleRoomAccesses
                             where ura.RoleId == RoleId
                             select new UserAccessDTO
                             {
                                 CompanyId = ura.CompanyId,
                                 CompanyName = string.Empty,
                                 EnterpriseId = ura.EnterpriseId,
                                 EnterpriseName = string.Empty,
                                 ID = ura.ID,
                                 RoleId = ura.RoleId,
                                 RoomId = ura.RoomId,
                                 RoomName = string.Empty
                             }).ToList();
            }
            return lstAccess;

        }

        public UserMasterDTO GetUserByID(long UserID)
        {
            UserMasterDTO objresult = new UserMasterDTO();
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                //                objresult = (from u in context.ExecuteStoreQuery<UserMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, A.GUID,us.RedirectURL 
                //,min(LicenceAcceptDate) as FirstLicenceAccept
                //,max(LicenceAcceptDate) as LastLicenceAccept
                //,count(la1.ID) as Acceptcount 
                //FROM UserMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID  
                //LEFT OUTER join UserSetting as Us on A.id = Us.UserId 
                //left outer join dbo.UserLicenceAccept as la1 on la1.userid=A.id " +
                //"WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.ID=" + UserID.ToString() + " group by A.ID,A.UserName,A.Password,A.Phone,A.Email,A.RoleId,A.SyncMins,A.SyncDailyTime" +
                //",A.Created,A.Updated,A.CreatedBy,A.LastUpdatedBy,A.IsDeleted,A.GUID,A.IsArchived,A.RoomId" +
                //",A.CompanyID,A.EnterpriseId,A.IsEnterPriseUser,A.UserType,A.IsLicenceAccepted,A.IsLocked" +
                //",B.UserName,C.UserName,us.RedirectURL")
                objresult = (from u in context.ExecuteStoreQuery<UserMasterDTO>(@";with Userlist as (
                                                                SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName,us.RedirectURL 
                                                                FROM UserMaster A 
                                                                LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                                                                LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID  
                                                                LEFT OUTER join UserSetting as Us on A.id = Us.UserId 

                                                                WHERE A.IsDeleted!=1 AND A.IsArchived != 1   )

                                                                ,Useraccept as
                                                                (
                                                                select min( LicenceAcceptDate)   as FirstLicenceAccept,
                                                                 max( LicenceAcceptDate)    as LastLicenceAccept
                                                                , isnull(count(1),0)   as Acceptcount,UA.UserID 
                                                                from dbo.[UserLicenceAccept]  as UA
                                                                inner join UserMaster U on U.ID = Ua.UserID
                                                                group by UA.UserID
                                                                )
                                                                SELECT UL.*,UA.FirstLicenceAccept,UA.LastLicenceAccept,isnull(UA.Acceptcount,0) as Acceptcount
                                                                FROM Userlist as UL
                                                                left JOIN Useraccept as UA
                                                                on Ul.id= UA.UserID
                                                                where  UL.ID=" + UserID.ToString())
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
                                 IsDeleted = u.IsDeleted,
                                 IsArchived = u.IsArchived,
                                 CompanyID = u.CompanyID,
                                 UserType = u.UserType,
                                 Created = u.Created,
                                 Updated = u.Updated,
                                 CreatedByName = u.CreatedByName,
                                 UpdatedByName = u.UpdatedByName,
                                 LastUpdatedBy = u.LastUpdatedBy,
                                 CreatedBy = u.CreatedBy,
                                 RedirectURL = u.RedirectURL,
                                 FirstLicenceAccept = u.FirstLicenceAccept,
                                 LastLicenceAccept = u.LastLicenceAccept,
                                 Acceptcount = u.Acceptcount,
                                 IseTurnsAdmin = u.IseTurnsAdmin,
                                 Prefix = u.Prefix,
                                 FirstName = u.FirstName,
                                 MiddleName = u.MiddleName,
                                 LastName = u.LastName,
                                 Gender = u.Gender,
                                 Phone2 = u.Phone2,
                                 EmployeeNumber = u.EmployeeNumber,
                                 CostCenter = u.CostCenter,
                                 JobTitle = u.JobTitle,
                                 Address = u.Address,
                                 Country = u.Country,
                                 State = u.State,
                                 City = u.City,
                                 PostalCode = u.PostalCode,
                                 UDF1 = u.UDF1,
                                 UDF2 = u.UDF2,
                                 UDF3 = u.UDF3,
                                 UDF4 = u.UDF4,
                                 UDF5 = u.UDF5,
                                 UDF6 = u.UDF6,
                                 UDF7 = u.UDF7,
                                 UDF8 = u.UDF8,
                                 UDF9 = u.UDF9,
                                 UDF10 = u.UDF10
                             }).SingleOrDefault();
            }
            return objresult;

        }

        public bool HideMessage(bool isCheked, int userid)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                UserPasswordChangeHistory objUserPasswordChangeHistory = context.UserPasswordChangeHistories.OrderByDescending(t => t.Id).FirstOrDefault(t => t.UserId == userid);
                if (objUserPasswordChangeHistory != null)
                {
                    objUserPasswordChangeHistory.IsCheked = isCheked;
                }
                context.SaveChanges();
                return true;
            }
        }

        public UserMasterDTO GetUserByName(string UserName)
        {
            UserMasterDTO objresult = new UserMasterDTO();
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                objresult = (from u in context.ExecuteStoreQuery<UserMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, A.GUID FROM UserMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.UserName='" + UserName + "'")
                             select new UserMasterDTO
                             {
                                 ID = u.ID,
                                 GUID = u.GUID,
                                 UserName = u.UserName,
                                 Password = u.Password,

                                 Phone = u.Phone,
                                 RoleID = u.RoleID,
                                 Email = u.Email,
                                 IsDeleted = u.IsDeleted,
                                 IsArchived = u.IsArchived,
                                 CompanyID = u.CompanyID,
                                 UserType = u.UserType,
                                 Created = u.Created,
                                 Updated = u.Updated,
                                 CreatedByName = u.CreatedByName,
                                 UpdatedByName = u.UpdatedByName,
                                 LastUpdatedBy = u.LastUpdatedBy,
                                 CreatedBy = u.CreatedBy,
                                 IsLicenceAccepted = u.IsLicenceAccepted,
                                 Prefix = u.Prefix,
                                 FirstName = u.FirstName,
                                 MiddleName = u.MiddleName,
                                 LastName = u.LastName,
                                 Gender = u.Gender,
                                 Phone2 = u.Phone2,
                                 EmployeeNumber = u.EmployeeNumber,
                                 CostCenter = u.CostCenter,
                                 JobTitle = u.JobTitle,
                                 Address = u.Address,
                                 Country = u.Country,
                                 State = u.State,
                                 City = u.City,
                                 PostalCode = u.PostalCode,
                                 UDF1 = u.UDF1,
                                 UDF2 = u.UDF2,
                                 UDF3 = u.UDF3,
                                 UDF4 = u.UDF4,
                                 UDF5 = u.UDF5,
                                 UDF6 = u.UDF6,
                                 UDF7 = u.UDF7,
                                 UDF8 = u.UDF8,
                                 UDF9 = u.UDF9,
                                 UDF10 = u.UDF10,
                                 EnterpriseId = u.EnterpriseId,
                             }).FirstOrDefault();
            }
            return objresult;

        }

        public void SaveUserSetting(UserSettingDTO objDTO)
        {
            try
            {
                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    UserMaster objUserMaster = context.UserMasters.FirstOrDefault(t => t.ID == objDTO.UserId);
                    UserSetting objUserSetting = context.UserSettings.FirstOrDefault(t => t.UserId == objDTO.UserId);
                    if (objUserSetting != null)
                    {
                        objUserSetting.RedirectURL = objDTO.RedirectURL;
                        objUserSetting.IsRequistionReportDisplay = objDTO.IsRequistionReportDisplay;
                        objUserSetting.SearchPattern = objDTO.SearchPattern;
                    }
                    context.SaveChanges();
                }
            }
            catch
            {

            }
        }

        public List<UserRoleModuleDetailsDTO> GetUserRoleModuleByIDRecord(Int64 UserID, Int64 RoleID, long UserType, Int64 RoomId, Int64 CompanyID, string ModuleId)
        {
            UserRoleModuleDetailsDTO objresult = new UserRoleModuleDetailsDTO();
            List<UserRoleModuleDetailsDTO> lstUserRoleModuleDetails = new List<UserRoleModuleDetailsDTO>();
            string qry = "";
            List<long> lstmodulesids = new List<long>();
            if (!string.IsNullOrWhiteSpace(ModuleId))
            {
                string[] arrm = ModuleId.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (arrm != null && arrm.Count() > 0)
                {
                    foreach (var item in arrm)
                    {
                        lstmodulesids.Add(Convert.ToInt64(item));
                    }
                }


            }

            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                if (UserType == 1)
                {
                    if (RoleID == -1)
                    {
                        lstUserRoleModuleDetails = (from u in context.ModuleMasters
                                                    where lstmodulesids.Contains(u.ID)
                                                    select new UserRoleModuleDetailsDTO
                                                    {
                                                        ID = u.ID,
                                                        RoleID = -1,
                                                        UserID = UserID,
                                                        ModuleID = u.ID,
                                                        ModuleName = u.ModuleName,
                                                        IsInsert = true,
                                                        IsUpdate = true,
                                                        IsDelete = true,
                                                        IsView = true,
                                                        IsChecked = true,
                                                        ShowDeleted = true,
                                                        ShowArchived = true,
                                                        ShowUDF = true,
                                                        IsModule = u.IsModule,
                                                        ModuleValue = u.Value,
                                                        GroupId = u.GroupID,
                                                        ParentID = u.ParentID,
                                                        ModuleURL = u.Value,
                                                        ImageName = u.ImageName,
                                                        ShowChangeLog = true
                                                    }).ToList();
                    }
                    else
                    {
                        if (UserID > 0)
                            qry = @"SELECT M.ID as ModuleID,M.DisplayName as ModuleName,M.IsModule,M.GroupID,ISNULL(A.ID,0) as ID ,ISnull(A.RoleID,0) as RoleID,ISnull(A.EnterpriseId,0) as EnteriseId,ISnull(A.CompanyId,0) as CompanyId,ISnull(A.RoomId,0) as RoomId ,ISNULL(A.IsInsert,0) as IsInsert,ISNULL(A.IsUpdate,0)  as IsUpdate,ISNULL(A.ISDelete,0) as ISDelete ,ISNULL(A.IsView,0) as IsView ,ISNULL(A.IsChecked,0) as IsChecked,ModuleValue,M.ParentID,M.Value as ModuleURL,M.ImageName as ImageName,ISNULL(A.ShowDeleted,0) as ShowDeleted,ISNULL(A.ShowArchived,0) as ShowArchived,ISNULL(A.ShowUDF,0) as ShowUDF,M.DisplayOrderNumber,ISNULL(A.ShowChangeLog,0) as ShowChangeLog FROM ModuleMaster M LEFT OUTER  JOIN UserRoleDetails A ON M.ID = A.ModuleID AND A.RoleId=" + RoleID + " and A.UserID=" + UserID + " WHERE ISNULL(M.IsDeleted,0) = 0 and A.ModuleID in (" + ModuleId + ") and A.RoomID=" + RoomId + " and A.CompanyID=" + CompanyID + " ORDER BY M.DisplayOrderNumber,M.GroupID,M.DisplayName";
                        else
                            qry = @"SELECT M.ID as ModuleID,M.DisplayName as ModuleName,M.IsModule,M.GroupID,ISNULL(A.ID,0) as ID ,ISnull(A.RoleID,0) as RoleID ,ISNULL(A.IsInsert,0) as IsInsert,ISNULL(A.IsUpdate,0)  as IsUpdate,ISNULL(A.ISDelete,0) as ISDelete ,ISNULL(A.IsView,0) as IsView,ISNULL(A.IsChecked,0) as IsChecked,ModuleValue,M.ParentID,M.Value as ModuleURL,M.ImageName as ImageName,ISNULL(A.ShowDeleted,0) as ShowDeleted,ISNULL(A.ShowArchived,0) as ShowArchived,ISNULL(A.ShowUDF,0) as ShowUDF,M.DisplayOrderNumber,ISNULL(A.ShowChangeLog,0) as ShowChangeLog FROM ModuleMaster M LEFT OUTER  JOIN UserRoleDetails A ON M.ID = A.ModuleID  AND A.RoleId='" + RoleId + "' WHERE ISNULL(M.IsDeleted,0) = 0 and A.ModuleID in (" + ModuleId + ") and A.RoomID=" + RoomId + " and A.CompanyID=" + CompanyID + " ORDER BY M.DisplayOrderNumber,M.GroupID,M.DisplayName";

                        lstUserRoleModuleDetails = (from u in context.ExecuteStoreQuery<UserRoleModuleDetailsDTO>(qry)
                                                    select new UserRoleModuleDetailsDTO
                                                    {
                                                        EnteriseId = u.EnteriseId,
                                                        CompanyId = u.CompanyId,
                                                        RoomId = u.RoomId,
                                                        ID = u.ID,
                                                        RoleID = u.RoleID,
                                                        UserID = u.UserID,
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
                                                        ImageName = u.ImageName,
                                                        DisplayOrderNumber = u.DisplayOrderNumber,
                                                        ShowChangeLog = u.ShowChangeLog
                                                    }).ToList();
                    }
                }



                return lstUserRoleModuleDetails;


            }
        }
    }
}
