using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using eTurns.DTO.Resources;
using System.Web;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public partial class ToolsSchedulerDAL : eTurnsBaseDAL
    {

        #region [Class Constructor]

        //public ToolsSchedulerDAL(base.DataBaseName)
        //{

        //}

        public ToolsSchedulerDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public ToolsSchedulerDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        /// <summary>
        /// Get Paged Records from the ToolsScheduler Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<ToolsSchedulerDTO> GetToolsSchedulerByRoomPlain(long RoomID, long CompanyId)
        {
            var params1 = new SqlParameter[] {
                                               new SqlParameter("@RoomID", RoomID),
                                               new SqlParameter("@CompanyID", CompanyId)                                               
                                             };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.Database.SqlQuery<ToolsSchedulerDTO>("exec [GetToolsSchedulerByRoomPlain] @RoomID,@CompanyID", params1)
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
                        TimeBaseUnitName = (u.TimeBasedFrequency > 0 ? (Convert.ToString(u.TimeBasedFrequency) + " " + Convert.ToString((TimebasedScheduleFreq)u.TimeBaseUnit)) : "")
                    }).AsParallel().ToList().OrderBy("ID DESC"); ;
            }
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
        public IEnumerable<ToolsSchedulerDTO> GetPagedToolsScheduler(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string sortColumnName, long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, 
                                                              string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            TotalCount = 0;
            string strQuer = "";
            string UDF1 = null;
            string UDF2 = null;
            string UDF3 = null;
            string UDF4 = null;
            string UDF5 = null;
            string ItemCreaters = null;
            string ItemUpdators = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string ScheduleFor = null;
            string ScheduleType = null;

            StartRowIndex = int.Parse(Math.Ceiling((double)(StartRowIndex / MaxRows)).ToString());

            if (!string.IsNullOrEmpty(SearchTerm) && SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
                DateTime FromdDate = DateTime.Now;
                DateTime ToDate = DateTime.Now;

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
                    ItemCreaters = FieldsPara[0];//.TrimEnd(',');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    ItemUpdators = FieldsPara[1];//.TrimEnd(',');
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
                    string[] arrReplenishTypes = FieldsPara[4].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF1 = UDF1 + supitem + "','";
                    }
                    UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF1 = HttpUtility.UrlDecode(UDF1);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    string[] arrReplenishTypes = FieldsPara[5].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF2 = UDF2 + supitem + "','";
                    }
                    UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF2 = HttpUtility.UrlDecode(UDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    string[] arrReplenishTypes = FieldsPara[6].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF3 = UDF3 + supitem + "','";
                    }
                    UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF3 = HttpUtility.UrlDecode(UDF3);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    string[] arrReplenishTypes = FieldsPara[7].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF4 = UDF4 + supitem + "','";
                    }
                    UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF4 = HttpUtility.UrlDecode(UDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    string[] arrReplenishTypes = FieldsPara[8].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF5 = UDF5 + supitem + "','";
                    }
                    UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF5 = HttpUtility.UrlDecode(UDF5);
                }

                if (!string.IsNullOrEmpty(FieldsPara[45]) && (!string.IsNullOrWhiteSpace(FieldsPara[45])))
                {
                    ScheduleFor = FieldsPara[45];
                }

                if (!string.IsNullOrEmpty(FieldsPara[46]) && (!string.IsNullOrWhiteSpace(FieldsPara[46])))
                {
                    ScheduleType = FieldsPara[46];
                }

            }

            strQuer = @"EXEC [GetPagedToolsScheduler] @CompnayID,@RoomID,@IsDeleted,@IsArchived,@StartRowIndex,@MaxRows,@sortColumnName,@SearchTerm,@ItemCreaters,@ItemUpdators,@CreatedDateFrom,@CreatedDateTo,@UpdatedDateFrom,@UpdatedDateTo,@UDF1,@UDF2,@UDF3,@UDF4,@UDF5,@ScheduleFor,@ScheduleType";

            List<SqlParameter> sqlParas = new List<SqlParameter>();
            sqlParas.Add(new SqlParameter("@CompnayID", CompanyID));
            sqlParas.Add(new SqlParameter("@RoomID", RoomID));
            sqlParas.Add(new SqlParameter("@IsDeleted", IsDeleted));
            sqlParas.Add(new SqlParameter("@IsArchived", IsArchived));
            sqlParas.Add(new SqlParameter("@StartRowIndex", StartRowIndex));
            sqlParas.Add(new SqlParameter("@MaxRows", MaxRows));

            if (!string.IsNullOrEmpty(sortColumnName) && sortColumnName != "null")
                sqlParas.Add(new SqlParameter("@sortColumnName", sortColumnName));
            else
                sqlParas.Add(new SqlParameter("@sortColumnName", DBNull.Value));

            if (!string.IsNullOrEmpty(SearchTerm) && SearchTerm != "null")
                sqlParas.Add(new SqlParameter("@SearchTerm", SearchTerm));
            else
                sqlParas.Add(new SqlParameter("@SearchTerm", DBNull.Value));


            sqlParas.Add(new SqlParameter("@ItemCreaters", ItemCreaters ?? string.Empty));
            sqlParas.Add(new SqlParameter("@ItemUpdators", ItemUpdators ?? string.Empty));
            sqlParas.Add(new SqlParameter("@CreatedDateFrom", CreatedDateFrom ?? string.Empty));
            sqlParas.Add(new SqlParameter("@CreatedDateTo", CreatedDateTo ?? string.Empty));
            sqlParas.Add(new SqlParameter("@UpdatedDateFrom", UpdatedDateFrom ?? string.Empty));
            sqlParas.Add(new SqlParameter("@UpdatedDateTo", UpdatedDateTo ?? string.Empty));
            sqlParas.Add(new SqlParameter("@UDF1", UDF1 ?? string.Empty));
            sqlParas.Add(new SqlParameter("@UDF2", UDF2 ?? string.Empty));
            sqlParas.Add(new SqlParameter("@UDF3", UDF3 ?? string.Empty));
            sqlParas.Add(new SqlParameter("@UDF4", UDF4 ?? string.Empty));
            sqlParas.Add(new SqlParameter("@UDF5", UDF5 ?? string.Empty));
            sqlParas.Add(new SqlParameter("@ScheduleFor", ScheduleFor ?? string.Empty));
            sqlParas.Add(new SqlParameter("@ScheduleType", ScheduleType ?? string.Empty));
            

            var params1 = sqlParas.ToArray();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var toolsSchedulers = (from u in context.Database.SqlQuery<ToolsSchedulerDTO>(strQuer, params1)
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
                                           TimeBaseUnitName = (u.TimeBasedFrequency > 0 ? (Convert.ToString(u.TimeBasedFrequency) + " " + Convert.ToString((TimebasedScheduleFreq)u.TimeBaseUnit)) : ""),
                                           TotalRecords = u.TotalRecords
                                       }).AsParallel().ToList();

                if (toolsSchedulers != null && toolsSchedulers.Any() && toolsSchedulers.Count() > 0)
                {
                    TotalCount = toolsSchedulers.ElementAt(0).TotalRecords; 
                }
                return toolsSchedulers;
            }            
        }

        public ToolsSchedulerDTO GetToolsSchedulerByGuidPlain(Guid GUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@GUID", GUID) };
                return context.Database.SqlQuery<ToolsSchedulerDTO>("exec [GetToolsSchedulerByGuidPlain] @GUID", params1).FirstOrDefault();
            }
        }

        public ToolsSchedulerDTO GetToolsSchedulerByGuidFull(Guid GUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@GUID", GUID) };
                return context.Database.SqlQuery<ToolsSchedulerDTO>("exec [GetToolsSchedulerByGuidFull] @GUID", params1).FirstOrDefault();
            }
        }


        /// <summary>
        /// Insert Record in the DataBase ToolsScheduler
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 Insert(ToolsSchedulerDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolsScheduler obj = new ToolsScheduler();
                obj.ID = 0;
                obj.SchedulerName = objDTO.SchedulerName;
                //  obj.StartDate = objDTO.StartDate;
                //  obj.EndDate = objDTO.EndDate;
                obj.SchedulerType = objDTO.SchedulerType;
                //  obj.Days = objDTO.Days;
                //  obj.WeekDays = objDTO.WeekDays;
                //  obj.MonthDays = objDTO.MonthDays;
                obj.OperationalHours = objDTO.OperationalHours;
                obj.Mileage = objDTO.Mileage;
                //  obj.ScheduleTime = objDTO.ScheduleTime;
                //  obj.ToolGUID = objDTO.ToolGUID;
                //  obj.ToolName = objDTO.ToolName;
                //  obj.AssetGUID = objDTO.AssetGUID;
                //  obj.AssetName = objDTO.AssetName;
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
                //  objDTO.GUID = Guid.NewGuid();
                obj.GUID = objDTO.GUID;
                obj.CompanyID = objDTO.CompanyID;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.ScheduleFor = objDTO.ScheduleFor;
                obj.Description = objDTO.Description;
                obj.TimeBasedFrequency = objDTO.TimeBasedFrequency;
                obj.TimeBaseUnit = objDTO.TimeBaseUnit;
                obj.RecurringDays = objDTO.RecurringDays;
                obj.CheckOuts = objDTO.CheckOuts;
                context.ToolsSchedulers.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;
                return obj.ID;
            }
        }

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool Edit(ToolsSchedulerDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolsScheduler obj = context.ToolsSchedulers.FirstOrDefault(t => t.ID == objDTO.ID);
                if (obj != null)
                {
                    obj.SchedulerName = objDTO.SchedulerName;
                    obj.SchedulerType = objDTO.SchedulerType;
                    obj.OperationalHours = objDTO.OperationalHours;
                    obj.Mileage = objDTO.Mileage;
                    obj.Updated = DateTime.UtcNow;
                    obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                    obj.Room = objDTO.Room;
                    obj.UDF1 = objDTO.UDF1;
                    obj.UDF2 = objDTO.UDF2;
                    obj.UDF3 = objDTO.UDF3;
                    obj.UDF4 = objDTO.UDF4;
                    obj.UDF5 = objDTO.UDF5;
                    obj.ScheduleFor = objDTO.ScheduleFor;
                    obj.Description = objDTO.Description;
                    obj.TimeBasedFrequency = objDTO.TimeBasedFrequency;
                    obj.TimeBaseUnit = objDTO.TimeBaseUnit;
                    obj.RecurringDays = objDTO.RecurringDays;
                    obj.CheckOuts = objDTO.CheckOuts;
                    context.SaveChanges();
                }
                return true;
            }
        }

        /// <summary>
        /// Delete Multiple Records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool DeleteToolsSchedulersByGuids(string GUIDs, long UserId)
        {
            var params1 = new SqlParameter[] { 
                                                new SqlParameter("@GUIDs", GUIDs ?? (object)DBNull.Value), 
                                                new SqlParameter("@UserId", UserId) };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("exec DeleteToolsSchedulersByGuids @GUIDs,@UserID", params1);
                return true;
            }            
        }

        public IEnumerable<ToolsSchedulerDTO> GetAssetToolScheduler(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, int ScheduleFor)
        {
            List<int> ScheduleTypes = new List<int>();
            IEnumerable<ToolsSchedulerDTO> IEToolsSchedulerDTO = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEToolsSchedulerDTO = (from u in context.ToolsSchedulers
                                       where u.ScheduleFor == ScheduleFor && u.Room == RoomID && u.CompanyID == CompanyID && (u.IsDeleted ?? false) == false && (u.IsArchived ?? false) == false
                                       select new ToolsSchedulerDTO
                                       {
                                           SchedulerName = u.SchedulerName,
                                           ID = u.ID,
                                           GUID = u.GUID
                                       }).OrderBy(x=>x.SchedulerName).AsParallel().ToList();
            }
            return IEToolsSchedulerDTO;
        }
        public IEnumerable<ToolsSchedulerDTO> GetAssetToolSchedulerAutoComplete(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, int ScheduleFor)
        {
            List<int> ScheduleTypes = new List<int>();
            IEnumerable<ToolsSchedulerDTO> IEToolsSchedulerDTO = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEToolsSchedulerDTO = (from u in context.ToolsSchedulers
                                       where u.ScheduleFor == ScheduleFor && u.Room == RoomID && u.CompanyID == CompanyID && (u.IsDeleted ?? false) == false && (u.IsArchived ?? false) == false
                                       select new ToolsSchedulerDTO
                                       {
                                           SchedulerName = u.SchedulerName,
                                           ID = u.ID,
                                           GUID = u.GUID
                                       }).AsParallel().ToList();
            }
            return IEToolsSchedulerDTO;
        }

        public ToolsSchedulerDTO GetAssetToolSchedulerByName(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string ScheduleName)
        {
            List<int> ScheduleTypes = new List<int>();
            ToolsSchedulerDTO IEToolsSchedulerDTO = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEToolsSchedulerDTO = (from u in context.ToolsSchedulers
                                       where u.SchedulerName == (ScheduleName) && u.Room == RoomID && u.CompanyID == CompanyID && (u.IsDeleted ?? false) == false && (u.IsArchived ?? false) == false
                                       select new ToolsSchedulerDTO
                                       {
                                           SchedulerName = u.SchedulerName,
                                           ID = u.ID,
                                           GUID = u.GUID,
                                           ScheduleFor = u.ScheduleFor
                                       }).FirstOrDefault();
            }
            return IEToolsSchedulerDTO;
        }

        public IEnumerable<ToolsSchedulerDTO> GetAssetToolSchedulerAutoComplete(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, Int64 ScheduleFor, string SchedulerName)
        {
            List<int> ScheduleTypes = new List<int>();
            IEnumerable<ToolsSchedulerDTO> IEToolsSchedulerDTO = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (!string.IsNullOrWhiteSpace(SchedulerName))
                {
                    IEToolsSchedulerDTO = (from u in context.ToolsSchedulers
                                           where u.SchedulerName.Contains(SchedulerName) && u.ScheduleFor == ScheduleFor && u.Room == RoomID && u.CompanyID == CompanyID && (u.IsDeleted ?? false) == false && (u.IsArchived ?? false) == false
                                           select new ToolsSchedulerDTO
                                           {
                                               SchedulerName = u.SchedulerName,
                                               ID = u.ID,
                                               GUID = u.GUID
                                           }).AsParallel().ToList();
                }
                else
                {
                    IEToolsSchedulerDTO = (from u in context.ToolsSchedulers
                                           where u.Room == RoomID && u.ScheduleFor == ScheduleFor && u.CompanyID == CompanyID && (u.IsDeleted ?? false) == false && (u.IsArchived ?? false) == false
                                           select new ToolsSchedulerDTO
                                           {
                                               SchedulerName = u.SchedulerName,
                                               ID = u.ID,
                                               GUID = u.GUID
                                           }).AsParallel().ToList();
                }
            }
            return IEToolsSchedulerDTO;
        }

        public List<ToolsSchedulerMappingDTO> GetScheduleMapping(Guid? ToolGUID, Guid? AssetGUID, Guid? ToolScheduleID, int? ScheduleType)
        {
            List<ToolsSchedulerMappingDTO> lstToolsScheduler = new List<ToolsSchedulerMappingDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstToolsScheduler = (from ts in context.ToolsSchedulerMappings
                                     join am in context.AssetMasters on ts.AssetGUID equals am.GUID into ts_am_join
                                     from ts_am in ts_am_join.DefaultIfEmpty()
                                     join tm in context.ToolMasters on ts.ToolGUID equals tm.GUID into ts_tm_join
                                     from ts_tm in ts_tm_join.DefaultIfEmpty()
                                     join sm in context.ToolsSchedulers on ts.ToolSchedulerGuid equals sm.GUID into ts_sm_join
                                     from ts_sm in ts_sm_join.DefaultIfEmpty()
                                     where (ts.ToolGUID ?? Guid.Empty) == (ToolGUID ?? Guid.Empty) && (ts.AssetGUID ?? Guid.Empty) == (AssetGUID ?? Guid.Empty) && (ScheduleType == null || ts_sm.SchedulerType == ScheduleType) && (ToolScheduleID == null || ((ts.ToolSchedulerGuid ?? Guid.Empty) == (ToolScheduleID ?? Guid.Empty))) && (ts.IsDeleted ?? false) == false
                                     select new ToolsSchedulerMappingDTO
                                     {
                                         AssetGUID = ts.AssetGUID,
                                         AssetToolGUID = ts.AssetGUID != null ? ts.AssetGUID : ts.ToolGUID,
                                         CompanyID = ts.CompanyID ?? 0,
                                         CompanyName = string.Empty,
                                         Created = ts.Created,
                                         CreatedBy = ts.CreatedBy,
                                         CreatedByName = string.Empty,
                                         CreatedDate = string.Empty,
                                         GUID = ts.GUID,
                                         ID = ts.ID,
                                         IsArchived = ts.IsArchived,
                                         IsDeleted = ts.IsDeleted,
                                         Itemname = ts_am != null ? ts_am.AssetName : ts_tm.ToolName,
                                         LastUpdatedBy = ts.LastUpdatedBy,
                                         MaintenanceName = ts.MaintenanceName,
                                         TrackingMeasurement = ts.TrackingMeasurement,
                                         Room = ts.Room ?? 0,
                                         RoomName = string.Empty,
                                         SchedulerFor = ts.SchedulerFor,
                                         SchedulerForName = string.Empty,
                                         SchedulerName = ts_sm.SchedulerName,
                                         //SchedulerTypeName = ts_sm.ScheduleType,
                                         ToolGUID = ts.ToolGUID,
                                         ToolSchedulerGuid = ts.ToolSchedulerGuid,
                                         UDF1 = ts.UDF1,
                                         UDF2 = ts.UDF2,
                                         UDF3 = ts.UDF3,
                                         UDF4 = ts.UDF4,
                                         UDF5 = ts.UDF5,
                                         Updated = ts.Updated,
                                         UpdatedByName = string.Empty,
                                         UpdatedDate = string.Empty,
                                         //SchedulerType = ts.SchedulerType
                                     }).ToList();
            }
            return lstToolsScheduler;
        }

        public bool CheckScheduleMapping(byte? SchedulerFor, Guid? ToolSchedulerGuid, Guid? AssetGUID, Guid? ToolGUID, long Room, long CompanyID, bool IsDeleted, bool IsArchived)
        {
            bool ret = false;
            ToolsSchedulerMappingDTO objToolsSchedulerMappingDTO = new ToolsSchedulerMappingDTO();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (ToolSchedulerGuid != null && AssetGUID != null)
                {
                    objToolsSchedulerMappingDTO = (from u in context.ToolsSchedulerMappings
                                                   where u.CompanyID == CompanyID && u.Room == Room
                                                   && u.IsArchived == IsArchived && u.IsDeleted == IsDeleted
                                                   && u.AssetGUID == AssetGUID && u.ToolSchedulerGuid == ToolSchedulerGuid
                                                   && u.SchedulerFor == SchedulerFor
                                                   select new ToolsSchedulerMappingDTO
                                                   {
                                                       ID = u.ID,
                                                       SchedulerFor = u.SchedulerFor,
                                                       ToolSchedulerGuid = u.ToolSchedulerGuid,
                                                       ToolGUID = u.ToolGUID,
                                                       AssetGUID = u.AssetGUID,
                                                       Created = u.Created,
                                                       CreatedBy = u.CreatedBy,
                                                       Updated = u.Updated,
                                                       LastUpdatedBy = u.LastUpdatedBy,
                                                       Room = u.Room ?? 0,
                                                       IsArchived = u.IsArchived,
                                                       IsDeleted = u.IsDeleted,
                                                       GUID = u.GUID,
                                                       CompanyID = u.CompanyID ?? 0,
                                                       UDF1 = u.UDF1,
                                                       UDF2 = u.UDF2,
                                                       UDF3 = u.UDF3,
                                                       UDF4 = u.UDF4,
                                                       UDF5 = u.UDF5,
                                                       MaintenanceName = u.MaintenanceName,
                                                       TrackingMeasurement = u.TrackingMeasurement
                                                   }
                        ).FirstOrDefault();
                }
                else if (ToolSchedulerGuid != null && ToolGUID != null)
                {
                    objToolsSchedulerMappingDTO = (from u in context.ToolsSchedulerMappings
                                                   where u.CompanyID == CompanyID && u.Room == Room
                                                   && u.IsArchived == IsArchived && u.IsDeleted == IsDeleted
                                                   && u.ToolGUID == ToolGUID && u.ToolSchedulerGuid == ToolSchedulerGuid
                                                   && u.SchedulerFor == SchedulerFor
                                                   select new ToolsSchedulerMappingDTO
                                                   {
                                                       ID = u.ID,
                                                       SchedulerFor = u.SchedulerFor,
                                                       ToolSchedulerGuid = u.ToolSchedulerGuid,
                                                       ToolGUID = u.ToolGUID,
                                                       AssetGUID = u.AssetGUID,
                                                       Created = u.Created,
                                                       CreatedBy = u.CreatedBy,
                                                       Updated = u.Updated,
                                                       LastUpdatedBy = u.LastUpdatedBy,
                                                       Room = u.Room ?? 0,
                                                       IsArchived = u.IsArchived,
                                                       IsDeleted = u.IsDeleted,
                                                       GUID = u.GUID,
                                                       CompanyID = u.CompanyID ?? 0,
                                                       UDF1 = u.UDF1,
                                                       UDF2 = u.UDF2,
                                                       UDF3 = u.UDF3,
                                                       UDF4 = u.UDF4,
                                                       UDF5 = u.UDF5,
                                                       MaintenanceName = u.MaintenanceName,
                                                       TrackingMeasurement = u.TrackingMeasurement
                                                   }
                            ).FirstOrDefault();
                }
            }
            if (objToolsSchedulerMappingDTO != null && objToolsSchedulerMappingDTO.ID > 0)
            {
                ret = true;
            }
            return ret;
        }

        public ToolsSchedulerDTO GetToolSchedulerByNameAndFor(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string ScheduleName, byte? ScheduleFor)
        {
            List<int> ScheduleTypes = new List<int>();
            ToolsSchedulerDTO IEToolsSchedulerDTO = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEToolsSchedulerDTO = (from u in context.ToolsSchedulers
                                       where u.SchedulerName == (ScheduleName)
                                       && u.ScheduleFor == ScheduleFor
                                       && u.Room == RoomID
                                       && u.CompanyID == CompanyID
                                       && (u.IsDeleted ?? false) == false && (u.IsArchived ?? false) == false
                                       select new ToolsSchedulerDTO
                                       {
                                           SchedulerName = u.SchedulerName,
                                           ID = u.ID,
                                           GUID = u.GUID,
                                           ScheduleFor = u.ScheduleFor
                                       }).FirstOrDefault();
            }
            return IEToolsSchedulerDTO;
        }

        public List<ToolsSchedulerMappingDTO> GetAllSchedulesforToolAsset(Guid? ToolGUID, Guid? AssetGUID, int? ScheduleType)
        {
            List<ToolsSchedulerMappingDTO> lstToolsScheduler = new List<ToolsSchedulerMappingDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstToolsScheduler = (from ts in context.ToolsSchedulerMappings
                                     join am in context.AssetMasters on ts.AssetGUID equals am.GUID into ts_am_join
                                     from ts_am in ts_am_join.DefaultIfEmpty()
                                     join tm in context.ToolMasters on ts.ToolGUID equals tm.GUID into ts_tm_join
                                     from ts_tm in ts_tm_join.DefaultIfEmpty()
                                     join sm in context.ToolsSchedulers on ts.ToolSchedulerGuid equals sm.GUID into ts_sm_join
                                     from ts_sm in ts_sm_join.DefaultIfEmpty()
                                     where (ts.ToolGUID ?? Guid.Empty) == (ToolGUID ?? Guid.Empty) && (ts.AssetGUID ?? Guid.Empty) == (AssetGUID ?? Guid.Empty) && (ScheduleType == null || ts_sm.SchedulerType == ScheduleType) && (ts.IsDeleted ?? false) == false
                                     select new ToolsSchedulerMappingDTO
                                     {
                                         AssetGUID = ts.AssetGUID,
                                         AssetToolGUID = ts.AssetGUID != null ? ts.AssetGUID : ts.ToolGUID,
                                         CompanyID = ts.CompanyID ?? 0,
                                         CompanyName = string.Empty,
                                         Created = ts.Created,
                                         CreatedBy = ts.CreatedBy,
                                         CreatedByName = string.Empty,
                                         CreatedDate = string.Empty,
                                         GUID = ts.GUID,
                                         ID = ts.ID,
                                         IsArchived = ts.IsArchived,
                                         IsDeleted = ts.IsDeleted,
                                         Itemname = ts_am != null ? ts_am.AssetName : ts_tm.ToolName,
                                         LastUpdatedBy = ts.LastUpdatedBy,
                                         MaintenanceName = ts.MaintenanceName,
                                         TrackingMeasurement = ts.TrackingMeasurement,
                                         Room = ts.Room ?? 0,
                                         RoomName = string.Empty,
                                         SchedulerFor = ts.SchedulerFor,
                                         SchedulerForName = string.Empty,
                                         SchedulerName = ts_sm.SchedulerName,
                                         //SchedulerTypeName = ts_sm.ScheduleType,
                                         ToolGUID = ts.ToolGUID,
                                         ToolSchedulerGuid = ts.ToolSchedulerGuid,
                                         UDF1 = ts.UDF1,
                                         UDF2 = ts.UDF2,
                                         UDF3 = ts.UDF3,
                                         UDF4 = ts.UDF4,
                                         UDF5 = ts.UDF5,
                                         Updated = ts.Updated,
                                         UpdatedByName = string.Empty,
                                         UpdatedDate = string.Empty,
                                         //SchedulerType = ts.SchedulerType
                                         ToolScheduleInfo = (from u in context.ToolsSchedulers
                                                             where u.GUID == ts.ToolSchedulerGuid
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
                                                                 MainScheduleType = u.SchedulerType,
                                                                 ScheduleFor = u.ScheduleFor,
                                                                 Description = u.Description,
                                                                 TimeBasedFrequency = u.TimeBasedFrequency,
                                                                 TimeBaseUnit = u.TimeBaseUnit,
                                                                 RecurringDays = u.RecurringDays,
                                                                 CheckOuts = u.CheckOuts
                                                             }).FirstOrDefault()
                                     }).ToList();
                if (lstToolsScheduler != null && lstToolsScheduler.Count() > 0)
                {
                    lstToolsScheduler.ForEach(t =>
                    {
                        if (t.ToolScheduleInfo.SchedulerType == 1)
                        {
                            t.ToolScheduleInfo.ScheduleParams = (context.RoomSchedules.Where(e => e.AssetToolID == t.ToolScheduleInfo.GUID).Select(ss => new SchedulerDTO()
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
                            }).FirstOrDefault());
                        }


                    });


                }
            }

            return lstToolsScheduler;
        }

        public List<ToolsSchedulerDTO> ToolsScheduleExportData(string GUIDs, long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, bool IsLineItemDeleted)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@GUIDs", GUIDs ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolsSchedulerDTO>("exec [GetToolSchedulerDataForExport] @RoomID,@CompanyID,@GUIDs", params1).ToList();
            }
        }

        public List<ToolsSchedulerMappingDTO> ToolsScheduleMappingExportData(string GUIDs, long RoomID, long CompanyID, bool IsArchived, bool IsDeleted)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@GUIDs", GUIDs ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolsSchedulerMappingDTO>("exec [GetToolSchedulerMappingDataForExport] @RoomID,@CompanyID,@GUIDs", params1).ToList();
            }
        }
        
    }
}


