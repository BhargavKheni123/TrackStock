using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;

namespace eTurns.DAL
{
    public class RoleMasterDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]
        public RoleMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public RoleMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region [Class Methods]
        public IEnumerable<RoleMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId, bool? IsCheckCreatedByName, bool? IsCheckUpdatedByName)
        {
            List<RoleMasterDTO> lstRoleMasterDTO = new List<RoleMasterDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), 
                                                   new SqlParameter("@CompanyID", CompanyId),
                                                   new SqlParameter("@IsCreatedByName", IsCheckCreatedByName ?? (object)DBNull.Value),
                                                   new SqlParameter("@IsUpdatedByName", IsCheckUpdatedByName ?? (object)DBNull.Value)  };

                lstRoleMasterDTO = (from u in context.Database.SqlQuery<RoleMasterDTO>("EXEC [GetRoleMaster] @RoomID,@CompanyID,@IsCreatedByName,@IsUpdatedByName", params1)
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
                
            }
            return lstRoleMasterDTO;
        }
        
        public RoleMasterDTO GetRecord(Int64 id)
        {
            RoleMasterDTO objresult = new RoleMasterDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ID", id) };

                objresult = (from u in context.Database.SqlQuery<RoleMasterDTO>("EXEC [GetRoleMasterByID] @ID", params1)
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
                if (objresult == null)
                {
                    objresult=new RoleMasterDTO();
                }
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
                var params1 = new SqlParameter[] { new SqlParameter("@ID", id) };

                objresult = (from u in context.Database.SqlQuery<RoleMasterDTO>("EXEC [GetRoleByID] @ID", params1)
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

                if(objresult != null)
                {
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
                }
                
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
        private List<RoleWiseRoomsAccessDetailsDTO> ConvertPermissions(List<RoleModuleDetailsDTO> objData, Int64 RoleID, ref string RoomLists)
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
                        objRoomDTO = new RoomDAL(base.DataBaseName).GetRoomByIDNormal(objRoleRooms.RoomID);

                        if (objRoomDTO != null)
                        {
                            objRoleRooms.RoomName = objRoomDTO.RoomName;
                            objRoleRooms.CompanyName = objRoomDTO.CompanyName;
                            objRoleRooms.EnterPriseName = objRoomDTO.EnterpriseName;
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
                        objRoomDTO = new RoomDAL(base.DataBaseName).GetRoomByIDNormal(objRoleRooms.RoomID);
                        if (objRoomDTO != null)
                        {
                            objRoleRooms.RoomName = objRoomDTO.RoomName;
                            objRoleRooms.CompanyName = objRoomDTO.CompanyName;
                            objRoleRooms.EnterPriseName = objRoomDTO.EnterpriseName;
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
        public List<RoleRoomReplanishmentDetailsDTO> GetRoleRoomReplanishmentDetailsRecord(Int64 RoleID)
        {
            RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
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
                        string strRoomIDs = string.Join(",", objRe.Select(x => x.RoomID).Distinct().ToArray());
                        List<RoomDTO> lstRoom = objRoomDAL.GetRoomByIDsNormal(strRoomIDs);
                        objRe.ForEach(t =>
                        {
                            RolePermissionInfo objRolePermissionInfo = new RolePermissionInfo();
                            objRolePermissionInfo.EnterPriseId = t.EnterpriseId;
                            objRolePermissionInfo.CompanyId = t.CompanyId;
                            objRolePermissionInfo.RoomId = t.RoomID;
                            RoomDTO objRoomDTO = new RoomDTO();
                            objRoomDTO = lstRoom.Where(x => x.RoomId == t.RoomID).FirstOrDefault();
                            if (objRoomDTO != null)
                            {
                                t.CompanyName = objRoomDTO.CompanyName;
                                t.RoomName = objRoomDTO.RoomName;
                            }
                        });
                    }
                    return objRe;
                }

            }
            return null;
        }
        public List<RoleModuleDetailsDTO> GetRoleModuleDetailsRecord(Int64 RoleID, Int64 RoomID)
        {
            //RoleModuleDetailsDTO objresult = new RoleModuleDetailsDTO();
            //string qry = "";
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<SqlParameter> para = new List<SqlParameter>() {
                new SqlParameter("@RoleID",RoleID),
                new SqlParameter("@RoomID",RoomID)
                };
                List<RoleModuleDetailsDTO> objRe = (from u in context.Database.SqlQuery<RoleModuleDetailsDTO>("exec uspGetRoleModuleDetailsForEnt @RoleID,@RoomID"
                                                    , para.ToArray())
                                                    select new RoleModuleDetailsDTO
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
                                                        GroupId = u.GroupId,
                                                        GUID = u.GUID,
                                                        CompanyID = u.CompanyID,
                                                        EnterpriseID = u.EnterpriseID,
                                                        DisplayOrderNumber = u.DisplayOrderNumber,
                                                        resourcekey = u.resourcekey,
                                                        ShowChangeLog = u.ShowChangeLog,
                                                        ToolTipResourceKey = u.ToolTipResourceKey
                                                    }).ToList<RoleModuleDetailsDTO>();
                return objRe;



                //if (RoleID > 0)
                //{
                //    List<RoleModuleDetailsDTO> objRe = new List<RoleModuleDetailsDTO>();
                //    if (RoomID > 0)
                //        qry = @"SELECT M.ID as ModuleID,M.DisplayName as ModuleName,M.IsModule,M.GroupID,M.ResourceKey,M.ToolTipResourceKey,ISNULL(A.ID,0) as ID ,ISnull(A.RoleID,0) as RoleID ,ISNULL(A.RoomID,0) as RoomID,ISNULL(A.CompanyID,0) as CompanyID,ISNULL(A.EnterpriseID,0) as EnterpriseID,ISNULL(R.RoomName,'') as RoomName,ISNULL(A.IsInsert,0) as IsInsert,ISNULL(A.IsUpdate,0)  as IsUpdate,ISNULL(A.ISDelete,0) as ISDelete ,ISNULL(A.IsView,0) as IsView ,ISNULL(A.IsChecked,0) as IsChecked,ModuleValue,ISNULL(A.ShowDeleted,0) as ShowDeleted,ISNULL(A.ShowArchived,0) as ShowArchived,ISNULL(A.ShowUDF,0) as ShowUDF,M.DisplayOrderNumber,ISNULL(A.ShowChangeLog,0) as ShowChangeLog FROM ModuleMaster M LEFT OUTER  JOIN RoleModuleDetails A ON M.ID = A.ModuleID AND A.RoleId='" + RoleID + "' and A.RoomID='" + RoomID + "'   LEFT OUTER  JOIN Room R ON R.ID = A.RoomID and ISNULL(R.IsDeleted,0) = 0  LEFT OUTER  JOIN CompanyMaster C ON C.ID = A.CompanyId and ISNULL(C.IsDeleted,0) = 0 WHERE ISNULL(M.IsDeleted,0) = 0   ORDER BY M.DisplayOrderNumber,M.GroupID,M.DisplayName";
                //    else
                //        qry = @"SELECT M.ID as ModuleID,M.DisplayName as ModuleName,M.IsModule,M.GroupID,M.ResourceKey,M.ToolTipResourceKey,ISNULL(A.ID,0) as ID ,ISnull(A.RoleID,0) as RoleID ,ISNULL(A.RoomID,0) as RoomID,ISNULL(A.CompanyID,0) as CompanyID,ISNULL(A.EnterpriseID,0) as EnterpriseID,ISNULL(R.RoomName,'') as RoomName,ISNULL(A.IsInsert,0) as IsInsert,ISNULL(A.IsUpdate,0)  as IsUpdate,ISNULL(A.ISDelete,0) as ISDelete ,ISNULL(A.IsView,0) as IsView,ISNULL(A.IsChecked,0) as IsChecked,ModuleValue,ISNULL(A.ShowDeleted,0) as ShowDeleted,ISNULL(A.ShowArchived,0) as ShowArchived,ISNULL(A.ShowUDF,0) as ShowUDF,M.DisplayOrderNumber,ISNULL(A.ShowChangeLog,0) as ShowChangeLog FROM ModuleMaster M LEFT OUTER  JOIN RoleModuleDetails A ON M.ID = A.ModuleID  AND A.RoleId='" + RoleID + "' LEFT OUTER  JOIN Room R ON R.ID = A.RoomID and ISNULL(R.IsDeleted,0) = 0  LEFT OUTER  JOIN CompanyMaster C ON C.ID = A.CompanyId and ISNULL(C.IsDeleted,0) = 0 WHERE ISNULL(M.IsDeleted,0) = 0   ORDER BY M.DisplayOrderNumber,M.GroupID,M.DisplayName";

                //    objRe = (from u in context.Database.SqlQuery<RoleModuleDetailsDTO>(qry)
                //             select new RoleModuleDetailsDTO
                //             {
                //                 ID = u.ID,
                //                 RoleID = u.RoleID,
                //                 RoomId = u.RoomId,
                //                 RoomName = u.RoomName,
                //                 ModuleID = u.ModuleID,
                //                 ModuleName = u.ModuleName,
                //                 IsInsert = u.IsInsert,
                //                 IsUpdate = u.IsUpdate,
                //                 IsDelete = u.IsDelete,
                //                 IsChecked = u.IsChecked,
                //                 IsView = u.IsView,
                //                 ShowDeleted = u.ShowDeleted,
                //                 ShowArchived = u.ShowArchived,
                //                 ShowUDF = u.ShowUDF,
                //                 IsModule = u.IsModule,
                //                 ModuleValue = u.ModuleValue,
                //                 GroupId = u.GroupId,
                //                 GUID = u.GUID,
                //                 CompanyID = u.CompanyID,
                //                 EnterpriseID = u.EnterpriseID,
                //                 DisplayOrderNumber = u.DisplayOrderNumber,
                //                 resourcekey = u.resourcekey,
                //                 ShowChangeLog = u.ShowChangeLog,
                //                 ToolTipResourceKey = u.ToolTipResourceKey
                //             }).ToList();
                //    return objRe;
                //}
                //else
                //{
                //    return (from u in context.Database.SqlQuery<RoleModuleDetailsDTO>(@"SELECT M.ID as ModuleID,M.DisplayName as ModuleName,M.ResourceKey,M.ToolTipResourceKey,M.IsModule,M.GroupID,ISNULL(A.ID,0) as ID ,ISnull(A.RoleID,0) as RoleID,ISNULL(A.RoomID,0) as RoomID,ISNULL(A.CompanyID,0) as CompanyID,ISNULL(A.EnterpriseID,0) as EnterpriseID,ISNULL(R.RoomName,'') as RoomName,ISNULL(A.IsInsert,0)  as IsInsert,ISNULL(A.IsUpdate,0) as IsUpdate ,ISNULL(A.ISDelete,0) as ISDelete,ISNULL(A.IsView,0) as IsView,ISNULL(A.IsChecked,0) as IsChecked,ModuleValue,ISNULL(A.ShowDeleted,0) as ShowDeleted,ISNULL(A.ShowArchived,0) as ShowArchived,ISNULL(A.ShowUDF,0) as ShowUDF,M.DisplayOrderNumber,ISNULL(A.ShowChangeLog,0) as ShowChangeLog FROM ModuleMaster M LEFT OUTER  JOIN RoleModuleDetails A ON M.ID = A.ModuleID AND A.RoleId='0'  LEFT OUTER  JOIN Room R ON R.ID = A.RoomID WHERE ISNULL(M.IsDeleted,0) = 0 ORDER BY M.DisplayOrderNumber,M.GroupID,M.DisplayName")
                //            select new RoleModuleDetailsDTO
                //            {
                //                ID = u.ID,
                //                RoleID = u.RoleID,
                //                RoomId = u.RoomId,
                //                RoomName = u.RoomName,
                //                ModuleID = u.ModuleID,
                //                ModuleName = u.ModuleName,
                //                IsInsert = u.IsInsert,
                //                IsUpdate = u.IsUpdate,
                //                IsDelete = u.IsDelete,
                //                IsView = u.IsView,
                //                ShowDeleted = u.ShowDeleted,
                //                ShowArchived = u.ShowArchived,
                //                ShowUDF = u.ShowUDF,
                //                IsModule = u.IsModule,
                //                ModuleValue = u.ModuleValue,
                //                GroupId = u.GroupId,
                //                CompanyID = u.CompanyID,
                //                EnterpriseID = u.EnterpriseID,
                //                DisplayOrderNumber = u.DisplayOrderNumber,
                //                resourcekey = u.resourcekey,
                //                ShowChangeLog = u.ShowChangeLog,
                //                ToolTipResourceKey = u.ToolTipResourceKey
                //            }).ToList();
                //}

            }
        }
       
        public Int64 Insert(RoleMasterDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                RoleMaster obj = new RoleMaster();
                obj.ID = objDTO.ID;
                obj.RoleName = objDTO.RoleName;
                obj.Description = objDTO.Description;
                obj.IsActive = objDTO.IsActive;
                obj.Created = DateTimeUtility.DateTimeNow;

                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.CompanyID = objDTO.CompanyID;
                obj.EnterpriseId = objDTO.EnterpriseId;
                obj.UserType = objDTO.UserType;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.IsActive = true;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.GUID = Guid.NewGuid();
                obj.EnterpriseId = objDTO.EnterpriseId;

                context.RoleMasters.Add(obj);
                context.SaveChanges();
                if (objDTO.UserType > 1)
                {


                    if (objDTO.RoleWiseRoomsAccessDetails != null && objDTO.RoleWiseRoomsAccessDetails.Count > 0)
                    {
                        foreach (RoleWiseRoomsAccessDetailsDTO item in objDTO.RoleWiseRoomsAccessDetails)
                        {
                            if (item.PermissionList != null && item.PermissionList.Count > 0)
                            {
                                if (objDTO.UserType == 2)
                                {
                                    item.PermissionList = item.PermissionList.Where(x => x.ModuleID != 41).ToList();
                                }
                                else if (objDTO.UserType == 3)
                                {
                                    item.PermissionList = item.PermissionList.Where(x => x.ModuleID != 39 && x.ModuleID != 41).ToList();
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

                                    context.RoleModuleDetails.Add(objDetails);
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
                    //if (objDTO.ReplenishingRooms != null && objDTO.ReplenishingRooms.Count > 0)
                    //{
                    //    foreach (RoleRoomReplanishmentDetailsDTO item in objDTO.ReplenishingRooms)
                    //    {
                    //        RoleRoomsDetail objDetails = new RoleRoomsDetail();
                    //        objDetails.GUID = Guid.NewGuid();
                    //        objDetails.RoleId = obj.ID;
                    //        objDetails.RoomId = item.RoomID;
                    //        objDetails.IsRoomAccess = false;
                    //        objDetails.EnterpriseId = item.EnterpriseId;
                    //        objDetails.CompanyId = item.CompanyId;
                    //        context.RoleRoomsDetails(objDetails);
                    //        // context.SaveChanges();

                    //    }
                    //}


                    context.SaveChanges();
                }
                return obj.ID;
            }


        }
        public bool Delete(Int64 id, Int64 userid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                RoleMaster obj = context.RoleMasters.Single(t => t.ID == id);
                obj.IsDeleted = true;
                obj.IsArchived = false;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.LastUpdatedBy = userid;
                context.RoleMasters.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                return true;
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
                var params1 = new SqlParameter[] { new SqlParameter("@RoleID", objDTO.ID) };
                context.Database.ExecuteSqlCommand("EXEC [DeleteRoleModuleDetailsByRoleID] @RoleID", params1);
                
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
                                    context.RoleModuleDetails.Add(objDetails);
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
                    //strQuery = "DELETE from RoleRoomsDetails where RoleID=" + objDTO.ID;
                    //context.Database.ExecuteSqlCommand(strQuery);

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
                var params1 = new SqlParameter[] { new SqlParameter("@IDs", IDs),
                                                   new SqlParameter("@UserID", userid) };

                context.Database.ExecuteSqlCommand("EXEC DeleteRoleMaster @IDs,@UserID", params1);
                return true;
            }
        }

        public bool UnDeleteRecords(string IDs, long userid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@IDs", IDs), new SqlParameter("@UserID", userid), new SqlParameter("@IsMasterUpdate", Convert.ToInt64(0)), new SqlParameter("@IsDeleteUndelete", Convert.ToInt64(0)) };
                context.Database.ExecuteSqlCommand("exec UpdateRoleMaster @IDs,@UserID,@IsMasterUpdate,@IsDeleteUndelete", params1);

                return true;
            }
        }
       
        public List<RoleMasterDTO> GetAllRoleByUserType(int UserType, long? CompanyId)
        {
            List<RoleMasterDTO> lstRoles = new List<RoleMasterDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (UserType == 2)
                {
                    lstRoles = (from u in context.RoleMasters
                                where u.IsActive == true && u.IsDeleted == false && u.IsArchived == false && u.UserType == 2
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
                if (UserType == 3)
                {
                    lstRoles = (from u in context.RoleMasters
                                where u.IsActive == true && u.IsDeleted == false && u.IsArchived == false && u.UserType == 3 && u.CompanyID == (CompanyId ?? 0)
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
            }
            return lstRoles;
        }
        public List<RoleMasterDTO> ValidateBeforeDeleteRoles(List<RoleMasterDTO> lstRoles)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstRoles.ForEach(t =>
                {
                    if (context.UserMasters.Any(it => it.RoleId == t.ID && it.IsDeleted == false && it.UserType == t.UserType))
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
        public bool UserHasCompanyAccess(long UserId, long CompanyID, long SelectedRoleId, long UsersRoleId)
        {
            bool retVal = false;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (SelectedRoleId == UsersRoleId)
                {
                    retVal = context.UserRoomAccesses.Any(t => t.UserId == UserId && t.CompanyId == CompanyID);
                }
                else
                {
                    return true;
                }
            }
            return retVal;

        }



        public bool UserHasRoomAccess(long UserId, long CompanyId, long RoomId, long SelectedRoleId, long UsersRoleId)
        {
            bool retVal = false;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (SelectedRoleId == UsersRoleId)
                {
                    retVal = context.UserRoomAccesses.Any(t => t.UserId == UserId && t.CompanyId == CompanyId && t.RoomId == RoomId);
                }
                else
                {
                    return true;
                }

            }
            return retVal;

        }

        public void UserHasCompanyRoomAccess(long UserId, List<RolePermissionInfo> lstCompanies, List<RolePermissionInfo> lstRooms, long SelectedRoleId, long UsersRoleId)
        {


            if (SelectedRoleId == UsersRoleId)
            {

                var cmpIds = lstCompanies.Select(c => c.CompanyId).ToList();

                // read with no lock
                using (var txn = new TransactionScope(
                    TransactionScopeOption.Required,
                    new TransactionOptions
                    {
                        IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
                    }
                ))
                {
                    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                    {
                        // get distinct companyid and roomid where company exists in lstRooms
                        var companyIdsToAccess = context.UserRoomAccesses
                             .Where(t => t.CompanyId > 0)
                             .Where(t => t.UserId == UserId && cmpIds.Contains(t.CompanyId))
                             .Select(t => new { CompanyId = t.CompanyId, RoomId = t.RoomId }).Distinct()
                             .ToList();

                        foreach (var room in lstRooms)
                        {
                            room.IsSelected = companyIdsToAccess
                                .Any(x => x.CompanyId == room.CompanyId && x.RoomId == room.RoomId && x.RoomId > 0);
                        }

                        foreach (var company in lstCompanies)
                        {
                            company.IsSelected = companyIdsToAccess.Any(cId => cId.CompanyId == company.CompanyId);
                        }

                    }

                    txn.Complete();
                }

            }
            else
            {
                foreach (var room in lstRooms)
                {
                    room.IsSelected = true;
                }

                foreach (var company in lstCompanies)
                {
                    company.IsSelected = true;
                }

            }
        }

        public void AssignEnterpiseRoleToCompanyUser(Int64 UserId, Int64 RoleId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var paramA = new SqlParameter[] { new SqlParameter("@UserId", UserId),
                                                  new SqlParameter("@RoleId", RoleId) };
                context.Database.ExecuteSqlCommand("EXEC [AssignEnterpiseRoleToCompanyUser] @UserId,@RoleId", paramA);
            }
        }

        public bool AddUpdateRoleData(RoleMasterDTO objDTO, Enums.DBOperation dBOperation, out string status, out long roleID)
        {
            status = "";
            roleID = 0;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.Updated = DateTimeUtility.DateTimeNow;

            if (dBOperation == Enums.DBOperation.INSERT)
            {
                // if insert 
                objDTO.IsActive = true;
                objDTO.IsDeleted = false;
                objDTO.IsArchived = false;
                objDTO.GUID = Guid.NewGuid();
            }


            var para = new List<SqlParameter> {
                new SqlParameter("@Operation", dBOperation.ToString()),
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
                new SqlParameter("@Room", objDTO.Room),
                new SqlParameter("@CompanyID", objDTO.CompanyID),
                new SqlParameter("@GUID", objDTO.GUID),
                new SqlParameter("@EnterpriseId", objDTO.EnterpriseId),
                new SqlParameter("@UserType", objDTO.UserType)
            };


            SqlParameter paraRoleModuleDetailsXML = new SqlParameter("@RoleModuleDetailsXML", DBNull.Value);
            var paraRoleRoomAccessXML = new SqlParameter("@RoleRoomAccessXML", DBNull.Value);


            if (objDTO.UserType > (int) eTurns.DTO.Enums.UserType.SuperAdmin)
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
                , "uspAddUpdateRoleData", para.ToArray());

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
            using (DataSet ds = SqlHelper.ExecuteDataset(base.DataBaseConnectionString, "uspGetRoleMasterDetailsForEnt", para))
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
                                    RoomName = row.Field<string>("RoomName"),
                                    LastUpdatedBy = row.Field<long?>("LastUpdatedBy"),
                                    CreatedBy = row.Field<long?>("CreatedBy"),
                                    Room = row.Field<long>("Room"),
                                    GUID = row.Field<Guid>("GUID"),
                                    CompanyID = row.Field<long?>("CompanyID"),
                                    EnterpriseId = row.Field<long>("EnterpriseId"),
                                    UserType = row.Field<int>("UserType"),
                                    IsDeleted = row.Field<bool>("IsDeleted"),
                                    IsArchived = row.Field<bool>("IsArchived"),
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

                    if (ds.Tables.Count >= 2 && ds.Tables[2] != null)
                    {
                        objres.lstAccess = ds.Tables[2].AsEnumerable().Select(row =>
                        new UserAccessDTO
                        {
                            CompanyId = row.Field<long>("CompanyId"),
                            EnterpriseId = row.Field<long>("EnterpriseId"),
                            ID = row.Field<long>("ID"),
                            RoleId = row.Field<long>("RoleId"),
                            RoomId = row.Field<long>("RoomId"),
                            UserId = row.Field<long>("UserId"),
                            EnterpriseName = row.Field<string>("EnterpriseName"),
                            CompanyName = row.Field<string>("CompanyName"),
                            RoomName = row.Field<string>("RoomName")
                        }).ToList();
                    }
                }
            }

            return objres;


            //RoleMasterDTO objresult = new RoleMasterDTO();
            //using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            //{

            //    objresult = (from u in context.Database.SqlQuery<RoleMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, A.GUID FROM RoleMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE  A.ID=" + id.ToString())
            //                 select new RoleMasterDTO
            //                 {
            //                     ID = u.ID,
            //                     RoleName = u.RoleName,
            //                     Description = u.Description,
            //                     Created = u.Created,
            //                     Updated = u.Updated,
            //                     CreatedByName = u.CreatedByName,
            //                     UpdatedByName = u.UpdatedByName,
            //                     RoomName = u.RoomName,
            //                     LastUpdatedBy = u.LastUpdatedBy,
            //                     CreatedBy = u.CreatedBy,
            //                     Room = u.Room,
            //                     GUID = u.GUID,
            //                     CompanyID = u.CompanyID,
            //                     EnterpriseId = u.EnterpriseId,
            //                     UserType = u.UserType,
            //                     IsDeleted = u.IsDeleted,
            //                     IsArchived = u.IsArchived
            //                 }).SingleOrDefault();

            //    objresult.PermissionList = GetRoleModuleDetailsRecord(id, 0);
            //    string RoomLists = "";
            //    if (objresult.PermissionList != null && objresult.PermissionList.Count > 0)
            //    {
            //        objresult.RoleWiseRoomsAccessDetails = ConvertPermissions(objresult.PermissionList, id, ref RoomLists);
            //        objresult.SelectedRoomAccessValue = RoomLists;
            //    }
            //    objresult.lstAccess = (from rra in context.RoleRoomAccesses
            //                           join cm in context.CompanyMasters on rra.CompanyId equals cm.ID into rra_cm_join
            //                           from rra_cm in rra_cm_join.DefaultIfEmpty()
            //                           join rm in context.Rooms on rra.RoomId equals rm.ID into rra_rm_join
            //                           from rra_rm in rra_rm_join.DefaultIfEmpty()
            //                           where rra.RoleId == id
            //                           select new UserAccessDTO
            //                           {
            //                               CompanyId = rra.CompanyId,
            //                               EnterpriseId = rra.EnterpriseId,
            //                               ID = rra.ID,
            //                               RoleId = id,
            //                               RoomId = rra.RoomId,
            //                               UserId = 0,
            //                               EnterpriseName = string.Empty,
            //                               CompanyName = rra_cm.Name,
            //                               RoomName = rra_rm.RoomName
            //                           }).ToList();


            //}

            //return objresult;
        }

        public List<UserRoleDTO> GetRoleUsers(int userType, long roleId, string roomIdCSV)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var paramA = new SqlParameter[] { new SqlParameter("@RoleId", roleId)
                    ,new SqlParameter("@RoomIdCSV", roomIdCSV)
                };

                List<UserRoleDTO> lst = context.Database.SqlQuery<UserRoleDTO>("EXEC [GetUsersByRoleForEnt] @RoleId,@RoomIdCSV", paramA).ToList();
                return lst;
            }
        }

        #endregion
    }
}
