using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

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
        public IEnumerable<UserMasterDTO> GetAllSuperUser()
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UserMasterDTO>("exec [GetAllSuperUser] ").ToList();
            }
        }

        public UserMasterDTO GetUserByIdPlain(long Id)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Id", Id) };
                return context.Database.SqlQuery<UserMasterDTO>("exec [GetUserByIdPlain] @Id", params1).FirstOrDefault();
            }
        }

        /// <summary>
        /// Get Particullar Record from the User Master by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public UserMasterDTO GetUserByIdFull(long Id)
        {
            UserMasterDTO objresult = new UserMasterDTO();
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@UserID", Id) };
                objresult = context.Database.SqlQuery<UserMasterDTO>("EXEC [GetUserWithLicenceDetailsByUserId] @UserID", params1).FirstOrDefault();

                if (objresult != null)
                {
                    objresult.PermissionList = GetUserRoleModuleDetailsRecord(Id, objresult.RoleID, objresult.UserType);
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

                    objresult.ReplenishingRooms = GetUserRoomReplanishmentDetailsRecord(Id);
                }
            }

            return objresult;
        }

        public List<UserWiseRoomsAccessDetailsDTO> GetUserAllRoomsAccessDetails(long UserId, long RoleId, int UserType)
        {
            var permissionList = GetUserRoleModuleDetailsRecord(UserId, RoleId, UserType);
            string RoomLists = "";
            List<UserWiseRoomsAccessDetailsDTO> userWiseAllRoomsAccessDetails = new List<UserWiseRoomsAccessDetailsDTO>();

            if (permissionList != null && permissionList.Any())
            {
                userWiseAllRoomsAccessDetails = ConvertUserPermissions(permissionList, RoleId, ref RoomLists);
                if (userWiseAllRoomsAccessDetails != null && userWiseAllRoomsAccessDetails.Count > 0)
                {
                    List<UserAccessDTO> lstUserRoomAccess = GetUserRoomAccessByUserIdPlain(UserId);
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

        public UserMasterDTO GetUserWithLicenceDetailsByUserId(long Id)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@UserID", Id) };
                return context.Database.SqlQuery<UserMasterDTO>("EXEC [GetUserWithLicenceDetailsByUserId] @UserID", params1).FirstOrDefault();
            }
        }

        public List<UserWiseRoomsAccessDetailsDTO> ConvertUserPermissions(List<UserRoleModuleDetailsDTO> objData, Int64 RoleID, ref string RoomLists)
        {
            EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
            List<EnterpriseDTO> lstEnterprises = new List<EnterpriseDTO>();
            List<UserWiseRoomsAccessDetailsDTO> objRooms = new List<UserWiseRoomsAccessDetailsDTO>();

            if (RoleID == -1)
            {
                UserWiseRoomsAccessDetailsDTO obj = new UserWiseRoomsAccessDetailsDTO();
                obj.PermissionList = objData;
                obj.RoomID = 0;
                obj.CompanyId = 0;
                obj.EnterpriseId = 0;
                objRooms.Add(obj);
            }
            else
            {
                UserWiseRoomsAccessDetailsDTO objRoleRooms;
                List<UserRoleModuleDetailsDTO> cps;
                var objTempPermissionList = objData.GroupBy(element => new { element.EnteriseId, element.CompanyId, element.RoomId, element.EnterpriseName, element.CompanyName, element.RoomName, element.IsEnterpriseActive, element.IsCompanyActive, element.IsRoomActive }).OrderBy(g => g.Key.EnteriseId);
                RoomLists = string.Join(",", objTempPermissionList.Select(t => (t.Key.EnteriseId + "_" + t.Key.CompanyId + "_" + t.Key.RoomId + "_" + t.Key.RoomName)).ToArray());

                foreach (var grpData in objTempPermissionList)
                {
                    objRoleRooms = new UserWiseRoomsAccessDetailsDTO();
                    objRoleRooms.EnterpriseId = grpData.Key.EnteriseId;
                    objRoleRooms.IsEnterpriseActive = grpData.Key.IsEnterpriseActive;
                    objRoleRooms.CompanyId = grpData.Key.CompanyId;
                    objRoleRooms.RoomID = grpData.Key.RoomId;
                    objRoleRooms.RoleID = RoleID;
                    cps = new List<UserRoleModuleDetailsDTO>();
                    cps = objData.Where(t => t.RoomId == grpData.Key.RoomId && t.CompanyId == grpData.Key.CompanyId && t.EnteriseId == grpData.Key.EnteriseId).ToList();

                    if (cps != null)
                    {
                        objRoleRooms.PermissionList = cps;
                    }
                    else
                    {
                        cps = new List<UserRoleModuleDetailsDTO>();
                    }

                    if (objRoleRooms.RoomID > 0)
                    {
                        RolePermissionInfo objRolePermissionInfo = new RolePermissionInfo();
                        objRolePermissionInfo.EnterPriseId = grpData.Key.EnteriseId;
                        objRolePermissionInfo.CompanyId = grpData.Key.CompanyId;
                        objRolePermissionInfo.RoomId = grpData.Key.RoomId;
                        objRoleRooms.RoomName = grpData.Key.RoomName;
                        objRoleRooms.IsRoomActive = grpData.Key.IsRoomActive;
                        objRoleRooms.CompanyName = grpData.Key.CompanyName;
                        objRoleRooms.IsCompanyActive = grpData.Key.IsCompanyActive;
                        objRoleRooms.EnterpriseName = grpData.Key.EnterpriseName;
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
        public List<UserRoomReplanishmentDetailsDTO> GetUserRoomReplanishmentDetailsRecord(long UserId)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                if (UserId > 0)
                {
                    var params1 = new SqlParameter[] { new SqlParameter("@UserID", UserId) };
                    return context.Database.SqlQuery<UserRoomReplanishmentDetailsDTO>("exec [GetUserRoomsDetailsByUserId] @UserID", params1).ToList();
                }
            }

            return null;
        }

        public List<UserRoleModuleDetailsDTO> GetUserRoleModuleDetailsByModuleAndEnterprise(long UserId, long RoleId, long ModuleId, long EnterpriseId)
        {
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@UserID", UserId),
                                                new SqlParameter("@RoleID", RoleId),
                                                new SqlParameter("@ModuleId", ModuleId),
                                                new SqlParameter("@EnterpriseId", EnterpriseId)
                                            };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UserRoleModuleDetailsDTO>("exec GetUserRoleModuleDetailsByModuleAndEnterprise @UserID,@RoleID,@ModuleId,@EnterpriseId", params1).ToList();
            }
        }

        public List<long> GetUserOrderAllowedRooms(long UserId, long RoleId, long EnterpriseId)
        {
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@UserID", UserId),
                                                new SqlParameter("@RoleID", RoleId),
                                                new SqlParameter("@EnterpriseId", RoleId)
                                            };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<long>("exec GetUserOrderAllowedRooms @UserID,@RoleID,@EnterpriseId", params1).ToList();
            }
        }

        /// <summary>
        /// Get Particullar Record from the User Master by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public List<UserRoleModuleDetailsDTO> GetUserRoleModuleDetailsRecord(long UserID, long RoleID, long UserType)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@UserID", UserID), new SqlParameter("@RoleID", RoleID), new SqlParameter("@UserType", UserType) };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UserRoleModuleDetailsDTO>("exec GetUserRoleModuleDetailsRecord @UserID,@RoleID,@UserType", params1).ToList();
            }
        }

        public List<UserRoleModuleDetailsDTO> GetUserRoleModuleDetailsDefault(long UserID, long RoleID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@UserID", UserID), new SqlParameter("@RoleID", RoleID) };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UserRoleModuleDetailsDTO>("exec GetUserRoleModuleDetailsDefault @UserID,@RoleID", params1).ToList();
            }
        }

        public List<UserWiseRoomsAccessDetailsDTO> GetSuperAdminRoomPermissions(long EnterpriseId, long CompanyID, long RoomId, long RoleId, long UserId)
        {
            List<UserWiseRoomsAccessDetailsDTO> lstpermissions = new List<UserWiseRoomsAccessDetailsDTO>();
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
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

                objUserWiseRoomsAccessDetailsDTO.PermissionList = context.Database.SqlQuery<UserRoleModuleDetailsDTO>("exec [GetSuperAdminRoomPermissions] @EnterpriseId,@CompanyID,@RoomId,@RoleId,@UserId", params1).ToList();

                lstpermissions.Add(objUserWiseRoomsAccessDetailsDTO);
            }
            return lstpermissions;
        }

        /// <summary>
        /// Insert Record in the DataBase Role Master
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public UserMasterDTO Insert(UserMasterDTO objDTO)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                //UserMaster obj = new UserMaster();

                //obj.ID = 0;
                //obj.UserName = objDTO.UserName;
                //obj.CompanyID = objDTO.CompanyID;
                //obj.Password = objDTO.Password;
                //obj.Phone = objDTO.Phone;
                //obj.Email = objDTO.Email;
                //obj.RoleId = objDTO.RoleID;
                //obj.EnterpriseId = objDTO.EnterpriseId;
                //obj.Created = DateTime.UtcNow;
                //obj.CreatedBy = objDTO.CreatedBy;
                //obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                //obj.RoomId = objDTO.Room;
                //obj.CompanyID = objDTO.CompanyID;
                //obj.Updated = DateTime.UtcNow;
                //obj.IsDeleted = false;
                //obj.IsArchived = false;
                //obj.GUID = Guid.NewGuid();
                //obj.UserType = objDTO.UserType;
                //obj.EnforceRolePermission = objDTO.EnforceRolePermission;
                //obj.IseTurnsAdmin = objDTO.IseTurnsAdmin;
                //obj.Prefix = objDTO.Prefix;
                //obj.FirstName = objDTO.FirstName;
                //obj.MiddleName = objDTO.MiddleName;
                //obj.LastName = objDTO.LastName;
                //obj.Gender = objDTO.Gender;
                //obj.Phone2 = objDTO.Phone2;
                //obj.EmployeeNumber = objDTO.EmployeeNumber;
                //obj.CostCenter = objDTO.CostCenter;
                //obj.JobTitle = objDTO.JobTitle;
                //obj.Address = objDTO.Address;
                //obj.Country = objDTO.Country;
                //obj.State = objDTO.State;
                //obj.City = objDTO.City;
                //obj.PostalCode = objDTO.PostalCode;
                //obj.UDF1 = objDTO.UDF1;
                //obj.UDF2 = objDTO.UDF2;
                //obj.UDF3 = objDTO.UDF3;
                //obj.UDF4 = objDTO.UDF4;
                //obj.UDF5 = objDTO.UDF5;
                //obj.UDF6 = objDTO.UDF6;
                //obj.UDF7 = objDTO.UDF7;
                //obj.UDF8 = objDTO.UDF8;
                //obj.UDF9 = objDTO.UDF9;
                //obj.UDF10 = objDTO.UDF10;
                //context.AddToUserMasters(obj);
                //context.SaveChanges();
                //objDTO.ID = obj.ID;
                bool enforceRolePermissionExisting = false;
                var retId = InsertUpdate(objDTO, out enforceRolePermissionExisting);
                objDTO.ID = retId;


                //if (!obj.EnforceRolePermission)
                if (objDTO.EnforceRolePermission)
                {
                    if (objDTO.UserType == 1)
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
                                    objDetails.RoomId = InternalItem.RoomId;
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

                        DataTable UserRoleDetailTable = ToDataTable(userRoleDetails); ;
                        DataTable UserRoomAccessTable = ToDataTable(userRoomAccessDetails);
                        InsertUserRoleAndRoomAccessDetails(UserRoleDetailTable, UserRoomAccessTable, objDTO.ID);
                    }
                }
                else
                {
                    context.Database.ExecuteSqlCommand("EXEC CopyRolePermissionToUser " + objDTO.ID + "");

                    if (objDTO.UserType == 1)
                    {
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
            }
            UpdateBillingRoomModulesByUser(objDTO.ID);

            return objDTO;

        }

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public UserMasterDTO Edit(UserMasterDTO objDTO)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                //UserMaster objUserMaster = context.UserMasters.FirstOrDefault(t => t.ID == objDTO.ID);
                //if (objUserMaster != null)
                {
                    bool enforceRolePermissionExisting = false;
                    var retId = InsertUpdate(objDTO, out enforceRolePermissionExisting);

                    bool CopyRolePermissionOnEdit = false;
                    //objUserMaster.ID = objDTO.ID;
                    //objUserMaster.UserName = objDTO.UserName;
                    ////objUserMaster.CompanyID = objDTO.CompanyID;
                    //if (!string.IsNullOrWhiteSpace(objDTO.Password))
                    //{
                    //    objUserMaster.Password = objDTO.Password;
                    //}
                    //objUserMaster.Phone = objDTO.Phone;
                    //objUserMaster.Email = objDTO.Email;
                    //objUserMaster.RoleId = objDTO.RoleID;
                    //objUserMaster.CreatedBy = objDTO.CreatedBy;
                    //objUserMaster.LastUpdatedBy = objDTO.LastUpdatedBy;
                    ////objUserMaster.RoomId = objDTO.Room;
                    ////objUserMaster.CompanyID = objDTO.CompanyID;
                    //objUserMaster.Updated = DateTime.UtcNow;
                    ////objUserMaster.Created = objDTO.Created;
                    ////objUserMaster.CreatedBy = objDTO.CreatedBy;
                    //objUserMaster.GUID = objDTO.GUID;
                    //objUserMaster.IsDeleted = objDTO.IsDeleted ?? false;
                    //objUserMaster.IsArchived = objDTO.IsArchived ?? false;


                    //objUserMaster.EnforceRolePermission = objDTO.EnforceRolePermission;
                    //objUserMaster.IseTurnsAdmin = objDTO.IseTurnsAdmin;
                    //objUserMaster.Prefix = objDTO.Prefix;
                    //objUserMaster.FirstName = objDTO.FirstName;
                    //objUserMaster.MiddleName = objDTO.MiddleName;
                    //objUserMaster.LastName = objDTO.LastName;
                    //objUserMaster.Gender = objDTO.Gender;
                    //objUserMaster.Phone2 = objDTO.Phone2;
                    //objUserMaster.EmployeeNumber = objDTO.EmployeeNumber;
                    //objUserMaster.CostCenter = objDTO.CostCenter;
                    //objUserMaster.JobTitle = objDTO.JobTitle;
                    //objUserMaster.Address = objDTO.Address;
                    //objUserMaster.Country = objDTO.Country;
                    //objUserMaster.State = objDTO.State;
                    //objUserMaster.City = objDTO.City;
                    //objUserMaster.PostalCode = objDTO.PostalCode;
                    //objUserMaster.UDF1 = objDTO.UDF1;
                    //objUserMaster.UDF2 = objDTO.UDF2;
                    //objUserMaster.UDF3 = objDTO.UDF3;
                    //objUserMaster.UDF4 = objDTO.UDF4;
                    //objUserMaster.UDF5 = objDTO.UDF5;
                    //objUserMaster.UDF6 = objDTO.UDF6;
                    //objUserMaster.UDF7 = objDTO.UDF7;
                    //objUserMaster.UDF8 = objDTO.UDF8;
                    //objUserMaster.UDF9 = objDTO.UDF9;
                    //objUserMaster.UDF10 = objDTO.UDF10;
                    //context.SaveChanges();

                    //if (!objUserMaster.EnforceRolePermission && objDTO.EnforceRolePermission)
                    //if (!enforceRolePermissionExisting && objDTO.EnforceRolePermission)
                    if (objDTO.EnforceRolePermission)
                    {
                        CopyRolePermissionOnEdit = true;
                    }


                    if (objDTO.UserType == 1)
                    {
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
                                        objDetails.UserID = objDTO.ID;
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

                            DataTable UserRoleDetailTable = ToDataTable(userRoleDetails); ;
                            DataTable UserRoomAccessTable = ToDataTable(userRoomAccessDetails);
                            UpdateUserRoleAndRoomAccessDetails(UserRoleDetailTable, UserRoomAccessTable, objDTO.ID);
                        }
                        else
                        {
                            if (CopyRolePermissionOnEdit)
                            {
                                context.Database.ExecuteSqlCommand("EXEC CopyRolePermissionToUser " + objDTO.ID + "");
                            }

                            //
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
                }
                
            }

            UpdateBillingRoomModulesByUser(objDTO.ID);
            return objDTO;
        }

        public UserMasterDTO EditProfile(UserMasterDTO objDTO, bool FromEPDetail)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                UserMaster objUserMaster = context.UserMasters.FirstOrDefault(t => t.ID == objDTO.ID);

                if (objUserMaster != null)
                {
                    if (!FromEPDetail)
                    {
                        objUserMaster.ID = objDTO.ID;
                        objUserMaster.UserName = objDTO.UserName;
                        objUserMaster.Prefix = objDTO.Prefix;
                        objUserMaster.FirstName = objDTO.FirstName;
                        objUserMaster.MiddleName = objDTO.MiddleName;
                        objUserMaster.LastName = objDTO.LastName;
                        objUserMaster.Gender = objDTO.Gender;
                        objUserMaster.Phone2 = objDTO.Phone2;
                        objUserMaster.EmployeeNumber = objDTO.EmployeeNumber;
                        objUserMaster.CostCenter = objDTO.CostCenter;
                        objUserMaster.JobTitle = objDTO.JobTitle;
                        objUserMaster.Address = objDTO.Address;
                        objUserMaster.Country = objDTO.Country;
                        objUserMaster.State = objDTO.State;
                        objUserMaster.City = objDTO.City;
                        objUserMaster.PostalCode = objDTO.PostalCode;
                        objUserMaster.LastUpdatedBy=objDTO.LastUpdatedBy;
                        if (!string.IsNullOrWhiteSpace(objDTO.Password))
                        {
                            objUserMaster.Password = objDTO.Password;
                        }

                        objUserMaster.Phone = objDTO.Phone;
                        objUserMaster.Email = objDTO.Email;
                        objUserMaster.Updated = DateTime.UtcNow;
                        objUserMaster.IseTurnsAdmin = objDTO.IseTurnsAdmin;
                        UserSetting objUserSetting = context.UserSettings.FirstOrDefault(t => t.UserId == objDTO.ID);

                        if (objUserSetting != null)
                        {
                            objUserSetting.RedirectURL = objDTO.RedirectURL;
                            context.SaveChanges();
                        }
                        else
                        {
                            objUserSetting = new UserSetting();
                            objUserSetting.ShowDateTime = false;
                            objUserSetting.IsNeverExpirePwd = false;
                            objUserSetting.IsRequistionReportDisplay = false;
                            objUserSetting.UserId = objDTO.ID;
                            objUserSetting.RedirectURL = objDTO.RedirectURL;
                            objUserSetting.IsRemember = true;
                            context.UserSettings.Add(objUserSetting);
                            context.SaveChanges();
                        }
                    }
                    else
                    {
                        objUserMaster.ID = objDTO.ID;
                        objUserMaster.UserName = objDTO.UserName;
                        objUserMaster.Prefix = objDTO.Prefix;
                        objUserMaster.FirstName = objDTO.FirstName;
                        objUserMaster.MiddleName = objDTO.MiddleName;
                        objUserMaster.LastName = objDTO.LastName;
                        objUserMaster.Gender = objDTO.Gender;
                        objUserMaster.Phone2 = objDTO.Phone2;
                        objUserMaster.EmployeeNumber = objDTO.EmployeeNumber;
                        objUserMaster.CostCenter = objDTO.CostCenter;
                        objUserMaster.JobTitle = objDTO.JobTitle;
                        objUserMaster.Address = objDTO.Address;
                        objUserMaster.Country = objDTO.Country;
                        objUserMaster.State = objDTO.State;
                        objUserMaster.City = objDTO.City;
                        objUserMaster.PostalCode = objDTO.PostalCode;

                        if (!string.IsNullOrWhiteSpace(objDTO.Password))
                        {
                            objUserMaster.Password = objDTO.Password;
                        }

                        objUserMaster.Email = objDTO.Email;
                        objUserMaster.Updated = DateTime.UtcNow;
                        objUserMaster.IseTurnsAdmin = objDTO.IseTurnsAdmin;
                        objUserMaster.IsNgNLFAllowed = objDTO.IsNgNLFAllowed;
                        UserSetting objUserSetting = context.UserSettings.FirstOrDefault(t => t.UserId == objDTO.ID);

                        if (objUserSetting != null)
                        {
                            objUserSetting.RedirectURL = objDTO.RedirectURL;
                            context.SaveChanges();
                        }
                        else
                        {
                            objUserSetting = new UserSetting();
                            objUserSetting.UserId = objDTO.ID;
                            objUserSetting.RedirectURL = objDTO.RedirectURL;
                            objUserSetting.IsRemember = true;
                            context.UserSettings.Add(objUserSetting);
                            context.SaveChanges();
                        }
                    }
                }
                return objDTO;
            }
        }

        public bool IsSAdminUserExist(long UserId, long EnterpriseID)
        {
            EnterpriseMasterDAL objEnterprise = new EnterpriseMasterDAL();
            EnterpriseDTO obj = new EnterpriseDTO();
            obj = objEnterprise.GetEnterpriseByIdPlain(EnterpriseID);

            if (obj != null)
            {
                var params1 = new SqlParameter[] {
                                                new SqlParameter("@UserID", UserId),
                                                new SqlParameter("@EnterpriseDBName", obj.EnterpriseDBName ?? (object) DBNull.Value)
                                            };
                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    var result = context.Database.SqlQuery<int?>("exec CheckSuperAdminUserExistence @UserID,@EnterpriseDBName ", params1).FirstOrDefault();
                    return (result != null && result > 0);
                }
            }

            return false;
        }

        public void InsertSAdminUserInChildDB(long NewUserID, UserMasterDTO objUserMasterDTO, long EnterpriseID)
        {
            EnterpriseMasterDAL objEnterprise = new EnterpriseMasterDAL();
            EnterpriseDTO obj = new EnterpriseDTO();
            obj = objEnterprise.GetEnterpriseByIdPlain(EnterpriseID);

            if (obj != null && objUserMasterDTO != null)
            {
                var params1 = new SqlParameter[] {
                                                new SqlParameter("@UserID", NewUserID),
                                                new SqlParameter("@UserName", objUserMasterDTO.UserName),
                                                new SqlParameter("@Password", objUserMasterDTO.Password),
                                                new SqlParameter("@Phone", objUserMasterDTO.Phone),
                                                new SqlParameter("@Email", objUserMasterDTO.Email),
                                                new SqlParameter("@RoleId", objUserMasterDTO.RoleID),
                                                new SqlParameter("@LastUpdatedBy", objUserMasterDTO.LastUpdatedBy),
                                                new SqlParameter("@GUID", objUserMasterDTO.GUID),
                                                new SqlParameter("@CompanyID", objUserMasterDTO.CompanyID),
                                                new SqlParameter("@UserType", objUserMasterDTO.UserType),
                                                new SqlParameter("@EnterpriseDBName", obj.EnterpriseDBName ?? (object) DBNull.Value)
                                            };

                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    context.Database.ExecuteSqlCommand("EXEC InsertSuperAdminUserInChildDB @UserID,@UserName,@Password,@Phone,@Email,@RoleId,@LastUpdatedBy,@GUID,@CompanyID,@UserType,@EnterpriseDBName ", params1);
                }
            }
        }

        public void SaveUserActions(UserLoginHistoryDTO objUserLoginHistoryDTO)
        {
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@UserId", objUserLoginHistoryDTO.UserId),
                                                new SqlParameter("@EventDate", objUserLoginHistoryDTO.EventDate),
                                                new SqlParameter("@EventType", objUserLoginHistoryDTO.EventType),
                                                new SqlParameter("@IpAddress", objUserLoginHistoryDTO.IpAddress ?? (object) DBNull.Value),
                                                new SqlParameter("@RoomId", objUserLoginHistoryDTO.RoomId),
                                                new SqlParameter("@CompanyId", objUserLoginHistoryDTO.CompanyId),
                                                new SqlParameter("@EnterpriseId", objUserLoginHistoryDTO.EnterpriseId),
                                                new SqlParameter("@CountryName", objUserLoginHistoryDTO.CountryName ?? (object) DBNull.Value),
                                                new SqlParameter("@RegionName", objUserLoginHistoryDTO.RegionName ?? (object) DBNull.Value),
                                                new SqlParameter("@CityName", objUserLoginHistoryDTO.CityName ?? (object) DBNull.Value),
                                                new SqlParameter("@ZipCode", objUserLoginHistoryDTO.ZipCode ?? (object) DBNull.Value)
                                            };

            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("EXEC LogUserLoginDetails @UserId,@EventDate,@EventType,@IpAddress,@RoomId,@CompanyId,@EnterpriseId,@CountryName,@RegionName,@CityName,@ZipCode ", params1);
            }
        }

        public UserLoginHistoryDTO GetUserLastActionDetail(long UserId)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@UserId", UserId) };

            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UserLoginHistoryDTO>("EXEC GetUserLastActionDetail @UserId ", params1).FirstOrDefault();
            }
        }

        public void UpdateTermsCondition(long id, bool IsLicenceAccepted)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@UserId", id),
                                                    new SqlParameter("@IsLicenceAccepted", IsLicenceAccepted)
                                                };
                context.Database.ExecuteSqlCommand("exec [UpdateUserLicenceAcceptance] @UserId,@IsLicenceAccepted", params1);
            }
        }

        public bool SaveChangePassword(UserPasswordChangeHistoryDTO objDTO)
        {
            UserPasswordChangeHistory obj = new UserPasswordChangeHistory();
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                obj.Id = 0;
                obj.OldPassword = objDTO.oldPassword;
                obj.LastUpdated = objDTO.LastUpdated;
                obj.UserId = objDTO.UserId;

                context.UserPasswordChangeHistories.Add(obj);

                context.SaveChanges();
                if (obj.Id > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }

        public bool ValidateOldPassword(string Password, long UserId)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var user = GetUserByIdPlain(UserId);
                return (user != null && user.Password == Password);
            }
        }

        public string GetOldPassword(long UserId)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var user = GetUserByIdPlain(UserId);

                if (user != null)
                {
                    return user.Password;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public void UpdatePassword(UserPasswordChangeHistoryDTO objDTO)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@UserId", objDTO.UserId),
                                                    new SqlParameter("@Password", objDTO.NewPassword)
                                                };
                context.Database.ExecuteSqlCommand("exec [UpdateUserPassword] @UserId,@Password", params1);
            }
        }

        public bool CheckPasswordChange(long UserId, int ExpiryDays)
        {
            bool IsForWarning = false;
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@UserId", UserId),
                                                new SqlParameter("@ExpiryDays", ExpiryDays) ,
                                                new SqlParameter("@IsForWarning", IsForWarning) ,

                                            };

            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<bool>("exec CheckUserPasswordExpiryOrWarning @UserId,@ExpiryDays,@IsForWarning ", params1).FirstOrDefault();
            }
        }

        public bool CheckPasswordWarning(long UserId, Int32 ExpiryDays, int? PasswordExpiryWarningDays)
        {
            bool IsForWarning = true;
            int WarningStartPriorDays = PasswordExpiryWarningDays ?? 0;

            if (ExpiryDays > 0)
            {
                if (ExpiryDays > WarningStartPriorDays)
                {
                    ExpiryDays = ExpiryDays - WarningStartPriorDays;
                }
            }

            var params1 = new SqlParameter[] {
                                                new SqlParameter("@UserId", UserId),
                                                new SqlParameter("@ExpiryDays", ExpiryDays) ,
                                                new SqlParameter("@IsForWarning", IsForWarning) ,
                                            };

            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<bool>("exec CheckUserPasswordExpiryOrWarning @UserId,@ExpiryDays,@IsForWarning ", params1).FirstOrDefault();
            }
        }

        public List<UserAccessDTO> GetUserRoomAccessByUserIdPlain(long UserId)
        {
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@UserId", UserId),
                                            };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UserAccessDTO>("exec GetUserRoomAccessByUserIdPlain @UserId ", params1).ToList();
            }
        }

        public List<UserAccessDTO> GetUserRoomAccessForSuperUserByUserIdPlain(long UserId)
        {
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@UserId", UserId),
                                            };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UserAccessDTO>("exec GetUserRoomAccessForSuperUserByUserIdPlain @UserId ", params1).ToList();
            }
        }

        public UserAccessDTO GetUserRoomAccessesByUserId(long EnterpriseID, long CompanyID, long RoomID, long UserID)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@EnterpriseID", EnterpriseID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@UserID", UserID) };
                return context.Database.SqlQuery<UserAccessDTO>("exec [GetUserRoomAccessesByEntCmpRoom] @EnterpriseID,@CompanyID,@RoomID,@UserID", params1).FirstOrDefault();
            }
        }

        public List<UserAccessDTO> GetRoleRoomAccessByRoleIdPlain(long RoleId)
        {
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@RoleId", RoleId),
                                            };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UserAccessDTO>("exec GetRoleRoomAccessByRoleIdPlain @RoleId ", params1).ToList();
            }
        }

        public UserMasterDTO GetUserByIdForService(long Id)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Id", Id) };
                return context.Database.SqlQuery<UserMasterDTO>("exec [GetUserByIdForService] @Id", params1).FirstOrDefault();
            }
        }

        public UserMasterDTO GetUserByIdNormal(long Id)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Id", Id) };
                return context.Database.SqlQuery<UserMasterDTO>("exec [GetUserByIdNormal] @Id ", params1).FirstOrDefault();
            }
        }

        public void UpdateUserPasswordChangeHistory(bool IsChecked, long UserId)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@UserId", UserId),
                                                    new SqlParameter("@IsChecked", IsChecked)
                                                };
                context.Database.ExecuteSqlCommand("exec [UpdateUserPasswordChangeHistory] @UserId,@IsChecked ", params1);
            }
        }

        public void ConvertentEnterpriseAdmin(long UserId)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@UserId", UserId)
                                                };
                context.Database.ExecuteSqlCommand("exec [SetUserAsEnterpriseAdmin] @UserId ", params1);
            }
        }

        public long AssignNormalRole(long UserId)
        {
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@UserId", UserId)
                                            };

            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<long>("exec AssignNormalRoleToUser @UserId ", params1).FirstOrDefault();
            }
        }

        public void AddNewCompRoomToRoleUserRoomAccess(List<UserRoomAccessDTO> UserRoomAccessTable)
        {
            if (UserRoomAccessTable != null && UserRoomAccessTable.Count > 0)
            {
                DataTable userRoomAccessTable = ToDataTable(UserRoomAccessTable);

                var paramUserRoomAccess = new SqlParameter("@UserRoomAccessTable", userRoomAccessTable);
                paramUserRoomAccess.SqlDbType = SqlDbType.Structured;
                paramUserRoomAccess.TypeName = "dbo.UserRoomAccessTable";

                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    var params1 = new SqlParameter[] { paramUserRoomAccess };
                    context.Database.ExecuteSqlCommand("exec AddCompanyToUserRoleAndRoomAccess @UserRoomAccessTable", params1);
                }
            }
        }

        public UserMasterDTO GetUserByNamePlain(string UserName)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@UserName", UserName) };
                return context.Database.SqlQuery<UserMasterDTO>("exec [GetUserByNamePlain] @UserName", params1).FirstOrDefault();
            }
        }

        public UserMasterLoginDTO GetUserByNameAndEnteprise(string UserName)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@UserName", UserName) };
                return context.Database.SqlQuery<UserMasterLoginDTO>("exec [GetUserAndEnterpriseByName] @UserName", params1).FirstOrDefault();
            }
        }

        public List<UserMasterDTO> GetPagedUsers(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string sortColumnName, bool IsArchived, bool IsDeleted, long LoggedInUser, string UserCreaters, string UserUpdators, string CreatedDateFrom, string CreatedDateTo, string UpdatedDateFrom, string UpdatedDateTo, string UserEnterprises, string UserCompanies, string UserRooms, string UserTypes, string UserRoles, string UDF1, string UDF2, string UDF3, string UDF4, string UDF5, string UDF6, string UDF7, string UDF8, string UDF9, string UDF10)
        {
            List<UserMasterDTO> lstUsers = new List<UserMasterDTO>();
            TotalCount = 0;
            var params1 = new SqlParameter[]
                        {
                                new SqlParameter("@StartRowIndex", StartRowIndex)
                              , new SqlParameter("@MaxRows", MaxRows)
                              , new SqlParameter("@SearchTerm", SearchTerm??string.Empty)
                              , new SqlParameter("@sortColumnName", sortColumnName)
                              , new SqlParameter("@UserCreaters", UserCreaters)
                              , new SqlParameter("@UserUpdators", UserUpdators)
                              , new SqlParameter("@CreatedDateFrom", CreatedDateFrom)
                              , new SqlParameter("@CreatedDateTo", CreatedDateTo)
                              , new SqlParameter("@UpdatedDateFrom", UpdatedDateFrom)
                              , new SqlParameter("@UpdatedDateTo", UpdatedDateTo)
                              , new SqlParameter("@IsDeleted", IsDeleted)
                              , new SqlParameter("@IsArchived", IsArchived)
                              , new SqlParameter("@UserEnterprises", UserEnterprises)
                              , new SqlParameter("@UserCompanies", UserCompanies)
                              , new SqlParameter("@UserRooms", UserRooms)
                              , new SqlParameter("@UserTypes", UserTypes)
                              , new SqlParameter("@UserRoles", UserRoles)
                              , new SqlParameter("@LoggedInUserID", LoggedInUser)
                              , new SqlParameter("@UDF1", UDF1)
                              , new SqlParameter("@UDF2", UDF2)
                              , new SqlParameter("@UDF3", UDF3)
                              , new SqlParameter("@UDF4", UDF4)
                              , new SqlParameter("@UDF5", UDF5)
                              , new SqlParameter("@UDF6", UDF6)
                              , new SqlParameter("@UDF7", UDF7)
                              , new SqlParameter("@UDF8", UDF8)
                              , new SqlParameter("@UDF9", UDF9)
                              , new SqlParameter("@UDF10", UDF10)
                        };

            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                lstUsers = context.Database.SqlQuery<UserMasterDTO>("exec GetPagedUsers @StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@UserCreaters,@UserUpdators,@CreatedDateFrom,@CreatedDateTo,@UpdatedDateFrom,@UpdatedDateTo,@IsDeleted,@IsArchived,@UserEnterprises,@UserCompanies,@UserRooms,@UserTypes,@UserRoles,@LoggedInUserID,@UDF1,@UDF2,@UDF3,@UDF4,@UDF5,@UDF6,@UDF7,@UDF8,@UDF9,@UDF10", params1).ToList();
                if (lstUsers != null && lstUsers.Count > 0)
                {
                    TotalCount = lstUsers.First().TotalRecords;
                }
            }

            return lstUsers;
        }

        public List<UserNS> GetPagedUsersNS(long LoggedInUser,bool IsDeleted)
        {
            List<UserNS> lstUsers = new List<UserNS>();

            var params1 = new SqlParameter[] { new SqlParameter("@LoggedInUserID", LoggedInUser), new SqlParameter("@IsDeleted", IsDeleted) };

            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                lstUsers = context.Database.SqlQuery<UserNS>("exec GetPagedUsersNarrowSearch @LoggedInUserID,@IsDeleted", params1).ToList();

            }

            return lstUsers;
        }

        public bool GetRequistionReportDisplayPermission(long UserId)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@UserId", UserId) };
                var IsRequistionReportDisplay = context.Database.SqlQuery<bool?>("EXEC GetRequistionReportDisplayPermissionByUserId @UserId ", params1).FirstOrDefault();
                return IsRequistionReportDisplay.HasValue ? IsRequistionReportDisplay.Value : false;
            }
        }

        public List<UserRoleModuleDetailsDTO> GetUserRoleModuleDetails(long UserId, long RoleId, long RoomId, long CompanyId, string ModuleIds)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
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

        public List<EnterpriseSuperAdmin> GetAllEpSuperAdmins(long EnterpriseID)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@EnterpriseID", EnterpriseID) };
                return context.Database.SqlQuery<EnterpriseSuperAdmin>("EXEC GetEnterpriseSuperAdmins @EnterpriseID ", params1).ToList();
            }
        }

        public List<EnterpriseSuperAdmin> GetAllEnterpriseUsers(long EnterpriseID)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@EnterpriseID", EnterpriseID) };
                return context.Database.SqlQuery<EnterpriseSuperAdmin>("EXEC GetAllEnterpriseUsers @EnterpriseID ", params1).ToList();
            }
        }

        public List<EnterpriseSuperAdmin> GetAllEnterpriseUserRoles(long EnterpriseID)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@EnterpriseID", EnterpriseID) };
                return context.Database.SqlQuery<EnterpriseSuperAdmin>("EXEC GetAllEnterpriseUserRoles @EnterpriseID ", params1).ToList();
            }
        }

        public List<UserMasterDTO> GetEnterpriseUsersByName(long? EnterpriseID, string EnterpriseName, string UserNames)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@EnterpriseID", EnterpriseID ?? (object)DBNull.Value), new SqlParameter("@EnterpriseName", EnterpriseName ?? (object)DBNull.Value), new SqlParameter("@UserNames", UserNames ?? (object)DBNull.Value) };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UserMasterDTO>("EXEC GetEnterpriseUsersByName @UserNames,@EnterpriseName,@EnterpriseID", params1).ToList();
            }
        }

        public UserRoleModuleDetailsDTO GetUserModulePermission(long UserID, long RoleID, long EnterpriseID, long CompanyID, long RoomID, int ModuleID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@UserID", UserID),
                                               new SqlParameter("@RoleID", RoleID),
                                               new SqlParameter("@EnterpriseID", EnterpriseID),
                                               new SqlParameter("@CompanyID", CompanyID),
                                               new SqlParameter("@RoomID", RoomID),
                                               new SqlParameter("@ModuleID", ModuleID)
                                            };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UserRoleModuleDetailsDTO>("exec [GetUserRoleModuleDetailsRecordSingle] @UserID,@RoleID,@EnterpriseID,@CompanyID,@RoomID,@ModuleID", params1).FirstOrDefault();
            }
        }

        public List<RoomModuleDetailsDTO> GetUserRoleModuleDetailsForAPI(long UserID, long RoleID, long RoomID)
        {
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@RoleID", RoleID),
                                                new SqlParameter("@UserID", UserID),
                                                new SqlParameter("@RoomID", RoomID)
                                            };

            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RoomModuleDetailsDTO>("exec GetUserRoleModuleDetailsForAPI @RoleID,@UserID,@RoomID", params1).ToList();
            }

        }

        /// <summary>
        /// This method is used to get the room access for the super admin
        /// </summary>
        /// <returns></returns>
        public List<UserAccessDTO> GetUserRoomAccessForSuperAdmin()
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UserAccessDTO>("EXEC GetUserRoomAccessForSuperAdmin ").ToList();
            }
        }

        /// <summary>
        /// This method is used to get the room access for the enterprise admin
        /// </summary>
        /// <param name="EnterpriseId"></param>
        /// <returns></returns>
        public List<UserAccessDTO> GetUserRoomAccessForEnterpriseAdmin(long EnterpriseId)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var param = new SqlParameter[] {
                                                    new SqlParameter("@EnterpriseId", EnterpriseId)
                                               };
                return context.Database.SqlQuery<UserAccessDTO>("EXEC GetUserRoomAccessForEnterpriseAdmin @EnterpriseId", param).ToList();
            }
        }

        private void InsertUserRoleAndRoomAccessDetails(DataTable UserRoleDetailTable, DataTable UserRoomAccessTable, long UserId)
        {
            var paramUserRoleDetail = new SqlParameter("@UserRoleDetailTable", UserRoleDetailTable);
            paramUserRoleDetail.SqlDbType = SqlDbType.Structured;
            paramUserRoleDetail.TypeName = "dbo.UserRoleDetailTable";

            var paramUserRoomAccess = new SqlParameter("@UserRoomAccessTable", UserRoomAccessTable);
            paramUserRoomAccess.SqlDbType = SqlDbType.Structured;
            paramUserRoomAccess.TypeName = "dbo.UserRoomAccessTable";

            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@UserID", UserId),
                                                    paramUserRoleDetail,
                                                    paramUserRoomAccess
                                                };
                context.Database.ExecuteSqlCommand("exec InsertUserRoleAndRoomAccessDetails @UserID,@UserRoleDetailTable,@UserRoomAccessTable", params1);
            }
        }

        private void UpdateUserRoleAndRoomAccessDetails(DataTable UserRoleDetailTable, DataTable UserRoomAccessTable, long UserId)
        {
            var paramUserRoleDetail = new SqlParameter("@UserRoleDetailTable", UserRoleDetailTable);
            paramUserRoleDetail.SqlDbType = SqlDbType.Structured;
            paramUserRoleDetail.TypeName = "dbo.UserRoleDetailTable";

            var paramUserRoomAccess = new SqlParameter("@UserRoomAccessTable", UserRoomAccessTable);
            paramUserRoomAccess.SqlDbType = SqlDbType.Structured;
            paramUserRoomAccess.TypeName = "dbo.UserRoomAccessTable";

            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@UserID", UserId),
                                                    paramUserRoleDetail,
                                                    paramUserRoomAccess
                                                };
                context.Database.CommandTimeout = 3600;
                    context.Database.ExecuteSqlCommand("exec UpdateUserRoleAndRoomAccessDetails @UserID,@UserRoleDetailTable,@UserRoomAccessTable", params1);
            }
        }

        private void UpdateUserRoomAccessDetails(DataTable UserRoomAccessTable, long UserId)
        {
            var paramUserRoomAccess = new SqlParameter("@UserRoomAccessTable", UserRoomAccessTable);
            paramUserRoomAccess.SqlDbType = SqlDbType.Structured;
            paramUserRoomAccess.TypeName = "dbo.UserRoomAccessTable";

            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@UserID", UserId),
                                                    paramUserRoomAccess
                                                };
                context.Database.CommandTimeout = 3600;
                context.Database.ExecuteSqlCommand("exec UpdateUserRoomAccessDetails @UserID,@UserRoomAccessTable", params1);
            }
        }

        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
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

        private long InsertUpdate(UserMasterDTO objDTO, out bool enforceRolePermissionExisting)
        {
            enforceRolePermissionExisting = true;
            List<SqlParameter> objPara = new List<SqlParameter>() {
            new SqlParameter("@ID", objDTO.ID),
            new SqlParameter("@UserName", objDTO.UserName),
            new SqlParameter("@Phone", objDTO.Phone),
            new SqlParameter("@Email", objDTO.Email),
            new SqlParameter("@RoleId", objDTO.RoleID),
            new SqlParameter("@EnterpriseId", objDTO.EnterpriseId),
            new SqlParameter("@CreatedBy", objDTO.CreatedBy),
            new SqlParameter("@LastUpdatedBy", objDTO.LastUpdatedBy),
            new SqlParameter("@RoomId", objDTO.Room),
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



            if (objDTO.ID == 0)
            {
                // insert

                objPara.Add(new SqlParameter("@IsArchived", false));
                objPara.Add(new SqlParameter("@GUID", Guid.NewGuid()));
                objPara.Add(new SqlParameter("@IsDeleted", false));
                objPara.Add(new SqlParameter("@Created", DateTime.UtcNow));
                objPara.Add(new SqlParameter("@Updated", DateTime.UtcNow));
                objPara.Add(new SqlParameter("@Password", objDTO.Password));
            }
            else
            {
                objPara.Add(new SqlParameter("@IsArchived", objDTO.IsArchived ?? false));
                objPara.Add(new SqlParameter("@GUID", objDTO.GUID));
                objPara.Add(new SqlParameter("@IsDeleted", objDTO.IsDeleted ?? false));
                objPara.Add(new SqlParameter("@Created", DateTime.UtcNow));
                objPara.Add(new SqlParameter("@Updated", DateTime.UtcNow));
                // update
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
            DataSet ds = SqlHelper.ExecuteDataset(base.DataBaseConnectionString, "uspAddUpdateUser", objPara.ToArray());
            long retId = 0;
            if (ds.Tables.Count > 0)
            {
                retId = Convert.ToInt64(ds.Tables[0].Rows[0]["RetId"]);
                enforceRolePermissionExisting = Convert.ToBoolean(ds.Tables[0].Rows[0]["EnforceRolePermissionExisting"]);
            }

            return retId;
        }

        public void DeleteUserByIds(string IDs, long UserId)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
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
        //    using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
        //    {
        //        var r = context.ExecuteStoreCommand("exec [EnableDisableModuleByBillingTypeForMaster] @RoomID", params1);
        //    }
        //}

        public void UpdateBillingRoomModulesByUser(long userID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@UserID", userID) };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var r = context.Database.ExecuteSqlCommand("EXEC updateBillingRoomModulesByUserInMaster @UserID", params1);
            }
        }

        public UserMasterDTO GetUserByNameAndEnterPriseIDPlain(string UserName,long EnterPriseID)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@UserName", UserName),
                new SqlParameter("@EnterPriseID", EnterPriseID)};
                return context.Database.SqlQuery<UserMasterDTO>("exec [GetUserByNameAndEnterPriseIDPlain] @UserName,@EnterPriseID", params1).FirstOrDefault();
            }
        }

        public void UpdateIsNgNLFAllowedByUserID(long UserID,bool IsNgNLFAllowed)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@UserID", UserID),
                                                    new SqlParameter("@IsNgNLFAllowed", IsNgNLFAllowed)
                                                };
                context.Database.ExecuteSqlCommand("exec [UpdateIsNgNLFAllowedByUserID] @UserID,@IsNgNLFAllowed", params1);
            }
        }

        public void UpdateIsNgNLFAllowedByUserIDs(string UserIDs,long updatedby)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@UserIds", UserIDs),
                                                    new SqlParameter("@UpdatedBy",updatedby)
                                                };
                context.Database.ExecuteSqlCommand("exec [EnableUsersForNLF] @UserIds,@UpdatedBy", params1);
            }
        }

        public DefaultRedirectURLs FetchRedirectURL(string redirecturl)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    var params1 = new SqlParameter[] { new SqlParameter("@redirecturl", redirecturl) };
                    return context.Database.SqlQuery<DefaultRedirectURLs>("exec [FetchRedirectURL] @redirecturl", params1).FirstOrDefault();
                }catch(Exception ex)
                {
                    return new DefaultRedirectURLs();
                }
            }
        }


        public void UpdateUserSettingForBorderState(long id)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@UserId", id)
                                                };
                context.Database.ExecuteSqlCommand("exec [UpdateUserSettingForBorderState] @UserId", params1);
            }
        }


    }//CLASS
}
