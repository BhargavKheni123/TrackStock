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

namespace eTurns.DAL
{
    public partial class UserMasterDALPart1 : eTurnsBaseDAL
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
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<UserMasterDTO> obj = (from u in context.ExecuteStoreQuery<UserMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,'' as CreatedDate,'' as UpdatedDate FROM UserMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1")
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
                                                          RoomName = u.RoomName,
                                                          CreatedDate = u.CreatedDate,
                                                          UpdatedDate = u.UpdatedDate,
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
                                                          UserType = u.UserType
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
        public IEnumerable<UserMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId)
        {

            //Get Cached-Media
            IEnumerable<UserMasterDTO> ObjCache = CacheHelper<IEnumerable<UserMasterDTO>>.GetCacheItem("Cached_UserMaster_" + CompanyId.ToString());
            if (ObjCache == null)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<UserMasterDTO> obj = (from u in context.ExecuteStoreQuery<UserMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,'' As CreatedDate,'' AS UpdatedDAte FROM UserMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.CompanyID = " + CompanyId.ToString())
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
                                                          RoomName = u.RoomName,
                                                          CreatedDate = u.CreatedDate,
                                                          UpdatedDate = u.UpdatedDate,
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
                    ObjCache = CacheHelper<IEnumerable<UserMasterDTO>>.AddCacheItem("Cached_UserMaster_" + CompanyId.ToString(), obj);
                }
            }
            if (RoomID == 0)
            {
                return ObjCache;
            }
            return ObjCache.Where(t => t.Room == RoomID);
        }
        public IEnumerable<UserMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId, string DBConnectionstring)
        {

            //Get Cached-Media
            IEnumerable<UserMasterDTO> ObjCache = null;
            if (ObjCache == null)
            {
                using (var context = new eTurnsEntities(DBConnectionstring))
                {
                    IEnumerable<UserMasterDTO> obj = (from u in context.ExecuteStoreQuery<UserMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,'' as CreatedDAte,'' as UpdatedDate FROM UserMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.CompanyID = " + CompanyId.ToString())
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
                                                          RoomName = u.RoomName,
                                                          CreatedDate = u.CreatedDate,
                                                          UpdatedDate = u.UpdatedDate,
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
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<UserMasterDTO> obj = (from u in context.ExecuteStoreQuery<UserMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,'' as CreatedDate,'' as UpdatedDate FROM UserMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.CompanyID = " + CompanyID.ToString() + @" AND A.Room = " + RoomID.ToString() + @" AND A.IsArchived != 1")
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
                                                          CreatedDate = u.CreatedDate,
                                                          UpdatedDate = u.UpdatedDate,
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
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    TotalCount = Convert.ToInt32(context.ExecuteStoreQuery<Int32>(@"SELECT Count(A.ID) FROM UserMaster as A WHERE A.IsDeleted!=1 AND A.CompanyID = " + CompanyID.ToString() + @" AND A.Room = " + RoomID.ToString() + @" AND A.IsArchived != 1 " + search + "").ToList()[0]);

                    IEnumerable<UserMasterDTO> obj = (from u in context.ExecuteStoreQuery<UserMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,'' AS CreatedDate,'' as UpdatedDate FROM UserMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.CompanyID = " + CompanyID.ToString() + @" AND A.Room = " + RoomID.ToString() + @" AND A.IsArchived != 1 " + search + " ORDER BY " + strSortinitializer)
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
                                                          CreatedDate = u.CreatedDate,
                                                          UpdatedDate = u.UpdatedDate,
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
                            item.ReplenishingRooms = GetUserRoomReplanishmentDetailsRecord(item.ID);
                        }
                    }
                    return obj;
                }
            }
            else if (SearchTerm.Contains("#IsDeletedRecords#"))
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    TotalCount = Convert.ToInt32(context.ExecuteStoreQuery<Int32>(@"SELECT Count(A.ID) FROM UserMaster as A WHERE A.IsArchived != 1").ToList()[0]);

                    IEnumerable<UserMasterDTO> obj = (from u in context.ExecuteStoreQuery<UserMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, A.IsDeleted,A.IsArchived,'' AS CreatedDate,'' AS UpdatedDAte FROM UserMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsArchived != 1  ORDER BY " + strSortinitializer)
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
                                                          CreatedDate = u.CreatedDate,
                                                          UpdatedDate = u.UpdatedDate,
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
                            item.ReplenishingRooms = GetUserRoomReplanishmentDetailsRecord(item.ID);
                        }
                    }
                    return obj;


                }
            }
            else if (SearchTerm.Contains("#IsArchivedRecords#"))
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    TotalCount = Convert.ToInt32(context.ExecuteStoreQuery<Int32>(@"SELECT Count(A.ID) FROM UserMaster as A WHERE A.IsDeleted != 1").ToList()[0]);

                    IEnumerable<UserMasterDTO> obj = (from u in context.ExecuteStoreQuery<UserMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, A.IsDeleted,A.IsArchived,'' as CreatedDate,'' as UpdatedDate FROM UserMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted != 1  ORDER BY " + strSortinitializer)
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
                                                          CreatedDate = u.CreatedDate,
                                                          UpdatedDate = u.UpdatedDate,
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
                            item.ReplenishingRooms = GetUserRoomReplanishmentDetailsRecord(item.ID);
                        }
                    }
                    return obj;
                }
            }
            else if (SearchTerm.Contains("#BOTH#"))
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    TotalCount = Convert.ToInt32(context.ExecuteStoreQuery<Int32>(@"SELECT Count(A.ID) FROM UserMaster as A").ToList()[0]);

                    IEnumerable<UserMasterDTO> obj = (from u in context.ExecuteStoreQuery<UserMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, A.IsDeleted,A.IsArchived,'' as CreatedDate,'' as UpdatedDate FROM UserMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID ORDER BY " + strSortinitializer)
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
                                                          CreatedDate = u.CreatedDate,
                                                          UpdatedDate = u.UpdatedDate,
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
                            item.ReplenishingRooms = GetUserRoomReplanishmentDetailsRecord(item.ID);
                        }
                    }
                    return obj;
                }
            }
            else
            {

                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    TotalCount = Convert.ToInt32(context.ExecuteStoreQuery<Int32>(@"SELECT Count(ID) FROM UserMaster WHERE IsDeleted!=1 AND CompanyID = " + CompanyID.ToString() + @" AND Room = " + RoomID.ToString() + @" AND IsArchived != 1 AND ((UserName like '%" + SearchTerm + "%') OR (ID like '%" + SearchTerm + "%'))").ToList()[0]);
                    IEnumerable<UserMasterDTO> obj = (from u in context.ExecuteStoreQuery<UserMasterDTO>("SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,'' as CreatedDate,'' as UpdatedDate FROM UserMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.CompanyID = " + CompanyID.ToString() + @" AND A.Room = " + RoomID.ToString() + @" AND A.IsArchived != 1 AND ((A.UserName like '%" + SearchTerm + "%') OR (A.ID like '%" + SearchTerm + "%')) ORDER BY " + strSortinitializer)
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
                                                          CreatedDate = u.CreatedDate,
                                                          UpdatedDate = u.UpdatedDate,
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
                            item.ReplenishingRooms = GetUserRoomReplanishmentDetailsRecord(item.ID);
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
        public UserMasterDTO CheckAuthantication(string Email, string Password)
        {
            UserMasterDTO objresult = new UserMasterDTO();
            if (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password))
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    objresult = (from u in context.ExecuteStoreQuery<UserMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.IsRoomActive, D.RoomName, A.GUID FROM UserMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.Email='" + Email + "' AND A.Password COLLATE Latin1_General_CS_AS = '" + Password + "' ")
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
                        objresult.PermissionList = GetUserRoleModuleDetailsRecord(objresult.ID, objresult.RoleID, 0);
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
        /// Get Particullar Record from the User Master by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public List<UserRoomReplanishmentDetailsDTO> GetUserRoomReplanishmentDetailsRecord(Int64 UserID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (UserID > 0)
                {
                    List<UserRoomReplanishmentDetailsDTO> objRe = new List<UserRoomReplanishmentDetailsDTO>();

                    objRe = (from u in context.UserRoomsDetails
                             where u.UserID == UserID
                             select new UserRoomReplanishmentDetailsDTO
                             {
                                 ID = u.ID,
                                 RoleID = u.RoleId,
                                 RoomID = u.RoomID,
                                 CompanyId = u.CompanyId,
                                 EnterpriseId = u.EnterpriseId,
                                 IsRoomAccess = u.IsRoomAccess
                             }).ToList();
                    return objRe;
                }

            }
            return null;
        }

        public UserMasterDTO GetUserDetails(long UserId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from um in context.UserMasters
                        where um.ID == UserId
                        select new UserMasterDTO
                        {
                            ID = um.ID,
                            GUID = um.GUID,
                            UserName = um.UserName,
                            Password = um.Password,
                            //ConfirmPassword = um.Password,
                            Phone = um.Phone,
                            RoleID = um.RoleId,
                            Email = um.Email,
                            Room = um.Room,
                            IsDeleted = um.IsDeleted,
                            IsArchived = um.IsArchived,
                            CompanyID = um.CompanyID,
                            Created = um.Created ?? DateTime.MinValue,
                            Updated = um.Updated,
                            LastUpdatedBy = um.LastUpdatedBy,
                            CreatedBy = um.CreatedBy,
                            Prefix = um.Prefix,
                            FirstName = um.FirstName,
                            MiddleName = um.MiddleName,
                            LastName = um.LastName,
                            Gender = um.Gender,
                            Phone2 = um.Phone2,
                            EmployeeNumber = um.EmployeeNumber,
                            CostCenter = um.CostCenter,
                            JobTitle = um.JobTitle,
                            Address = um.Address,
                            Country = um.Country,
                            State = um.State,
                            City = um.City,
                            PostalCode = um.PostalCode,
                            UDF1 = um.UDF1,
                            UDF2 = um.UDF2,
                            UDF3 = um.UDF3,
                            UDF4 = um.UDF4,
                            UDF5 = um.UDF5,
                            UDF6 = um.UDF6,
                            UDF7 = um.UDF7,
                            UDF8 = um.UDF8,
                            UDF9 = um.UDF9,
                            UDF10 = um.UDF10
                        }).FirstOrDefault();
            }
        }

        /// <summary>
        /// Get Particullar Record from the Role Master by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public UserMasterDTO GetHistoryRecord(Int64 id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
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
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                UserMaster obj = context.UserMasters.Single(t => t.ID == id);
                obj.IsDeleted = true;
                obj.IsArchived = false;
                obj.Updated = DateTimeUtility.DateTimeNow;
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
        public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyId)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE UserMaster SET Updated = '" + DateTime.Now.ToString() + "' , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE ID =" + item.ToString() + ";";
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
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "UPDATE UserMaster SET " + columnName + " = '" + value + "', Updated = DATETIME ('" + System.DateTime.Now.ToString(Convert.ToString((new System.Configuration.AppSettingsReader()).GetValue("FormatDateTimetoStore", typeof(System.String)))) + "') WHERE ID=" + id;
                context.ExecuteStoreCommand(strQuery);
            }
            return value;
        }

        public List<long> GetDistinctChildCompanyID(long UserId, long RoleId, long UserType)
        {
            List<long> lstCompany = new List<long>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (UserType == 2)
                {
                    if (RoleId == -2)
                    {
                        lstCompany = (from x in context.ExecuteStoreQuery<long>(@"SELECT distinct CompanyId from UserRoleDetails")
                                      select x).ToList();
                    }
                    else
                    {
                        lstCompany = (from x in context.ExecuteStoreQuery<long>(@"SELECT distinct CompanyId from UserRoleDetails WHERE UserID=" + UserId)
                                      select x).ToList();
                    }
                }
                else
                {
                    lstCompany = (from x in context.ExecuteStoreQuery<long>(@"SELECT distinct CompanyId from UserRoleDetails WHERE UserID=" + UserId)
                                  select x).ToList();
                }
                return lstCompany;
            }

        }

        public List<long> GetDistinctChildRoomID(long UserId, long RoleId, long UserType, long CompanyID)
        {
            List<long> lstCompany = new List<long>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (UserType == 2)
                {
                    if (RoleId == -2)
                    {
                        lstCompany = (from x in context.ExecuteStoreQuery<long>(@"SELECT distinct RoomId from UserRoleDetails WHERE CompanyId=" + CompanyID)
                                      select x).ToList();
                    }
                    else
                    {
                        lstCompany = (from x in context.ExecuteStoreQuery<long>(@"SELECT distinct RoomId from UserRoleDetails WHERE CompanyId=" + CompanyID + " AND UserID=" + UserId)
                                      select x).ToList();
                    }
                }
                else
                {
                    lstCompany = (from x in context.ExecuteStoreQuery<long>(@"SELECT distinct RoomId from UserRoleDetails WHERE CompanyId=" + CompanyID + " AND UserID=" + UserId)
                                  select x).ToList();
                }
                return lstCompany;
            }

        }

        public Int64 SetEnterpriseSuperAdmin(EnterpriseDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                UserMaster objCurrentEnterpriseSU = context.UserMasters.FirstOrDefault(t => t.RoleId == -2 && t.UserType == 2 && t.IsDeleted == false);
                if (objCurrentEnterpriseSU != null && objDTO.UserName != objCurrentEnterpriseSU.UserName)
                {
                    objCurrentEnterpriseSU.IsDeleted = true;
                    objCurrentEnterpriseSU.RoleId = 0;
                    objCurrentEnterpriseSU.UserType = 3;

                    UserMaster ExistingUser = context.UserMasters.FirstOrDefault(t => t.UserName == objDTO.UserName && t.IsDeleted == false);
                    if (ExistingUser != null)
                    {
                        ExistingUser.RoleId = -2;
                        ExistingUser.UserType = 2;
                        ExistingUser.Room = 0;
                        ExistingUser.CompanyID = 0;
                        if (objDTO.IsPasswordChanged == "1")
                        {
                            ExistingUser.Password = objDTO.EnterpriseUserPassword;
                        }
                        EnterpriseMaster objEnterpriseMaster = context.EnterpriseMasters.FirstOrDefault(t => t.ID == objDTO.ID);
                        objEnterpriseMaster.EnterpriseSuperAdmin = ExistingUser.ID;

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
                    return ExistingUser.ID;
                }
                return objCurrentEnterpriseSU.ID;
            }
        }
        //public Int64 SetEnterpriseSuperAdmin(List<EnterpriseSuperAdmin> lstSuperAdmins) {
        //    using (var context = new eTurnsMasterEntities)
        //    {

        //    }

        //}

        public List<UserSchedulerDTO> GetAllSchedulesForAllUsers()
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.ExecuteStoreQuery<UserSchedulerDTO>("EXEC [GetAllSchedulesForAllUsers]").ToList();
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
        /// Get Particullar Record from the User Master by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public List<UserRoleModuleDetailsDTO> GetUserRoleModuleDetailsRecord(Int64 UserID, Int64 RoleID, Int64 RoomID)
        {
            UserRoleModuleDetailsDTO objresult = new UserRoleModuleDetailsDTO();
            string qry = "";
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (RoleID > 0)
                {
                    List<UserRoleModuleDetailsDTO> objRe = new List<UserRoleModuleDetailsDTO>();
                    if (UserID > 0)
                        qry = @"SELECT ISNULL(R.IsRoomActive,0) as IsRoomActive,M.ID as ModuleID,M.DisplayName as ModuleName,M.IsModule,M.GroupID,M.ResourceKey,M.ToolTipResourceKey,ISNULL(A.ID,0) as ID ,ISnull(A.RoleID,0) as RoleID ,ISNULL(A.RoomID,0) as RoomID,ISNULL(A.CompanyId,0) as CompanyId,ISNULL(A.EnterpriseID,0) as EnteriseId,ISNULL(A.IsInsert,0) as IsInsert,ISNULL(A.IsUpdate,0)  as IsUpdate,ISNULL(A.ISDelete,0) as ISDelete ,ISNULL(A.IsView,0) as IsView ,ISNULL(A.IsChecked,0) as IsChecked,ModuleValue,M.ParentID,M.Value as ModuleURL,M.ImageName as ImageName,ISNULL(A.ShowDeleted,0) as ShowDeleted,ISNULL(A.ShowArchived,0) as ShowArchived,ISNULL(A.ShowUDF,0) as ShowUDF,M.DisplayOrderNumber,ISNULL(A.ShowChangeLog,0) as ShowChangeLog FROM ModuleMaster M LEFT OUTER  JOIN UserRoleDetails A ON M.ID = A.ModuleID AND A.RoleId='" + RoleID + "' and A.UserID='" + UserID + "'   LEFT OUTER  JOIN Room R ON R.ID = A.RoomID WHERE ISNULL(M.IsDeleted,0) = 0 and ISNULL(R.IsDeleted,0) = 0 ORDER BY M.DisplayOrderNumber,M.GroupID,M.DisplayName";
                    else
                        qry = @"SELECT ISNULL(R.IsRoomActive,0) as IsRoomActive,M.ID as ModuleID,M.DisplayName as ModuleName,M.IsModule,M.GroupID,M.ResourceKey,M.ToolTipResourceKey,ISNULL(A.ID,0) as ID ,ISnull(A.RoleID,0) as RoleID ,ISNULL(A.RoomID,0) as RoomID,ISNULL(A.CompanyId,0) as CompanyId,ISNULL(A.EnterpriseID,0) as EnteriseId,ISNULL(A.IsInsert,0) as IsInsert,ISNULL(A.IsUpdate,0)  as IsUpdate,ISNULL(A.ISDelete,0) as ISDelete ,ISNULL(A.IsView,0) as IsView,ISNULL(A.IsChecked,0) as IsChecked,ModuleValue,M.ParentID,M.Value as ModuleURL,M.ImageName as ImageName,ISNULL(A.ShowDeleted,0) as ShowDeleted,ISNULL(A.ShowArchived,0) as ShowArchived,ISNULL(A.ShowUDF,0) as ShowUDF,M.DisplayOrderNumber,ISNULL(A.ShowChangeLog,0) as ShowChangeLog FROM ModuleMaster M LEFT OUTER  JOIN UserRoleDetails A ON M.ID = A.ModuleID  AND A.RoleId=" + RoleID + " LEFT OUTER JOIN Room as R on A.RoomId = R.ID WHERE ISNULL(M.IsDeleted,0) = 0 ORDER BY M.DisplayOrderNumber,M.GroupID,M.DisplayName";

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
                                 ImageName = u.ImageName,
                                 CompanyId = u.CompanyId,
                                 EnteriseId = u.EnteriseId,
                                 IsRoomActive = u.IsRoomActive,
                                 DisplayOrderNumber = u.DisplayOrderNumber,
                                 resourcekey = u.resourcekey,
                                 ToolTipResourceKey = u.ToolTipResourceKey,
                                 ShowChangeLog = u.ShowChangeLog
                             }).ToList();
                    return objRe;
                }
                else
                {
                    return (from u in context.ExecuteStoreQuery<UserRoleModuleDetailsDTO>(@"SELECT ISNULL(R.IsRoomActive,0) as IsRoomActive, M.ID as ModuleID,M.DisplayName as ModuleName,M.IsModule,M.GroupID,M.ResourceKey,M.ToolTipResourceKey,ISNULL(A.ID,0) as ID ,ISnull(A.RoleID,0) as RoleID,ISNULL(A.RoomID,0) as RoomID,ISNULL(R.RoomName,'') as RoomName,ISNULL(A.IsInsert,0)  as IsInsert,ISNULL(A.IsUpdate,0) as IsUpdate ,ISNULL(A.ISDelete,0) as ISDelete,ISNULL(A.IsView,0) as IsView,ISNULL(A.IsChecked,0) as IsChecked,ModuleValue,M.ParentID,M.Value as ModuleURL,M.ImageName as ImageName,ISNULL(A.ShowDeleted,0) as ShowDeleted,ISNULL(A.ShowArchived,0) as ShowArchived,ISNULL(A.ShowUDF,0) as ShowUDF,M.DisplayOrderNumber,ISNULL(A.ShowChangeLog,0) as ShowChangeLog  FROM ModuleMaster M LEFT OUTER  JOIN UserRoleDetails A ON M.ID = A.ModuleID AND A.RoleId='0'  and A.UserID='0' LEFT OUTER  JOIN Room R ON R.ID = A.RoomID where m.isdeleted=0 ORDER BY M.DisplayOrderNumber,M.GroupID,M.DisplayName")
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
                                IsChecked = u.IsChecked,
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
                                IsRoomActive = u.IsRoomActive,
                                ImageName = u.ImageName,
                                DisplayOrderNumber = u.DisplayOrderNumber,
                                resourcekey = u.resourcekey,
                                ToolTipResourceKey = u.ToolTipResourceKey,
                                ShowChangeLog = u.ShowChangeLog
                            }).ToList();
                }

            }
        }

        /// <summary>
        /// Get Particullar Record from the User Master by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public UserMasterDTO GetRecord(long Id)
        {
            UserMasterDTO objresult = new UserMasterDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@UserID", Id) };
                objresult = context.ExecuteStoreQuery<UserMasterDTO>("exec [GetUserWithLicenceDetailsByUserId] @UserID", params1).FirstOrDefault();

                if (objresult != null)
                {
                    objresult.PermissionList = GetUserRoleModuleDetailsByUserIdAndRoleId(Id, objresult.RoleID);
                    string RoomLists = "";

                    if (objresult.PermissionList != null && objresult.PermissionList.Count > 0)
                    {
                        objresult.UserWiseAllRoomsAccessDetails = ConvertUserPermissions(objresult.PermissionList, objresult.RoleID, ref RoomLists);
                        objresult.SelectedRoomAccessValue = RoomLists;
                    }
                    objresult.lstAccess = GetUserRoomAccessesByUserId(Id);
                }
            }

            return objresult;
        }

        public List<UserMasterDTO> GetModuleWiseUsers(Int64 ModuleID, Int64 RoomID, Int64 CompanyId)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
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

        /// <summary>
        /// Insert Record in the DataBase Role Master
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 Insert(UserMasterDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                UserMaster obj = new UserMaster();

                obj.ID = objDTO.ID;
                obj.UserName = objDTO.UserName;
                obj.CompanyID = objDTO.CompanyID;
                obj.Password = objDTO.Password;
                obj.Phone = objDTO.Phone;
                obj.Email = objDTO.Email;
                obj.RoleId = objDTO.RoleID;
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.CompanyID = objDTO.CompanyID;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.UserType = objDTO.UserType;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.GUID = Guid.NewGuid();
                obj.EnforceRolePermission = objDTO.EnforceRolePermission;
                obj.IseTurnsAdmin = objDTO.IseTurnsAdmin;
                obj.Prefix = objDTO.Prefix;
                obj.FirstName = objDTO.FirstName;
                obj.MiddleName = objDTO.MiddleName;
                obj.LastName = objDTO.LastName;
                obj.Gender = objDTO.Gender;
                obj.Phone2 = objDTO.Phone2;
                obj.EmployeeNumber = objDTO.EmployeeNumber;
                obj.CostCenter = objDTO.CostCenter;
                obj.JobTitle = objDTO.JobTitle;
                obj.Address = objDTO.Address;
                obj.Country = objDTO.Country;
                obj.State = objDTO.State;
                obj.City = objDTO.City;
                obj.PostalCode = objDTO.PostalCode;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.UDF6 = objDTO.UDF6;
                obj.UDF7 = objDTO.UDF7;
                obj.UDF8 = objDTO.UDF8;
                obj.UDF9 = objDTO.UDF9;
                obj.UDF10 = objDTO.UDF10;
                context.UserMasters.AddObject(obj);
                context.SaveChanges();
                List<UserSchedulerDTO> lstUserRolesWithOrderScheduler = new List<UserSchedulerDTO>();

                if (!obj.EnforceRolePermission)
                {
                    if (objDTO.UserWiseAllRoomsAccessDetails != null && objDTO.UserWiseAllRoomsAccessDetails.Count > 0)
                    {
                        foreach (UserWiseRoomsAccessDetailsDTO item in objDTO.UserWiseAllRoomsAccessDetails)
                        {
                            foreach (UserRoleModuleDetailsDTO InternalItem in item.PermissionList)
                            {
                                UserRoleDetail objDetails = new UserRoleDetail();
                                objDetails.GUID = Guid.NewGuid();
                                objDetails.UserID = obj.ID;
                                objDetails.RoleID = obj.RoleId;
                                objDetails.IsChecked = InternalItem.IsChecked;
                                objDetails.IsDelete = InternalItem.IsDelete;
                                objDetails.IsInsert = InternalItem.IsInsert;
                                objDetails.IsUpdate = InternalItem.IsUpdate;
                                objDetails.IsView = InternalItem.IsView;
                                objDetails.ShowDeleted = InternalItem.ShowDeleted;
                                objDetails.ShowArchived = InternalItem.ShowArchived;
                                objDetails.ShowUDF = InternalItem.ShowUDF;
                                objDetails.ShowChangeLog = InternalItem.ShowChangeLog;

                                if (InternalItem.GroupId == 3 && InternalItem.IsChecked)
                                {
                                    objDetails.IsView = true;
                                }

                                if (InternalItem.GroupId == 3 && !InternalItem.IsChecked)
                                {
                                    objDetails.IsView = false;
                                }

                                objDetails.ModuleID = InternalItem.ModuleID;
                                objDetails.ModuleValue = InternalItem.ModuleValue;
                                objDetails.RoomId = item.RoomID;
                                objDetails.CompanyID = item.CompanyId;
                                objDetails.EnterpriseID = item.EnterpriseId;
                                context.UserRoleDetails.AddObject(objDetails);
                            }
                        }
                    }

                    IQueryable<UserRoomAccess> lstExisting = context.UserRoomAccesses.Where(t => t.UserId == obj.ID);

                    if (lstExisting.Any())
                    {
                        foreach (var item in lstExisting)
                        {
                            context.UserRoomAccesses.DeleteObject(item);
                        }
                    }

                    if (objDTO.lstAccess != null && objDTO.lstAccess.Count > 0)
                    {
                        foreach (var item in objDTO.lstAccess)
                        {
                            UserRoomAccess objUserRoomAccess = new UserRoomAccess();
                            objUserRoomAccess.UserId = obj.ID;
                            objUserRoomAccess.RoleId = obj.RoleId;
                            objUserRoomAccess.EnterpriseId = item.EnterpriseId;
                            objUserRoomAccess.CompanyId = item.CompanyId;
                            objUserRoomAccess.RoomId = item.RoomId;
                            context.UserRoomAccesses.AddObject(objUserRoomAccess);
                        }
                    }
                }
                else
                {
                    context.ExecuteStoreCommand("EXEC CopyRolePermissionToUser " + obj.ID + "");
                }

                context.SaveChanges();
                SetUserScheduler(obj.ID, obj.LastUpdatedBy ?? (obj.CreatedBy ?? obj.ID));

                return obj.ID;
            }
        }

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool Edit(UserMasterDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                UserMaster obj = context.UserMasters.FirstOrDefault(t => t.ID == objDTO.ID);

                if (obj != null)
                {
                    bool CopyRolePermissionOnEdit = false;
                    obj.ID = objDTO.ID;
                    obj.UserName = objDTO.UserName;
                    //obj.CompanyID = objDTO.CompanyID;
                    if (!string.IsNullOrWhiteSpace(objDTO.Password))
                    {
                        obj.Password = objDTO.Password;
                    }
                    obj.Phone = objDTO.Phone;
                    obj.Email = objDTO.Email;
                    obj.RoleId = objDTO.RoleID;
                    //obj.CreatedBy = objDTO.CreatedBy;
                    obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                    //obj.Room = objDTO.Room;
                    //obj.CompanyID = objDTO.CompanyID;
                    obj.Updated = DateTimeUtility.DateTimeNow;
                    obj.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
                    obj.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
                    obj.Prefix = objDTO.Prefix;
                    obj.FirstName = objDTO.FirstName;
                    obj.MiddleName = objDTO.MiddleName;
                    obj.LastName = objDTO.LastName;
                    obj.Gender = objDTO.Gender;
                    obj.Phone2 = objDTO.Phone2;
                    obj.EmployeeNumber = objDTO.EmployeeNumber;
                    obj.CostCenter = objDTO.CostCenter;
                    obj.JobTitle = objDTO.JobTitle;
                    obj.Address = objDTO.Address;
                    obj.Country = objDTO.Country;
                    obj.State = objDTO.State;
                    obj.City = objDTO.City;
                    obj.PostalCode = objDTO.PostalCode;
                    obj.UDF1 = objDTO.UDF1;
                    obj.UDF2 = objDTO.UDF2;
                    obj.UDF3 = objDTO.UDF3;
                    obj.UDF4 = objDTO.UDF4;
                    obj.UDF5 = objDTO.UDF5;
                    obj.UDF6 = objDTO.UDF6;
                    obj.UDF7 = objDTO.UDF7;
                    obj.UDF8 = objDTO.UDF8;
                    obj.UDF9 = objDTO.UDF9;
                    obj.UDF10 = objDTO.UDF10;

                    if (!obj.EnforceRolePermission && objDTO.EnforceRolePermission)
                    {
                        CopyRolePermissionOnEdit = true;
                    }

                    obj.EnforceRolePermission = objDTO.EnforceRolePermission;
                    context.SaveChanges();

                    if (!obj.EnforceRolePermission)
                    {
                        string strQuery = "EXEC DeleteUserRoleDetails " + objDTO.ID;
                        //context.ExecuteStoreCommand(strQuery);

                        if (context.UserRoleDetails.Any(t => t.UserID == objDTO.ID))
                        {
                            IQueryable<UserRoleDetail> lstUserRolePermission = context.UserRoleDetails.Where(t => t.UserID == objDTO.ID);
                            foreach (UserRoleDetail UserRoleDetailitem in lstUserRolePermission)
                            {
                                context.UserRoleDetails.DeleteObject(UserRoleDetailitem);
                            }
                        }

                        string strUpdateQry = "update UserPermission_History set IsRecent = 1 where UserID = " + objDTO.ID.ToString();
                        context.ExecuteStoreCommand(strUpdateQry);
                        List<Int64> lstModuleId = null;

                        if (objDTO.UserWiseAllRoomsAccessDetails != null && objDTO.UserWiseAllRoomsAccessDetails.Count > 0)
                        {
                            foreach (UserWiseRoomsAccessDetailsDTO item in objDTO.UserWiseAllRoomsAccessDetails)
                            {
                                lstModuleId = new List<long>();

                                foreach (UserRoleModuleDetailsDTO InternalItem in item.PermissionList)
                                {
                                    UserRoleDetail objDetails = new UserRoleDetail();
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
                                    if (InternalItem.GroupId == 3 && InternalItem.IsChecked)
                                    {
                                        objDetails.IsView = true;
                                    }
                                    if (InternalItem.GroupId == 3 && !InternalItem.IsChecked)
                                    {
                                        objDetails.IsView = false;
                                    }
                                    objDetails.ModuleID = InternalItem.ModuleID;
                                    objDetails.ModuleValue = InternalItem.ModuleValue;
                                    objDetails.RoleID = obj.RoleId;
                                    objDetails.RoomId = InternalItem.RoomId;
                                    objDetails.UserID = obj.ID;
                                    objDetails.CompanyID = InternalItem.CompanyId;
                                    objDetails.EnterpriseID = InternalItem.EnteriseId;
                                    context.UserRoleDetails.AddObject(objDetails);
                                    // context.SaveChanges();

                                    lstModuleId.Add(Convert.ToInt64(InternalItem.ModuleID));
                                }

                                lstModuleId = null;
                            }
                        }

                        strQuery = "DELETE from UserRoomsDetails where UserID=" + objDTO.ID;
                        context.ExecuteStoreCommand(strQuery);
                        IQueryable<UserRoomAccess> lstExisting = context.UserRoomAccesses.Where(t => t.UserId == obj.ID);

                        if (lstExisting.Any())
                        {
                            foreach (var item in lstExisting)
                            {
                                context.UserRoomAccesses.DeleteObject(item);
                            }
                        }
                        if (objDTO.lstAccess != null && objDTO.lstAccess.Count > 0)
                        {
                            foreach (var item in objDTO.lstAccess)
                            {
                                UserRoomAccess objUserRoomAccess = new UserRoomAccess();
                                objUserRoomAccess.UserId = obj.ID;
                                objUserRoomAccess.RoleId = obj.RoleId;
                                objUserRoomAccess.EnterpriseId = item.EnterpriseId;
                                objUserRoomAccess.CompanyId = item.CompanyId;
                                objUserRoomAccess.RoomId = item.RoomId;
                                context.UserRoomAccesses.AddObject(objUserRoomAccess);
                            }
                        }
                        context.SaveChanges();
                    }
                    else
                    {
                        if (CopyRolePermissionOnEdit)
                        {
                            context.ExecuteStoreCommand("EXEC CopyRolePermissionToUser " + obj.ID + "");
                        }
                    }
                    SetUserScheduler(obj.ID, obj.LastUpdatedBy ?? (obj.CreatedBy ?? obj.ID));
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public List<UserRoleModuleDetailsDTO> GetUserRoleModuleDetailsRecord(long UserID)
        {
            UserRoleModuleDetailsDTO objresult = new UserRoleModuleDetailsDTO();
            string qry = "";
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<UserRoleModuleDetailsDTO> objRe = new List<UserRoleModuleDetailsDTO>();
                if (UserID > 0)
                    qry = @"SELECT ISNULL(R.IsRoomActive,0) as IsRoomActive,M.ID as ModuleID,M.DisplayName as ModuleName,M.IsModule,M.GroupID,M.ResourceKey,M.ToolTipResourceKey,ISNULL(A.ID,0) as ID ,ISnull(A.RoleID,0) as RoleID ,ISNULL(A.RoomID,0) as RoomID,ISNULL(A.CompanyId,0) as CompanyId,ISNULL(A.EnterpriseID,0) as EnteriseId,ISNULL(A.IsInsert,0) as IsInsert,ISNULL(A.IsUpdate,0)  as IsUpdate,ISNULL(A.ISDelete,0) as ISDelete ,ISNULL(A.IsView,0) as IsView ,ISNULL(A.IsChecked,0) as IsChecked,ModuleValue,M.ParentID,M.Value as ModuleURL,M.ImageName as ImageName,ISNULL(A.ShowDeleted,0) as ShowDeleted,ISNULL(A.ShowArchived,0) as ShowArchived,ISNULL(A.ShowUDF,0) as ShowUDF,M.DisplayOrderNumber,ISNULL(A.ShowChangeLog,0) as ShowChangeLog FROM ModuleMaster M LEFT OUTER  JOIN UserRoleDetails A ON M.ID = A.ModuleID AND A.UserID='" + UserID + "'   LEFT OUTER  JOIN Room R ON R.ID = A.RoomID WHERE ISNULL(M.IsDeleted,0) = 0 and ISNULL(R.IsDeleted,0) = 0 ORDER BY M.DisplayOrderNumber,M.GroupID,M.DisplayName";
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
                             ImageName = u.ImageName,
                             CompanyId = u.CompanyId,
                             EnteriseId = u.EnteriseId,
                             IsRoomActive = u.IsRoomActive,
                             DisplayOrderNumber = u.DisplayOrderNumber,
                             resourcekey = u.resourcekey,
                             ShowChangeLog = u.ShowChangeLog,
                             ToolTipResourceKey = u.ToolTipResourceKey
                         }).ToList();
                return objRe;
                
            }
        }

        public UserMasterDTO GetRecordOnlyUserFields(Int64 id)
        {
            UserMasterDTO objresult = new UserMasterDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objresult = (from u in context.ExecuteStoreQuery<UserMasterDTO>(@"SELECT * FROM UserMaster WHERE IsDeleted!=1 AND IsArchived != 1 AND ID=" + id.ToString())
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
            }
            return objresult;
        }

        public void EditRoleByUserId(Int64 UserId, Int64 RoleId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                UserMaster objUserMaster = context.UserMasters.Where(t => t.ID == UserId).FirstOrDefault();
                if (objUserMaster != null)
                {
                    objUserMaster.RoleId = RoleId;
                }
                context.SaveChanges();

            }
        }

        public bool DeleteRoomFromUseAccess(string RoleID)
        {
            bool result = true;
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var params1 = new SqlParameter[] { new SqlParameter("@RoleID", RoleID) };
                    context.ExecuteStoreQuery<object>("exec DeleteUserRoomAccess @RoleID", params1);
                }
                return result;
            }
            catch
            {
                return false;
            }
        }

        public List<UserRoleModuleDetailsDTO> GetUserRoleModuleByIDRecord(Int64 UserID, Int64 RoleID, Int64 RoomID, Int64 CompanyID, string arrm)
        {
            UserRoleModuleDetailsDTO objresult = new UserRoleModuleDetailsDTO();
            string qry = "";
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (RoleID > 0)
                {
                    List<UserRoleModuleDetailsDTO> objRe = new List<UserRoleModuleDetailsDTO>();
                    if (UserID > 0)
                        qry = @"SELECT ISNULL(R.IsRoomActive,0) as IsRoomActive,M.ID as ModuleID,M.DisplayName as ModuleName,M.IsModule,M.GroupID,M.ResourceKey,M.ToolTipResourceKey,ISNULL(A.ID,0) as ID ,ISnull(A.RoleID,0) as RoleID ,ISNULL(A.RoomID,0) as RoomID,ISNULL(A.CompanyId,0) as CompanyId,ISNULL(A.EnterpriseID,0) as EnteriseId,ISNULL(A.IsInsert,0) as IsInsert,ISNULL(A.IsUpdate,0)  as IsUpdate,ISNULL(A.ISDelete,0) as ISDelete ,ISNULL(A.IsView,0) as IsView ,ISNULL(A.IsChecked,0) as IsChecked,ModuleValue,M.ParentID,M.Value as ModuleURL,M.ImageName as ImageName,ISNULL(A.ShowDeleted,0) as ShowDeleted,ISNULL(A.ShowArchived,0) as ShowArchived,ISNULL(A.ShowUDF,0) as ShowUDF,M.DisplayOrderNumber,ISNULL(A.ShowChangeLog,0) as ShowChangeLog FROM ModuleMaster M LEFT OUTER  JOIN UserRoleDetails A ON M.ID = A.ModuleID AND A.RoleId='" + RoleID + "' and A.UserID='" + UserID + "'   LEFT OUTER  JOIN Room R ON R.ID = A.RoomID WHERE ISNULL(M.IsDeleted,0) = 0 and ISNULL(R.IsDeleted,0) = 0 AND A.ModuleId in (" + arrm + ") and A.RoomID=" + RoomID + " and A.CompanyID=" + CompanyID + " ORDER BY M.DisplayOrderNumber,M.GroupID,M.DisplayName";
                    else
                        qry = @"SELECT ISNULL(R.IsRoomActive,0) as IsRoomActive,M.ID as ModuleID,M.DisplayName as ModuleName,M.IsModule,M.GroupID,M.ResourceKey,M.ToolTipResourceKey,ISNULL(A.ID,0) as ID ,ISnull(A.RoleID,0) as RoleID ,ISNULL(A.RoomID,0) as RoomID,ISNULL(A.CompanyId,0) as CompanyId,ISNULL(A.EnterpriseID,0) as EnteriseId,ISNULL(A.IsInsert,0) as IsInsert,ISNULL(A.IsUpdate,0)  as IsUpdate,ISNULL(A.ISDelete,0) as ISDelete ,ISNULL(A.IsView,0) as IsView,ISNULL(A.IsChecked,0) as IsChecked,ModuleValue,M.ParentID,M.Value as ModuleURL,M.ImageName as ImageName,ISNULL(A.ShowDeleted,0) as ShowDeleted,ISNULL(A.ShowArchived,0) as ShowArchived,ISNULL(A.ShowUDF,0) as ShowUDF,M.DisplayOrderNumber,ISNULL(A.ShowChangeLog,0) as ShowChangeLog FROM ModuleMaster M LEFT OUTER  JOIN UserRoleDetails A ON M.ID = A.ModuleID  AND A.RoleId=" + RoleID + " LEFT OUTER JOIN Room as R on A.RoomId = R.ID WHERE ISNULL(M.IsDeleted,0) = 0 AND A.ModuleId in (" + arrm + ") and A.RoomID=" + RoomID + " and A.CompanyID=" + CompanyID + " ORDER BY M.DisplayOrderNumber,M.GroupID,M.DisplayName";

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
                                 ImageName = u.ImageName,
                                 CompanyId = u.CompanyId,
                                 EnteriseId = u.EnteriseId,
                                 IsRoomActive = u.IsRoomActive,
                                 DisplayOrderNumber = u.DisplayOrderNumber,
                                 resourcekey = u.resourcekey,
                                 ShowChangeLog = u.ShowChangeLog,
                                 ToolTipResourceKey = u.ToolTipResourceKey
                             }).ToList();
                    return objRe;
                }
                else
                {
                    qry = @"SELECT ISNULL(R.IsRoomActive,0) as IsRoomActive, M.ID as ModuleID,M.DisplayName as ModuleName,M.IsModule,M.GroupID,M.ResourceKey,M.ToolTipResourceKey,ISNULL(A.ID,0) as ID ,ISnull(A.RoleID,0) as RoleID,ISNULL(A.RoomID,0) as RoomID,ISNULL(R.RoomName,'') as RoomName,ISNULL(A.IsInsert,1)  as IsInsert,ISNULL(A.IsUpdate,1) as IsUpdate ,ISNULL(A.ISDelete,1) as ISDelete,ISNULL(A.IsView,1) as IsView,ISNULL(A.IsChecked,1) as IsChecked,ModuleValue,M.ParentID,M.Value as ModuleURL,M.ImageName as ImageName,ISNULL(A.ShowDeleted,1) as ShowDeleted,ISNULL(A.ShowArchived,1) as ShowArchived,ISNULL(A.ShowUDF,1) as ShowUDF,M.DisplayOrderNumber,ISNULL(A.ShowChangeLog,1) as ShowChangeLog FROM ModuleMaster M LEFT OUTER  JOIN UserRoleDetails A ON M.ID = A.ModuleID AND A.RoleId=" + RoleID + "  and A.UserID=" + UserID + " and A.CompanyID=" + CompanyID + " and A.RoomId=" + RoomID + " LEFT OUTER  JOIN Room R ON R.ID = " + RoomID + " where m.isdeleted=0  AND M.ID in (" + arrm + ") ORDER BY M.DisplayOrderNumber,M.GroupID,M.DisplayName";
                    return (from u in context.ExecuteStoreQuery<UserRoleModuleDetailsDTO>(qry)
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
                                IsRoomActive = u.IsRoomActive,
                                ImageName = u.ImageName,
                                DisplayOrderNumber = u.DisplayOrderNumber,
                                resourcekey = u.resourcekey,
                                ShowChangeLog = u.ShowChangeLog,
                                IsChecked = u.IsChecked,
                                ToolTipResourceKey = u.ToolTipResourceKey
                            }).ToList();
                }

            }
        }

        public List<UserRoleModuleDetailsDTO> GetUserRoleModuleByIDRecord(Int64 UserID, Int64 RoleID, Int64 RoomID, Int64 CompanyID, string arrm)
        {
            UserRoleModuleDetailsDTO objresult = new UserRoleModuleDetailsDTO();
            string qry = "";
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (RoleID > 0)
                {
                    List<UserRoleModuleDetailsDTO> objRe = new List<UserRoleModuleDetailsDTO>();
                    if (UserID > 0)
                        qry = @"SELECT ISNULL(R.IsRoomActive,0) as IsRoomActive,M.ID as ModuleID,M.DisplayName as ModuleName,M.IsModule,M.GroupID,M.ResourceKey,M.ToolTipResourceKey,ISNULL(A.ID,0) as ID ,ISnull(A.RoleID,0) as RoleID ,ISNULL(A.RoomID,0) as RoomID,ISNULL(A.CompanyId,0) as CompanyId,ISNULL(A.EnterpriseID,0) as EnteriseId,ISNULL(A.IsInsert,0) as IsInsert,ISNULL(A.IsUpdate,0)  as IsUpdate,ISNULL(A.ISDelete,0) as ISDelete ,ISNULL(A.IsView,0) as IsView ,ISNULL(A.IsChecked,0) as IsChecked,ModuleValue,M.ParentID,M.Value as ModuleURL,M.ImageName as ImageName,ISNULL(A.ShowDeleted,0) as ShowDeleted,ISNULL(A.ShowArchived,0) as ShowArchived,ISNULL(A.ShowUDF,0) as ShowUDF,M.DisplayOrderNumber,ISNULL(A.ShowChangeLog,0) as ShowChangeLog FROM ModuleMaster M LEFT OUTER  JOIN UserRoleDetails A ON M.ID = A.ModuleID AND A.RoleId='" + RoleID + "' and A.UserID='" + UserID + "'   LEFT OUTER  JOIN Room R ON R.ID = A.RoomID WHERE ISNULL(M.IsDeleted,0) = 0 and ISNULL(R.IsDeleted,0) = 0 AND A.ModuleId in (" + arrm + ") and A.RoomID=" + RoomID + " and A.CompanyID=" + CompanyID + " ORDER BY M.DisplayOrderNumber,M.GroupID,M.DisplayName";
                    else
                        qry = @"SELECT ISNULL(R.IsRoomActive,0) as IsRoomActive,M.ID as ModuleID,M.DisplayName as ModuleName,M.IsModule,M.GroupID,M.ResourceKey,M.ToolTipResourceKey,ISNULL(A.ID,0) as ID ,ISnull(A.RoleID,0) as RoleID ,ISNULL(A.RoomID,0) as RoomID,ISNULL(A.CompanyId,0) as CompanyId,ISNULL(A.EnterpriseID,0) as EnteriseId,ISNULL(A.IsInsert,0) as IsInsert,ISNULL(A.IsUpdate,0)  as IsUpdate,ISNULL(A.ISDelete,0) as ISDelete ,ISNULL(A.IsView,0) as IsView,ISNULL(A.IsChecked,0) as IsChecked,ModuleValue,M.ParentID,M.Value as ModuleURL,M.ImageName as ImageName,ISNULL(A.ShowDeleted,0) as ShowDeleted,ISNULL(A.ShowArchived,0) as ShowArchived,ISNULL(A.ShowUDF,0) as ShowUDF,M.DisplayOrderNumber,ISNULL(A.ShowChangeLog,0) as ShowChangeLog FROM ModuleMaster M LEFT OUTER  JOIN UserRoleDetails A ON M.ID = A.ModuleID  AND A.RoleId=" + RoleID + " LEFT OUTER JOIN Room as R on A.RoomId = R.ID WHERE ISNULL(M.IsDeleted,0) = 0 AND A.ModuleId in (" + arrm + ") and A.RoomID=" + RoomID + " and A.CompanyID=" + CompanyID + " ORDER BY M.DisplayOrderNumber,M.GroupID,M.DisplayName";

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
                                 ImageName = u.ImageName,
                                 CompanyId = u.CompanyId,
                                 EnteriseId = u.EnteriseId,
                                 IsRoomActive = u.IsRoomActive,
                                 DisplayOrderNumber = u.DisplayOrderNumber,
                                 resourcekey = u.resourcekey,
                                 ShowChangeLog = u.ShowChangeLog,
                                 ToolTipResourceKey = u.ToolTipResourceKey
                             }).ToList();
                    return objRe;
                }
                else
                {
                    qry = @"SELECT ISNULL(R.IsRoomActive,0) as IsRoomActive, M.ID as ModuleID,M.DisplayName as ModuleName,M.IsModule,M.GroupID,M.ResourceKey,M.ToolTipResourceKey,ISNULL(A.ID,0) as ID ,ISnull(A.RoleID,0) as RoleID,ISNULL(A.RoomID,0) as RoomID,ISNULL(R.RoomName,'') as RoomName,ISNULL(A.IsInsert,1)  as IsInsert,ISNULL(A.IsUpdate,1) as IsUpdate ,ISNULL(A.ISDelete,1) as ISDelete,ISNULL(A.IsView,1) as IsView,ISNULL(A.IsChecked,1) as IsChecked,ModuleValue,M.ParentID,M.Value as ModuleURL,M.ImageName as ImageName,ISNULL(A.ShowDeleted,1) as ShowDeleted,ISNULL(A.ShowArchived,1) as ShowArchived,ISNULL(A.ShowUDF,1) as ShowUDF,M.DisplayOrderNumber,ISNULL(A.ShowChangeLog,1) as ShowChangeLog FROM ModuleMaster M LEFT OUTER  JOIN UserRoleDetails A ON M.ID = A.ModuleID AND A.RoleId=" + RoleID + "  and A.UserID=" + UserID + " and A.CompanyID=" + CompanyID + " and A.RoomId=" + RoomID + " LEFT OUTER  JOIN Room R ON R.ID = " + RoomID + " where m.isdeleted=0  AND M.ID in (" + arrm + ") ORDER BY M.DisplayOrderNumber,M.GroupID,M.DisplayName";
                    return (from u in context.ExecuteStoreQuery<UserRoleModuleDetailsDTO>(qry)
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
                                IsRoomActive = u.IsRoomActive,
                                ImageName = u.ImageName,
                                DisplayOrderNumber = u.DisplayOrderNumber,
                                resourcekey = u.resourcekey,
                                ShowChangeLog = u.ShowChangeLog,
                                IsChecked = u.IsChecked,
                                ToolTipResourceKey = u.ToolTipResourceKey
                            }).ToList();
                }

            }
        }

        public List<UserRoleModuleDetailsDTO> GetUserRoleModuleDetailsByUserIdRoleAndType(long UserID, long RoleID, long UserType)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@UserID", UserID),
                                               new SqlParameter("@RoleID", RoleID),
                                               new SqlParameter("@UserType", UserType)
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.ExecuteStoreQuery<UserRoleModuleDetailsDTO>("exec GetUserRoleModuleDetailsRecord @UserID,@RoleID,@UserType", params1).ToList();
            }
        }

        public bool DeleteUserRoomAccess(long RoleId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoleID", RoleId) };
                context.ExecuteStoreCommand("exec [DeleteUserRoomAccess] @RoleID", params1);
                return true;
            }
        }
    }
}
