using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eTurns.DTO;
using System.Data;
using Dynamite;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Collections;
using eTurns.DTO.Resources;
using System.Web;
using System.Data.SqlClient;

namespace eTurns.DAL
{
    public partial class ToolsSchedulerDAL : eTurnsBaseDAL
    {
        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ToolsSchedulerDTO> GetCachedData(Nullable<Guid> AssetGUID, Nullable<Guid> ToolGUID, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            string sSQL = "";

            //if (AssetGUID != null)
            //{
            //    sSQL += " A.AssetGUID = '" + AssetGUID.Value + "'";
            //}
            //else if (ToolGUID != null)
            //{
            //    sSQL += " A.ToolGUID = '" + ToolGUID.Value + "'";
            //}
            if (sSQL != "")
                sSQL += " AND ";
            if (IsArchived == false && IsDeleted == false)
            {
                sSQL += " A.IsDeleted != 1 AND A.IsArchived != 1 ";
            }
            else if (IsArchived && IsDeleted)
            {
                sSQL += " A.IsDeleted = 1 AND A.IsArchived = 1 ";
            }
            else if (IsArchived)
            {
                sSQL += " A.IsArchived = 1 ";
            }
            else if (IsDeleted)
            {
                sSQL += " A.IsDeleted =1 ";
            }

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<ToolsSchedulerDTO>(
                                        @"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName 
                                        FROM ToolsScheduler A 
                                        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                                        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                                        LEFT OUTER JOIN Room D on A.Room = D.ID 
                                        WHERE A.Room = " + RoomID.ToString() + " AND A.CompanyID = " + CompanyID.ToString() + " AND " + sSQL)
                        select new ToolsSchedulerDTO
                        {
                            ID = u.ID,
                            SchedulerName = u.SchedulerName,
                            SchedulerType = u.SchedulerType,
                            OperationalHours = u.OperationalHours,
                            Mileage = u.Mileage,
                            Created = u.Created,
                            CreatedBy = u.CreatedBy,
                            Updated = u.Updated,
                            LastUpdatedBy = u.LastUpdatedBy,
                            Room = u.Room,
                            IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                            IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                            GUID = u.GUID,
                            CompanyID = u.CompanyID,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            RoomName = u.RoomName,
                            MainScheduleType = u.SchedulerType,
                            ScheduleFor = u.ScheduleFor,
                            ScheduleForName = u.ScheduleFor == 1 ? "Asset" : "Tool",
                            Description = u.Description,
                            TimeBasedFrequency = u.TimeBasedFrequency,
                            TimeBaseUnit = u.TimeBaseUnit,
                            RecurringDays = u.RecurringDays,
                            CheckOuts = u.CheckOuts,
                            ScheduleParams = u.SchedulerType == 1 ? (context.RoomSchedules.Where(t => t.AssetToolID == u.GUID).Select(ss => new SchedulerDTO()
                            {
                                CompanyId = ss.CompanyId ?? 0,
                                CreatedBy = ss.CreatedBy ?? 0,
                                DailyRecurringDays = ss.DailyRecurringDays,
                                DailyRecurringType = ss.DailyRecurringType,
                                ScheduleID = ss.ScheduleID,
                                IsScheduleActive = ss.IsScheduleActive,
                                LastUpdatedBy = ss.LastUpdatedBy ?? 0,
                                LoadSheduleFor = ss.ScheduleFor,
                                MonthlyDateOfMonth = ss.MonthlyDateOfMonth,
                                MonthlyDayOfMonth = ss.MonthlyDayOfMonth,
                                MonthlyRecurringMonths = ss.MonthlyRecurringMonths,
                                MonthlyRecurringType = ss.MonthlyRecurringType,
                                RoomId = ss.RoomId ?? 0,
                                RoomName = string.Empty,
                                ScheduleMode = ss.ScheduleMode,
                                ScheduleRunDateTime = ss.ScheduleRunTime,
                                SubmissionMethod = ss.SubmissionMethod,
                                SupplierId = ss.SupplierId ?? 0,
                                SupplierName = string.Empty,
                                WeeklyOnFriday = ss.WeeklyOnFriday,
                                WeeklyOnMonday = ss.WeeklyOnMonday,
                                WeeklyOnSaturday = ss.WeeklyOnSaturday,
                                WeeklyOnSunday = ss.WeeklyOnSunday,
                                WeeklyOnThursday = ss.WeeklyOnThursday,
                                WeeklyOnTuesday = ss.WeeklyOnTuesday,
                                WeeklyOnWednesday = ss.WeeklyOnWednesday,
                                WeeklyRecurringWeeks = ss.WeeklyRecurringWeeks,
                                WeeklySelectedDays = string.Empty,
                                AssetToolID = ss.AssetToolID,
                                HourlyAtWhatMinute = ss.HourlyAtWhatMinute ?? 0,
                                HourlyRecurringHours = ss.HourlyRecurringHours ?? 0,
                                NextRunDate = ss.NextRunDate,
                                ScheduleName = ss.ScheduleName
                            }).FirstOrDefault()) : null,
                            TimeBaseUnitName = (u.TimeBasedFrequency > 0 ? (Convert.ToString(u.TimeBasedFrequency) + " " + Convert.ToString((TimebasedScheduleFreq)u.TimeBaseUnit)) : "")
                        }).AsParallel().ToList();
            }
        }

