using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public partial class ModuleMasterDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        public ModuleMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public ModuleMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion  
        #region [Class Methods]
        public List<ModuleMasterDTO> GetAllModuleNormal(string SortModuleBy)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@SortModuleBy", SortModuleBy) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ModuleMasterDTO>("exec [GetAllModuleNormal] @SortModuleBy", params1).ToList();
            }
        }
        public ModuleMasterDTO GetModuleByNameNormal(string ModuleName)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ModuleName", ModuleName) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ModuleMasterDTO>("exec [GetModuleByNameNormal] @ModuleName", params1).FirstOrDefault();
            }
        }
        public List<ParentModuleMasterDTO> GetParentModulePlain()
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ParentModuleMasterDTO>("exec [GetParentModulePlain] ").ToList();
            }
        }
        public List<ModuleMasterDTO> GetPagedModuleMaster(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            string CreatedByName = "";
            string UpdatedByName = "";
            string CreatedDateFrom = "";
            string CreatedDateTo = "";
            string UpdatedDateFrom = "";
            string UpdatedDateTo = "";
            if ((SearchTerm ?? string.Empty).Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
                if (Fields.Length > 2)
                {
                    if (!string.IsNullOrEmpty(Fields[2]))
                        SearchTerm = Fields[2];
                    else
                        SearchTerm = string.Empty;
                }
                else
                {
                    SearchTerm = string.Empty;
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    CreatedByName = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    UpdatedByName = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
            }
            else
            {
                SearchTerm = "";
            }

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                    new SqlParameter("@StartRowIndex", StartRowIndex),
                    new SqlParameter("@MaxRows", MaxRows),
                    new SqlParameter("@SearchTerm", SearchTerm),
                    new SqlParameter("@sortColumnName", sortColumnName),
                    new SqlParameter("@CreatedFrom", CreatedDateFrom),
                    new SqlParameter("@CreatedTo", CreatedDateTo),
                    new SqlParameter("@CreatedBy", CreatedByName),
                    new SqlParameter("@UpdatedFrom", UpdatedDateFrom),
                    new SqlParameter("@UpdatedTo", UpdatedDateTo),
                    new SqlParameter("@LastUpdatedBy", UpdatedByName),
                };

                List<ModuleMasterDTO> lstcats = context.Database.SqlQuery<ModuleMasterDTO>("exec [GetPagedModuleMaster] @StartRowIndex ,@MaxRows ,@SearchTerm ,@sortColumnName,@CreatedFrom ,@CreatedTo ,@CreatedBy ,@UpdatedFrom ,@UpdatedTo  ,@LastUpdatedBy", params1).ToList();
                TotalCount = 0;

                if (lstcats != null && lstcats.Count > 0)
                {
                    TotalCount = lstcats.First().TotalRecords ?? 0;
                }

                return lstcats;
            }
        }
        public ModuleMasterDTO GetModuleByIDNormal(Int64 ID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ModuleMasterDTO>("exec [GetModuleByIDNormal] @ID", params1).FirstOrDefault();
            }
        }
        public Int64 Insert(ModuleMasterDTO objDTO)
        {
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.Created = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ModuleMaster obj = new ModuleMaster();
                obj.ID = 0;
                obj.ModuleName = objDTO.ModuleName;
                obj.DisplayName = objDTO.DisplayName;
                obj.ParentID = objDTO.ParentID;
                obj.Value = objDTO.Value;
                obj.Priority = objDTO.Priority;
                obj.IsModule = objDTO.IsModule;
                obj.GroupID = objDTO.GroupId;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.Updated = objDTO.Updated;
                obj.Created = objDTO.Created;
                obj.IsDeleted = false;
                obj.GUID = Guid.NewGuid();

                context.ModuleMasters.Add(obj);
                context.SaveChanges();

                return obj.ID;
            }
        }

        public bool Edit(ModuleMasterDTO objDTO)
        {
            objDTO.Updated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ModuleMaster obj = new ModuleMaster();
                obj.ID = objDTO.ID;
                obj.ModuleName = objDTO.ModuleName;
                obj.DisplayName = objDTO.DisplayName;
                obj.ParentID = objDTO.ParentID;
                obj.Value = objDTO.Value;
                obj.Priority = objDTO.Priority;
                obj.IsModule = objDTO.IsModule;
                obj.GroupID = objDTO.GroupId;

                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.Updated = objDTO.Updated;
                obj.Created = objDTO.Created;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.GUID = objDTO.GUID;
                obj.IsDeleted = false;

                context.ModuleMasters.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                return true;
            }
        }
        public bool DeleteModulesByID(string IDs, Int64 userid)
        {
            if (!string.IsNullOrWhiteSpace(IDs))
            {
                using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    string vIDs = (IDs ?? string.Empty).TrimEnd(',');
                    if (!string.IsNullOrWhiteSpace(vIDs))
                    {
                        var params1 = new SqlParameter[] {
                          new SqlParameter("@UserID", userid)
                         ,new SqlParameter("@IDs", vIDs)
                         };

                        string strQuery = @"EXEC DeleteModulesByID @UserID,@IDs";
                        context.Database.ExecuteSqlCommand(strQuery, params1);

                        return true;
                    }
                    return false;
                }
            }
            return false;
        }
        public void UpdateModuleByID(int id, string value, string columnName)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", id)
                 , new SqlParameter("@ColumnName", columnName)
                 , new SqlParameter("@Value", value)
            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("exec [UpdateModuleByID] @ID,@ColumnName,@Value", params1);
            }

        }
        public string GetModuleNotificationByRoomPlain(Int32 ModuleId, Int64 RoomId)
        {
            ModuleNotificationDTO MNDTO = new ModuleNotificationDTO();
            var params1 = new SqlParameter[] { new SqlParameter("@ModuleId", ModuleId)
                                              , new SqlParameter("@RoomId", RoomId) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                MNDTO = context.Database.SqlQuery<ModuleNotificationDTO>("exec [GetModuleNotificationByRoomPlain] @ModuleId,@RoomId", params1).FirstOrDefault();
            }
            return MNDTO.Notification.ToString();
        }
        #endregion
    }
}
