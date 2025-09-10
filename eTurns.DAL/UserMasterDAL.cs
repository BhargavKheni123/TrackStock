using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;


namespace eTurns.DAL
{
    public partial class UserMasterDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        public UserMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public UserMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        public UserMasterDTO GetUserByIdPlain(long Id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Id", Id) };
                return context.Database.SqlQuery<UserMasterDTO>("exec [GetUserByIdPlain] @Id", params1).FirstOrDefault();
            }
        }

        public List<UserWiseRoomsAccessDetailsDTO> GetUserAllRoomsAccessDetails(long UserId, long RoleId)
        {
            var permissionList = GetUserRoleModuleDetailsByUserIdAndRoleId(UserId, RoleId);
            string RoomLists = "";
            List<UserWiseRoomsAccessDetailsDTO> userWiseAllRoomsAccessDetails = new List<UserWiseRoomsAccessDetailsDTO>();

            if (permissionList != null && permissionList.Any())
            {
                userWiseAllRoomsAccessDetails = ConvertUserPermissions(permissionList, RoleId, ref RoomLists);
                if (userWiseAllRoomsAccessDetails != null && userWiseAllRoomsAccessDetails.Count > 0)
                {
                    List<UserAccessDTO> lstUserRoomAccess = GetUserRoomAccessesByUserId(UserId);
                    if (lstUserRoomAccess != null && lstUserRoomAccess.Count > 0)
                    {
                        foreach (var item in userWiseAllRoomsAccessDetails)
                        {
                            UserAccessDTO UserRoomAccess = lstUserRoomAccess.Where(x => x.EnterpriseId == item.EnterpriseId && x.CompanyId == item.CompanyId && x.RoomId == item.RoomID).FirstOrDefault();
                            if (UserRoomAccess != null)
                                item.SupplierIDs = UserRoomAccess.SupplierIDs;
                        }
                    }
                }

            }
            return userWiseAllRoomsAccessDetails;
        }

        /// <summary>
        /// Get Particullar Record from the User Master by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public UserMasterDTO GetUserByIdFull(long Id)
        {
            UserMasterDTO objresult = new UserMasterDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@UserID", Id) };
                objresult = context.Database.SqlQuery<UserMasterDTO>("exec [GetUserWithLicenceDetailsByUserId] @UserID", params1).FirstOrDefault();

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

        public List<UserAccessDTO> GetUserRoomAccessesByUserId(long UserId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@UserID", UserId) };
                return context.Database.SqlQuery<UserAccessDTO>("exec [GetUserRoomAccessesByUserId] @UserID", params1).ToList();
            }
        }

        public UserAccessDTO GetUserRoomAccessesByUserId(long EnterpriseID, long CompanyID, long RoomID, long UserID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@EnterpriseID", EnterpriseID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@UserID", UserID) };
                return context.Database.SqlQuery<UserAccessDTO>("exec [GetUserRoomAccessesByEntCmpRoom] @EnterpriseID,@CompanyID,@RoomID,@UserID", params1).FirstOrDefault();
            }
        }

        public List<UserWiseRoomsAccessDetailsDTO> ConvertUserPermissions(List<UserRoleModuleDetailsDTO> objData, long RoleID, ref string RoomLists)
        {
            List<UserWiseRoomsAccessDetailsDTO> objRooms = new List<UserWiseRoomsAccessDetailsDTO>();

            if (RoleID == -2)
            {
                UserWiseRoomsAccessDetailsDTO obj = new UserWiseRoomsAccessDetailsDTO();
                obj.PermissionList = objData;
                obj.RoomID = 0;
                objRooms.Add(obj);
            }
            else
            {
                UserWiseRoomsAccessDetailsDTO objRoleRooms;
                var objTempPermissionList = objData.Where(t => t.EnteriseId > 0 && t.CompanyId > 0 && t.RoomId > 0)
                                                .GroupBy(element => new { element.RoomId, element.CompanyId, element.EnteriseId })
                                                .OrderBy(g => g.Key.EnteriseId);

                List<long> roomIds = new List<long>();
                if (roomIds != null && roomIds.Any())
                {
                    roomIds = objTempPermissionList.Select(e => e.Key.RoomId).ToList();
                }

                List<RoomDTO> rooms = new List<RoomDTO>();

                if (roomIds != null && roomIds.Any())
                {
                    var roomIdStr = string.Join(",", roomIds);
                    rooms = new RoomDAL(base.DataBaseName).GetRoomByIDsNormal(roomIdStr);
                }

                foreach (var grpData in objTempPermissionList)
                {
                    objRoleRooms = new UserWiseRoomsAccessDetailsDTO();
                    objRoleRooms.EnterpriseId = grpData.Key.EnteriseId;
                    objRoleRooms.CompanyId = grpData.Key.CompanyId;
                    objRoleRooms.RoomID = grpData.Key.RoomId;
                    objRoleRooms.RoleID = RoleID;
                    //objRoleRooms.IsCompanyActive = grpData.Key.IsCompanyActive;
                    List<UserRoleModuleDetailsDTO> cps = grpData.ToList();

                    if (cps != null)
                    {
                        objRoleRooms.PermissionList = cps;
                        objRoleRooms.RoomName = cps[0].RoomName;
                        objRoleRooms.IsCompanyActive = cps[0].IsCompanyActive;
                        objRoleRooms.IsRoomActive = cps[0].IsRoomActive;
                        objRoleRooms.CompanyName = cps[0].CompanyName;
                        objRoleRooms.EnterpriseName = cps[0].EnterpriseName;
                    }

                    if (objRoleRooms.RoomID > 0)
                    {
                        RoomDTO objRoomDTO = new RoomDTO();

                        if (rooms != null && rooms.Any())
                        {
                            objRoomDTO = rooms.Where(e => e.ID == objRoleRooms.RoomID).FirstOrDefault();
                        }

                        if (objRoomDTO != null && objRoomDTO.ID > 0)
                        {
                            objRoleRooms.RoomName = objRoomDTO.RoomName;
                            objRoleRooms.IsCompanyActive = objRoomDTO.IsCompanyActive;
                            objRoleRooms.IsRoomActive = objRoomDTO.IsRoomActive;
                            objRoleRooms.CompanyName = objRoomDTO.CompanyName;
                            objRoleRooms.EnterpriseName = objRoomDTO.EnterpriseName;
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

            return objRooms;
        }

        /// <summary>
        /// Get Particullar Record from the User Master by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public List<UserRoleModuleDetailsDTO> GetUserRoleModuleDetailsByUserIdAndRoleId(long UserId, long RoleId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@UserID", UserId),
                                                   new SqlParameter("@RoleID", RoleId)};

                return context.Database.SqlQuery<UserRoleModuleDetailsDTO>("exec [GetUserRoleModuleDetailsByUserIdAndRoleId] @UserID,@RoleID", params1).ToList();
            }
        }

        public List<UserRoleModuleDetailsDTO> GetUserRoleModuleDetailsByModuleAndEnterprise(long UserId, long RoleId, long ModuleId, long EnterpriseId)
        {
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@UserID", UserId),
                                                new SqlParameter("@RoleID", RoleId),
                                                new SqlParameter("@ModuleId", ModuleId),
                                                new SqlParameter("@EnterpriseId", EnterpriseId)
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UserRoleModuleDetailsDTO>("exec GetUserRoleModuleDetailsByModuleAndEnterprise @UserID,@RoleID,@ModuleId,@EnterpriseId", params1).ToList();
            }
        }

        public List<long> GetUserOrderAllowedRooms(long UserId, long RoleId)
        {
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@UserID", UserId),
                                                new SqlParameter("@RoleID", RoleId)
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<long>("exec GetUserOrderAllowedRooms @UserID,@RoleID", params1).ToList();
            }
        }

        /// <summary>
        /// Insert Record in the DataBase Role Master
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public long Insert(UserMasterDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                bool enforceRolePermissionExisting = true;
                long retId = InsertUpdate(objDTO, Enums.DBOperation.INSERT, out enforceRolePermissionExisting);
                objDTO.ID = retId;

                if (!objDTO.EnforceRolePermission)
                {
                    List<UserRoleDetailDTO> userRoleDetails = new List<UserRoleDetailDTO>();

                    if (objDTO.UserWiseAllRoomsAccessDetails != null && objDTO.UserWiseAllRoomsAccessDetails.Count > 0)
                    {
                        foreach (UserWiseRoomsAccessDetailsDTO item in objDTO.UserWiseAllRoomsAccessDetails)
                        {
                            foreach (UserRoleModuleDetailsDTO InternalItem in item.PermissionList)
                            {
                                UserRoleDetailDTO objDetails = new UserRoleDetailDTO();
                                objDetails.UserID = objDTO.ID;
                                objDetails.RoleID = objDTO.RoleID;
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
                                userRoleDetails.Add(objDetails);
                            }
                        }
                    }

                    List<UserRoomAccessDTO> userRoomAccessDetails = new List<UserRoomAccessDTO>();

                    if (objDTO.lstAccess != null && objDTO.lstAccess.Count > 0)
                    {
                        foreach (var item in objDTO.lstAccess)
                        {
                            UserRoomAccessDTO objUserRoomAccess = new UserRoomAccessDTO();
                            objUserRoomAccess.UserId = objDTO.ID;
                            objUserRoomAccess.RoleId = objDTO.RoleID;
                            objUserRoomAccess.EnterpriseID = item.EnterpriseId;
                            objUserRoomAccess.CompanyID = item.CompanyId;
                            objUserRoomAccess.RoomId = item.RoomId;
                            if (objDTO.UserWiseAllRoomsAccessDetails != null && objDTO.UserWiseAllRoomsAccessDetails.Count > 0)
                            {
                                UserWiseRoomsAccessDetailsDTO UserRoomAccessDTO = objDTO.UserWiseAllRoomsAccessDetails.Where(x => x.EnterpriseId == item.EnterpriseId && x.CompanyId == item.CompanyId && x.RoomID == item.RoomId).FirstOrDefault();
                                if (UserRoomAccessDTO != null)
                                    objUserRoomAccess.SupplierIDs = UserRoomAccessDTO.SupplierIDs;
                            }
                            userRoomAccessDetails.Add(objUserRoomAccess);
                        }
                    }

                    DataTable UserRoleDetailTable = CommonUtilityHelper.ToDataTable(userRoleDetails); ;
                    DataTable UserRoomAccessTable = CommonUtilityHelper.ToDataTable(userRoomAccessDetails);
                    InsertUserRoleAndRoomAccessDetails(UserRoleDetailTable, UserRoomAccessTable, objDTO.ID);
                }
                else
                {
                    context.Database.ExecuteSqlCommand("EXEC CopyRolePermissionToUser " + objDTO.ID + "");

                    List<UserRoomAccessDTO> userRoomAccessDetails = new List<UserRoomAccessDTO>();
                    if (objDTO.lstAccess != null && objDTO.lstAccess.Count > 0
                        && objDTO.UserWiseAllRoomsAccessDetails != null && objDTO.UserWiseAllRoomsAccessDetails.Count > 0)
                    {
                        foreach (var item in objDTO.lstAccess)
                        {
                            UserRoomAccessDTO objUserRoomAccess = new UserRoomAccessDTO();
                            objUserRoomAccess.UserId = objDTO.ID;
                            objUserRoomAccess.RoleId = objDTO.RoleID;
                            objUserRoomAccess.EnterpriseID = item.EnterpriseId;
                            objUserRoomAccess.CompanyID = item.CompanyId;
                            objUserRoomAccess.RoomId = item.RoomId;
                            if (objDTO.UserWiseAllRoomsAccessDetails != null && objDTO.UserWiseAllRoomsAccessDetails.Count > 0)
                            {
                                UserWiseRoomsAccessDetailsDTO UserRoomAccessDTO = objDTO.UserWiseAllRoomsAccessDetails.Where(x => x.EnterpriseId == item.EnterpriseId && x.CompanyId == item.CompanyId && x.RoomID == item.RoomId).FirstOrDefault();
                                if (UserRoomAccessDTO != null)
                                    objUserRoomAccess.SupplierIDs = UserRoomAccessDTO.SupplierIDs;
                            }
                            userRoomAccessDetails.Add(objUserRoomAccess);
                        }
                    }

                    DataTable UserRoomAccessTable = ToDataTable(userRoomAccessDetails);
                    UpdateUserRoomAccessDetails(UserRoomAccessTable, objDTO.ID);
                }
            }

            SetUserScheduler(objDTO.ID, objDTO.LastUpdatedBy ?? (objDTO.CreatedBy ?? objDTO.ID));
            UpdateBillingRoomModulesByUser(objDTO.ID);
            return objDTO.ID;
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
                //UserMaster obj = context.UserMasters.FirstOrDefault(t => t.ID == objDTO.ID);


                bool CopyRolePermissionOnEdit = false;


                bool enforceRolePermissionExisting = true;
                long retId = InsertUpdate(objDTO, Enums.DBOperation.UPDATE, out enforceRolePermissionExisting);



                if (objDTO.EnforceRolePermission)
                {
                    CopyRolePermissionOnEdit = true;
                }

                if (!objDTO.EnforceRolePermission)
                {
                    List<UserRoleDetailDTO> userRoleDetails = new List<UserRoleDetailDTO>();

                    if (objDTO.UserWiseAllRoomsAccessDetails != null && objDTO.UserWiseAllRoomsAccessDetails.Count > 0)
                    {
                        foreach (UserWiseRoomsAccessDetailsDTO item in objDTO.UserWiseAllRoomsAccessDetails)
                        {
                            foreach (UserRoleModuleDetailsDTO InternalItem in item.PermissionList)
                            {
                                UserRoleDetailDTO objDetails = new UserRoleDetailDTO();
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
                                objDetails.RoleID = objDTO.RoleID;
                                objDetails.RoomId = InternalItem.RoomId;
                                objDetails.UserID = objDTO.ID;
                                objDetails.CompanyID = InternalItem.CompanyId;
                                objDetails.EnterpriseID = InternalItem.EnteriseId;
                                userRoleDetails.Add(objDetails);
                            }
                        }
                    }

                    List<UserRoomAccessDTO> userRoomAccessDetails = new List<UserRoomAccessDTO>();

                    if (objDTO.lstAccess != null && objDTO.lstAccess.Count > 0)
                    {
                        foreach (var item in objDTO.lstAccess)
                        {
                            UserRoomAccessDTO objUserRoomAccess = new UserRoomAccessDTO();
                            objUserRoomAccess.UserId = objDTO.ID;
                            objUserRoomAccess.RoleId = objDTO.RoleID;
                            objUserRoomAccess.EnterpriseID = item.EnterpriseId;
                            objUserRoomAccess.CompanyID = item.CompanyId;
                            objUserRoomAccess.RoomId = item.RoomId;
                            if (objDTO.UserWiseAllRoomsAccessDetails != null && objDTO.UserWiseAllRoomsAccessDetails.Count > 0)
                            {
                                UserWiseRoomsAccessDetailsDTO UserRoomAccessDTO = objDTO.UserWiseAllRoomsAccessDetails.Where(x => x.EnterpriseId == item.EnterpriseId && x.CompanyId == item.CompanyId && x.RoomID == item.RoomId).FirstOrDefault();
                                if (UserRoomAccessDTO != null)
                                    objUserRoomAccess.SupplierIDs = UserRoomAccessDTO.SupplierIDs;
                            }
                            userRoomAccessDetails.Add(objUserRoomAccess);
                        }
                    }

                    DataTable UserRoleDetailTable = CommonUtilityHelper.ToDataTable(userRoleDetails); ;
                    DataTable UserRoomAccessTable = CommonUtilityHelper.ToDataTable(userRoomAccessDetails);
                    UpdateUserRoleAndRoomAccessDetails(UserRoleDetailTable, UserRoomAccessTable, objDTO.ID);
                }
                else
                {
                    if (CopyRolePermissionOnEdit)
                    {
                        context.Database.ExecuteSqlCommand("EXEC CopyRolePermissionToUser " + objDTO.ID + "");
                    }

                    List<UserRoomAccessDTO> userRoomAccessDetails = new List<UserRoomAccessDTO>();
                    if (objDTO.lstAccess != null && objDTO.lstAccess.Count > 0
                        && objDTO.UserWiseAllRoomsAccessDetails != null && objDTO.UserWiseAllRoomsAccessDetails.Count > 0)
                    {
                        foreach (var item in objDTO.lstAccess)
                        {
                            UserRoomAccessDTO objUserRoomAccess = new UserRoomAccessDTO();
                            objUserRoomAccess.UserId = objDTO.ID;
                            objUserRoomAccess.RoleId = objDTO.RoleID;
                            objUserRoomAccess.EnterpriseID = item.EnterpriseId;
                            objUserRoomAccess.CompanyID = item.CompanyId;
                            objUserRoomAccess.RoomId = item.RoomId;
                            if (objDTO.UserWiseAllRoomsAccessDetails != null && objDTO.UserWiseAllRoomsAccessDetails.Count > 0)
                            {
                                UserWiseRoomsAccessDetailsDTO UserRoomAccessDTO = objDTO.UserWiseAllRoomsAccessDetails.Where(x => x.EnterpriseId == item.EnterpriseId && x.CompanyId == item.CompanyId && x.RoomID == item.RoomId).FirstOrDefault();
                                if (UserRoomAccessDTO != null)
                                    objUserRoomAccess.SupplierIDs = UserRoomAccessDTO.SupplierIDs;
                                else if (UserRoomAccessDTO == null)
                                {
                                    objUserRoomAccess.SupplierIDs = item.SupplierIDs;
                                }
                            }
                            userRoomAccessDetails.Add(objUserRoomAccess);
                        }
                    }

                    DataTable UserRoomAccessTable = ToDataTable(userRoomAccessDetails);
                    UpdateUserRoomAccessDetails(UserRoomAccessTable, objDTO.ID);

                }
            }

            UpdateBillingRoomModulesByUser(objDTO.ID);

            SetUserScheduler(objDTO.ID, objDTO.LastUpdatedBy ?? (objDTO.CreatedBy ?? objDTO.ID));
            return true;


        }

        public bool EditProfile(UserMasterDTO objDTO, bool FromEPDetail)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                UserMasterDTO obj = GetUserByIdPlain(objDTO.ID);

                if (obj != null)
                {
                    if (!FromEPDetail)
                    {
                        obj.ID = objDTO.ID;
                        obj.UserName = objDTO.UserName;

                        if (!string.IsNullOrWhiteSpace(objDTO.Password))
                        {
                            obj.Password = objDTO.Password;
                        }

                        obj.Email = objDTO.Email;
                        obj.Phone = objDTO.Phone;
                        obj.Updated = DateTimeUtility.DateTimeNow;
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
                        context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        obj.ID = objDTO.ID;
                        obj.UserName = objDTO.UserName;

                        if (!string.IsNullOrWhiteSpace(objDTO.Password))
                        {
                            obj.Password = objDTO.Password;
                        }

                        obj.Email = objDTO.Email;
                        obj.Updated = DateTimeUtility.DateTimeNow;
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
                        context.SaveChanges();
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EditForSuperAdminProfile(UserMasterDTO objDTO)
        {
            try
            {
                if (objDTO != null && objDTO.ID > 0)
                {
                    using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                    {
                        var params1 = new SqlParameter[]
                          {
                                new SqlParameter("@ID", objDTO.ID )
                                , new SqlParameter("@UserName", objDTO.UserName ?? (object)DBNull.Value)
                                , new SqlParameter("@Email", objDTO.Email ?? (object)DBNull.Value)
                                , new SqlParameter("@Phone", objDTO.Phone ?? (object)DBNull.Value)
                                , new SqlParameter("@Prefix", objDTO.Prefix ?? (object)DBNull.Value)
                                , new SqlParameter("@FirstName", objDTO.FirstName?? (object)DBNull.Value)
                                , new SqlParameter("@MiddleName", objDTO.MiddleName?? (object)DBNull.Value)
                                , new SqlParameter("@LastName", objDTO.LastName?? (object)DBNull.Value)
                                , new SqlParameter("@Gender", objDTO.Gender?? (object)DBNull.Value)
                                , new SqlParameter("@Phone2", objDTO.Phone2?? (object)DBNull.Value)
                                , new SqlParameter("@EmployeeNumber", objDTO.EmployeeNumber?? (object)DBNull.Value)
                                , new SqlParameter("@CostCenter", objDTO.CostCenter?? (object)DBNull.Value)
                                , new SqlParameter("@JobTitle", objDTO.JobTitle?? (object)DBNull.Value)
                                , new SqlParameter("@Address", objDTO.Address?? (object)DBNull.Value)
                                , new SqlParameter("@Country", objDTO.Country?? (object)DBNull.Value)
                                , new SqlParameter("@State", objDTO.State?? (object)DBNull.Value)
                                , new SqlParameter("@City", objDTO.City?? (object)DBNull.Value)
                                , new SqlParameter("@PostalCode", objDTO.PostalCode?? (object)DBNull.Value)
                        };

                        context.Database.CommandTimeout = 600;
                        context.Database.ExecuteSqlCommand("exec [UpdateUserMasterData] @ID,@UserName,@Email,@Phone,@Prefix,@FirstName,@MiddleName,@LastName,@Gender,@Phone2,@EmployeeNumber,@CostCenter,@JobTitle,@Address,@Country,@State,@City, @PostalCode", params1);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<UserWiseRoomsAccessDetailsDTO> GetEnterpriseAdminRoomPermissions(long EnterpriseId, long CompanyID, long RoomId, long RoleId, long UserId)
        {
            List<UserWiseRoomsAccessDetailsDTO> lstpermissions = new List<UserWiseRoomsAccessDetailsDTO>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                UserWiseRoomsAccessDetailsDTO objUserWiseRoomsAccessDetailsDTO = new UserWiseRoomsAccessDetailsDTO();
                objUserWiseRoomsAccessDetailsDTO.EnterpriseId = EnterpriseId;
                objUserWiseRoomsAccessDetailsDTO.CompanyId = CompanyID;
                objUserWiseRoomsAccessDetailsDTO.RoomID = RoomId;
                objUserWiseRoomsAccessDetailsDTO.RoleID = RoleId;

                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@EnterpriseId", EnterpriseId),
                                                    new SqlParameter("@CompanyID", CompanyID),
                                                    new SqlParameter("@RoomId", RoomId),
                                                    new SqlParameter("@RoleId", RoleId),
                                                    new SqlParameter("@UserId", UserId)
                                                };

                objUserWiseRoomsAccessDetailsDTO.PermissionList = context.Database.SqlQuery<UserRoleModuleDetailsDTO>("exec [GetEnterpriseAdminRoomPermissions] @EnterpriseId,@CompanyID,@RoomId,@RoleId,@UserId", params1).ToList();
                lstpermissions.Add(objUserWiseRoomsAccessDetailsDTO);
            }
            return lstpermissions;
        }

        #region [for service]

        #endregion
        public void UpdateTermsCondition(long id, bool IsLicenceAccepted)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@UserId", id),
                                                    new SqlParameter("@IsLicenceAccepted", IsLicenceAccepted)
                                                };
                context.Database.ExecuteSqlCommand("exec [UpdateUserLicenceAcceptance] @UserId,@IsLicenceAccepted", params1);
            }
        }

        public void UpdatePassword(UserPasswordChangeHistoryDTO objDTO)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@UserId", objDTO.UserId),
                                                    new SqlParameter("@Password", objDTO.NewPassword)
                                                };
                context.Database.ExecuteSqlCommand("exec [UpdateUserPassword] @UserId,@Password", params1);
            }
        }

        public UserMasterDTO GetUserDetailsByEmail(string Email)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@Email", Email)
                                                };
                return context.Database.SqlQuery<UserMasterDTO>("exec [GetUserDetailsByEmail] @Email", params1).FirstOrDefault();
            }
        }

        public List<UserAccessDTO> GetUserAccessWithNames(long UserId, string EnterpriseName, bool IsEnterpriseActive)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@UserId", UserId),
                                                    new SqlParameter("@EnterpriseName", EnterpriseName),
                                                    new SqlParameter("@IsEnterpriseActive", IsEnterpriseActive)
                                                };

                return context.Database.SqlQuery<UserAccessDTO>("exec [GetUserAccessWithNames] @UserId,@EnterpriseName,@IsEnterpriseActive", params1).ToList();
            }
        }

        public List<UserAccessDTO> GetUserAccessWithNames(long UserId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@UserId", UserId),
                                                    new SqlParameter("@EnterpriseName", DBNull.Value),
                                                    new SqlParameter("@IsEnterpriseActive", DBNull.Value)
                                                };

                return context.Database.SqlQuery<UserAccessDTO>("exec [GetUserAccessWithNames] @UserId,@EnterpriseName,@IsEnterpriseActive", params1).ToList();
            }
        }

        public List<UserRoleModuleDetailsDTO> GetUserSupplierDetails(long UserId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<UserRoleModuleDetailsDTO> UserSupplierDetails = new List<UserRoleModuleDetailsDTO>();

                if (UserId > 0)
                {
                    var params1 = new SqlParameter[] { new SqlParameter("@UserId", UserId) };
                    UserSupplierDetails = context.Database.SqlQuery<UserRoleModuleDetailsDTO>("exec [GetUserSupplierDetails] @UserId", params1).ToList();
                }

                return UserSupplierDetails;
            }
        }

        public UserMasterDTO GetSystemUser(int UserType)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@UserType", UserType) };
                return context.Database.SqlQuery<UserMasterDTO>("exec [GetSystemUserByUserType] @UserType", params1).FirstOrDefault();
            }
        }

        public string GetUserNameByUserId(long UserId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@UserId", UserId) };
                return context.Database.SqlQuery<string>("exec [GetUserNameByUserId] @UserId", params1).FirstOrDefault();
            }
        }

        public void UpdateUserRoleByUserId(long UserId, long RoleId)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@UserId", UserId),
                                                    new SqlParameter("@RoleId", RoleId)
                                                };
                context.Database.ExecuteSqlCommand("exec [UpdateUserRoleByUserId] @UserId,@RoleId", params1);
            }
        }

        public List<UserRoleModuleDetailsDTO> GetUserRoleModuleDetails(long UserId, long RoleId, long RoomId, long CompanyId, string ModuleIds)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@UserId", UserId),
                                                    new SqlParameter("@RoleId", RoleId),
                                                    new SqlParameter("@RoomId", RoomId),
                                                    new SqlParameter("@CompanyId", CompanyId),
                                                    new SqlParameter("@ModuleIds", ModuleIds)
                                                };
                return context.Database.SqlQuery<UserRoleModuleDetailsDTO>("exec [GetUserRoleModuleDetails] @UserId,@RoleId,@RoomId,@CompanyId,@ModuleIds", params1).ToList();
            }
        }

        public UserMasterDTO GetEnterpriseSystemUser()
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UserMasterDTO>("exec [GetEnterpriseSystemUser] ").FirstOrDefault();
            }
        }

        public UserRoleModuleDetailsDTO GetUserModulePermission(long UserID, long RoleID, long EnterpriseID, long CompanyID, long RoomID, int ModuleID, long UserType)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@UserID", UserID), new SqlParameter("@RoleID", RoleID), new SqlParameter("@UserType", UserType), new SqlParameter("@EnterpriseID", EnterpriseID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@ModuleID", ModuleID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UserRoleModuleDetailsDTO>("exec [GetUserRoleModuleDetailsRecordSingle] @UserID,@RoleID,@UserType,@EnterpriseID,@CompanyID,@RoomID,@ModuleID", params1).FirstOrDefault();
            }
        }

        public List<UserRoleModuleDetailsDTO> GetUserRoleChangeLogRecords(int StartRowIndex, int MaxRows, out int TotalCount, long UserId, string Search)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                TotalCount = 0;
                StartRowIndex = int.Parse(Math.Ceiling((double)(StartRowIndex / MaxRows)).ToString());
                var paramCmp = new SqlParameter[]
                                                  {
                                                    new SqlParameter("@UserID", UserId),
                                                    new SqlParameter("@StartRowIndex", StartRowIndex),
                                                    new SqlParameter("@MaxRows", MaxRows),
                                                    new SqlParameter("@Search", Search)
                                                  };

                var userRoleModuleDetails = context.Database.SqlQuery<UserRoleModuleDetailsDTO>("exec [GetUserRolePermissionHistory] @UserID,@StartRowIndex,@MaxRows,@Search", paramCmp).ToList();

                if (userRoleModuleDetails != null && userRoleModuleDetails.Count() > 0)
                {
                    TotalCount = userRoleModuleDetails.ElementAt(0).TotalRecords;
                }

                return userRoleModuleDetails;
            }
        }

        public List<RoomModuleDetailsDTO> GetUserRoleModuleDetailsForAPI(long UserID, long RoleID, long RoomID)
        {
            List<RoomModuleDetailsDTO> lstAccess = new List<RoomModuleDetailsDTO>();

            var params1 = new SqlParameter[] {
                                                new SqlParameter("@RoleID", RoleID),
                                                new SqlParameter("@UserID", UserID),
                                                new SqlParameter("@RoomID", RoomID)
                                                 };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstAccess = context.Database.SqlQuery<RoomModuleDetailsDTO>("EXEC GetUserRoleModuleDetailsForAPI @RoleID,@UserID,@RoomID", params1).ToList();
            }

            return lstAccess;
        }


        /// <summary>
        /// 125 Module ID is for Order Approval Limit
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="EnterpriseID"></param>
        /// <param name="CompanyID"></param>
        /// <param name="RoomID"></param>
        /// <param name="ModuleID"></param>
        /// <returns></returns>
        public DollerApprovalLimitDTO GetOrderLimitByUserId(long UserID, long CompanyID, long RoomID, int ModuleID = 125)
        {
            DollerApprovalLimitDTO dollerLimit = null;
            var params1 = new SqlParameter[] {
                new SqlParameter("@UserID", UserID),
                new SqlParameter("@CompanyID", CompanyID),
                new SqlParameter("@RoomID", RoomID),
                new SqlParameter("@ModuleID", ModuleID) };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string moduleValue = context.Database.SqlQuery<string>("EXEC GetUserRoleDetailsModuleValue @UserID,@CompanyID,@RoomID,@ModuleID", params1).FirstOrDefault();

                if (!string.IsNullOrEmpty(moduleValue) && !moduleValue.ToLower().Equals("nolimit") && moduleValue.Contains("|"))
                {
                    string[] value = moduleValue.Split(new char[1] { '|' });

                    if (value != null && value.Length > 0)
                    {
                        dollerLimit = new DollerApprovalLimitDTO();
                        int fValue = 0;
                        int.TryParse(value[0], out fValue);
                        dollerLimit.FrequencyValue = fValue;

                        if (value != null && value.Length > 1)
                        {
                            ApprovalFrequencyType appFreqType;
                            Enum.TryParse<ApprovalFrequencyType>(value[1], out appFreqType);
                            dollerLimit.FrequencyType = appFreqType;

                            if (value != null && value.Length > 2)
                            {
                                double dValue = 0;
                                double.TryParse(value[2], out dValue);
                                dollerLimit.DollerLimit = dValue;

                                if (value != null && value.Length > 3)
                                {
                                    dValue = 0;
                                    double.TryParse(value[3], out dValue);
                                    dollerLimit.UsedLimit = dValue;

                                    if (value != null && value.Length > 4)
                                    {
                                        OrderLimitType appOLType;
                                        Enum.TryParse<OrderLimitType>(value[4], out appOLType);
                                        dollerLimit.OrderLimitType = appOLType;

                                        if (value != null && value.Length > 5)
                                        {
                                            double ItemApprovedQuantity = 0;
                                            double.TryParse(value[5], out ItemApprovedQuantity);

                                            if (ItemApprovedQuantity > 0)
                                                dollerLimit.ItemApprovedQuantity = ItemApprovedQuantity;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            dollerLimit = null;
                        }
                    }
                }
            }

            if (dollerLimit != null && (dollerLimit.DollerLimit == 0 && dollerLimit.ItemApprovedQuantity == 0))
            {
                dollerLimit = null;
            }

            return dollerLimit;
        }

        public void SaveDollerUsedLimt(double UsedValue, long UserID, long CompanyID, long RoomID, int ModuleID = 125)
        {
            string ModuleValue = string.Empty;
            DollerApprovalLimitDTO dollerLimit = GetOrderLimitByUserId(UserID, CompanyID, RoomID, ModuleID);

            if (dollerLimit != null)
            {
                ModuleValue = dollerLimit.FrequencyValue.ToString();
                ModuleValue += "|" + ((int)dollerLimit.FrequencyType).ToString();
                ModuleValue += "|" + dollerLimit.DollerLimit.ToString();
                ModuleValue += "|" + (dollerLimit.UsedLimit + UsedValue).ToString();
                ModuleValue += "|" + ((int)dollerLimit.OrderLimitType).ToString();

                var params1 = new SqlParameter[] {
                new SqlParameter("@ModuleValue", ModuleValue),
                new SqlParameter("@UserID", UserID),
                new SqlParameter("@CompanyID", CompanyID),
                new SqlParameter("@RoomID", RoomID),
                new SqlParameter("@ModuleID", ModuleID) };

                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    context.Database.ExecuteSqlCommand("EXEC UpdateUserRoleDetailsModuleValue @ModuleValue,@UserID,@CompanyID,@RoomID,@ModuleID", params1);
                }
            }
        }

        public void SetUserScheduler(long UserID, long CreatedBy)
        {
            SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(base.DataBaseName);
            List<UserSchedulerDTO> ScheduleConfigFromPermission = GetUserSchedules(UserID);
            List<UserSchedulerDTO> ScheduleConfigFromPermissionUpdated = new List<UserSchedulerDTO>();

            if (ScheduleConfigFromPermission != null && ScheduleConfigFromPermission.Count > 0)
            {
                List<SchedulerDTO> lstSchedules = objSupplierMasterDAL.GetRoomSchedulesForUser(UserID, 9);

                foreach (var Lineitem in ScheduleConfigFromPermission)
                {
                    UserSchedulerDTO objUserSchedulerDTO = new UserSchedulerDTO();
                    objUserSchedulerDTO.ActionToDo = string.Empty;
                    objUserSchedulerDTO.CompanyID = Lineitem.CompanyID;
                    objUserSchedulerDTO.EnterpriseID = Lineitem.EnterpriseID;
                    objUserSchedulerDTO.ModuleID = Lineitem.ModuleID;
                    objUserSchedulerDTO.ModuleValue = Lineitem.ModuleValue;
                    objUserSchedulerDTO.RoleID = Lineitem.RoleID;
                    objUserSchedulerDTO.RoomID = Lineitem.RoomID;
                    objUserSchedulerDTO.UserID = Lineitem.UserID;

                    int Frequency = 0;
                    int TimeBaseUnit = 0;
                    float OrderLimit = 0;
                    float OrderUsedLimit = 0;
                    int LimitType = 0;
                    string[] Arrayparams = (Lineitem.ModuleValue ?? string.Empty).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

                    if (Arrayparams.Length > 0)
                    {
                        int.TryParse(Arrayparams[0], out Frequency);
                    }
                    if (Arrayparams.Length > 1)
                    {
                        int.TryParse(Arrayparams[1], out TimeBaseUnit);
                    }
                    if (Arrayparams.Length > 2)
                    {
                        float.TryParse(Arrayparams[2], out OrderLimit);
                    }
                    if (Arrayparams.Length > 3)
                    {
                        float.TryParse(Arrayparams[3], out OrderUsedLimit);
                    }
                    if (Arrayparams.Length > 4)
                    {
                        int.TryParse(Arrayparams[4], out LimitType);
                    }
                    objUserSchedulerDTO.Frequency = Frequency;
                    objUserSchedulerDTO.TimeBaseUnit = TimeBaseUnit;
                    objUserSchedulerDTO.OrderLimit = OrderLimit;
                    objUserSchedulerDTO.OrderUsedLimit = OrderUsedLimit;
                    objUserSchedulerDTO.LimitType = LimitType;

                    if (TimeBaseUnit > 0 && Frequency > 0)
                    {
                        SchedulerDTO objSchedulerDTOUser = lstSchedules.FirstOrDefault(t => t.RoomId == Lineitem.RoomID && t.CompanyId == Lineitem.CompanyID);
                        if (objSchedulerDTOUser != null)
                        {
                            objUserSchedulerDTO.ScheduleID = objSchedulerDTOUser.ScheduleID;
                            if (TimeBaseUnit == 1)
                            {
                                if (objSchedulerDTOUser.DailyRecurringDays == Frequency)
                                {
                                    objUserSchedulerDTO.ActionToDo = "nochange";
                                }
                                else
                                {
                                    objUserSchedulerDTO.ActionToDo = "update";
                                }
                            }
                            if (TimeBaseUnit == 2)
                            {
                                if (objSchedulerDTOUser.WeeklyRecurringWeeks == Frequency)
                                {
                                    objUserSchedulerDTO.ActionToDo = "nochange";
                                }
                                else
                                {
                                    objUserSchedulerDTO.ActionToDo = "update";
                                }
                            }
                            if (TimeBaseUnit == 3)
                            {
                                if (objSchedulerDTOUser.MonthlyRecurringMonths == Frequency)
                                {
                                    objUserSchedulerDTO.ActionToDo = "nochange";
                                }
                                else
                                {
                                    objUserSchedulerDTO.ActionToDo = "update";
                                }
                            }
                        }
                        else
                        {
                            objUserSchedulerDTO.ActionToDo = "insert";
                        }

                    }
                    else
                    {
                        objSupplierMasterDAL.DeleteAllSchedulesForUserForRoom(UserID, 9, Lineitem.CompanyID, Lineitem.RoomID);
                        // Delete Schedule for this User for this Room-company
                    }
                    ScheduleConfigFromPermissionUpdated.Add(objUserSchedulerDTO);
                }
            }
            else
            {

                objSupplierMasterDAL.DeleteAllSchedulesForUser(UserID, 9);
                // Delete All Schedulers for this user if any
                // Days : 1
                // weeks : 2
                // Month : 3
                // year : 4

            }
            if (ScheduleConfigFromPermissionUpdated != null)
            {
                foreach (var item in ScheduleConfigFromPermissionUpdated)
                {
                    SchedulerDTO SupplierScheduleDTO = new SchedulerDTO();
                    SupplierScheduleDTO.AssetToolID = null;
                    SupplierScheduleDTO.AttachmentReportIDs = null;
                    SupplierScheduleDTO.AttachmentTypes = null;
                    SupplierScheduleDTO.BinNumber = null;
                    SupplierScheduleDTO.CompanyId = item.CompanyID;
                    SupplierScheduleDTO.Created = DateTime.UtcNow;
                    SupplierScheduleDTO.CreatedBy = CreatedBy;
                    SupplierScheduleDTO.CreatedByName = string.Empty;

                    if (item.TimeBaseUnit == 1)
                    {
                        SupplierScheduleDTO.ScheduleMode = 1;
                        SupplierScheduleDTO.DailyRecurringType = 1;
                        SupplierScheduleDTO.DailyRecurringDays = item.Frequency > 32766 ? (short)(32766) : (short)(item.Frequency);
                    }

                    if (item.TimeBaseUnit == 2)
                    {
                        SupplierScheduleDTO.ScheduleMode = 2;
                        SupplierScheduleDTO.WeeklyRecurringWeeks = item.Frequency > 32766 ? (short)(32766) : (short)(item.Frequency);
                        SupplierScheduleDTO.WeeklyOnMonday = true;
                    }

                    if (item.TimeBaseUnit == 3)
                    {
                        SupplierScheduleDTO.ScheduleMode = 3;
                        SupplierScheduleDTO.MonthlyRecurringType = 1;
                        SupplierScheduleDTO.MonthlyDateOfMonth = 1;
                        SupplierScheduleDTO.MonthlyRecurringMonths = item.Frequency > 32766 ? (short)(32766) : (short)(item.Frequency);
                    }

                    SupplierScheduleDTO.EmailAddress = string.Empty;
                    SupplierScheduleDTO.EmailTempateId = 0;
                    SupplierScheduleDTO.IsScheduleActive = true;
                    SupplierScheduleDTO.LastUpdatedBy = CreatedBy;
                    SupplierScheduleDTO.LoadSheduleFor = 9;
                    SupplierScheduleDTO.NextRunDate = null;
                    SupplierScheduleDTO.RecalcSchedule = false;
                    SupplierScheduleDTO.RoomId = item.RoomID;
                    SupplierScheduleDTO.ScheduleRunTime = "23:59:00";
                    SupplierScheduleDTO.ScheduleTime = new TimeSpan(23, 59, 0);
                    SupplierScheduleDTO.UserID = item.UserID;
                    SupplierScheduleDTO.ScheduledBy = CreatedBy;
                    SupplierScheduleDTO.ScheduleID = item.ScheduleID;

                    if (item.ActionToDo != "nochange")
                    {
                        objSupplierMasterDAL.SaveSupplierSchedule(SupplierScheduleDTO);
                    }
                }
            }
        }

        public List<UserSchedulerDTO> GetUserSchedules(long UserID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@userid", UserID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UserSchedulerDTO>("EXEC [GetUserSchedules] @userid", params1).ToList();
            }
        }

        public List<UserSchedulerDTO> UpdateUserOrderLimits(long UserID, long CompanyID, long RoomID)
        {
            var params1 = new SqlParameter[] {
                                               new SqlParameter("@userid", UserID),
                                               new SqlParameter("@CompanyID", CompanyID),
                                               new SqlParameter("@RoomID", RoomID)
                                            };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UserSchedulerDTO>("EXEC [UpdateUserOrderLimits] @userid,@CompanyID,@RoomID", params1).ToList();
            }
        }

        public bool SaveUserSupplierFilter(long UserId, List<UserWiseRoomsAccessDetailsDTO> UserWiseAllRoomsAccessDetails, Dictionary<long, string> EnterpriseList, long SuperAdminId)
        {
            foreach (KeyValuePair<long, string> enterprise in EnterpriseList.Where(e => !string.IsNullOrEmpty(e.Value)))
            {
                var userWiseAllRoomsAccessDetails = UserWiseAllRoomsAccessDetails.Where(e => e.EnterpriseId == enterprise.Key).ToList();

                var userSupplierFilterParam = (from u in userWiseAllRoomsAccessDetails
                                               select new UserSupplierFilterParam
                                               {
                                                   RoomId = u.RoomID,
                                                   CompanyId = u.CompanyId,
                                               }).ToList();


                if (userSupplierFilterParam != null && userSupplierFilterParam.Any())
                {
                    DataTable UserSupplierFilterParam = new DataTable();
                    UserSupplierFilterParam = CommonUtilityHelper.ToDataTable(userSupplierFilterParam);
                    string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(enterprise.Value, DbConnectionType.GeneralReadWrite.ToString("F"));
                    SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
                    DataSet dsBins = SqlHelper.ExecuteDataset(EturnsConnection, "InsertUserSupplierFilter", UserId, SuperAdminId, enterprise.Key, UserSupplierFilterParam);
                }
            }

            return true;
        }

        public bool DeleteUserSupplierFilter(long UserId, List<UserWiseRoomsAccessDetailsDTO> UserWiseAllRoomsAccessDetails, Dictionary<long, string> EnterpriseList)
        {
            foreach (KeyValuePair<long, string> enterprise in EnterpriseList.Where(e => !string.IsNullOrEmpty(e.Value)))
            {
                var userWiseAllRoomsAccessDetails = UserWiseAllRoomsAccessDetails.Where(e => e.EnterpriseId == enterprise.Key).ToList();
                var cmpIds = userWiseAllRoomsAccessDetails.Select(e => e.CompanyId).Distinct().ToList();
                var roomIds = userWiseAllRoomsAccessDetails.Where(e => cmpIds.Contains(e.CompanyId)).Select(e => e.RoomID).Distinct().ToList();

                if (cmpIds != null && cmpIds.Any() && roomIds != null && roomIds.Any())
                {
                    string companyIds = string.Join(",", cmpIds);
                    string roomIdList = string.Join(",", roomIds);

                    using (var context = new eTurnsEntities(DbConnectionHelper.GeteTurnsEntityFWConnectionString(enterprise.Value, DbConnectionType.EFReadWrite.ToString("F"))))
                    {
                        string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(enterprise.Value, DbConnectionType.GeneralReadWrite.ToString("F"));
                        SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
                        DataSet dsBins = SqlHelper.ExecuteDataset(EturnsConnection, "DeleteUserSupplierFilter", UserId, enterprise.Key, companyIds, roomIdList);
                    }
                }
            }
            return true;
        }

        private void InsertUserRoleAndRoomAccessDetails(DataTable UserRoleDetailTable, DataTable UserRoomAccessTable, long UserId)
        {
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            DataSet dsBins = SqlHelper.ExecuteDataset(EturnsConnection, "InsertUserRoleAndRoomAccessDetails", UserId, UserRoleDetailTable, UserRoomAccessTable);
        }

        private void UpdateUserRoleAndRoomAccessDetails(DataTable UserRoleDetailTable, DataTable UserRoomAccessTable, long UserId)
        {
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            DataSet dsBins = SqlHelper.ExecuteDataset(EturnsConnection, "UpdateUserRoleAndRoomAccessDetails", UserId, UserRoleDetailTable, UserRoomAccessTable);
        }

        private long InsertUpdate(UserMasterDTO objDTO, Enums.DBOperation dBOperation, out bool enforceRolePermissionExisting)
        {
            enforceRolePermissionExisting = true;

            List<SqlParameter> objPara = new List<SqlParameter>() {
                 new SqlParameter("@Operation",dBOperation.ToString()),
                new SqlParameter("@ID",objDTO.ID),
                new SqlParameter("@UserName", objDTO.UserName),
                new SqlParameter("@Phone", objDTO.Phone),
                new SqlParameter("@Email", objDTO.Email),
                new SqlParameter("@RoleId", objDTO.RoleID),
                new SqlParameter("@CreatedBy", objDTO.CreatedBy),
                new SqlParameter("@LastUpdatedBy", objDTO.LastUpdatedBy),

                new SqlParameter("@Room", objDTO.Room),
                new SqlParameter("@CompanyID", objDTO.CompanyID),

                new SqlParameter("@UserType", objDTO.UserType),

                new SqlParameter("@EnforceRolePermission", objDTO.EnforceRolePermission),
                new SqlParameter("@IseTurnsAdmin", objDTO.IseTurnsAdmin),
                new SqlParameter("@Prefix", objDTO.Prefix),
                new SqlParameter("@FirstName", objDTO.FirstName),
                new SqlParameter("@MiddleName", objDTO.MiddleName),
                new SqlParameter("@LastName", objDTO.LastName),
                new SqlParameter("@Gender", objDTO.Gender),
                new SqlParameter("@Phone2", objDTO.Phone2),
                new SqlParameter("@EmployeeNumber", objDTO.EmployeeNumber),
                new SqlParameter("@CostCenter", objDTO.CostCenter),
                new SqlParameter("@JobTitle", objDTO.JobTitle),
                new SqlParameter("@Address", objDTO.Address),
                new SqlParameter("@Country", objDTO.Country),
                new SqlParameter("@State", objDTO.State),
                new SqlParameter("@City", objDTO.City),
                new SqlParameter("@PostalCode", objDTO.PostalCode),
                new SqlParameter("@UDF1", objDTO.UDF1),
                new SqlParameter("@UDF2", objDTO.UDF2),
                new SqlParameter("@UDF3", objDTO.UDF3),
                new SqlParameter("@UDF4", objDTO.UDF4),
                new SqlParameter("@UDF5", objDTO.UDF5),
                new SqlParameter("@UDF6", objDTO.UDF6),
                new SqlParameter("@UDF7", objDTO.UDF7),
                new SqlParameter("@UDF8", objDTO.UDF8),
                new SqlParameter("@UDF9", objDTO.UDF9),
                new SqlParameter("@UDF10", objDTO.UDF10)

            };

            if (dBOperation == Enums.DBOperation.INSERT)
            {
                objPara.Add(new SqlParameter("@GUID", Guid.NewGuid()));
                objPara.Add(new SqlParameter("@Created", DateTimeUtility.DateTimeNow));
                objPara.Add(new SqlParameter("@Updated", DateTimeUtility.DateTimeNow));
                objPara.Add(new SqlParameter("@IsDeleted", false));
                objPara.Add(new SqlParameter("@IsArchived", false));
                objPara.Add(new SqlParameter("@Password", objDTO.Password));
            }
            else if (dBOperation == Enums.DBOperation.UPDATE)
            {
                objPara.Add(new SqlParameter("@GUID", DBNull.Value));
                objPara.Add(new SqlParameter("@Created", DateTimeUtility.DateTimeNow));
                objPara.Add(new SqlParameter("@Updated", DateTimeUtility.DateTimeNow));
                objPara.Add(new SqlParameter("@IsDeleted", objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false));
                objPara.Add(new SqlParameter("@IsArchived", objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false));

                if (!string.IsNullOrWhiteSpace(objDTO.Password))
                {
                    objPara.Add(new SqlParameter("@Password", objDTO.Password));
                }
                else
                {
                    objPara.Add(new SqlParameter("@Password", DBNull.Value));
                }

            }

            objPara.Add(new SqlParameter("@IsNgNLFAllowed", objDTO.IsNgNLFAllowed));
            DataSet ds = SqlHelper.ExecuteDataset(base.DataBaseConnectionString, "uspAddUpdateUserForEnt", objPara.ToArray());
            long retId = 0;

            if (ds.Tables.Count > 0)
            {
                retId = Convert.ToInt64(ds.Tables[0].Rows[0]["RetId"]);
                enforceRolePermissionExisting = Convert.ToBoolean(ds.Tables[0].Rows[0]["EnforceRolePermissionExisting"]);
            }

            return retId;

        }
        public List<UserRoleModuleDetailsDTO> GetUserHistoryByIdNormal(string IDs)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@IDs", IDs ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UserRoleModuleDetailsDTO>("exec [GetUserHistoryByIdNormal] @IDs", params1).ToList();
            }
        }

        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            System.Reflection.PropertyInfo[] Props = typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach (System.Reflection.PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        private void UpdateUserRoomAccessDetails(DataTable UserRoomAccessTable, long UserId)
        {
            var paramUserRoomAccess = new SqlParameter("@UserRoomAccessTable", UserRoomAccessTable);
            paramUserRoomAccess.SqlDbType = SqlDbType.Structured;
            paramUserRoomAccess.TypeName = "dbo.UserRoomAccessTable";

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@UserID", UserId),
                                                    paramUserRoomAccess
                                                };
                context.Database.ExecuteSqlCommand("exec UpdateUserRoomAccessDetails @UserID,@UserRoomAccessTable", params1);
            }
        }

        public void DeleteUserByIds(string IDs, long UserId)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@UserID", UserId),
                                                    new SqlParameter("@Ids", IDs)
                                                };

                context.Database.ExecuteSqlCommand("exec [DeleteUserByIds] @UserID,@Ids", params1);
            }
        }

        /// <summary>
        /// Enable or Disable modules in user and role permissions if room billig type is updated
        /// </summary>
        /// <param name="roomID"></param>
        //public void EnableDisableModuleByRoomBillingType(long roomID)
        //{
        //    var params1 = new SqlParameter[] { new SqlParameter("@RoomID", roomID) };
        //    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {
        //        var r = context.ExecuteStoreCommand("exec [EnableDisableModuleByRoomBillingType] @RoomID", params1);
        //    }
        //}

        public void UpdateBillingRoomModulesByUser(long userID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@UserID", userID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var r = context.Database.ExecuteSqlCommand("EXEC dbo.updateBillingRoomModulesByUser @UserID", params1);
            }
        }
        public void AddNewRoomPermissionsNew(long EnterpriseId, long CompanyID, long RoomId, long UserId, string MasterDBName)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("EXEC csp_AddNewRoomPermissions " + EnterpriseId.ToString() + ", " + CompanyID.ToString() + ", " + RoomId.ToString() + ", " + UserId.ToString() + ", '" + MasterDBName + "'");
            }
        }


        public DataSet GetUserPermissionViolationList()
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                //DataSet ds = SqlHelper.ExecuteDataset(DataBaseEntityConnectionString, "GetUserPermissionViolationList");


                //context.Database.ExecuteSqlCommand("EXEC CopyRolePermissionToUser " + objDTO.ID + "");

                //return context.Database.SqlQuery<DataSet>("exec GetUserPermissionViolationList", null).FirstOrDefault();

                string sqlConnectionString = DbConnectionHelper.GeteTurnsMasterSQLConnectionString(DbConnectionHelper.GetETurnsMasterDBName(), DbConnectionType.GeneralReadWrite.ToString("F"));
                SqlConnection EturnsMasterDbConnection = new SqlConnection(sqlConnectionString);

                DataSet ds = SqlHelper.ExecuteDataset(EturnsMasterDbConnection, "GetUserPermissionViolationList");

                return ds;
            }
        }

    }//class
}