        /// <summary>
        /// Get Paged Records from the ToolsScheduler Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<ToolsSchedulerDTO> GetAllRecords(Nullable<Guid> AssetGUID, Nullable<Guid> ToolGUID, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        {
            return GetCachedData(AssetGUID, ToolGUID, RoomID, CompanyId, IsArchived, IsDeleted).OrderBy("ID DESC");
        }

        /// <summary>
        /// Get Paged Records from the AssetMaster Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<ToolsSchedulerDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            // Get Cached-Media   
            IEnumerable<ToolsSchedulerDTO> ObjCache = GetCachedData(null, null, RoomID, CompanyID, IsArchived, IsDeleted);
            IEnumerable<ToolsSchedulerDTO> ObjGlobalCache = ObjCache;
            ObjCache = ObjCache.Where(t => (t.IsArchived == false && t.IsDeleted == false));
            TotalCount = 0;
            if (IsArchived && IsDeleted)
            {
                ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsArchived == IsArchived || t.IsDeleted == IsDeleted));
                ObjCache = ObjCache.Concat(ObjGlobalCache);
            }
            else if (IsArchived)
            {
                ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsArchived == IsArchived));
                ObjCache = ObjCache.Concat(ObjGlobalCache);
            }
            else if (IsDeleted)
            {
                ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsDeleted == IsDeleted));
                ObjCache = ObjCache.Concat(ObjGlobalCache);
            }
            if (String.IsNullOrEmpty(SearchTerm))
            {
                TotalCount = ObjCache.Count();
                return ObjCache = ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            // return ObjCache;  
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media
                //IEnumerable<AssetMasterDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

                string newSearchValue = string.Empty;
                //WI-1461 related changes 
                if (Fields.Length > 2)
                {
                    if (!string.IsNullOrEmpty(Fields[2]))
                        newSearchValue = Fields[2];
                    else
                        newSearchValue = string.Empty;
                }
                else
                {
                    newSearchValue = string.Empty;
                }

                ObjCache = ObjCache.Where(t =>
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedBy.ToString())))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.LastUpdatedBy.ToString())))
                    && ((Fields[1].Split('@')[2] == "") || (t.Created >= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone) && t.Created <= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone)))
                    && ((Fields[1].Split('@')[3] == "") || (t.Updated >= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone) && t.Updated <= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone)))
                    && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').ToList().Contains(t.UDF1)))
                    && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').ToList().Contains(t.UDF2)))
                    && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').ToList().Contains(t.UDF3)))
                    && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').ToList().Contains(t.UDF4)))
                    && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').ToList().Contains(t.UDF5)))
                    && ((Fields[1].Split('@')[45] == "") || (Fields[1].Split('@')[45].Split(',').ToList().Contains(t.ScheduleFor.ToString())))
                    && ((Fields[1].Split('@')[46] == "") || (Convert.ToInt32(Fields[1].Split('@')[46].Split(',')[0]) == 1 ? t.SchedulerType.ToString().ToLower() == "1" : true))
                    && ((Fields[1].Split('@')[46] == "") || (Convert.ToInt32(Fields[1].Split('@')[46].Split(',')[0]) == 2 ? t.SchedulerType.ToString().ToLower() == "2" : true))
                    && ((Fields[1].Split('@')[46] == "") || (Convert.ToInt32(Fields[1].Split('@')[46].Split(',')[0]) == 3 ? t.SchedulerType.ToString().ToLower() == "3" : true))
                    // && ((Fields[1].Split('@')[29] == "") || (Fields[1].Split('@')[29].Split(',').ToList().Contains(t.ToolCategoryID.ToString())))
                    );
                ObjCache = ObjCache.Where
                     (t => t.ID.ToString().Contains(newSearchValue) ||
                         (t.RoomName ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.SchedulerName ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.CreatedByName ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.UpdatedByName ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.ScheduleForName ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.Mileage == null ? "" : t.Mileage.ToString()).IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.OperationalHours == null ? "" : t.OperationalHours.ToString()).IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                          (t.Created).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.Updated).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         //(t.ScheduleType ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.UDF1 ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.UDF2 ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.UDF3 ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.UDF4 ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.UDF5 ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0
                         ||
                         (t.Description ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0
                         ||
                         (t.ScheduleForName ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0
                          ||
                         (t.ScheduleTypeName ?? "").IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0
                               ||
                         (t.OperationalHours).ToString().IndexOf(newSearchValue, StringComparison.InvariantCultureIgnoreCase) >= 0
                     );
                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                //Get Cached-Media
                //IEnumerable<AssetMasterDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                TotalCount = ObjCache.Where
                    (
                       t => t.ID.ToString().Contains(SearchTerm) ||
                        (t.RoomName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.SchedulerName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.CreatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UpdatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ScheduleForName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Mileage == null ? "" : t.Mileage.ToString()).IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.OperationalHours == null ? "" : t.OperationalHours.ToString()).IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        //(t.ScheduleType ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                         (t.Description ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                         ||
                         (t.ScheduleForName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                          ||
                         (t.ScheduleTypeName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).Count();
                return ObjCache.Where
                    (t => t.ID.ToString().Contains(SearchTerm) ||
                        (t.RoomName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.SchedulerName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.CreatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UpdatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ScheduleForName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Mileage == null ? "" : t.Mileage.ToString()).IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.OperationalHours == null ? "" : t.OperationalHours.ToString()).IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        //(t.ScheduleType ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                           ||
                         (t.Description ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                         ||
                         (t.ScheduleForName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                          ||
                         (t.ScheduleTypeName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                               ||
                         (t.OperationalHours).ToString().IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
        }

        /// <summary>
        /// Get Particullar Record from the ToolsScheduler by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public ToolsSchedulerDTO GetHistoryRecord(Int64 id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<ToolsSchedulerDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM ToolsScheduler_History A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.HistoryID=" + id.ToString())
                        select new ToolsSchedulerDTO
                        {
                            ID = u.ID,
                            SchedulerName = u.SchedulerName,
                            //StartDate = u.StartDate,
                            //EndDate = u.EndDate,
                            SchedulerType = u.SchedulerType,
                            //Days = u.Days,
                            //WeekDays = u.WeekDays,
                            //MonthDays = u.MonthDays,
                            OperationalHours = u.OperationalHours,
                            Mileage = u.Mileage,
                            //ScheduleTime = u.ScheduleTime,
                            //ToolGUID = u.ToolGUID,
                            //ToolName = u.ToolName,
                            //AssetGUID = u.AssetGUID,
                            //AssetName = u.AssetName,
                            Created = u.Created,
                            CreatedBy = u.CreatedBy,
                            Updated = u.Updated,
                            LastUpdatedBy = u.LastUpdatedBy,
                            Room = u.Room,
                            IsArchived = u.IsArchived,
                            IsDeleted = u.IsDeleted,
                            GUID = u.GUID,
                            CompanyID = u.CompanyID,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            ScheduleFor = u.ScheduleFor,
                            TimeBasedFrequency = u.TimeBasedFrequency,
                            TimeBaseUnit = u.TimeBaseUnit,
                            RecurringDays = u.RecurringDays,
                            CheckOuts = u.CheckOuts

                        }).SingleOrDefault();
            }
        }

        /// <summary>
        /// Get Particullar Record from the ToolsScheduler by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public ToolsSchedulerDTO GetRecord(Guid GUID, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            string sSQL = "";
            if (IsArchived == false && IsDeleted == false)
            {
                sSQL += " A.IsDeleted != 1 AND A.IsArchived != 1 ";
            }
            else if (IsArchived && IsDeleted)
            {
                sSQL += " A.IsDeleted = 1 AND A.IsArchived = 1 ";
            }
            else if (IsArchived)
            {
                sSQL += " A.IsArchived = 1 ";
            }
            else if (IsDeleted)
            {
                sSQL += " A.IsDeleted =1 ";
            }

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<ToolsSchedulerDTO>(
                                        @"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName ,ISNULL(R.ScheduleMode,0) as ScheduleMode
                                        FROM ToolsScheduler A 
                                        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                                        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                                        LEFT OUTER JOIN Room D on A.Room = D.ID 
                                        LEFT OUTER JOIN RoomSchedule R On R.AssetToolID=A.GUID
                                        WHERE A.GUID = '" + GUID.ToString() + "' AND A.Room = " + RoomID.ToString() + " AND A.CompanyID = " + CompanyID.ToString() + @" AND " + sSQL)
                        select new ToolsSchedulerDTO
                        {
                            ID = u.ID,
                            SchedulerName = u.SchedulerName,
                            //StartDate = u.StartDate,
                            //EndDate = u.EndDate,
                            SchedulerType = u.SchedulerType,
                            //Days = u.Days,
                            //WeekDays = u.WeekDays,
                            //MonthDays = u.MonthDays,
                            OperationalHours = u.OperationalHours,
                            Mileage = u.Mileage,
                            //ScheduleTime = u.ScheduleTime,
                            //ToolGUID = u.ToolGUID,
                            //ToolName = u.ToolName,
                            //AssetGUID = u.AssetGUID,
                            //AssetName = u.AssetName,
                            Created = u.Created,
                            CreatedBy = u.CreatedBy,
                            Updated = u.Updated,
                            LastUpdatedBy = u.LastUpdatedBy,
                            Room = u.Room,
                            IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                            IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                            GUID = u.GUID,
                            CompanyID = u.CompanyID,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            RoomName = u.RoomName,
                            MainScheduleType = u.SchedulerType,
                            ScheduleFor = u.ScheduleFor,
                            ScheduleMode = u.ScheduleMode,
                            Description = u.Description,
                            TimeBasedFrequency = u.TimeBasedFrequency,
                            TimeBaseUnit = u.TimeBaseUnit,
                            RecurringDays = u.RecurringDays,
                            CheckOuts = u.CheckOuts
                        }).SingleOrDefault();
            }

        }

        public ToolsSchedulerDTO GetRecord(Int64 ID, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            string sSQL = "";
            if (IsArchived == false && IsDeleted == false)
            {
                sSQL += " A.IsDeleted != 1 AND A.IsArchived != 1 ";
            }
            else if (IsArchived && IsDeleted)
            {
                sSQL += " A.IsDeleted = 1 AND A.IsArchived = 1 ";
            }
            else if (IsArchived)
            {
                sSQL += " A.IsArchived = 1 ";
            }
            else if (IsDeleted)
            {
                sSQL += " A.IsDeleted =1 ";
            }

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<ToolsSchedulerDTO>(
                                        @"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName 
                                        FROM ToolsScheduler A 
                                        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                                        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                                        LEFT OUTER JOIN Room D on A.Room = D.ID 
                                        WHERE A.ID = '" + ID.ToString() + "' AND A.Room = " + RoomID.ToString() + " AND A.CompanyID = " + CompanyID.ToString() + @" AND " + sSQL)
                        select new ToolsSchedulerDTO
                        {
                            ID = u.ID,
                            SchedulerName = u.SchedulerName,
                            //StartDate = u.StartDate,
                            //EndDate = u.EndDate,
                            SchedulerType = u.SchedulerType,
                            //Days = u.Days,
                            //WeekDays = u.WeekDays,
                            //MonthDays = u.MonthDays,
                            OperationalHours = u.OperationalHours,
                            Mileage = u.Mileage,
                            //ScheduleTime = u.ScheduleTime,
                            //ToolGUID = u.ToolGUID,
                            //ToolName = u.ToolName,
                            //AssetGUID = u.AssetGUID,
                            //AssetName = u.AssetName,
                            Created = u.Created,
                            CreatedBy = u.CreatedBy,
                            Updated = u.Updated,
                            LastUpdatedBy = u.LastUpdatedBy,
                            Room = u.Room,
                            IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                            IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                            GUID = u.GUID,
                            CompanyID = u.CompanyID,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            RoomName = u.RoomName,
                            MainScheduleType = u.SchedulerType,
                            ScheduleFor = u.ScheduleFor,
                            TimeBasedFrequency = u.TimeBasedFrequency,
                            TimeBaseUnit = u.TimeBaseUnit,
                            RecurringDays = u.RecurringDays,
                            CheckOuts = u.CheckOuts
                        }).SingleOrDefault();
            }

        }

        public ToolsSchedulerDTO GetRecordNew(Guid GUID, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            string sSQL = "";
            if (IsArchived == false && IsDeleted == false)
            {
                sSQL += " A.IsDeleted != 1 AND A.IsArchived != 1 ";
            }
            else if (IsArchived && IsDeleted)
            {
                sSQL += " A.IsDeleted = 1 AND A.IsArchived = 1 ";
            }
            else if (IsArchived)
            {
                sSQL += " A.IsArchived = 1 ";
            }
            else if (IsDeleted)
            {
                sSQL += " A.IsDeleted =1 ";
            }

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<ToolsSchedulerDTO>(
                                        @"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName ,ISNULL(R.ScheduleMode,0) as ScheduleMode
                                        FROM ToolsScheduler A 
                                        LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                                        LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                                        LEFT OUTER JOIN Room D on A.Room = D.ID 
                                        LEFT OUTER JOIN RoomSchedule R On R.AssetToolID=A.GUID
                                        WHERE A.GUID = '" + GUID.ToString() + "' AND A.Room = " + RoomID.ToString() + " AND A.CompanyID = " + CompanyID.ToString() + @" AND " + sSQL)
                        select new ToolsSchedulerDTO
                        {
                            ID = u.ID,
                            SchedulerName = u.SchedulerName,
                            //StartDate = u.StartDate,
                            //EndDate = u.EndDate,
                            SchedulerType = u.SchedulerType,
                            //Days = u.Days,
                            //WeekDays = u.WeekDays,
                            //MonthDays = u.MonthDays,
                            OperationalHours = u.OperationalHours,
                            Mileage = u.Mileage,
                            //ScheduleTime = u.ScheduleTime,
                            //ToolGUID = u.ToolGUID,
                            //ToolName = u.ToolName,
                            //AssetGUID = u.AssetGUID,
                            //AssetName = u.AssetName,
                            Created = u.Created,
                            CreatedBy = u.CreatedBy,
                            Updated = u.Updated,
                            LastUpdatedBy = u.LastUpdatedBy,
                            Room = u.Room,
                            IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                            IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                            GUID = u.GUID,
                            CompanyID = u.CompanyID,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            RoomName = u.RoomName,
                            MainScheduleType = u.SchedulerType,
                            ScheduleFor = u.ScheduleFor,
                            ScheduleMode = u.ScheduleMode,
                            Description = u.Description,
                            TimeBasedFrequency = u.TimeBasedFrequency,
                            TimeBaseUnit = u.TimeBaseUnit,
                            RecurringDays = u.RecurringDays,
                            CheckOuts = u.CheckOuts
                        }).SingleOrDefault();
            }

        }
    }
}
