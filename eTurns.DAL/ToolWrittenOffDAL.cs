using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;


namespace eTurns.DAL
{
    public class ToolWrittenOffDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        public ToolWrittenOffDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public ToolWrittenOffDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion
        public Int64 Insert(ToolWrittenOffDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;

            objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
            objDTO.Created = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolWrittenOff obj = new ToolWrittenOff();
                obj.ID = 0;
                obj.ToolGUID = objDTO.ToolGUID;
                obj.Quantity = objDTO.Quantity;
                obj.ToolWrittenOffCategoryID = objDTO.ToolWrittenOffCategoryID;
                obj.Details = objDTO.Details;
                obj.Created = objDTO.Created;
                obj.LastUpdated = objDTO.LastUpdated;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.IsDeleted = objDTO.IsDeleted;
                obj.IsArchived = objDTO.IsArchived;
                obj.CompanyID = objDTO.CompanyID;
                objDTO.GUID = Guid.NewGuid();
                obj.GUID = objDTO.GUID;

                obj.ReceivedOn = objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                obj.AddedFrom = objDTO.AddedFrom = "Web";
                obj.EditedFrom = objDTO.EditedFrom = "Web";

                context.ToolWrittenOffs.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;
                objDTO.GUID = obj.GUID;

                return obj.ID;
            }

        }

        public Int64 InsertForNewTool(ToolWrittenOffDTO objDTO, List<string> Serials)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;

            objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
            objDTO.Created = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                long Id = 0;
                foreach (var serial in Serials)
                {
                    ToolWrittenOff obj = new ToolWrittenOff();
                    obj.ID = 0;
                    obj.ToolGUID = objDTO.ToolGUID;
                    obj.Quantity = 1;
                    obj.ToolWrittenOffCategoryID = objDTO.ToolWrittenOffCategoryID;
                    obj.Details = objDTO.Details;
                    obj.Created = objDTO.Created;
                    obj.LastUpdated = objDTO.LastUpdated;
                    obj.CreatedBy = objDTO.CreatedBy;
                    obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                    obj.Room = objDTO.Room;
                    obj.IsDeleted = objDTO.IsDeleted;
                    obj.IsArchived = objDTO.IsArchived;
                    obj.CompanyID = objDTO.CompanyID;
                    objDTO.GUID = Guid.NewGuid();
                    obj.GUID = objDTO.GUID;
                    obj.ReceivedOn = objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    obj.ReceivedOnWeb = objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    obj.AddedFrom = objDTO.AddedFrom = "Web";
                    obj.EditedFrom = objDTO.EditedFrom = "Web";
                    obj.SerialNumber = serial;
                    context.ToolWrittenOffs.Add(obj);
                    context.SaveChanges();
                    objDTO.ID = obj.ID;
                    objDTO.GUID = obj.GUID;
                    Id = obj.ID;
                }
                return Id;
            }

        }

        public bool DeleteWrittenOffToolRecordsByToolGuids(string ToolGuids, long UserId, long CompanyID, long RoomId)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<Guid> toolGuidList = ToolGuids.TrimEnd(',').Split(',').Select(Guid.Parse).ToList();
                var writtenOffTools = context.ToolWrittenOffs.Where(f => toolGuidList.Contains(f.ToolGUID ?? Guid.Empty) && f.CompanyID == CompanyID && f.Room == RoomId).ToList();

                writtenOffTools.ForEach(a =>
                {
                    a.IsDeleted = true;
                    a.LastUpdated = DateTimeUtility.DateTimeNow;
                    a.LastUpdatedBy = UserId;
                    a.EditedFrom = "Web";
                });

                context.SaveChanges();
                return true;
            }
        }

        public bool UnWrittenOffToolsByToolGuids(string ToolGuids, long UserId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var sqlParams = new SqlParameter[] {
                    new SqlParameter("@ToolGuids", ToolGuids),
                    new SqlParameter("@UserId", UserId)
                };

                context.Database.ExecuteSqlCommand("exec [UnWrittenOffToolsByToolGuids] @ToolGuids,@UserId", sqlParams);
                return true;
            }
        }

        /// <summary>
        /// This method is used to delete written off tool records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="UserId"></param>
        /// <param name="CompanyID"></param>
        /// <returns></returns>
        public bool DeleteWrittenOffToolRecords(string IDs, long UserId, long CompanyID, long RoomId)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<long> idList = IDs.TrimEnd(',').Split(',').Select(long.Parse).ToList();
                var writtenOffTools = context.ToolWrittenOffs.Where(f => idList.Contains(f.ID) && f.CompanyID == CompanyID && f.Room == RoomId).ToList();
                writtenOffTools.ForEach(a =>
                {
                    a.IsDeleted = true;
                    a.LastUpdated = DateTimeUtility.DateTimeNow;
                    a.LastUpdatedBy = UserId;
                    a.EditedFrom = "Web";
                });

                context.SaveChanges();
                return true;
            }
        }

        public IEnumerable<ToolWrittenOffDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            string CreatedByName = "";
            string UpdatedByName = "";
            string CreatedDateFrom = "";
            string CreatedDateTo = "";
            string UpdatedDateFrom = "";
            string UpdatedDateTo = "";
            string WrittenOffCategory = "";

            if ((SearchTerm ?? string.Empty).Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');

                if (Fields.Length > 2)
                {
                    if (Fields[2] != null)
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
                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    string[] arrWrittenOffCategory = FieldsPara[4].Split(',');
                    foreach (string supitem in arrWrittenOffCategory)
                    {
                        WrittenOffCategory = WrittenOffCategory + supitem + "','";
                    }
                    WrittenOffCategory = WrittenOffCategory.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
            }

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var sqlParams = new SqlParameter[] {
                    new SqlParameter("@StartRowIndex", StartRowIndex),
                    new SqlParameter("@MaxRows", MaxRows),
                    new SqlParameter("@SearchTerm", SearchTerm ?? (object)DBNull.Value),
                    new SqlParameter("@sortColumnName", sortColumnName ?? (object)DBNull.Value),
                    new SqlParameter("@CreatedFrom", CreatedDateFrom),
                    new SqlParameter("@CreatedTo", CreatedDateTo),
                    new SqlParameter("@LastUpdatedFrom", UpdatedDateFrom),
                    new SqlParameter("@LastUpdatedTo", UpdatedDateTo),
                    new SqlParameter("@CreatedBy", CreatedByName),
                    new SqlParameter("@LastUpdatedBy", UpdatedByName),
                    new SqlParameter("@IsDeleted", IsDeleted),
                    new SqlParameter("@IsArchived", IsArchived),
                    new SqlParameter("@Room", RoomID),
                    new SqlParameter("@CompanyID", CompanyId),
                    new SqlParameter("@WrittenOffCategory", WrittenOffCategory),
                };

                List<ToolWrittenOffDTO> lstLocations = context.Database.SqlQuery<ToolWrittenOffDTO>("exec [GetPagedToolWrittenOff] @StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@CreatedFrom,@CreatedTo,@LastUpdatedFrom,@LastUpdatedTo,@CreatedBy,@LastUpdatedBy,@IsDeleted,@IsArchived,@Room,@CompanyID,@WrittenOffCategory", sqlParams).ToList();
                TotalCount = 0;
                if (lstLocations != null && lstLocations.Count > 0)
                {
                    TotalCount = lstLocations.First().TotalRecords ?? 0;
                }

                return lstLocations;
            }

        }

        public IEnumerable<ToolWrittenOffDTO> GetWrittenOffToolDetail(Guid ToolGUID, Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            string CreatedByName = "";
            string UpdatedByName = "";
            string CreatedDateFrom = "";
            string CreatedDateTo = "";
            string UpdatedDateFrom = "";
            string UpdatedDateTo = "";
            string WrittenOffCategory = "";

            if ((SearchTerm ?? string.Empty).Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');

                if (Fields.Length > 2)
                {
                    if (Fields[2] != null)
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
                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    string[] arrWrittenOffCategory = FieldsPara[4].Split(',');
                    foreach (string supitem in arrWrittenOffCategory)
                    {
                        WrittenOffCategory = WrittenOffCategory + supitem + "','";
                    }
                    WrittenOffCategory = WrittenOffCategory.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
            }

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var sqlParams = new SqlParameter[] {
                    new SqlParameter("@ToolGUID", ToolGUID),
                    new SqlParameter("@StartRowIndex", StartRowIndex),
                    new SqlParameter("@MaxRows", MaxRows),
                    new SqlParameter("@SearchTerm", SearchTerm ?? (object)DBNull.Value),
                    new SqlParameter("@sortColumnName", sortColumnName ?? (object)DBNull.Value),
                    new SqlParameter("@CreatedFrom", CreatedDateFrom),
                    new SqlParameter("@CreatedTo", CreatedDateTo),
                    new SqlParameter("@LastUpdatedFrom", UpdatedDateFrom),
                    new SqlParameter("@LastUpdatedTo", UpdatedDateTo),
                    new SqlParameter("@CreatedBy", CreatedByName),
                    new SqlParameter("@LastUpdatedBy", UpdatedByName),
                    new SqlParameter("@IsDeleted", IsDeleted),
                    new SqlParameter("@IsArchived", IsArchived),
                    new SqlParameter("@Room", RoomID),
                    new SqlParameter("@CompanyID", CompanyId),
                    new SqlParameter("@WrittenOffCategory", WrittenOffCategory),
                };

                List<ToolWrittenOffDTO> lstLocations = context.Database.SqlQuery<ToolWrittenOffDTO>("exec [GetWrittenOffToolDetail] @ToolGUID,@StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@CreatedFrom,@CreatedTo,@LastUpdatedFrom,@LastUpdatedTo,@CreatedBy,@LastUpdatedBy,@IsDeleted,@IsArchived,@Room,@CompanyID,@WrittenOffCategory", sqlParams).ToList();
                TotalCount = 0;
                if (lstLocations != null && lstLocations.Count > 0)
                {
                    TotalCount = lstLocations.First().TotalRecords ?? 0;
                }

                return lstLocations;
            }

        }

        public List<NarrowSearchDTO> GetWrittenOffToolListNarrowSearch(long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string NarrowSearchKey)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), 
                                               new SqlParameter("@CompanyID", CompanyID), 
                                               new SqlParameter("@IsArchived", IsArchived), 
                                               new SqlParameter("@IsDeleted", IsDeleted), 
                                               new SqlParameter("@NarrowSearchKey", NarrowSearchKey ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<NarrowSearchDTO>("exec [GetWrittenOffToolListNarrowSearch] @RoomID,@CompanyID,@IsArchived,@IsDeleted,@NarrowSearchKey", params1).ToList();
            }
        }
    }
}
