using Dynamite.Extensions;
using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public class PermissionTemplateDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        public PermissionTemplateDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public PermissionTemplateDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region [Class Methods]

        public PermissionTemplateDTO GetTemplateByID(long ID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from pt in context.PermissionTemplateMasters
                        join umc in context.UserMasters on pt.CreatedBy equals umc.ID into pt_umc_join
                        from pt_umc in pt_umc_join.DefaultIfEmpty()
                        join umu in context.UserMasters on pt.UpdatedBy equals umu.ID into pt_umu_join
                        from pt_umu in pt_umu_join.DefaultIfEmpty()
                        where pt.ID == ID
                        select new PermissionTemplateDTO
                        {
                            Created = pt.Created,
                            CreatedBy = pt.CreatedBy,
                            CreatedByName = pt_umc.UserName,
                            ID = pt.ID,
                            IsDeleted = pt.IsDeleted,
                            TemplateName = pt.TemplateName,
                            Description = pt.Description,
                            Updated = pt.Updated,
                            UpdatedBy = pt.UpdatedBy,
                            UpdatedByName = pt_umu.UserName



                        }).FirstOrDefault();
            }
        }

        public List<PermissionTemplateDTO> GetPagedTemplates(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, bool IsDeleted, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            List<PermissionTemplateDTO> lstTemplates = new List<PermissionTemplateDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {


                IEnumerable<PermissionTemplateDTO> permissionTemplateList = (from pt in context.PermissionTemplateMasters
                                                                             join umc in context.UserMasters on pt.CreatedBy equals umc.ID into pt_umc_join
                                                                             from pt_umc in pt_umc_join.DefaultIfEmpty()
                                                                             join umu in context.UserMasters on pt.UpdatedBy equals umu.ID into pt_umu_join
                                                                             from pt_umu in pt_umu_join.DefaultIfEmpty()
                                                                             orderby sortColumnName
                                                                             where pt.IsDeleted == IsDeleted
                                                                             select new PermissionTemplateDTO
                                                                             {
                                                                                 Created = pt.Created,
                                                                                 CreatedBy = pt.CreatedBy,
                                                                                 CreatedByName = pt_umc.UserName,
                                                                                 ID = pt.ID,
                                                                                 IsDeleted = pt.IsDeleted,
                                                                                 TemplateName = pt.TemplateName,
                                                                                 Description = pt.Description,
                                                                                 Updated = pt.Updated,
                                                                                 UpdatedBy = pt.UpdatedBy,
                                                                                 UpdatedByName = pt_umu.UserName
                                                                             }).ToList();
                if (!string.IsNullOrEmpty(SearchTerm))
                {
                    if (SearchTerm.Contains("[###]"))
                    {
                        string[] stringSeparators = new string[] { "[###]" };
                        string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                        string NewSearchValue = string.Empty;
                        if (Fields.Length > 2)
                        {
                            if (!string.IsNullOrEmpty(Fields[2]))
                                NewSearchValue = Fields[2];
                            else
                                NewSearchValue = string.Empty;
                        }
                        else
                        {
                            NewSearchValue = string.Empty;
                        }
                        permissionTemplateList = permissionTemplateList.Where(t =>
                               ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedBy.ToString())))
                           && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedBy.ToString())))
                           && ((Fields[1].Split('@')[2] == "") || (t.Created >= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone) && t.Created <= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone)))
                           && ((Fields[1].Split('@')[3] == "") || (t.Updated >= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone) && t.Updated <= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone)))
                              ).ToList();

                        permissionTemplateList = permissionTemplateList.Where(t =>

                               (t.TemplateName ?? "").IndexOf(NewSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0
                            || (t.Description ?? "").IndexOf(NewSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0
                            || (t.CreatedByName ?? "").IndexOf(NewSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0
                            || (t.UpdatedByName ?? "").IndexOf(NewSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0);
                    }
                    else
                    {
                        permissionTemplateList = permissionTemplateList.Where(t =>

                               (t.TemplateName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                            || (t.Description ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                            || (t.CreatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                            || (t.UpdatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0);
                    }
                }
                TotalCount = permissionTemplateList.Count();

                return permissionTemplateList.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows).ToList();
            }


        }
        public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE PermissionTemplateMaster SET Updated = getutcdate() ,UpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.Database.ExecuteSqlCommand(strQuery);
                return true;
            }
        }
        public bool UnDeleteRecords(string IDs, Int64 userid, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE PermissionTemplateMaster SET Updated = getutcdate() ,UpdatedBy = " + userid.ToString() + ", IsDeleted=0 WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.Database.ExecuteSqlCommand(strQuery);
                return true;
            }
        }
        public List<PermissionTemplateDTO> GetAllTemplates()
        {
            List<PermissionTemplateDTO> lstTemplates = new List<PermissionTemplateDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstTemplates = (from pt in context.PermissionTemplateMasters
                                join umc in context.UserMasters on pt.CreatedBy equals umc.ID into pt_umc_join
                                from pt_umc in pt_umc_join.DefaultIfEmpty()
                                join umu in context.UserMasters on pt.UpdatedBy equals umu.ID into pt_umu_join
                                from pt_umu in pt_umu_join.DefaultIfEmpty()
                                where pt.IsDeleted == false
                                select new PermissionTemplateDTO
                                {
                                    Created = pt.Created,
                                    CreatedBy = pt.CreatedBy,
                                    CreatedByName = pt_umc.UserName,
                                    ID = pt.ID,
                                    IsDeleted = pt.IsDeleted,
                                    TemplateName = pt.TemplateName,
                                    Description = pt.Description,
                                    Updated = pt.Updated,
                                    UpdatedBy = pt.UpdatedBy,
                                    UpdatedByName = pt_umu.UserName
                                }).ToList();
            }
            return lstTemplates;
        }

        public PermissionTemplateDTO SaveTemplate(PermissionTemplateDTO objPermissionTemplateDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                PermissionTemplateMaster objPermissionTemplateMaster = null;
                if (objPermissionTemplateDTO.ID > 0)
                {
                    objPermissionTemplateMaster = context.PermissionTemplateMasters.FirstOrDefault(t => t.ID == objPermissionTemplateDTO.ID);
                    if (objPermissionTemplateMaster != null)
                    {
                        objPermissionTemplateMaster.TemplateName = objPermissionTemplateDTO.TemplateName;
                        objPermissionTemplateMaster.Description = objPermissionTemplateDTO.Description;
                        objPermissionTemplateMaster.UpdatedBy = objPermissionTemplateDTO.UpdatedBy;
                        objPermissionTemplateMaster.Updated = DateTimeUtility.DateTimeNow;

                        context.SaveChanges();
                    }
                }
                else
                {
                    objPermissionTemplateMaster = new PermissionTemplateMaster();
                    objPermissionTemplateMaster.TemplateName = objPermissionTemplateDTO.TemplateName;
                    objPermissionTemplateMaster.Description = objPermissionTemplateDTO.Description;
                    objPermissionTemplateMaster.UpdatedBy = objPermissionTemplateDTO.UpdatedBy;
                    objPermissionTemplateMaster.Updated = DateTimeUtility.DateTimeNow;
                    objPermissionTemplateMaster.CreatedBy = objPermissionTemplateDTO.CreatedBy;
                    objPermissionTemplateMaster.Created = DateTimeUtility.DateTimeNow;
                    objPermissionTemplateMaster.IsDeleted = false;
                    objPermissionTemplateMaster.EnterpriseID = objPermissionTemplateDTO.EnterpriseID;
                    context.PermissionTemplateMasters.Add(objPermissionTemplateMaster);
                    context.SaveChanges();
                    objPermissionTemplateDTO.ID = objPermissionTemplateMaster.ID;
                }
            }
            if (objPermissionTemplateDTO.lstPermissions == null)
            {
                objPermissionTemplateDTO.lstPermissions = new List<PermissionTemplateDetailDTO>();
            }
            SaveTemplatePermissionMap(objPermissionTemplateDTO.lstPermissions.ToList(), objPermissionTemplateDTO.ID, objPermissionTemplateDTO.CreatedBy, objPermissionTemplateDTO.EnterpriseID);
            return objPermissionTemplateDTO;
        }

        public void SaveTemplatePermissionMap(List<PermissionTemplateDetailDTO> TemplatePermission, long PermissionTemplateID, long UserID, long EnterpriseID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IQueryable<PermissionTemplateDetail> lstMap = context.PermissionTemplateDetails.Where(t => t.PermissionTemplateID == PermissionTemplateID);
                if (lstMap.Any())
                {
                    foreach (var item in lstMap)
                    {
                        context.PermissionTemplateDetails.Remove(item);
                    }
                }
                if (TemplatePermission != null && TemplatePermission.Count > 0)
                {
                    foreach (var item in TemplatePermission)
                    {
                        PermissionTemplateDetail objPermissionTemplateDetail = new PermissionTemplateDetail();
                        objPermissionTemplateDetail.EnterpriseID = EnterpriseID;
                        objPermissionTemplateDetail.GUID = Guid.NewGuid();
                        objPermissionTemplateDetail.ID = item.ID;
                        objPermissionTemplateDetail.IsChecked = item.IsChecked;
                        objPermissionTemplateDetail.IsDelete = item.IsDelete;
                        objPermissionTemplateDetail.IsInsert = item.IsInsert;
                        objPermissionTemplateDetail.IsUpdate = item.IsUpdate;
                        objPermissionTemplateDetail.IsView = item.IsView;
                        objPermissionTemplateDetail.ModuleID = item.ModuleID;
                        objPermissionTemplateDetail.ModuleValue = item.ModuleValue;
                        objPermissionTemplateDetail.PermissionTemplateID = PermissionTemplateID;
                        objPermissionTemplateDetail.ShowArchived = item.ShowArchived;
                        objPermissionTemplateDetail.ShowDeleted = item.ShowDeleted;
                        objPermissionTemplateDetail.ShowUDF = item.ShowUDF;
                        context.PermissionTemplateDetails.Add(objPermissionTemplateDetail);
                    }
                }
                context.SaveChanges();
            }
        }

        public List<PermissionTemplateDetailDTO> GetPermissionDetailsRecord(Int64 PermissionTemplateID)
        {
            List<PermissionTemplateDetailDTO> lstMap = new List<PermissionTemplateDetailDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (PermissionTemplateID > 0)
                {
                    lstMap = (from mm in context.ModuleMasters.Where(x => !(x.IsDeleted ?? false))
                              join tpd in context.PermissionTemplateDetails on new { modid = mm.ID, tmplid = PermissionTemplateID } equals new { modid = tpd.ModuleID, tmplid = tpd.PermissionTemplateID } into mm_tpd_join
                              from mm_tpd in mm_tpd_join.DefaultIfEmpty()
                              select new PermissionTemplateDetailDTO
                              {
                                  DisplayOrderNumber = mm_tpd != null ? mm_tpd.ModuleMaster.DisplayOrderNumber : mm.DisplayOrderNumber,
                                  EnterpriseID = mm_tpd != null ? mm_tpd.EnterpriseID : 0,
                                  GroupId = mm_tpd != null ? mm_tpd.ModuleMaster.GroupID : mm.GroupID,
                                  ID = mm_tpd != null ? mm_tpd.ID : mm.ID,
                                  ImageName = mm_tpd != null ? mm_tpd.ModuleMaster.ImageName : mm.ImageName,
                                  IsChecked = mm_tpd != null ? mm_tpd.IsChecked : false,
                                  IsDelete = mm_tpd != null ? mm_tpd.IsDelete : false,
                                  IsInsert = mm_tpd != null ? mm_tpd.IsInsert : false,
                                  IsModule = mm_tpd != null ? mm_tpd.ModuleMaster.IsModule : mm.IsModule,
                                  IsUpdate = mm_tpd != null ? mm_tpd.IsUpdate : false,
                                  IsView = mm_tpd != null ? mm_tpd.IsView : false,
                                  ModuleID = mm_tpd != null ? mm_tpd.ModuleID : mm.ID,
                                  ModuleName = mm_tpd != null ? mm_tpd.ModuleMaster.ModuleName : mm.ModuleName,
                                  ModuleValue = mm_tpd != null ? mm_tpd.ModuleValue : null,
                                  ParentID = mm_tpd != null ? mm_tpd.ModuleMaster.ParentID : mm.ParentID,
                                  PermissionTemplateID = mm_tpd != null ? mm_tpd.PermissionTemplateID : PermissionTemplateID,
                                  ShowArchived = mm_tpd != null ? mm_tpd.ShowArchived : false,
                                  ShowDeleted = mm_tpd != null ? mm_tpd.ShowDeleted : false,
                                  ShowUDF = mm_tpd != null ? mm_tpd.ShowUDF : false,
                                  resourcekey = mm_tpd != null ? mm_tpd.ModuleMaster.ResourceKey : mm.ResourceKey,
                                  IsModuleDeleted = mm.IsDeleted ?? false,
                              }).ToList();
                }
                else
                {
                    lstMap = (from mm in context.ModuleMasters.Where(x => !(x.IsDeleted ?? false))
                              select new PermissionTemplateDetailDTO
                              {
                                  DisplayOrderNumber = mm.DisplayOrderNumber,
                                  EnterpriseID = 0,
                                  GroupId = mm.GroupID,
                                  ID = 0,
                                  ImageName = mm.ImageName,
                                  IsChecked = false,
                                  IsDelete = false,
                                  IsInsert = false,
                                  IsModule = mm.IsModule,
                                  IsUpdate = false,
                                  IsView = false,
                                  ModuleID = mm.ID,
                                  ModuleName = mm.ModuleName,
                                  ModuleValue = null,
                                  ParentID = mm.ParentID,
                                  PermissionTemplateID = 0,
                                  ShowArchived = false,
                                  ShowDeleted = false,
                                  ShowUDF = false,
                                  resourcekey = mm.ResourceKey
                              }).ToList();
                }
            }
            return lstMap;
        }

        public List<RoleModuleDetailsDTO> GetPermissionsByTemplateForRole(Int64 PermissionTemplateID, Int64 RoleID, Int64 RoomID, Int64 CompanyID, Int64 EnterpriseID)
        {
            List<RoleModuleDetailsDTO> lstMap = new List<RoleModuleDetailsDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (PermissionTemplateID > 0)
                {
                    lstMap = (from tpd in context.PermissionTemplateDetails
                              where tpd.PermissionTemplateID == PermissionTemplateID
                              select new RoleModuleDetailsDTO
                              {
                                  DisplayOrderNumber = tpd.ModuleMaster.DisplayOrderNumber,
                                  GroupId = tpd.ModuleMaster.GroupID,
                                  ID = tpd.ID,
                                  IsChecked = tpd.IsChecked,
                                  IsDelete = tpd.IsDelete,
                                  IsInsert = tpd.IsInsert,
                                  IsModule = tpd.ModuleMaster.IsModule,
                                  IsUpdate = tpd.IsUpdate,
                                  IsView = tpd.IsView,
                                  ModuleID = tpd.ModuleID,
                                  ModuleName = tpd.ModuleMaster.ModuleName,
                                  ModuleValue = tpd.ModuleValue,
                                  ShowArchived = tpd.ShowArchived,
                                  ShowDeleted = tpd.ShowDeleted,
                                  ShowUDF = tpd.ShowUDF,
                                  resourcekey = tpd.ModuleMaster.ResourceKey,
                                  RoleID = RoleID,
                                  RoomId = RoomID,
                                  CompanyID = CompanyID,
                                  EnterpriseID = EnterpriseID,
                                  ToolTipResourceKey = tpd.ModuleMaster.ToolTipResourceKey

                              }).ToList();
                }
                else
                {
                    lstMap = (from mm in context.ModuleMasters
                              select new RoleModuleDetailsDTO
                              {
                                  DisplayOrderNumber = mm.DisplayOrderNumber,
                                  GroupId = mm.GroupID,
                                  ID = 0,
                                  IsChecked = false,
                                  IsDelete = false,
                                  IsInsert = false,
                                  IsModule = mm.IsModule,
                                  IsUpdate = false,
                                  IsView = false,
                                  ModuleID = mm.ID,
                                  ModuleName = mm.ModuleName,
                                  ModuleValue = null,
                                  ShowArchived = false,
                                  ShowDeleted = false,
                                  ShowUDF = false,
                                  resourcekey = mm.ResourceKey,
                                  RoleID = RoleID,
                                  RoomId = RoomID,
                                  CompanyID = CompanyID,
                                  EnterpriseID = EnterpriseID,
                                  ToolTipResourceKey = mm.ToolTipResourceKey
                              }).ToList();
                }
            }
            return lstMap;
        }

        public List<UserRoleModuleDetailsDTO> GetPermissionsByTemplateForUser(Int64 PermissionTemplateID, Int64 RoleID, Int64 UserID, Int64 RoomID, Int64 CompanyID, Int64 EnterpriseID)
        {
            List<UserRoleModuleDetailsDTO> lstMap = new List<UserRoleModuleDetailsDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (PermissionTemplateID > 0)
                {
                    lstMap = (from tpd in context.PermissionTemplateDetails
                              where tpd.PermissionTemplateID == PermissionTemplateID
                              select new UserRoleModuleDetailsDTO
                              {
                                  DisplayOrderNumber = tpd.ModuleMaster.DisplayOrderNumber,
                                  GroupId = tpd.ModuleMaster.GroupID,
                                  ID = tpd.ID,
                                  IsChecked = tpd.IsChecked,
                                  IsDelete = tpd.IsDelete,
                                  IsInsert = tpd.IsInsert,
                                  IsModule = tpd.ModuleMaster.IsModule,
                                  IsUpdate = tpd.IsUpdate,
                                  IsView = tpd.IsView,
                                  ModuleID = tpd.ModuleID,
                                  ModuleName = tpd.ModuleMaster.ModuleName,
                                  ModuleValue = tpd.ModuleValue,
                                  ShowArchived = tpd.ShowArchived,
                                  ShowDeleted = tpd.ShowDeleted,
                                  ShowUDF = tpd.ShowUDF,
                                  resourcekey = tpd.ModuleMaster.ResourceKey,
                                  RoleID = RoleID,
                                  RoomId = RoomID,
                                  CompanyId = CompanyID,
                                  EnteriseId = EnterpriseID,
                                  UserID = UserID,
                                  ToolTipResourceKey = tpd.ModuleMaster.ToolTipResourceKey

                              }).ToList();
                }
                else
                {
                    lstMap = (from mm in context.ModuleMasters
                              select new UserRoleModuleDetailsDTO
                              {
                                  DisplayOrderNumber = mm.DisplayOrderNumber,
                                  GroupId = mm.GroupID,
                                  ID = 0,
                                  IsChecked = false,
                                  IsDelete = false,
                                  IsInsert = false,
                                  IsModule = mm.IsModule,
                                  IsUpdate = false,
                                  IsView = false,
                                  ModuleID = mm.ID,
                                  ModuleName = mm.ModuleName,
                                  ModuleValue = null,
                                  ShowArchived = false,
                                  ShowDeleted = false,
                                  ShowUDF = false,
                                  resourcekey = mm.ResourceKey,
                                  RoleID = RoleID,
                                  RoomId = RoomID,
                                  CompanyId = CompanyID,
                                  EnteriseId = EnterpriseID,
                                  UserID = UserID,
                                  ToolTipResourceKey = mm.ToolTipResourceKey
                              }).ToList();
                }
            }
            return lstMap;
        }

        public List<UserRoleModuleDetailsDTO> GetPermissionsByTemplateInactiveRoom()
        {
            List<UserRoleModuleDetailsDTO> lstMap = new List<UserRoleModuleDetailsDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                context.Database.ExecuteSqlCommand("EXEC InActiveRoomPermissionTemplate");

                lstMap = (from tpd in context.PermissionTemplateDetails
                          where tpd.PermissionTemplateMaster.TemplateName == "[[|InActiveRoomPermissionSet|]]"
                          select new UserRoleModuleDetailsDTO
                          {
                              DisplayOrderNumber = tpd.ModuleMaster.DisplayOrderNumber,
                              GroupId = tpd.ModuleMaster.GroupID,
                              ID = tpd.ID,
                              IsChecked = tpd.IsChecked,
                              IsDelete = tpd.IsDelete,
                              IsInsert = tpd.IsInsert,
                              IsModule = tpd.ModuleMaster.IsModule,
                              IsUpdate = tpd.IsUpdate,
                              IsView = tpd.IsView,
                              ModuleID = tpd.ModuleID,
                              ModuleName = tpd.ModuleMaster.ModuleName,
                              ModuleValue = tpd.ModuleValue,
                              ShowArchived = tpd.ShowArchived,
                              ShowDeleted = tpd.ShowDeleted,
                              ShowUDF = tpd.ShowUDF,
                              resourcekey = tpd.ModuleMaster.ResourceKey,
                              ToolTipResourceKey = tpd.ModuleMaster.ToolTipResourceKey
                          }).ToList();

            }
            return lstMap;
        }
        public bool CheckPermissionTemplateDuplicate(string TemplateName, long EnterpriseID, long Id)
        {
            bool result = false;
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {

                    var qry = (from em in context.PermissionTemplateMasters
                               where em.TemplateName == TemplateName && em.ID != Id
                                && em.IsDeleted == false && em.EnterpriseID == EnterpriseID
                               select em);
                    if (qry.Any())
                    {
                        result = false;
                    }
                    else
                    {
                        result = true;
                    }
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public IEnumerable<PermissionTemplateDTO> GetPagedPermissionTemplateChangeLog(long TemplateId, int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string sortColumnName)
        {
            TotalCount = 0;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@Id", TemplateId),
                                                    new SqlParameter("@StartRowIndex", StartRowIndex),
                                                    new SqlParameter("@MaxRows", MaxRows),
                                                    new SqlParameter("@SearchTerm", SearchTerm ?? string.Empty),
                                                    new SqlParameter("@sortColumnName", sortColumnName ?? string.Empty)
                                                  };
                var PermissionTemplateHistory = context.Database.SqlQuery<PermissionTemplateDTO>("exec [GetPermissionTemplateChangeLog] @Id,@StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName", params1).ToList();

                if (PermissionTemplateHistory != null && PermissionTemplateHistory.Any() && PermissionTemplateHistory.Count() > 0)
                {
                    TotalCount = PermissionTemplateHistory.ElementAt(0).TotalRecords;
                }

                return PermissionTemplateHistory;
            }
        }


        #endregion
    }
}
