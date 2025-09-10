using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using eTurnsMaster.DAL;
using System.Data.Objects;
using System.Dynamic;
using System.Data.SqlClient;
using System.Transactions;
using System.Threading.Tasks;

namespace eTurnsMaster.DAL
{
    public partial class RoleMasterDAL : eTurnsMasterBaseDAL,IDisposable
    {
        private bool disposedValue;

        public List<RoleMasterDTO> GetAllRoles(int? UserType)
        {
            List<RoleMasterDTO> lstRoles = new List<RoleMasterDTO>();
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                lstRoles = (from u in context.RoleMasters
                            where u.IsActive == true && u.IsDeleted == false && u.IsArchived == false && (u.UserType == null || u.UserType == UserType)

                            select new RoleMasterDTO
                            {
                                ID = u.ID,
                                RoleName = u.RoleName,
                                Description = u.Description,
                                IsActive = u.IsActive ?? false,
                                Created = u.Created,
                                Updated = u.Updated
                            }).ToList();
            }
            return lstRoles;
        }

        public RoleMasterDTO GetRecord(Int64 id)
        {
            RoleMasterDTO objresult = new RoleMasterDTO();

            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                objresult = (from rm in context.RoleMasters
                             join umcrtr in context.UserMasters on rm.CreatedBy equals umcrtr.ID into rm_umcrtr_join
                             from rm_umcrtr in rm_umcrtr_join.DefaultIfEmpty()
                             join umuptr in context.UserMasters on rm.LastUpdatedBy equals umuptr.ID into rm_umuptr_join
                             from rm_umuptr in rm_umuptr_join.DefaultIfEmpty()
                             join utm in context.UserTypeMasters on rm.UserType equals utm.UserTypeID
                             where rm.ID == id
                             select new RoleMasterDTO
                             {
                                 CompanyID = 0,
                                 CompanyName = string.Empty,
                                 Created = rm.Created,
                                 CreatedBy = rm.CreatedBy,
                                 CreatedByName = rm_umcrtr.UserName,
                                 Description = rm.Description,
                                 EnterpriseId = 0,
                                 EnterpriseName = string.Empty,
                                 GUID = rm.GUID ?? Guid.Empty,
                                 ID = rm.ID,
                                 IsActive = rm.IsActive ?? false,
                                 IsArchived = rm.IsArchived ?? false,
                                 IsDeleted = rm.IsDeleted ?? false,
                                 LastUpdatedBy = rm.LastUpdatedBy,
                                 RoleName = rm.RoleName,
                                 Updated = rm.Updated,
                                 UpdatedByName = rm_umuptr.UserName,
                                 UserType = rm.UserType ?? 0,
                                 UserTypeName = utm.UserTypeName

                             }).FirstOrDefault();

                objresult.PermissionList = GetRoleModuleDetailsRecord(id, 0, 0, 0);
                string RoomLists = "";
                if (objresult.PermissionList != null && objresult.PermissionList.Count > 0)
                {

                    objresult.RoleWiseRoomsAccessDetails = ConvertPermissions(objresult.PermissionList, id, ref RoomLists);


                    objresult.SelectedRoomAccessValue = RoomLists;
                }

            }
            return objresult;
        }

        public RoleMasterDTO GetRecord(Int64 id, long EnterpriseID, long CompanyID, long RoomID)
        {
            RoleMasterDTO objresult = new RoleMasterDTO();
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                objresult = (from rm in context.RoleMasters
                             join umcrtr in context.UserMasters on rm.CreatedBy equals umcrtr.ID into rm_umcrtr_join
                             from rm_umcrtr in rm_umcrtr_join.DefaultIfEmpty()
                             join umuptr in context.UserMasters on rm.LastUpdatedBy equals umuptr.ID into rm_umuptr_join
                             from rm_umuptr in rm_umuptr_join.DefaultIfEmpty()
                             join utm in context.UserTypeMasters on rm.UserType equals utm.UserTypeID
                             where rm.ID == id
                             select new RoleMasterDTO
                             {
                                 CompanyID = 0,
                                 CompanyName = string.Empty,
                                 Created = rm.Created,
                                 CreatedBy = rm.CreatedBy,
                                 CreatedByName = rm_umcrtr.UserName,
                                 Description = rm.Description,
                                 EnterpriseId = 0,
                                 EnterpriseName = string.Empty,
                                 GUID = rm.GUID ?? Guid.Empty,
                                 ID = rm.ID,
                                 IsActive = rm.IsActive ?? false,
                                 IsArchived = rm.IsArchived ?? false,
                                 IsDeleted = rm.IsDeleted ?? false,
                                 LastUpdatedBy = rm.LastUpdatedBy,
                                 RoleName = rm.RoleName,
                                 Updated = rm.Updated,
                                 UpdatedByName = rm_umuptr.UserName,
                                 UserType = rm.UserType ?? 0,
                                 UserTypeName = utm.UserTypeName

                             }).FirstOrDefault();

                objresult.PermissionList = GetRoleModuleDetailsRecord(id, RoomID, CompanyID, EnterpriseID);
                string RoomLists = "";
                if (objresult.PermissionList != null && objresult.PermissionList.Count > 0)
                {

                    objresult.RoleWiseRoomsAccessDetails = ConvertPermissions(objresult.PermissionList, id, ref RoomLists);


                    objresult.SelectedRoomAccessValue = RoomLists;
                }
            }
            return objresult;
        }

        private DataTable GetECRTable(List<RolePermissionInfo> lstRooms)
        {
            DataTable ReturnDT = new DataTable("ECRTable");
            try
            {
                DataColumn[] arrColumns = new DataColumn[] {
                new DataColumn() { AllowDBNull=true,ColumnName="EnterpriseID",DataType=typeof(Int64)},
                new DataColumn() { AllowDBNull=true,ColumnName="CompanyID",DataType=typeof(Int64)},
                new DataColumn() { AllowDBNull=true,ColumnName="RoomID",DataType=typeof(Int64)} };
                ReturnDT.Columns.AddRange(arrColumns);

                if (lstRooms != null && lstRooms.Count > 0)
                {
                    foreach (var item in lstRooms)
                    {
                        DataRow row = ReturnDT.NewRow();
                        row["EnterpriseID"] = item.EnterPriseId;
                        row["CompanyID"] = item.CompanyId;
                        row["RoomID"] = item.RoomId;
                        ReturnDT.Rows.Add(row);
                    }
                }

                return ReturnDT;
            }
            catch
            {
                return ReturnDT;
            }
        }
        

        private List<RoleWiseRoomsAccessDetailsDTO> ConvertPermissions(List<RoleModuleDetailsDTO> objData, Int64 RoleID, ref string RoomLists)
        {
            List<RoleWiseRoomsAccessDetailsDTO> objRooms = new List<RoleWiseRoomsAccessDetailsDTO>();

            RoleWiseRoomsAccessDetailsDTO objRoleRooms;

            var objTempPermissionList = objData.GroupBy(element => new { element.RoomId, element.CompanyID, element.EnterpriseID, element.RoomName, element.CompanyName, element.EnterpriseName })
                                            .OrderBy(g => g.Key.RoomId);
            RoomLists = string.Join(",", objTempPermissionList.Select(t => (t.Key.EnterpriseID + "_" + t.Key.CompanyID + "_" + t.Key.RoomId + "_" + t.Key.RoomName)).ToArray());
            foreach (var grpData in objTempPermissionList)
            {
                objRoleRooms = new RoleWiseRoomsAccessDetailsDTO();
                objRoleRooms.RoleID = RoleID;
                objRoleRooms.EnterpriseID = grpData.Key.EnterpriseID;
                objRoleRooms.CompanyID = grpData.Key.CompanyID;
                objRoleRooms.RoomID = grpData.Key.RoomId;
                List<RoleModuleDetailsDTO> cps = grpData.ToList();
                if (cps != null)
                {
                    objRoleRooms.PermissionList = cps;
                    objRoleRooms.RoomName = cps[0].RoomName;
                }
                if (objRoleRooms.RoomID > 0)
                {
                    EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
                    RolePermissionInfo objRolePermissionInfo = new RolePermissionInfo();
                    objRolePermissionInfo.EnterPriseId = objRoleRooms.EnterpriseID;
                    objRolePermissionInfo.CompanyId = objRoleRooms.CompanyID;
                    objRolePermissionInfo.RoomId = objRoleRooms.RoomID;
                    objRoleRooms.RoomName = grpData.Key.RoomName;
                    objRoleRooms.CompanyName = grpData.Key.CompanyName;
                    objRoleRooms.EnterPriseName = grpData.Key.EnterpriseName;
                }
                objRooms.Add(objRoleRooms);
            }

            return objRooms;
        }

        public List<RoleRoomReplanishmentDetailsDTO> GetRoleRoomReplanishmentDetailsRecord(Int64 RoleID)
        {
            EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                if (RoleID > 0)
                {
                    List<RoleRoomReplanishmentDetailsDTO> objRe = new List<RoleRoomReplanishmentDetailsDTO>();

                    objRe = (from u in context.RoleRoomsDetails
                             where u.RoleId == RoleID
                             select new RoleRoomReplanishmentDetailsDTO
                             {
                                 ID = u.ID,
                                 RoleID = u.RoleId,
                                 RoomID = u.RoomId,
                                 CompanyId = u.CompanyId,
                                 EnterpriseId = u.EnterpriseId,
                                 IsRoomAccess = u.IsRoomAccess
                             }).ToList();
                    if (objRe != null && objRe.Count > 0)
                    {

                        objRe.ForEach(t =>
                        {
                            RolePermissionInfo objRolePermissionInfo = new RolePermissionInfo();
                            objRolePermissionInfo.EnterPriseId = t.EnterpriseId;
                            objRolePermissionInfo.CompanyId = t.CompanyId;
                            objRolePermissionInfo.RoomId = t.RoomID;
                            RoomDTO objRoomDTO = new RoomDTO();
                            objRoomDTO = objEnterpriseMasterDAL.GetRoomById(objRolePermissionInfo);
                            if (objRoomDTO != null)
                            {
                                t.RoomName = objRoomDTO.RoomName;
                            }
                        });
                    }
                    return objRe;
                }

            }
            return null;
        }

        public List<RoleModuleDetailsDTO> GetRoleModuleDetailsRecord(Int64 RoleID, Int64 RoomID, Int64 CompanyId, Int64 EnterpriseId)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoleID", RoleID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyId", CompanyId), new SqlParameter("@EnterpriseId", EnterpriseId) };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RoleModuleDetailsDTO>("exec GetRoleModuleDetailsRecord @RoleID,@RoomID,@CompanyId,@EnterpriseId", params1).ToList();
            }
        }

        public Int64 Insert(RoleMasterDTO objDTO)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                RoleMaster obj = new RoleMaster();
                obj.ID = 0;
                obj.RoleName = objDTO.RoleName;
                obj.Description = objDTO.Description;
                obj.IsActive = objDTO.IsActive;
                obj.Created = DateTime.UtcNow;

                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Updated = DateTime.UtcNow;
                obj.UserType = objDTO.UserType;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.GUID = Guid.NewGuid();
                obj.EnterpriseId = objDTO.EnterpriseId;
                obj.CompanyId = objDTO.CompanyID ?? 0;
                context.RoleMasters.Add(obj);
                context.SaveChanges();
                if (objDTO.UserType == 1)
                {

                    if (objDTO.RoleWiseRoomsAccessDetails != null && objDTO.RoleWiseRoomsAccessDetails.Count > 0)
                    {
                        foreach (RoleWiseRoomsAccessDetailsDTO item in objDTO.RoleWiseRoomsAccessDetails)
                        {
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
                                objDetails.CompanyId = item.CompanyID;
                                objDetails.EnterpriseId = item.EnterpriseID;

                                context.RoleModuleDetails.Add(objDetails);
                                // context.SaveChanges();
                            }

                        }
                    }

                    IQueryable<RoleRoomAccess> lstExisting = context.RoleRoomAccesses.Where(t => t.RoleId == obj.ID);
                    if (lstExisting.Any())
                    {
                        foreach (var item in lstExisting)
                        {
                            context.RoleRoomAccesses.Remove(item);
                        }
                    }
                    if (objDTO.lstAccess != null && objDTO.lstAccess.Count > 0)
                    {
                        RoleRoomAccess objRoleRoomAccess = null;
                        foreach (var item in objDTO.lstAccess)
                        {
                            if (item.RoomId > 0)
                            {
                                objRoleRoomAccess = new RoleRoomAccess();
                                objRoleRoomAccess.RoleId = obj.ID;
                                objRoleRoomAccess.EnterpriseId = item.EnterpriseId;
                                objRoleRoomAccess.CompanyId = item.CompanyId;
                                objRoleRoomAccess.RoomId = item.RoomId;
                                context.RoleRoomAccesses.Add(objRoleRoomAccess);
                            }
                        }
                    }

                    context.SaveChanges();
                }
                return obj.ID;
            }


        }

        public bool Delete(Int64 id, Int64 userid)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                RoleMaster obj = context.RoleMasters.Single(t => t.ID == id);
                obj.IsDeleted = true;
                obj.IsArchived = false;
                obj.Updated = DateTime.UtcNow;
                obj.LastUpdatedBy = userid;
                context.RoleMasters.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                return true;
            }
        }

        public bool Edit(RoleMasterDTO objDTO)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                RoleMaster obj = new RoleMaster();
                obj = context.RoleMasters.FirstOrDefault(t => t.ID == objDTO.ID);
                
                if (obj != null)
                {
                    obj.RoleName = objDTO.RoleName;
                    obj.Description = objDTO.Description;
                    obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                    obj.Updated = DateTime.UtcNow;
                    try
                    {
                        context.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException)
                    {
                        context.Entry(context.RoleMasters).Reload();
                        context.SaveChanges();
                    }
                
                    if (objDTO.UserType == 1)
                    {
                        var DeleteRMDParam = new SqlParameter[] { new SqlParameter("@RoleID", objDTO.ID) };
                        context.Database.ExecuteSqlCommand("exec [DeleteRoleModuleDetailsByRoleId] @RoleID", DeleteRMDParam);
                        //string strQuery = "DELETE from RoleModuleDetails where RoleID=" + objDTO.ID;
                        //context.Database.ExecuteSqlCommand(strQuery);

                        if (objDTO.RoleWiseRoomsAccessDetails != null && objDTO.RoleWiseRoomsAccessDetails.Count > 0)
                        {
                            foreach (RoleWiseRoomsAccessDetailsDTO item in objDTO.RoleWiseRoomsAccessDetails)
                            {
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
                                    objDetails.CompanyId = item.CompanyID;
                                    objDetails.EnterpriseId = item.EnterpriseID;
                                    context.RoleModuleDetails.Add(objDetails);
                                    // context.SaveChanges();
                                }
                            }
                        }

                        IQueryable<RoleRoomAccess> lstExisting = context.RoleRoomAccesses.Where(t => t.RoleId == obj.ID);
                        
                        if (lstExisting.Any())
                        {
                            foreach (var item in lstExisting)
                            {
                                context.RoleRoomAccesses.Remove(item);
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
                                    context.RoleRoomAccesses.Add(objRoleRoomAccess);
                                }
                            }
                        }

                        var params1 = new SqlParameter[] { new SqlParameter("@RoleID", obj.ID) };
                        context.Database.SqlQuery<object>("exec DeleteUserRoomAccess @RoleID", params1);
                        context.SaveChanges();
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool DeleteRoleByIds(string Ids, int UserId)
        {
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@UserID", UserId),
                                                    new SqlParameter("@Ids", Ids)
                                                };

                context.Database.ExecuteSqlCommand("exec [DeleteRoleByIds] @UserID,@Ids", params1);
                return true;
            }
        }

        public List<RoleMasterDTO> ValidateBeforeDeleteRoles(List<RoleMasterDTO> lstRoles)
        {

            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                lstRoles.ForEach(t =>
                {
                    IQueryable<UserMaster> lstUsers = context.UserMasters.Where(it => it.RoleId == t.ID && it.IsDeleted == false && it.UserType == t.UserType);
                    if (lstUsers.Any())
                    {
                        t.CanBeDeleted = false;
                    }
                    else
                    {
                        t.CanBeDeleted = true;
                    }
                });
            }
            return lstRoles;
        }

        public List<RoleMasterDTO> DeleteRoles(List<RoleMasterDTO> lstRoles)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                int InUse = 0;
                int CanBeDeleted = 0;
                foreach (RoleMasterDTO Roleitem in lstRoles)
                {
                    if (Roleitem.UserType == 1)
                    {
                        if (!context.UserMasters.Any(t => t.RoleId == Roleitem.ID && t.IsDeleted == false && t.UserType == Roleitem.UserType))
                        {
                            RoleMaster objRoleMaster = context.RoleMasters.FirstOrDefault(t => t.ID == Roleitem.ID);
                            if (objRoleMaster != null)
                            {
                                objRoleMaster.IsDeleted = true;
                                objRoleMaster.Updated = DateTime.UtcNow;
                                objRoleMaster.LastUpdatedBy = Roleitem.LastUpdatedBy;
                                context.SaveChanges();
                            }
                            CanBeDeleted += 1;
                            Roleitem.ActionExecuited = true;
                        }
                        else
                        {
                            InUse += 1;
                            Roleitem.ActionExecuited = false;
                        }
                    }
                    else
                    {
                        if (!context.UserMasters.Any(t => t.RoleId == Roleitem.ID && t.UserType == Roleitem.UserType && t.EnterpriseId == Roleitem.EnterpriseId))
                        {
                            EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
                            EnterpriseDTO objEnterprise = new EnterpriseDTO();
                            objEnterprise = objEnterpriseMasterDAL.GetEnterpriseByIdPlain(Roleitem.EnterpriseId);
                            if (objEnterprise != null)
                            {
                                string EnterpriseDbConnection = MasterDbConnectionHelper.GeteTurnsSQLConnectionString(objEnterprise.EnterpriseDBName, MasterDbConnectionType.GeneralReadWrite.ToString("F"));
                                SqlHelper.ExecuteScalar(EnterpriseDbConnection, CommandType.Text, "UPDATE RoleMaster SET Isdeleted = 1 Where ID=" + Roleitem.ID);
                            }
                            CanBeDeleted += 1;
                            Roleitem.ActionExecuited = true;
                        }
                        else
                        {
                            InUse += 1;
                            Roleitem.ActionExecuited = false;
                        }
                    }
                }
                dynamic objret = new ExpandoObject();
                objret.InUse = InUse;
                objret.ToBeDeleted = CanBeDeleted;
                return lstRoles;
            }
        }

        public List<RoleMasterDTO> UnDeleteRoles(List<RoleMasterDTO> lstRoles, Int64 UserID)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                int InUse = 0;
                int CanBeDeleted = 0;
                foreach (RoleMasterDTO Roleitem in lstRoles)
                {
                    if (Roleitem.UserType == 1)
                    {
                        if (!context.UserMasters.Any(t => t.RoleId == Roleitem.ID && t.IsDeleted == false && t.UserType == Roleitem.UserType))
                        {
                            RoleMaster objRoleMaster = context.RoleMasters.FirstOrDefault(t => t.ID == Roleitem.ID);
                            if (objRoleMaster != null)
                            {
                                objRoleMaster.IsDeleted = false;
                                objRoleMaster.Updated = DateTime.UtcNow;
                                objRoleMaster.LastUpdatedBy = Roleitem.LastUpdatedBy;
                                context.SaveChanges();
                            }
                            CanBeDeleted += 1;
                            Roleitem.ActionExecuited = true;
                        }
                        else
                        {
                            InUse += 1;
                            Roleitem.ActionExecuited = false;
                        }
                    }
                    else
                    {
                        if (!context.UserMasters.Any(t => t.RoleId == Roleitem.ID && t.UserType == Roleitem.UserType && t.EnterpriseId == Roleitem.EnterpriseId))
                        {
                            EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
                            EnterpriseDTO objEnterprise = new EnterpriseDTO();
                            objEnterprise = objEnterpriseMasterDAL.GetEnterpriseByIdPlain(Roleitem.EnterpriseId);
                            if (objEnterprise != null)
                            {
                                string EnterpriseDbConnection = MasterDbConnectionHelper.GeteTurnsSQLConnectionString(objEnterprise.EnterpriseDBName, MasterDbConnectionType.GeneralReadWrite.ToString("F"));
                                ////SqlHelper.ExecuteScalar(EnterpriseDbConnection, CommandType.Text, "UPDATE RoleMaster SET Isdeleted = 0 Where ID=" + Roleitem.ID);
                                SqlHelper.ExecuteDataset(EnterpriseDbConnection, "UpdateRoleMaster", Convert.ToString(Roleitem.ID), UserID, 1, 1);
                            }
                            CanBeDeleted += 1;
                            Roleitem.ActionExecuited = true;
                        }
                        else
                        {
                            InUse += 1;
                            Roleitem.ActionExecuited = false;
                        }
                    }
                }
                dynamic objret = new ExpandoObject();
                objret.InUse = InUse;
                objret.ToBeDeleted = CanBeDeleted;
                return lstRoles;
            }
        }

        public List<RoleMasterDTO> DeleteLocalAdminRoles(List<RoleMasterDTO> lstRoles)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                int InUse = 0;
                int CanBeDeleted = 0;
                foreach (RoleMasterDTO Roleitem in lstRoles)
                {
                    if (Roleitem.UserType > 1)
                    {
                        if (!context.UserMasters.Any(t => t.RoleId == Roleitem.ID && t.IsDeleted == false && t.UserType == Roleitem.UserType))
                        {
                            RoleMaster objRoleMaster = context.RoleMasters.FirstOrDefault(t => t.ID == Roleitem.ID);
                            if (objRoleMaster != null)
                            {
                                objRoleMaster.IsDeleted = true;
                                objRoleMaster.Updated = DateTime.UtcNow;
                                objRoleMaster.LastUpdatedBy = Roleitem.LastUpdatedBy;
                                context.SaveChanges();
                            }
                            CanBeDeleted += 1;
                            Roleitem.ActionExecuited = true;
                        }
                        else
                        {
                            InUse += 1;
                            Roleitem.ActionExecuited = false;
                        }
                    }
                }
                dynamic objret = new ExpandoObject();
                objret.InUse = InUse;
                objret.ToBeDeleted = CanBeDeleted;
                return lstRoles;
            }
        }

        public List<RoleMasterDTO> UnDeleteLocalAdminRoles(List<RoleMasterDTO> lstRoles)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                int InUse = 0;
                int CanBeDeleted = 0;
                foreach (RoleMasterDTO Roleitem in lstRoles)
                {
                    if (Roleitem.UserType > 1)
                    {
                        if (!context.UserMasters.Any(t => t.RoleId == Roleitem.ID && t.IsDeleted == true && t.UserType == Roleitem.UserType))
                        {
                            RoleMaster objRoleMaster = context.RoleMasters.FirstOrDefault(t => t.ID == Roleitem.ID);
                            if (objRoleMaster != null)
                            {
                                objRoleMaster.IsDeleted = false;
                                objRoleMaster.Updated = DateTime.UtcNow;
                                objRoleMaster.LastUpdatedBy = Roleitem.LastUpdatedBy;
                                context.SaveChanges();
                            }
                            CanBeDeleted += 1;
                            Roleitem.ActionExecuited = true;
                        }
                        else
                        {
                            InUse += 1;
                            Roleitem.ActionExecuited = false;
                        }
                    }
                }
                dynamic objret = new ExpandoObject();
                objret.InUse = InUse;
                objret.ToBeDeleted = CanBeDeleted;
                return lstRoles;
            }
        }

        public void SetUserRoomAccess(long UserId
            , ref List<RolePermissionInfo> lstEnterPrises
            , ref List<RolePermissionInfo> lstCompanies
            , ref List<RolePermissionInfo> lstRooms
            )
        {

            var entIds = lstEnterPrises.Select(c => c.EnterPriseId).ToList();

            // read with no lock
            using (var txn = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions
                {
                    IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
                }
            ))
            {
                using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
                {
                    // get distinct companyid and roomid where company exists in lstRooms
                    var companyIdsToAccess = context.UserRoomAccesses.Where(t => t.UserId == UserId && entIds.Contains(t.EnterpriseId))
                         .Select(t => new { EnterpriseId = t.EnterpriseId, CompanyId = t.CompanyId, RoomId = t.RoomId }).Distinct()
                         .ToList();


                    foreach (var ent in lstEnterPrises)
                    {
                        ent.IsSelected = companyIdsToAccess.Any(t => t.EnterpriseId > 0 && t.EnterpriseId == ent.EnterPriseId);
                    }

                    foreach (var comp in lstCompanies)
                    {
                        comp.IsSelected = companyIdsToAccess
                            .Any(t => t.CompanyId > 0 && t.EnterpriseId == comp.EnterPriseId && t.CompanyId == comp.CompanyId);
                    }

                    //foreach (var room in lstRooms) 
                    Parallel.ForEach(lstRooms, room =>
                    {
                        room.IsSelected = companyIdsToAccess
                            .Any(t => t.RoomId > 0 && t.CompanyId > 0 && t.EnterpriseId == room.EnterPriseId
                            && t.CompanyId == room.CompanyId && t.RoomId == room.RoomId);
                    });

                }

                txn.Complete();
            }


        }

        public List<RoleMasterDTO> GetPagedRoles(long EnterPriseId, long CompanyId, long RoomId, Int32 StartRowIndex, Int32 MaxRows, out int TotalCount, string SearchTerm, string sortColumnName, bool IsArchived, bool IsDeleted, long LoggedInUser, string UserCreaters, string UserUpdators, string CreatedDateFrom, string CreatedDateTo, string UpdatedDateFrom, string UpdatedDateTo, string UserEnterprises, string UserCompanies, string UserRooms, string UserTypes, string UserRoles)
        {
            List<RoleMasterDTO> lstRoles = new List<RoleMasterDTO>();
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
                              , new SqlParameter("@CurrentEnterpriseID", EnterPriseId)
                              , new SqlParameter("@CurrentCompanyID", CompanyId)
                              , new SqlParameter("@CurrentRoomID", RoomId)
                        };

            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                lstRoles = context.Database.SqlQuery<RoleMasterDTO>("exec GetPagedRoles @StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@UserCreaters,@UserUpdators,@CreatedDateFrom,@CreatedDateTo,@UpdatedDateFrom,@UpdatedDateTo,@IsDeleted,@IsArchived,@UserEnterprises,@UserCompanies,@UserRooms,@UserTypes,@UserRoles,@LoggedInUserID,@CurrentEnterpriseID,@CurrentCompanyID,@CurrentRoomID", params1).ToList();
                if (lstRoles != null && lstRoles.Count > 0)
                {
                    TotalCount = lstRoles.First().TotalRecords;
                }
            }

            return lstRoles;
        }

        public List<RoleNS> GetPagedRoleNS(long LoggedInUser)
        {
            List<RoleNS> lstRoles = new List<RoleNS>();
            var params1 = new SqlParameter[] { new SqlParameter("@LoggedInUserID", LoggedInUser) };
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                lstRoles = context.Database.SqlQuery<RoleNS>("exec GetPagedRolesNarrowSearch @LoggedInUserID", params1).ToList();
            }
            return lstRoles;
        }

        public bool AddUpdateRoleDetails(RoleMasterDTO objDTO, out string status, out long roleID)
        {
            status = "";
            roleID = 0;
            objDTO.Created = DateTime.UtcNow;
            objDTO.Updated = DateTime.UtcNow;

            if (objDTO.ID <= 0)
            {
                // if insert 
                objDTO.IsDeleted = false;
                objDTO.IsArchived = false;
                objDTO.GUID = Guid.NewGuid();
                objDTO.CompanyID = objDTO.CompanyID ?? 0;
            }


            var para = new List<SqlParameter> {
                new SqlParameter("@ID", objDTO.ID),
                new SqlParameter("@RoleName", objDTO.RoleName),
                new SqlParameter("@Description", objDTO.Description),
                new SqlParameter("@IsActive", objDTO.IsActive),
                new SqlParameter("@Created", objDTO.Created),
                new SqlParameter("@CreatedBy", objDTO.CreatedBy),
                new SqlParameter("@Updated", objDTO.Updated),
                new SqlParameter("@LastUpdatedBy", objDTO.LastUpdatedBy),
                new SqlParameter("@IsDeleted", objDTO.IsDeleted),
                new SqlParameter("@IsArchived", objDTO.IsArchived),
                //new SqlParameter("@Room", objDTO.Room),
                new SqlParameter("@CompanyID", objDTO.CompanyID),
                new SqlParameter("@GUID", objDTO.GUID),
                //new SqlParameter("@UDF1", objDTO.UDF1),
                //new SqlParameter("@UDF2", objDTO.UDF2),
                //new SqlParameter("@UDF3", objDTO.UDF3),
                //new SqlParameter("@UDF4", objDTO.UDF4),
                //new SqlParameter("@UDF5", objDTO.UDF5),
                //new SqlParameter("@UDF6", objDTO.UDF6),
                //new SqlParameter("@UDF7", objDTO.UDF7),
                //new SqlParameter("@UDF8", objDTO.UDF8),
                //new SqlParameter("@UDF9", objDTO.UDF9),
                //new SqlParameter("@UDF10", objDTO.UDF10),
                new SqlParameter("@EnterpriseId", objDTO.EnterpriseId),
                new SqlParameter("@UserType", objDTO.UserType)
            };


            SqlParameter paraRoleModuleDetailsXML = new SqlParameter("@RoleModuleDetailsXML", DBNull.Value);
            var paraRoleRoomAccessXML = new SqlParameter("@RoleRoomAccessXML", DBNull.Value);


            if (objDTO.UserType == (int)MasterEnums.UserType.SuperAdmin)
            {
                if (objDTO.RoleWiseRoomsAccessDetails != null && objDTO.RoleWiseRoomsAccessDetails.Count > 0)
                {
                    paraRoleModuleDetailsXML.Value = objDTO.RoleWiseRoomsAccessDetails.ToXML<RoleWiseRoomsAccessDetailsDTO>();
                }

                if (objDTO.lstAccess != null && objDTO.lstAccess.Count > 0)
                {
                    paraRoleRoomAccessXML.Value = objDTO.lstAccess.ToXML<UserAccessDTO>();
                }

            }
            para.Add(paraRoleModuleDetailsXML);
            para.Add(paraRoleRoomAccessXML);

            var ds = SqlHelper.ExecuteDataset(base.DataBaseConnectionString, CommandType.StoredProcedure
                , "uspAddUpdateRoleDetails", para.ToArray());

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                status = Convert.ToString(ds.Tables[0].Rows[0]["Status"]);
                roleID = Convert.ToInt64(ds.Tables[0].Rows[0]["RoleID"]);

                objDTO.ID = status == "ok" ? roleID : objDTO.ID;
            }

            ds.Dispose();


            return status == "ok";
        }

        public RoleMasterDTO GetRoleModuleDetails(Int64 id)
        {
            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ID", id) };
            RoleMasterDTO objres = new RoleMasterDTO();
            using (DataSet ds = SqlHelper.ExecuteDataset(base.DataBaseConnectionString, "uspGetRoleMasterDetails", para))
            {
                if (ds.Tables.Count > 0)
                {
                    objres = (ds.Tables[0].AsEnumerable().Select(row =>
                                new RoleMasterDTO
                                {
                                    ID = row.Field<long>("ID"),
                                    RoleName = row.Field<string>("RoleName"),
                                    Description = row.Field<string>("Description"),
                                    Created = row.Field<DateTime?>("Created"),
                                    Updated = row.Field<DateTime?>("Updated"),
                                    CreatedByName = row.Field<string>("CreatedByName"),
                                    UpdatedByName = row.Field<string>("UpdatedByName"),
                                    LastUpdatedBy = row.Field<long?>("LastUpdatedBy"),
                                    CreatedBy = row.Field<long?>("CreatedBy"),
                                    GUID = row.Field<Guid>("GUID"),
                                    CompanyID = row.Field<long?>("CompanyID"),
                                    CompanyName = row.Field<string>("CompanyName"),
                                    EnterpriseId = row.Field<long>("EnterpriseId"),
                                    EnterpriseName = row.Field<string>("EnterpriseName"),
                                    IsActive = row.Field<bool>("IsActive"),
                                    IsDeleted = row.Field<bool>("IsDeleted"),
                                    IsArchived = row.Field<bool>("IsArchived"),
                                    UserType = row.Field<int>("UserType"),
                                    UserTypeName = row.Field<string>("UserTypeName")
                                })).SingleOrDefault();

                    if (ds.Tables.Count >= 1 && ds.Tables[1] != null)
                    {

                        objres.PermissionList = ds.Tables[1].AsEnumerable().Select(row =>
                        new RoleModuleDetailsDTO
                        {
                            ID = row.Field<long>("ID"),
                            RoleID = row.Field<long>("RoleID"),
                            RoomId = row.Field<long>("RoomId"),
                            RoomName = row.Field<string>("RoomName"),
                            ModuleID = row.Field<long>("ModuleID"),
                            ModuleName = row.Field<string>("ModuleName"),
                            IsInsert = row.Field<bool>("IsInsert"),
                            IsUpdate = row.Field<bool>("IsUpdate"),
                            IsDelete = row.Field<bool>("IsDelete"),
                            IsChecked = row.Field<bool>("IsChecked"),
                            IsView = row.Field<bool>("IsView"),
                            ShowDeleted = row.Field<bool>("ShowDeleted"),
                            ShowArchived = row.Field<bool>("ShowArchived"),
                            ShowUDF = row.Field<bool>("ShowUDF"),
                            IsModule = row.Field<bool>("IsModule"),
                            ModuleValue = row.Field<string>("ModuleValue"),
                            GroupId = row.Field<int?>("GroupId"),
                            //GUID = row.Field<Guid>("GUID"),
                            CompanyID = row.Field<long>("CompanyID"),
                            EnterpriseID = row.Field<long>("EnterpriseID"),
                            DisplayOrderNumber = row.Field<int?>("DisplayOrderNumber"),
                            resourcekey = row.Field<string>("resourcekey"),
                            ShowChangeLog = row.Field<bool>("ShowChangeLog"),
                            ToolTipResourceKey = row.Field<string>("ToolTipResourceKey")
                        }).ToList<RoleModuleDetailsDTO>();

                        string RoomLists = "";
                        if (objres.PermissionList != null && objres.PermissionList.Count > 0)
                        {
                            objres.RoleWiseRoomsAccessDetails = ConvertPermissions(objres.PermissionList, id, ref RoomLists);
                            objres.SelectedRoomAccessValue = RoomLists;
                        }
                    }
                }
            }

            return objres;
            
        }

        public List<UserRoleDTO> GetRoleUsers(int userType, long roleId, string roomIdCSV)
        {
            using (var context = new eTurns_MasterEntities(base.DataBaseEntityConnectionString))
            {
                var paramA = new SqlParameter[] { new SqlParameter("@RoleId", roleId),
                new SqlParameter("@RoomIdCSV", roomIdCSV)
                };

                List<UserRoleDTO> lst = context.Database.SqlQuery<UserRoleDTO>("EXEC [GetUsersByRole] @RoleId,@RoomIdCSV", paramA).ToList();
                return lst;
            }
        }
              


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~RoleMasterDAL()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
